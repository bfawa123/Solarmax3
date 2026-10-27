using System;
using System.Collections;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class ChallengeWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnTaskOkEvent);
		return true;
	}

	public void Awake()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		this.animatorMoney = 0;
		this.currentMoney = Solarmax.Singleton<LocalPlayer>.Get().playerData.money;
		this.levelTemplate.SetActive(false);
		this.chapterTemplate.SetActive(false);
		this.challengeTemplate.SetActive(false);
		EngineSystem engineSystem = Solarmax.Singleton<EngineSystem>.Get();
		engineSystem.onNetStatusChanged = (EngineSystem.OnNetStatusChanged)Delegate.Combine(engineSystem.onNetStatusChanged, new EngineSystem.OnNetStatusChanged(this.NetStatus));
		this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		this.NetStatus((NetworkReachability)Solarmax.Singleton<EngineSystem>.Get().GetNetworkRechability());
		this.challengeChapters = new Dictionary<ChapterInfo, ChallengeChapterTemplate>();
		this.UpdateUI();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (!base.transform.gameObject.activeSelf)
		{
			return;
		}
		if (eventId == EventId.UpdateMoney)
		{
			if (!this.moneyAnimator)
			{
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("Gold");
			}
			this.moneyAnimator = true;
			this.animatorMoney = Solarmax.Singleton<LocalPlayer>.Get().playerData.money - this.currentMoney;
		}
		else if (eventId == EventId.OnTaskOkEvent)
		{
			foreach (ChallengeChapterTemplate challengeChapterTemplate in this.challengeChapters.Values)
			{
				challengeChapterTemplate.RefreshReddotVisible();
			}
			foreach (ChallengeLevelTemplate challengeLevelTemplate in this.challengeLevel)
			{
				challengeLevelTemplate.RefreshUI();
			}
			if (this.challengeChapters.ContainsKey(this.choosedChapter))
			{
				this.onekey.enabled = this.challengeChapters[this.choosedChapter].ReddotVisible();
				UISprite component = this.onekey.GetComponent<UISprite>();
				component.alpha = ((!this.onekey.enabled) ? 0.4f : 1f);
			}
		}
	}

	public override void OnHide()
	{
		this.levelGrid.transform.DestroyChildren();
		this.chapterGrid.transform.DestroyChildren();
		this.challengeGrid.transform.DestroyChildren();
		this.challengeChapters.Clear();
	}

	private void Update()
	{
		if (this.moneyAnimator)
		{
			if (this.animatorMoney > 0)
			{
				this.animatorMoney--;
				this.currentMoney++;
				this.money.text = this.currentMoney.ToString();
			}
			else if (this.animatorMoney == 0)
			{
				this.moneyAnimator = false;
				this.currentMoney = Solarmax.Singleton<LocalPlayer>.Get().playerData.money;
				this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			}
		}
	}

	private void NetStatus(NetworkReachability reachability)
	{
		if (reachability != NetworkReachability.NotReachable)
		{
			if (reachability != NetworkReachability.ReachableViaCarrierDataNetwork)
			{
				if (reachability == NetworkReachability.ReachableViaLocalAreaNetwork)
				{
					this.netstatus.spriteName = "icon_net_wifi_03";
				}
			}
			else
			{
				this.netstatus.spriteName = "icon_net_mobile_03";
			}
		}
		else
		{
			this.netstatus.spriteName = "icon_net_offline";
		}
	}

	public void OnClickBack()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
	}

	public void OnClickAddMoney()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public void OnClickOneKey()
	{
		global::Coroutine.DelayDo(2f, new EventDelegate(delegate()
		{
			this.onekeyCD = true;
		}));
		if (this.choosedChapter == null || !this.onekeyCD)
		{
			return;
		}
		foreach (ChapterLevelGroup chapterLevelGroup in this.choosedChapter.levelList)
		{
			List<Achievement> challenges = AchievementModel.GetChallenges(chapterLevelGroup.groupID);
			List<string> list = new List<string>();
			foreach (Achievement achievement in challenges)
			{
				if (!Solarmax.Singleton<TaskConfigProvider>.Get().dataList.ContainsKey(achievement.id))
				{
					Solarmax.Singleton<LoggerSystem>.Instance.Error("challenge window 任务为空: " + achievement.id, new object[0]);
				}
				else
				{
					TaskConfig taskConfig = Solarmax.Singleton<TaskConfigProvider>.Get().dataList[achievement.taskId];
					if (taskConfig.status == TaskStatus.Completed)
					{
						list.Add(achievement.taskId);
					}
				}
			}
			if (list.Count != 0)
			{
				Solarmax.Singleton<TaskModel>.Get().ClaimAllReward(list, null, 1);
			}
		}
		this.onekeyCD = false;
	}

	public void OnClickFollowChallenge()
	{
	}

	public void OnTrackToggleChanged()
	{
		if (this.choosedChapter != null)
		{
			this.track = !this.track;
			this.trackToggle.SetActive(this.track);
			this.choosedChapter.trackChallenge = this.track;
			Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalChapters();
		}
	}

	public void UpdateUI()
	{
		this.dicToggle.Clear();
		int num = -1;
		for (int i = 1; i < Solarmax.Singleton<LevelDataHandler>.Get().chapterList.Count; i++)
		{
			GameObject gameObject = this.chapterGrid.gameObject.AddChild(this.chapterTemplate);
			gameObject.SetActive(true);
			ChallengeChapterTemplate component = gameObject.GetComponent<ChallengeChapterTemplate>();
			this.challengeChapters.Add(Solarmax.Singleton<LevelDataHandler>.Get().chapterList[i], component);
			component.EnsureInit(i, Solarmax.Singleton<LevelDataHandler>.Get().chapterList[i]);
			UIToggle toggle = gameObject.GetComponent<UIToggle>();
			this.dicToggle[toggle] = i;
			toggle.onChange.Add(new EventDelegate(delegate()
			{
				if (toggle.value)
				{
					this.StartCoroutine(this.OnChapterClicked(Solarmax.Singleton<LevelDataHandler>.Get().chapterList[this.dicToggle[toggle]]));
				}
			}));
			if (num == -1 && component.ReddotVisible())
			{
				num = i;
			}
		}
		this.chapterGrid.Reposition();
		this.chapterScroll.ResetPosition();
		num = ((num != -1) ? num : 1);
		float num2 = (float)Solarmax.Singleton<LevelDataHandler>.Get().chapterList.Count - 4.35f - 1f;
		float value = (float)(num - 1) * 1f / num2;
		foreach (KeyValuePair<UIToggle, int> keyValuePair in this.dicToggle)
		{
			if (keyValuePair.Value == num)
			{
				keyValuePair.Key.value = true;
				break;
			}
		}
		this.chapterScroll.verticalScrollBar.value = value;
		this.chapterScroll.UpdatePosition();
	}

	public IEnumerator OnChapterClicked(ChapterInfo chapter)
	{
		this.challengeLevel.Clear();
		this.onekeyCD = true;
		this.levelGrid.transform.DestroyChildren();
		this.levelGrid.Reposition();
		yield return null;
		this.choosedChapter = chapter;
		foreach (ChapterLevelGroup chapterLevelGroup in chapter.levelList)
		{
			List<Achievement> challenges = AchievementModel.GetChallenges(chapterLevelGroup.groupID);
			if (challenges.Count != 0)
			{
				GameObject gameObject = this.levelGrid.gameObject.AddChild(this.levelTemplate);
				gameObject.SetActive(true);
				ChallengeLevelTemplate component = gameObject.GetComponent<ChallengeLevelTemplate>();
				component.EnsureInit(chapterLevelGroup.groupID);
				this.challengeLevel.Add(component);
				this.track = this.choosedChapter.trackChallenge;
				this.trackToggle.SetActive(this.track);
			}
		}
		this.levelGrid.Reposition();
		this.levelScroll.ResetPosition();
		this.onekey.enabled = this.challengeChapters[this.choosedChapter].ReddotVisible();
		UISprite sprite = this.onekey.GetComponent<UISprite>();
		sprite.alpha = ((!this.onekey.enabled) ? 0.4f : 1f);
		yield break;
	}

	private const string WIFI_ICON = "icon_net_wifi_03";

	private const string MOBILE_ICON = "icon_net_mobile_03";

	private const string NOT_REACHABLE_ICON = "icon_net_offline";

	private const string CHAPTER_ICON = "Btn_CopyUI_{0}";

	public GameObject chapterTemplate;

	public GameObject levelTemplate;

	public GameObject challengeTemplate;

	public UIGrid chapterGrid;

	public UIGrid levelGrid;

	public UIGrid challengeGrid;

	public UILabel money;

	public UISprite netstatus;

	public UIScrollView chapterScroll;

	public UIScrollView levelScroll;

	public GameObject trackToggle;

	public UIButton onekey;

	private Dictionary<UIToggle, int> dicToggle = new Dictionary<UIToggle, int>();

	private bool track;

	private ChapterInfo choosedChapter;

	private bool onekeyCD = true;

	private Dictionary<ChapterInfo, ChallengeChapterTemplate> challengeChapters;

	private List<ChallengeLevelTemplate> challengeLevel = new List<ChallengeLevelTemplate>();

	private int currentMoney;

	private int animatorMoney;

	private bool moneyAnimator;
}
