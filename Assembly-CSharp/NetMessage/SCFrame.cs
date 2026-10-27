using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCFrame")]
	[Serializable]
	public class SCFrame : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=client")]
		public string gateway
		{
			get
			{
				return this._gateway;
			}
			set
			{
				this._gateway = value;
			}
		}

		[ProtoMember(1, IsRequired = true, Name = "frameNum", DataFormat = DataFormat.TwosComplement)]
		public int frameNum
		{
			get
			{
				return this._frameNum;
			}
			set
			{
				this._frameNum = value;
			}
		}

		[ProtoMember(2, Name = "users", DataFormat = DataFormat.TwosComplement)]
		public List<int> users
		{
			get
			{
				return this._users;
			}
		}

		[ProtoMember(3, Name = "frames", DataFormat = DataFormat.Default)]
		public List<PbFrames> frames
		{
			get
			{
				return this._frames;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private int _frameNum;

		private readonly List<int> _users = new List<int>();

		private readonly List<PbFrames> _frames = new List<PbFrames>();

		private IExtension extensionObject;
	}
}
