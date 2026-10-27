using System;

namespace ProtoBuf.Serializers
{
	internal interface IProtoSerializer
	{
		Type ExpectedType { get; }

		void Write(object value, ProtoWriter dest);

		object Read(object value, ProtoReader source);

		bool RequiresOldValue { get; }

		bool ReturnsValue { get; }
	}
}
