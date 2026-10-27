using System;
using System.Collections.Generic;
using NetMessage;

[Serializable]
public class BattleReportData
{
	public void Init(BattleReport data)
	{
		this.id = data.id;
		this.playCount = data.play_count;
		this.matchType = data.match_type;
		this.subType = data.sub_type;
		this.playerList = new List<SimplePlayerData>();
		for (int i = 0; i < data.data.Count; i++)
		{
			SimplePlayerData simplePlayerData = new SimplePlayerData();
			simplePlayerData.Init(data.data[i]);
			if (simplePlayerData.userId < 0)
			{
				simplePlayerData.name = AIManager.GetAIName(simplePlayerData.userId);
				simplePlayerData.icon = AIManager.GetAIIcon(simplePlayerData.userId);
			}
			this.playerList.Add(simplePlayerData);
		}
		this.groupList = new List<int>();
		for (int j = 0; j < this.playerList.Count; j++)
		{
			this.groupList.Add(-1);
		}
		if (!string.IsNullOrEmpty(data.group))
		{
			this.group = data.group;
			if (!string.IsNullOrEmpty(this.group))
			{
				string[] array = this.group.Split(new char[]
				{
					'|'
				});
				for (int k = 0; k < array.Length; k++)
				{
					string[] array2 = array[k].Split(new char[]
					{
						','
					});
					for (int l = 0; l < array2.Length; l++)
					{
						int index = int.Parse(array2[l]);
						this.groupList[index] = k;
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(data.match_id))
		{
			this.matchId = data.match_id;
		}
		this.time = data.time;
	}

	public long id;

	public int playCount;

	public string group;

	public string matchId;

	public List<SimplePlayerData> playerList;

	public List<int> groupList;

	public long time;

	public MatchType matchType;

	public CooperationType subType = CooperationType.CT_Null;
}
