using System;
using System.Collections.Generic;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class SettingWindow : BaseWindow
{
	private void Awake()
	{
		UISlider uislider = this.musicValue;
		uislider.onDragFinished = (UIProgressBar.OnDragFinished)Delegate.Combine(uislider.onDragFinished, new UIProgressBar.OnDragFinished(this.OnMusicSliderEnd));
		UISlider uislider2 = this.soundValue;
		uislider2.onDragFinished = (UIProgressBar.OnDragFinished)Delegate.Combine(uislider2.onDragFinished, new UIProgressBar.OnDragFinished(this.OnSoundSliderEnd));
		this.chineseBg.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnClickChinese);
		this.englishBg.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnClickEnglish);
		this.noralizeBg.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnClickfenbingNormalize);
		this.numberBg.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnClickfenbingNumber);
		this.TraditionalchineseBg.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnClickChineseTraditional);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnHttpNotice);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.settingToggleEvent = new EventDelegate(new EventDelegate.Callback(this.OnSettingToggle));
		this.announceToggleEvent = new EventDelegate(new EventDelegate.Callback(this.OnAnnounceToggle));
		this.settingToggle.onChange.Add(this.settingToggleEvent);
		this.announceToggle.onChange.Add(this.announceToggleEvent);
		this.musicSliderEvent = new EventDelegate(new EventDelegate.Callback(this.OnMusicChange));
		this.soundSliderEvent = new EventDelegate(new EventDelegate.Callback(this.OnSoundChange));
		if (this.IsCanShowPrivacy())
		{
			this.privateBtn.SetActive(true);
		}
		else
		{
			this.privateBtn.SetActive(false);
		}
		if (!string.IsNullOrEmpty(LocalPlayer.LocalNotice))
		{
			this.announceLabel.text = LocalPlayer.LocalNotice;
			return;
		}
		this.announceLabel.text = "没有设置公告内容喵~";
	}

	private bool IsCanShowPrivacy()
	{
		return UpgradeUtil.GetGameConfig().Oversea && false;
	}

	public override void OnHide()
	{
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
		for (int i = 0; i < this.sliderToggles.Length; i++)
		{
			this.sliderToggles[i].onChange.Add(this.sliderEvents[i]);
		}
		this.settingToggle.onChange.Remove(this.settingToggleEvent);
		this.announceToggle.onChange.Remove(this.announceToggleEvent);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnHttpNotice && NoticeRequest.Notice != null)
		{
			this.announceLabel.text = NoticeRequest.Notice.Content;
		}
	}

	public void OnSettingToggle()
	{
		if (this.settingToggle.value)
		{
			this.musicValue.onChange.Remove(this.musicSliderEvent);
			this.soundValue.onChange.Remove(this.soundSliderEvent);
			this.settingGo.SetActive(true);
			this.announceGo.SetActive(false);
			this.musicValue.onChange.Add(this.musicSliderEvent);
			this.soundValue.onChange.Add(this.soundSliderEvent);
			this.OnSettingShow();
		}
	}

	public void OnAnnounceToggle()
	{
		if (this.announceToggle.value)
		{
			this.settingGo.SetActive(false);
			this.announceGo.SetActive(true);
			this.OnAnnounceShow();
		}
	}

	public void OnSettingShow()
	{
		this.quitgame.SetActive(false);
		this.SetPage();
		this.quitgame.SetActive(true);
		this.sliderToggles[Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode].value = true;
		for (int i = 0; i < this.sliderToggles.Length; i++)
		{
			if (i != 0)
			{
				if (i != 1)
				{
					if (i == 2)
					{
						EventDelegate item = new EventDelegate(new EventDelegate.Callback(this.OnSliderChangeToRight));
						this.sliderEvents.Add(item);
						this.sliderToggles[2].onChange.Add(item);
					}
				}
				else
				{
					EventDelegate item = new EventDelegate(new EventDelegate.Callback(this.OnSliderChangeToLeft));
					this.sliderEvents.Add(item);
					this.sliderToggles[1].onChange.Add(item);
				}
			}
			else
			{
				EventDelegate item = new EventDelegate(new EventDelegate.Callback(this.OnSliderChangeToBottom));
				this.sliderEvents.Add(item);
				this.sliderToggles[0].onChange.Add(item);
			}
		}
	}

	public void OnAnnounceShow()
	{
		if (string.IsNullOrEmpty(SettingWindow.announceText))
		{
			if (!string.IsNullOrEmpty(LocalPlayer.LocalNotice))
			{
				SettingWindow.announceText = LocalPlayer.LocalNotice;
				return;
			}
			SettingWindow.announceText = "没有设置公告内容喵~";
		}
		this.announceLabel.text = SettingWindow.announceText;
	}

	public void SetPage()
	{
		float value = Solarmax.Singleton<LocalSettingStorage>.Get().music;
		this.musicValue.value = value;
		value = Solarmax.Singleton<LocalSettingStorage>.Get().sound;
		this.soundValue.value = value;
		this.chineseOn.gameObject.SetActive(false);
		this.TraditionalchineseOn.gameObject.SetActive(false);
		this.englishOn.gameObject.SetActive(false);
		int localLanguage = Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage;
		if (localLanguage != 40)
		{
			if (localLanguage == 41)
			{
				this.TraditionalchineseOn.gameObject.SetActive(true);
				goto IL_CA;
			}
			if (localLanguage != 6)
			{
				this.englishOn.gameObject.SetActive(true);
				goto IL_CA;
			}
		}
		this.chineseOn.gameObject.SetActive(true);
		IL_CA:
		for (int i = 0; i < this.effectToggles.Length; i++)
		{
			if (i == Solarmax.Singleton<LocalSettingStorage>.Get().effectLevel)
			{
				this.effectToggles[i].Set(true, false);
				this.SetToggleColor(this.effectToggles[i].gameObject, true);
			}
			else
			{
				this.effectToggles[i].Set(false, false);
				this.SetToggleColor(this.effectToggles[i].gameObject, false);
			}
		}
		if (Solarmax.Singleton<LocalSettingStorage>.Get().fightOption == 1)
		{
			this.numberOn.gameObject.SetActive(true);
			this.noralizeOn.gameObject.SetActive(false);
		}
		else
		{
			this.numberOn.gameObject.SetActive(false);
			this.noralizeOn.gameObject.SetActive(true);
		}
		string empty = string.Empty;
		this.version.text = Application.version;
	}

	private void SetToggleColor(GameObject go, bool on)
	{
		UISprite component = go.GetComponent<UISprite>();
		if (on)
		{
			component.color = this.btn_on_color;
		}
		else
		{
			component.color = this.btn_off_color;
		}
	}

	public void OnToggleValueChanged()
	{
		for (int i = 0; i < this.effectToggles.Length; i++)
		{
			if (this.effectToggles[i].value)
			{
				Solarmax.Singleton<LocalSettingStorage>.Get().effectLevel = i;
			}
			this.SetToggleColor(this.effectToggles[i].gameObject, this.effectToggles[i].value);
		}
	}

	public void OnFightToggleValueChanged()
	{
		for (int i = 0; i < this.fightOptionToggles.Length; i++)
		{
			if (this.fightOptionToggles[i].value)
			{
				Solarmax.Singleton<LocalSettingStorage>.Get().fightOption = i;
			}
			this.SetToggleColor(this.fightOptionToggles[i].gameObject, this.fightOptionToggles[i].value);
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	public void OnQuitGame()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
		{
			2,
			LanguageDataProvider.GetValue(2147),
			new EventDelegate(new EventDelegate.Callback(this.QuitGame))
		});
	}

	public void QuitGame()
	{
		Solarmax.Singleton<LocalStorageSystem>.Instance.NeedSaveToDisk();
		Solarmax.Singleton<LocalStorageSystem>.Instance.SaveStorage();
		Application.Quit();
	}

	public void OnClickChinese(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		SystemLanguage localLanguage = (SystemLanguage)Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage;
		if (localLanguage == SystemLanguage.Chinese)
		{
			return;
		}
		this.SelectLanguage = SystemLanguage.Chinese;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
		{
			2,
			LanguageDataProvider.GetValue(208),
			new EventDelegate(new EventDelegate.Callback(this.ModifyLanguange))
		});
	}

	public void OnClickEnglish(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		SystemLanguage localLanguage = (SystemLanguage)Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage;
		if (localLanguage == SystemLanguage.English)
		{
			return;
		}
		this.SelectLanguage = SystemLanguage.English;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
		{
			2,
			LanguageDataProvider.GetValue(208),
			new EventDelegate(new EventDelegate.Callback(this.ModifyLanguange))
		});
	}

	public void OnClickChineseTraditional(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		SystemLanguage localLanguage = (SystemLanguage)Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage;
		if (localLanguage == SystemLanguage.ChineseTraditional)
		{
			return;
		}
		this.SelectLanguage = SystemLanguage.ChineseTraditional;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
		{
			2,
			LanguageDataProvider.GetValue(208),
			new EventDelegate(new EventDelegate.Callback(this.ModifyLanguange))
		});
	}

	private void ModifyLanguange()
	{
		Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage = (int)this.SelectLanguage;
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalAccount(false);
		Solarmax.Singleton<LanguageDataProvider>.Get().Load();
		base.ReFreshLanguage();
		this.SetPage();
	}

	public void OnClickFeedback()
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		MiGameStatisticSDK.OpenWebView();
	}

	public void OnClickNoticeback()
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (!string.IsNullOrEmpty(LocalPlayer.LocalNotice))
		{
			this.announceLabel.text = LocalPlayer.LocalNotice;
			return;
		}
		this.announceLabel.text = "没有设置公告内容喵~";
	}

	private void OnSliderChangeToBottom()
	{
		if (this.sliderToggles[0].value)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode = 0;
		}
	}

	private void OnSliderChangeToLeft()
	{
		if (this.sliderToggles[1].value)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode = 1;
		}
	}

	private void OnSliderChangeToRight()
	{
		if (this.sliderToggles[2].value)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode = 2;
		}
	}

	public void OnMusicChange()
	{
		if (this.musicValue != null)
		{
			float value = this.musicValue.value;
			Solarmax.Singleton<AudioManger>.Get().ChangeBGVolume(value);
		}
	}

	public void OnMusicSliderEnd()
	{
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
	}

	public void OnSoundChange()
	{
		if (this.soundValue != null)
		{
			float value = this.soundValue.value;
			Solarmax.Singleton<AudioManger>.Get().ChangeSoundVolume(value);
		}
	}

	public void OnSoundSliderEnd()
	{
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
	}

	public void OnClickfenbingNormalize(GameObject go)
	{
		if (Solarmax.Singleton<LocalSettingStorage>.Get().fightOption != 0)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().fightOption = 0;
			this.SetPage();
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		}
	}

	public void OnClickfenbingNumber(GameObject go)
	{
		if (Solarmax.Singleton<LocalSettingStorage>.Get().fightOption != 1)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().fightOption = 1;
			this.SetPage();
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		}
	}

	public void OnOpenPrivactyDetails()
	{
		Debug.Log("OnOpenPrivactyDetails");
		MiGameLoginSDK.OpenPrivactyDetails();
	}

	public Animator animator;

	public UISlider musicValue;

	public UISlider soundValue;

	public UISprite noralizeBg;

	public UISprite noralizeOn;

	public UISprite numberBg;

	public UISprite numberOn;

	public UISprite chineseBg;

	public UISprite englishBg;

	public UISprite TraditionalchineseBg;

	public UISprite chineseOn;

	public UISprite TraditionalchineseOn;

	public UISprite englishOn;

	public UILabel chineseValue;

	public UILabel englishValue;

	public UIToggle[] effectToggles;

	public UIToggle[] fightOptionToggles;

	public UIToggle[] sliderToggles;

	public UILabel version;

	public UILabel ipLabel;

	public int entryType;

	public GameObject quitgame;

	public UIToggle settingToggle;

	public UIToggle announceToggle;

	public GameObject settingGo;

	public GameObject announceGo;

	public UILabel announceLabel;

	public GameObject privateBtn;

	private Color btn_on_color = new Color(0f, 0.81960785f, 1f, 1f);

	private Color btn_off_color = new Color(1f, 1f, 1f, 0.5f);

	private List<EventDelegate> sliderEvents = new List<EventDelegate>();

	private static string announceText;

	private EventDelegate settingToggleEvent;

	private EventDelegate announceToggleEvent;

	private EventDelegate musicSliderEvent;

	private EventDelegate soundSliderEvent;

	private SystemLanguage SelectLanguage = SystemLanguage.English;
}
