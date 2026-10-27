using System;
using System.Collections.Generic;
using System.Net.Sockets;
using NetMessage;
using Plugin;
using Solarmax;
using UnityEngine;

public class PVPBattleController : IBattleController, Lifecycle2
{
	public PVPBattleController(BattleData bd, BattleSystem bs)
	{
		this.battleData = bd;
		this.battleSystem = bs;
	}

	public bool Init()
	{
		this.tickEndCount = 0;
		this.diedTeamCount = 0;
		this.bSelectCenterPos = false;
		this.ApplyCoopWinConditionFromLevelConfig();
		this.battleTimeMax = this.GetBattleTimeMaxSec();
		this.survivorTimeMax = int.Parse(Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(6));
		this.speedUp = false;
		this.center = Vector3.zero;
		this.centerPlanet = string.Empty;
		this.bombTarget = null;
		this.lastElapsed = 0;
		this.narrowtimes = -2;
		this.hasTriggerClearBarrier = false;
        for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
        {
            Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
            team.groupID = i;
            team.teamFriend[i] = true;
        }
        return true;
	}

	private void ApplyCoopWinConditionFromLevelConfig()
	{
		if (!this.IsCoopVsPC())
		{
			return;
		}
		string matchId = this.battleData.matchId;
		if (string.IsNullOrEmpty(matchId))
		{
			return;
		}
		MapConfig data2 = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(matchId);
		if (data2 != null && data2.survivalTimeSec > 0f)
		{
			this.battleData.winType = "alive";
			this.battleData.winTypeParam1 = data2.survivalTimeSec.ToString();
			this.battleData.winTypeParam2 = string.Empty;
			return;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(matchId);
		if (data == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.battleData.winType))
		{
			this.battleData.winType = data.winType;
		}
		if (string.IsNullOrEmpty(this.battleData.winTypeParam1))
		{
			this.battleData.winTypeParam1 = data.winTypeParam1;
		}
		if (string.IsNullOrEmpty(this.battleData.winTypeParam2))
		{
			this.battleData.winTypeParam2 = data.winTypeParam2;
		}
	}

	public void Tick(int frame, float interval)
	{
		if (this.battleSystem.battleData.battleType == BattlePlayType.Replay)
		{
			if (this.battleSystem.battleData.useCommonEndCondition)
			{
				this.UpdateEndSingle(frame, interval);
			}
			else
			{
				this.UpdateEndAllOccupiedSingle(frame, interval);
			}
		}
		else
		{
			this.UpdateEnd(frame, interval);
		}
		this.UpdateEndOtherWinLoseType(frame, interval);
		this.UpdateResuming(frame, interval);
		if (this.battleSystem.battleData.gameType == GameType.PVP && this.battleSystem.battleData.battleType != BattlePlayType.Replay)
		{
			this.UpdateBattleSpeed();
			this.UpdateBombPlanet();
		}
	}

	public void Destroy()
	{
		this.bSelectCenterPos = false;
	}

	public void Reset()
	{
		this.tickEndCount = 0;
		this.tickEndOtherCount = 0;
		this.bSelectCenterPos = false;
		this.battleEndDatas.Clear();
		this.coopSurvivalWinFired = false;
	}

	public void OnRecievedFramePacket(SCFrame frame)
	{
		List<global::Packet> list = new List<global::Packet>();
        for (int i = 0; i < frame.users.Count; i++)
		{
            if (frame.users.Count > 4)
			{
				Debug.LogError(" replay error mapid= " + Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
			}
			int num;
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.PVP)
			{
				num = Solarmax.Singleton<BattleSystem>.Instance.battleData.pvpUserToTeamIndex[frame.users[i]];
			}
			else
			{
				num = frame.users[i];
			}
			PbFrames pbFrames = frame.frames[i];
            if (pbFrames != null && pbFrames.frames.Count != 0)
			{
				for (int j = 0; j < pbFrames.frames.Count; j++)
				{
					PbFrame pbFrame = pbFrames.frames[j];
                    if (pbFrame != null)
					{
						global::Packet packet = new global::Packet();
						if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType == BattlePlayType.Replay)
						{
							packet.team = (TEAM)num;
						}
						else
						{
							packet.team = (TEAM)(num + 1);
						}
						packet.packet = Json.DeCode<FramePacket>(pbFrame.content);
						list.Add(packet);
                    }
                }
			}
		}
		this.battleSystem.lockStep.AddFrame(frame.frameNum, list.ToArray());
		list.Clear();
	}

	public void OnRecievedScriptFrame(PbSCFrames frames)
	{
		int i = 0;
		while (i < frames.frames.Count)
		{
			List<global::Packet> list = new List<global::Packet>();
			int frameNum = frames.frames[i].frameNum;
			for (int j = i; j < frames.frames.Count; j++)
			{
				SCFrame scframe = frames.frames[j];
				if (scframe.frameNum != frameNum)
				{
					break;
				}
				i++;
				PbFrames pbFrames = scframe.frames[0];
				if (pbFrames != null && pbFrames.frames.Count != 0)
				{
					for (int k = 0; k < pbFrames.frames.Count; k++)
					{
						PbFrame pbFrame = pbFrames.frames[k];
						if (pbFrame != null)
						{
							list.Add(new global::Packet
							{
								team = TEAM.Neutral,
								packet = Json.DeCode<FramePacket>(pbFrame.content)
							});
						}
					}
				}
			}
			this.battleSystem.lockStep.AddFrame(frameNum, list.ToArray());
            list.Clear();
		}
	}

	public void OnRunFramePacket(FrameNode frameNode)
	{
        this.battleSystem.sceneManager.RunFramePacket(frameNode);
	}

	public void OnPlayerMove(Node from, Node to)
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.SendMoveMessaeg(from, to);
	}

	public void PlayerGiveUp()
	{
		this.SendGiveUpFrame();
	}

	public void OnPlayerDiedTimeout(TEAM t, int frame)
	{
		List<Node> usefulNodeList = this.battleSystem.sceneManager.nodeManager.GetUsefulNodeList();
		foreach (Node node in usefulNodeList)
		{
			node.BombShip(t, TEAM.Neutral, 1f);
		}
		List<Ship> flyShip = this.battleSystem.sceneManager.shipManager.GetFlyShip(t);
		for (int i = 0; i < flyShip.Count; i++)
		{
			flyShip[i].Bomb(NodeType.None);
		}
		this.OnPlayerDiedOrGiveUp(EndType.ET_Dead, t, frame);
		if (t == this.battleSystem.battleData.currentTeam)
		{
			TouchHandler.HideOperater();
		}
	}

	public void OnPlayerGiveUp(TEAM giveUpTeam)
	{
		this.OnPlayerDiedOrGiveUp(EndType.ET_Giveup, giveUpTeam, this.battleSystem.GetCurrentFrame());
	}

	public void OnPlayerDirectQuit(TEAM team)
	{
		this.QuitBattle(false);
	}

	private void OnPlayerDiedOrGiveUpSingle(EndType type, TEAM team, int frame)
	{
		bool flag = false;
		for (int i = 0; i < this.battleEndDatas.Count; i++)
		{
			if (this.battleEndDatas[i].team == team)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team);
			this.OnPlayerEnterEnd(team2, type, frame);
		}
		int num = 1;
		int j = 0;
		while (j < LocalPlayer.MaxTeamNum)
		{
			Team team3 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)j);
			if (team3 != null && team3.Valid() && !team3.isEnd)
			{
				for (int k = j + 1; k < LocalPlayer.MaxTeamNum; k++)
				{
					Team team4 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)k);
					if (team4 != null && team4.Valid() && !team4.isEnd && !team3.IsFriend(team4.groupID))
					{
						num++;
						break;
					}
				}
				if (num == 1)
				{
					Debug.Log(string.Concat(new object[]
					{
						"加入最后一个队伍:",
						team3.team,
						"    name:",
						team3.playerData.name
					}));
					this.OnPlayerEnterEnd(team3, EndType.ET_Win, frame);
					this.battleData.winTEAM = team3.team;
					break;
				}
				break;
			}
			else
			{
				j++;
			}
		}
		if (num == 1)
		{
			this.QuitBattle(true);
		}
	}

	private void UpdateEnd(int frame, float dt)
	{
		int num = this.tickEndCount + 1;
		this.tickEndCount = num;
		if (num == 5)
		{
			for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
			{
				Team team = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)i);
				if (team != null && team.Valid() && !team.isEnd && team.GetLoseAllMatrixTime(dt * 5f) > (float)this.survivorTimeMax)
				{
					this.OnPlayerDiedTimeout(team.team, frame);
				}
				if (team != null && team.Valid() && !team.isEnd && team.CheckDead())
				{
					this.OnPlayerDiedOrGiveUp(EndType.ET_Dead, team.team, frame);
				}
			}
			if (this.diedTeamCount != this.battleData.currentPlayers - 1)
			{
				this.tickEndCount = 0;
			}
		}
		if (this.battleSystem.sceneManager.GetBattleTime() > (float)this.battleTimeMax)
		{
			for (int j = 0; j < LocalPlayer.MaxTeamNum; j++)
			{
				Team team2 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)j);
				if (team2 != null && team2.Valid() && !team2.isEnd && !team2.CheckDead())
				{
					this.OnPlayerEnterEnd(team2, EndType.ET_Timeout, frame);
				}
			}
			this.QuitBattle(true);
		}
	}

	private void UpdateEndSingle(int frame, float dt)
	{
		int num = this.tickEndCount + 1;
		this.tickEndCount = num;
		if (num == 5)
		{
			for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
			{
				Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
				if (team != null && team.Valid() && !team.isEnd && team.CheckPVEDead())
				{
					this.OnPlayerDiedOrGiveUpSingle(EndType.ET_Dead, team.team, frame);
				}
			}
			if (this.battleEndDatas.Count != this.battleData.currentPlayers)
			{
				this.tickEndCount = 0;
			}
		}
	}

	private void UpdateEndOtherWinLoseType(int frame, float interval)
	{
		int num = this.tickEndOtherCount + 1;
		this.tickEndOtherCount = num;
		if (num == 5)
		{
			bool flag = false;
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "alive" || Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "surival" || Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "survival")
			{
				float num2;
				if (float.TryParse(Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1, out num2) && num2 > 0f && Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime() >= num2)
				{
					if (this.IsCoopVsPC())
					{
						this.TriggerCoopSurvivalWin(frame);
					}
					else
					{
						this.battleData.winTEAM = this.battleData.currentTeam;
						this.QuitBattle(true);
					}
					return;
				}
			}
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "occupy")
			{
				int num3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.OccupiedSomeone((int)this.battleData.currentTeam, Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1, Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam2);
				if (num3 > 0)
				{
					this.battleData.winTEAM = (TEAM)num3;
					flag = true;
				}
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "killnum")
			{
				for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
				{
					Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
					if (team != null && team.hitships >= int.Parse(Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1))
					{
						this.battleData.winTEAM = team.team;
						flag = true;
					}
				}
			}
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.loseType == "lostaimplanet")
			{
				string text = string.Empty;
				MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
				if (currentTable != null && !string.IsNullOrEmpty(currentTable.lostAimPlanet))
				{
					text = currentTable.lostAimPlanet;
				}
				else
				{
					text = Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam1;
					string loseTypeParam = Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam2;
					if (!string.IsNullOrEmpty(loseTypeParam) && loseTypeParam != text)
					{
						text = text + "," + loseTypeParam;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					string[] array2 = text.Split(new char[] { ',' });
					int num4 = (int)this.battleData.currentTeam;
					Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam);
					bool flag2 = false;
					string[] array3 = array2;
					for (int j = 0; j < array3.Length; j++)
					{
						string text2 = array3[j].Trim();
						if (!string.IsNullOrEmpty(text2))
						{
							bool flag3 = false;
							if (currentTable != null && currentTable.mbcList != null)
							{
								foreach (MapBuildingConfig mapBuildingConfig in currentTable.mbcList)
								{
									if (mapBuildingConfig.tag == text2)
									{
										if (mapBuildingConfig.camption == num4)
										{
											flag3 = true;
										}
										else if (team2 != null)
										{
											Team team4 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)mapBuildingConfig.camption);
											if (team4 != null && team2.IsFriend(team4.groupID))
											{
												flag3 = true;
											}
										}
										break;
									}
								}
							}
							if (!flag3)
							{
								continue;
							}
							Node node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(text2);
							if (node != null && (int)node.team != num4 && (team2 == null || !node.currentTeam.IsFriend(team2.groupID)))
							{
								flag2 = true;
								break;
							}
						}
					}
					if (flag2)
					{
						for (int k = 0; k < LocalPlayer.MaxTeamNum; k++)
						{
							if (k != num4)
							{
								Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)k);
								if (team3 != null && team3.Valid())
								{
									this.battleData.winTEAM = (TEAM)k;
									break;
								}
							}
						}
						this.QuitBattle(true);
						return;
					}
				}
			}
			if (flag)
			{
				this.QuitBattle(true);
				return;
			}
			this.tickEndOtherCount = 0;
		}
	}

	private void TriggerCoopSurvivalWin(int frame)
	{
		if (this.coopSurvivalWinFired)
		{
			return;
		}
		this.coopSurvivalWinFired = true;
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam);
		if (team == null)
		{
			this.battleData.winTEAM = this.battleData.currentTeam;
			this.QuitBattle(true);
			return;
		}
		TEAM tEAM = this.battleData.currentTeam;
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
			if (team2 != null && team2.Valid() && team.IsFriend(team2.groupID))
			{
				tEAM = team2.team;
				break;
			}
		}
		this.battleData.winTEAM = tEAM;
		for (int j = 0; j < LocalPlayer.MaxTeamNum; j++)
		{
			Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)j);
			if (team3 != null && team3.Valid())
			{
				if (team.IsFriend(team3.groupID))
				{
					this.OnPlayerEnterEnd(team3, EndType.ET_Win, frame);
				}
				else
				{
					this.OnPlayerEnterEnd(team3, EndType.ET_Dead, frame);
				}
			}
		}
		this.QuitBattle(true);
	}

	private void UpdateEndOtherWinLoseTypeSingle(int frame, float interval)
	{
		if (++this.tickEndOtherCount == 5)
		{
			bool flag = false;
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "occupy")
			{
				int num = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.OccupiedSomeone((int)this.battleData.currentTeam, Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1, Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam2);
				if (num == (int)this.battleData.currentTeam)
				{
					flag = true;
				}
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "alive" || Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "surival" || Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "survival")
			{
				if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime() >= float.Parse(Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1))
				{
					flag = true;
				}
			}
			else if (!(Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "score"))
			{
				if (Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "killnum" && Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).hitships >= int.Parse(Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1))
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.battleData.winTEAM = this.battleData.currentTeam;
				this.QuitBattle(true);
				return;
			}
			this.tickEndOtherCount = 0;
		}
	}

	private void UpdateEndAllOccupiedSingle(int frame, float interval)
	{
		if (++this.tickEndCount == 5)
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.AllOccupied((int)this.battleData.currentTeam))
			{
				this.battleData.winTEAM = this.battleData.currentTeam;
				this.QuitBattle(true);
				return;
			}
			this.tickEndCount = 0;
		}
	}

	private void OnPlayerEnterEnd(Team t, EndType type, int frame)
	{
		BattleEndData battleEndData = new BattleEndData();
		battleEndData.team = t.team;
		battleEndData.userName = t.playerData.name;
		battleEndData.userId = t.playerData.userId;
		battleEndData.destroy = t.destory;
		battleEndData.survive = t.current;
		battleEndData.endType = type;
		battleEndData.endFrame = frame;
		if (type == EndType.ET_Dead || type == EndType.ET_Giveup)
		{
			t.isEnd = true;
		}
		bool flag = false;
		for (int i = 0; i < this.battleEndDatas.Count; i++)
		{
			if (this.battleEndDatas[i].userId == battleEndData.userId)
			{
				flag = true;
				if (battleEndData.endType == EndType.ET_Win)
				{
					this.battleEndDatas[i].endType = battleEndData.endType;
				}
				else if (battleEndData.endType == EndType.ET_Giveup && this.battleEndDatas[i].endType == EndType.ET_Dead)
				{
					this.battleEndDatas[i].endType = battleEndData.endType;
				}
				break;
			}
		}
		if (!flag)
		{
			this.battleEndDatas.Add(battleEndData);
		}
	}

	public void OnPlayerDiedOrGiveUp(EndType type, TEAM team, int frame)
	{
		Team team2 = this.battleSystem.sceneManager.teamManager.GetTeam(team);
		bool flag = false;
		for (int i = 0; i < this.battleEndDatas.Count; i++)
		{
			BattleEndData battleEndData = this.battleEndDatas[i];
			if (battleEndData.team == team && battleEndData.endType == type)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format("PlayerDiedOrGiveUp  Name:{0} Team:{1} type:{2}  frame:{3}", new object[]
			{
				team2.playerData.name,
				team,
				type,
				frame
			}), new object[0]);
			return;
		}
		this.OnPlayerEnterEnd(team2, type, frame);
		if (type == EndType.ET_Giveup)
		{
			team2.aiEnable = true;
			Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("玩家:{0} 投降，使用AI替代", team2.playerData.name), new object[0]);
		}
		if (type == EndType.ET_Dead)
		{
			this.diedTeamCount++;
		}
		MapConfig currentMapConfig = this.GetCurrentMapConfig();
		Team team3 = this.battleSystem.sceneManager.teamManager.GetTeam(this.battleData.currentTeam);
		// allAlive 关卡要求所有玩家必须存活，队友死亡或投降都会触发合作失败
		if (this.IsCoopVsPC() && currentMapConfig != null && currentMapConfig.allAlive == 1 && team3 != null && team3.Valid() && team2 != null && team3.IsFriend(team2.groupID))
		{
			TEAM team4 = TEAM.Neutral;
			for (int j = 0; j < LocalPlayer.MaxTeamNum; j++)
			{
				Team team5 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)j);
				if (team5 != null && team5.Valid() && !team3.IsFriend(team5.groupID))
				{
					team4 = team5.team;
					break;
				}
			}
			for (int k = 0; k < LocalPlayer.MaxTeamNum; k++)
			{
				Team team6 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)k);
				if (team6 != null && team6.Valid() && !team6.isEnd)
				{
					if (team3.IsFriend(team6.groupID))
					{
						this.OnPlayerEnterEnd(team6, EndType.ET_Dead, frame);
					}
					else
					{
						this.OnPlayerEnterEnd(team6, EndType.ET_Win, frame);
						if (team4 == TEAM.Neutral)
						{
							team4 = team6.team;
						}
					}
				}
			}
			this.battleData.winTEAM = team4;
			this.QuitBattle(true);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PlayerDead, new object[]
			{
				team
			});
			return;
		}
		int num = 1;
		int l = 0;
		while (l < LocalPlayer.MaxTeamNum)
		{
			Team team7 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)l);
			if (team7 != null && team7.Valid() && !team7.isEnd)
			{
				for (int m = l + 1; m < LocalPlayer.MaxTeamNum; m++)
				{
					Team team8 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)m);
					if (team8 != null && team8.Valid() && !team8.isEnd && !team7.IsFriend(team8.groupID))
					{
						num++;
						break;
					}
				}
				if (num == 1)
				{
					Debug.Log(string.Concat(new object[]
					{
						"加入最后一个队伍:",
						team7.team,
						"    name:",
						team7.playerData.name
					}));
					this.OnPlayerEnterEnd(team7, EndType.ET_Win, frame);
					this.battleData.winTEAM = team7.team;
					for (int n = 0; n < LocalPlayer.MaxTeamNum; n++)
					{
						Team team9 = this.battleSystem.sceneManager.teamManager.GetTeam((TEAM)n);
						if (team9 != null && team9.Valid() && team7.IsFriend(team9.groupID) && (!team9.aiEnable || team9.playerData.userId < 0) && team7.team != team9.team)
						{
							this.OnPlayerEnterEnd(team9, EndType.ET_Win, frame);
						}
					}
					break;
				}
				break;
			}
			else
			{
				l++;
			}
		}
		if (num == 1)
		{
			this.QuitBattle(true);
		}
		else if (team == this.battleData.currentTeam)
		{
			if (type == EndType.ET_Giveup)
			{
				this.QuitBattle(false);
			}
			else
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState = GameState.GameWatch;
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSelfDied, new object[0]);
			}
		}
		if (type == EndType.ET_Giveup)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PlayerGiveUp, new object[]
			{
				team
			});
			return;
		}
		if (type == EndType.ET_Dead)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PlayerDead, new object[]
			{
				team
			});
		}
	}

	private void SendGiveUpFrame()
	{
        byte[] content = Json.EnCodeBytes(new FramePacket
		{
			type = 2,
			giveup = new GiveUpPacket
			{
				team = this.battleData.currentTeam
			}
		});
        PbFrame pbFrame = new PbFrame();
		CSFrame csframe = new CSFrame();
		pbFrame.content = content;
		csframe.frame = pbFrame;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe, false);
	}

	public void QuitBattle(bool finish = false)
	{
		this.battleSystem.StopLockStep();
		if (Solarmax.Singleton<UISystem>.Instance.IsWindowVisible("ResumingWindow"))
		{
			Solarmax.Singleton<UISystem>.Instance.HideWindow("ResumingWindow");
		}
		if (finish)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleEndWindow");
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.winTEAM);
			Color color = team.color;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.ShowWinEffect(team, color);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFinishedColor, new object[]
			{
				team.color,
				team
			});
			if (!this.battleData.isReplay)
			{
				if (team.groupID != -1)
				{
					List<Team> friendTeam = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetFriendTeam(this.battleData.winTEAM);
					foreach (Team team2 in friendTeam)
					{
						Solarmax.Singleton<TaskModel>.Get().WinTeam(team2);
					}
				}
				else
				{
					Solarmax.Singleton<TaskModel>.Get().WinTeam(team);
				}
			}
		}
		else if (this.battleData.gameState != GameState.Watcher)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleEndWindow");
		}
		if (this.battleData.isReplay)
		{
			this.battleSystem.replayManager.NotifyPlayRecordEnd();
			this.battleSystem.replayManager.PlayRecordOver();
		}
		else
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFinishedBattle, new object[0]);
			this.battleSystem.battleData.gameState = GameState.GameEnd;
			CSQuitBattle csquitBattle = new CSQuitBattle();
			for (int i = 0; i < this.battleEndDatas.Count; i++)
			{
				BattleEndData battleEndData = this.battleEndDatas[i];
				EndEvent endEvent = new EndEvent();
				endEvent.userid = battleEndData.userId;
				endEvent.end_type = battleEndData.endType;
				endEvent.end_frame = battleEndData.endFrame;
				endEvent.end_destroy = battleEndData.destroy;
				endEvent.end_survive = battleEndData.survive;
				csquitBattle.events.Add(endEvent);
				Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(battleEndData.team);
				if (!this.battleData.isReplay && team3.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
				{
					Solarmax.Singleton<TaskModel>.Get().PvpEndDataHandler(battleEndData);
				}
			}
			Solarmax.Singleton<NetSystem>.Instance.Send<CSQuitBattle>(90, csquitBattle, false);
		}
	}

	public void UpdateResuming(int frame, float interval)
	{
		if (this.battleData.silent && this.battleSystem.lockStep.messageCount < 5)
		{
			this.battleData.resumingFrame = -1;
			this.battleSystem.sceneManager.SilentMode(false);
			Solarmax.Singleton<UISystem>.Instance.HideWindow("ResumingWindow");
		}
	}

	private void UpdateBattleSpeed()
	{
		if (this.speedUp)
		{
			return;
		}
		int num = Mathf.RoundToInt(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime());
		int num2 = 120;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
		{
			MapConfig currentMapConfig = this.GetCurrentMapConfig();
			if (currentMapConfig != null)
			{
				if (currentMapConfig.pvpAccelerateEnable == 0)
				{
					this.speedUp = true;
					return;
				}
				if (currentMapConfig.pvpAccelerateTimeMin == 0)
				{
					this.speedUp = true;
					return;
				}
				if (currentMapConfig.pvpAccelerateTimeMin > 0)
				{
					num2 = currentMapConfig.pvpAccelerateTimeMin * 60;
				}
			}
		}
		if (this.IsCoopVsPC())
		{
			MapConfig currentMapConfig = this.GetCurrentMapConfig();
			if (currentMapConfig != null && currentMapConfig.coopAccelerateEnable == 0)
			{
				this.speedUp = true;
				return;
			}
			num2 = 300;
			if (currentMapConfig != null)
			{
				if (currentMapConfig.coopAccelerateTimeMin == 0)
				{
					this.speedUp = true;
					return;
				}
				if (currentMapConfig.coopAccelerateTimeMin > 0)
				{
					num2 = currentMapConfig.coopAccelerateTimeMin * 60;
				}
			}
		}
		if (num >= num2)
		{
			this.speedUp = true;
			byte[] content = Json.EnCodeBytes(new FramePacket
			{
				type = 3
			});
			PbFrame pbFrame = new PbFrame();
			CSFrame csframe = new CSFrame();
			pbFrame.content = content;
			csframe.frame = pbFrame;
			Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe, false);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnPVPBattleAccelerate, null);
		}
	}

	private bool IsCoopVsPC()
	{
		CooperationType battleSubType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
		return battleSubType == CooperationType.CT_4vPC || battleSubType == CooperationType.CT_3vPC || battleSubType == CooperationType.CT_2vPC;
	}

	private MapConfig GetCurrentMapConfig()
	{
		return Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId.ToString());
	}

	private int GetBattleTimeMaxSec()
	{
		int num = int.Parse(Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(4)) * 60;
		if (this.IsCoopVsPC())
		{
			MapConfig currentMapConfig = this.GetCurrentMapConfig();
			if (currentMapConfig != null && currentMapConfig.coopTimeLimitMin > 0)
			{
				num = currentMapConfig.coopTimeLimitMin * 60;
			}
		}
		if (this.battleData.battleSubType == CooperationType.CT_3v1)
		{
			MapConfig currentMapConfig2 = this.GetCurrentMapConfig();
			if (currentMapConfig2 != null && currentMapConfig2.pvpTimeLimitMin > 0)
			{
				num = currentMapConfig2.pvpTimeLimitMin * 60;
			}
		}
		if (this.battleData.winType == "alive" || this.battleData.winType == "surival" || this.battleData.winType == "survival")
		{
			float num2;
			if (float.TryParse(this.battleData.winTypeParam1, out num2) && num2 > 0f)
			{
				int num3 = Mathf.CeilToInt(num2);
				if (num < num3)
				{
					num = num3;
				}
			}
		}
		return num;
	}

	/// <summary>
	/// 地图切换后刷新时间上限（合作模式按当前 mapconfig 重新计算，避免沿用第一张地图的时限）
	/// </summary>
	public void RefreshBattleTimeMax()
	{
		this.battleTimeMax = this.GetBattleTimeMaxSec();
	}

	/// <summary>
	/// 地图切换后重置加速状态（加速会翻倍所有队伍的移动/生产/占领速度，切图后需恢复原速并按新地图的 CoopAccelerateTimeMin 重新计时）
	/// </summary>
	public void ResetBattleSpeed()
	{
		if (!this.speedUp)
		{
			return;
		}
		this.speedUp = false;
		SceneManager sceneManager = Solarmax.Singleton<BattleSystem>.Instance.sceneManager;
		sceneManager.teamManager.SetAllTeamMoveSpeed(-1f);
		sceneManager.teamManager.SetAllTeamProduceSpeed(-1f);
		sceneManager.teamManager.SetAllOccupySpeed(-1f);
	}

	private void UpdateBombPlanet()
	{
		int num = Mathf.RoundToInt(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime());
		if (num < 240)
		{
			return;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
		{
			return;
		}
		List<Node> usefulNodeList = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetUsefulNodeList();
		if (!this.bSelectCenterPos)
		{
			this.bSelectCenterPos = true;
			int count = usefulNodeList.Count;
			int index = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.Range(0, count);
			this.centerPlanet = usefulNodeList[index].tag;
			this.center = usefulNodeList[index].GetPosition();
			if (string.IsNullOrEmpty(this.centerPlanet))
			{
				return;
			}
		}
		if (!this.hasTriggerClearBarrier)
		{
			this.hasTriggerClearBarrier = true;
			byte[] content = Json.EnCodeBytes(new FramePacket
			{
				type = 13
			});
			PbFrame pbFrame = new PbFrame();
			CSFrame csframe = new CSFrame();
			pbFrame.content = content;
			csframe.frame = pbFrame;
			Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe, false);
		}
		if (usefulNodeList.Count <= 1)
		{
			return;
		}
		if (!this.hasSentBoomEvent)
		{
			this.hasSentBoomEvent = true;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnPVPBattleBoom, null);
		}
		if (this.lastElapsed == 0 || num - this.lastElapsed >= 8)
		{
			this.lastElapsed = num;
			if (this.bombTarget == null)
			{
				float[] array = new float[3];
				array[0] = (array[1] = (array[2] = 0f));
				string[] array2 = new string[3];
				foreach (Node node in usefulNodeList)
				{
					float magnitude = (node.GetPosition() - this.center).magnitude;
					if (array[0] == 0f)
					{
						array[0] = magnitude;
						array2[0] = node.tag;
					}
					else if (magnitude >= array[0])
					{
						array[2] = array[1];
						array2[2] = array2[1];
						array[1] = array[0];
						array2[1] = array2[0];
						array[0] = magnitude;
						array2[0] = node.tag;
					}
					else if (array[1] == 0f)
					{
						array[1] = magnitude;
						array2[1] = node.tag;
					}
					else if (magnitude >= array[1])
					{
						array[2] = array[1];
						array2[2] = array2[1];
						array[1] = magnitude;
						array2[1] = node.tag;
					}
					else if (array[2] == 0f || magnitude > array[2])
					{
						array[2] = magnitude;
						array2[2] = node.tag;
					}
				}
				if (array[0] == 0f && array[1] == 0f && array[2] == 0f)
				{
					return;
				}
				int num2 = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.Range(0, 2);
				Debug.Log("RandomSprite8");
				if (array[num2] == 0f)
				{
					if (array[(num2 + 1) % 3] == 0f)
					{
						if (array[(num2 + 2) % 3] == 0f)
						{
							return;
						}
						num2 = (num2 + 2) % 3;
					}
					else
					{
						num2 = (num2 + 1) % 3;
					}
				}
				float num3 = array[num2];
				this.bombTarget = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(array2[num2]);
				if (this.bombTarget != null)
				{
					byte[] content2 = Json.EnCodeBytes(new FramePacket
					{
						type = 16,
						effect = new DriftEffect
						{
							tag = this.bombTarget.tag,
							effect = "Eff_XJ_Djs",
							time = 10f
						}
					});
					PbFrame pbFrame2 = new PbFrame();
					CSFrame csframe2 = new CSFrame();
					pbFrame2.content = content2;
					csframe2.frame = pbFrame2;
					Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe2, false);
					this.narrowtimes = 0;
				}
			}
		}
		if (this.narrowtimes >= 0 && this.bombTarget != null)
		{
			if (this.narrowtimes == 150)
			{
				byte[] content3 = Json.EnCodeBytes(new FramePacket
				{
					type = 6,
					effect = new DriftEffect
					{
						tag = this.bombTarget.tag,
						effect = "EFF_XJ_Boom_1",
						time = 6f,
						scale = this.bombTarget.GetWidth() / 0.2f
					}
				});
				PbFrame pbFrame3 = new PbFrame();
				CSFrame csframe3 = new CSFrame();
				pbFrame3.content = content3;
				csframe3.frame = pbFrame3;
				Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe3, false);
			}
			if (this.bombTarget.GetWidth() > 0.02f)
			{
			}
			this.narrowtimes++;
			if (this.narrowtimes >= 222)
			{
				byte[] content4 = Json.EnCodeBytes(new FramePacket
				{
					type = 4,
					bomb = new PlanetBomb
					{
						tag = this.bombTarget.tag
					}
				});
				PbFrame pbFrame4 = new PbFrame();
				CSFrame csframe4 = new CSFrame();
				pbFrame4.content = content4;
				csframe4.frame = pbFrame4;
				Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe4, false);
				this.bombTarget = null;
				this.narrowtimes = -2;
			}
		}
	}

	public SceneManager sceneManager;

	private BattleData battleData;

	private BattleSystem battleSystem;

	private int tickEndCount;
	private int tickEndOtherCount;

	private List<BattleEndData> battleEndDatas = new List<BattleEndData>();

	private int diedTeamCount;

	private int battleTimeMax;

	private int survivorTimeMax;

	private bool speedUp;

	private Vector3 center = Vector3.zero;

	private string centerPlanet = string.Empty;

	private Node bombTarget;

	private int lastElapsed;

	private int narrowtimes = -2;

	private bool bSelectCenterPos;

	private bool hasSentBoomEvent;

	private bool hasTriggerClearBarrier;

	private bool isResumingBattle;

	private bool coopSurvivalWinFired;
}
