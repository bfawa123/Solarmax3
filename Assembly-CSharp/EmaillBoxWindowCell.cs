using System;
using Solarmax;
using UnityEngine;

public class EmaillBoxWindowCell : MonoBehaviour
{
	public void SetEamilBoxInfo(SimplePlayerData data)
	{
		this.playerData = data;
		this.icon.spriteName = this.playerData.icon;
		this.nameLabel.text = this.playerData.name;
		this.scoreLabel.text = this.playerData.score.ToString();
		if (data.online)
		{
			this.onlineLabel.text = LanguageDataProvider.GetValue(804);
		}
		else
		{
			this.onlineLabel.text = LanguageDataProvider.GetValue(805);
		}
		this.careBtn.gameObject.SetActive(true);
	}

	public void OnCareClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.playerData.userId, true);
	}

	public void OnCareResult(bool cared)
	{
	}

	public UISprite icon;

	public UILabel nameLabel;

	public UILabel onlineLabel;

	public UILabel descLabel;

	public UILabel scoreLabel;

	public UIButton careBtn;

	public SimplePlayerData playerData;
}
