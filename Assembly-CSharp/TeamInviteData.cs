using System;
using System.Collections.Generic;
using NetMessage;

public class TeamInviteData : Solarmax.Singleton<TeamInviteData>
{
	public TeamInviteData()
	{
		this.Reset();
	}

	public int leaderId
	{
		get
		{
			if (this.teamPlayers.Count > 0)
			{
				return this.teamPlayers[0].userId;
			}
			return 0;
		}
	}

	public void Reset()
	{
		this.battleType = BattleType.Group2v2;
		this.version = 0;
		this.isLeader = false;
		this.teamPlayers.Clear();
	}

	public BattleType battleType;

	public int version;

	public bool isLeader;

	public List<SimplePlayerData> teamPlayers = new List<SimplePlayerData>();
}
