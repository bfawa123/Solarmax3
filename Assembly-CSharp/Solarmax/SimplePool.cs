using System;
using System.Collections.Generic;
using System.Linq;

namespace Solarmax
{
	public class SimplePool<T> where T : IPoolObject<T>, new()
	{
		public SimplePool()
		{
			this.mAsyncLocker = new object();
			this.mPoolSize = 0;
			this.mFreeObjects = new Queue<T>();
			this.mBusyObjects = new List<T>();
		}

		public void Init(int size)
		{
			for (int i = 0; i < this.mPoolSize; i++)
			{
				this.mFreeObjects.Enqueue(this.NewOne());
			}
		}

		private T NewOne()
		{
			T t = Activator.CreateInstance<T>();
			if (t != null)
			{
				this.mPoolSize++;
				t.InitPool(this);
			}
			return t;
		}

		private DerivedT NewOne<DerivedT>() where DerivedT : T, new()
		{
			DerivedT derivedT = Activator.CreateInstance<DerivedT>();
			if (derivedT != null)
			{
				this.mPoolSize++;
				derivedT.InitPool(this);
			}
			return derivedT;
		}

		public virtual T Alloc()
		{
			object obj = this.mAsyncLocker;
			T result;
			lock (obj)
			{
				T t = (T)((object)null);
				if (this.mFreeObjects.Count > 0)
				{
					t = this.mFreeObjects.Dequeue();
				}
				else
				{
					t = this.NewOne();
				}
				this.mBusyObjects.Add(t);
				result = t;
			}
			return result;
		}

		public virtual DerivedT Alloc<DerivedT>() where DerivedT : T, new()
		{
			object obj = this.mAsyncLocker;
			DerivedT result;
			lock (obj)
			{
				DerivedT derivedT = (DerivedT)((object)null);
				if (this.mFreeObjects.Count > 0)
				{
					derivedT = (this.mFreeObjects.Dequeue() as DerivedT);
				}
				else
				{
					derivedT = this.NewOne<DerivedT>();
				}
				this.mBusyObjects.Add((T)((object)derivedT));
				result = derivedT;
			}
			return result;
		}

		public virtual void Recycle(T t)
		{
			object obj = this.mAsyncLocker;
			lock (obj)
			{
				if (t != null)
				{
					t.OnRecycle();
					this.mBusyObjects.Remove(t);
					this.mFreeObjects.Enqueue(t);
					this.mPoolSize--;
				}
			}
		}

		public int GetSize()
		{
			return this.mPoolSize;
		}

		public int GetFreeCount()
		{
			return this.mFreeObjects.Count;
		}

		public List<T> GetAllObjects()
		{
			List<T> list = this.mFreeObjects.ToList<T>();
			list.AddRange(this.mBusyObjects);
			return list;
		}

		public void Clear()
		{
			this.mFreeObjects.Clear();
			this.mBusyObjects.Clear();
		}

		private object mAsyncLocker;

		private int mPoolSize;

		protected Queue<T> mFreeObjects;

		protected List<T> mBusyObjects;
	}
}
