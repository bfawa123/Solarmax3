using System;
using System.Collections;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class CPvPRoomWindow : BaseWindow
{
	private void ApplyModeOffset(float offsetX)
	{
		if (!this.modeOffsetInitialized)
		{
			this.modeOffsetRoots = this.GetModeOffsetRoots();
			this.modeOffsetRootPositions = new Vector3[this.modeOffsetRoots.Length];
			for (int i = 0; i < this.modeOffsetRoots.Length; i++)
			{
				Transform transform = this.modeOffsetRoots[i];
				if (transform != null)
				{
					this.modeOffsetRootPositions[i] = transform.localPosition;
				}
			}
			this.modeOffsetInitialized = true;
		}
		if (this.modeOffsetRoots == null || this.modeOffsetRootPositions == null)
		{
			return;
		}
		for (int j = 0; j < this.modeOffsetRoots.Length; j++)
		{
			Transform transform2 = this.modeOffsetRoots[j];
			if (transform2 != null)
			{
				Vector3 vector = this.modeOffsetRootPositions[j];
				transform2.localPosition = new Vector3(vector.x + offsetX, vector.y, vector.z);
			}
		}
	}

	private void RestoreModeOffset()
	{
		if (!this.modeOffsetInitialized || this.modeOffsetRoots == null || this.modeOffsetRootPositions == null)
		{
			return;
		}
		for (int i = 0; i < this.modeOffsetRoots.Length; i++)
		{
			Transform transform = this.modeOffsetRoots[i];
			if (transform != null)
			{
				transform.localPosition = this.modeOffsetRootPositions[i];
			}
		}
	}

	private Transform[] GetModeOffsetRoots()
	{
		List<Transform> list = new List<Transform>();
		if (this.buttonArray != null)
		{
			for (int i = 0; i < this.buttonArray.Length; i++)
			{
				UIButton uibutton = this.buttonArray[i];
				if (uibutton != null)
				{
					this.TryAddUniqueTransform(list, uibutton.transform.parent);
				}
			}
		}
		if (this.titleLable != null)
		{
			for (int j = 0; j < this.titleLable.Length; j++)
			{
				UILabel uilabel = this.titleLable[j];
				if (uilabel != null)
				{
					this.TryAddUniqueTransform(list, uilabel.transform.parent);
				}
			}
		}
		if (this.marks != null)
		{
			for (int k = 0; k < this.marks.Length; k++)
			{
				UISprite uisprite = this.marks[k];
				if (uisprite != null)
				{
					this.TryAddUniqueTransform(list, uisprite.transform.parent);
				}
			}
		}
		this.TryAddUniqueTransform(list, (this.seasonTime != null) ? this.seasonTime.transform : null);
		this.TryAddUniqueTransform(list, (this.seasonReward != null) ? this.seasonReward.transform : null);
		List<Transform> list2 = new List<Transform>();
		for (int l = 0; l < list.Count; l++)
		{
			Transform transform = list[l];
			bool flag = false;
			for (int m = 0; m < list.Count; m++)
			{
				Transform transform2 = list[m];
				if (!(transform2 == null) && transform != transform2 && transform != null && transform.IsChildOf(transform2))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list2.Add(transform);
			}
		}
		return list2.ToArray();
	}

	private void TryAddUniqueTransform(List<Transform> list, Transform t)
	{
		if (t == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == t)
			{
				return;
			}
		}
		list.Add(t);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.UpdateAccumulMoney);
		base.RegisterEvent(EventId.OnUpdateLeagueMode);
		base.RegisterEvent(EventId.OnSeasonReward);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.SetPlayerBaseInfo();
		this.SetSublabelModel(Solarmax.Singleton<LocalPvpSeasonSystem>.Get().pvpType, Solarmax.Singleton<LocalPvpSeasonSystem>.Get().seasonStart, Solarmax.Singleton<LocalPvpSeasonSystem>.Get().seasonEnd);
		if (!Solarmax.Singleton<LocalPlayer>.Get().IsInSeason())
		{
			this.seasonTip2.text = LanguageDataProvider.GetValue(1162);
		}
		this.RefreshMoneyUI(Solarmax.Singleton<LocalPlayer>.Get().nCurAccumulMoney, Solarmax.Singleton<LocalPlayer>.Get().nMaxAccumulMoney);
		this.Remove3v1ButtonUI();
		this.Ensure3v1ButtonUI();
		this.CenterModeLayout();

	}

	public override void OnHide()
	{
		base.StopAllCoroutines();
		this.RestoreModeOffset();
		this.Remove3v1ButtonUI();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.UpdateMoney)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
		else if (eventId == EventId.UpdateAccumulMoney)
		{
			this.RefreshMoneyUI(Solarmax.Singleton<LocalPlayer>.Get().nCurAccumulMoney, Solarmax.Singleton<LocalPlayer>.Get().nMaxAccumulMoney);
		}
		else if (eventId == EventId.OnUpdateLeagueMode)
		{
			this.SetPlayerBaseInfo();
			this.SetSublabelModel(Solarmax.Singleton<LocalPvpSeasonSystem>.Get().pvpType, Solarmax.Singleton<LocalPvpSeasonSystem>.Get().seasonStart, Solarmax.Singleton<LocalPvpSeasonSystem>.Get().seasonEnd);
			this.RefreshMoneyUI(Solarmax.Singleton<LocalPlayer>.Get().nCurAccumulMoney, Solarmax.Singleton<LocalPlayer>.Get().nMaxAccumulMoney);
			this.Remove3v1ButtonUI();
			this.Ensure3v1ButtonUI();
			this.CenterModeLayout();
			SeasonRewardView component = this.seasonReward.GetComponent<SeasonRewardView>();
			component.UpdateUI();
		}
		else if (eventId == EventId.OnSeasonReward)
		{
			int num = (int)args[0];
			SeasonRewardModel seasonRewardModel = Solarmax.Singleton<SeasonRewardModel>.Get();
			seasonRewardModel.rewardStatus[num.ToString()] = true;
			SeasonRewardView component2 = this.seasonReward.GetComponent<SeasonRewardView>();
			component2.RefreshUI();
		}
	}

	private bool IsCanClick()
	{
		DateTime? dateTime = this.searchFriendTime;
		if (dateTime != null)
		{
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchFriendTime.Value;
			if (timeSpan.TotalSeconds < 5.0)
			{
				string message = string.Format(LanguageDataProvider.GetValue(1147), 5 - timeSpan.Seconds);
				Tips.Make(message);
				return false;
			}
		}
		this.searchFriendTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		return true;
	}

	public void OnFourClick()
	{
		if (!this.IsCanClick())
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Ladder, string.Empty, string.Empty, CooperationType.CT_1v1v1v1, 4, true, string.Empty, -1, string.Empty, false);
	}

	public void OnThreeClick()
	{
		if (!this.IsCanClick())
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Ladder, string.Empty, string.Empty, CooperationType.CT_1v1v1, 3, true, string.Empty, -1, string.Empty, false);
	}

	public void OnOneClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Ladder, string.Empty, string.Empty, CooperationType.CT_1v1, 2, true, string.Empty, -1, string.Empty, false);
	}

	private void Remove3v1ButtonUI()
	{
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform transform = componentsInChildren[i];
			if (!(transform == null))
			{
				string name = transform.name;
				if (name == "Btn_3v1" || name == "Lbl_3v1" || name == "Mark_3v1")
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}
	}

	private void Ensure3v1ButtonUI()
	{
		if (this.buttonArray == null || this.buttonArray.Length < 2 || this.buttonArray[1] == null)
		{
			return;
		}
		UIButton uibutton = this.buttonArray[1];
		Transform parent = uibutton.transform.parent;
		if (parent == null)
		{
			return;
		}
		if (parent.Find("Btn_3v1") != null)
		{
			return;
		}
		float num = 0f;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(uibutton.gameObject);
		gameObject.name = "Btn_3v1";
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = uibutton.transform.localPosition;
		gameObject.transform.localRotation = uibutton.transform.localRotation;
		gameObject.transform.localScale = uibutton.transform.localScale;
		gameObject.transform.SetSiblingIndex(Mathf.Clamp(uibutton.transform.GetSiblingIndex() + 1, 0, parent.childCount));
		UIButton component = gameObject.GetComponent<UIButton>();
		if (component != null)
		{
			component.onClick.Clear();
			component.onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnThreeVOneClick)));
			component.enabled = true;
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(parent, uibutton.transform, true, true);
		Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(parent, gameObject.transform, true, true);
		float num2 = Mathf.Clamp(bounds.size.x * 0.08f, 30f, 80f);
		float num3 = bounds.max.x + num2 + 150f;
		float num4 = num3 - bounds2.min.x;
		gameObject.transform.localPosition = gameObject.transform.localPosition + new Vector3(num4, 0f, 0f);
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, uibutton.transform.localPosition.y, gameObject.transform.localPosition.z);
		num = gameObject.transform.localPosition.x - uibutton.transform.localPosition.x;
		if (this.titleLable != null && this.titleLable.Length > 1 && this.titleLable[1] != null)
		{
			UILabel uilabel = this.titleLable[1];
			Transform parent2 = uilabel.transform.parent;
			if (parent2 != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(uilabel.gameObject);
				gameObject2.name = "Lbl_3v1";
				gameObject2.transform.parent = parent2;
				gameObject2.transform.localPosition = uilabel.transform.localPosition + new Vector3(num, 0f, 0f);
				gameObject2.transform.localRotation = uilabel.transform.localRotation;
				gameObject2.transform.localScale = uilabel.transform.localScale;
				UILabel component2 = gameObject2.GetComponent<UILabel>();
				if (component2 != null)
				{
					component2.mKey = -1;
					component2.text = LanguageDataProvider.GetValue(7022);
				}
				gameObject2.SetActive(true);
			}
		}
		if (this.marks != null && this.marks.Length > 1 && this.marks[1] != null)
		{
			UISprite uisprite3 = this.marks[1];
			Transform parent3 = uisprite3.transform.parent;
			if (parent3 != null)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(uisprite3.gameObject);
				gameObject3.name = "Mark_3v1";
				gameObject3.transform.parent = parent3;
				gameObject3.transform.localPosition = uisprite3.transform.localPosition + new Vector3(num, 0f, 0f);
				gameObject3.transform.localRotation = uisprite3.transform.localRotation;
				gameObject3.transform.localScale = uisprite3.transform.localScale;
				gameObject3.SetActive(true);
			}
		}
		gameObject.SetActive(true);
		NGUITools.UpdateWidgetCollider(gameObject);
		this.TrySet3v1ButtonTexture(gameObject);
	}

	private void CenterModeLayout()
	{
		Transform[] modeOffsetRoots = this.GetModeOffsetRoots();
		if (modeOffsetRoots == null || modeOffsetRoots.Length == 0)
		{
			return;
		}
		Transform transform = base.transform;
		bool flag = false;
		Bounds bounds = default(Bounds);
		for (int i = 0; i < modeOffsetRoots.Length; i++)
		{
			Transform transform2 = modeOffsetRoots[i];
			if (!(transform2 == null))
			{
				Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(transform, transform2, true, true);
				if (!flag)
				{
					bounds = bounds2;
					flag = true;
				}
				else
				{
					bounds.Encapsulate(bounds2.min);
					bounds.Encapsulate(bounds2.max);
				}
			}
		}
		if (!flag)
		{
			return;
		}
		this.ApplyModeOffset(-bounds.center.x);
	}

	private void TrySet3v1ButtonTexture(GameObject root)
	{
		if (root == null)
		{
			return;
		}
		UITexture componentInChildren = root.GetComponentInChildren<UITexture>(true);
		if (componentInChildren == null)
		{
			return;
		}
		if (this.classTexture != null)
		{
			componentInChildren.mainTexture = this.classTexture;
			return;
		}
		base.StartCoroutine(this.Load3v1ClassTexture(componentInChildren));
	}

	private IEnumerator Load3v1ClassTexture(UITexture target)
	{
		if (target == null)
		{
			yield break;
		}
		string url = Application.streamingAssetsPath + "/res/data/image/class.jpg";
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			this.classTexture = www.texture;
			if (this.classTexture != null && target != null)
			{
				target.mainTexture = this.classTexture;
			}
		}
		www.Dispose();
		yield break;
	}

	public void OnThreeVOneClick()
	{
		if (!this.IsCanClick())
		{
			return;
		}
		LevelConfigConfigProvider levelConfigConfigProvider = Solarmax.Singleton<LevelConfigConfigProvider>.Get();
		LevelConfig levelConfig = null;
		if (levelConfigConfigProvider != null)
		{
			List<LevelConfig> levelsByPrefix = levelConfigConfigProvider.GetLevelsByPrefix("44");
			if (levelsByPrefix != null && levelsByPrefix.Count > 0)
			{
				levelConfig = levelsByPrefix[UnityEngine.Random.Range(0, levelsByPrefix.Count)];
			}
		}
		string text = "440101";
		string text2 = string.Empty;
		if (levelConfig != null)
		{
			text = levelConfig.id;
			text2 = levelConfig.map;
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = text;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Ladder, string.Empty, text2, CooperationType.CT_3v1, 4, true, string.Empty, -1, text, false);
	}

	private Texture2D classTexture;
	public void OnTwoClick()
	{
		if (!this.IsCanClick())
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Ladder, string.Empty, string.Empty, CooperationType.CT_2v2, 4, false, string.Empty, -1, string.Empty, false);
	}

	public void OnTwoClickEx()
	{
		if (!this.IsCanClick())
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Ladder, string.Empty, string.Empty, CooperationType.CT_2v2, 4, true, string.Empty, -1, string.Empty, false);
	}

	public void OnBackClick()
	{
		Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
	}

	private void SetPlayerBaseInfo()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData != null)
		{
			if (string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().playerData.name))
			{
				this.playerName.text = "无名喵喵";
			}
			else
			{
				this.playerName.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.name;
			}
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			int score = Solarmax.Singleton<LocalPlayer>.Get().playerData.score;
			this.playerScore.text = score.ToString();
		}
	}

	private void SetSublabelModel(int nType, int starDay, int endDay)
	{
		this.seasonTime.SetActive(false);
		foreach (UIButton uibutton in this.buttonArray)
		{
			uibutton.enabled = false;
			uibutton.onClick.Clear();
			uibutton.gameObject.SetActive(true);
		}
		foreach (GameObject gameObject in this.secondButton)
		{
			gameObject.SetActive(false);
		}
		foreach (UILabel uilabel in this.titleLable)
		{
			uilabel.gameObject.SetActive(true);
		}
		foreach (UISprite uisprite in this.marks)
		{
			uisprite.gameObject.SetActive(true);
		}

		long num = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		if (num >= (long)starDay && num <= (long)endDay)
		{
			DateTime dateTime = new DateTime(1970, 1, 1);
			DateTime dateTime2 = dateTime.AddSeconds((double)endDay);
			string text = dateTime2.ToLocalTime().ToString();
			DateTime dateTime3 = new DateTime(1970, 1, 1);
			string str = dateTime3.AddSeconds((double)starDay).ToLocalTime().ToString("yyyy.MM.dd");
			string str2 = dateTime2.ToLocalTime().ToString("yyyy.MM.dd");
			this.seasonTime.SetActive(true);
			UILabel component = this.seasonTime.transform.Find("date").GetComponent<UILabel>();
			component.text = str + "-" + str2;
			foreach (UIButton uibutton2 in this.buttonArray)
			{
				uibutton2.enabled = true;
			}
			if (nType == 3)
			{
				this.titleLable[0].text = LanguageDataProvider.GetValue(2035);
				this.titleLable[1].text = LanguageDataProvider.GetValue(2143);
				this.titleLable[2].text = LanguageDataProvider.GetValue(2144);
				this.titleLable[3].text = LanguageDataProvider.GetValue(2072);
				this.buttonArray[0].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnFourClick)));
				this.buttonArray[1].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnOneClick)));
				this.buttonArray[3].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnThreeClick)));
				this.buttonArray[2].onClick.Add(new EventDelegate(delegate()
				{
					this.secondButton[2].SetActive(true);
					this.titleLable[2].gameObject.SetActive(false);
				}));
				this.marks[0].spriteName = this.bigMarkNames[3];
				this.marks[1].spriteName = this.smallMarkNames[0];
				this.marks[2].spriteName = this.smallMarkNames[1];
				this.marks[3].spriteName = this.smallMarkNames[2];
				this.seasonNotice.text = LanguageDataProvider.Format(2216, new object[]
				{
					LanguageDataProvider.GetValue(2035)
				});
			}
			if (nType == 2)
			{
				this.titleLable[0].text = LanguageDataProvider.GetValue(2072);
				this.titleLable[1].text = LanguageDataProvider.GetValue(2143);
				this.titleLable[2].text = LanguageDataProvider.GetValue(2144);
				this.titleLable[3].text = LanguageDataProvider.GetValue(2035);
				this.buttonArray[1].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnOneClick)));
				this.buttonArray[0].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnThreeClick)));
				this.buttonArray[3].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnFourClick)));
				this.buttonArray[2].onClick.Add(new EventDelegate(delegate()
				{
					this.secondButton[2].SetActive(true);
					this.titleLable[2].gameObject.SetActive(false);
				}));
				this.marks[0].spriteName = this.bigMarkNames[2];
				this.marks[1].spriteName = this.smallMarkNames[0];
				this.marks[2].spriteName = this.smallMarkNames[1];
				this.marks[3].spriteName = this.smallMarkNames[3];
				this.seasonNotice.text = LanguageDataProvider.Format(2216, new object[]
				{
					LanguageDataProvider.GetValue(2072)
				});
			}
			if (nType == 0)
			{
				this.titleLable[0].text = LanguageDataProvider.GetValue(2143);
				this.titleLable[1].text = LanguageDataProvider.GetValue(2144);
				this.titleLable[2].text = LanguageDataProvider.GetValue(2072);
				this.titleLable[3].text = LanguageDataProvider.GetValue(2035);
				this.buttonArray[0].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnOneClick)));
				this.buttonArray[2].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnThreeClick)));
				this.buttonArray[3].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnFourClick)));
				this.buttonArray[1].onClick.Add(new EventDelegate(delegate()
				{
					this.secondButton[1].SetActive(true);
					this.titleLable[1].gameObject.SetActive(false);
				}));
				this.marks[0].spriteName = this.bigMarkNames[0];
				this.marks[1].spriteName = this.smallMarkNames[1];
				this.marks[2].spriteName = this.smallMarkNames[2];
				this.marks[3].spriteName = this.smallMarkNames[3];
				this.seasonNotice.text = LanguageDataProvider.Format(2216, new object[]
				{
					LanguageDataProvider.GetValue(2143)
				});
			}
			if (nType == 1)
			{
				this.titleLable[0].text = LanguageDataProvider.GetValue(2144);
				this.titleLable[1].text = LanguageDataProvider.GetValue(2143);
				this.titleLable[2].text = LanguageDataProvider.GetValue(2072);
				this.titleLable[3].text = LanguageDataProvider.GetValue(2035);
				this.buttonArray[0].onClick.Add(new EventDelegate(delegate()
				{
					this.secondButton[0].SetActive(true);
					this.titleLable[0].gameObject.SetActive(false);
				}));
				this.buttonArray[1].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnOneClick)));
				this.buttonArray[2].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnThreeClick)));
				this.buttonArray[3].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnFourClick)));
				this.marks[0].spriteName = this.bigMarkNames[1];
				this.marks[1].spriteName = this.smallMarkNames[0];
				this.marks[2].spriteName = this.smallMarkNames[2];
				this.marks[3].spriteName = this.smallMarkNames[3];
				this.seasonNotice.text = LanguageDataProvider.Format(2216, new object[]
				{
					LanguageDataProvider.GetValue(2144)
				});
			}
		}
		else
		{
			this.seasonTime.SetActive(false);
			foreach (UIButton uibutton3 in this.buttonArray)
			{
				uibutton3.enabled = true;
			}
			this.titleLable[0].text = LanguageDataProvider.GetValue(2035);
			this.titleLable[1].text = LanguageDataProvider.GetValue(2143);
			this.titleLable[2].text = LanguageDataProvider.GetValue(2144);
			this.titleLable[3].text = LanguageDataProvider.GetValue(2072);
			this.buttonArray[0].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnFourClick)));
			this.buttonArray[1].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnOneClick)));
			this.buttonArray[3].onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnThreeClick)));
			this.buttonArray[2].onClick.Add(new EventDelegate(delegate()
			{
				this.secondButton[2].SetActive(true);
				this.titleLable[2].gameObject.SetActive(false);
			}));
			this.marks[0].spriteName = this.bigMarkNames[3];
			this.marks[1].spriteName = this.smallMarkNames[0];
			this.marks[2].spriteName = this.smallMarkNames[1];
			this.marks[3].spriteName = this.smallMarkNames[2];
			this.seasonNotice.text = LanguageDataProvider.GetValue(2217);
		}
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnClickAddMoney()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public void OnClickAvatar()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CollectionWindow");
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	private void RefreshMoneyUI(int nCurMoney, int nMaxMoney)
	{
		string text = string.Format(LanguageDataProvider.GetValue(2262), nCurMoney, nMaxMoney);
		this.accumulation.text = text;
	}

	public NetTexture playerIcon;

	public UILabel playerName;

	public UILabel playerMoney;

	public UILabel playerScore;

	public UILabel[] titleLable;

	public UILabel month;

	public UILabel day;

	public UIButton[] buttonArray;

	public GameObject seasonTime;

	public UILabel seasonNotice;

	public UILabel seasonTip2;

	public GameObject[] secondButton;

	public UISprite[] marks;

	public GameObject seasonReward;

	public UILabel accumulation;

	private bool isSpecialMode = false;

	private bool modeOffsetInitialized;

	private Transform[] modeOffsetRoots;

	private Vector3[] modeOffsetRootPositions;

	private string[] bigMarkNames = new string[]
	{
		"Signal1",
		"Signal2",
		"Signal3",
		"Signal4"
	};

	private string[] smallMarkNames = new string[]
	{
		"Signal1_s",
		"Signal2_s",
		"Signal3_s",
		"Signal4_s"
	};

	private DateTime? searchFriendTime;
}
