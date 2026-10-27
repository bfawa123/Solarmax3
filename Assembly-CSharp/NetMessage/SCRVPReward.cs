using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCRVPReward")]
	[Serializable]
	public class SCRVPReward : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "pvp_reward", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int pvp_reward
		{
			get
			{
				return this._pvp_reward;
			}
			set
			{
				this._pvp_reward = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _pvp_reward;

		private IExtension extensionObject;
	}
}
