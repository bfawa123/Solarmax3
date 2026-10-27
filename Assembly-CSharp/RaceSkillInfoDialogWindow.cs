using System;
using Solarmax;
using UnityEngine;

public class RaceSkillInfoDialogWindow : BaseWindow
{
	private void Start()
	{
	}

	private void Awake()
	{
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnRaceSkillInfoSelect);
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
		if (eventId == EventId.OnRaceSkillInfoSelect)
		{
			this.curRaceId = (int)args[0];
			this.selectSkillID = (int)args[1];
			this.selectSkillIndex = (int)args[2];
			this.ShowSkillInfo();
		}
	}

	private void ShowSkillInfo()
	{
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("RaceSkillInfoDialogWindow");
	}

	public void OnSkillLevelUp()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestRaceSkillLevelUp(this.curRaceId, this.selectSkillIndex + 1);
	}

	private int curRaceId;

	private int selectSkillIndex;

	private int selectSkillID;

	public UILabel skillDesc;

	public GameObject skillInfo;

	public UILabel skillInfoAdd;

	public UIButton skillLevelUp;

	public UILabel skillLevelUpCost;

	private Color colorB = new Color(0f, 0.82f, 1f, 1f);

	private Color colorW = new Color(1f, 1f, 1f, 0.5f);
}
