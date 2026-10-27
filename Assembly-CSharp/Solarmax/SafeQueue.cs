using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class SafeQueue<T>
	{
		public SafeQueue()
		{
			this.syncRoot = new object();
			this.queue = new Queue<T>();
		}

		public SafeQueue(IEnumerable<T> collection)
		{
			this.syncRoot = new object();
			this.queue = new Queue<T>(collection);
		}

		public SafeQueue(int capacity)
		{
			this.syncRoot = new object();
			this.queue = new Queue<T>(capacity);
		}

		internal object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		public void Enqueue(T t)
		{
			object obj = this.SyncRoot;
			lock (obj)
			{
				this.queue.Enqueue(t);
			}
		}

		public T Dequeue()
		{
			T result = default(T);
			object obj = this.SyncRoot;
			lock (obj)
			{
				result = this.queue.Dequeue();
			}
			return result;
		}

		public void Clear()
		{
			object obj = this.SyncRoot;
			lock (obj)
			{
				this.queue.Clear();
			}
		}

		public bool Contains(T t)
		{
			bool result = false;
			object obj = this.SyncRoot;
			lock (obj)
			{
				result = this.queue.Contains(t);
			}
			return result;
		}

		private Queue<T> queue;

		private object syncRoot;
	}
}
