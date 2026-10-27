using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class DecimalSerializer : IProtoSerializer
	{
		public DecimalSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return DecimalSerializer.expectedType;
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
			return BclHelpers.ReadDecimal(source);
		}

		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteDecimal((decimal)value, dest);
		}

		private static readonly Type expectedType = typeof(decimal);
	}
}
