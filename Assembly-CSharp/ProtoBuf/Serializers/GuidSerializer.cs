using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class GuidSerializer : IProtoSerializer
	{
		public GuidSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return GuidSerializer.expectedType;
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
			BclHelpers.WriteGuid((Guid)value, dest);
		}

		public object Read(object value, ProtoReader source)
		{
			return BclHelpers.ReadGuid(source);
		}

		private static readonly Type expectedType = typeof(Guid);
	}
}
