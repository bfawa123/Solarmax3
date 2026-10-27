using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class DataHandlerSystem : Solarmax.Singleton<DataHandlerSystem>, Lifecycle
	{
		public bool Init()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("DataHandlerSystem   init   begin", new object[0]);
			this.RegistDataHandler(Solarmax.Singleton<FriendDataHandler>.Instance);
			this.RegistDataHandler(Solarmax.Singleton<LevelDataHandler>.Instance);
			this.RegistDataHandler(Solarmax.Singleton<UserSyncSysteam>.Instance);
			this.RegistDataHandler(Solarmax.Singleton<OrderManager>.Instance);
			this.RegistDataHandler(Solarmax.Singleton<ReconnectHandler>.Instance);
			this.RegistDataHandler(Solarmax.Singleton<ItemDataHandler>.Instance);
			bool result = this.doInit();
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("DataHandlerSystem   init   end", new object[0]);
			return result;
		}

		public void Tick(float interval)
		{
			int i = 0;
			int count = this.mDataHandler.Count;
			while (i < count)
			{
				this.mDataHandler[i].Tick(interval);
				i++;
			}
		}

		public void Destroy()
		{
			this.mDataHandler.Clear();
		}

		private bool doInit()
		{
			bool flag = true;
			int i = 0;
			int count = this.mDataHandler.Count;
			while (i < count)
			{
				flag &= this.mDataHandler[i].Init();
				i++;
			}
			return flag;
		}

		private void RegistDataHandler(IDataHandler dataHandler)
		{
			this.mDataHandler.Add(dataHandler);
		}

		private List<IDataHandler> mDataHandler = new List<IDataHandler>();
	}
}
