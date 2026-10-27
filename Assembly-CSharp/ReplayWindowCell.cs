using System;
using System.Collections.Generic;
using System.Linq;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ReplayWindowCell : MonoBehaviour
{
	public void SetInfo(int index, BattleReportData brd, bool myReport, UIScrollView scroll = null)
	{
		this.reportData = brd;
		this.isLocal = false;
		List<SimplePlayerData> list = new List<SimplePlayerData>();
		foreach (SimplePlayerData item in brd.playerList)
		{
			list.Add(item);
		}
		list.Sort((SimplePlayerData arg0, SimplePlayerData arg1) => -arg0.scoreMod.CompareTo(arg1.scoreMod));
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < list.Count; j++)
		{
			SimplePlayerData simplePlayerData = list[j];
			if (simplePlayerData.userId != Solarmax.Singleton<LocalPlayer>.Get().playerData.userId || j == 0)
			{
			}
			Team team = null;
			for (int k = 0; k < brd.playerList.Count; k++)
			{
				if (brd.playerList[k].userId == simplePlayerData.userId)
				{
					team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)(k + 1));
					break;
				}
			}
			this.races[j].gameObject.SetActive(true);
			this.races[j].transform.Find("icon_effect").gameObject.SetActive(true);
			this.races[j].transform.Find("icon").gameObject.SetActive(true);
			this.races[j].transform.Find("name").gameObject.SetActive(true);
			UISprite component = this.races[j].transform.Find("icon_effect").GetComponent<UISprite>();
			NetTexture component2 = this.races[j].transform.Find("icon").GetComponent<NetTexture>();
			UILabel component3 = this.races[j].transform.Find("name").GetComponent<UILabel>();
			if (simplePlayerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
			{
				this.races[j].transform.Find("kuang").gameObject.SetActive(true);
			}
			component3.text = simplePlayerData.name;
			string text = simplePlayerData.icon;
			if (!text.EndsWith(".png"))
			{
				text += ".png";
			}
			component2.scroll = scroll;
			component2.picUrl = text;
			Color color = team.color;
			color.a = 1f;
			component.color = color;
		}
		this.playLable.text = LanguageDataProvider.GetValue(2019);
		this.name.gameObject.SetActive(true);
		this.mode.gameObject.SetActive(true);
		switch (this.reportData.matchType)
		{
		case MatchType.MT_Ladder:
			this.mode.text = LanguageDataProvider.GetValue(2049);
			goto IL_3D1;
		case MatchType.MT_Room:
			if (this.reportData.subType == CooperationType.CT_4vPC)
			{
				this.mode.text = LanguageDataProvider.GetValue(2069);
			}
			else
			{
				this.mode.text = LanguageDataProvider.GetValue(2034);
			}
			goto IL_3D1;
		case MatchType.MT_Cooperation:
			this.mode.text = LanguageDataProvider.GetValue(2069);
			goto IL_3D1;
		}
		this.mode.text = LanguageDataProvider.GetValue(2215);
		IL_3D1:
		switch (this.reportData.subType)
		{
		case CooperationType.CT_1v1:
			this.name.text = LanguageDataProvider.GetValue(2143);
			this.icon.spriteName = "Signal1_s";
			goto IL_702;
		case CooperationType.CT_2v2:
			this.name.text = LanguageDataProvider.GetValue(2144);
			this.icon.spriteName = "Signal2_s";
			goto IL_702;
		case CooperationType.CT_1v1v1:
			this.name.text = LanguageDataProvider.GetValue(2072);
			this.icon.spriteName = "Signal3_s";
			goto IL_702;
		case CooperationType.CT_1v1v1v1:
			this.name.text = LanguageDataProvider.GetValue(2035);
			this.icon.spriteName = "Signal4_s";
			goto IL_702;
		case CooperationType.CT_4vPC:
		{
			LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.reportData.matchId);
			List<ChapterAssistConfig> allData = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData(data.chapter);
			if (allData != null && allData.Count > 0)
			{
				for (int l = 0; l < allData.Count; l++)
				{
					if (this.reportData.matchId == allData[l].level1)
					{
						this.name.text = LanguageDataProvider.GetValue(allData[l].name);
						this.name.text = this.name.text + " - " + LanguageDataProvider.GetValue(2104);
					}
					else if (this.reportData.matchId == allData[l].level2)
					{
						this.name.text = LanguageDataProvider.GetValue(allData[l].name);
						this.name.text = this.name.text + " - " + LanguageDataProvider.GetValue(2105);
					}
					else if (this.reportData.matchId == allData[l].level3)
					{
						this.name.text = LanguageDataProvider.GetValue(allData[l].name);
						this.name.text = this.name.text + " - " + LanguageDataProvider.GetValue(2106);
					}
					else if (this.reportData.matchId == allData[l].level4)
					{
						this.name.text = LanguageDataProvider.GetValue(allData[l].name);
						this.name.text = this.name.text + " - " + LanguageDataProvider.GetValue(2107);
					}
				}
			}
			else
			{
				this.name.text = string.Empty;
			}
			this.icon.spriteName = "Signal6_s";
			goto IL_702;
		}
		}
		this.name.text = string.Empty;
		IL_702:
		DateTime d = new DateTime(1970, 1, 1);
		d = d.AddSeconds((double)this.reportData.time);
		TimeSpan timeSpan = DateTime.UtcNow - d;
		if (timeSpan.Days > 0)
		{
			this.time.text = string.Format(LanguageDataProvider.GetValue(102), timeSpan.Days, timeSpan.Hours);
		}
		else if (timeSpan.Hours > 0)
		{
			this.time.text = string.Format(LanguageDataProvider.GetValue(103), timeSpan.Hours, timeSpan.Minutes);
		}
		else
		{
			this.time.text = string.Format(LanguageDataProvider.GetValue(104), Mathf.CeilToInt((float)timeSpan.Minutes));
		}
	}

	public void SetInfoLocal(int index, string szName, string mapId, long timeStamp, int nWin, UIScrollView scroll = null)
	{
		this.isLocal = true;
		this.levelID = mapId;
		this.fileName = szName;
		MapConfig table = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.levelID);
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.levelID);
		if (table == null)
		{
			return;
		}
		base.gameObject.SetActive(true);
		List<MapPlayerConfig> list = table.mpcList.Where((MapPlayerConfig x, int i) => table.mpcList.FindIndex((MapPlayerConfig z) => z.camption == x.camption) == i).ToList<MapPlayerConfig>();
		List<MapPlayerConfig> list2 = new List<MapPlayerConfig>();
		for (int m = 0; m < list.Count; m++)
		{
			int camption = list[m].camption;
			if (camption != 0)
			{
				if (camption == data.playerTeam)
				{
					list2.Add(list[m]);
					break;
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			int camption2 = list[j].camption;
			if (camption2 != 0)
			{
				if (camption2 != data.playerTeam)
				{
					list2.Add(list[j]);
				}
			}
		}
		list.Clear();
		int num = this.races.Length;
		List<MapPlayerConfig> list3 = list2;
		for (int k = 0; k < this.races.Length; k++)
		{
			this.races[k].gameObject.SetActive(false);
		}
		for (int l = 0; l < list3.Count; l++)
		{
			if (l >= num)
			{
				break;
			}
			int camption3 = list3[l].camption;
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)camption3);
			this.races[l].gameObject.SetActive(true);
			this.races[l].transform.Find("icon_effect").gameObject.SetActive(true);
			this.races[l].transform.Find("name").gameObject.SetActive(true);
			UISprite component = this.races[l].transform.Find("icon_effect").GetComponent<UISprite>();
			NetTexture component2 = this.races[l].transform.Find("icon").GetComponent<NetTexture>();
			UILabel component3 = this.races[l].transform.Find("name").GetComponent<UILabel>();
			if (team.team == (TEAM)data.playerTeam)
			{
				this.races[l].transform.Find("icon").gameObject.SetActive(true);
				component3.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.name;
				string text = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
				if (!text.EndsWith(".png"))
				{
					text += ".png";
				}
				component2.scroll = scroll;
				component2.picUrl = text;
			}
			else
			{
				this.races[l].transform.Find("icon").gameObject.SetActive(true);
				component2.scroll = scroll;
				component2.picUrl = this.DEFAULT_ICON;
				component3.text = LanguageDataProvider.GetValue(19);
			}
			Color color = team.color;
			color.a = 1f;
			component.color = color;
		}
		this.playLable.text = LanguageDataProvider.GetValue(2019);
		this.name.gameObject.SetActive(true);
		this.resultScore.gameObject.SetActive(true);
		LevelConfig data2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.levelID);
		ChapterConfig data3 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data2.chapter);
		if (data2 != null && data3 != null)
		{
			this.name.text = LanguageDataProvider.GetValue(data3.name) + " - " + LanguageDataProvider.GetValue(data2.levelName);
		}
		else
		{
			this.name.text = string.Empty;
		}
		if (data3.type == 0)
		{
			this.mode.text = LanguageDataProvider.GetValue(1151);
			this.icon.spriteName = "Signal5_s";
		}
		else
		{
			this.mode.text = LanguageDataProvider.GetValue(1152);
			this.icon.spriteName = "Signal5_s";
		}
		DateTime d = new DateTime(1970, 1, 1);
		d = d.AddSeconds((double)timeStamp);
		TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
		if (timeSpan.Days > 0)
		{
			this.time.text = string.Format(LanguageDataProvider.GetValue(102), timeSpan.Days, timeSpan.Hours);
		}
		else if (timeSpan.Hours > 0)
		{
			this.time.text = string.Format(LanguageDataProvider.GetValue(103), timeSpan.Hours, timeSpan.Minutes);
		}
		else
		{
			this.time.text = string.Format(LanguageDataProvider.GetValue(104), Mathf.CeilToInt((float)timeSpan.Minutes));
		}
		list3.Clear();
	}

	public void OnPlayClick()
	{
		if (this.isLocal)
		{
			PbSCFrames pbSCFrames = ReplayCollectManager.Get().LoadFileToReplayData(this.fileName, false);
			if (pbSCFrames != null)
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("UnTouchWindow");
				Solarmax.Singleton<BattleSystem>.Instance.replayManager.TryPlayRecord(pbSCFrames, false);
				base.Invoke("HideInvoke", 3f);
			}
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.BattleReportPlay(this.reportData);
		Solarmax.Singleton<UISystem>.Get().ShowWindow("UnTouchWindow");
		base.Invoke("HideInvoke", 3f);
	}

	public void OnScriptPlayClick()
	{
		PbSCFrames msg = ReplayCollectManager.Get().LoadScriptFileToReplayData("100202", false);
		Solarmax.Singleton<UISystem>.Get().ShowWindow("UnTouchWindow");
		Solarmax.Singleton<BattleSystem>.Instance.replayManager.TryPlayScript(msg);
		base.Invoke("HideInvoke", 3f);
	}

	private void HideInvoke()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("UnTouchWindow");
	}

	public GameObject bg;

	public GameObject[] races;

	public new UILabel name;

	public UILabel mode;

	public UISprite icon;

	public UILabel resultScore;

	public UILabel time;

	public UILabel playLable;

	private readonly string DEFAULT_ICON = "select_head_0.png";

	private BattleReportData reportData;

	private Color winColor = new Color(1f, 0.63f, 0.04f, 1f);

	private Color loseColor = new Color(1f, 0.42f, 0.42f, 1f);

	private string levelID = string.Empty;

	private string fileName = string.Empty;

	private bool isLocal;
}
