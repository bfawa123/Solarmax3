using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class Team
{
	public Team()
	{
		this.teamFriend = new bool[LocalPlayer.MaxTeamNum];
	}

	public Team(CampConfig cfg)
	{
		this.teamFriend = new bool[LocalPlayer.MaxTeamNum];
		this.mCampConfig = cfg;
		this.playerData = new PlayerData();
		this.playerData.Reset();
		this.playerData.currentTeam = this;
		for (int i = 0; i < this.attribute.Length; i++)
		{
			this.attribute[i] = new AttributeObject();
		}
		this.skillBuffLogic = new NewSkillBuffLogic();
		this.Clear();
	}

	public TeamManager teamManager { get; set; }

	public PlayerData playerData { get; set; }

	public Color color { get; set; }

	public string iconname { get; set; }

	public TEAM team { get; set; }

	public float hpMax
	{
		get
		{
			return this.GetAttributeFloat(TeamAttr.HpMax);
		}
	}

	public float attack
	{
		get
		{
			return this.GetAttributeFloat(TeamAttr.Attack);
		}
	}

	public float speed
	{
		get
		{
			return this.GetAttributeFloat(TeamAttr.Speed);
		}
	}

	public int current
	{
		get
		{
			return this.GetAttributeInt(TeamAttr.Population);
		}
	}

	public bool isDead
	{
		get
		{
			if (this.current <= 0 && this.currentMax < 0)
			{
				Debug.LogErrorFormat("捕获到最大人口数为负值, contact fangjun, Team:{0}, id:{1}, currentMax:{2}", new object[]
				{
					this.team,
					this.playerData.userId,
					this.currentMax
				});
			}
			return this.current <= 0 && this.currentMax <= 0;
		}
	}

	public int currentMax
	{
		get
		{
			return this.GetAttributeInt(TeamAttr.PopulationMax);
		}
	}

	public int groupID { get; set; }

	public int scoreMod { get; set; }

	public int resultOrder { get; set; }

	public int resultRank { get; set; }

	public EndType resultEndtype { get; set; }

	public int rewardMoney { get; set; }

	public int rewardMuitly { get; set; }

	public int leagueMvp { get; set; }

	public bool isEnd { get; set; }

	public bool hidePopution
	{
		get
		{
			return this.GetAttributeInt(TeamAttr.HidePop) > 0;
		}
	}

	public bool hideFly
	{
		get
		{
			return this.GetAttributeInt(TeamAttr.HideFly) > 0;
		}
	}

	public void Clear()
	{
		this.playerData.Reset();
		this.destory = 0;
		this.hitships = 0;
		this.produces = 0;
		this.star = 1;
		this.scoreMod = 0;
		this.resultOrder = 0;
		this.resultRank = -1;
		this.resultEndtype = EndType.ET_Dead;
		this.leagueMvp = 0;
		this.isEnd = false;
		this.groupID = (int)this.team;
		this.aiEnable = false;
		this.cdList.Clear();
		this.skillBuffLogic.Destroy();
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			this.teamFriend[i] = false;
		}
		if (this.groupID >= 0 && this.groupID < LocalPlayer.MaxTeamNum)
		{
			this.teamFriend[this.groupID] = true;
		}
		for (int j = 0; j < this.attribute.Length; j++)
		{
			this.attribute[j].Reset();
		}
		this.SetAttributeBase(TeamAttr.Speed, this.mCampConfig.speed);
		this.SetAttributeBase(TeamAttr.Attack, this.mCampConfig.attack);
		this.SetAttributeBase(TeamAttr.HpMax, this.mCampConfig.maxhp);
		this.SetAttributeBase(TeamAttr.CapturedSpeed, this.mCampConfig.capturedspeed);
		this.SetAttributeBase(TeamAttr.OccupiedSpeed, this.mCampConfig.occupiedspeed);
		this.SetAttributeBase(TeamAttr.ProduceSpeed, this.mCampConfig.producespeed);
		this.SetAttributeBase(TeamAttr.BeCapturedSpeed, this.mCampConfig.becapturedspeed);
	}

	public void SetCampConfig(CampConfig cfg)
	{
		if (cfg == null)
		{
			return;
		}
		this.mCampConfig = cfg;
		this.color = cfg.campcolor;
		this.SetAttributeBase(TeamAttr.Speed, this.mCampConfig.speed);
		this.SetAttributeBase(TeamAttr.Attack, this.mCampConfig.attack);
		this.SetAttributeBase(TeamAttr.HpMax, this.mCampConfig.maxhp);
		this.SetAttributeBase(TeamAttr.CapturedSpeed, this.mCampConfig.capturedspeed);
		this.SetAttributeBase(TeamAttr.OccupiedSpeed, this.mCampConfig.occupiedspeed);
		this.SetAttributeBase(TeamAttr.ProduceSpeed, this.mCampConfig.producespeed);
		this.SetAttributeBase(TeamAttr.BeCapturedSpeed, this.mCampConfig.becapturedspeed);
	}

	public void StartTeam()
	{
		this.playerData.skillPower = 0;
		this.cdList.Clear();
	}

	public bool IsFriend(int group)
	{
		if (!Team.UsingNewTeammate)
		{
			return this.groupID != -1 && group != -1 && this.groupID == group;
		}
		if (group < 0 || group >= LocalPlayer.MaxTeamNum)
		{
			return false;
		}
		if (this.groupID < 0 || this.groupID >= LocalPlayer.MaxTeamNum)
		{
			return false;
		}
		return this.IsTeammate(group);
	}

	public object Clone()
	{
		return base.MemberwiseClone();
	}

	public bool Valid()
	{
		return this.playerData.userId != -1;
	}

	public void UpdateEvent(int frame, float dt)
	{
		int i = 0;
		while (i < this.cdList.Count)
		{
			this.cdList[i].cd -= dt;
			if (this.cdList[i].cd <= 0f)
			{
				this.cdList.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
		this.checkLoseAllMatrix(dt);
		this.skillBuffLogic.Tick(frame, dt);
	}

	public void AddSkillCD(SkillCoolDown cd)
	{
		this.cdList.Add(cd);
	}

	public float CheckSkillCD(int skillID)
	{
		for (int i = 0; i < this.cdList.Count; i++)
		{
			if (this.cdList[i].skillID == skillID)
			{
				return this.cdList[i].cd;
			}
		}
		return 0f;
	}

	public bool CheckDead()
	{
		return this.current <= 0 && !this.teamManager.sceneManager.nodeManager.CheckHaveNode((int)this.team);
	}

	public bool CheckPVEDead()
	{
		if (this.current <= 0)
		{
			if (this.playerData.userId > 0)
			{
				if (this.teamManager.sceneManager.nodeManager.CheckHaveProduceNode((int)this.team))
				{
					return false;
				}
			}
			else if (this.teamManager.sceneManager.nodeManager.CheckHaveNode((int)this.team))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void SetAttributeBase(TeamAttr attr, float num)
	{
		AttributeObject attributeObject = this.attribute[(int)attr];
		attributeObject.baseNum = num;
		attributeObject.Calculate();
	}

	public void SetAttribute(TeamAttr attr, float num, bool absolute)
	{
		AttributeObject attributeObject = this.attribute[(int)attr];
		if (absolute)
		{
			attributeObject.addNum += num;
		}
		else
		{
			attributeObject.addPercent += num;
		}
		if (attr == TeamAttr.HideFly || attr == TeamAttr.HidePop || attr == TeamAttr.StopBeCapture || attr == TeamAttr.QuickMove)
		{
			attributeObject.addNum = (float)((num <= 0f) ? 0 : 1);
		}
		attributeObject.Calculate();
	}

	public float GetAttributeFloat(TeamAttr attr)
	{
		return this.attribute[(int)attr].fixedNum;
	}

	public int GetAttributeInt(TeamAttr attr)
	{
		return Convert.ToInt32(this.GetAttributeFloat(attr));
	}

	public float GetAttributeBaseFloat(TeamAttr attr)
	{
		return this.attribute[(int)attr].baseNum;
	}

	public int GetAttributeBaseInt(TeamAttr attr)
	{
		return Convert.ToInt32(this.GetAttributeBaseFloat(attr));
	}

	public int GetAttributeAD(TeamAttr attr)
	{
		AttributeObject attributeObject = this.attribute[(int)attr];
		return attributeObject.fixedNum.CompareTo(attributeObject.baseNum);
	}

	public AttributeObject GetAttribute(TeamAttr attr)
	{
		return this.attribute[(int)attr];
	}

	private void checkLoseAllMatrix(float dt)
	{
	}

	public float GetLoseAllMatrixTime(float dt = 0f)
	{
		if (this.teamManager.sceneManager.nodeManager.CheckHaveNode((int)this.team))
		{
			this.m_loseAllMatrixTime = 0f;
			return 0f;
		}
		this.m_loseAllMatrixTime += dt;
		return this.m_loseAllMatrixTime;
	}

	public bool IsTeammate(int group)
	{
		if (group < 0 || group >= LocalPlayer.MaxTeamNum)
		{
			return false;
		}
		if (this.groupID < 0 || this.groupID >= LocalPlayer.MaxTeamNum)
		{
			return false;
		}
        Team team = this.teamManager.GetTeam((TEAM)group);
		if (team == null)
		{
			return false;
		}
		LoggerSystem.CodeComments("代码注释-结盟列表的运作原理by天凌喵：结盟列表类型bool[]，表示自身与其他队伍的结盟关系。例如队伍0与队伍3结盟，那么队伍0的结盟列表第四项[3]为true,队伍3的结盟列表第一项[0]为true");
        return this.teamFriend[group] == team.teamFriend[group] && this.teamFriend[group] && team.teamFriend[this.groupID];
	}

	public bool aiEnable;

	public int destory;

	public int hitships;

	public int produces;

	public int star = 1;

	private List<SkillCoolDown> cdList = new List<SkillCoolDown>();

	public NewSkillBuffLogic skillBuffLogic;

	private AttributeObject[] attribute = new AttributeObject[13];

	public bool attributeChanged;

	private float m_loseAllMatrixTime;

	private CampConfig mCampConfig;

	public static bool UsingNewTeammate = true;

	public bool[] teamFriend = new bool[LocalPlayer.MaxTeamNum];
}
