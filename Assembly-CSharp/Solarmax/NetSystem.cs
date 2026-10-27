using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class NetSystem : Solarmax.Singleton<NetSystem>, Lifecycle
	{
		public NetSystem()
		{
			this.helper = new PacketHelper();
			this.ping = new PingHelper();
			this.mConnectorMap = new Dictionary<int, INetConnector>();
			this.mCacheMap = new Stack<NetPacket>();
		}

		public bool Init()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("NetSystem    init  begin", new object[0]);
			this.RegisterConnector(1, ConnectionType.TCP, new PacketFormat(), new PacketHandlerManager(), new Callback<bool>(this.ConnectedCallback), null, new Callback(this.DisConnectedCallback), null);
			foreach (INetConnector netConnector in this.mConnectorMap.Values)
			{
				netConnector.Init();
			}
			this.helper.RegisterAllPacketHandler();
			this.ping.Init();
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("NetSystem    init  end", new object[0]);
			return true;
		}

		public void Tick(float interval)
		{
            foreach (INetConnector netConnector in this.mConnectorMap.Values)
			{
				netConnector.Tick(interval);
			}
			this.ping.Tick(interval);
		}

		public void Destroy()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("NetSystem    destroy  begin", new object[0]);
			foreach (INetConnector netConnector in this.mConnectorMap.Values)
			{
				netConnector.Destroy();
			}
			this.mCacheMap.Clear();
			this.ping.Destroy();
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("NetSystem    destroy  end", new object[0]);
		}

		public void RegisterConnector(int uid, ConnectionType type, IPacketFormat pf, IPacketHandlerManager phm, Callback<bool> connected, Callback<int, byte[]> recieved, Callback disconnected, Callback error)
		{
			INetConnector netConnector;
			if (type != ConnectionType.TCP)
			{
				netConnector = new TCPConnector(pf, phm);
			}
			else
			{
				netConnector = new TCPConnector(pf, phm);
			}
			netConnector.OnConnected = connected;
			netConnector.OnRecieved = recieved;
			netConnector.OnDisconnected = disconnected;
			netConnector.OnError = error;
			netConnector.SetUid(uid);
			this.mConnectorMap.Add(uid, netConnector);
		}

		public void Connect(int uid, string address, int port)
		{
			if (this.mConnectorMap.ContainsKey(uid))
			{
				this.mConnectorMap[uid].Connect(address, port);
			}
		}

		public void Connect(string address, int port)
		{
			this.Connect(1, address, port);
		}

		public void Send<T>(int packetId, T proto, bool bShowC = true) where T : class
		{
			if (packetId != 23)
				Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("NetSystem  Send  with id {0}", packetId), new object[0]);
            int key = 1;
            if (this.mConnectorMap.ContainsKey(key))
            {
                NetPacket netPacket = new NetPacket(packetId);
                netPacket.EncodeProto<T>(proto);
                if (this.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED && bShowC)
                {
                    if (this.IsReconnecting)
                    {
                        this.mCacheMap.Clear();
                    }
                    if (!this.IsReconnecting)
                    {
                        this.IsReconnecting = true;
                        Singleton<UISystem>.Instance.ShowWindow("ReconnectWindow");
                    }
                    this.mCacheMap.Push(netPacket);
                }
                else
                {
                    this.mConnectorMap[key].SendPacket(netPacket);
                }
            }
        }

		public void Send2Cache<T>(int packetId, T proto) where T : class
		{
			int key = 1;
			if (this.mConnectorMap.ContainsKey(key))
			{
				NetPacket netPacket = new NetPacket(packetId);
				netPacket.EncodeProto<T>(proto);
				this.mCacheMap.Push(netPacket);
			}
		}

		public void ClearSendCache()
		{
			this.mCacheMap.Clear();
		}

		public void SendCache2Net()
		{
			int count = this.mCacheMap.Count;
			for (int i = 0; i < count; i++)
			{
				NetPacket netPacket = this.mCacheMap.Pop();
				if (netPacket != null)
				{
					int key = 1;
					if (this.mConnectorMap.ContainsKey(key))
					{
						int packetType = netPacket.GetPacketType();
                        Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("NetSystem SendCache2Net type:{0}", packetType), new object[0]);
						this.mConnectorMap[key].SendPacket(netPacket);
					}
				}
			}
		}

		public void Close(int uid)
		{
			if (this.mConnectorMap.ContainsKey(uid))
			{
				this.mConnectorMap[uid].DisConnect();
			}
		}

		public void Close()
		{
			this.Close(1);
		}

		public INetConnector GetConnector(int uid)
		{
			INetConnector result = null;
			this.mConnectorMap.TryGetValue(uid, out result);
			return result;
		}

		public INetConnector GetConnector()
		{
			return this.GetConnector(1);
		}

		public void ConnectedCallback(bool status)
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Info("已连接服务器：" + this.GetConnector().GetHost().ToString(), new object[0]);
		}

		public void DisConnectedCallback()
		{
			this.ping.Pong(-1f);
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.NetworkStatus, new object[]
			{
				false
			});
			if (Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver)
			{
				return;
			}
			BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
			if (battleData.gameState == GameState.Game || battleData.gameState == GameState.GameWatch || battleData.gameState == GameState.Watcher)
			{
				if (battleData.gameType == GameType.PVP || battleData.gameType == GameType.League)
				{
                    Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBattleDisconnect, new object[0]);
				}
			}
			else if (battleData.gameType == GameType.PVP || battleData.gameType == GameType.League)
			{
                Solarmax.Singleton<BattleSystem>.Instance.Reset();
                Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
                Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
				{
					1,
					LanguageDataProvider.GetValue(21),
					new EventDelegate(new EventDelegate.Callback(this.BackStartWindow))
				});
			}
		}

		public void BackStartWindow()
		{
            Solarmax.Singleton<UISystem>.Get().HideAllWindow();
            Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
		}

		private Dictionary<int, INetConnector> mConnectorMap;

		private Stack<NetPacket> mCacheMap;

		public PacketHelper helper;

		public PingHelper ping;

		public bool IsReconnecting;
	}
}
