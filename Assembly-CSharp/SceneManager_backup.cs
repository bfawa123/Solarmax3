using System;
using System.Collections;
using System.Collections.Generic;
using Plugin;
using Solarmax;
using UnityEngine;

public class SceneManager : Lifecycle2
{
	public SceneManager(BattleData bd)
	{
		this.battleData = bd;
		this.teamManager = new TeamManager(this);
		this.nodeManager = new NodeManager(this);
		this.shipManager = new ShipManager(this);
		this.aiManager = new AIManager(this);
		this.warpManager = new WarpManager();
		this.newSkillManager = new NewSkillManager(this);
	}

	public WarpManager warpManager { get; set; }

	public TeamManager teamManager { get; set; }

	public NodeManager nodeManager { get; set; }

	public ShipManager shipManager { get; set; }

	public AIManager aiManager { get; set; }

	public NewSkillManager newSkillManager { get; set; }

	public BattleData battleData { get; set; }

	public bool Init()
	{
		if (this.teamManager.Init())
		{
			if (this.nodeManager.Init())
			{
				if (this.shipManager.Init())
				{
					if (this.aiManager.Init())
					{
						if (this.warpManager.Init())
						{
							if (!this.newSkillManager.Init())
							{
								this.battleScaleSpeed = this.produceRates[this.curProcduceIndex].produceRate;
							}
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public void Tick(int frame, float interval)
	{
		this.teamManager.Tick(frame, interval);
		this.nodeManager.Tick(frame, interval);
		this.shipManager.Tick(frame, interval);
		this.aiManager.Tick(frame, interval);
		this.warpManager.Tick(frame, interval);
		this.newSkillManager.Tick(frame, interval);
		this.m_BattleTime += interval;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.PVP && this.curProcduceIndex < this.produceRates.Count - 1 && this.m_BattleTime > this.produceRates[this.curProcduceIndex + 1].startTime)
		{
			this.curProcduceIndex++;
			this.battleScaleSpeed = this.produceRates[this.curProcduceIndex].produceRate;
		}
		this.TickBlackStrike(frame);
		this.TickAssimilation(frame);
		this.TickMystery(frame);
		this.TickSetParty(frame);
	}

        private bool IsSpaceCore(Node node)
        {
                return !(node == null) && node.perfab == "Entity_Dilator";
        }

        private bool HasProtectedObjectTag(string raw, string tag)
        {
                if (string.IsNullOrEmpty(raw) || string.IsNullOrEmpty(tag))
                {
                        return false;
                }
                string[] array = raw.Split(new char[]
                {
                        ','
                }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length; i++)
                {
                        if (string.Equals(array[i].Trim(), tag, StringComparison.OrdinalIgnoreCase))
                        {
                                return true;
                        }
                }
                return false;
        }

        public bool IsProtectedObjectBySpaceCore(Node node)
        {
                if (node == null || node.currentTeam == null || node.team == TEAM.Neutral)
                {
                        return false;
                }
                for (int i = 0; i < this.nodes.Count; i++)
                {
                        Node node2 = this.nodes[i];
                        if (!(node2 == null) && node2 != node && this.IsSpaceCore(node2) && node2.currentTeam != null && node2.team == node.team && this.HasProtectedObjectTag(node2.spaceCoreProtectedObjectTags, node.tag))
                        {
                                return true;
                        }
                }
                return false;
        }

	private void TickAssimilation(int frame)
	{
		if (frame < this.assimilationLastFrame)
		{
			this.assimilationFired = false;
			this.assimilationInit = false;
			this.assimilationRemaining = 0;
			this.assimilationNextTime = 0f;
		}
		this.assimilationLastFrame = frame;
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
		if (data == null || !data.IfAssimilation())
		{
			return;
		}
		if (!this.assimilationInit)
		{
			this.assimilationInit = true;
			this.assimilationRemaining = ((data.assimilationRepeatCount > 0) ? data.assimilationRepeatCount : 1);
			this.assimilationNextTime = ((data.assimilationTriggerTime > 0f) ? data.assimilationTriggerTime : 0f);
		}
		if (this.assimilationFired || this.assimilationRemaining <= 0 || this.m_BattleTime < this.assimilationNextTime)
		{
			return;
		}
		Team team = this.teamManager.GetTeam((TEAM)data.assimilationCamp);
		if (team == null)
		{
			return;
		}
		TEAM teamId = team.team;
		if (!team.aiEnable && team.team != this.battleData.currentTeam)
		{
			team.aiEnable = true;
			team.playerData.userId = -10000 - data.assimilationCamp;
			team.playerData.name = AIManager.GetAIName(team.playerData.userId);
			team.playerData.icon = AIManager.GetAIIcon(team.playerData.userId);
			int aiStrategy = -1;
			if (data.teamAITypes != string.Empty)
			{
				aiStrategy = data.teamAiType[data.assimilationCamp];
			}
			if (aiStrategy < 0)
			{
				aiStrategy = this.battleData.aiStrategy;
			}
			if (aiStrategy < 0)
			{
				aiStrategy = 3;
			}
			this.aiManager.AddAI(team, aiStrategy, team.playerData.level, this.battleData.aiLevel, 1f);
		}
		float num = data.assimilationRadius * data.assimilationRadius;
		Vector3 vector = new Vector3(data.assimilationX, data.assimilationY, 0f);
		this.ApplyAssimilationAt(vector, num, team, teamId);
		if (data.assimilationHasSecondPoint)
		{
			Vector3 vector2 = new Vector3(data.assimilationX2, data.assimilationY2, 0f);
			this.ApplyAssimilationAt(vector2, num, team, teamId);
		}
		this.assimilationRemaining--;
		this.assimilationNextTime = this.m_BattleTime + 1f;
		if (this.assimilationRemaining <= 0)
		{
			this.assimilationFired = true;
		}
	}

	private void TickMystery(int frame)
	{
		if (frame < this.mysteryLastFrame)
		{
			this.EndMystery();
			this.mysteryInit = false;
			this.mysteryTriggered = false;
			this.mysteryLastFrame = 0;
		}
		this.mysteryLastFrame = frame;
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
		if (data == null || !data.IfMystery())
		{
			this.EndMystery();
			this.mysteryTriggered = false;
			return;
		}
		if (!this.mysteryInit)
		{
			this.mysteryInit = true;
			this.mysteryCamp = data.mysteryCamp;
			this.mysteryStartTime = data.mysteryStartTime;
			this.mysteryDuration = data.mysteryDuration;
			this.mysteryEndTime = 0f;
		}
		if (!this.mysteryActive)
		{
			if (this.mysteryTriggered)
			{
				return;
			}
			if (this.m_BattleTime < this.mysteryStartTime || this.mysteryDuration <= 0f)
			{
				return;
			}
			this.StartMystery();
		}
		if (!this.mysteryActive)
		{
			return;
		}
		if (this.mysteryEndTime > 0f && this.m_BattleTime >= this.mysteryEndTime)
		{
			this.EndMystery();
			return;
		}
		this.ApplyMysteryVisibility();
	}

	private void TickSetParty(int frame)
	{
		if (frame < this.setPartyLastFrame)
		{
			this.ResetSetParty(true);
		}
		this.setPartyLastFrame = frame;
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
		if (data == null || !data.IfSetParty())
		{
			this.ResetSetParty(true);
			return;
		}
		this.EnsureSetPartyStates(data);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < this.setPartyStates.Count; i++)
		{
			SceneManager.SetPartyRuntimeState setPartyRuntimeState = this.setPartyStates[i];
			MapSetPartyConfig config = setPartyRuntimeState.config;
			if (config == null || !config.IsValid())
			{
				continue;
			}
			if (!setPartyRuntimeState.active)
			{
				if (this.m_BattleTime >= setPartyRuntimeState.nextStartTime)
				{
					this.StartSetParty(setPartyRuntimeState);
					flag = true;
				}
			}
			else if (this.m_BattleTime >= setPartyRuntimeState.endTime)
			{
				this.EndSetParty(setPartyRuntimeState);
				flag2 = true;
			}
		}
		if (flag)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("狂欢时刻开始", new object[0]);
			Tips.Make(Tips.TipsType.FlowUp, "狂欢时刻开始", 2f);
		}
		if (flag2)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("狂欢时刻结束", new object[0]);
			Tips.Make(Tips.TipsType.FlowUp, "狂欢时刻结束", 2f);
		}
	}

	private void EnsureSetPartyStates(MapConfig config)
	{
		if (config == null)
		{
			this.ResetSetParty(false);
			return;
		}
		if (this.setPartyStates.Count == config.setPartyList.Count)
		{
			return;
		}
		this.ResetSetParty(false);
		for (int i = 0; i < config.setPartyList.Count; i++)
		{
			MapSetPartyConfig mapSetPartyConfig = config.setPartyList[i];
			if (mapSetPartyConfig != null && mapSetPartyConfig.IsValid())
			{
				SceneManager.SetPartyRuntimeState item = new SceneManager.SetPartyRuntimeState();
				item.config = mapSetPartyConfig;
				this.SyncSetPartyStateToBattleTime(item);
				this.setPartyStates.Add(item);
			}
		}
	}

	private void SyncSetPartyStateToBattleTime(SceneManager.SetPartyRuntimeState state)
	{
		if (state == null || state.config == null || !state.config.IsValid())
		{
			return;
		}
		state.active = false;
		state.endTime = 0f;
		state.nextStartTime = state.config.interval;
		float num = state.config.interval + state.config.continueTime;
		if (num <= 0f)
		{
			return;
		}
		if (this.m_BattleTime < state.config.interval)
		{
			return;
		}
		float num2 = this.m_BattleTime - state.config.interval;
		float num3 = num2 % num;
		float num4 = this.m_BattleTime - num3;
		if (num3 < state.config.continueTime)
		{
			state.active = true;
			state.endTime = num4 + state.config.continueTime;
			state.nextStartTime = num4 + num;
			for (int i = 0; i < state.config.camps.Count; i++)
			{
				Team team = this.teamManager.GetTeam((TEAM)state.config.camps[i]);
				if (team != null)
				{
					this.ApplySetPartyBuff(team, state.config, 1f);
				}
			}
			return;
		}
		state.nextStartTime = num4 + num;
	}

	private void StartSetParty(SceneManager.SetPartyRuntimeState state)
	{
		if (state == null || state.config == null || state.active)
		{
			return;
		}
		state.active = true;
		state.endTime = this.m_BattleTime + state.config.continueTime;
		for (int i = 0; i < state.config.camps.Count; i++)
		{
			Team team = this.teamManager.GetTeam((TEAM)state.config.camps[i]);
			if (team != null)
			{
				this.ApplySetPartyBuff(team, state.config, 1f);
				if (state.config.shipAdd > 0)
				{
					this.AddSetPartyShips(team, state.config.shipAdd);
				}
			}
		}
	}

	private void EndSetParty(SceneManager.SetPartyRuntimeState state)
	{
		if (state == null || state.config == null || !state.active)
		{
			return;
		}
		for (int i = 0; i < state.config.camps.Count; i++)
		{
			Team team = this.teamManager.GetTeam((TEAM)state.config.camps[i]);
			if (team != null)
			{
				this.ApplySetPartyBuff(team, state.config, -1f);
			}
		}
		state.active = false;
		state.endTime = 0f;
		state.nextStartTime = this.m_BattleTime + state.config.interval;
	}

	private void ApplySetPartyBuff(Team team, MapSetPartyConfig config, float sign)
	{
		if (team == null || config == null)
		{
			return;
		}
		float num = (config.speedUp - 1f) * sign;
		if (Mathf.Abs(num) > 0.0001f)
		{
			team.SetAttribute(TeamAttr.Speed, num, false);
		}
		float num2 = (config.produceUp - 1f) * sign;
		if (Mathf.Abs(num2) > 0.0001f)
		{
			team.SetAttribute(TeamAttr.ProduceSpeed, num2, false);
		}
		float num3 = (config.captureUp - 1f) * sign;
		if (Mathf.Abs(num3) > 0.0001f)
		{
			team.SetAttribute(TeamAttr.CapturedSpeed, num3, false);
		}
		float num4 = (config.occupyUp - 1f) * sign;
		if (Mathf.Abs(num4) > 0.0001f)
		{
			team.SetAttribute(TeamAttr.OccupiedSpeed, num4, false);
		}
	}

	private void AddSetPartyShips(Team team, int shipCount)
	{
		if (team == null || shipCount <= 0)
		{
			return;
		}
		List<Node> usefulNodeList = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (!(node == null) && node.type != NodeType.BarrierLine && node.team == team.team && node.currentTeam != null && node.currentTeam.team == team.team)
			{
				this.AddShip(node, shipCount, (int)team.team, true, true);
			}
		}
	}

	private void ResetSetParty(bool revertActiveBuff)
	{
		if (revertActiveBuff)
		{
			for (int i = 0; i < this.setPartyStates.Count; i++)
			{
				SceneManager.SetPartyRuntimeState setPartyRuntimeState = this.setPartyStates[i];
				if (setPartyRuntimeState != null && setPartyRuntimeState.active)
				{
					this.EndSetParty(setPartyRuntimeState);
				}
			}
		}
		this.setPartyStates.Clear();
		this.setPartyLastFrame = 0;
	}

	private void StartMystery()
	{
		Team team = this.teamManager.GetTeam((TEAM)this.mysteryCamp);
		if (team == null)
		{
			return;
		}
		this.mysteryTriggered = true;
		this.mysteryPrevHideFly = team.GetAttributeInt(TeamAttr.HideFly);
		this.mysteryPrevHidePop = team.GetAttributeInt(TeamAttr.HidePop);
		if (this.ShouldHideMysteryFromLocal(team))
		{
			team.SetAttribute(TeamAttr.HideFly, 1f, true);
			team.SetAttribute(TeamAttr.HidePop, 1f, true);
		}
		this.mysteryActive = true;
		this.mysteryEndTime = this.m_BattleTime + this.mysteryDuration;
		this.mysteryNodeForceHidePrev.Clear();
		this.ApplyMysteryVisibility();
	}

	private bool ShouldHideMysteryFromLocal(Team mysteryTeam)
	{
		if (mysteryTeam == null)
		{
			return false;
		}
		Team team = this.teamManager.GetTeam((TEAM)this.battleData.currentTeam);
		if (team == null)
		{
			return true;
		}
		if (team.team == mysteryTeam.team)
		{
			return false;
		}
		return !team.IsFriend(mysteryTeam.groupID);
	}

	private void ApplyMysteryVisibility()
	{
		Team team = this.teamManager.GetTeam((TEAM)this.mysteryCamp);
		if (team == null)
		{
			return;
		}
		if (!this.ShouldHideMysteryFromLocal(team))
		{
			foreach (KeyValuePair<string, bool> keyValuePair in this.mysteryNodeForceHidePrev)
			{
				Node node = this.nodeManager.GetNode(keyValuePair.Key);
				if (node != null)
				{
					node.forceHideNode = keyValuePair.Value;
				}
			}
			this.mysteryNodeForceHidePrev.Clear();
			return;
		}
		List<Node> usefulNodeList = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			bool flag = node != null && node.currentTeam != null && node.currentTeam.team == team.team;
			if (flag)
			{
				if (!this.mysteryNodeForceHidePrev.ContainsKey(node.tag))
				{
					this.mysteryNodeForceHidePrev[node.tag] = node.forceHideNode;
				}
				node.forceHideNode = true;
			}
			else if (node != null && this.mysteryNodeForceHidePrev.ContainsKey(node.tag))
			{
				node.forceHideNode = this.mysteryNodeForceHidePrev[node.tag];
				this.mysteryNodeForceHidePrev.Remove(node.tag);
			}
		}
		List<Ship> flyShip = this.shipManager.GetFlyShip(team.team);
		for (int j = 0; j < flyShip.Count; j++)
		{
			Ship ship = flyShip[j];
			EntityShip entityShip = ship.entity as EntityShip;
			if (entityShip != null)
			{
				entityShip.SetForceHide(true);
			}
			else if (ship.entity != null)
			{
				ship.entity.SetAlpha(0f);
			}
		}
	}

	private void EndMystery()
	{
		if (!this.mysteryActive)
		{
			this.mysteryNodeForceHidePrev.Clear();
			this.mysteryEndTime = 0f;
			return;
		}
		Team team = this.teamManager.GetTeam((TEAM)this.mysteryCamp);
		if (team != null)
		{
			team.SetAttribute(TeamAttr.HideFly, (float)this.mysteryPrevHideFly, true);
			team.SetAttribute(TeamAttr.HidePop, (float)this.mysteryPrevHidePop, true);
			List<Ship> flyShip = this.shipManager.GetFlyShip(team.team);
			for (int i = 0; i < flyShip.Count; i++)
			{
				Ship ship = flyShip[i];
				EntityShip entityShip = ship.entity as EntityShip;
				if (entityShip != null)
				{
					entityShip.SetForceHide(false);
				}
			}
		}
		foreach (KeyValuePair<string, bool> keyValuePair in this.mysteryNodeForceHidePrev)
		{
			Node node = this.nodeManager.GetNode(keyValuePair.Key);
			if (node != null)
			{
				node.forceHideNode = keyValuePair.Value;
			}
		}
		this.mysteryNodeForceHidePrev.Clear();
		this.mysteryActive = false;
		this.mysteryEndTime = 0f;
	}

	private void ApplyAssimilationAt(Vector3 center, float radiusSqr, Team targetTeam, TEAM targetTeamId)
	{
		List<Node> usefulNodeList = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node != null && node.CanBeTarget())
			{
				Vector3 position = node.GetPosition();
				float sqrMagnitude = (position - center).sqrMagnitude;
				if (sqrMagnitude <= radiusSqr)
				{
					TEAM team = node.team;
					if (team != TEAM.Neutral && team != targetTeamId && targetTeam.IsFriend((int)team))
					{
						continue;
					}
					node.AssimilateTo(targetTeam);
				}
			}
		}
		for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
		{
			TEAM team2 = (TEAM)j;
			if (team2 != targetTeamId)
			{
				if (targetTeam.IsFriend((int)team2))
				{
					continue;
				}
				List<Ship> flyShip = this.shipManager.GetFlyShip(team2);
				for (int k = flyShip.Count - 1; k >= 0; k--)
				{
					Ship ship = flyShip[k];
					if (ship != null)
					{
						Vector3 position2 = ship.GetPosition();
						if ((position2 - center).sqrMagnitude <= radiusSqr)
						{
							this.shipManager.RemoveFlyShip(ship);
							if (ship.currentTeam != null && ship.currentTeam != targetTeam)
							{
								ship.currentTeam.SetAttribute(TeamAttr.Population, (float)(-ship.num), true);
								targetTeam.SetAttribute(TeamAttr.Population, (float)ship.num, true);
							}
							ship.currentTeam = targetTeam;
							this.shipManager.AddFlyShip(ship);
						}
					}
				}
			}
		}
	}

	private void TickBlackStrike(int frame)
	{
		if (frame < this.blackStrikeLastFrame)
		{
			this.blackStrikeCheckedLevel = false;
			this.blackStrikeEnabled = false;
			this.blackStrikeFired = false;
			this.blackStrikeThreshold = 220;
			this.blackStrikeTeleportHideAt = 0f;
			this.blackStrikeTeleportHideFrame = 0;
			this.blackStrikeTeleportHidden = true;
		}
		this.blackStrikeLastFrame = frame;
		if (!this.blackStrikeCheckedLevel)
		{
			this.blackStrikeCheckedLevel = true;
			LevelConfigConfigProvider levelConfigConfigProvider = Solarmax.Singleton<LevelConfigConfigProvider>.Get();
			LevelConfig levelConfig = (levelConfigConfigProvider != null) ? levelConfigConfigProvider.GetData(this.battleData.matchId) : null;
			this.blackStrikeEnabled = (levelConfig != null && levelConfig.IfBlackStrike());
			if (!this.blackStrikeEnabled)
			{
				MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
				this.blackStrikeEnabled = (data != null && data.IfBlackStrike());
				if (this.blackStrikeEnabled)
				{
					this.blackStrikeThreshold = data.GetBlackStrikeThreshold();
				}
			}
			else
			{
				this.blackStrikeThreshold = levelConfig.GetBlackStrikeThreshold();
			}
			this.blackStrikeLevelId = this.battleData.matchId;
		}
		if (!this.blackStrikeEnabled)
		{
			return;
		}
		this.UpdateBlackStrikeTeleport(frame);
		Team team2 = this.teamManager.GetTeam(TEAM.Team_Black1);
		if (team2 != null)
		{
			team2.SetAttribute(TeamAttr.HideFly, 0f, true);
			team2.SetAttribute(TeamAttr.HidePop, 1f, true);
		}
		if (this.blackStrikeFired)
		{
			return;
		}
		int num = 0;
		bool flag = false;
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			if ((TEAM)i != TEAM.Team_Black1)
			{
				Team team = this.teamManager.GetTeam((TEAM)i);
				if (team != null)
				{
					int current = team.current;
					if (current > this.blackStrikeThreshold)
					{
						flag = true;
					}
					if (current > num)
					{
						num = current;
					}
				}
			}
		}
		if (!flag || num <= 0)
		{
			return;
		}
		if (this.nodeManager.GetNode("BlackStrike") != null)
		{
			return;
		}
		this.blackStrikeFired = true;
		this.SpawnBlackStrikeNode(num, frame);
	}

	private void UpdateBlackStrikeTeleport(int frame)
	{
		bool flag = this.blackStrikeTeleportHideFrame > 0 && frame >= this.blackStrikeTeleportHideFrame;
		bool flag2 = this.blackStrikeTeleportHideAt > 0f && this.m_BattleTime >= this.blackStrikeTeleportHideAt;
		if (!flag && !flag2)
		{
			return;
		}
		Node node = this.nodeManager.GetNode("BlackStrike");
		if (node != null)
		{
			this.ClearShipsOnNode(node, TEAM.Team_Black1);
			this.nodeManager.RemoveNode("BlackStrike");
		}
		this.blackStrikeTeleportHideAt = 0f;
		this.blackStrikeTeleportHideFrame = 0;
		this.blackStrikeTeleportHidden = true;
	}

	private void ClearShipsOnNode(Node node, TEAM team)
	{
		if (node == null)
		{
			return;
		}
		Team team2 = this.teamManager.GetTeam(team);
		if (team2 == null)
		{
			return;
		}
		List<Ship> ships = node.GetShips(team);
		if (ships == null)
		{
			return;
		}
		while (ships.Count > 0)
		{
			Ship ship = ships[ships.Count - 1];
			if (ship == null)
			{
				ships.RemoveAt(ships.Count - 1);
			}
			else
			{
				ships.RemoveAt(ships.Count - 1);
				node.numArray[(int)team] -= ship.num;
				team2.SetAttribute(TeamAttr.Population, (float)(-ship.num), true);
				this.shipManager.Recycle(ship);
			}
		}
		node.updateHUD(false);
	}

	private void SpawnBlackStrikeNode(int maxShips, int frame)
	{
		if (this.nodeManager.GetNode("BlackStrike") != null)
		{
			return;
		}
		MapNodeConfigProvider mapNodeConfigProvider = Solarmax.Singleton<MapNodeConfigProvider>.Instance;
		MapNodeConfig mapNodeConfig = (mapNodeConfigProvider != null) ? mapNodeConfigProvider.GetData("teleport", 1) : null;
		if (mapNodeConfig == null)
		{
			return;
		}
		float num = mapNodeConfig.size * 0.8f;
		float nodesize = mapNodeConfig.nodesize * 0.8f;
		Node node = this.AddNode(mapNodeConfig.id, 7, mapNodeConfig.typeEnum, mapNodeConfig.aiWeight, mapNodeConfig.aiUnitLost, 0f, 0f, num, "BlackStrike", mapNodeConfig.createshipnum, mapNodeConfig.createship, (float)mapNodeConfig.hp, mapNodeConfig.food, mapNodeConfig.attackrange, mapNodeConfig.attackspeed, mapNodeConfig.attackpower, 0f, 0, string.Empty, string.Empty, string.Empty, string.Empty, false, nodesize, mapNodeConfig.perfab, mapNodeConfig.skills, 0f, -1, 0f, 0f);
		if (node == null)
		{
			return;
		}
		node.ignoreHideFlyForNode = true;
		if (node.entity != null)
		{
			GameObject gameObject = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Warp") as GameObject;
			if (gameObject == null)
			{
				gameObject = (Solarmax.Singleton<AssetManager>.Get().GetResources("entity_warp") as GameObject);
			}
			if (gameObject != null)
			{
				SpriteRenderer componentInChildren = null;
				Transform transform = gameObject.transform.Find("image");
				if (transform != null)
				{
					componentInChildren = transform.GetComponent<SpriteRenderer>();
				}
				if (componentInChildren == null)
				{
					componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
				}
				GameObject imageGameobject = node.entity.GetImageGameobject();
				if (componentInChildren != null && imageGameobject != null)
				{
					SpriteRenderer component = imageGameobject.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.sprite = componentInChildren.sprite;
					}
				}
			}
		}
		this.blackStrikeTeleportHideAt = this.m_BattleTime + 3f;
		this.blackStrikeTeleportHideFrame = frame + 75;
		this.blackStrikeTeleportHidden = false;
		Team team = this.teamManager.GetTeam(TEAM.Team_Black1);
		if (team != null)
		{
			team.SetAttribute(TeamAttr.HidePop, 1f, true);
			team.SetAttribute(TeamAttr.HideFly, 0f, true);
			if (!team.aiEnable)
			{
				team.aiEnable = true;
				team.playerData.userId = -10007;
				team.playerData.name = AIManager.GetAIName(team.playerData.userId);
				team.playerData.icon = AIManager.GetAIIcon(team.playerData.userId);
				int aiStrategy = -1;
				MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
				if (data != null && data.teamAITypes != string.Empty)
				{
					aiStrategy = data.teamAiType[7];
				}
				if (aiStrategy < 0)
				{
					aiStrategy = this.battleData.aiStrategy;
				}
				if (aiStrategy < 0)
				{
					aiStrategy = 3;
				}
				this.aiManager.AddAI(team, aiStrategy, team.playerData.level, this.battleData.aiLevel, 1f);
			}
		}
		int count = Mathf.CeilToInt((float)maxShips * 1.3f);
		node.AddShip(7, count, true, true);
	}

	public void UpdateRender(float interval)
	{
		this.shipManager.UpdateRender(interval);
	}

	public void Destroy()
	{
		this.EndMystery();
		this.ResetSetParty(true);
		this.teamManager.Destroy();
		this.nodeManager.Destroy();
		this.shipManager.Destroy();
		this.aiManager.Destroy();
		this.warpManager.Destroy();
		this.newSkillManager.Destroy();
		this.m_BattleTime = 0f;
		this.curProcduceIndex = 0;
		this.battleScaleSpeed = this.produceRates[this.curProcduceIndex].produceRate;
		this.blackStrikeLevelId = string.Empty;
		this.blackStrikeCheckedLevel = false;
		this.blackStrikeEnabled = false;
		this.blackStrikeFired = false;
		this.blackStrikeTeleportHideAt = 0f;
		this.blackStrikeTeleportHideFrame = 0;
		this.blackStrikeTeleportHidden = true;
		this.assimilationFired = false;
		this.assimilationLastFrame = 0;
		this.assimilationInit = false;
		this.assimilationRemaining = 0;
		this.assimilationNextTime = 0f;
		this.mysteryInit = false;
		this.mysteryActive = false;
		this.mysteryTriggered = false;
		this.mysteryLastFrame = 0;
		this.mysteryCamp = 0;
		this.mysteryStartTime = 0f;
		this.mysteryDuration = 0f;
		this.mysteryEndTime = 0f;
		this.mysteryPrevHideFly = 0;
		this.mysteryPrevHidePop = 0;
		this.mysteryNodeForceHidePrev.Clear();
	}

	public void Release()
	{
	}

	public void Reset()
	{
		this.EndMystery();
		this.ResetSetParty(true);
		this.m_BattleTime = 0f;
		this.blackStrikeLevelId = string.Empty;
		this.blackStrikeCheckedLevel = false;
		this.blackStrikeEnabled = false;
		this.blackStrikeFired = false;
		this.blackStrikeTeleportHideAt = 0f;
		this.blackStrikeTeleportHideFrame = 0;
		this.blackStrikeTeleportHidden = true;
		this.assimilationFired = false;
		this.assimilationLastFrame = 0;
		this.assimilationInit = false;
		this.assimilationRemaining = 0;
		this.assimilationNextTime = 0f;
		this.mysteryInit = false;
		this.mysteryActive = false;
		this.mysteryTriggered = false;
		this.mysteryLastFrame = 0;
		this.mysteryCamp = 0;
		this.mysteryStartTime = 0f;
		this.mysteryDuration = 0f;
		this.mysteryEndTime = 0f;
		this.mysteryPrevHideFly = 0;
		this.mysteryPrevHidePop = 0;
		this.mysteryNodeForceHidePrev.Clear();
	}

	public bool GetBattleRate()
	{
		return this.m_BattleTime >= 180f;
	}

	public float GetBattleTime()
	{
		return this.m_BattleTime;
	}

	public void RunFramePacket(FrameNode node)
	{
        if (node.msgList == null || node.msgList.Length == 0)
		{
			return;
		}
        for (int i = 0; i < node.msgList.Length; i++)
		{
			this.ExcutePacket(node.msgList[i] as global::Packet);
		}
	}

	private void ExcutePacket(global::Packet packet)
	{
        if (packet.packet.type == 0)
		{
			if (packet.packet.move.team == TEAM.Neutral)
			{
				this.nodeManager.MoveTo(packet.packet.move, packet.team);
			}
			else
			{
				this.nodeManager.MoveTo(packet.packet.move, packet.packet.move.team);
			}
		}
		else if (packet.packet.type == 1)
		{
			this.newSkillManager.OnCast(packet.packet.skill);
		}
		else if (packet.packet.type == 2)
		{
			Solarmax.Singleton<BattleSystem>.Instance.OnPlayerGiveUp(packet.packet.giveup.team);
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.DestroyTeam(packet.packet.giveup.team, TEAM.Neutral);
		}
		else if (packet.packet.type == 3)
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.SetAllTeamMoveSpeed(1f);
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.SetAllTeamProduceSpeed(1f);
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.SetAllOccupySpeed(1f);
		}
		else if (packet.packet.type == 4)
		{
			Node node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.bomb.tag);
			if (node != null)
			{
				if (TouchHandler.currentNode == node)
				{
					TouchHandler.HideOperater();
				}
				node.ReturnToNeutral();
				Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.RemoveNode(node.tag);
				Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.RemoveUnusedBarrier();
				Solarmax.Singleton<BattleSystem>.Instance.sceneManager.shipManager.DestroyAllShipToTargetNode(node);
			}
		}
		else if (packet.packet.type == 6)
		{
			Node node2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.effect.tag);
			if (node2 != null)
			{
				if (packet.packet.effect.effect == "EFF_XJ_Boom_1")
				{
					Solarmax.Singleton<AudioManger>.Get().PlayPlanetExplosion(node2.GetPosition());
					Solarmax.Singleton<EffectManager>.Get().PlaySyncEffect(node2, packet.packet.effect.effect, packet.packet.effect.time, packet.packet.effect.scale);
				}
				else
				{
					Solarmax.Singleton<EffectManager>.Get().PlaySyncEffect(node2, packet.packet.effect.effect, packet.packet.effect.time, packet.packet.effect.scale);
				}
			}
		}
		else if (packet.packet.type == 7)
		{
			Node node3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.attack.tag);
			if (node3 != null)
			{
				node3.AttackToShip(0, node3.AttackSpeed);
			}
		}
		else if (packet.packet.type == 8)
		{
			Node node4 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.addattack.tag);
			if (node4 != null)
			{
				node4.AttackToAddShip(0, node4.AttackSpeed);
			}
		}
		else if (packet.packet.type == 9)
		{
			Node node5 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.gunattack.tag);
			if (node5 != null)
			{
				node5.GunturretAttack(0, node5.AttackSpeed);
			}
		}
		else if (packet.packet.type == 10)
		{
			Node node6 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.laserattack.tag);
			if (node6 != null)
			{
				node6.LasergunAttack(0, node6.AttackSpeed);
			}
		}
		else if (packet.packet.type == 11)
		{
			Node node7 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.twistattack.tag);
			if (node7 != null)
			{
				node7.TwistShip(0, node7.AttackSpeed);
			}
		}
		else if (packet.packet.type == 12)
		{
			Node node8 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.team.tag);
			if (node8 != null)
			{
				node8.ReturnToNeutral();
				node8.hp = node8.hpMax;
				node8.currentTeam = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(packet.packet.team.t);
				Solarmax.Singleton<EffectManager>.Get().AddHalo(node8, node8.currentTeam.color, true, false);
				Solarmax.Singleton<AudioManger>.Get().PlayCapture(node8.GetPosition());
				node8.currentTeam.teamManager.sceneManager.newSkillManager.OnOccupied(node8, node8.currentTeam);
				node8.currentSkill = null;
				node8.OccupiedHUD();
			}
		}
		else if (packet.packet.type == 13)
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.RemoveALLBarriers();
		}
		else if (packet.packet.type == 14)
		{
			Node node9 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.skill.tag);
			Team team = this.teamManager.GetTeam(packet.packet.team.t);
			BaseNewSkill baseNewSkill = this.newSkillManager.AddSkillEX(packet.packet.skill.skillID, team);
			if (baseNewSkill != null)
			{
				baseNewSkill.owerNode = node9;
			}
			baseNewSkill.OnCast(this.teamManager.GetTeam(packet.packet.skill.from));
		}
		else if (packet.packet.type == 15)
		{
			Team team2 = this.teamManager.GetTeam(packet.team);
			Node node10 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.unknown.tag);
			BaseNewSkill baseNewSkill2 = this.newSkillManager.AddSkillEX(packet.packet.unknown.skillID, team2);
			if (baseNewSkill2 != null)
			{
				baseNewSkill2.owerNode = node10;
				baseNewSkill2.scriptMisc = packet.packet.unknown.transformId;
			}
			baseNewSkill2.OnCast(this.teamManager.GetTeam(packet.packet.unknown.from));
		}
		else if (packet.packet.type == 16)
		{
			Node node11 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(packet.packet.effect.tag);
			if (node11 != null)
			{
				node11.choosedBombTarget = true;
				Solarmax.Singleton<EffectManager>.Get().PlaySyncEffect(node11, packet.packet.effect.effect, packet.packet.effect.time, 1f);
			}
		}
	}

	public void CreateScene(IList<int> usr, bool isEditer = false, bool isHaveRandom = false)
	{
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
		if (data == null)
		{
			Debug.LogErrorFormat("CreateScene-Load table is error!!! {0}", new object[]
			{
				this.battleData.matchId
			});
			this.Release();
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			Solarmax.Singleton<UISystem>.Get().ShowWindow("LogoWindow");
			return;
		}
		this.CreateScene(isEditer, data, usr, isHaveRandom);
	}

	public void CreateScene(bool isEditer, MapConfig table, IList<int> usr = null, bool isHaveRandom = false)
	{
		if (this.nodeManager != null)
		{
			this.nodeManager.Destroy();
		}
		if (this.shipManager != null)
		{
			this.shipManager.Destroy();
		}
		if (this.aiManager != null)
		{
			this.aiManager.Destroy();
		}
		if (this.warpManager != null)
		{
			this.warpManager.Destroy();
		}
		if (this.newSkillManager != null)
		{
			this.newSkillManager.Destroy();
		}
		this.battleData.currentTable = table;
		this.CreateNode(table.mbcList);
		this.CreateLines(table.mlcList);
		this.CreateBarrierPoints(table.mbcList);
		this.CreateShip(table.mpcList);
		for (int i = 0; i < table.player_count; i++)
		{
			Team team = this.teamManager.GetTeam((TEAM)(i + 1));
			this.newSkillManager.OnBorn(team);
		}
		if (!isEditer)
		{
			for (int j = 3; j < 8; j++)
			{
				global::Coroutine.DelayDo((float)j, new EventDelegate(delegate()
				{
					foreach (Node node in this.nodes)
					{
						if (node != null)
						{
							node.updateHUD(true);
						}
					}
				}));
			}
		}
		if (!isEditer)
		{
			LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.battleData.matchId);
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data.musicName))
				{
					Solarmax.Singleton<AudioManger>.Get().PlayAudioBG(data.musicName, 0.5f);
				}
				else
				{
					Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Wandering", 0.5f);
				}
			}
			else
			{
				Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Wandering", 0.5f);
			}
		}
		// 添加相机手势控制（双指平移缩放 + 右键滚轮）
		Camera camera = Camera.main;
		if (camera == null)
		{
			camera = (UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera);
		}
		if (camera != null && camera.GetComponent<CameraGestureController>() == null)
		{
			camera.gameObject.AddComponent<CameraGestureController>();
			Solarmax.Singleton<LoggerSystem>.Instance.Info("CameraGestureController added to: " + camera.name, new object[0]);
		}
		else if (camera == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("CameraGestureController: no camera found in scene", new object[0]);
		}
	}

	public void CreateLines(List<MapLineConfig> lines)
	{
		if (lines == null)
		{
			return;
		}
		foreach (MapLineConfig mapLineConfig in lines)
		{
			this.AddBarrierLines(mapLineConfig.point1, mapLineConfig.point2);
		}
	}

	private void CreateBarrierPoints(List<MapBuildingConfig> building)
	{
		if (building == null)
		{
			return;
		}
		foreach (MapBuildingConfig mapBuildingConfig in building)
		{
			if (mapBuildingConfig.type == "BarrierPoint")
			{
				this.nodeManager.dynamicBarriers.Add(mapBuildingConfig);
			}
		}
		if (this.nodeManager.dynamicBarriers.Count < 2)
		{
			this.nodeManager.dynamicBarriers.Clear();
			return;
		}
		for (int i = 0; i < this.nodeManager.dynamicBarriers.Count; i++)
		{
			MapBuildingConfig point1 = this.nodeManager.dynamicBarriers[i];
			for (int j = i + 1; j < this.nodeManager.dynamicBarriers.Count; j++)
			{
				MapBuildingConfig point2 = this.nodeManager.dynamicBarriers[j];
				MapPlayerConfig mapPlayerConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mpcList.Find((MapPlayerConfig n) => n.tag == point1.tag);
				MapPlayerConfig mapPlayerConfig2 = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mpcList.Find((MapPlayerConfig n) => n.tag == point2.tag);
				bool show;
				bool flag2;
				bool flag = flag2 = (show = false);
				if (mapPlayerConfig != null && mapPlayerConfig.ship > 0)
				{
					flag2 = true;
				}
				if (mapPlayerConfig2 != null && mapPlayerConfig2.ship > 0)
				{
					flag = true;
				}
				Vector3 zero;
				Vector3 b = zero = Vector3.zero;
				zero.x = point1.x;
				zero.y = point1.y;
				b.x = point2.x;
				b.y = point2.y;
				float num = Vector3.Distance(zero, b);
				if (flag2 == flag && point1.fbpRange >= num && point2.fbpRange >= num)
				{
					show = true;
				}
				this.nodeManager.AddDynamicBarrierLines(point1.tag, point2.tag, show);
			}
		}
	}

	private void CreateShip(List<MapPlayerConfig> players)
	{
		if (players == null)
		{
			return;
		}
		foreach (MapPlayerConfig mapPlayerConfig in players)
		{
			if (mapPlayerConfig != null)
			{
				int ship = mapPlayerConfig.ship;
				this.AddShip(mapPlayerConfig.tag, ship, mapPlayerConfig.camption, true);
			}
		}
	}

	private void CreateNode(List<MapBuildingConfig> building)
	{
		if (building == null)
		{
			return;
		}
		this.nodes.Clear();
		foreach (MapBuildingConfig mapBuildingConfig in building)
		{
			MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(mapBuildingConfig.type, mapBuildingConfig.size);
			if (data == null)
			{
				break;
			}
			if (string.IsNullOrEmpty(mapBuildingConfig.aistrategy))
			{
				mapBuildingConfig.aistrategy = "-1";
			}
			Node node = this.AddNode(data.id, mapBuildingConfig.camption, data.typeEnum, data.aiWeight, data.aiUnitLost, mapBuildingConfig.x, mapBuildingConfig.y, data.size, mapBuildingConfig.tag, data.createshipnum, data.createship, (float)data.hp, data.food, data.attackrange, data.attackspeed, data.attackpower, 0f, mapBuildingConfig.orbit, mapBuildingConfig.orbitParam1, mapBuildingConfig.orbitParam2, mapBuildingConfig.orbitParam3, mapBuildingConfig.orbitParam4, mapBuildingConfig.orbitClockWise, data.nodesize, data.perfab, data.skills, mapBuildingConfig.fAngle, int.Parse(mapBuildingConfig.aistrategy), mapBuildingConfig.lasergunAngle, mapBuildingConfig.lasergunRange);
			if (node == null)
			{
				continue;
			}
                        if (this.IsSpaceCore(node))
                        {
                                node.spaceCoreProtectedObjectTags = mapBuildingConfig.protectedObjectTags;
                        }
                        node.invincibleMinutes = mapBuildingConfig.invincibleMinutes;
                        node.invincibleEndTime = ((mapBuildingConfig.invincibleMinutes > 0f) ? (this.GetBattleTime() + mapBuildingConfig.invincibleMinutes * 60f) : 0f);
			node.shipProduceOverride = mapBuildingConfig.shipProduceOverride;
			node.RevoSpeed = (float)mapBuildingConfig.orbitRevoSpeed;
			if (node != null && Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "occupy" && (Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1 == node.tag || Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam2 == node.tag))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Solarmax.Singleton<AssetManager>.Get().GetResources("Effect_Aims")) as GameObject;
				gameObject.transform.SetParent(node.entity.GetGO().transform);
				float num = 0.4f / data.size;
				gameObject.transform.localScale = new Vector3(0.03f * num, 0.03f * num, 0.03f * num);
				gameObject.transform.localPosition = new Vector3(-1.7f * num, 1.3f * num, 0f);
			}
			this.nodes.Add(node);
		}
	}

	public Node AddNode(int id, int team, int kind, int weight, int unitlost, float x, float y, float scale, string tag, int buildNum, float buildTimer, float hpMax, int pop, float rage, float speed, float power, float skillspeed, int orbit, string orbitParam1, string orbitParam2, string orbitParam3, string orbitParam4, bool orbitclockwise, float nodesize, string perfab, string skills, float fAngle, int aistrategy, float lasergunAngle = 0f, float lasergunRange = 0f)
	{
		float x2 = (float)Math.Round((double)x, 2);
		float y2 = (float)Math.Round((double)y, 2);
		Node node = this.nodeManager.AddNode(id, team, kind, weight, unitlost, x2, y2, scale, tag, buildNum, buildTimer, hpMax, pop, rage, speed, power, skillspeed, this.battleData.mapEdit, orbit, orbitParam1, orbitParam2, orbitParam3, orbitParam4, orbitclockwise, nodesize, perfab, skills, fAngle, aistrategy);
		if (node != null)
		{
			node.sceneManager = this;
			node.nodePerfab = perfab;
			Team team2 = this.teamManager.GetTeam((TEAM)team);
			node.InitSkills(skills, team2);
			if (node.nodeType == NodeType.Mirror)
			{
				node.RotateNodeImage(fAngle);
			}
			else if (node.nodeType == NodeType.Gunturret)
			{
				node.SetNodeAngle(0f);
				node.RotateNodeImage(fAngle);
			}
			else
			{
				node.SetNodeAngle(fAngle);
			}
			if (node.nodeType == NodeType.FixedWarpDoor)
			{
				this.nodeManager.AddFixedPortal(node.GetPosition(), fAngle, nodesize);
			}
			if (node.nodeType == NodeType.Lasergun && lasergunRange > 0f && lasergunAngle > 0f)
			{
				node.RotateNodeImage(lasergunAngle);
			}
		}
		return node;
	}

	public int GetNodeCount()
	{
		return this.nodeManager.GetNodeCount();
	}

	public Node AddNode(string tag, int kind, float dx, float dy, string perfab)
	{
		if (this.battleData.mapEdit)
		{
			if (this.battleData.currentTable == null)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable = new MapConfig("newmap1");
			}
			if (this.battleData.currentTable.mbcList == null)
			{
				this.battleData.currentTable.mbcList = new List<MapBuildingConfig>();
			}
			MapBuildingConfig mapBuildingConfig = new MapBuildingConfig();
			this.battleData.currentTable.mbcList.Add(mapBuildingConfig);
			mapBuildingConfig.tag = tag;
			mapBuildingConfig.x = dx;
			mapBuildingConfig.y = dy;
			mapBuildingConfig.type = this.GetType(kind);
			mapBuildingConfig.size = 1;
		}
		return this.nodeManager.AddNode(tag, kind, dx, dy, NodeAttribute.GetScale(kind, 1), this.battleData.mapEdit, perfab);
	}

	public Node GetNode(string tag)
	{
		return this.nodeManager.GetNode(tag);
	}

	public string GetType(int kind)
	{
		string[] array = new string[]
		{
			string.Empty,
			"star",
			"castle",
			"teleport",
			"tower",
			"barrier",
			"barrierline",
			"master",
			"defense",
			"power",
			"BlackHole",
			"House",
			"Arsenal",
			"AircraftCarrier",
			"Lasercannon",
			"Attackship",
			"Lifeship",
			"Speedship",
			"Captureship",
			"AntiAttackship",
			"AntiLifeship",
			"AntiSpeedship",
			"AntiCaptureship",
			"Magicstar",
			"Hiddenstar",
			"FixedWarpDoor",
			"Clouds",
			"Inhibit",
			"Twist",
			"AddTower",
			"Sacrifice",
			"OverKill",
			"CloneTo",
			"Treasure",
			"UnknownStar",
			"Lasergun",
			"Mirror",
			"BarrierPoint",
			"Gunturret",
			"Diffusion",
			"Curse",
			"Cannon"
		};
		return array[kind];
	}

	public void RemoveNode(string tag)
	{
		this.nodeManager.RemoveNode(tag);
		if (this.battleData.mapEdit)
		{
			MapBuildingConfig item = this.battleData.currentTable.mbcList.Find((MapBuildingConfig s) => tag.Equals(s.tag));
			this.battleData.currentTable.mbcList.Remove(item);
			if (this.battleData.currentTable.mpcList != null)
			{
				MapPlayerConfig mapPlayerConfig = this.battleData.currentTable.mpcList.Find((MapPlayerConfig i) => tag.Equals(i.tag));
				if (mapPlayerConfig != null)
				{
					this.battleData.currentTable.mpcList.Remove(mapPlayerConfig);
				}
			}
			if (this.battleData.currentTable.mlcList != null)
			{
				for (;;)
				{
					MapLineConfig mapLineConfig = this.battleData.currentTable.mlcList.Find((MapLineConfig s) => s.point1 == tag || s.point2 == tag);
					if (mapLineConfig == null)
					{
						break;
					}
					this.battleData.currentTable.mlcList.Remove(mapLineConfig);
					this.nodeManager.DelBarrierLines(mapLineConfig.point1, mapLineConfig.point2);
				}
			}
		}
	}

	public void AddShip(Node node, int count, int team, bool noAnim = true, bool normal = true)
	{
		if (node == null)
		{
			return;
		}
		node.AddShip(team, count, noAnim, normal);
	}

	public void AddShip(string tag, int count, int team, bool noAnim = true)
	{
		Node node = this.nodeManager.GetNode(tag);
		if (node == null)
		{
			return;
		}
		this.AddShip(node, count, team, noAnim, true);
	}

	public void AddBarrierLines(string barrierX, string barrierY)
	{
		this.nodeManager.AddBarrierLines(barrierX, barrierY);
	}

	public bool GetIntersection(Vector3 v3X, Vector3 v3Y)
	{
		return this.nodeManager.IntersectBarrierLien(v3X, v3Y);
	}

	public bool IsFixedPortal(Vector3 v3X, Vector3 v3Y)
	{
		return this.nodeManager.IsFixedPortal(v3X, v3Y);
	}

	public void AddMapInfo(string name, string num)
	{
		if (!this.MapInfo.ContainsKey(name))
		{
			this.MapInfo.Add(name, num);
		}
	}

	public Dictionary<string, string> GetMapInfo()
	{
		return this.MapInfo;
	}

	public void AddProduce(float startTime, float rateProduce)
	{
		ShipProduce item = default(ShipProduce);
		item.startTime = startTime;
		item.produceRate = rateProduce;
		this.produceRates.Add(item);
	}

	public float GetbattleScaleSpeed()
	{
		return this.battleScaleSpeed;
	}

	public void DestroyTeam(TEAM team, TEAM win)
	{
		List<Node> usefulNodeList = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node.team == team)
			{
				node.BombShip(team, win, 1f);
				node.currentTeam = this.teamManager.GetTeam(TEAM.Neutral);
				if (node.capturingTeam != TEAM.Neutral && node.capturingTeam != team)
				{
					node.hp = 0f;
				}
				if (node.capturingTeam == TEAM.Neutral && node.occupiedTeam == TEAM.Neutral)
				{
					node.hp = 0f;
				}
				if (Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Maker))
				{
					Solarmax.Singleton<EffectManager>.Get().AddHalo(node, node.currentTeam.color, true, false);
					Solarmax.Singleton<AudioManger>.Get().PlayCapture(node.GetPosition());
				}
			}
			else if (node.GetShipCount((int)team) > 0)
			{
				node.BombShip(team, win, 1f);
			}
		}
		List<Ship> flyShip = this.shipManager.GetFlyShip(team);
		int j = flyShip.Count - 1;
		while (j >= 0)
		{
			if (flyShip[j].num == 1)
			{
				flyShip[j].Bomb(NodeType.None);
				j--;
			}
			else
			{
				flyShip[j].Bomb(NodeType.None);
			}
		}
	}

	public void DestroyTeamTemporary(TEAM team)
	{
		List<Node> usefulNodeList = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			int tempShipCount = node.GetTempShipCount((int)team);
			if (node.GetTempShipCount((int)team) > 0)
			{
				node.BombTempShipNum(team, tempShipCount);
			}
		}
		List<Ship> flyShip = this.shipManager.GetFlyShip(team);
		int j = flyShip.Count - 1;
		while (j >= 0)
		{
			if (flyShip[j].temNum == 0)
			{
				j--;
			}
			else
			{
				flyShip[j].Bomb(NodeType.None);
			}
		}
	}

	public void FadePlanet(bool fadeIn, float duration)
	{
		List<Node> usefulNodeList = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node.entity != null)
			{
				node.entity.FadeEntity(fadeIn, duration);
			}
		}
		List<Node> barrierNodeList = this.nodeManager.GetBarrierNodeList();
		for (int j = 0; j < barrierNodeList.Count; j++)
		{
			Node node2 = barrierNodeList[j];
			if (node2.entity != null)
			{
				node2.entity.FadeEntity(fadeIn, duration);
			}
		}
	}

	public void ShowWinEffect(Team win, Color color)
	{
		List<Node> usefulNodeList = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node.type != NodeType.Barrier && node.type != NodeType.BarrierLine)
			{
				Solarmax.Singleton<EffectManager>.Get().AddHalo(node, color, true, false);
				node.entity.SetColor(color);
				node.entity.SetAlpha(1f);
			}
			for (int j = 0; j < LocalPlayer.MaxTeamNum; j++)
			{
				TEAM team = (TEAM)j;
				Team team2 = this.teamManager.GetTeam(team);
				if (win != team2 && !win.IsFriend(team2.groupID))
				{
					node.BombShip(team, win.team, 1f);
				}
			}
		}
	}

	public void SilentMode(bool status)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("========SilentMode : {0},, frame:{1}", status, Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame()), new object[0]);
		this.battleData.silent = status;
		this.nodeManager.SilentMode(status);
		this.shipManager.SilentMode(status);
	}

	public Dictionary<string, string> MapInfo = new Dictionary<string, string>();

	private float m_BattleTime;

	public List<ShipProduce> produceRates = new List<ShipProduce>();

	public int curProcduceIndex;

	public float battleScaleSpeed = 1f;

	private bool blackStrikeCheckedLevel;

	private bool blackStrikeEnabled;

	private bool blackStrikeFired;

	private float blackStrikeTeleportHideAt;

	private int blackStrikeTeleportHideFrame;

	private bool blackStrikeTeleportHidden;

	private string blackStrikeLevelId = string.Empty;

	private int blackStrikeThreshold = 220;

	private int blackStrikeLastFrame;

	private bool assimilationFired;

	private int assimilationLastFrame;

	private bool assimilationInit;

	private int assimilationRemaining;

	private float assimilationNextTime;

	private bool mysteryInit;

	private bool mysteryActive;

	private bool mysteryTriggered;

	private int mysteryLastFrame;

	private int mysteryCamp;

	private float mysteryStartTime;

	private float mysteryDuration;

	private float mysteryEndTime;

	private int mysteryPrevHideFly;

	private int mysteryPrevHidePop;

	private Dictionary<string, bool> mysteryNodeForceHidePrev = new Dictionary<string, bool>();

	private List<Node> nodes = new List<Node>();

	private List<SceneManager.SetPartyRuntimeState> setPartyStates = new List<SceneManager.SetPartyRuntimeState>();

	private int setPartyLastFrame;

	private bool mapSwitched = false;

	private bool isFading = false;

	private float fadeAlpha = 1f;

	private float fadeSpeed = 2f;

	private class SetPartyRuntimeState
	{
		public MapSetPartyConfig config;

		public bool active;

		public float nextStartTime;

		public float endTime;
	}

	#region Map & Music Switch

	/// <summary>
	/// 检查节点被攻陷/摧毁是否触发地图切换
	/// </summary>
	public void CheckAndTriggerMapSwitch(Node node, Team previousOwner)
	{
		if (this.mapSwitched || this.isFading || node == null || previousOwner == null)
		{
			return;
		}

		// 检查节点是否被玩家阵营攻陷（包括玩家和队友AI）
		Team playerTeam = this.teamManager.GetTeam(this.battleData.currentTeam);
		if (node.currentTeam == null || !playerTeam.IsFriend(node.currentTeam.groupID))
		{
			return; // 不是玩家阵营攻陷的,忽略
		}

		// 检查节点是否有 "The end" 标签
		MapConfig mapConfig = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
		if (mapConfig == null || string.IsNullOrEmpty(mapConfig.the_end) || node.tag != mapConfig.the_end)
		{
			return; // 不是触发节点
		}

		// 获取当前关卡配置
		LevelConfig levelConfig = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.battleData.matchId);
		if (levelConfig == null || string.IsNullOrEmpty(levelConfig.insideMap) || string.IsNullOrEmpty(levelConfig.insideMusicName))
		{
			return; // 没有配置第二地图或音乐
		}

		// 触发切换
		this.SwitchMapAndMusic(levelConfig);
	}

	/// <summary>
	/// 执行地图和音乐切换
	/// </summary>
	private void SwitchMapAndMusic(LevelConfig levelConfig)
	{
		this.mapSwitched = true;
		this.isFading = true;

		// 1. 暂停游戏
		Solarmax.Singleton<BattleSystem>.Instance.SetPause(true);

		// 2. 停止当前音乐
		Solarmax.Singleton<AudioManger>.Get().MuteBGAudio(true);

		// 3. 渐白屏幕
		Coroutine.Start(this.FadeScreenCoroutine(levelConfig));
	}

	/// <summary>
	/// 渐白屏幕的协程
	/// </summary>
	private IEnumerator FadeScreenCoroutine(LevelConfig levelConfig)
	{
		// 渐白
		while (this.fadeAlpha > 0f)
		{
			this.fadeAlpha -= Time.deltaTime * this.fadeSpeed;
			this.fadeAlpha = Mathf.Clamp01(this.fadeAlpha);

			// 更新白色遮罩(仅背景层,不影响UI)
			this.UpdateWhiteOverlay(1f - this.fadeAlpha);

			yield return null;
		}

		// 切换地图和音乐
		this.SwitchToInsideMap(levelConfig);

		// 暂停一小段时间让玩家看到新地图
		yield return new WaitForSeconds(0.5f);

		// 恢复游戏,开始渐回
		this.isFading = false;
		Coroutine.Start(this.UnfadeScreenCoroutine());
	}

	/// <summary>
	/// 切换到内部地图
	/// </summary>
	private void SwitchToInsideMap(LevelConfig levelConfig)
	{
		MapConfig newMap = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(levelConfig.insideMap);
		if (newMap != null)
		{
			// 停止当前音乐
			Solarmax.Singleton<AudioManger>.Get().StopAudioBG();

			// 更新当前地图
			this.battleData.currentTable = newMap;
			this.battleData.matchId = levelConfig.insideMap;

			// 重新创建场景
			this.CreateScene(false, newMap, null, false);

			// 切换音乐
			Solarmax.Singleton<AudioManger>.Get().PlayAudioBG(levelConfig.insideMusicName, 0.5f);
		}
	}

	/// <summary>
	/// 渐回屏幕的协程
	/// </summary>
	private IEnumerator UnfadeScreenCoroutine()
	{
		// 渐回
		while (this.fadeAlpha < 1f)
		{
			this.fadeAlpha += Time.deltaTime * this.fadeSpeed;
			this.fadeAlpha = Mathf.Clamp01(this.fadeAlpha);

			// 更新白色遮罩
			this.UpdateWhiteOverlay(1f - this.fadeAlpha);

			yield return null;
		}

		// 完全恢复后,恢复游戏
		this.UpdateWhiteOverlay(0f);
		this.isFading = false;

		// 恢复游戏 (在渐回完成后)
		Solarmax.Singleton<BattleSystem>.Instance.SetPause(false);
		this.SetInteractionEnabled(true);
	}

	/// <summary>
	/// 更新白色遮罩层
	/// </summary>
	private void UpdateWhiteOverlay(float alpha)
	{
		// 创建一个全屏的白色面板用于渐白效果
		// 这个面板应该在不阻挡UI的情况下覆盖背景
		// 实现方式:创建一个Canvas,设置为只在Background层渲染
		if (this.whiteOverlay == null)
		{
			GameObject overlayObj = new GameObject("MapSwitchWhiteOverlay");
			overlayObj.transform.SetParent(this.transform, false);

			// 设置为背景层
			overlayObj.layer = LayerMask.NameToLayer("Invisible");

			// 添加UI组件
			Canvas canvas = overlayObj.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceCamera;
			canvas.planeDistance = 100; // 在背景之前

			UIImage image = overlayObj.AddComponent<UIImage>();
			image.color = new Color(1f, 1f, 1f, alpha);
			image.type = UIImage.Type.Sliced;

			this.whiteOverlay = overlayObj;
		}
		else
		{
			UIImage image = this.whiteOverlay.GetComponent<UIImage>();
			if (image != null)
			{
				image.color = new Color(1f, 1f, 1f, alpha);
			}
		}
	}

	/// <summary>
	/// 设置交互是否可用
	/// </summary>
	private void SetInteractionEnabled(bool enabled)
	{
		// 设置一个全局标志位,在游戏逻辑中检查此标志
		this.inputEnabled = enabled;

		// 可以在这里添加禁用UI交互的具体逻辑
		// 例如禁用战斗窗口的按钮等
	}

	/// <summary>
	/// 检查输入是否可用
	/// </summary>
	public bool IsInputEnabled()
	{
		return this.inputEnabled;
	}

	private bool inputEnabled = true;

	/// <summary>
	/// 禁用所有UI交互
	/// </summary>
	private void DisableAllUIInteraction()
	{
		// 暂时留空,可以在需要时扩展
		// 例如可以禁用所有UI按钮的点击事件
	}

	/// <summary>
	/// 启用所有UI交互
	/// </summary>
	private void EnableAllUIInteraction()
	{
		// 重新启用UI交互
	}

	private GameObject whiteOverlay;

	#endregion
}
