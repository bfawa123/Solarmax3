using System;
using UnityEngine;

namespace Solarmax
{
	public class PingHelper : Lifecycle
	{
		public void Reset()
		{
			this.lastPingTime = 0f;
			this.MaxNoReplyCount = 3;
			this.pingInterval = 5;
			this.noReplyCount = 0;
			this.lastNetState = Solarmax.Singleton<EngineSystem>.Instance.GetNetworkRechability();
		}

		public bool Init()
		{
			this.Reset();
			this.enable = true;
			return true;
		}

		public void Tick(float interval)
		{
			if (this.enable)
			{
				this.timeCount += interval;
				if (this.timeCount > (float)this.pingInterval)
				{
					this.timeCount -= (float)this.pingInterval;
					this.Ping();
				}
			}
		}

		public void Destroy()
		{
			this.enable = false;
			this.Reset();
		}

		private void Ping()
		{
			if (this.noReplyCount >= this.MaxNoReplyCount)
			{
				if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
				{
					Debug.LogFormat("由于连续{0}次未收到Ping回复，主动断开连接！", new object[]
					{
						this.noReplyCount
					});
                    Solarmax.Singleton<NetSystem>.Instance.Close();
				}
				this.noReplyCount = 0;
				this.lastPingTime = 0f;
			}
			if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
			{
                Solarmax.Singleton<NetSystem>.Instance.helper.PingNet();
				this.noReplyCount++;
			}
			int networkRechability = Solarmax.Singleton<EngineSystem>.Instance.GetNetworkRechability();
			if (this.lastNetState != networkRechability)
			{
				this.lastNetState = networkRechability;
				Debug.LogFormat("由于网络状态改变，主动断开连接！", new object[]
				{
					this.noReplyCount
				});
				if (this.lastNetState != 0 && networkRechability == 0)
				{
                    Solarmax.Singleton<NetSystem>.Instance.Close();
				}
			}
		}

		public void Pong(float time)
		{
			this.lastPingTime = time;
			this.noReplyCount = 0;
		}

		public void GetNetPic(out string pic, out Color color)
		{
			pic = "icon_net_offline";
			color = Color.red;
			if (this.lastPingTime > 150f)
			{
				color = Color.red;
			}
			else if (this.lastPingTime > 100f)
			{
				color = Color.yellow;
			}
			else if (this.lastPingTime > 0f)
			{
				color = Color.green;
			}
			int networkRechability = Solarmax.Singleton<EngineSystem>.Instance.GetNetworkRechability();
			if (networkRechability == 0)
			{
				pic = "icon_net_offline";
			}
			else if (networkRechability == 1)
			{
				if (this.lastPingTime > 150f)
				{
					pic = "icon_net_mobile_01";
				}
				else if (this.lastPingTime > 100f)
				{
					pic = "icon_net_mobile_02";
				}
				else if (this.lastPingTime > 0f)
				{
					pic = "icon_net_mobile_03";
				}
			}
			else if (networkRechability == 2)
			{
				if (this.lastPingTime > 150f)
				{
					pic = "icon_net_wifi_01";
				}
				else if (this.lastPingTime > 100f)
				{
					pic = "icon_net_wifi_02";
				}
				else if (this.lastPingTime > 0f)
				{
					pic = "icon_net_wifi_03";
				}
			}
		}

		public float lastPingTime;

		public int MaxNoReplyCount;

		public int pingInterval;

		private int noReplyCount;

		private float timeCount;

		private bool enable;

		private int lastNetState;
	}
}
