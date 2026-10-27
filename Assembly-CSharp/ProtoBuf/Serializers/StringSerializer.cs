using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class StringSerializer : IProtoSerializer
	{
		public StringSerializer(TypeModel model)
		{
		}

		public Type ExpectedType
		{
			get
			{
				return StringSerializer.expectedType;
			}
		}

		public void Write(object value, ProtoWriter dest)
		{
			ProtoWriter.WriteString((string)value, dest);
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
			return source.ReadString();
		}

		private static readonly Type expectedType = typeof(string);
	}
}
