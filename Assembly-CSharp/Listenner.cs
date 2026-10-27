using System;
using System.Collections.Generic;
using Plugin;

public abstract class Listenner
{
	public Listenner()
	{
		this.releaseHandler = new List<RunLockStepEvent>();
		this.updateHandler = new List<RunLockStepLogic>();
	}

	private void Release()
	{
	}

	protected void AddListenner(RunLockStepEvent handler)
	{
		this.releaseHandler.Add(handler);
	}

	protected void RemoveListenner(RunLockStepEvent handler)
	{
		this.releaseHandler.Remove(handler);
	}

	protected void InvokeReleaseEvent()
	{
		for (int i = 0; i < this.releaseHandler.Count; i++)
		{
			this.releaseHandler[i]();
		}
	}

	protected void AddListenner(RunLockStepLogic handler)
	{
		this.updateHandler.Add(handler);
	}

	protected void RemoveListenner(RunLockStepLogic handler)
	{
		this.updateHandler.Remove(handler);
	}

	protected void InvokeLogicEvent(int frame, float dt)
	{
		for (int i = 0; i < this.updateHandler.Count; i++)
		{
			this.updateHandler[i](frame, dt);
		}
	}

	protected List<RunLockStepEvent> releaseHandler;

	protected List<RunLockStepLogic> updateHandler;
}
