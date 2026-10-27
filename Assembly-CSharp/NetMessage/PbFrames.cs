using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PbFrames")]
	[Serializable]
	public class PbFrames : IExtensible
	{
		[ProtoMember(1, Name = "frames", DataFormat = DataFormat.Default)]
		public List<PbFrame> frames
		{
			get
			{
				return this._frames;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<PbFrame> _frames = new List<PbFrame>();

		private IExtension extensionObject;
	}
}
