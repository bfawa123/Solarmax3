using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal class SystemTypeSerializer : IProtoSerializer
	{
		public SystemTypeSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return SystemTypeSerializer.expectedType;
			}
		}

		void IProtoSerializer.Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteType((Type)value, dest);
		}

		object IProtoSerializer.Read(object value, ProtoReader source)
		{
			return source.ReadType();
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

		private static readonly Type expectedType = typeof(Type);
	}
}
