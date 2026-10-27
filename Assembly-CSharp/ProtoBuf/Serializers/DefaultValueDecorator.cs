using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class DefaultValueDecorator : ProtoDecoratorBase
	{
		public DefaultValueDecorator(TypeModel model, object defaultValue, IProtoSerializer tail) : base(tail)
		{
			if (defaultValue == null)
			{
				throw new ArgumentNullException("defaultValue");
			}
			Type type = model.MapType(defaultValue.GetType());
			if (type != tail.ExpectedType)
			{
				throw new ArgumentException("Default value is of incorrect type", "defaultValue");
			}
			this.defaultValue = defaultValue;
		}

		public override Type ExpectedType
		{
			get
			{
				return this.Tail.ExpectedType;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return this.Tail.RequiresOldValue;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return this.Tail.ReturnsValue;
			}
		}

		public override void Write(object value, ProtoWriter dest)
		{
			if (!object.Equals(value, this.defaultValue))
			{
				this.Tail.Write(value, dest);
			}
		}

		public override object Read(object value, ProtoReader source)
		{
			return this.Tail.Read(value, source);
		}

		private readonly object defaultValue;
	}
}
