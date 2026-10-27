using System;

namespace ProtoBuf
{
	[ProtoContract]
	public class FriendData
	{
		[ProtoMember(1)]
		public int DbID;

		[ProtoMember(2)]
		public int OtherRoleID;

		[ProtoMember(3)]
		public string OtherRoleName;

		[ProtoMember(4)]
		public int OtherLevel;

		[ProtoMember(5)]
		public int Occupation;

		[ProtoMember(6)]
		public int OnlineState;

		[ProtoMember(7)]
		public string Position;

		[ProtoMember(8)]
		public int FriendType;
	}
}
