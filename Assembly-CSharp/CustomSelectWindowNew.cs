using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class CustomSelectWindowNew : BaseWindow
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
			float num4 = this.changePageLength * (float)(-(float)CustomSelectWindowNew.selectMapIndex);
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
		if (num5 != CustomSelectWindowNew.selectMapIndex && num5 >= 0 && num5 <= this.MapIndexMax)
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
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		this.SetVersion();
		this.mapList.Add("X");
		string data = Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(5);
		string[] array = data.Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			this.mapList.Add(array[i]);
		}
		this.mapList.Add("PVP");
		this.mapList.Add("League");
		if (this.MapIndexMax == 0)
		{
			Debug.LogError("关卡数据不正确");
		}
		if (this.MapIndexMax == this.mapList.Count - 2)
		{
			this.MapIndexMax = this.mapList.Count - 1;
		}
		if (CustomSelectWindowNew.selectMapIndex > this.MapIndexMax)
		{
			CustomSelectWindowNew.selectMapIndex = -1;
		}
		if (CustomSelectWindowNew.selectMapIndex == -1)
		{
			if (this.MapIndexMax == this.mapList.Count - 1)
			{
				CustomSelectWindowNew.selectMapIndex = this.MapIndexMax - 1;
				this.PlayEnter(false);
			}
			else
			{
				CustomSelectWindowNew.selectMapIndex = 0;
				this.PlayEnter(true);
			}
		}
		else
		{
			this.PlayEnter(false);
			if (CustomSelectWindowNew.selectMapIndex == this.MapIndexMax - 1 && CustomSelectWindowNew.selectMapIndex < this.mapList.Count - 3)
			{
				base.Invoke("ShowNextMap", 1f);
			}
		}
		for (int j = 0; j < this.mapList.Count; j++)
		{
			GameObject gameObject = this.numParent.AddChild(this.numTemplate);
			gameObject.name = "num" + j;
			gameObject.SetActive(true);
			gameObject.transform.localPosition = this.numPositionInterval * (float)j;
			UILabel component = gameObject.GetComponent<UILabel>();
			if (j == 0)
			{
				component.text = LanguageDataProvider.GetValue(301);
			}
			else if (this.mapList[j].Equals("PVP"))
			{
				component.text = LanguageDataProvider.GetValue(302);
			}
			else if (this.mapList[j].Equals("League"))
			{
				component.text = LanguageDataProvider.GetValue(304);
			}
			else
			{
				string value = LanguageDataProvider.GetValue(300);
				component.text = string.Format(value, j);
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
		if (this.MapIndexMax == this.mapList.Count - 1 && string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().playerData.name))
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("SingleClearWindow");
			this.ShowMap(this.MapIndexMax - 2, false, true);
		}
		else
		{
			this.ShowMap(CustomSelectWindowNew.selectMapIndex, false, true);
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
		CustomSelectWindowNew.selectMapIndex = index;
		if (CustomSelectWindowNew.selectMapIndex == 0)
		{
			this.mapShow.Switch(string.Empty, imme);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
			if (!this.logo.activeSelf && !this.bLogoIsFadeIn)
			{
				this.FadeLogo(true);
			}
		}
		else if (CustomSelectWindowNew.selectMapIndex == this.mapList.Count - 1)
		{
			this.mapShow.Switch(string.Empty, imme);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.League;
			Solarmax.Singleton<UISystem>.Get().ShowWindow("LeagueWindow");
			if (Solarmax.Singleton<UISystem>.Get().IsWindowVisible("StartWindow"))
			{
				Solarmax.Singleton<UISystem>.Get().FadeOutWindow2("StartWindow");
			}
		}
		else if (CustomSelectWindowNew.selectMapIndex == this.mapList.Count - 2)
		{
			this.mapShow.Switch(string.Empty, imme);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
			Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
			if (Solarmax.Singleton<UISystem>.Get().IsWindowVisible("LeagueWindow"))
			{
				Solarmax.Singleton<UISystem>.Get().FadeOutWindow2("LeagueWindow");
			}
		}
		else
		{
			this.mapShow.Switch(this.mapList[index], imme);
			if (this.logo.activeSelf && !this.bLogoIsFadeIn)
			{
				this.FadeLogo(false);
			}
			if (Solarmax.Singleton<UISystem>.Get().IsWindowVisible("StartWindow"))
			{
				Solarmax.Singleton<UISystem>.Get().FadeOutWindow2("StartWindow");
			}
			if (Solarmax.Singleton<UISystem>.Get().IsWindowVisible("LeagueWindow"))
			{
				Solarmax.Singleton<UISystem>.Get().FadeOutWindow2("LeagueWindow");
			}
		}
		if (bSetGB)
		{
			this.numParent.transform.localPosition = -this.numPositionInterval * (float)index;
		}
		float subNumAlpha = (float)(-(float)CustomSelectWindowNew.selectMapIndex) * this.numPositionInterval.x;
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
		float x = -this.changePageLength * (float)CustomSelectWindowNew.selectMapIndex;
		float x2 = -this.changePageLength * (float)(CustomSelectWindowNew.selectMapIndex + 1);
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
		if (CustomSelectWindowNew.selectMapIndex == 0 || CustomSelectWindowNew.selectMapIndex >= this.mapList.Count - 2 || CustomSelectWindowNew.selectMapIndex > this.MapIndexMax)
		{
			return;
		}
		float x = this.numSelect.transform.localScale.x;
		if (x < 0.9f)
		{
			return;
		}
		string matchId = this.mapList[CustomSelectWindowNew.selectMapIndex];
		Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightNext = (CustomSelectWindowNew.selectMapIndex == this.MapIndexMax);
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(matchId, GameType.Single, true);
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
		if (num == 0 && this.MapIndexMax != CustomSelectWindowNew.selectMapIndex)
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
		if (num == CustomSelectWindowNew.selectMapIndex)
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

	public void OnGMSendCurLevel()
	{
	}

	public void OnGMChangeAccount()
	{
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CustomSelectWindowNew");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
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
		Solarmax.Singleton<LoggerSystem>.Instance.Info("CustomSelectWindowNew StartSingleBattle", new object[0]);
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().FadeBattle(true, new EventDelegate(delegate()
		{
			Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
		}));
		Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleWindow_off");
		GuideManager.StartGuide(GuildCondition.GC_Level, Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId, null);
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

	private Vector3 localpos;

	private bool bLogoIsFadeIn;

	private int gmTemp;
}
