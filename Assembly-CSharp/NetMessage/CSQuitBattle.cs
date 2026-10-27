using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSQuitBattle")]
	[Serializable]
	public class CSQuitBattle : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("url=fix|to=battle")]
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

		[ProtoMember(1, Name = "events", DataFormat = DataFormat.Default)]
		public List<EndEvent> events
		{
			get
			{
				return this._events;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=fix|to=battle";

		private readonly List<EndEvent> _events = new List<EndEvent>();

		private IExtension extensionObject;
	}
}
