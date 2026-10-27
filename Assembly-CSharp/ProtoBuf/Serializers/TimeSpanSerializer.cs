using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class TimeSpanSerializer : IProtoSerializer
	{
		public TimeSpanSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return TimeSpanSerializer.expectedType;
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
			return BclHelpers.ReadTimeSpan(source);
		}

		public void Write(object value, ProtoWriter dest)
		{
			BclHelpers.WriteTimeSpan((TimeSpan)value, dest);
		}

		private static readonly Type expectedType = typeof(TimeSpan);
	}
}
