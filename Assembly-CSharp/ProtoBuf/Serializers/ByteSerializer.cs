using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class ByteSerializer : IProtoSerializer
	{
		public ByteSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return ByteSerializer.expectedType;
			}
		}

		bool IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		bool IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteByte((byte)value, dest);
		}

		public object Read(object value, ProtoReader source)
		{
			return source.ReadByte();
		}

		private static readonly Type expectedType = typeof(byte);
	}
}
