using System;

namespace Solarmax
{
	public class Singleton<T> where T : new()
	{
		private static T GetInstance()
		{
			if (Singleton<T>.instance == null)
			{
				object @lock = Singleton<T>.m_lock;
				lock (@lock)
				{
					if (Singleton<T>.instance == null)
					{
						Singleton<T>.instance = Activator.CreateInstance<T>();
					}
				}
			}
			return (T)((object)Singleton<T>.instance);
		}

		public static T Instance
		{
			get
			{
				return Singleton<T>.GetInstance();
			}
		}

		public static T Get()
		{
			return Singleton<T>.GetInstance();
		}

		private static volatile object instance = null;

		private static object m_lock = new object();
	}
}
