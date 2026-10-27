using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCAchievementLoad")]
	[Serializable]
	public class SCAchievementLoad : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
		public ErrCode code
		{
			get
			{
				return this._code;
			}
			set
			{
				this._code = value;
			}
		}

		[ProtoMember(2, Name = "achieves", DataFormat = DataFormat.Default)]
		public List<PbAchievement> achieves
		{
			get
			{
				return this._achieves;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private readonly List<PbAchievement> _achieves = new List<PbAchievement>();

		private IExtension extensionObject;
	}
}
