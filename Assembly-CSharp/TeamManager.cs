using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class TeamManager : Lifecycle2
{
	public TeamManager(SceneManager mgr)
	{
		this.sceneManager = mgr;
	}

	public SceneManager sceneManager { get; set; }

	public bool Init()
	{
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			CampConfig data = Solarmax.Singleton<CampConfigConfigProvider>.Instance.GetData(i.ToString());
			if (data != null)
			{
				this.teamArray[i] = new Team(data);
				this.teamArray[i].team = (TEAM)i;
				this.teamArray[i].color = data.campcolor;
				this.teamArray[i].teamManager = this;
			}
		}
		this.Release();
		return true;
	}

	public void Tick(int frame, float interval)
	{
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			this.teamArray[i].UpdateEvent(frame, interval);
		}
		if (frame % 50 != 0)
		{
			return;
		}
		for (int j = 0; j < this.teamArray.Length; j++)
		{
			if (this.teamArray[j].playerData != null)
			{
				PlayerData playerData = this.teamArray[j].playerData;
				int skillPower = playerData.skillPower;
				playerData.skillPower = skillPower + 1;
			}
		}
	}

	public void Destroy()
	{
		this.Release();
	}

	public void Release()
	{
		Team[] array = this.teamArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
	}

	public Team GetTeam(TEAM team)
	{
		if ((int)team < 0 || (int)team >= this.teamArray.Length)
		{
			return null;
		}
		return this.teamArray[(int)team];
	}

	public List<Team> GetFriendTeam(TEAM team)
	{
		List<Team> list = new List<Team>();
		if (team < TEAM.Neutral && (int)team >= this.teamArray.Length)
		{
			return list;
		}
		Team team2 = this.teamArray[(int)team];
		list.Add(team2);
		if (team2.groupID == -1)
		{
			return list;
		}
		foreach (Team team3 in this.teamArray)
		{
			if (team3 != team2 && team3.groupID == team2.groupID)
			{
				list.Add(team3);
			}
		}
		return list;
	}

	public Team GetTeam(int userId)
	{
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			if (this.teamArray[i].playerData.userId == userId)
			{
				return this.teamArray[i];
			}
		}
		return null;
	}

	public TEAM GetTEAM(int userId)
	{
		Team team = this.GetTeam(userId);
		if (team != null)
		{
			return team.team;
		}
		return TEAM.Neutral;
	}

	public bool OnlyTeam(int eTEAM)
	{
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			if ((int)this.teamArray[i].team != eTEAM && this.teamArray[i].current > 0)
			{
				return false;
			}
		}
		return true;
	}

	public int OnlyTeam()
	{
		int num = LocalPlayer.MaxTeamNum;
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			if (this.teamArray[i].current > 0)
			{
				if (num != LocalPlayer.MaxTeamNum)
				{
					if (!this.teamArray[num].IsFriend(this.teamArray[i].groupID))
					{
						return LocalPlayer.MaxTeamNum;
					}
					if (this.teamArray[i].current > this.teamArray[num].current)
					{
						num = (int)this.teamArray[i].team;
					}
				}
				else
				{
					num = (int)this.teamArray[i].team;
				}
			}
		}
		return num;
	}

	public void AddDestory(TEAM team)
	{
		Team team2 = this.GetTeam(team);
		if (team2 == null)
		{
			return;
		}
		team2.destory++;
	}

	public void AddHitShip(TEAM team)
	{
		Team team2 = this.GetTeam(team);
		if (team2 == null)
		{
			return;
		}
		team2.hitships++;
	}

	public void AddProduce(TEAM team, int num)
	{
		Team team2 = this.GetTeam(team);
		if (team2 == null)
		{
			return;
		}
		team2.produces += num;
	}

	public int[] GetDestoryArray()
	{
		int[] array = new int[LocalPlayer.MaxTeamNum];
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.GetTeam((TEAM)i);
			if (team != null)
			{
				if (team.Valid())
				{
					array[i] = team.destory;
				}
				else
				{
					array[i] = -1;
				}
			}
		}
		return array;
	}

	public TEAM CalcSkillTar(TEAM source, SkillConfig tab)
	{
		TEAM result = TEAM.Neutral;
		switch (tab.skilltype)
		{
		case 1:
		case 2:
		case 3:
		case 5:
		case 9:
		case 11:
		case 15:
		case 18:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
			result = source;
			break;
		case 4:
			for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
			{
				Team team = this.GetTeam((TEAM)i);
				if (team.team != TEAM.Neutral && team.team != source)
				{
					result = team.team;
					break;
				}
			}
			break;
		case 6:
		case 12:
		case 19:
			result = TEAM.Team_7;
			break;
		case 7:
		case 8:
		case 10:
		{
			int num = 0;
			for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
			{
				Team team2 = this.GetTeam((TEAM)j);
				if (team2.team != TEAM.Neutral && team2.team != source && team2.current >= num)
				{
					num = team2.current;
					result = team2.team;
				}
			}
			break;
		}
		case 13:
		{
			int num2 = 0;
			for (int k = 1; k < LocalPlayer.MaxTeamNum; k++)
			{
				Team team3 = this.GetTeam((TEAM)k);
				if (team3.team != TEAM.Neutral && team3.team != source && team3.current >= num2)
				{
					num2 = team3.current;
					result = team3.team;
				}
			}
			break;
		}
		case 16:
			result = source;
			break;
		case 17:
			result = TEAM.Team_7;
			break;
		}
		return result;
	}

	public void AddSkillCD(TEAM source, SkillCoolDown cd)
	{
		if (source == TEAM.Team_7)
		{
			return;
		}
		this.teamArray[(int)source].AddSkillCD(cd);
	}

	public float CheckSkillCD(TEAM t, int skillID)
	{
		if (t == TEAM.Team_7)
		{
			return 0f;
		}
		return this.teamArray[(int)t].CheckSkillCD(skillID);
	}

	public void SetAllTeamMoveSpeed(float speedPercent = 1f)
	{
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			if (this.teamArray[i].GetAttribute(TeamAttr.Speed).addPercent != speedPercent)
			{
				this.teamArray[i].SetAttribute(TeamAttr.Speed, speedPercent, false);
			}
		}
	}

	public void SetAllTeamProduceSpeed(float speedPercent = 1f)
	{
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			if (this.teamArray[i].GetAttribute(TeamAttr.ProduceSpeed).addPercent != speedPercent)
			{
				this.teamArray[i].SetAttribute(TeamAttr.ProduceSpeed, speedPercent, false);
			}
		}
	}

	public void SetAllOccupySpeed(float speedPercent = 1f)
	{
		for (int i = 0; i < this.teamArray.Length; i++)
		{
			if (this.teamArray[i].GetAttribute(TeamAttr.CapturedSpeed).addPercent != speedPercent)
			{
				this.teamArray[i].SetAttribute(TeamAttr.CapturedSpeed, speedPercent, false);
				this.teamArray[i].SetAttribute(TeamAttr.OccupiedSpeed, speedPercent, false);
			}
		}
	}

	private static Team[] STATIC_TEAM = new Team[]
	{
		new Team
		{
			color = new Color32(204, 204, 204, 0),
			team = TEAM.Neutral,
			iconname = "avatar_bg_normal"
		},
		new Team
		{
			color = new Color32(95, 182, byte.MaxValue, 0),
			team = TEAM.Team_1,
			iconname = "avatar_bg_faction_blue"
		},
		new Team
		{
			color = new Color32(byte.MaxValue, 93, 147, 0),
			team = TEAM.Team_2,
			iconname = "avatar_bg_faction_red"
		},
		new Team
		{
			color = new Color32(byte.MaxValue, 140, 90, 0),
			team = TEAM.Team_3,
			iconname = "avatar_bg_faction_yellow"
		},
		new Team
		{
			color = new Color32(202, byte.MaxValue, 110, 0),
			team = TEAM.Team_4,
			iconname = "avatar_bg_faction_green"
		},
		new Team
		{
			color = new Color32(153, 153, 153, 0),
			team = TEAM.Team_5
		},
		new Team
		{
			color = new Color32(153, 153, 153, 0),
			team = TEAM.Team_6
		},
		new Team
		{
			color = new Color32(254, 197, 54, 0),
			team = TEAM.Team_Black1
		},
		new Team
		{
			color = new Color32(27, 146, 75, 0),
			team = TEAM.Team_Black2
		}
	};

	private Team[] teamArray = new Team[LocalPlayer.MaxTeamNum];
}
