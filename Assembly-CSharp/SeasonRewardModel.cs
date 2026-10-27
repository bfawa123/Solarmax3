using System;
using System.Collections.Generic;
using Solarmax;

public class SeasonRewardModel : Solarmax.Singleton<SeasonRewardModel>
{
	public void Init(int id, int score, List<bool> status)
	{
		if (Solarmax.Singleton<SeasonRewardProvider>.Get().dataList.ContainsKey(id.ToString()))
		{
			this.outSeason = false;
			bool flag = this.seasonId > 0 && this.seasonId != id;
			this.seasonId = id;
			this.seasonMaxScore = score;
			SeasonRewardConfig seasonRewardConfig = Solarmax.Singleton<SeasonRewardProvider>.Get().dataList[this.seasonId.ToString()];
			if (flag)
			{
				this.seasonMaxScore = 0;
				for (int i = 1; i <= seasonRewardConfig.count; i++)
				{
					this.rewardStatus[i.ToString()] = status[i - 1];
				}
				return;
			}
		}
		else
		{
			this.outSeason = true;
		}
	}

	public void Init(int id)
	{
		if (Solarmax.Singleton<SeasonRewardProvider>.Get().dataList.ContainsKey(id.ToString()))
		{
			this.outSeason = false;
			bool flag = this.seasonId > 0 && this.seasonId != id;
			this.seasonId = id;
			SeasonRewardConfig seasonRewardConfig = Solarmax.Singleton<SeasonRewardProvider>.Get().dataList[this.seasonId.ToString()];
			if (flag)
			{
				this.seasonMaxScore = 0;
				for (int i = 1; i <= seasonRewardConfig.count; i++)
				{
					this.rewardStatus[i.ToString()] = false;
				}
			}
		}
		else
		{
			this.outSeason = true;
		}
	}

	public int seasonId = -1;

	public int seasonMaxScore;

	public Dictionary<string, bool> rewardStatus = new Dictionary<string, bool>();

	public int claimRewardType;

	public bool outSeason;
}
