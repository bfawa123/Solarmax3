using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class UInt64Serializer : IProtoSerializer
	{
		public UInt64Serializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return UInt64Serializer.expectedType;
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

		public object Read(object value, ProtoReader source)
		{
			return source.ReadUInt64();
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt64((ulong)value, dest);
		}

		private static readonly Type expectedType = typeof(ulong);
	}
}
