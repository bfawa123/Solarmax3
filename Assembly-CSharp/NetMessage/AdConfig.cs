using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "AdConfig")]
	[Serializable]
	public class AdConfig : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "channel", DataFormat = DataFormat.TwosComplement)]
		public AdChannel channel
		{
			get
			{
				return this._channel;
			}
			set
			{
				this._channel = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "app_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string app_id
		{
			get
			{
				return this._app_id;
			}
			set
			{
				this._app_id = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "horizontal_video_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string horizontal_video_id
		{
			get
			{
				return this._horizontal_video_id;
			}
			set
			{
				this._horizontal_video_id = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private AdChannel _channel;

		private string _app_id = string.Empty;

		private string _horizontal_video_id = string.Empty;

		private IExtension extensionObject;
	}
}
