using System;
using Solarmax;

public class FriendNotifyWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnFriendNotifyResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		base.gameObject.SetActive(false);
		base.Invoke("AutoHide", 3f);
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnFriendNotifyResult)
		{
			this.data = (SimplePlayerData)args[0];
			this.icon.picUrl = this.data.icon;
			this.notice.text = string.Format(LanguageDataProvider.GetValue(816), this.data.name);
			this.score.text = this.data.score.ToString();
			base.gameObject.SetActive(true);
		}
	}

	public void OnCareClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.data.userId, true);
		this.OnIgnoreClick();
	}

	public void OnIgnoreClick()
	{
		base.CancelInvoke("AutoHide");
		this.AutoHide();
	}

	private void AutoHide()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("FriendNotifyWindow");
	}

	public NetTexture icon;

	public UILabel notice;

	public UILabel score;

	public UIButton careBtn;

	public UIButton ignoreBtn;

	private SimplePlayerData data;
}
