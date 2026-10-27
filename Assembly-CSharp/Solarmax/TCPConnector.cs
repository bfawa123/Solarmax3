using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Solarmax
{
	public class TCPConnector : INetConnector
	{
		public TCPConnector(IPacketFormat packetFormat, IPacketHandlerManager packetHandlerManager) : base(packetFormat, packetHandlerManager)
		{
			this.mSocket = null;
			this.mNetStream = new NetStream(INetConnector.MAX_SOCKET_BUFFER_SIZE * 2);
			this.tempReadPacketLength = 0;
			this.tempReadPacketType = 0;
			this.tempReadPacketData = null;
			this.mReadCompleteCallback = new AsyncCallback(this.ReadComplete);
			this.mSendCompleteCallback = new AsyncCallback(this.SendComplete);
			this.mSendThread = new AsyncThread(new Callback<AsyncThread>(this.SendLogic));
			this.mSendThread.Start();
		}

		public override bool Init()
		{
			base.Init();
			return true;
		}

		public override void Tick(float interval)
		{
			base.Tick(interval);
			this.doDecodeMessage();
		}

		public override void Destroy()
		{
			base.Destroy();
			this.mSendThread.Stop();
			this.DisConnect();
		}

		public override ConnectionType GetConnectionType()
		{
			return ConnectionType.TCP;
		}

		public static IPAddress GetIPv6Address(string address)
		{
			return null;
		}

		public static IPEndPoint CreateIPEndPoint(string address, int port)
		{
			return null;
		}

		public override void Connect(string address, int port)
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("TCPConnector  try connect addr {0} port {1}", address, port), new object[0]);
			//IPEndPoint ipendPoint = TCPConnector.CreateIPEndPoint(address, port);
			//base.SetConnectStatus(ConnectionStatus.CONNECTING);
			//base.Connect(address, port);
			//if (ipendPoint != null)
			//{
			//	this.mSocket = new TcpClient(ipendPoint.AddressFamily);
			//}
			//else
			//{
			//	this.mSocket = new TcpClient();
			//}
			//new AsyncThread(delegate(AsyncThread thread)
			//{
			//	base.SetConnectStatus(ConnectionStatus.CONNECTED);
			//	base.CallbackConnected(base.IsConnected());
			//}).Start();

            IPEndPoint endPoint = TCPConnector.CreateIPEndPoint(address, port);
            base.SetConnectStatus(ConnectionStatus.CONNECTING);
            base.Connect(address, port);
            if (endPoint != null)
            {
                this.mSocket = new TcpClient(endPoint.AddressFamily);
            }
            else
            {
                this.mSocket = new TcpClient();
            }
            AsyncThread asyncThread = new AsyncThread(delegate (AsyncThread thread)
            {
                try
                {
                    this.mSocket.NoDelay = true;
                    if (endPoint != null)
                    {
                        this.mSocket.Connect(endPoint);
                    }
                    else
                    {
                        this.mSocket.Connect(this.mRemoteHost.GetAddress(), this.mRemoteHost.GetPort());
                    }
                    this.mSocket.GetStream().BeginRead(this.mNetStream.AsyncPipeIn, 0, INetConnector.MAX_SOCKET_BUFFER_SIZE, this.mReadCompleteCallback, this);
                    this.SetConnectStatus(ConnectionStatus.CONNECTED);
                }
                catch (Exception ex)
                {
                    Singleton<LoggerSystem>.Instance.Error(ex.Message, new object[0]);
                    this.SetConnectStatus(ConnectionStatus.ERROR);
                }
                this.CallbackConnected(this.IsConnected());
            });
            asyncThread.Start();
        }

		public override void SendPacket(IPacket packet)
		{
            byte[] buffer = null;
            this.mPacketFormat.GenerateBuffer(ref buffer, packet);
            this.mNetStream.PushOutStream(buffer);
   //         int packetType = packet.GetPacketType();
			//PacketHelper helper = Solarmax.Singleton<NetSystem>.Instance.helper;
			//if (packetType == 210)
			//{
   //             Solarmax.Singleton<LoggerSystem>.Instance.Info("TCPConnector SendPacket with ID 210", new object[0]);
			//	helper.OnLoadOneChapter(1, default(PacketEvent));
			//	return;
			//}
			//if (packetType == 291)
			//{
   //             Solarmax.Singleton<LoggerSystem>.Instance.Info("TCPConnector SendPacket with ID 291", new object[0]);
			//	helper.OnSCAdeward(1, default(PacketEvent));
			//	return;
			//}
   //         Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("Warning: TCPConnector SendPacket Type Error with type {0}", packetType), new object[0]);
		}

		public override void DisConnect()
		{
			if (base.IsConnected())
			{
				base.SetConnectStatus(ConnectionStatus.DISCONNECTED);
				base.CallbackDisconnected();
				this.mSocket.GetStream().Close();
				this.mSocket.Close();
				this.mSocket = null;
				this.mNetStream.Clear();
			}
		}

		private void ReadComplete(IAsyncResult ar)
		{
			try
			{
				int num = this.mSocket.GetStream().EndRead(ar);
				if (num > 0)
				{
					this.mNetStream.FinishedIn(num);
					this.mSocket.GetStream().BeginRead(this.mNetStream.AsyncPipeIn, 0, INetConnector.MAX_SOCKET_BUFFER_SIZE, this.mReadCompleteCallback, this);
				}
				else
				{
					Debug.LogError("Ping 读取数据为0，将要断开此链接接:" + this.mRemoteHost.ToString());
					this.DisConnect();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("链接：" + this.mRemoteHost.ToString() + ", 发生读取错误：" + ex.Message);
				this.DisConnect();
			}
		}

		private void SendComplete(IAsyncResult ar)
		{
			try
			{
				this.mSocket.GetStream().EndWrite(ar);
				int num = (int)ar.AsyncState;
				if (num > 0)
				{
					this.mNetStream.FinishedOut(num);
				}
				else
				{
					Debug.LogError("发送数据为0，将要断开此链接接:" + this.mRemoteHost.ToString());
					this.DisConnect();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("发生写入错误：" + ex.Message);
				this.DisConnect();
			}
		}

		private void doDecodeMessage()
		{
			while (this.mNetStream.InStreamLength > 0 && this.mPacketFormat.CheckHavePacket(this.mNetStream.InStream, this.mNetStream.InStreamLength))
			{
				this.mPacketFormat.DecodePacket(this.mNetStream.InStream, ref this.tempReadPacketLength, ref this.tempReadPacketType, ref this.tempReadPacketData);
				try
				{
					this.mPacketHandlerManager.DispatchHandler(this.tempReadPacketType, this.tempReadPacketData);
					base.CallbackRecieved(this.tempReadPacketType, this.tempReadPacketData);
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("消息内发生错误：{0}", new object[]
					{
						ex.ToString()
					});
				}
				this.mNetStream.PopInStream(this.tempReadPacketLength);
			}
		}

		private void SendLogic(AsyncThread thread)
		{
			while (thread.IsWorking())
			{
				this.doSendMessage();
				Thread.Sleep(20);
			}
		}

		private void doSendMessage()
		{
			int outStreamLength = this.mNetStream.OutStreamLength;
			if (base.IsConnected() && this.mNetStream.AsyncPipeOutIdle && outStreamLength > 0 && this.mSocket.GetStream().CanWrite)
			{
				try
				{
					this.mSocket.GetStream().BeginWrite(this.mNetStream.AsyncPipeOut, 0, outStreamLength, this.mSendCompleteCallback, outStreamLength);
				}
				catch (Exception ex)
				{
					Debug.LogError("发送数据错误：" + ex.Message);
					this.DisConnect();
				}
			}
		}

		private TcpClient mSocket;

		private NetStream mNetStream;

		private int tempReadPacketLength;

		private int tempReadPacketType;

		private byte[] tempReadPacketData;

		private AsyncCallback mReadCompleteCallback;

		private AsyncCallback mSendCompleteCallback;

		private AsyncThread mSendThread;
	}
}
