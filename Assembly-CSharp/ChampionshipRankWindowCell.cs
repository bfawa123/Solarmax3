using System;
using NetMessage;
using UnityEngine;

public class ChampionshipRankWindowCell : MonoBehaviour
{
	public void SetInfo(int index, MemberInfo member)
	{
		this.rankIndex = index;
		this.data = member;
		this.icon.picUrl = this.data.icon;
		this.nameLabel.text = this.data.name;
		this.score.text = this.data.score.ToString();
		this.rank.text = (this.rankIndex + 1).ToString();
	}

	public NetTexture icon;

	public UILabel nameLabel;

	public UILabel score;

	public UILabel rank;

	private MemberInfo data;

	private int rankIndex;
}
