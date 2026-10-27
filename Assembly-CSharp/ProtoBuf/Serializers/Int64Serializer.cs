using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class Int64Serializer : IProtoSerializer
	{
		public Int64Serializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return Int64Serializer.expectedType;
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
			return source.ReadInt64();
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteInt64((long)value, dest);
		}

		private static readonly Type expectedType = typeof(long);
	}
}
