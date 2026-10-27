using System;

public class Singleton<T> : Listenner
{
	public static T Get()
	{
		return Singleton<T>.SingletonCreator.instance;
	}

	private class SingletonCreator
	{
		internal static readonly T instance = Activator.CreateInstance<T>();
	}
}
