using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class CooperationLevelWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.SelectCooperationLevel);
		base.RegisterEvent(EventId.OnStartMatchResult);
		base.RegisterEvent(EventId.UpdateChapterWindow);
		this.Difficult1.onClick = new UIEventListener.VoidDelegate(this.OnClickDifficult1);
		this.Difficult2.onClick = new UIEventListener.VoidDelegate(this.OnClickDifficult2);
		this.Difficult3.onClick = new UIEventListener.VoidDelegate(this.OnClickDifficult3);
		this.Difficult4.onClick = new UIEventListener.VoidDelegate(this.OnClickDifficult4);
		return true;
	}

	private void OnEnable()
	{
		BGManager.Inst.ChangeBackground(BackgroundType.CooperationLevel);
	}

	public override void OnShow()
	{
		this.SetPlayerBaseInfo();
		if (this.isShowShop)
		{
			this.isShowShop = false;
			if (this.lineGo != null)
			{
				this.lineGo.SetActive(true);
			}
			return;
		}
		base.OnShow();
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Linerender");
		this.lineGo = (UnityEngine.Object.Instantiate(resources) as GameObject);
		this.lineGo.transform.SetParent(GameObject.Find("Battle").transform);
		this.lineRender = this.lineGo.GetComponentInChildren<LineRenderer>();
		BGManager.Inst.ChangeBackground(BackgroundType.CooperationLevel);
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter != null)
		{
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(currentChapter.id);
			if (data != null)
			{
				ResourcesUtil.SetChapterBgTexture(BackgroundType.CooperationLevel, 0, data.chapterBG);
			}
		}
	}

	public override void OnHide()
	{
		if (this.isShowShop)
		{
			if (this.lineGo != null)
			{
				this.lineGo.SetActive(false);
			}
			return;
		}
		this.curDisplayChapterID = null;
		BGManager.Inst.ChangeBackground(BackgroundType.Normal);
		UnityEngine.Object.Destroy(this.lineGo);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (eventId == EventId.OnStartMatchResult)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<UISystem>.Instance.HideWindow("CooperationLevelWindow");
				Solarmax.Singleton<UISystem>.Instance.ShowWindow("RoomWaitWindow");
			}
			else if (errCode == ErrCode.EC_MatchIsFull)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(902), 1f);
			}
			else if (errCode == ErrCode.EC_MatchIsMember)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(903), 1f);
			}
			else if (errCode == ErrCode.EC_NotInMatch)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1142), 1f);
			}
			else if (errCode == ErrCode.EC_NotMaster)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(905), 1f);
			}
			else if (errCode == ErrCode.EC_RoomNotExist)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(906), 1f);
			}
			else
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.Format(901, new object[]
				{
					errCode
				}), 1f);
			}
		}
		else if (eventId == EventId.UpdateMoney)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
		else if (eventId == EventId.SelectCooperationLevel)
		{
			string value = (string)args[0];
			if (!string.IsNullOrEmpty(value))
			{
				CooperationLevelWindow.selectCooperationLevel = value;
				ChapterAssistConfig data = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetData(CooperationLevelWindow.selectCooperationLevel);
				if (data != null)
				{
					this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
					this.RefreshDiffuseLock(data);
				}
			}
			this.refreshLevelSelectMark();
			this.autoSelectDifficult();
		}
		else if (eventId == EventId.UpdateChapterWindow)
		{
			if (args.Length < 2)
			{
				return;
			}
			string text = args[1] as string;
			if (!string.IsNullOrEmpty(text))
			{
				this.curDisplayChapterID = text;
				this.DisplayCaptherLevel(text);
				this.autoSelectDifficult();
				this.RefreshDifficultLabelSize(this.selectDiffuse);
				this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
			}
		}
	}

	private void refreshLevelSelectMark()
	{
		if (CooperationLevelWindow.selectCooperationLevel != string.Empty)
		{
			for (int i = 0; i < this._cell.Count; i++)
			{
				if (this._cell[i].Config.id == CooperationLevelWindow.selectCooperationLevel)
				{
					this._cell[i].selected.SetActive(true);
					this._cell[i].unSelected.SetActive(false);
				}
				else
				{
					this._cell[i].selected.SetActive(false);
					this._cell[i].unSelected.SetActive(true);
				}
			}
		}
	}

	private void ShowBg(ChapterInfo curChapter)
	{
		if (curChapter == null)
		{
			return;
		}
		BGManager.Inst.ChangeBackground(BackgroundType.CooperationLevel);
		if (curChapter != null)
		{
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(curChapter.id);
			if (data != null)
			{
				ResourcesUtil.SetChapterBgTexture(BackgroundType.CooperationLevel, 0, data.chapterBG);
			}
		}
	}

	private void DisplayCaptherLevel(string chapterID)
	{
		if (string.IsNullOrEmpty(chapterID))
		{
			return;
		}
		this.LevelRoot.transform.DestroyChildren();
		this._cell.Clear();
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(chapterID);
		this.ShowBg(chapterInfo);
		List<Vector3> list = new List<Vector3>();
		if (chapterInfo != null)
		{
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
			if (data == null)
			{
				Debug.LogError("error read config " + chapterInfo.id);
				return;
			}
			List<ChapterAssistConfig> allData = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData(chapterInfo.id);
			if (allData != null && allData.Count > 0)
			{
				Vector3 b = Vector3.zero;
				this.lineRender.positionCount = allData.Count;
				for (int i = 0; i < allData.Count; i++)
				{
					ChapterAssistConfig chapterAssistConfig = allData[i];
					GameObject gameObject = this.LevelRoot.AddChild(this.template);
					CooperationAssistCell component = gameObject.GetComponent<CooperationAssistCell>();
					gameObject.name = "num" + i;
					gameObject.SetActive(true);
					bool unLock = this.CheckAssistLevelUnLock(chapterAssistConfig);
					component.SetInfo(chapterAssistConfig, unLock, 0);
					gameObject.transform.localPosition = new Vector2(chapterAssistConfig.posX, chapterAssistConfig.posY);
					Vector3 position = gameObject.transform.position;
					this._cell.Add(component);
					if (i > 0)
					{
						GameObject gameObject2 = this.LevelRoot.AddChild(this.lineUnit);
						gameObject2.SetActive(true);
						gameObject2.transform.localPosition = new Vector3(b.x, b.y, b.z);
						Vector3 localPosition = gameObject.transform.localPosition;
						float magnitude = (localPosition - b).magnitude;
						Vector3 to = localPosition - b;
						float num = Mathf.Atan((b.y - localPosition.y) / (b.x - localPosition.x)) * 180f / 3.1415927f;
						float y = magnitude / 220f * 100f;
						gameObject2.transform.localScale = new Vector3(100f, y, 100f);
						float num2 = Vector3.Angle(base.transform.forward, to);
						gameObject2.transform.localEulerAngles = new Vector3(0f, 0f, 90f + num);
					}
					b = gameObject.transform.localPosition;
				}
				CooperationLevelWindow.selectCooperationLevel = allData[0].id;
				this.RefreshPayChapter(CooperationLevelWindow.selectCooperationLevel);
				this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
				this.RefreshDiffuseLock(allData[0]);
			}
			this.chapterName.text = LanguageDataProvider.GetValue(2069) + " - " + LanguageDataProvider.GetValue(data.name);
		}
	}

	private bool CheckAssistLevelUnLock(ChapterAssistConfig config)
	{
		if (config == null)
		{
			return false;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(config.level1);
		if (data == null)
		{
			return false;
		}
		string dependLevel = data.dependLevel;
		LevelConfig data2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(dependLevel);
		if (data2 == null)
		{
			return true;
		}
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(data2.chapter);
		return chapterInfo != null && Solarmax.Singleton<LevelDataHandler>.Instance.IsUnLock(dependLevel);
	}

	private void SetPlayerBaseInfo()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData != null)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
	}

	public void OnClickDifficult1(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.selectDiffuse == 1)
		{
			return;
		}
		if (this.difLock[0].activeSelf)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2110), 1f);
			return;
		}
		this.selectDiffuse = 1;
		this.SelectLine.transform.localPosition = new Vector3(this.Difficult1.gameObject.transform.localPosition.x, this.Difficult1.gameObject.transform.localPosition.y + 27f, 0f);
		this.RefreshDifficultLabelSize(this.selectDiffuse);
		this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
	}

	public void OnClickDifficult2(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.selectDiffuse == 2)
		{
			return;
		}
		if (this.difLock[1].activeSelf)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2110), 1f);
			return;
		}
		this.selectDiffuse = 2;
		this.SelectLine.transform.localPosition = new Vector3(this.Difficult2.gameObject.transform.localPosition.x, this.Difficult2.gameObject.transform.localPosition.y + 27f, 0f);
		this.RefreshDifficultLabelSize(this.selectDiffuse);
		this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
	}

	public void OnClickDifficult3(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.selectDiffuse == 3)
		{
			return;
		}
		if (this.difLock[2].activeSelf)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2111), 1f);
			return;
		}
		this.selectDiffuse = 3;
		this.SelectLine.transform.localPosition = new Vector3(this.Difficult3.gameObject.transform.localPosition.x, this.Difficult3.gameObject.transform.localPosition.y + 27f, 0f);
		this.RefreshDifficultLabelSize(this.selectDiffuse);
		this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
	}

	public void OnClickDifficult4(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.selectDiffuse == 4)
		{
			return;
		}
		if (this.difLock[3].activeSelf)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2112), 1f);
			return;
		}
		this.selectDiffuse = 4;
		this.SelectLine.transform.localPosition = new Vector3(this.Difficult4.gameObject.transform.localPosition.x, this.Difficult4.gameObject.transform.localPosition.y + 27f, 0f);
		this.RefreshDifficultLabelSize(this.selectDiffuse);
		this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
	}

	public void RefreshDifficultLabelSize(int index)
	{
		for (int i = 0; i < this.difLock.Length; i++)
		{
			if (i == index - 1)
			{
				this.difLock[i].transform.parent.transform.GetComponent<UILabel>().fontSize = 50;
			}
			else
			{
				this.difLock[i].transform.parent.transform.GetComponent<UILabel>().fontSize = 32;
			}
		}
	}

	private string GetMatchMap(ChapterAssistConfig config)
	{
		if (config == null)
		{
			return string.Empty;
		}
		if (this.selectDiffuse == 1)
		{
			return config.level1;
		}
		if (this.selectDiffuse == 2)
		{
			return config.level2;
		}
		if (this.selectDiffuse == 3)
		{
			return config.level3;
		}
		if (this.selectDiffuse == 4)
		{
			return config.level4;
		}
		return string.Empty;
	}

	public void OnFriendTeam()
	{
		if (string.IsNullOrEmpty(CooperationLevelWindow.selectCooperationLevel))
		{
			Tips.Make(Tips.TipsType.FlowUp, "请选择关卡后,再开始战斗!!!", 1f);
			return;
		}
		ChapterAssistConfig data = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetData(CooperationLevelWindow.selectCooperationLevel);
		if (data != null)
		{
			string matchMap = this.GetMatchMap(data);
			int num = data.playerNum;
			int userId = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId;
			Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(num);
			if (num == 2)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_2vPC, 2, false, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
			else if (num == 3)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_3vPC, 3, false, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
			else if (num == 4)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_4vPC, 4, false, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
			else if (num == 5)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_4vPC, 5, false, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
		}
	}

	public void OnQuikTeam()
	{
		if (string.IsNullOrEmpty(CooperationLevelWindow.selectCooperationLevel))
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1139), 1f);
			return;
		}
		ChapterAssistConfig data = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetData(CooperationLevelWindow.selectCooperationLevel);
		if (data != null)
		{
			string matchMap = this.GetMatchMap(data);
			int num = data.playerNum;
			Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(num);
			if (num == 2)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_2vPC, 2, true, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
			else if (num == 3)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_3vPC, 3, true, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
			else if (num == 4)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_4vPC, 4, true, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
			else if (num == 5)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, matchMap, CooperationType.CT_4vPC, 5, true, this.curDisplayChapterID, this.selectDiffuse, data.id, false);
			}
		}
	}

	public void OnClickBack()
	{
		if (!string.IsNullOrEmpty(CooperationLevelWindow.selectCooperationLevel))
		{
			ChapterAssistConfig data = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetData(CooperationLevelWindow.selectCooperationLevel);
			if (data != null && (data.playerNum == 2 || data.playerNum == 3 || data.playerNum == 4))
			{
				Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(data.playerNum);
			}
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationRoomWindow");
	}

	private void RefreshPayChapter(string levelAssistID)
	{
		for (int i = 0; i < this._cell.Count; i++)
		{
			CooperationAssistCell cooperationAssistCell = this._cell[i];
			if (cooperationAssistCell != null)
			{
				bool unLock = false;
				ChapterAssistConfig data = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetData(cooperationAssistCell.Config.id);
				if (data != null)
				{
					unLock = this.CheckAssistLevelUnLock(data);
				}
				int nStart = this.RefreshDiffuseStart(data);
				cooperationAssistCell.RefreshBtnStats(levelAssistID, unLock, nStart);
			}
		}
	}

	private void RefreshAssistInfo(string levelAssistID)
	{
		if (string.IsNullOrEmpty(levelAssistID))
		{
			return;
		}
		ChapterAssistConfig data = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetData(levelAssistID);
		if (data != null)
		{
			this.levelName.text = LanguageDataProvider.GetValue(data.name);
			this.SetRewardInfo(data);
			this.RefreshDiffuseLock(data);
		}
	}

	private void RefreshDiffuseLock(ChapterAssistConfig config)
	{
		if (config == null)
		{
			return;
		}
		bool flag = this.RefreshDiffuseLockOne(config.level1);
		if (flag)
		{
			this.difLock[0].SetActive(false);
		}
		else
		{
			this.difLock[0].SetActive(true);
		}
		flag = this.RefreshDiffuseLockOne(config.level2);
		if (flag)
		{
			this.difLock[1].SetActive(false);
		}
		else
		{
			this.difLock[1].SetActive(true);
		}
		flag = this.RefreshDiffuseLockOne(config.level3);
		if (flag)
		{
			this.difLock[2].SetActive(false);
		}
		else
		{
			this.difLock[2].SetActive(true);
		}
		flag = this.RefreshDiffuseLockOne(config.level4);
		if (flag)
		{
			this.difLock[3].SetActive(false);
		}
		else
		{
			this.difLock[3].SetActive(true);
		}
		int num = this.RefreshDiffuseStart(config);
		for (int i = 0; i < this.difFinishLabel.Length; i++)
		{
			if (i < num)
			{
				this.difFinishLabel[i].SetActive(true);
			}
			else
			{
				this.difFinishLabel[i].SetActive(false);
			}
		}
	}

	private int RefreshDiffuseStart(ChapterAssistConfig config)
	{
		if (config == null)
		{
			return 0;
		}
		int num = 0;
		bool flag = this.IsMabyDisplayStart(config.level1);
		if (flag)
		{
			num++;
		}
		flag = this.IsMabyDisplayStart(config.level2);
		if (flag)
		{
			num++;
		}
		flag = this.IsMabyDisplayStart(config.level3);
		if (flag)
		{
			num++;
		}
		flag = this.IsMabyDisplayStart(config.level4);
		if (flag)
		{
			num++;
		}
		return num;
	}

	private bool RefreshDiffuseLockOne(string levelID)
	{
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelID);
		if (data == null)
		{
			return false;
		}
		string dependLevel = data.dependLevel;
		LevelConfig data2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(dependLevel);
		if (data2 == null)
		{
			return true;
		}
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(data2.chapter);
		return chapterInfo != null && Solarmax.Singleton<LevelDataHandler>.Instance.IsUnLock(dependLevel);
	}

	private bool IsMabyDisplayStart(string levelID)
	{
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelID);
		if (data == null)
		{
			return false;
		}
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(data.chapter);
		return chapterInfo != null && Solarmax.Singleton<LevelDataHandler>.Instance.IsUnLock(levelID);
	}

	private void autoSelectDifficult()
	{
		if (!this.difLock[3].activeSelf)
		{
			this.selectDiffuse = 4;
			this.SelectLine.transform.localPosition = new Vector3(this.Difficult4.gameObject.transform.localPosition.x, this.Difficult4.gameObject.transform.localPosition.y + 27f, 0f);
			this.RefreshDifficultLabelSize(this.selectDiffuse);
			return;
		}
		if (!this.difLock[2].activeSelf)
		{
			this.selectDiffuse = 3;
			this.SelectLine.transform.localPosition = new Vector3(this.Difficult3.gameObject.transform.localPosition.x, this.Difficult3.gameObject.transform.localPosition.y + 27f, 0f);
			this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
			this.RefreshDifficultLabelSize(this.selectDiffuse);
			return;
		}
		if (!this.difLock[1].activeSelf)
		{
			this.selectDiffuse = 2;
			this.SelectLine.transform.localPosition = new Vector3(this.Difficult2.gameObject.transform.localPosition.x, this.Difficult2.gameObject.transform.localPosition.y + 27f, 0f);
			this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
			this.RefreshDifficultLabelSize(this.selectDiffuse);
			return;
		}
		if (this.difLock[0].activeSelf)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2110), 1f);
			return;
		}
		this.selectDiffuse = 1;
		this.SelectLine.transform.localPosition = new Vector3(this.Difficult1.gameObject.transform.localPosition.x, this.Difficult1.gameObject.transform.localPosition.y + 27f, 0f);
		this.RefreshAssistInfo(CooperationLevelWindow.selectCooperationLevel);
		this.RefreshDifficultLabelSize(this.selectDiffuse);
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnClickAddMoney()
	{
		this.isShowShop = true;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
		BGManager.Inst.ChangeBackground(BackgroundType.Normal);
	}

	private void SetRewardInfo(ChapterAssistConfig config)
	{
		if (config != null)
		{
			if (this.selectDiffuse == 1)
			{
				if (config.reward1 == 1)
				{
					this.reward.text = config.value1;
					this.rewardIcon.spriteName = "icon_jewelB";
				}
				else if (config.reward1 == 2)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value1;
				}
				else if (config.reward1 == 2)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value1;
				}
			}
			else if (this.selectDiffuse == 2)
			{
				if (config.reward2 == 1)
				{
					this.reward.text = config.value2;
					this.rewardIcon.spriteName = "icon_jewelB";
				}
				else if (config.reward2 == 2)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value2;
				}
				else if (config.reward2 == 2)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value2;
				}
			}
			else if (this.selectDiffuse == 3)
			{
				if (config.reward3 == 1)
				{
					this.reward.text = config.value3;
					this.rewardIcon.spriteName = "icon_jewelB";
				}
				else if (config.reward3 == 2)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value3;
				}
				else if (config.reward3 == 3)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value3;
				}
			}
			else if (this.selectDiffuse == 4)
			{
				if (config.reward4 == 1)
				{
					this.reward.text = config.value4;
					this.rewardIcon.spriteName = "icon_jewelB";
				}
				else if (config.reward4 == 2)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value4;
				}
				else if (config.reward4 == 3)
				{
					this.reward.text = string.Empty;
					this.rewardIcon.spriteName = config.value4;
				}
			}
		}
	}

	public GameObject LevelRoot;

	public GameObject template;

	public UILabel playerMoney;

	public UILabel chapterName;

	public UILabel playerNum;

	public UILabel reward;

	public UILabel levelName;

	public UISprite rewardIcon;

	public UIEventListener Difficult1;

	public UIEventListener Difficult2;

	public UIEventListener Difficult3;

	public UIEventListener Difficult4;

	public GameObject SelectLine;

	public GameObject[] difLock;

	public GameObject[] difFinishLabel;

	public GameObject lineUnit;

	private const float LINE_UNIT_LENGTH = 220f;

	private LineRenderer lineRender;

	private List<ChapterAssistConfig> _assistList = new List<ChapterAssistConfig>();

	private List<CooperationAssistCell> _cell = new List<CooperationAssistCell>();

	private int selectDiffuse = 1;

	private static string selectCooperationLevel = string.Empty;

	private string curDisplayChapterID;

	private GameObject lineGo;

	private bool isShowShop;
}
