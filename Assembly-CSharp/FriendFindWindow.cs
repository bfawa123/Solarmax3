using System;
using NetMessage;
using Solarmax;

public class FriendFindWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnFriendSearchResultShow);
		base.RegisterEvent(EventId.OnFriendFollowResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnHide()
	{
		this.curUserID = -1;
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnFriendSearchResultShow)
		{
			this.playerData = (args[0] as SimplePlayerData);
			bool flag = (bool)args[1];
			int num = (int)args[2];
			int num2 = (int)args[3];
			int num3 = (int)args[4];
			int num4 = (int)args[5];
			int num5 = (int)args[6];
			this.IsFromMainUI = (bool)args[7];
			int num6 = (int)args[8];
			this.curUserID = this.playerData.userId;
			this.scoreLabel.text = this.playerData.score.ToString();
			this.nameLabel.text = this.playerData.name;
			this.guanzhu.text = num.ToString();
			if (num4 <= 0)
			{
				this.shenglv.text = "0%";
				this.changci.text = "0";
				this.Mvp.text = "0";
			}
			else
			{
				this.changci.text = num4.ToString();
				this.Mvp.text = num3.ToString();
				double value = (double)num3 / ((double)num4 * 1.0) * 100.0;
				this.shenglv.text = Math.Round(value, 1).ToString() + "%";
			}
			this.portrait.Load(this.playerData.icon, null, null);
			bool flag2 = Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(this.playerData);
			if (flag2)
			{
				this.cellType = 0;
				this.followBtn.gameObject.SetActive(false);
			}
			else
			{
				this.cellType = 2;
				this.followBtn.isEnabled = true;
			}
			if (this.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
			{
				this.followBtn.gameObject.SetActive(false);
			}
			this.starLabel.text = num5.ToString();
			this.challengeLabel.text = num6.ToString();
		}
		else if (eventId == EventId.OnFriendFollowResult)
		{
			int num7 = (int)args[0];
			bool flag3 = (bool)args[1];
			ErrCode errCode = (ErrCode)args[2];
			if (num7 != this.playerData.userId)
			{
				return;
			}
			if (errCode == ErrCode.EC_Ok)
			{
				if (flag3)
				{
					this.followBtn.isEnabled = false;
					this.followBtnLabel.text = LanguageDataProvider.GetValue(808);
				}
				else
				{
					this.followBtn.isEnabled = true;
					this.followBtnLabel.text = LanguageDataProvider.GetValue(806);
				}
			}
			else
			{
				string text = (!flag3) ? LanguageDataProvider.GetValue(810) : LanguageDataProvider.GetValue(809);
				text = string.Format("{0} userId: {1} 失败！", text, num7);
				Tips.Make(Tips.TipsType.FlowUp, text, 1f);
			}
		}
	}

	public void OnFollowClick()
	{
		if (this.cellType == 0 || this.cellType == 1)
		{
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
			string text = string.Format(LanguageDataProvider.GetValue(812), this.nameLabel.text);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
			{
				3,
				text,
				new EventDelegate(new EventDelegate.Callback(this.FollowClick))
			});
		}
		if (this.cellType == 2)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.playerData.userId, true);
			Solarmax.Singleton<UISystem>.Get().HideWindow("FriendFindWindow");
		}
	}

	private void FollowClick()
	{
		if (this.cellType == 0 || this.cellType == 1)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.playerData.userId, false);
		}
		if (this.cellType == 2)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.playerData.userId, true);
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("FriendFindWindow");
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("FriendFindWindow");
	}

	public UILabel scoreLabel;

	public UILabel nameLabel;

	public UILabel guanzhu;

	public UILabel fensi;

	public NetTexture icon;

	public UIButton followBtn;

	public UILabel followBtnLabel;

	public UILabel changci;

	public UILabel Mvp;

	public UILabel shenglv;

	public PortraitTemplate portrait;

	public UILabel starLabel;

	public UILabel challengeLabel;

	private bool IsFromMainUI;

	private int curUserID = -1;

	private SimplePlayerData playerData;

	private int cellType;
}
