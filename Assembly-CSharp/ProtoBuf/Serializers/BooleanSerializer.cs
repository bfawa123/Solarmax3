using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class BooleanSerializer : IProtoSerializer
	{
		public BooleanSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return BooleanSerializer.expectedType;
			}
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteBoolean((bool)value, dest);
		}

		public object Read(object value, ProtoReader source)
		{
			return source.ReadBoolean();
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

		private static readonly Type expectedType = typeof(bool);
	}
}
