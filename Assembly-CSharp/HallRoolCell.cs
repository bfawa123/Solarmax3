using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class HallRoolCell : MonoBehaviour
{
	public void SetInfoLocal(MatchSynopsis matchInfo)
	{
		this.roomID = matchInfo.match_id;
		if (matchInfo.match_lock)
		{
			this.lockIcon.SetActive(true);
		}
		else
		{
			this.lockIcon.SetActive(false);
		}
		if (this.nameLabel != null)
		{
			this.nameLabel.mKey = -1;
		}
		if (this.RoomID != null)
		{
			this.RoomID.mKey = -1;
		}
		if (this.matchType != null)
		{
			this.matchType.mKey = -1;
		}
		if (this.roomType != null)
		{
			this.roomType.mKey = -1;
		}
		if (this.watchNum != null)
		{
			this.watchNum.mKey = -1;
		}
		if (this.fightNum != null)
		{
			this.fightNum.mKey = -1;
		}
		this.nameLabel.text = matchInfo.match_name;
		this.RoomID.text = matchInfo.match_id;
		this.matchType.text = this.GetMatchtype(matchInfo.c_type, matchInfo.map_id);
		this.roomType.text = this.GetRoomtype(matchInfo.c_type);
		this.bg.spriteName = this.GetMatchtypeSprite(matchInfo.c_type);
		int playerNumByMatchtype = this.GetPlayerNumByMatchtype(matchInfo.c_type);
		this.nMaxPlayerNum = playerNumByMatchtype;
		if (matchInfo.watch_count == 3)
		{
			this.nWatchNum = -1;
			this.watchNum.text = string.Format(LanguageDataProvider.GetValue(2201), new object[0]);
		}
		else
		{
			this.nWatchNum = matchInfo.watch_count;
			this.watchNum.text = string.Format(LanguageDataProvider.GetValue(2198), matchInfo.watch_count, 3);
		}
		if (matchInfo.fight_count == playerNumByMatchtype)
		{
			this.nFightNum = -1;
			this.fightNum.text = string.Format(LanguageDataProvider.GetValue(2202), new object[0]);
		}
		else
		{
			this.nFightNum = matchInfo.fight_count;
			this.fightNum.text = string.Format(LanguageDataProvider.GetValue(2199), matchInfo.fight_count, playerNumByMatchtype);
		}
		string value = LanguageDataProvider.GetValue(2083);
		string value2 = LanguageDataProvider.GetValue(2041);
		string value3 = LanguageDataProvider.GetValue(2036);
		UILabel[] componentsInChildren = base.GetComponentsInChildren<UILabel>(true);
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			UILabel uilabel = componentsInChildren[k];
			if (!(uilabel == null) && uilabel != this.nameLabel && uilabel != this.RoomID && uilabel != this.matchType && uilabel != this.roomType && uilabel != this.watchNum && uilabel != this.fightNum)
			{
				if (uilabel.mKey == 2083 || uilabel.text == value)
				{
					string text = uilabel.gameObject.name;
					string text2 = (uilabel.transform.parent != null) ? uilabel.transform.parent.name : string.Empty;
					string text3 = (text + " " + text2).ToLower();
					uilabel.mKey = -1;
					if (text3.Contains("watch") || text3.Contains("look") || text3.Contains("see") || text3.Contains("guan") || text3.Contains("kan"))
					{
						uilabel.text = value2;
					}
					else if (text3.Contains("join") || text3.Contains("fight") || text3.Contains("enter") || text3.Contains("jia") || text3.Contains("ru"))
					{
						uilabel.text = value3;
					}
				}
			}
		}
	}

	public void OnJoinWatchClick(GameObject go)
	{
		if (this.nWatchNum >= 3)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2201), 1f);
			return;
		}
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HallWindow";
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, this.roomID, string.Empty, CooperationType.CT_Null, 0, false, string.Empty, -1, string.Empty, true);
	}

	public void OnJoinFightClick(GameObject go)
	{
		if (this.nFightNum >= this.nMaxPlayerNum)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2202), 1f);
			return;
		}
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HallWindow";
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, this.roomID, string.Empty, CooperationType.CT_Null, 0, false, string.Empty, -1, string.Empty, false);
	}

	private string GetMatchtype(CooperationType cType, string mapid)
	{
		if (cType == CooperationType.CT_1v1)
		{
			return LanguageDataProvider.GetValue(2143);
		}
		if (cType == CooperationType.CT_1v1v1)
		{
			return LanguageDataProvider.GetValue(2072);
		}
		if (cType == CooperationType.CT_1v1v1v1)
		{
			return LanguageDataProvider.GetValue(2035);
		}
		if (cType == CooperationType.CT_2v2)
		{
			return LanguageDataProvider.GetValue(2144);
		}
		if (cType == CooperationType.CT_2v2 || cType == CooperationType.CT_2vPC || cType == CooperationType.CT_3vPC || cType == CooperationType.CT_4vPC)
		{
			return this.GetWindowTitle(mapid);
		}
		return string.Empty;
	}

	private string GetMatchtypeSprite(CooperationType cType)
	{
		if (cType == CooperationType.CT_1v1)
		{
			return "Signal1";
		}
		if (cType == CooperationType.CT_1v1v1)
		{
			return "Signal3";
		}
		if (cType == CooperationType.CT_1v1v1v1)
		{
			return "Signal4";
		}
		if (cType == CooperationType.CT_2v2)
		{
			return "Signal2";
		}
		if (cType == CooperationType.CT_2v2 || cType == CooperationType.CT_2vPC || cType == CooperationType.CT_3vPC || cType == CooperationType.CT_4vPC)
		{
			return "Signal6";
		}
		return string.Empty;
	}

	private int GetPlayerNumByMatchtype(CooperationType cType)
	{
		if (cType == CooperationType.CT_1v1)
		{
			return 2;
		}
		if (cType == CooperationType.CT_1v1v1)
		{
			return 3;
		}
		if (cType == CooperationType.CT_1v1v1v1)
		{
			return 4;
		}
		if (cType == CooperationType.CT_2v2)
		{
			return 4;
		}
		if (cType == CooperationType.CT_2vPC)
		{
			return 2;
		}
		if (cType == CooperationType.CT_3vPC)
		{
			return 3;
		}
		if (cType == CooperationType.CT_4vPC)
		{
			return 4;
		}
		return 0;
	}

	private string GetRoomtype(CooperationType cType)
	{
		string result = string.Empty;
		switch (cType)
		{
		case CooperationType.CT_1v1:
		case CooperationType.CT_2v2:
		case CooperationType.CT_1v1v1:
		case CooperationType.CT_1v1v1v1:
			result = LanguageDataProvider.GetValue(2034);
			break;
		case CooperationType.CT_2vPC:
		case CooperationType.CT_3vPC:
		case CooperationType.CT_4vPC:
			result = LanguageDataProvider.GetValue(2069);
			break;
		}
		return result;
	}

	private string GetWindowTitle(string matchid)
	{
		string text = string.Empty;
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(matchid);
		List<ChapterAssistConfig> allData = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData(data.chapter);
		if (allData != null && allData.Count > 0)
		{
			ChapterConfig data2 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data.chapter);
			if (data2 != null)
			{
				text = LanguageDataProvider.GetValue(data2.name);
			}
			for (int i = 0; i < allData.Count; i++)
			{
				if (matchid == allData[i].level1)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2104);
				}
				else if (matchid == allData[i].level2)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2105);
				}
				else if (matchid == allData[i].level3)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2106);
				}
				else if (matchid == allData[i].level4)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2107);
				}
			}
		}
		return text;
	}

	private string roomID;

	private DateTime? searchFriendTime;

	public GameObject lockIcon;

	public UISprite bg;

	public UILabel nameLabel;

	public UILabel RoomID;

	public UILabel matchType;

	public UILabel roomType;

	public UILabel watchNum;

	public UILabel fightNum;

	private int nWatchNum;

	private int nFightNum;

	private int nMaxPlayerNum = 4;
}
