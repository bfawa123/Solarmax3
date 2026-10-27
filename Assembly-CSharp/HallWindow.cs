using System;
using System.Collections.Generic;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class HallWindow : BaseWindow
{
	private void Awake()
	{
		this.scrollView.onShowMore = new UIScrollView.OnDragNotification(this.OnReportShowMore);
		this.scrollView.onShowLess = new UIScrollView.OnDragNotification(this.OnReportShowLess);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnSearchBattleRoomsResult);
		base.RegisterEvent(EventId.OnSearchBattleRoomsNull);
		base.RegisterEvent(EventId.OnModifyRoomNameResult);
		base.RegisterEvent(EventId.UpdateMoney);
		return true;
	}

	private void InitChapterList()
	{
		this.popupChapters.Clear();
		string value = LanguageDataProvider.GetValue(2204);
		this.popupChapters.AddItem(value, string.Empty);
		List<ChapterAssistConfig> assistConfigs = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData();
		HashSet<string> chapterIds = new HashSet<string>();
		for (int i = 0; i < assistConfigs.Count; i++)
		{
			if (assistConfigs[i] != null && !string.IsNullOrEmpty(assistConfigs[i].dependChapter))
			{
				chapterIds.Add(assistConfigs[i].dependChapter);
			}
		}
		List<ChapterInfo> coopertionList = Solarmax.Singleton<LevelDataHandler>.Instance.coopertionList;
		for (int i = 0; i < coopertionList.Count; i++)
		{
			ChapterInfo chapterInfo = coopertionList[i];
			if (!chapterIds.Contains(chapterInfo.id))
			{
				continue;
			}
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
			if (data == null)
			{
				Debug.LogError("error read config " + chapterInfo.id);
				return;
			}
			string value2 = LanguageDataProvider.GetValue(data.name);
			this.popupChapters.AddItem(value2, chapterInfo.id);
		}
		this.chapterName.text = value;
	}

	private void InitRoomModesList()
	{
		this.popupChapters.Clear();
		string value = LanguageDataProvider.GetValue(2203);
		this.popupChapters.AddItem(value, 7);
		this.popupChapters.AddItem(LanguageDataProvider.GetValue(2143), 0);
		this.popupChapters.AddItem(LanguageDataProvider.GetValue(2144), 1);
		this.popupChapters.AddItem(LanguageDataProvider.GetValue(2072), 2);
		this.popupChapters.AddItem(LanguageDataProvider.GetValue(2035), 3);
		this.chapterName.text = value;
	}

	private string GetRoomType2String(CooperationType eType)
	{
		if (eType == CooperationType.CT_1v1)
		{
			return LanguageDataProvider.GetValue(2143);
		}
		if (eType == CooperationType.CT_2v2)
		{
			return LanguageDataProvider.GetValue(2144);
		}
		if (eType == CooperationType.CT_1v1v1)
		{
			return LanguageDataProvider.GetValue(2072);
		}
		if (eType == CooperationType.CT_1v1v1v1)
		{
			return LanguageDataProvider.GetValue(2035);
		}
		return LanguageDataProvider.GetValue(2203);
	}

	private void InitChapterLevelList(string chapterID)
	{
		this.popupLevels.Clear();
		string value = LanguageDataProvider.GetValue(2205);
		this.popupLevels.AddItem(value, string.Empty);
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(chapterID);
		if (chapterInfo != null)
		{
			List<ChapterAssistConfig> allData = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData(chapterInfo.id);
			if (allData != null && allData.Count > 0)
			{
				for (int i = 0; i < allData.Count; i++)
				{
					ChapterAssistConfig chapterAssistConfig = allData[i];
					string value2 = LanguageDataProvider.GetValue(chapterAssistConfig.name);
					this.popupLevels.AddItem(value2, chapterAssistConfig.id);
				}
			}
		}
		this.levelName.text = value;
	}

	private void InitLevelDiffuseList()
	{
		this.popupDiffuse.Clear();
		string value = LanguageDataProvider.GetValue(2206);
		this.popupDiffuse.AddItem(value, -1);
		this.popupDiffuse.AddItem(LanguageDataProvider.GetValue(2104), 1);
		this.popupDiffuse.AddItem(LanguageDataProvider.GetValue(2105), 2);
		this.popupDiffuse.AddItem(LanguageDataProvider.GetValue(2106), 3);
		this.popupDiffuse.AddItem(LanguageDataProvider.GetValue(2107), 4);
		this.diffuseName.text = value;
	}

	private string GetDiffuse2String(int nDif)
	{
		if (nDif == 1)
		{
			return LanguageDataProvider.GetValue(2104);
		}
		if (nDif == 2)
		{
			return LanguageDataProvider.GetValue(2105);
		}
		if (nDif == 3)
		{
			return LanguageDataProvider.GetValue(2106);
		}
		if (nDif == 4)
		{
			return LanguageDataProvider.GetValue(2107);
		}
		return LanguageDataProvider.GetValue(2206);
	}

	public override void Release()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = string.Empty;
		this.uiToggleAllrooms.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnSearchAllRooms)));
		this.uiToggleCoopertion.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnSearchCooperationType)));
		this.uiToggleRoomType.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnSearchRoomType)));
		this.xButton.SetActive(false);
		this.RefreshPlayerInfo();
		this.UpdateSelectPanel();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnSearchBattleRoomsResult)
		{
			IList<MatchSynopsis> list = (IList<MatchSynopsis>)args[0];
			if (list.Count > 0)
			{
				this.ncurPageNum = (int)args[1] / 12;
			}
			int num = (int)args[2];
			if (num == 1)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2233), 1f);
			}
			this.SetPage(list);
		}
		else if (eventId == EventId.OnSearchBattleRoomsNull)
		{
		}
		if (eventId == EventId.UpdateMoney)
		{
			this.RefreshPlayerInfo();
		}
	}

	private void OnReportShowMore()
	{
		this.requestType = 0;
		int num = this.ncurPageNum + 1;
		this.OnLoadBattleRoomsRequest(0, num * 12, CooperationType.CT_Null, string.Empty, null, -1);
	}

	private void OnReportShowLess()
	{
		if (this.ncurPageNum > 1)
		{
			this.requestType = 0;
			int num = this.ncurPageNum - 1;
			this.OnLoadBattleRoomsRequest(0, num * 12, CooperationType.CT_Null, string.Empty, null, -1);
		}
	}

	public override void OnHide()
	{
		base.UnRegisterEvent(EventId.OnSearchBattleRoomsResult);
		base.UnRegisterEvent(EventId.OnSearchBattleRoomsNull);
		base.UnRegisterEvent(EventId.OnModifyRoomNameResult);
		base.UnRegisterEvent(EventId.UpdateMoney);
	}

	private void RefreshPlayerInfo()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		if (playerData == null)
		{
			return;
		}
		this.playerMoney.text = playerData.money.ToString();
	}

	public void OnBackClick()
	{
		Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
	}

	public void OnClearInputClicked()
	{
		this.inputLabel.text = string.Empty;
		this.inputSearch.value = string.Empty;
		this.xButton.SetActive(false);
	}

	public void OnInputClick()
	{
		string text = this.inputSearch.value.Trim();
		text = text.Replace('\r', ' ');
		text = text.Replace('\t', ' ');
		text = text.Replace('\n', ' ');
		while (this.EncodingTextLength(text) > 20)
		{
			text = text.Substring(0, text.Length - 1);
		}
		this.inputLabel.text = text;
		if (!string.IsNullOrEmpty(text) && !this.xButton.activeSelf)
		{
			this.xButton.SetActive(true);
		}
	}

	private bool CheckOptLimit()
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

	public void OnSearchAllRooms()
	{
		if (!this.uiToggleAllrooms.value)
		{
			return;
		}
		this.requestType = 0;
		this.selectType = 0;
		this.ncurPageNum = 0;
		this.selectMode = HallWindow.SELECTMODE.ALL;
		this.UpdateSelectPanel();
		this.OnLoadBattleRoomsRequest(0, 0, CooperationType.CT_Null, string.Empty, null, -1);
		this.DisplayNullPage();
	}

	public void OnSearchCooperationType()
	{
		if (!this.uiToggleCoopertion.value)
		{
			return;
		}
		this.requestType = 0;
		this.selectType = 1;
		this.ncurPageNum = 0;
		this.selectMode = HallWindow.SELECTMODE.CooperationType;
		this.UpdateSelectPanel();
		this.InitChapterList();
		this.InitLevelDiffuseList();
		this.OnLoadBattleRoomsRequest(1, 0, CooperationType.CT_Null, string.Empty, null, -1);
		this.DisplayNullPage();
	}

	public void OnSearchRoomType()
	{
		if (!this.uiToggleRoomType.value)
		{
			return;
		}
		this.requestType = 0;
		this.selectType = 2;
		this.ncurPageNum = 0;
		this.selectMode = HallWindow.SELECTMODE.Room;
		this.UpdateSelectPanel();
		this.InitRoomModesList();
		this.OnLoadBattleRoomsRequest(2, 0, CooperationType.CT_Null, string.Empty, null, -1);
		this.DisplayNullPage();
	}

	private void UpdateSelectPanel()
	{
		if (this.selectMode == HallWindow.SELECTMODE.ALL)
		{
			this.TypeSelect.SetActive(false);
		}
		else if (this.selectMode == HallWindow.SELECTMODE.CooperationType)
		{
			this.TypeSelect.SetActive(true);
			this.popupLevels.Clear();
			this.popupChapters.gameObject.SetActive(true);
			this.popupLevels.gameObject.SetActive(true);
			this.popupDiffuse.gameObject.SetActive(true);
		}
		else
		{
			this.TypeSelect.SetActive(true);
			this.popupChapters.gameObject.SetActive(true);
			this.popupLevels.gameObject.SetActive(false);
			this.popupDiffuse.gameObject.SetActive(false);
		}
		this.selectParamChapter = string.Empty;
		this.selectParamLevel = string.Empty;
		this.selectParamDiffuse = -1;
		this.selectParamRoom = CooperationType.CT_Null;
	}

	public void OnSearchByroomID()
	{
		if (!this.CheckOptLimit())
		{
			return;
		}
		string value = this.inputSearch.value;
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.OnSearchRoomRequest(value);
		this.DisplayNullPage();
	}

	public void OnCreateRoom()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("HallWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HallSelectionWindow");
	}

	public void OnSelectParamChapter()
	{
		this.requestType = 0;
		if (this.selectMode == HallWindow.SELECTMODE.Room)
		{
			int num = (int)this.popupChapters.data;
			if (num == (int)this.selectParamRoom)
			{
				return;
			}
			this.selectParamRoom = (CooperationType)num;
			this.chapterName.text = this.GetRoomType2String(this.selectParamRoom);
			this.OnLoadBattleRoomsRequest(2, 0, this.selectParamRoom, this.selectParamChapter, this.selectParamLevel, this.selectParamDiffuse);
			this.DisplayNullPage();
		}
		else
		{
			string value = (string)this.popupChapters.data;
			if (string.IsNullOrEmpty(value))
			{
				this.selectParamChapter = string.Empty;
				string value2 = LanguageDataProvider.GetValue(2204);
				this.chapterName.text = value2;
				this.InitChapterLevelList(string.Empty);
				this.OnLoadBattleRoomsRequest(1, 0, CooperationType.CT_Null, string.Empty, this.selectParamLevel, this.selectParamDiffuse);
				this.DisplayNullPage();
				return;
			}
			if (this.selectParamChapter.Equals(value))
			{
				return;
			}
			this.selectParamChapter = value;
			this.InitChapterLevelList(this.selectParamChapter);
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.selectParamChapter);
			if (data == null)
			{
				Debug.LogError("error read config " + this.selectParamChapter);
				return;
			}
			this.chapterName.text = LanguageDataProvider.GetValue(data.name);
			this.OnLoadBattleRoomsRequest(1, 0, CooperationType.CT_Null, this.selectParamChapter, this.selectParamLevel, this.selectParamDiffuse);
			this.DisplayNullPage();
		}
	}

	public void OnSelectParamLevel()
	{
		this.requestType = 0;
		string value = (string)this.popupLevels.data;
		if (string.IsNullOrEmpty(value))
		{
			this.selectParamLevel = string.Empty;
			this.levelName.text = LanguageDataProvider.GetValue(2205);
			this.OnLoadBattleRoomsRequest(1, 0, CooperationType.CT_Null, this.selectParamChapter, string.Empty, this.selectParamDiffuse);
			this.DisplayNullPage();
			return;
		}
		if (this.selectParamLevel.Equals(value))
		{
			return;
		}
		this.selectParamLevel = value;
		ChapterAssistConfig data = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetData(this.selectParamLevel);
		if (data == null)
		{
			Debug.LogError("error read ChapterAssistConfig " + this.selectParamChapter);
			return;
		}
		string value2 = LanguageDataProvider.GetValue(data.name);
		this.levelName.text = value2;
		this.OnLoadBattleRoomsRequest(1, 0, CooperationType.CT_Null, this.selectParamChapter, this.selectParamLevel, this.selectParamDiffuse);
		this.DisplayNullPage();
	}

	public void OnSelectParamDiffuse()
	{
		int num = (int)this.popupDiffuse.data;
		if (this.selectParamDiffuse == num)
		{
			return;
		}
		this.requestType = 0;
		this.ncurPageNum = 0;
		this.selectParamDiffuse = num;
		this.diffuseName.text = this.GetDiffuse2String(this.selectParamDiffuse);
		this.OnLoadBattleRoomsRequest(1, 0, CooperationType.CT_Null, this.selectParamChapter, this.selectParamLevel, this.selectParamDiffuse);
		this.DisplayNullPage();
	}

	public void UpdateCurPageData()
	{
		if (!this.CheckOptLimit())
		{
			return;
		}
		this.DisplayNullPage();
		this.requestType = 1;
		if (this.selectMode == HallWindow.SELECTMODE.Room)
		{
			this.OnLoadBattleRoomsRequest(this.selectType, this.ncurPageNum, this.selectParamRoom, string.Empty, null, -1);
		}
		else
		{
			this.OnLoadBattleRoomsRequest(this.selectType, this.ncurPageNum, CooperationType.CT_Null, this.selectParamChapter, this.selectParamLevel, this.selectParamDiffuse);
		}
	}

	private void DisplayNullPage()
	{
		int count = this.cacheList.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.cacheList[i];
			gameObject.SetActive(false);
		}
		this.RepositionList();
	}

	private void SetPage(IList<MatchSynopsis> list)
	{
		int count = this.cacheList.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = this.cacheList[i];
			gameObject.SetActive(false);
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2;
			if (j < count)
			{
				gameObject2 = this.cacheList[j];
			}
			else
			{
				gameObject2 = this.parent.AddChild(this.template);
				gameObject2.SetActive(true);
				gameObject2.name = "infomation_" + j.ToString();
				this.cacheList.Add(gameObject2);
			}
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
				HallRoolCell component = gameObject2.GetComponent<HallRoolCell>();
				component.SetInfoLocal(list[j]);
			}
		}
		this.RepositionList();
	}

	private void RepositionList()
	{
		UITable uitable = null;
		if (this.parent != null)
		{
			uitable = this.parent.GetComponent<UITable>();
			if (uitable == null)
			{
				uitable = this.parent.GetComponentInChildren<UITable>(true);
			}
			if (uitable == null)
			{
				uitable = this.parent.GetComponentInParent<UITable>();
			}
		}
		if (uitable != null)
		{
			uitable.Reposition();
		}
		UIGrid uigrid = null;
		if (this.parent != null)
		{
			uigrid = this.parent.GetComponent<UIGrid>();
			if (uigrid == null)
			{
				uigrid = this.parent.GetComponentInChildren<UIGrid>(true);
			}
			if (uigrid == null)
			{
				uigrid = this.parent.GetComponentInParent<UIGrid>();
			}
		}
		if (uigrid != null)
		{
			uigrid.Reposition();
		}
		if (this.scrollView != null)
		{
			this.scrollView.UpdateScrollbars(true);
			this.scrollView.ResetPosition();
		}
	}

	private int EncodingTextLength(string s)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(s.Substring(i, 1));
			num += ((bytes.Length <= 1) ? 1 : 2);
		}
		return num;
	}

	public void OnLoadBattleRoomsRequest(int type, int start = 0, CooperationType cType = CooperationType.CT_Null, string chapterID = "", string strLevel = null, int diffculty = -1)
	{
		CSLoadMatchList csloadMatchList = new CSLoadMatchList();
		csloadMatchList.type = type;
		csloadMatchList.c_type = cType;
		csloadMatchList.chapter = chapterID;
		csloadMatchList.difficulty = diffculty;
		csloadMatchList.start = start;
		csloadMatchList.level = strLevel;
		csloadMatchList.optype = this.requestType;
		Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSLoadMatchList>(320, csloadMatchList);
	}

	public UILabel playerMoney;

	public UILabel inputLabel;

	public UIInput inputSearch;

	public GameObject xButton;

	public global::Ping PingView;

	public UIToggle uiToggleAllrooms;

	public UIToggle uiToggleCoopertion;

	public UIToggle uiToggleRoomType;

	public GameObject parent;

	public GameObject template;

	public UIScrollView scrollView;

	public GameObject TypeSelect;

	public UIPopupList popupChapters;

	public UILabel chapterName;

	public UIPopupList popupLevels;

	public UILabel levelName;

	public UIPopupList popupDiffuse;

	public UILabel diffuseName;

	private List<GameObject> cacheList = new List<GameObject>();

	private DateTime? searchFriendTime;

	private int selectType;

	private string selectParamChapter = string.Empty;

	private string selectParamLevel = string.Empty;

	private int selectParamDiffuse = -1;

	private CooperationType selectParamRoom = CooperationType.CT_Null;

	private HallWindow.SELECTMODE selectMode;

	private int ncurPageNum;

	private int requestType;

	private enum SELECTMODE
	{
		ALL,
		CooperationType,
		Room
	}
}
