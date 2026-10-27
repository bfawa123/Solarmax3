using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class RaceInfoWindow : BaseWindow
{
	private void Awake()
	{
		for (int i = 0; i < this.skills.Length; i++)
		{
			Transform transform = this.skills[i].transform.Find("kuang");
			UIEventListener component = transform.gameObject.GetComponent<UIEventListener>();
			component.onClick = new UIEventListener.VoidDelegate(this.OnSkillClick);
		}
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnRaceWindowSelect);
		base.RegisterEvent(EventId.OnRaceSkillLevelUp);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnRaceWindowSelect)
		{
			RaceData raceData = args[0] as RaceData;
			this.curRaceId = raceData.race;
			this.curRaceLevel = raceData.level;
			this.curRaceSkills = new int[raceData.skills.Count];
			for (int i = 0; i < this.curRaceSkills.Length; i++)
			{
				this.curRaceSkills[i] = raceData.skills[i];
			}
			this.raceConfig = Solarmax.Singleton<RaceConfigProvider>.Instance.GetData(raceData.race);
			this.SetInfo();
		}
		else if (eventId == EventId.OnRaceSkillLevelUp)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_RaceSkillIndexErr)
			{
				Tips.Make(Tips.TipsType.Debug, LanguageDataProvider.GetValue(11), 1f);
			}
			else if (errCode == ErrCode.EC_RaceSkillInvalidSkill)
			{
				Tips.Make(Tips.TipsType.Debug, LanguageDataProvider.GetValue(12), 1f);
			}
			else if (errCode == ErrCode.EC_RaceCostNotEnough)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(13), 1f);
			}
			else if (errCode == ErrCode.EC_Ok)
			{
				int num = (int)args[1];
				if (num != this.curRaceId)
				{
					return;
				}
				this.curRaceLevel = (int)args[2];
				int num2 = (int)args[3];
				int num3 = (int)args[4];
				this.curRaceSkills[num2 - 1] = num3;
				this.SetInfo();
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRaceSkillInfoSelect, new object[]
				{
					this.curRaceId,
					num3,
					this.selectSkillIndex
				});
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(14), 1f);
			}
		}
	}

	private void SetInfo()
	{
		this.raceName.text = this.raceConfig.name;
		this.raceLevel.text = "Lv." + this.curRaceLevel;
		this.raceIcon.spriteName = this.raceConfig.icon;
		this.raceDesc.text = this.raceConfig.desc;
		for (int i = 0; i < this.skills.Length; i++)
		{
			RaceSkillConfig data = Solarmax.Singleton<RaceSkillConfigProvider>.Instance.GetData(this.curRaceSkills[i]);
			if (data != null)
			{
				string spriteName = "Default";
				if (!string.IsNullOrEmpty(data.image))
				{
					spriteName = data.image;
				}
				this.skills[i].transform.Find("icon").GetComponent<UISprite>().spriteName = spriteName;
				this.skills[i].transform.Find("lv").GetComponent<UILabel>().text = "Lv." + data.level;
				this.skills[i].transform.Find("name").GetComponent<UILabel>().text = data.name;
				UILabel component = this.skills[i].transform.Find("exp").GetComponent<UILabel>();
				UIProgressBar component2 = this.skills[i].transform.Find("slider").GetComponent<UIProgressBar>();
			}
		}
	}

	private void OnSkillClick(GameObject go)
	{
		if (this.curRaceId <= 0)
		{
			return;
		}
		string name = go.transform.parent.name;
		this.selectSkillIndex = int.Parse(name.Substring(5)) - 1;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("RaceSkillInfoDialogWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRaceSkillInfoSelect, new object[]
		{
			this.curRaceId,
			this.curRaceSkills[this.selectSkillIndex],
			this.selectSkillIndex
		});
		this.SetInfo();
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestRaceData();
		Solarmax.Singleton<UISystem>.Get().HideWindow("RaceInfoWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("RaceWindow");
	}

	public UILabel raceName;

	public UILabel raceLevel;

	public UISprite raceIcon;

	public UILabel raceDesc;

	public GameObject[] skills;

	public GameObject cup;

	public GameObject gold;

	public GameObject diamond;

	private int curRaceId;

	private int curRaceLevel;

	private int[] curRaceSkills;

	private RaceConfig raceConfig;

	private int selectSkillIndex;

	private Color colorB = new Color(0f, 0.82f, 1f, 1f);

	private Color colorW = new Color(1f, 1f, 1f, 0.5f);
}
