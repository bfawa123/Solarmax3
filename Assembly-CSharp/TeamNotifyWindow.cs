using System;
using NetMessage;
using Solarmax;

public class TeamNotifyWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnTeamInviteRequest);
		base.RegisterEvent(EventId.OnTeamInviteResponse);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		base.Invoke("AutoHide", 3f);
	}

	public override void OnHide()
	{
		base.CancelInvoke("AutoHide");
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnTeamInviteRequest)
		{
			BattleType battleType = (BattleType)args[0];
			this.leader = (SimplePlayerData)args[1];
			this.timestamp = (int)args[2];
			this.icon.picUrl = this.leader.icon;
			if (battleType == BattleType.Group2v2)
			{
				this.tip.text = string.Format(LanguageDataProvider.GetValue(409), this.leader.name);
			}
			else if (battleType == BattleType.Group3v3)
			{
				this.tip.text = string.Format(LanguageDataProvider.GetValue(410), this.leader.name);
			}
			this.score.text = this.leader.score.ToString();
		}
		else if (eventId == EventId.OnTeamInviteResponse)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_Ok)
			{
				this.AutoHide();
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("TeamWindow");
			}
			else if (errCode == ErrCode.EC_TeamNoExist)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(915), 1f);
			}
			else if (errCode == ErrCode.EC_TeamFull)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(916), 1f);
			}
		}
	}

	public void OnYesClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.TeamInviteResponse(true, this.leader.userId, this.timestamp);
	}

	public void OnNoClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.TeamInviteResponse(false, this.leader.userId, this.timestamp);
	}

	public void AutoHide()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("TeamNotifyWindow");
	}

	public NetTexture icon;

	public UILabel tip;

	public UILabel score;

	private SimplePlayerData leader;

	private int timestamp;
}
