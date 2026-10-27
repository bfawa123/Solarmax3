using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSPveRankReportLoad")]
	[Serializable]
	public class CSPveRankReportLoad : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "levelId", DataFormat = DataFormat.Default)]
		public string levelId
		{
			get
			{
				return this._levelId;
			}
			set
			{
				this._levelId = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _levelId;

		private IExtension extensionObject;
	}
}
