using System;
using UnityEngine;

namespace Solarmax
{
	public class EngineSystem : Solarmax.Singleton<EngineSystem>, Lifecycle
	{
		public bool Init()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("EngineSystem    init  begin", new object[0]);
			string empty = string.Empty;
			if (Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("fps", out empty))
			{
				this.SetFPS(Converter.ConvertNumber<int>(empty));
			}
			Application.runInBackground = true;
			Application.targetFrameRate = 60;
			Screen.sleepTimeout = -1;
			Input.multiTouchEnabled = true;
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("EngineSystem    init  end", new object[0]);
			return true;
		}

		public void Tick(float interval)
		{
			if (this.onNetStatusChanged != null)
			{
				if (this.checkDelta < 1f)
				{
					this.checkDelta += interval;
				}
				else
				{
					this.checkDelta = 0f;
					this.onNetStatusChanged((NetworkReachability)this.GetNetworkRechability());
				}
			}
		}

		public void Destroy()
		{
		}

		private void SetFPS(int fps)
		{
			if (fps > 0)
			{
				this.mFPS = fps;
			}
		}

		public int GetFPS()
		{
			return this.mFPS;
		}

		public int GetNetworkRechability()
		{
			NetworkReachability internetReachability = Application.internetReachability;
			int result = 0;
			if (internetReachability == NetworkReachability.NotReachable)
			{
				result = 0;
			}
			else if (internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
			{
				result = 1;
			}
			else if (internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
			{
				result = 2;
			}
			return result;
		}

		public string GetOS()
		{
			return SystemInfo.operatingSystem;
		}

		public string GetUUID()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public string GetDeviceModel()
		{
			return SystemInfo.deviceModel;
		}

		public string GetPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
				return "osx_editor";
			case RuntimePlatform.OSXPlayer:
				return "osx";
			case RuntimePlatform.WindowsPlayer:
				return "windows";
			case RuntimePlatform.WindowsEditor:
				return "windows_editor";
			case RuntimePlatform.IPhonePlayer:
				return "ios";
			case RuntimePlatform.Android:
				return "android";
			}
			return "UnknownPlatform";
		}

		private const float NET_STATUS_CHECK_INTERVAL = 1f;

		private int mFPS = 30;

		private float checkDelta;

		public EngineSystem.OnNetStatusChanged onNetStatusChanged;

		public delegate void OnNetStatusChanged(NetworkReachability status);
	}
}
