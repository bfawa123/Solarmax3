using System;

namespace Solarmax
{
	public struct PacketEvent
	{
		public PacketEvent(int id, object data)
		{
			this._id = id;
			this._data = data;
		}

		public int id
		{
			get
			{
				return this._id;
			}
		}

		public object Data
		{
			get
			{
				return this._data;
			}
		}

		private int _id;

		private object _data;
	}
}
