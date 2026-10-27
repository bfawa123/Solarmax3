using System;

namespace Solarmax
{
	public class NetStream
	{
		public NetStream(int bufferSize)
		{
			this.mReadBuffer = new TBuffer<byte>(bufferSize);
			this.mReadBufferTemp = new byte[bufferSize / 2];
			this.mWriteBuffer = new TBuffer<byte>(bufferSize);
			this.mPipeInIdle = true;
			this.mPipeOutIdle = true;
		}

		public byte[] AsyncPipeIn
		{
			get
			{
				this.mPipeInIdle = false;
				return this.mReadBufferTemp;
			}
		}

		public bool AsyncPipeInIdle
		{
			get
			{
				return this.mPipeInIdle;
			}
		}

		public void FinishedIn(int length)
		{
			this.mReadBuffer.Push(this.mReadBufferTemp, length);
			this.mPipeInIdle = true;
		}

		public byte[] AsyncPipeOut
		{
			get
			{
				this.mPipeOutIdle = false;
				return this.mWriteBuffer.Buffer();
			}
		}

		public bool AsyncPipeOutIdle
		{
			get
			{
				return this.mPipeOutIdle;
			}
		}

		public void FinishedOut(int length)
		{
			this.mWriteBuffer.Pop(length);
			this.mPipeOutIdle = true;
		}

		public byte[] InStream
		{
			get
			{
				return this.mReadBuffer.Buffer();
			}
		}

		public int InStreamLength
		{
			get
			{
				return this.mReadBuffer.DataSize();
			}
		}

		public byte[] OutStream
		{
			get
			{
				return this.mWriteBuffer.Buffer();
			}
		}

		public int OutStreamLength
		{
			get
			{
				return this.mWriteBuffer.DataSize();
			}
		}

		public void PushInStream(byte[] buffer)
		{
			this.mReadBuffer.Push(buffer);
		}

		public void PopInStream(int length)
		{
			this.mReadBuffer.Pop(length);
		}

		public void PushOutStream(byte[] buffer)
		{
			this.mWriteBuffer.Push(buffer);
		}

		public void PopOutStream(int length)
		{
			this.mWriteBuffer.Pop(length);
		}

		public void Clear()
		{
			this.mReadBuffer.Clear();
			this.mWriteBuffer.Clear();
		}

		private TBuffer<byte> mReadBuffer;

		private byte[] mReadBufferTemp;

		private TBuffer<byte> mWriteBuffer;

		private volatile bool mPipeInIdle;

		private volatile bool mPipeOutIdle;
	}
}
