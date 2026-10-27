using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSGenerateUrl")]
	[Serializable]
	public class CSGenerateUrl : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "objectname", DataFormat = DataFormat.Default)]
		public string objectname
		{
			get
			{
				return this._objectname;
			}
			set
			{
				this._objectname = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "method", DataFormat = DataFormat.Default)]
		public string method
		{
			get
			{
				return this._method;
			}
			set
			{
				this._method = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "contenttype", DataFormat = DataFormat.Default)]
		public string contenttype
		{
			get
			{
				return this._contenttype;
			}
			set
			{
				this._contenttype = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "file", DataFormat = DataFormat.Default)]
		public string file
		{
			get
			{
				return this._file;
			}
			set
			{
				this._file = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "eventId", DataFormat = DataFormat.TwosComplement)]
		public int eventId
		{
			get
			{
				return this._eventId;
			}
			set
			{
				this._eventId = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _objectname;

		private string _method;

		private string _contenttype;

		private string _file;

		private int _eventId;

		private IExtension extensionObject;
	}
}
