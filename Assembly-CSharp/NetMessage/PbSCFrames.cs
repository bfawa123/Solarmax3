using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PbSCFrames")]
	[Serializable]
	public class PbSCFrames : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "ready", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public SCReady ready
		{
			get
			{
				return this._ready;
			}
			set
			{
				this._ready = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "start", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public SCStartBattle start
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		[ProtoMember(3, Name = "frames", DataFormat = DataFormat.Default)]
		public List<SCFrame> frames
		{
			get
			{
				return this._frames;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "finish", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public SCFinishBattle finish
		{
			get
			{
				return this._finish;
			}
			set
			{
				this._finish = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private SCReady _ready;

		private SCStartBattle _start;

		private readonly List<SCFrame> _frames = new List<SCFrame>();

		private SCFinishBattle _finish;

		private IExtension extensionObject;
	}
}
