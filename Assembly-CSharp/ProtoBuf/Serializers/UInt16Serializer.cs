using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal class UInt16Serializer : IProtoSerializer
	{
		public UInt16Serializer(TypeModel model)
		{
		}

		public virtual Type ExpectedType
		{
			get
			{
				return UInt16Serializer.expectedType;
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

		public virtual object Read(object value, ProtoReader source)
		{
			return source.ReadUInt16();
		}

		public virtual void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt16((ushort)value, dest);
		}

		private static readonly Type expectedType = typeof(ushort);
	}
}
