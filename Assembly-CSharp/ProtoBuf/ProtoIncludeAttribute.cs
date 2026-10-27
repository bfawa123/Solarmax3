using System;
using System.ComponentModel;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public sealed class ProtoIncludeAttribute : Attribute
	{
		public ProtoIncludeAttribute(int tag, Type knownType) : this(tag, (knownType != null) ? knownType.AssemblyQualifiedName : string.Empty)
		{
		}

		public ProtoIncludeAttribute(int tag, string knownTypeName)
		{
			if (tag <= 0)
			{
				throw new ArgumentOutOfRangeException("tag", "Tags must be positive integers");
			}
			if (Helpers.IsNullOrEmpty(knownTypeName))
			{
				throw new ArgumentNullException("knownTypeName", "Known type cannot be blank");
			}
			this.tag = tag;
			this.knownTypeName = knownTypeName;
		}

		public int Tag
		{
			get
			{
				return this.tag;
			}
		}

		public string KnownTypeName
		{
			get
			{
				return this.knownTypeName;
			}
		}

		public Type KnownType
		{
			get
			{
				return TypeModel.ResolveKnownType(this.KnownTypeName, null, null);
			}
		}

		[DefaultValue(DataFormat.Default)]
		public DataFormat DataFormat
		{
			get
			{
				return this.dataFormat;
			}
			set
			{
				this.dataFormat = value;
			}
		}

		private readonly int tag;

		private readonly string knownTypeName;

		private DataFormat dataFormat;
	}
}
