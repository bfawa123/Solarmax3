using System;
using System.Collections.Generic;
using Plugin;

public abstract class ObjectPool : Listenner
{
	public ObjectPool()
	{
		this.pool = new Queue<IPoolNode>();
		this.livePool = new List<IPoolNode>();
	}

	protected Queue<IPoolNode> pool { get; set; }

	protected List<IPoolNode> livePool { get; set; }

	public virtual void Release()
	{
		foreach (IPoolNode node in this.livePool.ToArray())
		{
			this.Recovery(node);
		}
		for (int j = 0; j < this.livePool.Count; j++)
		{
			this.livePool[j].Release();
			base.RemoveListenner(new RunLockStepLogic(this.livePool[j].Update));
			base.RemoveListenner(new RunLockStepEvent(this.livePool[j].Release));
		}
		IPoolNode[] array2 = this.pool.ToArray();
		for (int k = 0; k < array2.Length; k++)
		{
			array2[k].Release();
			base.RemoveListenner(new RunLockStepLogic(array2[k].Update));
			base.RemoveListenner(new RunLockStepEvent(array2[k].Release));
		}
		this.pool.Clear();
		this.livePool.Clear();
	}

	private void AddLive(IPoolNode node)
	{
		this.livePool.Add(node);
	}

	public T GetObject<T>()
	{
		IPoolNode poolNode = null;
		if (this.pool.Count > 0)
		{
			poolNode = this.pool.Dequeue();
		}
		if (poolNode == null)
		{
			poolNode = (Activator.CreateInstance(typeof(T)) as IPoolNode);
			base.AddListenner(new RunLockStepLogic(poolNode.Update));
			base.AddListenner(new RunLockStepEvent(poolNode.Release));
		}
		poolNode.SetPool(this);
		this.AddLive(poolNode);
		return (T)((object)poolNode);
	}

	public void Recovery(IPoolNode node)
	{
		this.livePool.Remove(node);
		node.Recovery();
		this.pool.Enqueue(node);
	}

	public void CreateFreeObject<T>()
	{
		IPoolNode poolNode = Activator.CreateInstance(typeof(T)) as IPoolNode;
		poolNode.SetPool(this);
		poolNode.Release();
		this.pool.Enqueue(poolNode);
	}

	public void Update(int frame, float dt)
	{
		for (int i = 0; i < this.livePool.Count; i++)
		{
			this.livePool[i].Update(frame, dt);
		}
	}
}
