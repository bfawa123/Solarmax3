using System;
using NetMessage;

public class FriendSimplePlayer
{
	public void Init(PveRankReport rp)
	{
		this.account = rp.accountid;
		this.name = rp.playerName;
		this.icon = rp.playerIcon;
		this.score = rp.score;
	}

	public string account;

	public string name;

	public string icon;

	public int score;
}
