using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class RankWindow : BaseWindow
{
	private void Awake()
	{
		this.rankList = new List<PlayerData>();
		this.pvpTab.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnTabClick);
		this.starTab.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnTabClick);
		this.challengeTab.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnTabClick);
		this.template.gameObject.SetActive(false);
		for (int i = 0; i < this.wrapContent.transform.childCount; i++)
		{
			this.templates.Add(this.wrapContent.transform.GetChild(i));
		}
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.LoadRankList);
		base.RegisterEvent(EventId.OnFriendFollowResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.model = Solarmax.Singleton<RankModel>.Get();
		this.model.Init();
		this.wrapContent.enabled = false;
		this.OnTabClick(this.pvpTab.gameObject);
	}

	public override void OnHide()
	{
		this.model.Clear();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.LoadRankList)
		{
			List<PlayerData> datas = args[0] as List<PlayerData>;
			this.model.AddCurrentTypeData(datas);
			if (this.switchTab)
			{
				this.switchTab = false;
				int num = (int)args[1] + 1;
				int selfInfo = (int)args[2] + 1;
				this.UpdateUI();
				this.SetSelfInfo(selfInfo);
			}
			else
			{
				this.wrapContent.maxIndex = 0;
				this.wrapContent.minIndex = -(this.model.GetCurrentTypeDataCount() - 1);
			}
		}
		else if (eventId == EventId.OnFriendFollowResult)
		{
			int num2 = (int)args[0];
			bool flag = (bool)args[1];
			ErrCode errCode = (ErrCode)args[2];
			if (errCode == ErrCode.EC_Ok && flag)
			{
				foreach (Transform transform in this.templates)
				{
					if (transform.gameObject.activeSelf)
					{
						RankWindowCell component = transform.gameObject.GetComponent<RankWindowCell>();
						component.OnAddFriendSuccess();
					}
				}
			}
		}
	}

	private void SetSelfInfo(int self)
	{
		if (self <= 0)
		{
			this.selfRankValue.text = string.Empty;
		}
		else
		{
			this.selfRankValue.text = self.ToString();
		}
		for (int i = 0; i < this.rankIcon.Length; i++)
		{
			if (i == this.chooseType)
			{
				this.rankIcon[i].SetActive(true);
			}
			else
			{
				this.rankIcon[i].SetActive(false);
			}
		}
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		if (this.chooseType == 0)
		{
			this.selfNumValue.text = playerData.score.ToString();
		}
		else if (this.chooseType == 1)
		{
			this.selfNumValue.text = Solarmax.Singleton<LevelDataHandler>.Instance.allStars.ToString();
		}
		else if (this.chooseType == 2)
		{
			UILabel uilabel = this.selfNumValue;
			string text = Solarmax.Singleton<LevelDataHandler>.Instance.GetChapterCompletedChallenges().ToString();
			this.selfNumValue.text = text;
			uilabel.text = text;
		}
		this.titleTable.Reposition();
	}

	private void UpdateUI()
	{
		this.wrapContent.enabled = true;
		this.ResetTemplates();
		List<PlayerData> currentTypeData = this.model.GetCurrentTypeData();
		this.wrapContent.maxIndex = 0;
		this.wrapContent.minIndex = -(currentTypeData.Count - 1);
		int num;
		if (currentTypeData.Count > 15)
		{
			this.wrapContent.onInitializeItem = new UIWrapContent.OnInitializeItem(this.OnDrageItem);
			num = 15;
		}
		else
		{
			num = currentTypeData.Count;
		}
		for (int i = 0; i < 15; i++)
		{
			this.wrapContent.transform.GetChild(i).gameObject.SetActive(i < currentTypeData.Count);
		}
		for (int j = 0; j < num; j++)
		{
			this.UpdateChildTemplate(j, j);
		}
		this.scrollView.ResetPosition();
	}

	private void ResetTemplates()
	{
		int num = 0;
		foreach (Transform transform in this.templates)
		{
			transform.transform.localPosition = new Vector3(0f, (float)(-(float)num * 160), 0f);
			num++;
		}
	}

	private void OnDrageItem(GameObject gameObject, int wrapIndex, int realIndex)
	{
		this.UpdateChildTemplate(wrapIndex, realIndex);
		if (-realIndex > this.model.GetCurrentTypeDataCount() - 5)
		{
			this.OnScrollViewShowMore();
		}
	}

	private void UpdateChildTemplate(int wrapIndex, int realIndex)
	{
		List<PlayerData> currentTypeData = this.model.GetCurrentTypeData();
		realIndex = Mathf.Abs(realIndex);
		Transform child = this.wrapContent.transform.GetChild(wrapIndex);
		RankWindowCell component = child.GetComponent<RankWindowCell>();
		if (realIndex < currentTypeData.Count)
		{
			PlayerData playerData = currentTypeData[realIndex];
			component.SetCell(realIndex + 1, playerData.userId, playerData.name, playerData.score, playerData.icon, this.chooseType, this.scrollView);
		}
	}

	private void OnTabClick(GameObject go)
	{
		if (this.selectTab != null && go == this.selectTab)
		{
			return;
		}
		this.rankList.Clear();
		this.dicCell.Clear();
		this.switchTab = true;
		this.selectTab = go;
		if (go == this.pvpTab.gameObject)
		{
			this.chooseType = 0;
			this.model.currentType = RankModel.RankType.PVP;
			Solarmax.Singleton<NetSystem>.Instance.helper.LoadRank(this.model.GetCurrentTypeDataCount(), 0);
		}
		else if (go == this.starTab.gameObject)
		{
			this.chooseType = 1;
			this.model.currentType = RankModel.RankType.Challenage;
			Solarmax.Singleton<NetSystem>.Instance.helper.LoadRank(this.model.GetCurrentTypeDataCount(), 1);
		}
		else if (go == this.challengeTab.gameObject)
		{
			this.chooseType = 2;
			this.model.currentType = RankModel.RankType.Star;
			Solarmax.Singleton<NetSystem>.Instance.helper.LoadRank(this.model.GetCurrentTypeDataCount(), 2);
		}
		for (int i = 0; i < 15; i++)
		{
			this.wrapContent.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	private void OnScrollViewShowMore()
	{
		DateTime? dateTime = this.searchFriendTime;
		if (dateTime != null && (Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchFriendTime.Value).TotalSeconds < 2.0)
		{
			return;
		}
		this.searchFriendTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		Solarmax.Singleton<NetSystem>.Instance.helper.LoadRank(this.model.GetCurrentTypeDataCount(), this.chooseType);
	}

	public GameObject pvpTab;

	public GameObject starTab;

	public GameObject challengeTab;

	public UIScrollView scrollView;

	public UIGrid grid;

	public RankWindowCell template;

	public UILabel selfRankValue;

	private DateTime? searchFriendTime;

	public UILabel selfNumValue;

	public GameObject[] rankIcon;

	public UITable titleTable;

	public UIWrapContent wrapContent;

	private const int MAX_REUSE = 15;

	private const int TEMPLATE_HEIGHT = 160;

	private GameObject selectTab;

	private List<PlayerData> rankList;

	private int chooseType;

	private Dictionary<int, RankWindowCell> dicCell = new Dictionary<int, RankWindowCell>();

	private RankModel model;

	private List<Transform> templates = new List<Transform>();

	private bool switchTab;
}
