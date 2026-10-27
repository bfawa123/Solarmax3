using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ReplayWindow : BaseWindow
{
	private void Awake()
	{
		this.selectTab = null;
		this.myReportList = new List<BattleReportData>();
		this.hotReportList = new List<BattleReportData>();
		this.myToggle.onChange.Add(new EventDelegate(delegate()
		{
			this.OnMyToggle();
		}));
		this.hotToggle.onChange.Add(new EventDelegate(delegate()
		{
			this.OnHotToggle();
		}));
		this.scrollView.onShowMore = new UIScrollView.OnDragNotification(this.OnReportShowMore);
		this.scrollView.onShowLess = new UIScrollView.OnDragNotification(this.OnReportShowLess);
		ReplayCollectManager.Get().LoadReplayFromDisk();
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnBattleReportLoad);
		base.RegisterEvent(EventId.OnBattleReportPlay);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (this.hotReportList.Count <= 0)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.BattleReportLoad(false, 0);
		}
		if (this.selectTab == null)
		{
			this.selectTab = this.myTab;
			this.SetPageInfo(this.myTab.gameObject);
		}
		else
		{
			this.SetPageInfo(this.selectTab.gameObject);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnBattleReportLoad)
		{
			bool flag = (bool)args[0];
			List<BattleReportData> list = args[2] as List<BattleReportData>;
			this.hotReportList.AddRange(list);
			if (this.selectTab == this.hotTab)
			{
				this.RefreshScrollView(list, true);
			}
			if (list.Count > 0)
			{
				this.ncurPageNum++;
			}
			int num = this.hotReportList.Count % 10;
			this.nmaxPageNum = this.hotReportList.Count / 10;
			if (num > 0)
			{
				this.nmaxPageNum++;
			}
		}
		else if (eventId == EventId.OnBattleReportPlay)
		{
			this.frames = (args[0] as PbSCFrames);
			Solarmax.Singleton<UISystem>.Get().ShowWindow("UnTouchWindow");
			Solarmax.Singleton<BattleSystem>.Instance.replayManager.TryPlayRecord(this.frames, false);
		}
	}

	private void RefreshScrollView(List<BattleReportData> data, bool useOldData)
	{
		if (data.Count <= 0)
		{
			return;
		}
		if (this.displays.Count <= 0)
		{
			for (int i = 0; i < 10; i++)
			{
				GameObject gameObject = this.grid.gameObject.AddChild(this.template);
				gameObject.SetActive(false);
				this.displays.Add(gameObject);
			}
		}
		int num = 0;
		int count = data.Count;
		while (num < count && num < this.displays.Count)
		{
			this.displays[num].SetActive(true);
			ReplayWindowCell component = this.displays[num].GetComponent<ReplayWindowCell>();
			component.SetInfo(num, data[num], false, this.scrollView);
			num++;
		}
		this.grid.Reposition();
		this.scrollView.ResetPosition();
	}

	private void RefreshScrollViewLocal()
	{
		this.grid.transform.DestroyChildren();
		this.displays.Clear();
		List<ReplayFile> localRecordList = ReplayCollectManager.Get().localRecordList;
		int i = 0;
		int count = localRecordList.Count;
		while (i < count)
		{
			ReplayFile replayFile = localRecordList[i];
			GameObject gameObject = this.grid.gameObject.AddChild(this.template);
			gameObject.SetActive(false);
			ReplayWindowCell component = gameObject.GetComponent<ReplayWindowCell>();
			component.SetInfoLocal(i, replayFile.fileName, replayFile.levelId, replayFile.timeStamp, replayFile.nWin, this.scrollView);
			i++;
		}
		this.grid.Reposition();
		this.scrollView.ResetPosition();
	}

	private void DisplayCurPage()
	{
		if (this.ncurPageNum <= 0 || this.ncurPageNum > this.nmaxPageNum)
		{
			return;
		}
		int num = (this.ncurPageNum - 1) * 10;
		int num2 = this.ncurPageNum * 10;
		List<BattleReportData> list = new List<BattleReportData>();
		int num3 = num;
		while (num3 < num2 && num3 < this.hotReportList.Count)
		{
			list.Add(this.hotReportList[num3]);
			num3++;
		}
		this.RefreshScrollView(list, true);
	}

	private void OnHotToggle()
	{
		if (this.myToggle.value)
		{
			this.selectTab = this.myTab;
			this.SetPageInfo(this.myTab.gameObject);
		}
		else if (this.hotToggle.value)
		{
			this.selectTab = this.hotTab;
			this.SetPageInfo(this.selectTab.gameObject);
		}
	}

	private void OnMyToggle()
	{
		if (this.myToggle.value)
		{
			this.selectTab = this.myTab;
			this.SetPageInfo(this.myTab.gameObject);
		}
		else if (this.hotToggle.value)
		{
			this.selectTab = this.hotTab;
			this.SetPageInfo(this.selectTab.gameObject);
		}
	}

	private void SetPageInfo(GameObject go)
	{
		this.myTab.color = this.unselectColor;
		this.hotTab.color = this.unselectColor;
		if (go.name.Equals(this.myTab.gameObject.name))
		{
			this.selectTab = this.myTab;
		}
		else
		{
			this.selectTab = this.hotTab;
		}
		this.grid.transform.DestroyChildren();
		this.displays.Clear();
		this.selectTab.color = this.selectColor;
		if (this.selectTab == this.hotTab)
		{
			this.RefreshScrollView(this.hotReportList, true);
		}
		else
		{
			this.RefreshScrollViewLocal();
		}
	}

	private void OnReportShowMore()
	{
		if (this.selectTab == this.hotTab)
		{
			if (this.ncurPageNum >= this.nmaxPageNum)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.BattleReportLoad(false, this.hotReportList.Count);
			}
			if (this.ncurPageNum < this.nmaxPageNum)
			{
				this.ncurPageNum++;
				this.DisplayCurPage();
			}
		}
	}

	private void OnReportShowLess()
	{
		if (this.selectTab == this.hotTab)
		{
			if (this.ncurPageNum > 1 && this.ncurPageNum <= this.nmaxPageNum)
			{
				this.ncurPageNum--;
				this.DisplayCurPage();
			}
			else if (this.ncurPageNum == 1)
			{
				this.ncurPageNum = 0;
				this.hotReportList.Clear();
				Solarmax.Singleton<NetSystem>.Instance.helper.BattleReportLoad(false, 0);
			}
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
	}

	public UILabel myTab;

	public UILabel hotTab;

	public UIToggle myToggle;

	public UIToggle hotToggle;

	public GameObject template;

	public UIScrollView scrollView;

	public UIGrid grid;

	private UILabel selectTab;

	private int ncurPageNum;

	private int nmaxPageNum;

	private List<BattleReportData> myReportList;

	private List<BattleReportData> hotReportList;

	private PbSCFrames frames;

	private Color selectColor = new Color(1f, 1f, 1f, 1f);

	private Color unselectColor = new Color(1f, 1f, 1f, 0.5f);

	private List<GameObject> displays = new List<GameObject>();
}
