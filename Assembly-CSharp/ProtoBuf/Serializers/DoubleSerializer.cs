using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class DoubleSerializer : IProtoSerializer
	{
		public DoubleSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return DoubleSerializer.expectedType;
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
			return source.ReadDouble();
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteDouble((double)value, dest);
		}

		private static readonly Type expectedType = typeof(double);
	}
}
