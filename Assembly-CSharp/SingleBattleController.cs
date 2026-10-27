using System;
using System.Collections.Generic;
using NetMessage;
using Plugin;
using Solarmax;
using UnityEngine;

public class SingleBattleController : IBattleController, Lifecycle2
{
	public SingleBattleController(BattleData bd, BattleSystem bs)
	{
		this.battleData = bd;
		this.battleSystem = bs;
	}

	public bool Init()
	{
		this.tickEndCount = 0;
		List<global::Packet> list = new List<global::Packet>();
		this.emptyPackets = list.ToArray();
		this.emptyPacketId = 0;
		this.battleSystem.battleData.battleType = BattlePlayType.Normalize;
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Instance.lockStep;
		int num = this.emptyPacketId + 1;
		this.emptyPacketId = num;
		int frame = num;
		object[] array = this.emptyPackets;
		object[] msgList = array;
		lockStep.AddFrame(frame, msgList);
		int num2 = 0;
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
			team.groupID = i;
			team.teamFriend[i] = true;
			if (team != null && team.Valid())
			{
				num2++;
			}
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.currentTeam != null)
		{
			this.battleData.currentTeam = Solarmax.Singleton<LocalPlayer>.Get().playerData.currentTeam.team;
		}
		this.useCommonEndCondition = (num2 > 1);
		return true;
	}

	public void Tick(int frame, float interval)
	{
		if (this.useCommonEndCondition)
		{
			this.UpdateEnd(frame, interval);
		}
		else
		{
			this.UpdateEndAllOccupied(frame, interval);
		}
		this.UpdateEndOtherWinLoseType(frame, interval);
		if (Solarmax.Singleton<BattleSystem>.Instance.lockStep.messageCount < 2)
		{
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.AddFrame(++this.emptyPacketId, this.emptyPackets);
		}
	}

	public void Destroy()
	{
	}

	public void Reset()
	{
		this.tickEndCount = 0;
		this.tickEndOtherCount = 0;
		this.battleEndDatas.Clear();
	}

	public void OnRecievedFramePacket(SCFrame frame)
	{
		List<global::Packet> list = new List<global::Packet>();
		for (int i = 0; i < frame.users.Count; i++)
		{
			int num = frame.users[i];
			PbFrames pbFrames = frame.frames[i];
			if (pbFrames != null && pbFrames.frames.Count != 0)
			{
				for (int j = 0; j < pbFrames.frames.Count; j++)
				{
					PbFrame pbFrame = pbFrames.frames[j];
					if (pbFrame != null)
					{
						list.Add(new global::Packet
						{
							team = (TEAM)(num + 1),
							packet = Json.DeCode<FramePacket>(pbFrame.content)
						});
					}
				}
			}
		}
		this.battleSystem.lockStep.AddFrame(frame.frameNum, list.ToArray());
	}

	public void OnRecievedScriptFrame(PbSCFrames frame)
	{
	}

	public void OnRunFramePacket(FrameNode frameNode)
	{
		this.battleSystem.sceneManager.RunFramePacket(frameNode);
	}

	public void OnPlayerMove(Node from, Node to)
	{
		int shipFactCount = from.GetShipFactCount((int)Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		if (shipFactCount == 0)
		{
			return;
		}
		FramePacket framePacket = new FramePacket();
		framePacket.type = 0;
		framePacket.move = new MovePacket();
		framePacket.move.from = from.tag;
		framePacket.move.to = to.tag;
		if (Solarmax.Singleton<LocalSettingStorage>.Get().fightOption == 1)
		{
			int num = (int)Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber;
			framePacket.move.optype = 1;
			if (num > 0 && shipFactCount <= num)
			{
				num = shipFactCount;
			}
			if (num == -1)
			{
				num = shipFactCount;
			}
			framePacket.move.rate = (float)num;
			from.nodeManager.MoveTo(from, to, this.battleData.currentTeam, 0f, num);
		}
		else
		{
			framePacket.move.optype = 0;
			framePacket.move.rate = Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderRate;
			from.nodeManager.MoveTo(from, to, this.battleData.currentTeam, framePacket.move.rate, 0);
		}
		byte[] content = Json.EnCodeBytes(framePacket);
		PbFrame pbFrame = new PbFrame();
		pbFrame.content = content;
		ReplayCollectManager.Get().AddReplayFrame(pbFrame, -1);
	}

	public void PlayerGiveUp()
	{
		this.QuitBattle(false);
	}

	public void OnPlayerGiveUp(TEAM giveUpTeam)
	{
	}

	public void OnPlayerDirectQuit(TEAM team)
	{
	}

	private void UpdateEnd(int frame, float dt)
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
					Solarmax.Singleton<LoggerSystem>.Instance.Info("SingleBattleController   UpdateEnd  ---> OnPlayerDiedOrGiveUp", new object[0]);
					this.OnPlayerDiedOrGiveUp(EndType.ET_Dead, team.team, frame);
				}
			}
			if (this.battleEndDatas.Count != this.battleData.currentPlayers)
			{
				this.tickEndCount = 0;
			}
		}
	}

	private void OnPlayerEnterEnd(Team t, EndType type, int frame)
	{
		BattleEndData battleEndData = new BattleEndData();
		battleEndData.team = t.team;
		battleEndData.userId = t.playerData.userId;
		battleEndData.destroy = t.destory;
		battleEndData.endType = type;
		battleEndData.endFrame = frame;
		t.isEnd = true;
		this.battleEndDatas.Add(battleEndData);
	}

	public void OnPlayerDiedOrGiveUp(EndType type, TEAM team, int frame)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("SingleBattleController   OnPlayerDiedOrGiveUp", new object[0]);
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
		int[] array = new int[LocalPlayer.MaxTeamNum];
		for (int j = 0; j < LocalPlayer.MaxTeamNum; j++)
		{
			Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)j);
			if (team3 != null && team3.Valid() && !team3.isEnd)
			{
				for (int k = 0; k < LocalPlayer.MaxTeamNum; k++)
				{
					Team team4 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)k);
					if (team4 != null && team4.Valid() && !team4.isEnd && !team3.teamFriend[team4.groupID])
					{
						array[j]++;
					}
				}
				if (array[j] == 0)
				{
					this.battleData.winTEAM = team3.team;
					if (team3.teamFriend[(int)this.battleData.currentTeam])
					{
						this.battleData.winTEAM = this.battleData.currentTeam;
					}
					this.QuitBattle(true);
				}
			}
		}
		if (this.battleEndDatas.Count == this.battleData.currentPlayers - 1)
		{
			for (int l = 0; l < LocalPlayer.MaxTeamNum; l++)
			{
				Team team5 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)l);
				if (team5 != null && team5.Valid() && !team5.isEnd)
				{
					this.OnPlayerEnterEnd(team5, EndType.ET_Win, frame);
					this.battleData.winTEAM = team5.team;
					break;
				}
			}
		}
		if (this.battleEndDatas.Count == this.battleData.currentPlayers || Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam == team)
		{
			this.QuitBattle(true);
		}
	}

	public void QuitBattle(bool finish = false)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("SingleBattleController  QuitBattle", new object[0]);
		if (this.battleData.winTEAM == this.battleData.currentTeam)
		{
			Solarmax.Singleton<AchievementManager>.Get().SettlementAchievement(finish);
		}
		if (finish && this.battleData.battleType == BattlePlayType.Normalize && Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam == this.battleData.winTEAM)
		{
			ReplayCollectManager.Get().EndReplayCollect(1);
		}
		Solarmax.Singleton<BattleSystem>.Instance.StopLockStep();
		if (finish)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.winTEAM);
			Color color = team.color;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.ShowWinEffect(team, color);
			Debug.Log(string.Concat(new object[]
			{
				"win team hitships: ",
				team.hitships,
				" destroy :  ",
				team.destory
			}));
			Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleEndWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFinished, new object[0]);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFinishedColor, new object[]
			{
				color,
				team
			});
			if (this.battleData.winTEAM != this.battleData.currentTeam)
			{
				Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
				int destory = team2.destory;
				int hitships = team2.hitships;
				int produces = team2.produces;
				float battleTime = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
				MonoSingleton<FlurryAnalytis>.Instance.FlurryBattleEndEvent(this.battleData.matchId, "2", "0", "0", hitships.ToString(), destory.ToString(), battleTime.ToString());
			}
			Solarmax.Singleton<TaskModel>.Get().FinishTaskEvent(FinishConntion.Pve, 1);
			GuideManager.TriggerGuidecompleted(GuildEndEvent.complteBattle);
		}
		else
		{
			Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
			int destory2 = team3.destory;
			int hitships2 = team3.hitships;
			int produces2 = team3.produces;
			float battleTime2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
			MonoSingleton<FlurryAnalytis>.Instance.FlurryBattleEndEvent(this.battleData.matchId, "1", "0", "0", hitships2.ToString(), destory2.ToString(), battleTime2.ToString());
			MiGameAnalytics.MiAnalyticsBattleEndEvent(this.battleData.matchId, "1", "0", "0", hitships2.ToString(), destory2.ToString(), battleTime2.ToString());
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			Solarmax.Singleton<BattleSystem>.Instance.BeginFadeOut();
			Solarmax.Singleton<ShipFadeManager>.Get().SetFadeType(ShipFadeManager.FADETYPE.OUT, 0.1f);
			Solarmax.Singleton<UISystem>.Get().FadeBattle(false, new EventDelegate(delegate()
			{
				if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.SingleLevel || Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.GuildeLevel || Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.Single)
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow("LobbyWindowView");
					if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.GuildeLevel)
					{
						Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnGuiledEndStartGame, new object[0]);
					}
					Solarmax.Singleton<BattleSystem>.Instance.Reset();
				}
				if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.PayLevel)
				{
					Solarmax.Singleton<BattleSystem>.Instance.Reset();
					Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("ChapterWindow", EventId.UpdateChapterWindow, new object[]
					{
						2
					}));
				}
			}));
			RapidBlurEffect behavior = Camera.main.GetComponent<RapidBlurEffect>();
			if (behavior != null)
			{
				behavior.enabled = true;
				behavior.MainBgScale(true, 4.5f, 0.035f);
				global::Coroutine.DelayDo(0.55f, new EventDelegate(delegate()
				{
					behavior.enabled = false;
				}));
			}
		}
		GuideManager.ClearGuideData();
		if (finish)
		{
			AppsFlyerTool.FlyerPveBattleEndEvent("1");
			return;
		}
		AppsFlyerTool.FlyerPveBattleEndEvent("0");
	}

	public void Retry()
	{
		Solarmax.Singleton<BattleSystem>.Instance.StopLockStep();
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		int destory = team.destory;
		int hitships = team.hitships;
		int produces = team.produces;
		float battleTime = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
		MonoSingleton<FlurryAnalytis>.Instance.FlurryBattleEndEvent(this.battleData.matchId, "1", "0", "0", hitships.ToString(), destory.ToString(), battleTime.ToString());
		MiGameAnalytics.MiAnalyticsBattleEndEvent(this.battleData.matchId, "1", "0", "0", hitships.ToString(), destory.ToString(), battleTime.ToString());
		GuideManager.ClearGuideData();
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(this.battleData.matchId, GameType.SingleLevel, false);
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.FadePlanet(true, 0f);
		Solarmax.Singleton<ShipFadeManager>.Get().SetFadeType(ShipFadeManager.FADETYPE.IN, 0f);
		Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
	}

	private void UpdateEndAllOccupied(int frame, float interval)
	{
		// 地图切换期间不检查胜利条件
		if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.isFading)
		{
			return;
		}

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

	private void UpdateEndOtherWinLoseType(int frame, float interval)
	{
		// 地图切换期间不检查胜利条件
		if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.isFading)
		{
			return;
		}

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
					string[] array = text.Split(new char[] { ',' });
					int num2 = (int)this.battleData.currentTeam;
					Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam);
					bool flag2 = false;
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string text2 = array2[i].Trim();
						if (!string.IsNullOrEmpty(text2))
						{
							bool flag3 = false;
							if (currentTable != null && currentTable.mbcList != null)
							{
								foreach (MapBuildingConfig mapBuildingConfig in currentTable.mbcList)
								{
									if (mapBuildingConfig.tag == text2)
									{
										if (mapBuildingConfig.camption == num2)
										{
											flag3 = true;
										}
										else if (team != null)
										{
											Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)mapBuildingConfig.camption);
											if (team3 != null && team.IsFriend(team3.groupID))
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
							if (node != null && (int)node.team != num2 && (team == null || !node.currentTeam.IsFriend(team.groupID)))
							{
								flag2 = true;
								break;
							}
						}
					}
					if (flag2)
					{
						for (int j = 0; j < LocalPlayer.MaxTeamNum; j++)
						{
							if (j != num2)
							{
								Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)j);
								if (team2 != null && team2.Valid())
								{
									this.battleData.winTEAM = (TEAM)j;
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
				this.battleData.winTEAM = this.battleData.currentTeam;
				this.QuitBattle(true);
				return;
			}
			this.tickEndOtherCount = 0;
		}
	}

	private BattleSystem battleSystem;

	public SceneManager sceneManager;

	public BattleData battleData;

	private int tickEndCount;
	private int tickEndOtherCount;

	private List<BattleEndData> battleEndDatas = new List<BattleEndData>();

	private global::Packet[] emptyPackets;

	private int emptyPacketId;

	private bool useCommonEndCondition = true;
}
