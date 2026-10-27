using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class WaitWindow2 : BaseWindow
{
	public void Awake()
	{
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnRoomRefresh);
		base.RegisterEvent(EventId.OnStartMatch3);
		base.RegisterEvent(EventId.OnMatch3Notify);
		base.RegisterEvent(EventId.OnStartMatch2);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		base.InvokeRepeating("UpdateTime", 0f, 1f);
		for (int i = 0; i < this.posRoots.Length; i++)
		{
			this.posRoots[i].SetActive(false);
		}
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Wandering", 0.5f);
		this.tips.gameObject.SetActive(false);
		this.SetPage(true);
	}

	public void UpdateTime()
	{
		this.time.text = string.Format("{0:D2}:{1:D2}", this.timeCount / 60, this.timeCount % 60);
		this.timeCount++;
	}

	public override void OnHide()
	{
		for (int i = 0; i < this.infoList.Count; i++)
		{
			UnityEngine.Object.Destroy(this.infoList[i]);
		}
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		switch (eventId)
		{
		case EventId.OnRoomRefresh:
		{
			IList<UserData> list = args[0] as IList<UserData>;
			for (int i = 0; i < this.playerCount; i++)
			{
				if (i < list.Count)
				{
					PlayerData playerData = new PlayerData();
					playerData.Init(list[i]);
					this.SetPosInfo(i, playerData);
				}
				else
				{
					this.SetPosInfo(i, null);
				}
			}
			break;
		}
		case EventId.OnStartMatch2:
			this.playerCount = Solarmax.Singleton<LocalPlayer>.Get().CurBattlePlayerNum;
			this.SetPage(false);
			this.SetPosInfo(0, Solarmax.Singleton<LocalPlayer>.Get().playerData);
			break;
		case EventId.OnStartMatch3:
			this.playerCount = 4;
			this.SetPage(false);
			break;
		case EventId.OnMatch3Notify:
		{
			List<PlayerData> list2 = args[0] as List<PlayerData>;
			for (int j = 0; j < this.playerCount; j++)
			{
				if (j < list2.Count)
				{
					PlayerData data = list2[j];
					this.SetPosInfo(j, data);
				}
				else
				{
					this.SetPosInfo(j, null);
				}
			}
			break;
		}
		}
	}

	private void PlayAnimation()
	{
		string stateName = string.Empty;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight)
		{
			stateName = "WaitWindow_2v2_in";
		}
		else if (this.playerCount == 2)
		{
			stateName = "WaitWindow_1v1_in";
		}
		else if (this.playerCount == 3)
		{
			stateName = "WaitWindow_1v1v1_in";
		}
		else if (this.playerCount == 4)
		{
			stateName = "WaitWindow_1v1v1v1_in";
		}
		Animator component = base.gameObject.GetComponent<Animator>();
		component.Play(stateName);
	}

	private void SetPage(bool useMapInfo)
	{
		if (useMapInfo)
		{
			if (string.IsNullOrEmpty(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId))
			{
				return;
			}
			MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
			this.playerCount = data.player_count;
		}
		if (this.playerCount < 2)
		{
			return;
		}
		GameObject posRoot = this.GetPosRoot();
		posRoot.SetActive(true);
		for (int i = 0; i < this.playerCount; i++)
		{
			GameObject gameObject = posRoot.transform.Find("pos" + (i + 1)).gameObject;
			GameObject gameObject2 = gameObject.AddChild(this.infoTemplate);
			gameObject2.name = "info";
			gameObject2.SetActive(true);
			this.SetPosInfo(i, null);
		}
		this.tips.text = string.Format(LanguageDataProvider.GetValue(417), this.playerCount);
		this.tips.gameObject.SetActive(true);
	}

	private GameObject GetPosRoot()
	{
		GameObject result;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight)
		{
			result = this.posRoots[3];
		}
		else
		{
			result = this.posRoots[this.playerCount - 2];
		}
		return result;
	}

	private void SetPosInfo(int pos, PlayerData data)
	{
		GameObject posRoot = this.GetPosRoot();
		GameObject gameObject = posRoot.transform.Find("pos" + (pos + 1)).Find("info").gameObject;
		if (data == null)
		{
			gameObject.transform.Find("full").gameObject.SetActive(false);
			gameObject.transform.Find("empty").gameObject.SetActive(true);
			return;
		}
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)(pos + 1));
		Color color = team.color;
		color.a = 1f;
		gameObject.transform.Find("full").gameObject.SetActive(true);
		gameObject.transform.Find("empty").gameObject.SetActive(false);
		UILabel component = gameObject.transform.Find("full/score/num").GetComponent<UILabel>();
		NetTexture component2 = gameObject.transform.Find("full/usericon").GetComponent<NetTexture>();
		UILabel component3 = gameObject.transform.Find("full/name").GetComponent<UILabel>();
		component.text = data.score.ToString();
		component2.picUrl = data.icon;
		component2.picColor = color;
		component3.text = data.name;
		component3.color = color;
		Animator component4 = gameObject.GetComponent<Animator>();
		if (data.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			component4.Play("WaitWindow_information1_mine");
		}
		else
		{
			component4.Play("WaitWindow_information1_other");
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("WaitWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomSelectWindowNew");
	}

	public void OnQuitClicked()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestQuitRoom();
	}

	public UILabel tips;

	public UILabel time;

	public GameObject infoTemplate;

	public GameObject[] posRoots;

	public GameObject cancelBtn;

	private List<GameObject> infoList = new List<GameObject>();

	private int playerCount;

	private int timeCount;
}
