using System;
using System.Collections.Generic;

public class HeadEffectManager
{
	public static AnyPool<IPoolGo> GetEffectPool(string name)
	{
		if (!HeadEffectManager.poolDic.ContainsKey(name))
		{
			Func<IPoolGo> create = () => new HeadEffect().CreateEffect(name);
			AnyPool<IPoolGo> value = new AnyPool<IPoolGo>(create);
			HeadEffectManager.poolDic.Add(name, value);
		}
		return HeadEffectManager.poolDic[name];
	}

	private static Dictionary<string, AnyPool<IPoolGo>> poolDic = new Dictionary<string, AnyPool<IPoolGo>>();
}
