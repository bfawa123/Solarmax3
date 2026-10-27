using System;

namespace ProtoBuf.Serializers
{
	internal abstract class ProtoDecoratorBase : IProtoSerializer
	{
		protected ProtoDecoratorBase(IProtoSerializer tail)
		{
			this.Tail = tail;
		}

		public abstract Type ExpectedType { get; }

		public abstract bool ReturnsValue { get; }

		public abstract bool RequiresOldValue { get; }

		public abstract void Write(object value, ProtoWriter dest);

		public abstract object Read(object value, ProtoReader source);

		protected readonly IProtoSerializer Tail;
	}
}
