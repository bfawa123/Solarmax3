using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal interface IProtoTypeSerializer : IProtoSerializer
	{
		bool HasCallbacks(TypeModel.CallbackType callbackType);

		void Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context);
	}
}
