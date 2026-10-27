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
		this.destroyLine = new DestroyLineController(this);
	}

	public WarpManager warpManager { get; set; }

	public TeamManager teamManager { get; set; }

	public NodeManager nodeManager { get; set; }

	public ShipManager shipManager { get; set; }

	public AIManager aiManager { get; set; }

	public NewSkillManager newSkillManager { get; set; }

	public BattleData battleData { get; set; }

	public DestroyLineController destroyLine { get; set; }

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
		this.destroyLine.PreTick(interval);
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
		this.destroyLine.PostTick();
		this.ProcessPendingRemoves();
	}

	/// <summary>
	/// 反同步校验（临时调试用）：计算当前逻辑帧的状态哈希（天体位置 + 飞行飞船数 + 势力人口）。
	/// 联机时两台机器每帧哈希应一致，第一个不一致的帧就是不同步的起点。
	/// </summary>
	public int ComputeSyncHash()
	{
		int hash = 17;
		List<Node> nodes = this.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < nodes.Count; i++)
		{
			Node n = nodes[i];
			if (n == null)
			{
				continue;
			}
			Vector3 p = n.GetPosition();
			hash = hash * 31 + (int)Math.Round((double)p.x * 100.0);
			hash = hash * 31 + (int)Math.Round((double)p.y * 100.0);
		}
		for (int t = 0; t < LocalPlayer.MaxTeamNum; t++)
		{
			List<Ship> fly = this.shipManager.GetFlyShip((TEAM)t);
			hash = hash * 31 + fly.Count;
		}
		for (int t2 = 0; t2 < LocalPlayer.MaxTeamNum; t2++)
		{
			Team team = this.teamManager.GetTeam((TEAM)t2);
			if (team != null)
			{
				hash = hash * 31 + team.current;
				hash = hash * 31 + team.currentMax;
			}
		}
		return hash;
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

	/// <summary>
	/// 以某点为中心、半径 radius 同化周围天体到 targetTeamId 势力（供 Heart boss 等调用）。
	/// </summary>
	public void AssimilateAround(Vector3 center, float radius, TEAM targetTeamId)
	{
		Team targetTeam = this.teamManager.GetTeam(targetTeamId);
		if (targetTeam == null)
		{
			return;
		}
		this.ApplyAssimilationAt(center, radius * radius, targetTeam, targetTeamId);
	}

	/// <summary>
	/// 炸星：清空节点与驻留飞船并播放爆炸特效，然后延迟移除该节点。
	/// </summary>
	public void ExplodeNode(Node node)
	{
		if (node == null)
		{
			return;
		}
		Solarmax.Singleton<AudioManger>.Get().PlayPlanetExplosion(node.GetPosition());
		Solarmax.Singleton<EffectManager>.Get().PlaySyncEffect(node, "EFF_XJ_Boom_1", 0f, 1f);
		node.ReturnToNeutral();
		this.shipManager.DestroyAllShipToTargetNode(node);
		if (!this.pendingRemoveNodes.Contains(node))
		{
			this.pendingRemoveNodes.Add(node);
		}
	}

	/// <summary>
	/// 在 Tick 迭代结束后统一移除 pending 的节点（避免迭代中修改节点列表导致越界）。
	/// </summary>
	private void ProcessPendingRemoves()
	{
		if (this.pendingRemoveNodes.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.pendingRemoveNodes.Count; i++)
		{
			Node node = this.pendingRemoveNodes[i];
			if (node != null)
			{
				this.nodeManager.RemoveNode(node.tag);
				this.nodeManager.RemoveUnusedBarrier();
			}
		}
		this.pendingRemoveNodes.Clear();
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
		this.mapSwitchIndex = 0;
		this.mapSwitched = false;
		this.isFading = false;
		if (this.storyTextGo != null)
		{
			GameObject.Destroy(this.storyTextGo);
			this.storyTextGo = null;
		}
		this.pendingRemoveNodes.Clear();
		this.teamManager.Destroy();
		this.nodeManager.Destroy();
		this.shipManager.Destroy();
		this.aiManager.Destroy();
		this.warpManager.Destroy();
		this.newSkillManager.Destroy();
		this.destroyLine.Reset();
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
		this.mapSwitchIndex = 0;
		this.mapSwitched = false;
		this.isFading = false;
		if (this.storyTextGo != null)
		{
			GameObject.Destroy(this.storyTextGo);
			this.storyTextGo = null;
		}
		this.pendingRemoveNodes.Clear();
		this.destroyLine.Reset();
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
		this.destroyLine.Setup(table);
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
				// 如果不是地图切换场景，才自动播放音乐
				if (!this.skipPlayMusicOnCreateScene && !string.IsNullOrEmpty(data.musicName))
				{
					Solarmax.Singleton<AudioManger>.Get().PlayAudioBG(data.musicName, 0.5f);
				}
				else if (this.skipPlayMusicOnCreateScene)
				{
					Debug.Log("[MapSwitch] CreateScene: 跳过自动播放音乐（地图切换模式）");
				}
				else
				{
					Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Wandering", 0.5f);
				}
			}
			else
			{
				if (!this.skipPlayMusicOnCreateScene)
				{
					Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Wandering", 0.5f);
				}
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
			// Heart 的机制按 type 字符串决定（typeEnum 只用于贴图/编辑器，机制仍走 HeartNode）
			int kind = data.typeEnum;
			if (string.Equals(mapBuildingConfig.type, "Heart"))
			{
				kind = (int)NodeType.Heart;
			}
			Node node = this.AddNode(data.id, mapBuildingConfig.camption, kind, data.aiWeight, data.aiUnitLost, mapBuildingConfig.x, mapBuildingConfig.y, data.size, mapBuildingConfig.tag, data.createshipnum, data.createship, (float)data.hp, data.food, data.attackrange, data.attackspeed, data.attackpower, 0f, mapBuildingConfig.orbit, mapBuildingConfig.orbitParam1, mapBuildingConfig.orbitParam2, mapBuildingConfig.orbitParam3, mapBuildingConfig.orbitParam4, mapBuildingConfig.orbitClockWise, data.nodesize, data.perfab, data.skills, mapBuildingConfig.fAngle, int.Parse(mapBuildingConfig.aistrategy), mapBuildingConfig.lasergunAngle, mapBuildingConfig.lasergunRange);
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
			node.appear = mapBuildingConfig.appear;
			if (node is HeartNode)
			{
				((HeartNode)node).SetConnectedTags(mapBuildingConfig.connected);
			}
			node.RevoSpeed = (float)mapBuildingConfig.orbitRevoSpeed;
			if (node != null && Solarmax.Singleton<BattleSystem>.Instance.battleData.winType == "occupy" && (Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1 == node.tag || Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam2 == node.tag))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Solarmax.Singleton<AssetManager>.Get().GetResources("Effect_Aims")) as GameObject;
				gameObject.transform.SetParent(node.entity.GetGO().transform);
				float num = 0.4f / data.size;
				gameObject.transform.localScale = new Vector3(0.03f * num, 0.03f * num, 0.03f * num);
				gameObject.transform.localPosition = new Vector3(-1.7f * num, 1.3f * num, 0f);
			}
			// The_end 节点提示：复用 occupy 的 Effect_Aims 瞄准标记（地图切换链的目标节点）
			MapConfig mapCfg = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
			if (node != null && mapCfg != null && !string.IsNullOrEmpty(mapCfg.the_end) && node.tag == mapCfg.the_end)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Solarmax.Singleton<AssetManager>.Get().GetResources("Effect_Aims")) as GameObject;
				gameObject2.transform.SetParent(node.entity.GetGO().transform);
				float num2 = 0.4f / data.size;
				gameObject2.transform.localScale = new Vector3(0.03f * num2, 0.03f * num2, 0.03f * num2);
				gameObject2.transform.localPosition = new Vector3(-1.7f * num2, 1.3f * num2, 0f);
				// 染成蓝色，与 occupy 的默认颜色区分
				SpriteRenderer[] aimRenderers2 = gameObject2.GetComponentsInChildren<SpriteRenderer>(true);
				for (int i = 0; i < aimRenderers2.Length; i++)
				{
					aimRenderers2[i].color = new Color(0.3f, 0.6f, 1f, 1f);
				}
			}
			this.nodes.Add(node);
		}
		// Heart 的 connected 子天体可能排在本天体之后（例如 306510.1 的 Heart tag=0 排在 castles 之前），
		// 等所有节点都创建完成后再画 Heart -> 子天体连线，否则 GetNode 找不到子天体导致连线缺失。
		for (int i = 0; i < this.nodes.Count; i++)
		{
			Node node2 = this.nodes[i];
			if (node2 is HeartNode)
			{
				((HeartNode)node2).CreateConnectedLines();
			}
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
			"Cannon",
			"Heart"
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

	/// <summary>
	/// 当前地图切换链的下标（指向 inside_map / inside_musicName 列表中下一张要切换的地图）。
	/// </summary>
	private int mapSwitchIndex = 0;

	/// <summary>待延迟移除的节点（在 Tick 迭代中不能直接 RemoveNode，待 nodeManager.Tick 结束后统一移除）。</summary>
	private List<Node> pendingRemoveNodes = new List<Node>();

	public bool isFading = false;

	private float fadeAlpha = 1f;

	private float fadeSpeed = 2f;

	/// <summary>
	/// 白色遮罩对象
	/// </summary>
	private GameObject whiteOverlay = null;

	/// <summary>
	/// 剧情字幕对象（白屏切换期间显示）
	/// </summary>
	private GameObject storyTextGo = null;

	/// <summary>
	/// 标记是否在场景创建时跳过自动播放音乐（用于地图切换）
	/// </summary>
	private bool skipPlayMusicOnCreateScene = false;

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
		Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 检查节点 tag=" + (node != null ? node.tag.ToString() : "null") +
		           ", previousOwner=" + (previousOwner != null ? previousOwner.groupID.ToString() : "null"));

		if (this.mapSwitched || this.isFading || node == null || previousOwner == null)
		{
			Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 跳过 - mapSwitched=" + this.mapSwitched + ", isFading=" + this.isFading +
			           ", node=" + (node != null) + ", previousOwner=" + (previousOwner != null));
			return;
		}

		// 只有我方（玩家或友军）攻陷 The_end 才触发，敌方/中立攻陷不触发
		Team playerTeam = this.teamManager.GetTeam(this.battleData.currentTeam);
		Team capturer = this.teamManager.GetTeam(node.occupiedTeam);
		if (!playerTeam.IsFriend(capturer.groupID))
		{
			Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 跳过 - 攻陷者不是我方，occupiedTeam=" + node.occupiedTeam);
			return;
		}

		Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 我方攻陷，继续检查");

		// 检查节点是否有 "The end" 标签
		MapConfig mapConfig = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
		if (mapConfig == null || string.IsNullOrEmpty(mapConfig.the_end) || node.tag != mapConfig.the_end)
		{
			Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 跳过 - 不是The_end节点。mapConfig=" + (mapConfig != null) +
			           ", the_end=" + (mapConfig != null ? mapConfig.the_end : "null") + ", node.tag=" + node.tag);
			return; // 不是触发节点
		}

		Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 节点标签匹配 The_end=" + mapConfig.the_end);

		// 获取原始关卡配置（地图切换后 matchId 会变成第二张地图ID，需用原始 levelId 查找完整切换链）
		string levelConfigId = string.IsNullOrEmpty(this.battleData.levelId) ? this.battleData.matchId : this.battleData.levelId;
		LevelConfig levelConfig = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelConfigId);
		if (levelConfig == null)
		{
			Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 跳过 - 未找到关卡配置。levelConfigId=" + levelConfigId);
			return;
		}
		string[] insideMapList = levelConfig.GetInsideMapList();
		string[] insideMusicList = levelConfig.GetInsideMusicNameList();
		if (insideMapList.Length == 0 || this.mapSwitchIndex >= insideMapList.Length)
		{
			Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 无下一张地图（inside_map 未配置或切换链已走完），攻陷 The_end 触发胜利");
			this.TriggerVictory();
			return;
		}
		if (insideMusicList.Length == 0)
		{
			Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 跳过 - 未配置 inside_musicName");
			return;
		}
		string nextMap = insideMapList[this.mapSwitchIndex].Trim();
		string nextMusic = insideMusicList[Mathf.Min(this.mapSwitchIndex, insideMusicList.Length - 1)].Trim();
		if (string.IsNullOrEmpty(nextMap) || string.IsNullOrEmpty(nextMusic))
		{
			Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 跳过 - 下一张地图或音乐为空");
			return;
		}

		Debug.Log("[MapSwitch] CheckAndTriggerMapSwitch: 所有检查通过，触发第 " + (this.mapSwitchIndex + 1) + " 次地图切换 -> " + nextMap);

		// 触发切换
		this.SwitchMapAndMusic(nextMap, nextMusic, mapConfig.storySentences, mapConfig.storyKeep, mapConfig.storyFade);
	}

	/// <summary>
	/// 攻陷 The_end 且无下一张 inside_map 时触发胜利（单机/合作走 singleBattleController，PVP 走 pvpBattleController）。
	/// </summary>
	private void TriggerVictory()
	{
		BattleSystem bs = Solarmax.Singleton<BattleSystem>.Instance;
		if (bs == null)
		{
			return;
		}
		bs.battleData.winTEAM = bs.battleData.currentTeam;
		if (bs.battleData.gameType == GameType.PVP)
		{
			bs.pvpBattleController.QuitBattle(true);
		}
		else
		{
			bs.singleBattleController.QuitBattle(true);
		}
	}

	/// <summary>
	/// 执行地图和音乐切换
	/// </summary>
	private void SwitchMapAndMusic(string insideMap, string insideMusicName, List<string> storySentences, float storyKeep, float storyFade)
	{
		Debug.Log("[MapSwitch] 开始地图切换，insideMap=" + insideMap + ", insideMusic=" + insideMusicName);

		this.mapSwitched = true;
		this.isFading = true;

		// 1. 暂停游戏
		Solarmax.Singleton<BattleSystem>.Instance.SetPause(true);

		// 2. 停止当前音乐
		Solarmax.Singleton<AudioManger>.Get().MuteBGAudio(true);

		// 3. 渐白屏幕
		Coroutine.Start(this.FadeScreenCoroutine(insideMap, insideMusicName, storySentences, storyKeep, storyFade));
	}

	/// <summary>
	/// 渐白屏幕的协程
	/// </summary>
	private IEnumerator FadeScreenCoroutine(string insideMap, string insideMusicName, List<string> storySentences, float storyKeep, float storyFade)
	{
		Debug.Log("[MapSwitch] FadeScreenCoroutine: 开始渐白");
		// 渐白
		while (this.fadeAlpha > 0f)
		{
			this.fadeAlpha -= Time.deltaTime * this.fadeSpeed;
			this.fadeAlpha = Mathf.Clamp01(this.fadeAlpha);

			// 更新白色遮罩(仅背景层,不影响UI)
			this.UpdateWhiteOverlay(1f - this.fadeAlpha);

			yield return null;
		}

		Debug.Log("[MapSwitch] FadeScreenCoroutine: 渐白完成");
		// 白屏期间播放剧情字幕（若有），播完再切换
		if (storySentences != null && storySentences.Count > 0)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("[MapSwitch] FadeScreenCoroutine: 播放剧情字幕，共 " + storySentences.Count + " 句");
			yield return this.PlayStoryCoroutine(storySentences, storyKeep, storyFade);
		}
		else
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("[MapSwitch] FadeScreenCoroutine: 无剧情字幕（storySentences 为空），跳过字幕");
		}

		Debug.Log("[MapSwitch] FadeScreenCoroutine: 切换地图");
		// 切换地图和音乐
		this.SwitchToInsideMap(insideMap, insideMusicName);

		// 暂停一小段时间让玩家看到新地图
		yield return new WaitForSeconds(0.5f);

		// 恢复游戏,开始渐回
		this.isFading = false;
		Coroutine.Start(this.UnfadeScreenCoroutine());
	}

	/// <summary>
	/// 白屏期间播放剧情字幕：逐句显示（每句从左到右逐字浮现），播完停留 keep 秒后渐隐，再出下一句。
	/// </summary>
	private IEnumerator PlayStoryCoroutine(List<string> sentences, float keep, float fade)
	{
		if (sentences == null || sentences.Count == 0)
		{
			yield break;
		}

		Solarmax.Singleton<LoggerSystem>.Instance.Info("[MapSwitch] PlayStoryCoroutine: 开始播放剧情字幕，共 " + sentences.Count + " 句，keep=" + keep + ", fade=" + fade);

		UIRoot uiRoot = Solarmax.Singleton<UISystem>.Get().GetNGUIRoot();
		if (uiRoot == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("[MapSwitch] PlayStoryCoroutine: 找不到 UIRoot，字幕无法创建！");
			yield break;
		}

		GameObject storyGo = new GameObject("StoryText");
		storyGo.layer = uiRoot.gameObject.layer;
		storyGo.transform.SetParent(uiRoot.transform, false);
		storyGo.transform.localPosition = Vector3.zero;
		storyGo.transform.localScale = Vector3.one;

		UIPanel panel = storyGo.AddComponent<UIPanel>();
		panel.depth = 1000;
		// 白屏遮罩是手动 renderQueue=3001 的 Quad，这里显式把字幕画到更高层级，避免被白屏盖住。
		panel.renderQueue = UIPanel.RenderQueue.Explicit;
		panel.startingRenderQueue = 3100;

		UILabel label = storyGo.AddComponent<UILabel>();
		UILabel src = this.FindFontLabel();
		if (src != null)
		{
			if (src.bitmapFont != null)
			{
				label.bitmapFont = src.bitmapFont;
				Solarmax.Singleton<LoggerSystem>.Instance.Info("[MapSwitch] PlayStoryCoroutine: 字幕字体来源=" + src.name + "（位图字体 " + src.bitmapFont.name + "）");
								}
			else if (src.trueTypeFont != null)
			{
				label.trueTypeFont = src.trueTypeFont;
				Solarmax.Singleton<LoggerSystem>.Instance.Info("[MapSwitch] PlayStoryCoroutine: 字幕字体来源=" + src.name + "（动态字体 " + src.trueTypeFont.name + "）");
								}
		}
		else
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("[MapSwitch] PlayStoryCoroutine: 找不到任何带字体的 UILabel，字幕将无法显示！");
						}

		label.supportEncoding = true;
		label.alignment = NGUIText.Alignment.Center;
		label.pivot = UIWidget.Pivot.Center;
		label.overflowMethod = UILabel.Overflow.ClampContent;
		label.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		label.fontSize = 48;
		float uiHeight = uiRoot.activeHeight;
		Transform camTransform = uiRoot.transform.Find("Camera");
		Camera uiCamera = (camTransform != null) ? camTransform.GetComponent<Camera>() : Camera.main;
		float aspect = (uiCamera != null) ? uiCamera.aspect : ((float)Screen.width / (float)Screen.height);
		label.width = Mathf.RoundToInt(uiHeight * aspect * 0.8f);
		label.height = Mathf.RoundToInt((float)label.fontSize * 3f);
		Solarmax.Singleton<LoggerSystem>.Instance.Info("[MapSwitch] PlayStoryCoroutine: 字幕 label 尺寸 " + label.width + "x" + label.height + ", fontSize=" + label.fontSize + ", uiHeight=" + uiHeight + ", aspect=" + aspect);

		TypewriterEffect tw = storyGo.AddComponent<TypewriterEffect>();
		tw.charsPerSecond = 15;

		this.storyTextGo = storyGo;

		for (int i = 0; i < sentences.Count; i++)
		{
			string sentence = sentences[i];
			if (string.IsNullOrEmpty(sentence))
			{
				continue;
			}

			// 逐句显示：填入本句后调用 ResetToBeginning 重置打字机（含 mNextChar 计时），逐字从左到右浮现
			label.alpha = 1f;
			label.text = sentence;
			tw.ResetToBeginning();

			float typeTimeout = Mathf.Max(5f, (float)sentence.Length * 0.3f);
			float typeElapsed = 0f;
			while (tw.isActive && typeElapsed < typeTimeout)
			{
				typeElapsed += Time.deltaTime;
				yield return null;
			}

			// 本句播完后停留 keep 秒
			if (keep > 0f)
			{
				yield return new WaitForSeconds(keep);
			}

			// 渐隐本句
			if (fade > 0f)
			{
				float elapsed = 0f;
				while (elapsed < fade)
				{
					elapsed += Time.deltaTime;
					label.alpha = Mathf.Clamp01(1f - elapsed / fade);
					yield return null;
				}
			}
			label.alpha = 0f;
		}

		if (storyGo != null)
		{
			GameObject.Destroy(storyGo);
		}
		this.storyTextGo = null;
	}

	/// <summary>
	/// 找一个使用中文字体的 UILabel，优先 MFYueHei（猫饭悦黑），用于给剧情字幕拷贝字体。
	/// </summary>
	private UILabel FindFontLabel()
	{
		UILabel[] allLabels = Resources.FindObjectsOfTypeAll<UILabel>();

		// 1. 优先：MFYueHei（猫饭悦黑）
		for (int i = 0; i < allLabels.Length; i++)
		{
			UILabel l = allLabels[i];
			if (l != null && this.GetFontName(l).ToLower().IndexOf("mfyuehei") >= 0)
			{
				return l;
			}
		}

		// 2. 其次：fzdbsjw（方正大标宋）
		for (int i = 0; i < allLabels.Length; i++)
		{
			UILabel l = allLabels[i];
			if (l != null && this.GetFontName(l).ToLower().IndexOf("fzdbsjw") >= 0)
			{
				return l;
			}
		}

		// 3. 兜底：任意带字体的 label（跳过数字/符号字体 downlink / countdown）
		for (int i = 0; i < allLabels.Length; i++)
		{
			UILabel l = allLabels[i];
			if (l == null)
			{
				continue;
			}
			string fn = this.GetFontName(l).ToLower();
			if (fn.Length > 0 && fn.IndexOf("downlink") < 0 && fn.IndexOf("countdown") < 0)
			{
				return l;
			}
		}

		// 4. 最后：任意带字体的 label
		for (int i = 0; i < allLabels.Length; i++)
		{
			UILabel l = allLabels[i];
			if (l != null && (l.bitmapFont != null || l.trueTypeFont != null))
			{
				return l;
			}
		}
		return null;
	}

	private string GetFontName(UILabel l)
	{
		if (l == null)
		{
			return string.Empty;
		}
		if (l.bitmapFont != null)
		{
			return l.bitmapFont.name;
		}
		if (l.trueTypeFont != null)
		{
			return l.trueTypeFont.name;
		}
		return string.Empty;
	}

	/// <summary>
	/// 切换到内部地图
	/// </summary>
	private void SwitchToInsideMap(string insideMap, string insideMusicName)
	{
		Debug.Log("[MapSwitch] SwitchToInsideMap: 尝试读取地图 " + insideMap);

		// 先检查 MapConfigProvider 是否加载了这个地图
		MapConfig checkMap = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(insideMap);
		if (checkMap == null)
		{
			Debug.LogError("[MapSwitch] SwitchToInsideMap: 地图 " + insideMap + " 未在 MapConfigProvider 中注册！");
			Debug.LogError("[MapSwitch] 请检查：1. maplist.xml 中是否有 " + insideMap + " 的注册  2. 是否重启了 Unity Editor");

			// 列出所有已加载的地图，帮助调试
			Debug.Log("[MapSwitch] 已加载的地图数量: " + Solarmax.Singleton<MapConfigProvider>.Instance.GetAllData().Count);
			return;
		}

		MapConfig newMap = checkMap;
		Debug.Log("[MapSwitch] SwitchToInsideMap: 成功获取地图对象，ID=" + newMap.id);

		// 停止当前音乐
		Solarmax.Singleton<AudioManger>.Get().MuteBGAudio(true);

		// 更新当前地图（levelId 仅在首次切换时记录原始关卡ID，避免链式切换时被覆盖）
		this.battleData.currentTable = newMap;
		if (string.IsNullOrEmpty(this.battleData.levelId))
		{
			this.battleData.levelId = this.battleData.matchId;
		}
		this.battleData.matchId = insideMap;
		this.mapSwitchIndex++;

		Debug.Log("[MapSwitch] SwitchToInsideMap: 更新 battleData.matchId = " + this.battleData.matchId);

		// 标记跳过自动播放音乐
		this.skipPlayMusicOnCreateScene = true;

		// 在创建新场景前，清零统计数据（保留 AI 配置，不清除 aiEnable）
		Debug.Log("[MapSwitch] SwitchToInsideMap: 创建新场景，清零统计数据");
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			TEAM teamId = (TEAM)i;
			Team team = this.teamManager.GetTeam(teamId);
			if (team != null)
			{
				Debug.Log("[MapSwitch] SwitchToInsideMap: 清零队伍 " + teamId + " 的统计数据");
				// 清零当前飞船数和上限（Population / PopulationMax 属性），避免代入第二地图
				team.SetAttribute(TeamAttr.Population, -(float)team.current, true);
				team.SetAttribute(TeamAttr.PopulationMax, -(float)team.currentMax, true);
				// 注意：不清除 aiEnable、groupID、teamFriend，以保留 AI 配置
			}
		}


		// 重新创建场景（这是一个耗时操作，会阻塞）
		Debug.Log("[MapSwitch] SwitchToInsideMap: 调用 CreateScene - 开始");
		this.CreateScene(false, newMap, null, false);
		Debug.Log("[MapSwitch] SwitchToInsideMap: 调用 CreateScene - 完成");

		// 恢复标记
		this.skipPlayMusicOnCreateScene = false;

		// 清除第一个地图中已战败势力的实体（CreateScene 会按新地图配置重建所有势力，需把已结束势力彻底移除，否则它们会残留却无法操控）
		Debug.Log("[MapSwitch] SwitchToInsideMap: 清除已战败势力实体");
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.teamManager.GetTeam((TEAM)i);
			if (team != null && team.isEnd)
			{
				Debug.Log("[MapSwitch] SwitchToInsideMap: 清除势力 " + (TEAM)i);
				this.DestroyTeam((TEAM)i, TEAM.Neutral);
			}
		}

		// 激活新地图的天体实体（CreateScene 创建的实体默认是隐藏的）
		Debug.Log("[MapSwitch] SwitchToInsideMap: 激活天体实体 FadePlanet(true)");
		this.FadePlanet(true, 0f);

		// 激活飞船
		Debug.Log("[MapSwitch] SwitchToInsideMap: 激活飞船 SetFadeType(IN)");
		Solarmax.Singleton<ShipFadeManager>.Get().SetFadeType(ShipFadeManager.FADETYPE.IN, 0f);

		// 重新启动 AI（CreateScene 会销毁 AI 管理器，需要重新初始化，采用新地图 teamAITypes）
		Debug.Log("[MapSwitch] SwitchToInsideMap: 重新启动 AI（采用新地图 teamAITypes）");
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.teamManager.GetTeam((TEAM)i);
			if (team != null && team.aiEnable && !team.isEnd)
			{
				int aiStrategy = -1;
				if (newMap != null && newMap.teamAiType != null && i < newMap.teamAiType.Count)
				{
					aiStrategy = newMap.teamAiType[i];
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
				Debug.Log("[MapSwitch] SwitchToInsideMap: 重新加入 AI 队伍 " + (TEAM)i + ", 策略=" + aiStrategy);
			}
		}
		this.aiManager.Start(1f);

		// 恢复游戏状态（CreateScene可能会重置某些状态）
		Debug.Log("[MapSwitch] SwitchToInsideMap: CreateScene完成，恢复游戏状态");
		Solarmax.Singleton<BattleSystem>.Instance.SetPause(false);
		this.SetInteractionEnabled(true);


		// 切换音乐：先解除静音，再播放新音乐
		Debug.Log("[MapSwitch] SwitchToInsideMap: 播放新音乐 " + insideMusicName);
		Solarmax.Singleton<AudioManger>.Get().MuteBGAudio(false);
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG(insideMusicName, 0.5f);

		// 地图切换后刷新战斗时间上限（合作模式按第二张地图的 mapconfig 重新计算）
		BaseWindow battleWindow = Solarmax.Singleton<UISystem>.Get().GetWindow("BattleWindow");
		if (battleWindow is BattleWindow)
		{
			((BattleWindow)battleWindow).RefreshBattleTime();
		}

		// 同步刷新战斗控制器的超时判定上限（否则判定仍沿用第一张地图的时限，UI 却显示新地图时限，导致提前超时失败）
		if (Solarmax.Singleton<BattleSystem>.Instance.pvpBattleController != null)
		{
			Solarmax.Singleton<BattleSystem>.Instance.pvpBattleController.RefreshBattleTimeMax();
			Solarmax.Singleton<BattleSystem>.Instance.pvpBattleController.ResetBattleSpeed();
		}
	}

	/// <summary>
	/// 渐回屏幕的协程
	/// </summary>
	private IEnumerator UnfadeScreenCoroutine()
	{
		Debug.Log("[MapSwitch] UnfadeScreenCoroutine: 开始渐回");

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

		// 清理白色遮罩对象
		if (this.whiteOverlay != null)
		{
			Debug.Log("[MapSwitch] UnfadeScreenCoroutine: 销毁白色遮罩");
			GameObject.Destroy(this.whiteOverlay);
			this.whiteOverlay = null;
		}

		this.isFading = false;

		// 重置地图切换状态，允许下次触发
		this.mapSwitched = false;
		Debug.Log("[MapSwitch] UnfadeScreenCoroutine: 重置 mapSwitched=false，恢复游戏");

		// 恢复游戏 (在渐回完成后)
		Solarmax.Singleton<BattleSystem>.Instance.SetPause(false);
		this.SetInteractionEnabled(true);
	}

	/// <summary>
	/// 更新白色遮罩层
	/// </summary>
	private void UpdateWhiteOverlay(float alpha)
	{
		// 天体贴图在 UI 图层（由 UI 摄像机渲染），所以白屏也要放进 UI 摄像机，才能盖住天体。
		// renderQueue=3001：高于天体 SpriteRenderer(3000)，低于 HUD 的 NGUI drawCall(3002+)。
		if (this.whiteOverlay == null)
		{
			UIRoot uiRoot = Solarmax.Singleton<UISystem>.Get().GetNGUIRoot();
			Transform uiCameraTransform = (uiRoot != null) ? uiRoot.transform.Find("Camera") : null;
			Camera camera = (uiCameraTransform != null) ? uiCameraTransform.GetComponent<Camera>() : null;
			if (camera == null)
			{
				Debug.LogError("[MapSwitch] 找不到 UI 摄像机，无法创建白色遮罩");
				return;
			}

			GameObject overlayObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
			overlayObj.name = "MapSwitchWhiteOverlay";
			overlayObj.layer = uiRoot.gameObject.layer; // UI 图层

			// 挂到 UI 根节点下，使用 UI 坐标系（与 UI 元素同层）
			overlayObj.transform.SetParent(uiRoot.transform, false);
			overlayObj.transform.localRotation = Quaternion.identity;
			overlayObj.transform.localPosition = Vector3.zero;

			Renderer renderer = overlayObj.GetComponent<Renderer>();
			if (renderer != null)
			{
				renderer.material = new Material(Shader.Find("Sprites/Default"));
				renderer.material.color = new Color(1f, 1f, 1f, alpha);
				renderer.material.renderQueue = 3001; // 天体(3000)之上、HUD(3002+)之下
				renderer.sortingLayerName = "Default";
				renderer.sortingOrder = 0;
			}

			// 移除碰撞体，避免干扰射线检测
			Collider collider = overlayObj.GetComponent<Collider>();
			if (collider != null)
			{
				UnityEngine.Object.Destroy(collider);
			}

			// 铺满全屏：用 UI 根节点的虚拟高度（activeHeight）换算，避免被 UI Root 的缩放缩小到看不见
			float uiHeight = uiRoot.activeHeight;
			float uiWidth = uiHeight * camera.aspect;
			overlayObj.transform.localScale = new Vector3(uiWidth, uiHeight, 1f);

			this.whiteOverlay = overlayObj;

			Debug.Log("[MapSwitch] 创建白色遮罩: UI图层 renderQueue=3001（天体之上、HUD之下）");
		}
		else
		{
			Renderer renderer = this.whiteOverlay.GetComponent<Renderer>();
			if (renderer != null && renderer.material != null)
			{
				// 确保每次更新时透明度都正确
				renderer.material.color = new Color(1f, 1f, 1f, Mathf.Clamp01(alpha));

				// 如果透明度为0，禁用渲染器以避免不必要的渲染
				renderer.enabled = alpha > 0f;
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

	#endregion
}

