using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class SelectRaceWindow_new : BaseWindow
{
	private void Awake()
	{
		this.raceTip.SetActive(false);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnStartSelectRace);
		base.RegisterEvent(EventId.OnRoomListREfresh);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < this.playerIconList.Length; i++)
		{
			this.playerIconList[i].gameObject.SetActive(false);
			this.playerIconEffectList[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < this.playerNameList.Length; j++)
		{
			this.playerNameList[j].text = string.Empty;
		}
		for (int k = 0; k < this.kuangList.Length; k++)
		{
			UIEventListener uieventListener = this.kuangList[k];
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.OnIconClicked));
			UIEventListener uieventListener2 = this.kuangList[k];
			uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.OnPressIcon));
		}
	}

	public override void OnHide()
	{
	}

	private void SetInfo()
	{
		for (int i = 0; i < this.iconList.Length; i++)
		{
			RaceConfig data = Solarmax.Singleton<RaceConfigProvider>.Instance.GetData(i + 1);
			if (data != null && !string.IsNullOrEmpty(data.icon))
			{
				this.iconList[i].spriteName = data.icon;
				this.iconList[i].gameObject.SetActive(true);
			}
			else
			{
				Debug.LogErrorFormat("Race table error, key:{0}", new object[]
				{
					i + 1
				});
			}
			RaceData raceData = this.selfRaces[i];
			this.levelList[i].text = "Lv." + raceData.level;
			if (raceData.race_lock)
			{
				Color gray = Color.gray;
				gray.a = 0.6f;
				this.iconList[i].color = gray;
				this.iconList[i].transform.parent.Find("lock").gameObject.SetActive(true);
				this.kuangList[i].gameObject.GetComponent<UISprite>().spriteName = "avatar_bg_disable";
			}
			else
			{
				this.iconList[i].color = Color.white;
				this.iconList[i].transform.parent.Find("lock").gameObject.SetActive(false);
				this.kuangList[i].gameObject.GetComponent<UISprite>().spriteName = "avatar_bg_normal";
			}
		}
		string battleMap = Solarmax.Singleton<LocalPlayer>.Get().battleMap;
		this.curMap.Switch(battleMap, true);
		this.readyBtn.isEnabled = false;
		this.NameLabel.text = string.Empty;
		this.DescLabel.text = string.Empty;
		int num = 0;
		if (Solarmax.Singleton<LocalPlayer>.Get().mathPlayer != null)
		{
			for (int j = 0; j < Solarmax.Singleton<LocalPlayer>.Get().mathPlayer.Count; j++)
			{
				PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().mathPlayer[j];
				Color color = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)((byte)num + 1)).color;
				color.a = 1f;
				this.playerNameList[num].text = playerData.name;
				this.playerNameList[num].color = color;
				num++;
			}
		}
		this.timeCount = 10;
		base.InvokeRepeating("UpdateTime", 0f, 1f);
	}

	private void UpdateTime()
	{
		if (this.timeCount == 0)
		{
			return;
		}
		this.timeCount--;
		this.TimeLabel.text = string.Format("{0:D2}:{1:D2}", this.timeCount / 60, this.timeCount % 60);
		if (this.timeCount == 0)
		{
			this.ClearClickEvent();
		}
	}

	private void OnIconClicked(GameObject obj)
	{
		string name = obj.transform.parent.name;
		int num = int.Parse(name.Substring(4)) - 1;
		if (this.selfRaces == null || this.selfRaces[num].race_lock)
		{
			return;
		}
		RaceConfig data = Solarmax.Singleton<RaceConfigProvider>.Instance.GetData(num + 1);
		this.NameLabel.text = data.name;
		this.DescLabel.text = data.desc;
		this.readyBtn.isEnabled = true;
		if (this.selectIndex >= 0)
		{
			this.kuangList[this.selectIndex].gameObject.GetComponent<UISprite>().spriteName = "avatar_bg_normal";
		}
		this.kuangList[num].gameObject.GetComponent<UISprite>().spriteName = "avatar_bg_selected";
		this.selectIndex = num;
		Solarmax.Singleton<NetSystem>.Instance.helper.SendSelectRace(this.selectIndex + 1);
	}

	public void OnStartGame()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.SendRaceEntry();
		Solarmax.Singleton<LocalPlayer>.Get().battleRace = this.selectIndex + 1;
		this.ClearClickEvent();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId != EventId.OnStartSelectRace)
		{
			if (eventId == EventId.OnRoomListREfresh)
			{
				if (args.Length != 3)
				{
					return;
				}
				IList<int> list = args[0] as IList<int>;
				IList<int> list2 = args[1] as IList<int>;
				IList<bool> list3 = args[2] as IList<bool>;
				for (int i = 0; i < list.Count; i++)
				{
					int num = list[i];
					int num2 = list2[i];
					bool flag = list3[i];
					if (num2 <= 0)
					{
						Tips.Make(Tips.TipsType.FlowUp, string.Format(LanguageDataProvider.GetValue(15), num, num2), 1f);
					}
					else
					{
						Color color = this.playerNameList[i].color;
						UISprite uisprite = this.playerIconList[i];
						UISprite uisprite2 = this.playerIconEffectList[i];
						if (flag)
						{
							RaceConfig data = Solarmax.Singleton<RaceConfigProvider>.Instance.GetData(num2);
							if (data != null && !string.IsNullOrEmpty(data.icon))
							{
								uisprite.spriteName = data.icon;
								uisprite2.spriteName = data.icon + "_faction_bg";
								uisprite2.color = color;
							}
							else
							{
								Debug.LogErrorFormat("Race table error, key:{0}", new object[]
								{
									i + 1
								});
							}
							uisprite.gameObject.SetActive(true);
							uisprite2.gameObject.SetActive(true);
						}
						else
						{
							uisprite.gameObject.SetActive(false);
							uisprite2.gameObject.SetActive(false);
						}
					}
				}
			}
		}
		else
		{
			this.selfRaces = (args[0] as IList<RaceData>);
			this.SetInfo();
		}
	}

	private void ClearClickEvent()
	{
		this.readyBtn.isEnabled = false;
		for (int i = 0; i < this.kuangList.Length; i++)
		{
			UIEventListener uieventListener = this.kuangList[i];
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(uieventListener.onClick, new UIEventListener.VoidDelegate(this.OnIconClicked));
		}
	}

	public void OnPressIcon(GameObject obj, bool state)
	{
		if (state)
		{
			this.PressIcon(obj);
		}
		else
		{
			this.ReleaseIcon();
		}
	}

	private void PressIcon(GameObject obj)
	{
		string name = obj.transform.parent.name;
		int index = int.Parse(name.Substring(4)) - 1;
		if (this.selfRaces == null || this.selfRaces[index].race_lock)
		{
			return;
		}
		this.pressRaceIndex = index;
		this.racePress = true;
	}

	private void Update()
	{
		if (this.racePress && this.racePressTime < 0.5f)
		{
			this.racePressTime += Time.deltaTime;
			if (this.racePressTime > 0.5f)
			{
				this.ShowRaceTip(this.pressRaceIndex);
			}
		}
	}

	private void ShowRaceTip(int index)
	{
		if (this.selfRaces == null || this.selfRaces[index].race_lock)
		{
			return;
		}
		RaceConfig data = Solarmax.Singleton<RaceConfigProvider>.Instance.GetData(index + 1);
		this.raceTip.SetActive(true);
		int num = 0;
		PlayerData playerData = null;
		int num2 = 0;
		while (Solarmax.Singleton<LocalPlayer>.Get().mathPlayer != null && num2 < Solarmax.Singleton<LocalPlayer>.Get().mathPlayer.Count)
		{
			playerData = Solarmax.Singleton<LocalPlayer>.Get().mathPlayer[num2];
			if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == playerData.userId)
			{
				break;
			}
			num++;
			num2++;
		}
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)((byte)num + 1));
		Color color = team.color;
		color.a = 1f;
		this.tipIconBG.color = color;
		this.tipIcon.spriteName = data.icon;
		this.username.text = playerData.name;
		this.raceinfo.text = string.Format("{0} / {1}级", data.name, this.selfRaces[index].level);
		for (int i = 0; i < 6; i++)
		{
			if (i < this.selfRaces[index].skills.Count)
			{
				int id = this.selfRaces[index].skills[i];
				RaceSkillConfig data2 = Solarmax.Singleton<RaceSkillConfigProvider>.Instance.GetData(id);
				if (data2 != null)
				{
					this.skillname[i].enabled = true;
					this.skilldesc[i].enabled = true;
					this.skillname[i].text = data2.name;
					this.skilldesc[i].text = data2.desc;
				}
				else
				{
					this.skillname[i].enabled = false;
					this.skilldesc[i].enabled = false;
				}
			}
			else
			{
				this.skillname[i].enabled = false;
				this.skilldesc[i].enabled = false;
			}
		}
	}

	public void ReleaseIcon()
	{
		this.raceTip.SetActive(false);
		this.racePress = false;
		this.racePressTime = 0f;
	}

	public UISprite[] iconList;

	public UIEventListener[] kuangList;

	public UILabel[] levelList;

	private int selectIndex = -1;

	public UISprite[] playerIconList;

	public UISprite[] playerIconEffectList;

	public UILabel[] playerNameList;

	public MapShow curMap;

	public UIButton readyBtn;

	public UILabel NameLabel;

	public UILabel DescLabel;

	public UILabel TimeLabel;

	private int timeCount = 20;

	public GameObject raceTip;

	public UISprite tipIconBG;

	public UISprite tipIcon;

	public UILabel username;

	public UILabel raceinfo;

	public UILabel[] skillname;

	public UILabel[] skilldesc;

	private IList<RaceData> selfRaces;

	private int pressRaceIndex;

	private bool racePress;

	private float racePressTime;
}
