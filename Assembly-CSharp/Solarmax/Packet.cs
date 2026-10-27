using System;

namespace Solarmax
{
	public class Packet
	{
		public Packet(int msgId, object msg)
		{
			this.MsgId = msgId;
			this.Msg = msg;
		}

		public int MsgId { get; private set; }

		public object Msg { get; private set; }
	}
}
