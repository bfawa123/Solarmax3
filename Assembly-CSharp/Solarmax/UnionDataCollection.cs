using System;
using System.Collections;

namespace Solarmax
{
	public class UnionDataCollection
	{
		public UnionDataCollection()
		{
			this.mCollection = new Hashtable();
		}

		public T Get<T>()
		{
			T result = default(T);
			Type typeFromHandle = typeof(T);
			if (this.mCollection.ContainsKey(typeFromHandle))
			{
				result = (T)((object)this.mCollection[typeFromHandle]);
			}
			return result;
		}

		public void Add<T>(T v)
		{
			if (v == null)
			{
				return;
			}
			Type type = v.GetType();
			if (this.mCollection.ContainsKey(type))
			{
				this.mCollection[type] = v;
			}
			else
			{
				this.mCollection.Add(type, v);
			}
		}

		public void Remove<T>(T v)
		{
			Type typeFromHandle = typeof(T);
			if (this.mCollection.ContainsKey(typeFromHandle))
			{
				this.RemoveType<T>();
			}
		}

		public void RemoveType<T>()
		{
			Type typeFromHandle = typeof(T);
			if (this.mCollection.ContainsKey(typeFromHandle))
			{
				this.mCollection.Remove(typeFromHandle);
			}
		}

		public void Clear()
		{
			this.mCollection.Clear();
		}

		public void Foreach(Callback<object> cb)
		{
			IEnumerator enumerator = this.mCollection.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object arg = enumerator.Current;
					cb(arg);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public T GetOrNew<T>() where T : new()
		{
			T t = this.Get<T>();
			if (t == null)
			{
				t = Activator.CreateInstance<T>();
				this.Add<T>(t);
			}
			return t;
		}

		private Hashtable mCollection;
	}
}
