using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			if (MonoSingleton<T>.s_IsDestroyed)
			{
				return (T)((object)null);
			}
			if (MonoSingleton<T>.s_Instance == null)
			{
				MonoSingleton<T>.s_Instance = (UnityEngine.Object.FindObjectOfType(typeof(T)) as T);
				if (MonoSingleton<T>.s_Instance == null)
				{
					GameObject gameObject = new GameObject(typeof(T).Name);
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					MonoSingleton<T>.s_Instance = (gameObject.AddComponent(typeof(T)) as T);
				}
			}
			return MonoSingleton<T>.s_Instance;
		}
	}

	protected virtual void OnDestroy()
	{
		if (MonoSingleton<T>.s_Instance)
		{
			UnityEngine.Object.Destroy(MonoSingleton<T>.s_Instance);
		}
		MonoSingleton<T>.s_Instance = (T)((object)null);
		MonoSingleton<T>.s_IsDestroyed = true;
	}

	private static T s_Instance;

	private static bool s_IsDestroyed;
}
