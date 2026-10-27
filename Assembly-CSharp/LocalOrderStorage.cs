using System;
using Solarmax;

public class LocalOrderStorage : Solarmax.Singleton<LocalOrderStorage>, ILocalStorage
{
	public string Name()
	{
		return "LocalOrder1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		this.OrderCount = Solarmax.Singleton<OrderManager>.Get().padding.Count + Solarmax.Singleton<OrderManager>.Get().history.Count;
		manager.PutInt("OrderCount", this.OrderCount);
		int num = 0;
		this.OrderCount = Solarmax.Singleton<OrderManager>.Get().padding.Count;
		for (int i = 0; i < this.OrderCount; i++)
		{
			manager.PutString("OrderID" + ++num, Solarmax.Singleton<OrderManager>.Get().padding[i]);
		}
		this.OrderCount = Solarmax.Singleton<OrderManager>.Get().history.Count;
		for (int j = 0; j < this.OrderCount; j++)
		{
			manager.PutString("OrderID" + ++num, Solarmax.Singleton<OrderManager>.Get().history[j]);
		}
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		this.OrderCount = manager.GetInt("OrderCount", 0);
		int num = 0;
		for (int i = 0; i < this.OrderCount; i++)
		{
			string @string = manager.GetString("OrderID" + ++num, string.Empty);
			Solarmax.Singleton<OrderManager>.Get().AddhHstoryOrder(@string);
		}
	}

	public void Clear(LocalStorageSystem manager)
	{
	}

	private int OrderCount;

	public int ver = 1;
}
