using System;
using NetMessage;

[Serializable]
public class SimplePlayerData
{
	public SimplePlayerData()
	{
	}

	public SimplePlayerData(SimplePlayerData data)
	{
		this.userId = data.userId;
		this.name = data.name;
		this.icon = data.icon;
		this.score = data.score;
		this.level = data.level;
		this.online = data.online;
		this.onBattle = data.onBattle;
		this.onStats = data.onStats;
		this.scoreMod = data.scoreMod;
		this.destroyNum = data.destroyNum;
		this.surviveNum = data.surviveNum;
		this.raceId = data.raceId;
		this.power = data.power;
		this.money = data.money;
	}

	public void Init(UserData sud)
	{
		this.userId = sud.userid;
		this.name = sud.name;
		this.icon = sud.icon;
		this.score = sud.score;
		this.level = sud.level;
		this.online = false;
		this.onStats = 0;
		if (sud.online)
		{
			this.online = sud.online;
			this.onStats = 2;
		}
		this.onBattle = false;
		if (sud.onBattle)
		{
			this.onBattle = sud.onBattle;
			this.onStats = 1;
		}
		this.scoreMod = sud.score_mod;
		this.destroyNum = sud.destroy_num;
		this.surviveNum = sud.survive_num;
	}

	public int userId;

	public string name;

	public string icon;

	public int score;

	public int level;

	public bool online;

	public bool onBattle;

	public int onStats;

	public int scoreMod;

	public int destroyNum;

	public int surviveNum;

	public int raceId;

	public int power;

	public int money;
}
