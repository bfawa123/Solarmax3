using System;
using System.Collections.Generic;

public class AnyPool<T> where T : IPoolGo, new()
{
	public AnyPool(Func<T> create)
	{
		this.create = create;
	}

	public T Alloc()
	{
		if (this.free.Count > 0)
		{
			object obj = this.work;
			lock (obj)
			{
				LinkedListNode<T> last = this.free.Last;
				this.free.RemoveLast();
				this.work.Add(last.Value);
				return last.Value;
			}
		}
		object obj2 = this.work;
		T go2;
		lock (obj2)
		{
			T go = this.create();
			this.work.Add(go);
			go.Init(delegate
			{
				this.Recycle(go);
			}, delegate
			{
				this.Destroy(go);
			});
			go2 = go;
		}
		return go2;
	}

	private void Recycle(T go)
	{
		object obj = this.work;
		lock (obj)
		{
			this.work.Remove(go);
			this.free.AddFirst(go);
		}
	}

	private void Destroy(T go)
	{
		object obj = this.work;
		lock (obj)
		{
			this.work.Remove(go);
			this.free.Remove(go);
		}
	}

	public void Clear()
	{
		this.work.Clear();
		this.free.Clear();
	}

	private LinkedList<T> free = new LinkedList<T>();

	private List<T> work = new List<T>();

	private Func<T> create;
}
