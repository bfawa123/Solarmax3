using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class CharSerializer : UInt16Serializer
	{
		public CharSerializer(TypeModel model) : base(model)
		{
		}

		public override Type ExpectedType
		{
			get
			{
				return CharSerializer.expectedType;
			}
		}

		public override void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteUInt16((ushort)((char)value), dest);
		}

		public override object Read(object value, ProtoReader source)
		{
			return (char)source.ReadUInt16();
		}

		private static readonly Type expectedType = typeof(char);
	}
}
