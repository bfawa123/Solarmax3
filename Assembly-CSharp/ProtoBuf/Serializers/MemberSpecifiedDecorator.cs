using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class MemberSpecifiedDecorator : ProtoDecoratorBase
	{
		public MemberSpecifiedDecorator(MethodInfo getSpecified, MethodInfo setSpecified, IProtoSerializer tail) : base(tail)
		{
			if (getSpecified == null && setSpecified == null)
			{
				throw new InvalidOperationException();
			}
			this.getSpecified = getSpecified;
			this.setSpecified = setSpecified;
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
			if (this.getSpecified == null || (bool)this.getSpecified.Invoke(value, null))
			{
				this.Tail.Write(value, dest);
			}
		}

		public override object Read(object value, ProtoReader source)
		{
			object result = this.Tail.Read(value, source);
			if (this.setSpecified != null)
			{
				this.setSpecified.Invoke(value, new object[]
				{
					true
				});
			}
			return result;
		}

		private readonly MethodInfo getSpecified;

		private readonly MethodInfo setSpecified;
	}
}
