using System;
using System.Collections.Generic;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class FriendWindow : BaseWindow
{
	private void Awake()
	{
		this.playerList = new List<SimplePlayerData>();
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnFriendLoadResult);
		base.RegisterEvent(EventId.OnFriendSearchResult);
		base.RegisterEvent(EventId.OnFriendSearchResultAll);
		base.RegisterEvent(EventId.OnFriendFollowResult);
		base.RegisterEvent(EventId.OnFriendNotifyResult);
		base.RegisterEvent(EventId.OnFriendStateNotice);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.SetInfo();
		this.searchFriendTime = null;
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendLoad(0, false);
	}

	public override void OnHide()
	{
		this.playerList.Clear();
		this.cellList.Clear();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		switch (eventId)
		{
		case EventId.OnFriendFollowResult:
			if (this.deleteButton.activeSelf && this.isSearchFriend)
			{
				string value = this.inputComp.value;
				if (value == string.Empty)
				{
					List<SimplePlayerData> myFriendList = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
					this.grid.transform.DestroyChildren();
					this.RefreshScrollView(myFriendList);
				}
				else
				{
					Solarmax.Singleton<NetSystem>.Instance.helper.FriendSearch(value, 0, 0);
				}
			}
			else
			{
				List<SimplePlayerData> myFriendList2 = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
				this.grid.transform.DestroyChildren();
				this.RefreshScrollView(myFriendList2);
			}
			break;
		case EventId.OnFriendLoadResult:
		{
			List<SimplePlayerData> myFriendList3 = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
			this.grid.transform.DestroyChildren();
			this.RefreshScrollView(myFriendList3);
			break;
		}
		default:
			if (eventId == EventId.UpdateMoney)
			{
				this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			}
			break;
		case EventId.OnFriendSearchResultAll:
		{
			List<SCFriendSearch> list = (List<SCFriendSearch>)args[0];
			List<SimplePlayerData> myFriendList4 = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
			List<SimplePlayerData> list2 = new List<SimplePlayerData>();
			string value2 = this.inputComp.value;
			foreach (SimplePlayerData simplePlayerData in myFriendList4)
			{
				if (simplePlayerData.name.Contains(value2) || value2.Contains(simplePlayerData.name))
				{
					list2.Add(simplePlayerData);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				SimplePlayerData simplePlayerData2 = new SimplePlayerData();
				simplePlayerData2.Init(list[i].data);
				if (simplePlayerData2 != null && !Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(simplePlayerData2.userId))
				{
					list2.Add(simplePlayerData2);
				}
			}
			if (list2.Count > 0)
			{
				this.grid.transform.DestroyChildren();
				this.RefreshScrollView(list2);
				this.isSearchFriend = true;
			}
			list.Clear();
			break;
		}
		case EventId.OnFriendSearchResult:
		{
			SimplePlayerData simplePlayerData3 = args[0] as SimplePlayerData;
			bool flag = (bool)args[1];
			int num = (int)args[2];
			int num2 = (int)args[3];
			int num3 = (int)args[4];
			int num4 = (int)args[5];
			int num5 = (int)args[6];
			int num6 = (int)args[8];
			if (simplePlayerData3 != null)
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendFindWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendSearchResultShow, new object[]
				{
					simplePlayerData3,
					flag,
					num,
					num2,
					num3,
					num4,
					num5,
					true,
					num6
				});
			}
			break;
		}
		case EventId.OnFriendStateNotice:
		{
			List<SimplePlayerData> myFriendList5 = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
			this.RefreshFriendStatus(myFriendList5);
			break;
		}
		}
	}

	private void SetInfo()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData != null)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
	}

	public void OnInputClick()
	{
		if (this.inputComp.text == null)
		{
			this.deleteButton.SetActive(false);
		}
		else
		{
			this.deleteButton.SetActive(true);
		}
		this.inputPage.SetActive(true);
		string text = this.inputComp.value.Trim();
		text = text.Replace('\r', ' ');
		text = text.Replace('\t', ' ');
		text = text.Replace('\n', ' ');
		while (this.EncodingTextLength(text) > 20)
		{
			text = text.Substring(0, text.Length - 1);
		}
		this.inputComp.value = text;
	}

	public void OnInputSearch()
	{
		string text = this.inputSearchName.value.Trim();
		text = text.Replace('\r', ' ');
		text = text.Replace('\t', ' ');
		text = text.Replace('\n', ' ');
		while (this.EncodingTextLength(text) > 20)
		{
			text = text.Substring(0, text.Length - 1);
		}
		this.inputSearchName.value = text;
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

	public void OnDeleteSearchText()
	{
		this.isSearchFriend = false;
		this.inputComp.value = string.Empty;
		this.deleteButton.SetActive(false);
		List<SimplePlayerData> myFriendList = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
		this.grid.transform.DestroyChildren();
		this.RefreshScrollView(myFriendList);
	}

	public void OnSearchMyFriend()
	{
		DateTime? dateTime = this.searchFriendTime;
		if (dateTime != null)
		{
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchFriendTime.Value;
			if (timeSpan.TotalSeconds < 5.0)
			{
				string message = string.Format(LanguageDataProvider.GetValue(1147), 5 - timeSpan.Seconds);
				Tips.Make(message);
				return;
			}
		}
		this.searchFriendTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		string value = this.inputComp.value;
		if (value == string.Empty)
		{
			List<SimplePlayerData> myFriendList = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
			this.grid.transform.DestroyChildren();
			this.RefreshScrollView(myFriendList);
		}
		else
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendSearch(value, 0, 0);
		}
	}

	public void OnInputOKClick()
	{
		string value = this.inputSearchName.value;
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		DateTime? dateTime = this.searchFriendTime;
		if (dateTime != null)
		{
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchFriendTime.Value;
			if (timeSpan.TotalSeconds < 5.0)
			{
				string message = string.Format(LanguageDataProvider.GetValue(1147), 5 - timeSpan.Seconds);
				Tips.Make(message);
				return;
			}
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendSearch(value, 0, 0);
		this.searchFriendTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
	}

	public void OnAddFriendsEvent()
	{
		if (this.confirmPanel.activeSelf)
		{
			return;
		}
		this.memberPanel.SetActive(false);
		this.confirmPanel.SetActive(true);
	}

	public void OnCloseConfirmPanel()
	{
		if (!this.confirmPanel.activeSelf)
		{
			return;
		}
		this.memberPanel.SetActive(true);
		this.confirmPanel.SetActive(false);
		this.SetInfo();
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendLoad(0, false);
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	private void RefreshScrollView(List<SimplePlayerData> data)
	{
		if (data != null)
		{
			this.cellList.Clear();
			for (int i = 0; i < data.Count; i++)
			{
				GameObject gameObject = this.grid.gameObject.AddChild(this.infoTemplate);
				gameObject.name = "infomation_" + data[i].userId;
				gameObject.SetActive(true);
				FriendWindowCell component = gameObject.GetComponent<FriendWindowCell>();
				this.cellList.Add(component);
				component.SetFollowerInfo(data[i], this.scrollView);
			}
		}
		this.grid.Reposition();
		this.scrollView.ResetPosition();
	}

	private void RefreshFriendStatus(List<SimplePlayerData> data)
	{
		if (data != null)
		{
			for (int i = 0; i < data.Count; i++)
			{
				for (int j = 0; j < this.cellList.Count; j++)
				{
					FriendWindowCell friendWindowCell = this.cellList[j];
					if (friendWindowCell.playerData.userId == data[i].userId)
					{
						friendWindowCell.SetFollowerInfo(data[i], this.scrollView);
					}
				}
			}
		}
	}

	private void OnTabClick(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (go.name.Equals(this.friendTab.gameObject.name))
		{
			this.selectTab = this.friendTab;
			this.selectLine.transform.localPosition = new Vector3(-110f, 244f, 0f);
		}
		else
		{
			this.selectTab = this.blackTab;
			this.selectLine.transform.localPosition = new Vector3(113f, 244f, 0f);
		}
		if (this.selectTab == this.friendTab)
		{
			List<SimplePlayerData> myFriendList = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
			this.grid.transform.DestroyChildren();
			this.RefreshScrollView(myFriendList);
		}
		else
		{
			List<SimplePlayerData> data = null;
			this.grid.transform.DestroyChildren();
			this.RefreshScrollView(data);
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

	public UILabel friendTab;

	public UILabel blackTab;

	private UILabel selectTab;

	public GameObject selectLine;

	public UILabel playerMoney;

	public UIScrollView scrollView;

	public UIGrid grid;

	public GameObject infoTemplate;

	public GameObject inputPage;

	public UIInput inputComp;

	public UIInput inputSearchName;

	private DateTime? searchFriendTime;

	public GameObject deleteButton;

	public GameObject confirmPanel;

	public GameObject memberPanel;

	private List<SimplePlayerData> playerList;

	private bool isSearchFriend;

	private float selectFollowerTabTime;

	private List<FriendWindowCell> cellList = new List<FriendWindowCell>();
}
