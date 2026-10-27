using System;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class ProtoPartialMemberAttribute : ProtoMemberAttribute
	{
		public ProtoPartialMemberAttribute(int tag, string memberName) : base(tag)
		{
			if (Helpers.IsNullOrEmpty(memberName))
			{
				throw new ArgumentNullException("memberName");
			}
			this.memberName = memberName;
		}

		public string MemberName
		{
			get
			{
				return this.memberName;
			}
		}

		private readonly string memberName;
	}
}
