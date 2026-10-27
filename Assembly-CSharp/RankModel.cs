using System;
using System.Collections.Generic;

public class RankModel : Solarmax.Singleton<RankModel>
{
	public void Init()
	{
		for (int i = 0; i < 3; i++)
		{
			this.dicRankType[(RankModel.RankType)i] = new List<PlayerData>();
		}
	}

	public void AddCurrentTypeData(List<PlayerData> datas)
	{
		if (datas.Count == 0)
		{
			return;
		}
		this.dicRankType[this.currentType].AddRange(datas);
	}

	public List<PlayerData> GetCurrentTypeData()
	{
		return this.dicRankType[this.currentType];
	}

	public int GetCurrentTypeDataCount()
	{
		return this.dicRankType[this.currentType].Count;
	}

	public void Clear()
	{
		foreach (KeyValuePair<RankModel.RankType, List<PlayerData>> keyValuePair in this.dicRankType)
		{
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.Clear();
			}
		}
		this.dicRankType.Clear();
	}

	public RankModel.RankType currentType;

	private Dictionary<RankModel.RankType, List<PlayerData>> dicRankType = new Dictionary<RankModel.RankType, List<PlayerData>>();

	public enum RankType
	{
		PVP,
		Challenage,
		Star,
		None
	}
}
