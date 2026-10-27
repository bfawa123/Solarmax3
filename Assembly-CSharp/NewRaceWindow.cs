using System;
using Solarmax;

public class NewRaceWindow : BaseWindow
{
	private void Awake()
	{
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnSCRaceNotify);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnHide()
	{
		GuideManager.ClearGuideData();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnSCRaceNotify)
		{
			int nRaceID = (int)args[0];
			this.OnRaceNotify(nRaceID);
		}
	}

	public void OnClickOK()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("NewRaceWindow");
	}

	private void OnRaceNotify(int nRaceID)
	{
		RaceConfig data = Solarmax.Singleton<RaceConfigProvider>.Instance.GetData(nRaceID);
		if (data != null)
		{
			this.RaceSprite.spriteName = data.icon;
			string value = LanguageDataProvider.GetValue(303);
			this.RaceName.text = string.Format(value, data.name);
		}
	}

	public UISprite RaceSprite;

	public UILabel RaceName;
}
