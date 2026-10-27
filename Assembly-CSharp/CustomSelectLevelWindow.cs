using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class CustomSelectLevelWindow : BaseWindow
{
	private void Awake()
	{
		this.moveTrigger.onClick = new UIEventListener.VoidDelegate(this.OnTriggerClick);
		this.moveTrigger.onDragStart = new UIEventListener.VoidDelegate(this.OnTriggerDragStart);
		this.moveTrigger.onDrag = new UIEventListener.VectorDelegate(this.OnTriggerDrag);
		this.moveTrigger.onDragEnd = new UIEventListener.VoidDelegate(this.OnTriggerDragEnd);
		this.moveTrigger.onPress = new UIEventListener.BoolDelegate(this.OnTriggerPress);
		this.sensitive *= Solarmax.Singleton<UISystem>.Get().GetNGUIRoot().pixelSizeAdjustment;
		this.changePageLength = 240f;
		this.logo.SetActive(false);
		this.mapShow.transform.localScale = Vector3.one * 0.6f;
	}

	private void Update()
	{
		if (this.mouseDown)
		{
			this.deltaScroll.x = this.deltaScroll.x - this.deltaScroll.x * 0.5f;
		}
		else
		{
			this.deltaScroll.x = this.deltaScroll.x - this.deltaScroll.x * 0.025f;
		}
		float num = this.changePageLength / 2f;
		this.localpos = this.numParent.transform.localPosition;
		this.localpos.x = this.localpos.x + this.deltaScroll.x;
		this.numParent.transform.localPosition = this.localpos;
		if (!this.mouseDown && Math.Abs(this.deltaScroll.x) < 2f)
		{
			this.deltaScroll.x = 0f;
			int num2 = Math.Abs((int)((this.numParent.transform.localPosition.x - num) / this.changePageLength));
			float num3 = (float)(-(float)num2) * this.changePageLength - this.numParent.transform.localPosition.x;
			this.localpos.x = this.localpos.x + num3 * 0.1f;
			this.numParent.transform.localPosition = this.localpos;
		}
		if (this.numParent.transform.localPosition.x > 0f)
		{
			this.localpos.x = 0f;
			this.numParent.transform.localPosition = this.localpos;
		}
		if (this.numParent.transform.localPosition.x < (float)(-(float)this.MapIndexMax) * this.changePageLength)
		{
			this.localpos.x = (float)(-(float)this.MapIndexMax) * this.changePageLength;
			this.numParent.transform.localPosition = this.localpos;
		}
		if (!this.bIsShowMap)
		{
			float num4 = this.changePageLength * (float)(-(float)CustomSelectLevelWindow.selectMapIndex);
			this.currentAlpha = Mathf.Abs(this.numParent.transform.localPosition.x - num4) / num;
			if (this.currentAlpha > 1f)
			{
				this.currentAlpha -= 1f;
			}
			else
			{
				this.currentAlpha = 1f - this.currentAlpha;
			}
			if (this.currentAlpha > 1f)
			{
				this.currentAlpha = 1f;
			}
			this.mapShow.ManualFade(this.currentAlpha);
		}
		int num5 = Math.Abs((int)((this.numParent.transform.localPosition.x - num) / this.changePageLength));
		if (num5 != CustomSelectLevelWindow.selectMapIndex && num5 >= 0 && num5 <= this.MapIndexMax)
		{
			this.ShowMap(num5, true, false);
			this.mapShow.ManualFade(0f);
			this.SetSubNumAlpha(this.numParent.transform.localPosition.x);
		}
		float num6 = Math.Abs(this.numParent.transform.localPosition.x) / (this.changePageLength * (float)(this.mapList.Count - 1));
		float num7 = num6 * -78.66f - this.bgParent.transform.localPosition.x;
		this.localpos = this.bgParent.transform.localPosition;
		this.localpos.x = this.localpos.x + num7 * 0.05f;
		this.bgParent.transform.localPosition = this.localpos;
	}

	private void ScrollToLevel(int index, float duration)
	{
		this.bIsShowMap = false;
		float x = -this.changePageLength * (float)index;
		Vector3 localPosition = this.numParent.transform.localPosition;
		Vector3 localPosition2 = this.numParent.transform.localPosition;
		localPosition.x = x;
		TweenPosition tweenPosition = this.numParent.GetComponent<TweenPosition>();
		if (tweenPosition == null)
		{
			tweenPosition = this.numParent.AddComponent<TweenPosition>();
		}
		tweenPosition.ResetToBeginning();
		this.numParent.transform.localPosition = localPosition2;
		tweenPosition.from = this.numParent.transform.localPosition;
		tweenPosition.to = localPosition;
		tweenPosition.duration = duration;
		tweenPosition.Play(true);
	}

	private int GetClosestIndex(float touchX, float touchY)
	{
		return -1;
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnRenameFinished);
		base.RegisterEvent(EventId.OnStartSingleBattle);
		base.RegisterEvent(EventId.OnManualSelectLeaguePage);
		base.RegisterEvent(EventId.UpdatePower);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		this.mapList.Clear();
		this.levelList.Clear();
		this.mapList.Add("X");
		this.levelList.Add("X");
		this.levelStar1.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnSelectStar);
		this.levelStar2.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnSelectStar);
		this.levelStar3.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnSelectStar);
		this.refreshMap();
		for (int i = 0; i < this.mapList.Count; i++)
		{
			GameObject gameObject = this.numParent.AddChild(this.numTemplate);
			gameObject.name = "num" + i;
			gameObject.SetActive(true);
			gameObject.transform.localPosition = this.numPositionInterval * (float)i;
			UILabel component = gameObject.GetComponent<UILabel>();
			if (i == 0)
			{
				component.text = LanguageDataProvider.GetValue(301);
			}
			else if (this.mapList[i].Equals("PVP"))
			{
				component.text = LanguageDataProvider.GetValue(302);
			}
			else if (this.mapList[i].Equals("League"))
			{
				component.text = LanguageDataProvider.GetValue(304);
			}
			else
			{
				string value = LanguageDataProvider.GetValue(300);
				component.text = string.Format(value, i);
			}
			UIEventListener component2 = gameObject.GetComponent<UIEventListener>();
			component2.onClick = new UIEventListener.VoidDelegate(this.OnNumClick);
			component2.onDragStart = new UIEventListener.VoidDelegate(this.OnTriggerDragStart);
			component2.onDrag = new UIEventListener.VectorDelegate(this.OnTriggerDrag);
			component2.onDragEnd = new UIEventListener.VoidDelegate(this.OnTriggerDragEnd);
			component2.onPress = new UIEventListener.BoolDelegate(this.OnTriggerPress);
		}
		this.numTemplate.SetActive(false);
		this.bgParent = GameObject.Find("Battle/BG");
		this.ShowMap(CustomSelectLevelWindow.selectMapIndex, false, true);
		this.powerValue.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.power.ToString();
	}

	private void refreshMap()
	{
		this.MapIndexMax = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.achieveLevels + 1;
		if (this.MapIndexMax == 0)
		{
			Debug.LogError("关卡数据不正确");
		}
		this.MapIndexMax = ((this.MapIndexMax <= this.mapList.Count - 1) ? this.MapIndexMax : (this.mapList.Count - 1));
		if (CustomSelectLevelWindow.selectMapIndex > this.MapIndexMax)
		{
			CustomSelectLevelWindow.selectMapIndex = -1;
		}
		if (CustomSelectLevelWindow.selectMapIndex == -1)
		{
			if (this.MapIndexMax == this.mapList.Count - 1)
			{
				CustomSelectLevelWindow.selectMapIndex = this.MapIndexMax;
				this.PlayEnter(false);
			}
			else
			{
				CustomSelectLevelWindow.selectMapIndex = 0;
				this.PlayEnter(false);
			}
		}
		else
		{
			this.PlayEnter(false);
			if (CustomSelectLevelWindow.selectMapIndex == this.MapIndexMax - 1)
			{
				base.Invoke("ShowNextMap", 1f);
			}
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnRenameFinished)
		{
			this.ScrollToLevel(this.MapIndexMax - 1, 0.5f);
		}
		else if (eventId == EventId.OnStartSingleBattle)
		{
			this.OnStartSingleBattle();
		}
		else if (eventId == EventId.OnManualSelectLeaguePage)
		{
			this.ShowMap(this.mapList.Count - 1, false, true);
		}
		else if (eventId == EventId.UpdatePower)
		{
			this.powerValue.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.power.ToString();
		}
	}

	private void SetVersion()
	{
		string text = string.Empty;
		Solarmax.Singleton<ConfigSystem>.Instance.TryGetConfig("version", out text);
		RemoteHost host = Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetHost();
		string[] array = host.GetAddress().Split(new char[]
		{
			'.'
		});
		text = string.Format("Ver: {0}_{1}:{2}", text, array[array.Length - 1], host.GetPort());
		this.version.text = text;
	}

	private void ShowMap(int index, bool imme = false, bool bSetGB = false)
	{
		if (!imme)
		{
			this.bIsShowMap = true;
		}
		CustomSelectLevelWindow.selectMapIndex = index;
		if (CustomSelectLevelWindow.selectMapIndex == 0)
		{
			this.mapShow.Switch(string.Empty, imme);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
			if (!this.logo.activeSelf && !this.bLogoIsFadeIn)
			{
				this.FadeLogo(true);
			}
			this.levelStar1.gameObject.SetActive(false);
			this.levelStar2.gameObject.SetActive(false);
			this.levelStar3.gameObject.SetActive(false);
			this.powerIcon.gameObject.SetActive(false);
			this.powerValue.gameObject.SetActive(false);
		}
		else
		{
			this.mapShow.Switch(this.mapList[index], imme);
			if (this.logo.activeSelf && !this.bLogoIsFadeIn)
			{
				this.FadeLogo(false);
			}
			this.levelStar1.gameObject.SetActive(true);
			this.levelStar2.gameObject.SetActive(true);
			this.levelStar3.gameObject.SetActive(true);
			this.powerIcon.gameObject.SetActive(true);
			this.powerValue.gameObject.SetActive(true);
			this.ShowLevelStar(this.levelList[index]);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.SingleLevel;
		}
		if (bSetGB)
		{
			this.numParent.transform.localPosition = -this.numPositionInterval * (float)index;
		}
		float subNumAlpha = (float)(-(float)CustomSelectLevelWindow.selectMapIndex) * this.numPositionInterval.x;
		this.SetSubNumAlpha(subNumAlpha);
	}

	private void FadeLogo(bool bFadeIn)
	{
		TweenAlpha tweenAlpha = this.logo.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = this.logo.AddComponent<TweenAlpha>();
		}
		this.logo.SetActive(true);
		this.bLogoIsFadeIn = true;
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = (float)((!bFadeIn) ? 1 : 0);
		tweenAlpha.to = (float)((!bFadeIn) ? 0 : 1);
		tweenAlpha.duration = 0.5f;
		tweenAlpha.SetOnFinished(delegate()
		{
			this.bLogoIsFadeIn = false;
			if (!bFadeIn)
			{
				this.logo.SetActive(false);
			}
		});
		tweenAlpha.Play(true);
	}

	private void ShowNextMap()
	{
		this.bIsShowMap = false;
		float x = -this.changePageLength * (float)CustomSelectLevelWindow.selectMapIndex;
		float x2 = -this.changePageLength * (float)(CustomSelectLevelWindow.selectMapIndex + 1);
		Vector3 localPosition = this.numParent.transform.localPosition;
		Vector3 localPosition2 = this.numParent.transform.localPosition;
		localPosition2.x = x;
		localPosition.x = x2;
		TweenPosition tweenPosition = this.numParent.GetComponent<TweenPosition>();
		if (tweenPosition == null)
		{
			tweenPosition = this.numParent.AddComponent<TweenPosition>();
		}
		tweenPosition.ResetToBeginning();
		this.numParent.transform.localPosition = localPosition2;
		tweenPosition.from = localPosition2;
		tweenPosition.to = localPosition;
		tweenPosition.duration = 0.5f;
		tweenPosition.Play(true);
	}

	private void SetSubNumAlpha(float numPositionX)
	{
		for (int i = 0; i < this.mapList.Count; i++)
		{
			GameObject gameObject = this.numParent.transform.Find("num" + i).gameObject;
			UILabel component = gameObject.GetComponent<UILabel>();
			float num = 0.6f - Mathf.Abs(numPositionX + gameObject.transform.localPosition.x) * (0.1f / this.numPositionInterval.x);
			if (i > this.MapIndexMax)
			{
				num -= 0.2f;
			}
			component.alpha = num;
		}
	}

	private void OnTriggerClick(GameObject go)
	{
		if (CustomSelectLevelWindow.selectMapIndex == 0 || CustomSelectLevelWindow.selectMapIndex > this.MapIndexMax)
		{
			return;
		}
		float x = this.numSelect.transform.localScale.x;
		if (x < 0.9f)
		{
			return;
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.power > 0)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestStartLevel(this.levelList[CustomSelectLevelWindow.selectMapIndex]);
		}
		else
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(202), 1f);
		}
	}

	private void OnStartLevelResult()
	{
		string matchId = this.mapList[CustomSelectLevelWindow.selectMapIndex];
		string text = this.levelList[CustomSelectLevelWindow.selectMapIndex];
		Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightNext = (CustomSelectLevelWindow.selectMapIndex == this.MapIndexMax);
		if (this.difficultyLevel == 0)
		{
			this.difficultyLevel = 1;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.difficultyLevel = this.difficultyLevel;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = 1;
		Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(text, 0);
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(text);
		if (data != null)
		{
			if (this.difficultyLevel == 1)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.easyAI;
			}
			else if (this.difficultyLevel == 2)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.generalAI;
			}
			else if (this.difficultyLevel == 3)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.hardAI;
			}
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiParam = data.aiParam;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.dyncDiffType = data.dyncDiffType;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.winType = data.winType;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1 = data.winTypeParam1;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam2 = data.winTypeParam2;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.loseType = data.loseType;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam1 = data.loseTypeParam1;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam2 = data.loseTypeParam2;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(matchId, GameType.SingleLevel, true);
	}

	private void OnTriggerDragStart(GameObject go)
	{
		this.dragTotal = Vector2.zero;
		this.mouseDown = true;
		this.bIsShowMap = false;
	}

	private void OnTriggerPress(GameObject go, bool isPressed)
	{
		if (this.dragging && isPressed)
		{
			this.downIndex = true;
		}
		else
		{
			this.downIndex = false;
		}
	}

	private void OnTriggerDrag(GameObject go, Vector2 delta)
	{
		this.dragTotal += delta * this.sensitive;
		if (Math.Abs(delta.x) > 2f)
		{
			this.dragging = true;
		}
		this.deltaScroll.x = this.deltaScroll.x + delta.x;
	}

	private void OnTriggerDragEnd(GameObject go)
	{
		this.mouseDown = false;
		if (Math.Abs(this.deltaScroll.x) > 150f)
		{
			if (this.deltaScroll.x > 0f)
			{
				this.deltaScroll.x = 100f + (float)UnityEngine.Random.Range(0, 50);
			}
			else
			{
				this.deltaScroll.x = -(100f + (float)UnityEngine.Random.Range(0, 50));
			}
		}
	}

	private void OnNumClick(GameObject go)
	{
		int num = int.Parse(go.name.Substring(3));
		if (num > this.MapIndexMax)
		{
			return;
		}
		if (num == 0 && this.MapIndexMax != CustomSelectLevelWindow.selectMapIndex)
		{
			int index = this.MapIndexMax;
			if (this.MapIndexMax == this.mapList.Count - 1)
			{
				index = this.MapIndexMax - 1;
			}
			this.deltaScroll.x = 0f;
			this.ScrollToLevel(index, 0.5f);
			return;
		}
		if (num == CustomSelectLevelWindow.selectMapIndex)
		{
			return;
		}
		this.deltaScroll.x = 0f;
		this.ScrollToLevel(num, 0.5f);
	}

	private void PlayEnter(bool play)
	{
		if (play)
		{
			this.whiteMask.SetActive(true);
			TweenPosition component = this.whiteMask.GetComponent<TweenPosition>();
			component.PlayForward();
			TweenAlpha component2 = this.whiteMask.GetComponent<TweenAlpha>();
			component2.PlayForward();
			TweenScale component3 = this.whiteMask.GetComponent<TweenScale>();
			component3.PlayForward();
		}
		else
		{
			this.whiteMask.SetActive(false);
		}
	}

	public void OnCloseClick()
	{
	}

	public void OnBackCity()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("LobbyWindowView", EventId.UpdateChaptersWindow, new object[0]));
	}

	public void OnStartSingleBattle()
	{
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.NoticeSelfTeam, new object[0]);
		Solarmax.Singleton<ShipFadeManager>.Get().SetShipAlpha(0f);
		this.mapShow.AlphaFadeOut(0.5f, null);
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 0.6f;
		tweenAlpha.SetOnFinished(delegate()
		{
			this.StartSingleBattle();
		});
	}

	public void StartSingleBattle()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().FadeBattle(true, new EventDelegate(delegate()
		{
			Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
		}));
		Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleWindow_off");
		GuideManager.StartGuide(GuildCondition.GC_Level, Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId, null);
	}

	private void ReLoadMapRes()
	{
	}

	public void OnSelectStar(GameObject obj)
	{
		UISprite component = obj.gameObject.GetComponent<UISprite>();
		if (component == this.levelStar1)
		{
			this.SetLevelStar(1);
			this.difficultyLevel = 1;
		}
		else if (component == this.levelStar2)
		{
			this.SetLevelStar(2);
			this.difficultyLevel = 2;
		}
		else if (component == this.levelStar3)
		{
			this.SetLevelStar(3);
			this.difficultyLevel = 3;
		}
	}

	public void SetLevelStar(int star)
	{
		this.difficultyLevel = star;
		switch (star)
		{
		case 1:
			this.levelStar1.spriteName = "icon_star";
			this.levelStar2.spriteName = "icon_star_bg";
			this.levelStar3.spriteName = "icon_star_bg";
			break;
		case 2:
			this.levelStar1.spriteName = "icon_star";
			this.levelStar2.spriteName = "icon_star";
			this.levelStar3.spriteName = "icon_star_bg";
			break;
		case 3:
			this.levelStar1.spriteName = "icon_star";
			this.levelStar2.spriteName = "icon_star";
			this.levelStar3.spriteName = "icon_star";
			break;
		default:
			this.levelStar1.spriteName = "icon_star_bg";
			this.levelStar2.spriteName = "icon_star_bg";
			this.levelStar3.spriteName = "icon_star_bg";
			break;
		}
	}

	private void ShowLevelStar(string levelId)
	{
		int levelStar = Solarmax.Singleton<LevelDataHandler>.Instance.QueryLevelStar(levelId);
		this.SetLevelStar(levelStar);
	}

	public GameObject logo;

	public UILabel version;

	public MapShow mapShow;

	public UIEventListener moveTrigger;

	public GameObject numParent;

	public GameObject numTemplate;

	public GameObject numSelect;

	public GameObject whiteMask;

	private GameObject bgParent;

	public UISprite levelStar1;

	public UISprite levelStar2;

	public UISprite levelStar3;

	public GameObject powerIcon;

	public UILabel powerValue;

	private int difficultyLevel;

	private List<string> levelList = new List<string>();

	private List<string> mapList = new List<string>();

	private float sensitive = 1f;

	private Vector2 dragTotal = Vector2.zero;

	private float dragSpeedX;

	private bool isOut = true;

	private float currentAlpha;

	private float changePageLength;

	private Vector3 numPositionInterval = new Vector3(240f, 0f, 0f);

	private static int selectMapIndex = -1;

	private const string PVPMapName = "PVP";

	private const string LeagueMapName = "League";

	private int MapIndexMax;

	private bool mouseDown;

	private bool dragging;

	private bool downIndex;

	private bool bIsShowMap;

	private Vector2 deltaScroll = Vector2.zero;

	private Dictionary<string, MapConfig> m_mapEditor = new Dictionary<string, MapConfig>();

	private Vector3 localpos;

	private bool bLogoIsFadeIn;
}
