using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;

public class PlayerData
{
	public PlayerData()
	{
		this.power = 0;
	}

	public Team currentTeam { get; set; }

	public int skillPower { get; set; }

	public void Init(UserData data)
	{
		this.userId = data.userid;
		this.name = data.name;
		if (!string.IsNullOrEmpty(this.name))
		{
			Solarmax.Singleton<LocalAccountStorage>.Get().name = this.name;
			Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalAccount(false);
		}
		this.icon = data.icon;
		this.score = data.score;
		this.level = data.level;
		this.money = data.gold;
		this.mvp_count = data.mvp_count;
		this.battle_count = data.battle_count;
	}

	public void Init(PlayerData data)
	{
		this.userId = data.userId;
		this.name = data.name;
		this.icon = data.icon;
		this.score = data.score;
		this.level = data.level;
		this.money = data.money;
		this.power = data.power;
		this.unionName = string.Empty;
		this.skillPower = 0;
	}

	public void InitRace(IList<RaceData> raceDatas)
	{
		this.raceList.Clear();
		this.raceList.AddRange(raceDatas);
		this.raceList.Sort((RaceData args0, RaceData arg1) => args0.race.CompareTo(arg1.race));
	}

	public void CaleLookedAds(int timeStamp, int nNumServer)
	{
		DateTime d = new DateTime(1970, 1, 1);
		if (timeStamp != (Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).Days)
		{
			this.nLookAdsNum = 0;
		}
		else
		{
			this.nLookAdsNum = nNumServer;
		}
	}

	public void Reset()
	{
		this.userId = -1;
		this.name = string.Empty;
		this.icon = string.Empty;
		this.score = 0;
		this.level = 0;
		this.raceId = 0;
		this.unionName = string.Empty;
		this.skillPower = 0;
		this.castSkills.Clear();
		this.singleFightLevel = string.Empty;
		this.singleFightNext = false;
		this.raceList.Clear();
		for (int i = 0; i < this.clientStorages.Length; i++)
		{
			this.clientStorages[i] = string.Empty;
		}
	}

	public int userId;

	public string name;

	public string icon;

	public int score;

	public int level;

	public int money;

	public int destroyNum;

	public string unionName;

	public List<int> castSkills = new List<int>();

	public int raceId;

	public int raceLevel;

	public int[] raceSkillId = new int[6];

	public string singleFightLevel = string.Empty;

	public bool singleFightNext;

	public int mvp_count;

	public int battle_count;

	public long RegisteredtimeStamp;

	public ChessItem[] chesses = new ChessItem[ChessItem.CHESS_MAX];

	public int timechestid;

	public int battlechestid;

	public long timechest;

	public long timechestcost;

	public int curbattlechest;

	public int maxbattlechest;

	public Pack pack = new Pack();

	public List<RaceData> raceList = new List<RaceData>();

	public string[] clientStorages = new string[128];

	public int power;

	public int nLookAdsNum;

	public Dictionary<AdChannel, AdConfig> adchannel;

	private const int DAYSENCODE = 86400;
}
