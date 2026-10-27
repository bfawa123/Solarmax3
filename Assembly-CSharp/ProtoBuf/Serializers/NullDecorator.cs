using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class NullDecorator : ProtoDecoratorBase
	{
		public NullDecorator(TypeModel model, IProtoSerializer tail) : base(tail)
		{
			if (!tail.ReturnsValue)
			{
				throw new NotSupportedException("NullDecorator only supports implementations that return values");
			}
			if (Helpers.IsValueType(tail.ExpectedType))
			{
				this.expectedType = model.MapType(typeof(Nullable<>)).MakeGenericType(new Type[]
				{
					tail.ExpectedType
				});
			}
			else
			{
				this.expectedType = tail.ExpectedType;
			}
		}

		public override Type ExpectedType
		{
			get
			{
				return this.expectedType;
			}
		}

		public override bool ReturnsValue
		{
			get
			{
				return true;
			}
		}

		public override bool RequiresOldValue
		{
			get
			{
				return true;
			}
		}

		public override object Read(object value, ProtoReader source)
		{
			SubItemToken token = ProtoReader.StartSubItem(source);
			int num;
			while ((num = source.ReadFieldHeader()) > 0)
			{
				if (num == 1)
				{
					value = this.Tail.Read(value, source);
				}
				else
				{
					source.SkipField();
				}
			}
			ProtoReader.EndSubItem(token, source);
			return value;
		}

		public override void Write(object value, ProtoWriter dest)
		{
			SubItemToken token = ProtoWriter.StartSubItem(null, dest);
			if (value != null)
			{
				this.Tail.Write(value, dest);
			}
			ProtoWriter.EndSubItem(token, dest);
		}

		private readonly Type expectedType;

		public const int Tag = 1;
	}
}
