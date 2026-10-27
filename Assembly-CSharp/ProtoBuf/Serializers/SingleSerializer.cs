using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class SingleSerializer : IProtoSerializer
	{
		public SingleSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return SingleSerializer.expectedType;
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
			return source.ReadSingle();
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteSingle((float)value, dest);
		}

		private static readonly Type expectedType = typeof(float);
	}
}
