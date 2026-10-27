using System;

namespace Solarmax
{
	public class INetConnector : Lifecycle
	{
		public INetConnector(IPacketFormat packetFormat, IPacketHandlerManager packetHandlerManager)
		{
			this.mUid = -1;
			this.mRemoteHost = null;
			this.OnConnected = null;
			this.OnRecieved = null;
			this.OnDisconnected = null;
			this.OnError = null;
			this.mPacketFormat = packetFormat;
			this.mPacketHandlerManager = packetHandlerManager;
			this.mDisconnectEvent = false;
		}

		public virtual bool Init()
		{
			return this.mPacketHandlerManager.Init();
		}

		public virtual void Tick(float interval)
		{
			this.mPacketHandlerManager.Tick(interval);
			if (this.mDisconnectEvent)
			{
				this.mDisconnectEvent = false;
				this.OnDisconnected();
			}
		}

		public virtual void Destroy()
		{
			this.mPacketHandlerManager.Destroy();
		}

		public virtual ConnectionType GetConnectionType()
		{
			return ConnectionType.UNKNOW;
		}

		public virtual void Connect(string address, int port)
		{
            this.mRemoteHost = new RemoteHost(address, port);
        }

		public virtual void SendPacket(IPacket packet)
		{
		}

		public virtual void DisConnect()
		{
		}

		protected void CallbackConnected(bool status)
		{
			if (this.OnConnected != null)
			{
				this.OnConnected(status);
			}
		}

		protected void CallbackRecieved(int ptype, byte[] ms)
		{
			if (this.OnRecieved != null && ms != null)
			{
				this.OnRecieved(ptype, ms);
			}
		}

		protected void CallbackDisconnected()
		{
			if (this.OnDisconnected != null)
			{
				this.mDisconnectEvent = true;
			}
		}

		protected void CallbackError()
		{
			if (this.OnError != null)
			{
				this.OnError();
			}
		}

		public void SetUid(int id)
		{
			this.mUid = id;
		}

		public int GetUid()
		{
			return this.mUid;
		}

		public RemoteHost GetHost()
		{
			return this.mRemoteHost;
		}

		public bool IsConnected()
		{
			return this.mConnectedStatus == ConnectionStatus.CONNECTED;
		}

		public ConnectionStatus GetConnectStatus()
		{
			return this.mConnectedStatus;
		}

		public void SetConnectStatus(ConnectionStatus status)
		{
			this.mConnectedStatus = status;
		}

		public void RegisterHandler(int packetType, MessageHandler handler)
		{
			this.mPacketHandlerManager.RegisterHandler(packetType, handler);
		}

		public static int MAX_SOCKET_BUFFER_SIZE = 4096;

		public Callback<bool> OnConnected;

		public Callback<int, byte[]> OnRecieved;

		public Callback OnDisconnected;

		public Callback OnError;

		private int mUid;

		protected RemoteHost mRemoteHost;

		protected IPacketFormat mPacketFormat;

		protected IPacketHandlerManager mPacketHandlerManager;

		private ConnectionStatus mConnectedStatus;

		private bool mDisconnectEvent;
	}
}
