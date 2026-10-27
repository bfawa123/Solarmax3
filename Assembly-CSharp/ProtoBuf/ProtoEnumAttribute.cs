using System;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class ProtoEnumAttribute : Attribute
	{
		public int Value
		{
			get
			{
				return this.enumValue;
			}
			set
			{
				this.enumValue = value;
				this.hasValue = true;
			}
		}

		public bool HasValue()
		{
			return this.hasValue;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		private bool hasValue;

		private int enumValue;

		private string name;
	}
}
