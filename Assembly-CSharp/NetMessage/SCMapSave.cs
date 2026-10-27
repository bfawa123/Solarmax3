using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMapSave")]
	[Serializable]
	public class SCMapSave : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "mapid", DataFormat = DataFormat.TwosComplement)]
		public int mapid
		{
			get
			{
				return this._mapid;
			}
			set
			{
				this._mapid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "err", DataFormat = DataFormat.TwosComplement)]
		public ErrCode err
		{
			get
			{
				return this._err;
			}
			set
			{
				this._err = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _mapid;

		private ErrCode _err;

		private IExtension extensionObject;
	}
}
