using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CultureInfoDict : Solarmax.Singleton<CultureInfoDict>
{
	public CultureInfoDict()
	{
		this.dictCultureInfos = new Dictionary<string, CultureInfo>();
		CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
		foreach (CultureInfo cultureInfo in cultures)
		{
			RegionInfo regionInfo = new RegionInfo(cultureInfo.LCID);
			string isocurrencySymbol = regionInfo.ISOCurrencySymbol;
			if (!this.dictCultureInfos.ContainsKey(isocurrencySymbol))
			{
				this.dictCultureInfos.Add(isocurrencySymbol, cultureInfo);
			}
		}
	}

	public string FormatCurrency(float v, string isoCurrencySymbol)
	{
		CultureInfo provider;
		if (!this.dictCultureInfos.TryGetValue(isoCurrencySymbol, out provider))
		{
			Debug.LogErrorFormat("Can not find CultureInfo of ISOCurrencySymbol {0}!", new object[]
			{
				isoCurrencySymbol
			});
			return null;
		}
		return v.ToString("C2", provider);
	}

	public void Test()
	{
		float[] array = new float[]
		{
			1.234f,
			1f,
			1234.56f,
			1234.567f,
			0.1f
		};
		string[] array2 = new string[]
		{
			"CNY",
			"HKD",
			"USD"
		};
		for (int i = 0; i < array2.Length; i++)
		{
			for (int j = 0; j < array.Length; j++)
			{
				Debug.LogFormat("{0}: {1}", new object[]
				{
					array2[i],
					this.FormatCurrency(array[j], array2[i])
				});
			}
		}
	}

	private readonly Dictionary<string, CultureInfo> dictCultureInfos;
}
