using System;

namespace Solarmax
{
	public class IPoolObject<T> where T : IPoolObject<T>, new()
	{
		public IPoolObject()
		{
			this.mAssociatedPool = null;
		}

		public void InitPool(SimplePool<T> associatedPool)
		{
			this.mAssociatedPool = associatedPool;
		}

		protected void Recycle(T t)
		{
			this.mAssociatedPool.Recycle(t);
		}

		public virtual void OnRecycle()
		{
		}

		private SimplePool<T> mAssociatedPool;
	}
}
