using System;
using System.Collections.Generic;
using System.Linq;
using NetMessage;
using Solarmax;
using UnityEngine;

namespace Plugin
{
	public class LockStep
	{
		public LockStep()
		{
			this.maxFrame = 10;
			this.logicFrame = 3;
			this.FRAME_TIME = 1f / (float)(this.maxFrame * this.logicFrame);
			this.frameThreshold = 3;
			this.playSpeed = 0f;
			this.Release();
		}

		private FrameNode currentNode { get; set; }

		private float totalFrameTime { get; set; }

		private float currentFrameTimer { get; set; }

		private int currentFrame { get; set; }

		private int maxFrame { get; set; }

		private int logicFrame { get; set; }

		private float FRAME_TIME { get; set; }

		public int frameThreshold { get; set; }

		public int FPS
		{
			get
			{
				return this.currentFPS;
			}
		}

		public float playSpeed
		{
			get
			{
				return this.speed;
			}
			set
			{
				if (this.speed != value)
				{
					this.speed = value;
					if (this.onPlaySpeedChange != null)
					{
						this.onPlaySpeedChange(this.speed);
					}
				}
			}
		}

		public int runFrameCount { get; set; }

		public int messageCount
		{
			get
			{
				return this.frameQueue.Count;
			}
		}

		private int runToFrame { get; set; }

		private int frameCount { get; set; }

		private int currentFPS { get; set; }

		private float totalTime { get; set; }

		public bool isAlive { get; set; }

		public RunLockStepFrameError frameError { get; set; }

		private int lastFrame { get; set; }

		private void Release()
		{
			this.Reset();
			this.currentNode = null;
			this.packetHandler = null;
			this.lockstepHandler = null;
			this.beginHandler = null;
			this.endHandler = null;
			this.runFrameCount = 0;
		}

		public void Reset()
		{
			this.isAlive = false;
			this.currentNode = null;
			this.frameQueue.Clear();
			this.totalFrameTime = 0f;
			this.currentFrame = 0;
			this.currentFPS = 0;
			this.totalTime = 0f;
			this.totalFrameTime = 0f;
			this.runToFrame = 0;
			this.lastFrame = 0;
			this.replay = false;
			this.replaySingle = false;
		}

		public int GetCurrentFrame()
		{
			return this.currentFrame;
		}

		public void StarLockStep()
		{
			if (this.isAlive)
			{
				throw new Exception("LockStep is alive.");
			}
			this.totalFrameTime = 0f;
			this.currentNode = null;
			this.currentFrame = 0;
			this.isAlive = true;
		}

		public void StopLockStep(bool release = false)
		{
			if (release)
			{
				this.Release();
			}
			else
			{
				this.Reset();
			}
		}

		public void AddFrame(int frame, object[] msgList)
		{
            if (this.lastFrame >= frame)
			{
				return;
			}
			FrameNode frameNode = new FrameNode();
			frameNode.frame = frame;
			frameNode.msgList = msgList;
			this.frameQueue.Enqueue(frameNode);
            this.lastFrame = frame;
		}

		public void RunToFrame(int frame)
		{
			this.runToFrame = frame * this.logicFrame;
		}

		public void AddListennerSpeed(RunLockStepSpeed handler)
		{
			this.speedHandler = handler;
			if (this.speedHandler == null)
			{
				this.speedHandler = new RunLockStepSpeed(this.TimeScaleChange);
			}
		}

		public void AddRunLockStepFrameError(RunLockStepFrameError errorHandle)
		{
			this.frameError = errorHandle;
		}

		public void AddListennerEnd(RunLockStepEvent handler)
		{
			this.endHandler = ((this.endHandler != null) ? ((RunLockStepEvent)Delegate.Combine(this.endHandler, handler)) : handler);
		}

		public void AddListennerBegin(RunLockStepEvent handler)
		{
			this.beginHandler = ((this.beginHandler != null) ? ((RunLockStepEvent)Delegate.Combine(this.beginHandler, handler)) : handler);
		}

		public void AddListennerPacket(RunPacketHandler handler)
		{
			this.packetHandler = ((this.packetHandler != null) ? ((RunPacketHandler)Delegate.Combine(this.packetHandler, handler)) : handler);
		}

		public void AddListennerLogic(RunLockStepLogic handler)
		{
			this.lockstepHandler = ((this.lockstepHandler != null) ? ((RunLockStepLogic)Delegate.Combine(this.lockstepHandler, handler)) : handler);
		}

		public void Tick(float dt)
		{
			if (!this.isAlive)
			{
				return;
			}
			this.CalcFPS(dt);
			if (this.frameQueue.Count == 0 && !this.replaySingle)
			{
				return;
			}
			if (this.replaySingle)
			{
				this.ReplaySingleProcess(dt);
			}
			else if (this.replay)
			{
				this.ReplayProcess(dt);
			}
			else
			{
				this.NormalProcess(dt);
			}
		}

		private void ReplayProcess(float dt)
		{
			if (this.runToFrame > this.currentFrame)
			{
				int num = this.logicFrame * this.frameQueue.Count;
				int num2 = (this.runFrameCount > 0) ? (this.runFrameCount * this.logicFrame) : num;
				num2 = ((num2 < num) ? num2 : num);
				for (int i = 0; i < num2; i++)
				{
					this.NextFrameLogic();
				}
			}
			else if (this.playSpeed > 1f)
			{
				int num3 = Mathf.RoundToInt(this.playSpeed);
				for (int j = 0; j < num3; j++)
				{
					this.NextFrameLogic();
				}
			}
			else if (this.playSpeed < 1f)
			{
				this.totalFrameTime += dt * this.playSpeed;
				if (this.totalFrameTime >= this.FRAME_TIME)
				{
					this.totalFrameTime -= this.FRAME_TIME;
					this.NextFrameLogic();
				}
			}
			else
			{
				this.totalFrameTime += dt;
				if (this.totalFrameTime >= this.FRAME_TIME)
				{
					this.totalFrameTime -= this.FRAME_TIME;
					this.NextFrameLogic();
				}
			}
		}

		private void ReplaySingleProcess(float dt)
		{
			if (this.runToFrame > this.currentFrame)
			{
				int num = this.logicFrame * this.frameQueue.Count;
				int num2 = (this.runFrameCount > 0) ? (this.runFrameCount * this.logicFrame) : num;
				num2 = ((num2 < num) ? num2 : num);
				for (int i = 0; i < num2; i++)
				{
					this.NextFrameSingleLogic();
				}
			}
			else if (this.playSpeed > 1f)
			{
				int num3 = Mathf.RoundToInt(this.playSpeed);
				for (int j = 0; j < num3; j++)
				{
					this.NextFrameSingleLogic();
				}
			}
			else if (this.playSpeed < 1f)
			{
				this.totalFrameTime += dt * this.playSpeed;
				if (this.totalFrameTime >= this.FRAME_TIME)
				{
					this.totalFrameTime -= this.FRAME_TIME;
					this.NextFrameSingleLogic();
				}
			}
			else
			{
				this.totalFrameTime += dt;
				if (this.totalFrameTime >= this.FRAME_TIME)
				{
					this.totalFrameTime -= this.FRAME_TIME;
					this.NextFrameSingleLogic();
				}
			}
		}

		private void NormalProcess(float dt)
		{
			if (this.frameQueue.Count > this.frameThreshold)
			{
				if (this.runToFrame > this.currentFrame)
				{
					int num = this.logicFrame * this.frameQueue.Count;
					int num2 = (this.runFrameCount > 0) ? (this.runFrameCount * this.logicFrame) : num;
					num2 = ((num2 < num) ? num2 : num);
					for (int i = 0; i < num2; i++)
					{
						this.NextFrameLogic();
					}
				}
				else if (this.currentFPS >= 20)
				{
					for (int j = 0; j < this.logicFrame; j++)
					{
						this.NextFrameLogic();
					}
				}
				else
				{
					for (int k = 0; k < this.logicFrame * this.frameQueue.Count; k++)
					{
						this.NextFrameLogic();
					}
				}
			}
			else
			{
				if (this.speedHandler != null)
				{
					Time.timeScale = this.speedHandler(this.frameQueue.Count);
				}
				this.totalFrameTime += dt;
				if (this.totalFrameTime >= this.FRAME_TIME)
				{
					this.totalFrameTime -= this.FRAME_TIME;
					this.NextFrameLogic();
				}
			}
		}

		private void CalcFPS(float dt)
		{
			this.frameCount++;
			this.totalTime += dt;
			if (this.totalTime >= 1f)
			{
				this.currentFPS = this.frameCount;
				this.frameCount = 0;
				this.totalTime = 0f;
			}
		}

		private void NextFrameLogic()
		{
			if (this.frameQueue.Count == 0)
			{
				return;
			}
			this.currentFrame++;
            if (this.currentFrame % this.logicFrame == 0)
			{
				this.currentNode = this.frameQueue.Dequeue();
                if (this.currentNode.frame * this.logicFrame != this.currentFrame && this.frameError != null)
				{
                    this.frameError(this.currentFrame, this.currentNode);
				}
				if (this.packetHandler != null)
				{
                    this.packetHandler(this.currentNode);
				}
			}
			this.currentFrameTimer = this.FRAME_TIME;
			if (this.lockstepHandler != null)
			{
				this.lockstepHandler(this.currentFrame, this.currentFrameTimer);
			}
		}

		private void NextFrameSingleLogic()
		{
			if (this.frameQueue.Count == 0 && this.currentNode == null)
			{
				return;
			}
			this.currentFrame++;
			if (this.lockstepHandler != null)
			{
				this.lockstepHandler(this.currentFrame, this.FRAME_TIME);
			}
			if (this.frameQueue.Count > 0 && this.currentNode == null)
			{
				this.currentNode = this.frameQueue.Dequeue();
			}
			if (this.currentNode != null && this.currentFrame == this.currentNode.frame)
			{
				if (this.packetHandler != null)
				{
					this.packetHandler(this.currentNode);
				}
				this.currentNode = null;
			}
		}

		private float TimeScaleChange(int len)
		{
			if (len <= 1)
			{
				return 1f;
			}
			if (len <= this.frameThreshold)
			{
				return 1f;
			}
			return 3f;
		}

		private Queue<FrameNode> frameQueue = new Queue<FrameNode>();

		private RunPacketHandler packetHandler;

		private RunLockStepLogic lockstepHandler;

		private RunLockStepEvent beginHandler;

		private RunLockStepEvent endHandler;

		private RunLockStepSpeed speedHandler;

		private float speed = 1f;

		public LockStep.OnPlaySpeedChange onPlaySpeedChange;

		public bool replay;

		public bool replaySingle;

		public delegate void OnPlaySpeedChange(float speed);
	}
}
