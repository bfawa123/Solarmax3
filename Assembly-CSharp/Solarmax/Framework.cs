using System;

namespace Solarmax
{
	public class Framework : Solarmax.Singleton<Framework>, Lifecycle
	{
		public bool Init()
		{
			if (this.IsInitFramework)
			{
				return true;
			}
			if (Solarmax.Singleton<DataProviderSystem>.Instance.InitDelay())
			{
				if (Solarmax.Singleton<DataHandlerSystem>.Instance.Init())
				{
					if (Solarmax.Singleton<LocalStorageSystem>.Instance.Init())
					{
						if (Solarmax.Singleton<OldLocalStorageSystem>.Instance.Init())
						{
							if (Solarmax.Singleton<ThirdPartySystem>.Instance.Init())
							{
								if (Solarmax.Singleton<NetSystem>.Instance.Init())
								{
									if (Solarmax.Singleton<BattleSystem>.Instance.Init())
									{
                                        Solarmax.Singleton<CollectionModel>.Get().EnsureInit();
										this.IsInitFramework = true;
										return true;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		public bool InitLanguageAndPing()
		{
			if (Solarmax.Singleton<DataProviderSystem>.Instance.Init())
			{
				if (Solarmax.Singleton<ConfigSystem>.Instance.Init())
				{
					if (Solarmax.Singleton<LoggerSystem>.Instance.Init())
					{
						if (Solarmax.Singleton<TimeSystem>.Instance.Init())
						{
							if (Solarmax.Singleton<EventSystem>.Instance.Init())
							{
                                Solarmax.Singleton<LanguageDataProvider>.Get().Load();
								if (Solarmax.Singleton<EngineSystem>.Instance.Init())
								{
									if (MonoSingleton<UpdateSystem>.Instance.Init())
									{
										if (Solarmax.Singleton<UISystem>.Instance.Init())
										{
											return true;
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		public void Tick(float interval)
		{
            Solarmax.Singleton<ConfigSystem>.Instance.Tick(interval);
            Solarmax.Singleton<LoggerSystem>.Instance.Tick(interval);
            Solarmax.Singleton<DataProviderSystem>.Instance.Tick(interval);
            Solarmax.Singleton<DataHandlerSystem>.Instance.Tick(interval);
            Solarmax.Singleton<TimeSystem>.Instance.Tick(interval);
            Solarmax.Singleton<EventSystem>.Instance.Tick(interval);
            Solarmax.Singleton<EngineSystem>.Instance.Tick(interval);
			MonoSingleton<UpdateSystem>.Instance.Tick(interval);
            Solarmax.Singleton<LocalStorageSystem>.Instance.Tick(interval);
            Solarmax.Singleton<UISystem>.Instance.Tick(interval);
            Solarmax.Singleton<ThirdPartySystem>.Instance.Tick(interval);
            Solarmax.Singleton<NetSystem>.Instance.Tick(interval);
            Solarmax.Singleton<BattleSystem>.Instance.Tick(interval);
			if (Solarmax.Singleton<LocalPlayer>.Get() != null)
			{
                Solarmax.Singleton<LocalPlayer>.Get().Tick(interval);
			}
		}

		public void UpdateRender(float interval)
		{
            Solarmax.Singleton<BattleSystem>.Instance.UpdateRender(interval);
		}

		public void Destroy()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("Framework destroy begin", new object[0]);
            Solarmax.Singleton<ConfigSystem>.Instance.Destroy();
            Solarmax.Singleton<EventSystem>.Instance.Destroy();
            Solarmax.Singleton<DataProviderSystem>.Instance.Destroy();
            Solarmax.Singleton<DataHandlerSystem>.Instance.Destroy();
            Solarmax.Singleton<TimeSystem>.Instance.Destroy();
            Solarmax.Singleton<EngineSystem>.Instance.Destroy();
			MonoSingleton<UpdateSystem>.Instance.Destroy();
            Solarmax.Singleton<LocalStorageSystem>.Instance.Destroy();
            Solarmax.Singleton<UISystem>.Instance.Destroy();
            Solarmax.Singleton<ThirdPartySystem>.Instance.Destroy();
            Solarmax.Singleton<NetSystem>.Instance.Destroy();
            Solarmax.Singleton<BattleSystem>.Instance.Destroy();
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("Framework destroy end.", new object[0]);
            Solarmax.Singleton<LoggerSystem>.Instance.Destroy();
		}

		public void SetWritableRootDir(string path)
		{
			this.mWritableRootDir = path;
		}

		public string GetWritableRootDir()
		{
			return this.mWritableRootDir;
		}

		private string mWritableRootDir = string.Empty;

		private bool IsInitFramework;
	}
}
