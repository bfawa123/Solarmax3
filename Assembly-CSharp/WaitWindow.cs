using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class WaitWindow : BaseWindow
{
	public void Awake()
	{
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnReady);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < this.posRoots.Length; i++)
		{
			this.posRoots[i].SetActive(false);
		}
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Wandering", 0.5f);
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
		if (eventId == EventId.OnReady)
		{
			this.SetPage();
			MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
			List<Team> list = new List<Team>();
			for (int i = 0; i < data.player_count; i++)
			{
				TEAM team = (TEAM)(i + 1);
				Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team);
				list.Add(team2);
			}
			list.Sort((Team arg0, Team arg1) => arg0.groupID.CompareTo(arg1.groupID));
			for (int j = 0; j < list.Count; j++)
			{
				this.SetPosInfo(j, list[j]);
			}
			this.PlayAnimation();
		}
	}

	private void PlayAnimation()
	{
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
		string stateName = string.Empty;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight)
		{
			stateName = "WaitWindow_2v2_in";
		}
		else if (data.player_count == 2)
		{
			stateName = "WaitWindow_1v1_in";
		}
		else if (data.player_count == 3)
		{
			stateName = "WaitWindow_1v1v1_in";
		}
		else if (data.player_count == 4)
		{
			stateName = "WaitWindow_1v1v1v1_in";
		}
		Animator component = base.gameObject.GetComponent<Animator>();
		component.Play(stateName);
	}

	private void SetPage()
	{
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
		GameObject posRoot = this.GetPosRoot();
		posRoot.SetActive(true);
		for (int i = 0; i < data.player_count; i++)
		{
			GameObject gameObject = posRoot.transform.Find("pos" + (i + 1)).gameObject;
			GameObject gameObject2 = gameObject.AddChild(this.infoTemplate);
			gameObject2.name = "info";
			gameObject2.SetActive(true);
			this.SetPosInfo(i, null);
		}
	}

	private GameObject GetPosRoot()
	{
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
		GameObject result;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight)
		{
			result = this.posRoots[3];
		}
		else
		{
			result = this.posRoots[data.player_count - 2];
		}
		return result;
	}

	private void SetPosInfo(int pos, Team t)
	{
		GameObject posRoot = this.GetPosRoot();
		GameObject gameObject = posRoot.transform.Find("pos" + (pos + 1)).Find("info").gameObject;
		if (t == null)
		{
			gameObject.transform.Find("full").gameObject.SetActive(false);
			gameObject.transform.Find("empty").gameObject.SetActive(true);
			return;
		}
		PlayerData playerData = t.playerData;
		Color color = t.color;
		color.a = 1f;
		gameObject.transform.Find("full").gameObject.SetActive(true);
		gameObject.transform.Find("empty").gameObject.SetActive(false);
		UILabel component = gameObject.transform.Find("full/score/num").GetComponent<UILabel>();
		UISprite component2 = gameObject.transform.Find("full/icon").GetComponent<UISprite>();
		NetTexture component3 = gameObject.transform.Find("full/icon/usericon").GetComponent<NetTexture>();
		UILabel component4 = gameObject.transform.Find("full/icon/name").GetComponent<UILabel>();
		UILabel component5 = gameObject.transform.Find("full/union/name").GetComponent<UILabel>();
		component.text = playerData.score.ToString();
		component3.picUrl = playerData.icon;
		component3.picColor = color;
		component2.color = color;
		component4.text = playerData.name;
		component4.color = color;
		component5.text = playerData.unionName;
		Animator component6 = gameObject.GetComponent<Animator>();
		if (playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			component6.Play("WaitWindow_information1_mine");
		}
		else
		{
			component6.Play("WaitWindow_information1_other");
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("WaitWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
	}

	public GameObject infoTemplate;

	public GameObject[] posRoots;

	private List<GameObject> infoList = new List<GameObject>();
}
