using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class TeamWindowCell : MonoBehaviour
{
	public void SetInfo(SimplePlayerData spd)
	{
		this.playerData = spd;
		this.icon.picUrl = this.playerData.icon;
		this.nameLabel.text = this.playerData.name;
		this.score.text = this.playerData.score.ToString();
		if (this.playerData.online && !this.playerData.onBattle)
		{
			this.inviteBtn.isEnabled = true;
			this.inviteBtnLabel.text = LanguageDataProvider.GetValue(411);
		}
		else
		{
			this.inviteBtn.isEnabled = false;
			if (!this.playerData.online)
			{
				this.inviteBtnLabel.text = LanguageDataProvider.GetValue(412);
			}
			if (this.playerData.onBattle)
			{
				this.inviteBtnLabel.text = LanguageDataProvider.GetValue(413);
			}
		}
	}

	public void OnInviteSend()
	{
		this.inviteBtn.isEnabled = false;
		this.inviteBtnLabel.text = LanguageDataProvider.GetValue(414);
	}

	public void OnInviteResponse(ErrCode code)
	{
		if (code == ErrCode.EC_Ok)
		{
			this.inviteBtnLabel.text = LanguageDataProvider.GetValue(415);
		}
		else
		{
			this.inviteBtnLabel.text = LanguageDataProvider.GetValue(416);
		}
	}

	public void OnInviteClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.TeamInvite(this.playerData.userId);
	}

	public NetTexture icon;

	public UILabel nameLabel;

	public UILabel score;

	public UIButton inviteBtn;

	public UILabel inviteBtnLabel;

	private SimplePlayerData playerData;
}
