using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class SByteSerializer : IProtoSerializer
	{
		public SByteSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return SByteSerializer.expectedType;
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
			return source.ReadSByte();
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteSByte((sbyte)value, dest);
		}

		private static readonly Type expectedType = typeof(sbyte);
	}
}
