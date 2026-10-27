using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCPveRankReportLoad")]
	[Serializable]
	public class SCPveRankReportLoad : IExtensible
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

		[ProtoMember(3, Name = "report", DataFormat = DataFormat.Default)]
		public List<PveRankReport> report
		{
			get
			{
				return this._report;
			}
		}

		[ProtoMember(4, Name = "report_friend", DataFormat = DataFormat.Default)]
		public List<PveRankReport> report_friend
		{
			get
			{
				return this._report_friend;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _levelId;

		private readonly List<PveRankReport> _report = new List<PveRankReport>();

		private readonly List<PveRankReport> _report_friend = new List<PveRankReport>();

		private IExtension extensionObject;
	}
}
