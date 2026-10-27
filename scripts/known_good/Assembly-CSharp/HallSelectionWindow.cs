using System;
using NetMessage;
using Solarmax;

public class HallSelectionWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		return true;
	}

	public override void Release()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	public override void OnHide()
	{
	}

	public void OnBnPvpCooperationClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		EventDelegate eventDelegate = new EventDelegate(new EventDelegate.Callback(this.OpenCooperation2));
		EventDelegate eventDelegate2 = new EventDelegate(new EventDelegate.Callback(this.OpenCooperation3));
		EventDelegate eventDelegate3 = new EventDelegate(new EventDelegate.Callback(this.OpenCooperation4));
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
		{
			6,
			LanguageDataProvider.GetValue(7018),
			eventDelegate,
			eventDelegate2,
			eventDelegate3,
			LanguageDataProvider.GetValue(7019),
			LanguageDataProvider.GetValue(7020),
			LanguageDataProvider.GetValue(7021)
		});
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	private void OpenCooperationInternal(int playerNum)
	{
		Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(playerNum);
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HallWindow";
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationRoomWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBackWindowName, new object[]
		{
			"HallWindow"
		});
	}

	private void OpenCooperation2()
	{
		this.OpenCooperationInternal(2);
	}

	private void OpenCooperation3()
	{
		this.OpenCooperationInternal(3);
	}

	private void OpenCooperation4()
	{
		this.OpenCooperationInternal(4);
	}

	public void OnBnPvpDuelClick()
	{
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HallWindow";
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_1v1, 2, false, string.Empty, -1, string.Empty, false);
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnBnPvpMeleeClick()
	{
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HallWindow";
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_1v1v1, 3, false, string.Empty, -1, string.Empty, false);
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnBnPvpRoomClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CreateRoomWindow");
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnBackClick()
	{
		Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HallWindow");
	}
}
