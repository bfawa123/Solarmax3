using System;
using System.IO;
using System.Text;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	public sealed class ProtoWriter : IDisposable
	{
		public ProtoWriter(Stream dest, TypeModel model, SerializationContext context)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (!dest.CanWrite)
			{
				throw new ArgumentException("Cannot write to stream", "dest");
			}
			this.dest = dest;
			this.ioBuffer = BufferPool.GetBuffer();
			this.model = model;
			this.wireType = WireType.None;
			if (context == null)
			{
				context = SerializationContext.Default;
			}
			else
			{
				context.Freeze();
			}
			this.context = context;
		}

		public ProtoWriter(Stream dest)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (!dest.CanWrite)
			{
				throw new ArgumentException("Cannot write to stream", "dest");
			}
			this.dest = dest;
			this.ioBuffer = BufferPool.GetBuffer();
			this.wireType = WireType.None;
		}

		public static void WriteObject(object value, int key, ProtoWriter writer)
		{
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			SubItemToken token = ProtoWriter.StartSubItem(value, writer);
			if (key >= 0)
			{
				writer.model.Serialize(key, value, writer);
			}
			else if (writer.model == null || !writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
			{
				TypeModel.ThrowUnexpectedType(value.GetType());
			}
			ProtoWriter.EndSubItem(token, writer);
		}

		public static void WriteRecursionSafeObject(object value, int key, ProtoWriter writer)
		{
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			SubItemToken token = ProtoWriter.StartSubItem(null, writer);
			writer.model.Serialize(key, value, writer);
			ProtoWriter.EndSubItem(token, writer);
		}

		internal static void WriteObject(object value, int key, ProtoWriter writer, PrefixStyle style, int fieldNumber)
		{
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			if (writer.wireType != WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			switch (style)
			{
			case PrefixStyle.Base128:
				writer.wireType = WireType.String;
				writer.fieldNumber = fieldNumber;
				if (fieldNumber > 0)
				{
					ProtoWriter.WriteHeaderCore(fieldNumber, WireType.String, writer);
				}
				break;
			case PrefixStyle.Fixed32:
			case PrefixStyle.Fixed32BigEndian:
				writer.fieldNumber = 0;
				writer.wireType = WireType.Fixed32;
				break;
			default:
				throw new ArgumentOutOfRangeException("style");
			}
			SubItemToken token = ProtoWriter.StartSubItem(value, writer, true);
			if (key < 0)
			{
				if (!writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
				{
					TypeModel.ThrowUnexpectedType(value.GetType());
				}
			}
			else
			{
				writer.model.Serialize(key, value, writer);
			}
			ProtoWriter.EndSubItem(token, writer, style);
		}

		internal int GetTypeKey(ref Type type)
		{
			return this.model.GetKey(ref type);
		}

		internal NetObjectCache NetCache
		{
			get
			{
				return this.netCache;
			}
		}

		internal WireType WireType
		{
			get
			{
				return this.wireType;
			}
		}

		public static void WriteFieldHeader(int fieldNumber, WireType wireType, ProtoWriter writer)
		{
			if (writer.wireType != WireType.None)
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					"Cannot write a ",
					wireType,
					" header until the ",
					writer.wireType,
					" data has been written"
				}));
			}
			if (fieldNumber < 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (writer.packedFieldNumber == 0)
			{
				writer.fieldNumber = fieldNumber;
				writer.wireType = wireType;
				ProtoWriter.WriteHeaderCore(fieldNumber, wireType, writer);
			}
			else
			{
				if (writer.packedFieldNumber != fieldNumber)
				{
					throw new InvalidOperationException(string.Concat(new object[]
					{
						"Field mismatch during packed encoding; expected ",
						writer.packedFieldNumber,
						" but received ",
						fieldNumber
					}));
				}
				switch (wireType)
				{
				case WireType.Variant:
				case WireType.Fixed64:
				case WireType.Fixed32:
					break;
				default:
					if (wireType != WireType.SignedVariant)
					{
						throw new InvalidOperationException("Wire-type cannot be encoded as packed: " + wireType);
					}
					break;
				}
				writer.fieldNumber = fieldNumber;
				writer.wireType = wireType;
			}
		}

		internal static void WriteHeaderCore(int fieldNumber, WireType wireType, ProtoWriter writer)
		{
			uint value = (uint)(fieldNumber << 3 | (int)(wireType & (WireType)7));
			ProtoWriter.WriteUInt32Variant(value, writer);
		}

		public static void WriteBytes(byte[] data, ProtoWriter writer)
		{
			ProtoWriter.WriteBytes(data, 0, data.Length, writer);
		}

		public static void WriteBytes(byte[] data, int offset, int length, ProtoWriter writer)
		{
			if (data == null)
			{
				throw new ArgumentNullException("blob");
			}
			WireType wireType = writer.wireType;
			if (wireType != WireType.Fixed32)
			{
				if (wireType != WireType.Fixed64)
				{
					if (wireType != WireType.String)
					{
						throw ProtoWriter.CreateException(writer);
					}
					ProtoWriter.WriteUInt32Variant((uint)length, writer);
					writer.wireType = WireType.None;
					if (length == 0)
					{
						return;
					}
					if (writer.flushLock == 0 && length > writer.ioBuffer.Length)
					{
						ProtoWriter.Flush(writer);
						writer.dest.Write(data, offset, length);
						writer.position += length;
						return;
					}
				}
				else if (length != 8)
				{
					throw new ArgumentException("length");
				}
			}
			else if (length != 4)
			{
				throw new ArgumentException("length");
			}
			ProtoWriter.DemandSpace(length, writer);
			Helpers.BlockCopy(data, offset, writer.ioBuffer, writer.ioIndex, length);
			ProtoWriter.IncrementedAndReset(length, writer);
		}

		private static void CopyRawFromStream(Stream source, ProtoWriter writer)
		{
			byte[] array = writer.ioBuffer;
			int num = array.Length - writer.ioIndex;
			int num2 = 1;
			while (num > 0 && (num2 = source.Read(array, writer.ioIndex, num)) > 0)
			{
				writer.ioIndex += num2;
				writer.position += num2;
				num -= num2;
			}
			if (num2 <= 0)
			{
				return;
			}
			if (writer.flushLock == 0)
			{
				ProtoWriter.Flush(writer);
				while ((num2 = source.Read(array, 0, array.Length)) > 0)
				{
					writer.dest.Write(array, 0, num2);
					writer.position += num2;
				}
			}
			else
			{
				for (;;)
				{
					ProtoWriter.DemandSpace(128, writer);
					if ((num2 = source.Read(writer.ioBuffer, writer.ioIndex, writer.ioBuffer.Length - writer.ioIndex)) <= 0)
					{
						break;
					}
					writer.position += num2;
					writer.ioIndex += num2;
				}
			}
		}

		private static void IncrementedAndReset(int length, ProtoWriter writer)
		{
			writer.ioIndex += length;
			writer.position += length;
			writer.wireType = WireType.None;
		}

		public static SubItemToken StartSubItem(object instance, ProtoWriter writer)
		{
			return ProtoWriter.StartSubItem(instance, writer, false);
		}

		private void CheckRecursionStackAndPush(object instance)
		{
			int num;
			if (this.recursionStack == null)
			{
				this.recursionStack = new MutableList();
			}
			else if (instance != null && (num = this.recursionStack.IndexOfReference(instance)) >= 0)
			{
				throw new ProtoException("Possible recursion detected (offset: " + (this.recursionStack.Count - num).ToString() + " level(s)): " + instance.ToString());
			}
			this.recursionStack.Add(instance);
		}

		private void PopRecursionStack()
		{
			this.recursionStack.RemoveLast();
		}

		private static SubItemToken StartSubItem(object instance, ProtoWriter writer, bool allowFixed)
		{
			if (++writer.depth > 25)
			{
				writer.CheckRecursionStackAndPush(instance);
			}
			if (writer.packedFieldNumber != 0)
			{
				throw new InvalidOperationException("Cannot begin a sub-item while performing packed encoding");
			}
			switch (writer.wireType)
			{
			case WireType.String:
				writer.wireType = WireType.None;
				ProtoWriter.DemandSpace(32, writer);
				writer.flushLock++;
				writer.position++;
				return new SubItemToken(writer.ioIndex++);
			case WireType.StartGroup:
				writer.wireType = WireType.None;
				return new SubItemToken(-writer.fieldNumber);
			case WireType.Fixed32:
			{
				if (!allowFixed)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.DemandSpace(32, writer);
				writer.flushLock++;
				SubItemToken result = new SubItemToken(writer.ioIndex);
				ProtoWriter.IncrementedAndReset(4, writer);
				return result;
			}
			}
			throw ProtoWriter.CreateException(writer);
		}

		public static void EndSubItem(SubItemToken token, ProtoWriter writer)
		{
			ProtoWriter.EndSubItem(token, writer, PrefixStyle.Base128);
		}

		private static void EndSubItem(SubItemToken token, ProtoWriter writer, PrefixStyle style)
		{
			if (writer.wireType != WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			int value = token.value;
			if (writer.depth <= 0)
			{
				throw ProtoWriter.CreateException(writer);
			}
			if (writer.depth-- > 25)
			{
				writer.PopRecursionStack();
			}
			writer.packedFieldNumber = 0;
			if (value < 0)
			{
				ProtoWriter.WriteHeaderCore(-value, WireType.EndGroup, writer);
				writer.wireType = WireType.None;
				return;
			}
			switch (style)
			{
			case PrefixStyle.Base128:
			{
				int num = writer.ioIndex - value - 1;
				int num2 = 0;
				uint num3 = (uint)num;
				while ((num3 >>= 7) != 0U)
				{
					num2++;
				}
				if (num2 == 0)
				{
					writer.ioBuffer[value] = (byte)(num & 127);
				}
				else
				{
					ProtoWriter.DemandSpace(num2, writer);
					byte[] array = writer.ioBuffer;
					Helpers.BlockCopy(array, value + 1, array, value + 1 + num2, num);
					num3 = (uint)num;
					do
					{
						array[value++] = (byte)((num3 & 127U) | 128U);
					}
					while ((num3 >>= 7) != 0U);
					array[value - 1] = (byte)((int)array[value - 1] & -129);
					writer.position += num2;
					writer.ioIndex += num2;
				}
				break;
			}
			case PrefixStyle.Fixed32:
			{
				int num = writer.ioIndex - value - 4;
				ProtoWriter.WriteInt32ToBuffer(num, writer.ioBuffer, value);
				break;
			}
			case PrefixStyle.Fixed32BigEndian:
			{
				int num = writer.ioIndex - value - 4;
				byte[] array2 = writer.ioBuffer;
				ProtoWriter.WriteInt32ToBuffer(num, array2, value);
				byte b = array2[value];
				array2[value] = array2[value + 3];
				array2[value + 3] = b;
				b = array2[value + 1];
				array2[value + 1] = array2[value + 2];
				array2[value + 2] = b;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("style");
			}
			writer.flushLock--;
		}

		public SerializationContext Context
		{
			get
			{
				return this.context;
			}
		}

		void IDisposable.Dispose()
		{
			this.Dispose();
		}

		private void Dispose()
		{
			if (this.dest != null)
			{
				ProtoWriter.Flush(this);
				this.dest = null;
			}
			this.model = null;
			BufferPool.ReleaseBufferToPool(ref this.ioBuffer);
		}

		internal static int GetPosition(ProtoWriter writer)
		{
			return writer.position;
		}

		private static void DemandSpace(int required, ProtoWriter writer)
		{
			if (writer.ioBuffer.Length - writer.ioIndex < required)
			{
				if (writer.flushLock == 0)
				{
					ProtoWriter.Flush(writer);
					if (writer.ioBuffer.Length - writer.ioIndex >= required)
					{
						return;
					}
				}
				BufferPool.ResizeAndFlushLeft(ref writer.ioBuffer, required + writer.ioIndex, 0, writer.ioIndex);
			}
		}

		public void Close()
		{
			if (this.depth != 0 || this.flushLock != 0)
			{
				throw new InvalidOperationException("Unable to close stream in an incomplete state");
			}
			this.Dispose();
		}

		internal void CheckDepthFlushlock()
		{
			if (this.depth != 0 || this.flushLock != 0)
			{
				throw new InvalidOperationException("The writer is in an incomplete state");
			}
		}

		public TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		internal static void Flush(ProtoWriter writer)
		{
			if (writer.flushLock == 0 && writer.ioIndex != 0)
			{
				writer.dest.Write(writer.ioBuffer, 0, writer.ioIndex);
				writer.ioIndex = 0;
			}
		}

		private static void WriteUInt32Variant(uint value, ProtoWriter writer)
		{
			ProtoWriter.DemandSpace(5, writer);
			int num = 0;
			do
			{
				writer.ioBuffer[writer.ioIndex++] = (byte)((value & 127U) | 128U);
				num++;
			}
			while ((value >>= 7) != 0U);
			byte[] array = writer.ioBuffer;
			int num2 = writer.ioIndex - 1;
			array[num2] &= 127;
			writer.position += num;
		}

		internal static uint Zig(int value)
		{
			return (uint)(value << 1 ^ value >> 31);
		}

		internal static ulong Zig(long value)
		{
			return (ulong)(value << 1 ^ value >> 63);
		}

		private static void WriteUInt64Variant(ulong value, ProtoWriter writer)
		{
			ProtoWriter.DemandSpace(10, writer);
			int num = 0;
			do
			{
				writer.ioBuffer[writer.ioIndex++] = (byte)((value & 127UL) | 128UL);
				num++;
			}
			while ((value >>= 7) != 0UL);
			byte[] array = writer.ioBuffer;
			int num2 = writer.ioIndex - 1;
			array[num2] &= 127;
			writer.position += num;
		}

		public static void WriteString(string value, ProtoWriter writer)
		{
			if (writer.wireType != WireType.String)
			{
				throw ProtoWriter.CreateException(writer);
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0)
			{
				ProtoWriter.WriteUInt32Variant(0U, writer);
				writer.wireType = WireType.None;
				return;
			}
			int byteCount = ProtoWriter.encoding.GetByteCount(value);
			ProtoWriter.WriteUInt32Variant((uint)byteCount, writer);
			ProtoWriter.DemandSpace(byteCount, writer);
			int bytes = ProtoWriter.encoding.GetBytes(value, 0, value.Length, writer.ioBuffer, writer.ioIndex);
			ProtoWriter.IncrementedAndReset(bytes, writer);
		}

		public static void WriteUInt64(ulong value, ProtoWriter writer)
		{
			switch (writer.wireType)
			{
			case WireType.Variant:
				ProtoWriter.WriteUInt64Variant(value, writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed64:
				ProtoWriter.WriteInt64((long)value, writer);
				return;
			case WireType.Fixed32:
				ProtoWriter.WriteUInt32(checked((uint)value), writer);
				return;
			}
			throw ProtoWriter.CreateException(writer);
		}

		public static void WriteInt64(long value, ProtoWriter writer)
		{
			WireType wireType = writer.wireType;
			switch (wireType)
			{
			case WireType.Variant:
				if (value >= 0L)
				{
					ProtoWriter.WriteUInt64Variant((ulong)value, writer);
					writer.wireType = WireType.None;
				}
				else
				{
					ProtoWriter.DemandSpace(10, writer);
					byte[] array = writer.ioBuffer;
					int num = writer.ioIndex;
					array[num] = (byte)(value | 128L);
					array[num + 1] = (byte)((int)(value >> 7) | 128);
					array[num + 2] = (byte)((int)(value >> 14) | 128);
					array[num + 3] = (byte)((int)(value >> 21) | 128);
					array[num + 4] = (byte)((int)(value >> 28) | 128);
					array[num + 5] = (byte)((int)(value >> 35) | 128);
					array[num + 6] = (byte)((int)(value >> 42) | 128);
					array[num + 7] = (byte)((int)(value >> 49) | 128);
					array[num + 8] = (byte)((int)(value >> 56) | 128);
					array[num + 9] = 1;
					ProtoWriter.IncrementedAndReset(10, writer);
				}
				return;
			case WireType.Fixed64:
			{
				ProtoWriter.DemandSpace(8, writer);
				byte[] array = writer.ioBuffer;
				int num = writer.ioIndex;
				array[num] = (byte)value;
				array[num + 1] = (byte)(value >> 8);
				array[num + 2] = (byte)(value >> 16);
				array[num + 3] = (byte)(value >> 24);
				array[num + 4] = (byte)(value >> 32);
				array[num + 5] = (byte)(value >> 40);
				array[num + 6] = (byte)(value >> 48);
				array[num + 7] = (byte)(value >> 56);
				ProtoWriter.IncrementedAndReset(8, writer);
				return;
			}
			default:
				if (wireType != WireType.SignedVariant)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.WriteUInt64Variant(ProtoWriter.Zig(value), writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed32:
				ProtoWriter.WriteInt32(checked((int)value), writer);
				return;
			}
		}

		public static void WriteUInt32(uint value, ProtoWriter writer)
		{
			switch (writer.wireType)
			{
			case WireType.Variant:
				ProtoWriter.WriteUInt32Variant(value, writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed64:
				ProtoWriter.WriteInt64((long)value, writer);
				return;
			case WireType.Fixed32:
				ProtoWriter.WriteInt32((int)value, writer);
				return;
			}
			throw ProtoWriter.CreateException(writer);
		}

		public static void WriteInt16(short value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32((int)value, writer);
		}

		public static void WriteUInt16(ushort value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((uint)value, writer);
		}

		public static void WriteByte(byte value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((uint)value, writer);
		}

		public static void WriteSByte(sbyte value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32((int)value, writer);
		}

		private static void WriteInt32ToBuffer(int value, byte[] buffer, int index)
		{
			buffer[index] = (byte)value;
			buffer[index + 1] = (byte)(value >> 8);
			buffer[index + 2] = (byte)(value >> 16);
			buffer[index + 3] = (byte)(value >> 24);
		}

		public static void WriteInt32(int value, ProtoWriter writer)
		{
			WireType wireType = writer.wireType;
			switch (wireType)
			{
			case WireType.Variant:
				if (value >= 0)
				{
					ProtoWriter.WriteUInt32Variant((uint)value, writer);
					writer.wireType = WireType.None;
				}
				else
				{
					ProtoWriter.DemandSpace(10, writer);
					byte[] array = writer.ioBuffer;
					int num = writer.ioIndex;
					array[num] = (byte)(value | 128);
					array[num + 1] = (byte)(value >> 7 | 128);
					array[num + 2] = (byte)(value >> 14 | 128);
					array[num + 3] = (byte)(value >> 21 | 128);
					array[num + 4] = (byte)(value >> 28 | 128);
					array[num + 5] = (array[num + 6] = (array[num + 7] = (array[num + 8] = byte.MaxValue)));
					array[num + 9] = 1;
					ProtoWriter.IncrementedAndReset(10, writer);
				}
				return;
			case WireType.Fixed64:
			{
				ProtoWriter.DemandSpace(8, writer);
				byte[] array = writer.ioBuffer;
				int num = writer.ioIndex;
				array[num] = (byte)value;
				array[num + 1] = (byte)(value >> 8);
				array[num + 2] = (byte)(value >> 16);
				array[num + 3] = (byte)(value >> 24);
				array[num + 4] = (array[num + 5] = (array[num + 6] = (array[num + 7] = 0)));
				ProtoWriter.IncrementedAndReset(8, writer);
				return;
			}
			default:
				if (wireType != WireType.SignedVariant)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.WriteUInt32Variant(ProtoWriter.Zig(value), writer);
				writer.wireType = WireType.None;
				return;
			case WireType.Fixed32:
				ProtoWriter.DemandSpace(4, writer);
				ProtoWriter.WriteInt32ToBuffer(value, writer.ioBuffer, writer.ioIndex);
				ProtoWriter.IncrementedAndReset(4, writer);
				return;
			}
		}

		public static void WriteDouble(double value, ProtoWriter writer)
		{
			WireType wireType = writer.wireType;
			if (wireType != WireType.Fixed32)
			{
				if (wireType != WireType.Fixed64)
				{
					throw ProtoWriter.CreateException(writer);
				}
				ProtoWriter.WriteInt64((long)value, writer);
				return;
			}
			else
			{
				float value2 = (float)value;
				if (Helpers.IsInfinity(value2) && !Helpers.IsInfinity(value))
				{
					throw new OverflowException();
				}
				ProtoWriter.WriteSingle(value2, writer);
				return;
			}
		}

		public static void WriteSingle(float value, ProtoWriter writer)
		{
			WireType wireType = writer.wireType;
			if (wireType == WireType.Fixed32)
			{
				ProtoWriter.WriteInt32((int)value, writer);
				return;
			}
			if (wireType != WireType.Fixed64)
			{
				throw ProtoWriter.CreateException(writer);
			}
			ProtoWriter.WriteDouble((double)value, writer);
		}

		public static void ThrowEnumException(ProtoWriter writer, object enumValue)
		{
			string str = (enumValue != null) ? (enumValue.GetType().FullName + "." + enumValue.ToString()) : "<null>";
			throw new ProtoException("No wire-value is mapped to the enum " + str);
		}

		internal static Exception CreateException(ProtoWriter writer)
		{
			return new ProtoException(string.Concat(new object[]
			{
				"Invalid serialization operation with wire-type ",
				writer.wireType,
				" at position ",
				writer.position
			}));
		}

		public static void WriteBoolean(bool value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((!value) ? 0U : 1U, writer);
		}

		public static void AppendExtensionData(IExtensible instance, ProtoWriter writer)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (writer.wireType != WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			IExtension extensionObject = instance.GetExtensionObject(false);
			if (extensionObject != null)
			{
				Stream stream = extensionObject.BeginQuery();
				try
				{
					ProtoWriter.CopyRawFromStream(stream, writer);
				}
				finally
				{
					extensionObject.EndQuery(stream);
				}
			}
		}

		public static void SetPackedField(int fieldNumber, ProtoWriter writer)
		{
			if (fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			writer.packedFieldNumber = fieldNumber;
		}

		internal string SerializeType(Type type)
		{
			return TypeModel.SerializeType(this.model, type);
		}

		public void SetRootObject(object value)
		{
			this.NetCache.SetKeyedObject(0, value);
		}

		public static void WriteType(Type value, ProtoWriter writer)
		{
			ProtoWriter.WriteString(writer.SerializeType(value), writer);
		}

		private Stream dest;

		private TypeModel model;

		private readonly NetObjectCache netCache = new NetObjectCache();

		private int fieldNumber;

		private int flushLock;

		private WireType wireType;

		private int depth;

		private const int RecursionCheckDepth = 25;

		private MutableList recursionStack;

		private readonly SerializationContext context;

		private byte[] ioBuffer;

		private int ioIndex;

		private int position;

		private static readonly UTF8Encoding encoding = new UTF8Encoding();

		private int packedFieldNumber;
	}
}
