using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class CooperationRoomWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnHaveNewChapterUnlocked);
		base.RegisterEvent(EventId.OnBackWindowName);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.table = this.LevelRoot.GetComponent<UITable>();
		this.SetPlayerBaseInfo();
		this.DisplayCapthers();
	}

	public override void OnHide()
	{
		this.Cells.Clear();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.UpdateMoney)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
		else if (eventId == EventId.OnHaveNewChapterUnlocked)
		{
			string chapter = (string)args[0];
			this.RefreshPayChapter(chapter);
		}
		else if (eventId == EventId.OnBackWindowName)
		{
			this.backWindow = (string)args[0];
		}
	}

	private void ClearTable()
	{
		this.LevelRoot.transform.DestroyChildren();
	}

	private void DisplayCapthers()
	{
		this.LevelRoot.transform.DestroyChildren();
		this.Cells.Clear();
		this.displayedChapters.Clear();
		List<ChapterAssistConfig> allData = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData();
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < allData.Count; i++)
		{
			ChapterAssistConfig chapterAssistConfig = allData[i];
			if (chapterAssistConfig != null && !string.IsNullOrEmpty(chapterAssistConfig.dependChapter))
			{
				hashSet.Add(chapterAssistConfig.dependChapter);
			}
		}
		List<ChapterInfo> coopertionList = Solarmax.Singleton<LevelDataHandler>.Instance.coopertionList;
		for (int i = 0; i < coopertionList.Count; i++)
		{
			ChapterInfo info = coopertionList[i];
			if (info == null || string.IsNullOrEmpty(info.id) || !hashSet.Contains(info.id))
			{
				continue;
			}
			GameObject gameObject = this.LevelRoot.AddChild(this.template);
			CooperationWidnowCell component = gameObject.GetComponent<CooperationWidnowCell>();
			gameObject.name = "num" + i;
			gameObject.SetActive(true);
			component.SetInfo(info);
			this.Cells.Add(gameObject);
			this.displayedChapters.Add(info);
		}
		this.table.Reposition();
		this.scrollView.ResetPosition();
	}

	private void SetPlayerBaseInfo()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData != null)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
	}

	public void OnClickBack()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow(this.backWindow);
	}

	private void RefreshPayChapter(string chapter)
	{
		for (int i = 0; i < this.Cells.Count && i < this.displayedChapters.Count; i++)
		{
			GameObject gameObject = this.Cells[i];
			CooperationWidnowCell component = gameObject.GetComponent<CooperationWidnowCell>();
			ChapterInfo chapterInfo = this.displayedChapters[i];
			if (chapterInfo.id.Equals(chapter))
			{
				component.SetInfo(chapterInfo);
			}
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

	public GameObject LevelRoot;

	public GameObject template;

	public UIScrollView scrollView;

	public UILabel playerMoney;

	private string backWindow = "HomeWindow";

	private List<GameObject> Cells = new List<GameObject>();

	private List<ChapterInfo> displayedChapters = new List<ChapterInfo>();

	private UITable table;
}
