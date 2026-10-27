using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class UriDecorator : ProtoDecoratorBase
	{
		public UriDecorator(TypeModel model, IProtoSerializer tail) : base(tail)
		{
		}

		public override Type ExpectedType
		{
			get
			{
				return UriDecorator.expectedType;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return true;
			}
		}

		public override void Write(object value, ProtoWriter dest)
		{
			this.Tail.Write(((Uri)value).AbsoluteUri, dest);
		}

		public override object Read(object value, ProtoReader source)
		{
			string text = (string)this.Tail.Read(null, source);
			return (text.Length != 0) ? new Uri(text) : null;
		}

		private static readonly Type expectedType = typeof(Uri);
	}
}
