using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSGetUserData")]
	[Serializable]
	public class CSGetUserData : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("auth=1|payload=100|clr=userid|to=data")]
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

		[ProtoMember(1, IsRequired = true, Name = "account", DataFormat = DataFormat.Default)]
		public string account
		{
			get
			{
				return this._account;
			}
			set
			{
				this._account = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "app_version", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string app_version
		{
			get
			{
				return this._app_version;
			}
			set
			{
				this._app_version = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "imei_md5", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string imei_md5
		{
			get
			{
				return this._imei_md5;
			}
			set
			{
				this._imei_md5 = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "channel", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string channel
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

		[ProtoMember(5, IsRequired = false, Name = "device_model", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string device_model
		{
			get
			{
				return this._device_model;
			}
			set
			{
				this._device_model = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "os_version", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string os_version
		{
			get
			{
				return this._os_version;
			}
			set
			{
				this._os_version = value;
			}
		}

		[ProtoMember(7, IsRequired = true, Name = "token", DataFormat = DataFormat.Default)]
		public string token
		{
			get
			{
				return this._token;
			}
			set
			{
				this._token = value;
			}
		}

		[ProtoMember(8, IsRequired = false, Name = "web_test", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool web_test
		{
			get
			{
				return this._web_test;
			}
			set
			{
				this._web_test = value;
			}
		}

		[ProtoMember(9, IsRequired = false, Name = "ver_code", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long ver_code
		{
			get
			{
				return this._ver_code;
			}
			set
			{
				this._ver_code = value;
			}
		}

		[ProtoMember(10, IsRequired = false, Name = "version_name", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string version_name
		{
			get
			{
				return this._version_name;
			}
			set
			{
				this._version_name = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "auth=1|payload=100|clr=userid|to=data";

		private string _account;

		private string _app_version = string.Empty;

		private string _imei_md5 = string.Empty;

		private string _channel = string.Empty;

		private string _device_model = string.Empty;

		private string _os_version = string.Empty;

		private string _token;

		private bool _web_test;

		private long _ver_code;

		private string _version_name = string.Empty;

		private IExtension extensionObject;
	}
}
