using System;

namespace Solarmax
{
	public class IObject
	{
		public IObject()
		{
			this.mInstanceId = this._INSTANCEID_();
		}

		private long _INSTANCEID_()
		{
			return IObject._instance_id_index_dump_ += 1L;
		}

		~IObject()
		{
		}

		public long GetInstanceId()
		{
			return this.mInstanceId;
		}

		private static long _instance_id_index_dump_;

		private long mInstanceId;
	}
}
