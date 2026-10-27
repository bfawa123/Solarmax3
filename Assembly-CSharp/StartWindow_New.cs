using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class StartWindow_New : BaseWindow
{
	public void Awake()
	{
		for (int i = 0; i < this.subButtons.Length; i++)
		{
			this.subButtons[i].onClick = new UIEventListener.VoidDelegate(this.OnSubButtonClick);
		}
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnGetRoomList);
		base.RegisterEvent(EventId.OnJoinRoom);
		base.RegisterEvent(EventId.OnCreateRoom);
		base.RegisterEvent(EventId.OnRoomListREfresh);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < this.HouseList.Length; i++)
		{
			UIEventListener uieventListener = this.HouseList[i];
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.SelectPlayerCount));
		}
		for (int j = 0; j < this.roomList.Length; j++)
		{
			UIEventListener uieventListener2 = this.roomList[j];
			uieventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener2.onClick, new UIEventListener.VoidDelegate(this.OnRoomSelect));
		}
		for (int k = 0; k < this.createList.Length; k++)
		{
			UIEventListener uieventListener3 = this.createList[k];
			uieventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener3.onClick, new UIEventListener.VoidDelegate(this.OnCreateSelect));
		}
		this.HorseObj.SetActive(true);
		this.RoomObj.SetActive(false);
		this.CreateObj.SetActive(false);
		this.SetPlayerInfo();
		this.PlayAnimation("StartWindow_h2_in");
	}

	public override void OnHide()
	{
	}

	private void SetPlayerInfo()
	{
		this.userRewardLabel.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.score.ToString();
		this.userNameLabel.text = string.Format("Hi, {0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.name);
		this.userIconTexture.picUrl = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		switch (eventId)
		{
		case EventId.OnJoinRoom:
			this.users = (IList<UserData>)args[0];
			this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.PlayHideAniOver)));
			this.PlayAnimation("StartWindow_h2_out");
			break;
		case EventId.OnCreateRoom:
			this.users = (IList<UserData>)args[0];
			this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.PlayHideAniOver)));
			this.PlayAnimation("StartWindow_h2_out2");
			break;
		case EventId.OnRoomListREfresh:
		{
			int num = (int)args[0];
			int num2 = (int)args[1];
			for (int i = 0; i < this.rooms.Count; i++)
			{
				if (this.rooms[i].roomid == num)
				{
					this.roomUserNum[i].text = string.Format("{0}/{1}", num2, this.selectPlayerCount);
					if (num2 == this.selectPlayerCount)
					{
						Solarmax.Singleton<NetSystem>.Instance.helper.RequestRoomList(this.selectPlayerCount);
					}
					break;
				}
			}
			break;
		}
		}
	}

	public void SelectPlayerCount(GameObject obj)
	{
		string name = obj.transform.parent.name;
		if (name != null)
		{
			if (!(name == "map1"))
			{
				if (!(name == "map2"))
				{
					if (name == "map3")
					{
						this.selectPlayerCount = 4;
					}
				}
				else
				{
					this.selectPlayerCount = 3;
				}
			}
			else
			{
				this.selectPlayerCount = 2;
			}
		}
		this.HouseSelect.transform.parent = obj.transform.parent;
		this.HouseSelect.transform.localPosition = Vector3.zero;
	}

	public void GetRoomListOnClicked()
	{
		this.mapList.Clear();
		Dictionary<string, MapConfig> allData = Solarmax.Singleton<MapConfigProvider>.Instance.GetAllData();
		int num = this.selectPlayerCount;
		if (num != 2)
		{
			if (num != 3)
			{
				if (num == 4)
				{
					foreach (KeyValuePair<string, MapConfig> keyValuePair in allData)
					{
						if (keyValuePair.Value != null && keyValuePair.Value.pvp && keyValuePair.Value.player_count == this.selectPlayerCount)
						{
							this.mapList.Add(keyValuePair.Key);
						}
					}
					if (this.mapList.Count == 0)
					{
						for (int i = 401; i < 410; i++)
						{
							this.mapList.Add(i.ToString());
						}
					}
				}
			}
			else
			{
				foreach (KeyValuePair<string, MapConfig> keyValuePair2 in allData)
				{
					if (keyValuePair2.Value != null && keyValuePair2.Value.pvp && keyValuePair2.Value.player_count == this.selectPlayerCount)
					{
						this.mapList.Add(keyValuePair2.Key);
					}
				}
				if (this.mapList.Count == 0)
				{
					foreach (KeyValuePair<string, MapConfig> keyValuePair3 in allData)
					{
						if (keyValuePair3.Value != null && keyValuePair3.Value.vertical && keyValuePair3.Value.player_count == this.selectPlayerCount)
						{
							this.mapList.Add(keyValuePair3.Key);
						}
					}
				}
			}
		}
		else
		{
			for (int j = 5; j < 24; j++)
			{
				this.mapList.Add(string.Format("s{0}v", j));
			}
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestRoomList(this.selectPlayerCount);
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.OnOneToTwoFinish)));
		this.PlayAnimation("StartWindow_h2_center-center2");
	}

	public void OnRoomSelect(GameObject obj)
	{
		string name = obj.transform.parent.name;
		if (name != null)
		{
			if (!(name == "map1"))
			{
				if (!(name == "map2"))
				{
					if (!(name == "map3"))
					{
						if (!(name == "map4"))
						{
							if (!(name == "map5"))
							{
								if (name == "map6")
								{
									this.selectRoom = this.rooms[5].roomid;
								}
							}
							else
							{
								this.selectRoom = this.rooms[4].roomid;
							}
						}
						else
						{
							this.selectRoom = this.rooms[3].roomid;
						}
					}
					else
					{
						this.selectRoom = this.rooms[2].roomid;
					}
				}
				else
				{
					this.selectRoom = this.rooms[1].roomid;
				}
			}
			else
			{
				this.selectRoom = this.rooms[0].roomid;
			}
		}
		this.RoomSelect.transform.parent = obj.transform.parent;
		this.RoomSelect.transform.localPosition = Vector3.zero;
	}

	public void JoinGameOnClicked()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestJoinRoom(this.selectRoom);
	}

	public void OpenCreateRoom()
	{
	}

	public void OnCreateSelect(GameObject obj)
	{
		string name = obj.transform.parent.name;
		if (name != null)
		{
			if (!(name == "map1"))
			{
				if (!(name == "map2"))
				{
					if (!(name == "map3"))
					{
						if (!(name == "map4"))
						{
							if (!(name == "map5"))
							{
								if (name == "map6")
								{
									this.selectMap = this.mapList[5];
								}
							}
							else
							{
								this.selectMap = this.mapList[4];
							}
						}
						else
						{
							this.selectMap = this.mapList[3];
						}
					}
					else
					{
						this.selectMap = this.mapList[2];
					}
				}
				else
				{
					this.selectMap = this.mapList[1];
				}
			}
			else
			{
				this.selectMap = this.mapList[0];
			}
		}
		this.createSelect.transform.parent = obj.transform.parent;
		this.createSelect.transform.localPosition = Vector3.zero;
	}

	public void OnCreateRoom()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestCreateRoom(this.selectMap);
	}

	public void OnCancalCreate()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestRoomList(this.selectPlayerCount);
		this.HorseObj.SetActive(false);
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.OnThrToTwoFinish)));
		this.PlayAnimation("StartWindow_h2_center3-center2");
	}

	public void RoomCancalAniOver()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestUnWatchRooms();
		this.CreateObj.SetActive(false);
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.OnTwoToOneFinish)));
		this.PlayAnimation("StartWindow_h2_center2-center");
	}

	public void PlayHideAniOver()
	{
		this.aniPlayer.onFinished.Clear();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("WaitWindow");
		Solarmax.Singleton<UISystem>.Get().HideWindow("StartWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRoomRefresh, new object[]
		{
			this.users
		});
	}

	private void CloseToWindow(string windowName)
	{
		this.waitToShowWindow = windowName;
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.HideWindowDelegate)));
		this.PlayAnimation("StartWindow_h2_out");
	}

	private void HideWindowDelegate()
	{
		this.aniPlayer.onFinished.Clear();
		Solarmax.Singleton<UISystem>.Get().HideWindow("StartWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow(this.waitToShowWindow);
	}

	public void OnSettingClick()
	{
		this.CloseToWindow("SettingWindow");
	}

	public void OnSubButtonClick(GameObject go)
	{
		if (go.name.Equals("store"))
		{
			this.CloseToWindow("ReplayWindow");
		}
	}

	private void OnOneToTwoFinish()
	{
		this.aniPlayer.onFinished.Clear();
		this.HorseObj.SetActive(false);
		this.RoomObj.SetActive(true);
		this.CreateObj.SetActive(false);
	}

	private void OnTwoToThrFinish()
	{
		this.aniPlayer.onFinished.Clear();
		this.HorseObj.SetActive(false);
		this.RoomObj.SetActive(false);
		this.CreateObj.SetActive(true);
	}

	private void OnThrToTwoFinish()
	{
		this.aniPlayer.onFinished.Clear();
		this.HorseObj.SetActive(false);
		this.RoomObj.SetActive(true);
		this.CreateObj.SetActive(false);
	}

	private void OnTwoToOneFinish()
	{
		this.aniPlayer.onFinished.Clear();
		this.CreateObj.SetActive(false);
		this.RoomObj.SetActive(false);
		this.HorseObj.SetActive(true);
	}

	private void PlayAnimation(string state)
	{
		this.aniPlayer.clipName = state;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	public UILabel userRewardLabel;

	public UILabel userNameLabel;

	public NetTexture userIconTexture;

	public UIPlayAnimation aniPlayer;

	public GameObject HorseObj;

	public UIEventListener[] HouseList;

	public GameObject HouseSelect;

	public GameObject RoomObj;

	public UIEventListener[] roomList;

	public UITexture[] roomPicList;

	public UILabel[] roomUserNum;

	public GameObject RoomSelect;

	private List<RoomInfo> rooms = new List<RoomInfo>();

	private IList<UserData> users;

	public GameObject CreateObj;

	public UIEventListener[] createList;

	public UITexture[] createPicList;

	public UILabel[] createText;

	public GameObject createSelect;

	public UIEventListener[] subButtons;

	private int selectPlayerCount = 2;

	private string selectMap = "D1";

	private int selectRoom;

	private List<string> mapList = new List<string>();

	private string waitToShowWindow;
}
