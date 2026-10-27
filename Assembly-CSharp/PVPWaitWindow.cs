using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class PVPWaitWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnMatchInit);
		base.RegisterEvent(EventId.OnMatchUpdate);
		base.RegisterEvent(EventId.OnMatchQuit);
		base.RegisterEvent(EventId.OnHideQuitMatchButton);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.timeCount = -1;
		this.timeLabel.gameObject.SetActive(false);
		this.tips.text = string.Empty;
		base.InvokeRepeating("TimeUpdate", 0f, 1f);
	}

	public override void OnHide()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.root.SetActive(true);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnMatchInit)
		{
			this.matchId = (string)args[0];
			this.roomId = (string)args[1];
			IList<UserData> list = (IList<UserData>)args[2];
			IList<int> list2 = (IList<int>)args[3];
			this.timeCount = (int)args[5];
			int num = 0;
			int num2 = 4;
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC)
			{
				num2 = 2;
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC)
			{
				num2 = 3;
			}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
			{
				num2 = 4;
			}
			if (num2 > list.Count)
			{
				num2 = list.Count;
			}
			for (int i = 0; i < num2; i++)
			{
				PlayerData playerData = new PlayerData();
				playerData.Init(list[i]);
				if (playerData.userId > 0)
				{
					num++;
				}
				else
				{
					playerData.name = AIManager.GetAIName(playerData.userId);
					playerData.icon = AIManager.GetAIIcon(playerData.userId);
				}
				int num3 = list2[i];
				this.nowUserDatas[num3] = playerData;
				this.SetModelPage();
				this.SetPage();
				this.CheckAllEntered();
				MonoSingleton<FlurryAnalytis>.Instance.FlurryPVPBattleMatchEvent("0", this.matchId, "0", num.ToString(), string.Empty);
				AppsFlyerTool.FlyerPVPBattleStartEvent();
				MiGameAnalytics.MiAnalyticsPVPBattleMatchEvent("0", this.matchId, "0", num.ToString(), string.Empty);
			}
		}
		else if (eventId == EventId.OnMatchUpdate)
		{
			IList<UserData> list3 = (IList<UserData>)args[0];
			IList<int> list4 = (IList<int>)args[1];
			IList<int> list5 = (IList<int>)args[2];
			for (int j = 0; j < list5.Count; j++)
			{
				int num4 = list5[j];
				this.nowUserDatas[num4] = null;
			}
			for (int k = 0; k < list3.Count; k++)
			{
				PlayerData playerData2 = new PlayerData();
				playerData2.Init(list3[k]);
				if (playerData2.userId <= 0)
				{
					playerData2.name = AIManager.GetAIName(playerData2.userId);
					playerData2.icon = AIManager.GetAIIcon(playerData2.userId);
				}
				int num5 = list4[k];
				this.nowUserDatas[num5] = playerData2;
			}
			this.SetPage();
			this.CheckAllEntered();
		}
		else if (eventId == EventId.OnMatchQuit)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_NotMaster)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(905), 1f);
			}
			else if (errCode != ErrCode.EC_Ok)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.Format(901, new object[]
				{
					errCode
				}), 1f);
			}
			else if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.root.SetActive(false);
				Solarmax.Singleton<BattleSystem>.Instance.Reset();
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("PvPRoomWindow");
			}
		}
		else if (eventId == EventId.OnHideQuitMatchButton)
		{
			this.cancelBtn.SetActive(false);
		}
	}

	private void TimeUpdate()
	{
		if (this.timeCount < 0)
		{
			return;
		}
		this.timeLabel.text = string.Format("{0:D2}:{1:D2}", this.timeCount / 60, this.timeCount % 60);
		this.timeLabel.gameObject.SetActive(true);
		this.timeCount--;
	}

	private void CheckAllEntered()
	{
		int num = 0;
		for (int i = 0; i < this.nowUserDatas.Length; i++)
		{
			if (this.nowUserDatas[i] != null)
			{
				num++;
			}
		}
		if (this.IsAll(num))
		{
			this.tips.text = LanguageDataProvider.GetValue(636);
		}
	}

	private bool IsAll(int num)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC)
		{
			if (num == 2)
			{
				return true;
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC)
		{
			if (num == 3)
			{
				return true;
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1v1)
		{
			if (num == 4)
			{
				return true;
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
		{
			if (num == 4)
			{
				return true;
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
		{
			if (num == 4)
			{
				return true;
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC && num == 4)
		{
			return true;
		}
		return false;
	}

	public void SetPage()
	{
		this.SetUIPage();
		for (int i = 0; i < this.nowUserDatas.Length; i++)
		{
			this.SetPlayerInfo(this.playerGos[i], this.nowUserDatas[i]);
		}
	}

	private void SetPlayerInfo(GameObject go, PlayerData pd)
	{
		go.SetActive(true);
		NetTexture component = go.transform.Find("Portrait/IconB").GetComponent<NetTexture>();
		UILabel component2 = go.transform.Find("name").GetComponent<UILabel>();
		UILabel component3 = go.transform.Find("score/score").GetComponent<UILabel>();
		GameObject gameObject = go.transform.Find("Matching").gameObject;
		GameObject gameObject2 = go.transform.Find("score/pic").gameObject;
		if (pd == null)
		{
			gameObject2.SetActive(false);
			gameObject.SetActive(true);
			component.gameObject.SetActive(false);
			component2.gameObject.SetActive(false);
			component3.gameObject.SetActive(false);
		}
		else
		{
			gameObject2.SetActive(true);
			gameObject.SetActive(false);
			component.gameObject.SetActive(true);
			component2.gameObject.SetActive(true);
			component3.gameObject.SetActive(true);
			string text = pd.icon;
			if (!text.EndsWith(".png"))
			{
				text += ".png";
			}
			component.picUrl = text;
			if (pd.userId == -1)
			{
				component2.text = LanguageDataProvider.Format(908, new object[]
				{
					pd.name
				});
			}
			else
			{
				component2.text = pd.name;
			}
			component3.text = pd.score.ToString();
		}
	}

	private void SetUIPage()
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC)
		{
			int num = this.player1V1.Length;
			for (int i = 0; i < num; i++)
			{
				this.playerGos[i] = this.player1V1[i];
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
		{
			int num2 = this.player2V2.Length;
			for (int j = 0; j < num2; j++)
			{
				this.playerGos[j] = this.player2V2[j];
			}
			// 2v2原版不展示取消按钮，现在展示，方便玩家退出
			//this.cancelBtn.SetActive(false);
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC)
		{
			int num3 = this.player3H.Length;
			for (int k = 0; k < num3; k++)
			{
				this.playerGos[k] = this.player3H[k];
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
		{
			int num4 = this.player4H.Length;
			for (int l = 0; l < num4; l++)
			{
				this.playerGos[l] = this.player4H[l];
			}
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.matchType == MatchType.MT_Room)
		{
			this.cancelBtn.SetActive(false);
		}
	}

	private void SetModelPage()
	{
		this.page1V1.SetActive(false);
		this.page2V2.SetActive(false);
		this.page3H.SetActive(false);
		this.page4H.SetActive(false);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC)
		{
			this.page1V1.SetActive(true);
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
		{
			this.page2V2.SetActive(true);
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC)
		{
			this.page3H.SetActive(true);
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
		{
			this.page4H.SetActive(true);
		}
	}

	public void OnClickCancel()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(-1);
	}

	public UILabel tips;

	public UILabel timeLabel;

	public GameObject[] playerGos;

	public GameObject[] player1V1;

	public GameObject[] player2V2;

	public GameObject[] player3H;

	public GameObject[] player4H;

	public GameObject page1V1;

	public GameObject page2V2;

	public GameObject page3H;

	public GameObject page4H;

	private int timeCount;

	private PlayerData[] nowUserDatas = new PlayerData[4];

	private string matchId;

	private string roomId;

	public GameObject cancelBtn;
}
