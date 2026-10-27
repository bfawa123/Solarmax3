using System;
using System.Collections.Generic;
using System.Reflection;
using Solarmax;
using UnityEngine;

public class FriendSmartAILogic : BaseAILogic
{
	public override void Init(Team t, int aiStrategy, int level, int Difficulty, float Tick = 1f)
	{
		AIData aiData = base.GetAiData(t, true);
		aiData.Reset();
		aiData.aiStrategy = aiStrategy;
		aiData.aiTimeInterval = 3f;
		aiData.actionTime = 5f;
		aiData.actionDelayDefend = 0.1f;
		aiData.actionDelayAttack = 0.1f;
		aiData.actionDelayGather = 0.1f;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.PVP)
		{
			this.actionTick = Tick;
		}
		else
		{
			this.actionTick = (float)Solarmax.Singleton<BattleSystem>.Instance.battleData.aiParam;
		}
		base.RegisterCallback(AIStatus.Idle, new BaseAILogic.OnAIStatusCallback(this.Idle));
		base.RegisterCallback(AIStatus.Defend, new BaseAILogic.OnAIStatusCallback(this.Defend));
		base.RegisterCallback(AIStatus.Attack, new BaseAILogic.OnAIStatusCallback(this.Attack));
		base.RegisterCallback(AIStatus.Gather, new BaseAILogic.OnAIStatusCallback(this.Gather));
		base.RegisterCallback(AIStatus.Defend_V1, new BaseAILogic.OnAIStatusCallback(this.DefendV1));
		base.RegisterCallback(AIStatus.Attack_V1, new BaseAILogic.OnAIStatusCallback(this.AttackV1));
		base.RegisterCallback(AIStatus.Defend_V2, new BaseAILogic.OnAIStatusCallback(this.DefendV2));
		base.RegisterCallback(AIStatus.Attack_V2, new BaseAILogic.OnAIStatusCallback(this.AttackV2));
		base.RegisterCallback(AIStatus.Gather_V1, new BaseAILogic.OnAIStatusCallback(this.GatherV1));
		base.RegisterCallback(AIStatus.Defend_V3, new BaseAILogic.OnAIStatusCallback(this.DefendV3));
		base.RegisterCallback(AIStatus.Attack_V3, new BaseAILogic.OnAIStatusCallback(this.AttackV3));
		base.RegisterCallback(AIStatus.Gather_V2, new BaseAILogic.OnAIStatusCallback(this.GatherV2));
		base.RegisterCallback(AIStatus.Attack_Low, new BaseAILogic.OnAIStatusCallback(this.Attack_Low));
		base.RegisterCallback(AIStatus.Defend_V4, new BaseAILogic.OnAIStatusCallback(this.DefendV4));
		base.RegisterCallback(AIStatus.Attack_V4, new BaseAILogic.OnAIStatusCallback(this.AttackV4));
		base.RegisterCallback(AIStatus.Gather_V3, new BaseAILogic.OnAIStatusCallback(this.GatherV3));
		base.RegisterCallback(AIStatus.vt1_AttackV4, new BaseAILogic.OnAIStatusCallback(this.vt1_AttackV4));
		base.RegisterCallback(AIStatus.vt1_DefendAdd, new BaseAILogic.OnAIStatusCallback(this.vt1_DefendAdd));
		base.RegisterCallback(AIStatus.vt1_DefendAdd_Beta, new BaseAILogic.OnAIStatusCallback(this.vt1_DefendAdd_Beta));
		base.RegisterCallback(AIStatus.vt1_GatherV3, new BaseAILogic.OnAIStatusCallback(this.vt1_GatherV3));
		this.teamActions[(int)t.team] = Solarmax.Singleton<AIStrategyConfigProvider>.Instance.GetAIActions(aiStrategy);
	}

	public override void Release(Team t)
	{
		base.GetAiData(t, false).Reset();
	}

	private bool Idle(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		aiData.aiStatus = AIStatus.Defend;
		return true;
	}

	private bool CalcTarget_Oldversion(Team t)
	{
		AIData aiData = base.GetAiData(t, false);
		Node node = null;
		int count = aiData.targetList.Count;
		int num = Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(1, 15);
		if (num >= 1 && num <= 9 && count >= 1)
		{
			node = aiData.targetList[0];
		}
		if (num > 9 && num <= 14 && count >= 2)
		{
			node = aiData.targetList[1];
		}
		if (num > 14 && num <= 15 && count >= 3)
		{
			node = aiData.targetList[2];
		}
		if (node == null && count >= 1)
		{
			node = aiData.targetList[0];
		}
		aiData.targetList.Clear();
		aiData.targetList.Add(node);
		return true;
	}

	private bool CalcTarget(Team t)
	{
		AIData aiData = base.GetAiData(t, false);
		List<Node> list = new List<Node>();
		for (int i = 0; i < aiData.targetList.Count; i++)
		{
			int j;
			for (j = 0; j < aiData.senderList.Count; j++)
			{
				if (aiData.senderList[j].AICanLink(aiData.targetList[i], t))
				{
					break;
				}
			}
			if (j < aiData.senderList.Count)
			{
				list.Add(aiData.targetList[i]);
			}
		}
		if (list.Count == 0)
		{
			aiData.targetList.Clear();
			return false;
		}
		int count = list.Count;
		int num = Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(1, 15);
		if (num >= 1 && num <= 9 && count >= 1)
		{
			Node value = list[0];
		}
		if (num > 9 && num <= 14 && count >= 2)
		{
			Node value = list[1];
			list[1] = list[0];
			list[0] = value;
		}
		if (num > 14 && num <= 15 && count >= 3)
		{
			Node value = list[2];
			list[2] = list[0];
			list[0] = value;
		}
		aiData.targetList.Clear();
		aiData.targetList = null;
		aiData.targetList = list;
		return true;
	}

	private bool CalcTargetV1(Team t)
	{
		AIData aiData = base.GetAiData(t, false);
		int count = aiData.targetList.Count;
		int num = Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(1, 15);
		if (num >= 1 && num <= 9 && count >= 1)
		{
			Node value = aiData.targetList[0];
		}
		if (num > 9 && num <= 14 && count >= 2)
		{
			Node value = aiData.targetList[1];
			aiData.targetList[1] = aiData.targetList[0];
			aiData.targetList[0] = value;
		}
		if (num > 14 && num <= 15 && count >= 3)
		{
			Node value = aiData.targetList[2];
			aiData.targetList[2] = aiData.targetList[0];
			aiData.targetList[0] = value;
		}
		return true;
	}

	private float CalcOccupiedTime(Node target, Team team, int count)
	{
		if (count <= 0)
		{
			return 50f;
		}
		int groupID = team.groupID;
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			if (i != (int)team.team && base.sceneManager.teamManager.GetTeam((TEAM)i).IsFriend(groupID))
			{
				count += target.numArray[i];
			}
		}
		float num = (-5000f / (float)(count + 100) + 50f) / (3f * target.entity.GetScale());
		num *= base.sceneManager.GetbattleScaleSpeed();
		if (num > 100f)
		{
			num = 100f;
		}
		num *= team.GetAttributeFloat(TeamAttr.OccupiedSpeed);
		num *= target.GetAttributeFloat(NodeAttr.OccupiedSpeed);
		if (target.type == NodeType.Tower || target.type == NodeType.Castle)
		{
			num *= 0.5f;
		}
		return target.hpMax / num;
	}

	private bool Defend(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.targetList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node.type != NodeType.Barrier && node.type != NodeType.Curse && node.IsOurNode(t.team))
			{
				int num = node.TeamStrength(t.team, true);
				int num2 = node.OppStrength(t.team);
				if (num2 != 0 && (num <= 5 || num <= num2 * 2))
				{
					float magnitude = (node.GetPosition() - b).magnitude;
					float num3 = (float)(num2 - num);
					node.aiValue = magnitude + num3;
					aiData.targetList.Add(node);
				}
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
		aiData.senderList.Clear();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node2 = usefulNodeList[j];
			if ((so == null || node2 == so) && node2.aiTimers[(int)t.team] <= 0f && (node2.aistrategy < 0 || so != null) && node2.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				float num4 = (float)node2.TeamStrength(t.team, false);
				if (num4 >= 5f && (node2.IsOurNode(t.team) || node2.TeamStrength(t.team, true) <= node2.OppStrength(t.team)))
				{
					node2.aiStrength = -num4;
					aiData.senderList.Add(node2);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		this.CalcTarget(t);
		for (int k = 0; k < aiData.targetList.Count; k++)
		{
			Node node3 = aiData.targetList[k];
			for (int l = 0; l < aiData.senderList.Count; l++)
			{
				Node node4 = aiData.senderList[l];
				if (node4 != node3 && node4.AICanLink(node3, t) && node4.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true) > node3.OppStrength(t.team))
				{
					int num5 = node3.OppStrength(t.team) * 2 - node3.TeamStrength(t.team, true);
					int num6 = (int)(this.GetLengthInTowerRange(t, node4, node3) * 3.3f + 0.5f);
					if (num6 <= 0 || node4.TeamStrength(t.team, false) >= num6)
					{
						num5 += num6;
						LoggerSystem.CodeComments("代码修改-24.12.05-余音回响1.0.2.3-疑似与AI回防特性有关--num5<5 --> num5<0");
						if (num5 < 0)
						{
							num5 = node4.TeamStrength(t.team, false);
						}
						node4.AIMoveTo(t.team, node3, num5);
						node4.ResetAiTimer(t.team, aiData.aiTimeInterval);
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool Attack(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.targetList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
                        if (node.CanBeAITarget(t.team) && !node.IsOurNode(t.team) && (node.team != TEAM.Neutral || node.TeamStrength(t.team, true) <= 5 || node.TeamStrength(t.team, true) <= node.OppStrength(t.team) * 2))
			{
				float magnitude = (node.GetPosition() - b).magnitude;
				float num = (float)(node.OppStrength(t.team) - node.TeamStrength(t.team, true));
				node.aiValue = magnitude + num;
				aiData.targetList.Add(node);
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
		aiData.senderList.Clear();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node2 = usefulNodeList[j];
			if ((so == null || node2 == so) && node2.aiTimers[(int)t.team] <= 0f && (node2.aistrategy < 0 || so != null) && node2.GetAttributeInt(NodeAttr.Ice) <= 0 && node2.IsOurNode(t.team))
			{
				int num2 = node2.OppStrength(t.team);
				if ((num2 != 0 || node2.state != NodeState.Capturing) && node2.TeamStrength(t.team, false) >= 5)
				{
					int num3 = node2.TeamStrength(t.team, true);
					if (num2 <= 0 || num3 <= num2)
					{
						node2.aiStrength = -(float)node2.TeamStrength(t.team, false);
						aiData.senderList.Add(node2);
					}
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		this.CalcTarget(t);
		bool flag = false;
		int num4 = 0;
		List<int> list = new List<int>();
		for (int k = 0; k < aiData.targetList.Count; k++)
		{
			Node node3 = aiData.targetList[k];
			for (int l = 0; l < aiData.senderList.Count; l++)
			{
				Node node4 = aiData.senderList[l];
				if (node4 != node3 && node4.AICanLink(node3, t))
				{
					if (node4.TeamStrength(t.team, false) + node3.TeamStrength(t.team, true) > node3.OppStrength(t.team))
					{
						int num5 = (int)((float)(node3.OppStrength(t.team) * 2) - ((float)node3.TeamStrength(t.team, true) * 0.5f + 0.5f));
						int num6 = (int)(this.GetLengthInTowerRange(t, node4, node3) * 3.3f + 0.5f);
						if (num6 > 0 && node4.TeamStrength(t.team, false) < num6)
						{
							goto IL_3A8;
						}
						num5 += num6;
						if (node4.OppStrength(t.team) == 0 || node4.OppStrength(t.team) > node4.TeamStrength(t.team, true))
						{
							num5 = node4.TeamStrength(t.team, false);
						}
						node4.AIMoveTo(t.team, node3, num5);
						node4.ResetAICalculateCache();
						node4.ResetAiTimer(t.team, aiData.aiTimeInterval);
						flag = true;
					}
					num4 += node4.TeamStrength(t.team, false);
					list.Add(l);
				}
				IL_3A8:;
			}
		}
		if (flag)
		{
			return true;
		}
		for (int m = 0; m < aiData.targetList.Count; m++)
		{
			Node node5 = aiData.targetList[m];
			if (num4 + node5.TeamStrength(t.team, true) > node5.OppStrength(t.team))
			{
				for (int n = 0; n < list.Count; n++)
				{
					Node node6 = aiData.senderList[list[n]];
					if (node6 != node5 && node6.AICanLink(node5, t))
					{
						int num7 = (int)((float)(node5.OppStrength(t.team) * 2) - ((float)node5.TeamStrength(t.team, true) * 0.5f + 0.5f));
						int num8 = (int)(this.GetLengthInTowerRange(t, node6, node5) * 3.3f + 0.5f);
						if (num8 <= 0 || node6.TeamStrength(t.team, false) >= num8)
						{
							num7 += num8;
							if (node6.OppStrength(t.team) == 0 || node6.OppStrength(t.team) > node6.TeamStrength(t.team, true))
							{
								num7 = node6.TeamStrength(t.team, false);
							}
							node6.AIMoveTo(t.team, node5, num7);
							node6.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
				return true;
			}
		}
		return false;
	}

	private bool Gather(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null))
			{
				LoggerSystem.CodeComments("代码修改-24.12.05-余音回响1.0.2.3-疑似影响AI聚兵飞船下限，同时影响AI的撤退--  >=10 --> >=5");
				if (node.TeamStrength(t.team, false) >= 5 && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && (node.OppStrength(t.team) <= 0 || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
				{
					node.aiStrength = -(float)node.TeamStrength(t.team, false) - (float)node.OppStrength(t.team);
					node.aiValue = -(float)node.GetOppLinks(t, usefulNodeList);
					if (node.type == NodeType.WarpDoor)
					{
						node.aiValue -= 1f;
						node.aiValue += node.aiStrength;
					}
					aiData.senderList.Add(node);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		aiData.targetList.Clear();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node2 = usefulNodeList[j];
                        if (node2.CanBeAITarget(t.team) && node2.IsOurNode(t.team))
			{
				node2.aiStrength = -(float)node2.TeamStrength(t.team, false) - (float)node2.OppStrength(t.team);
				node2.aiValue = -(float)node2.GetOppLinks(t, usefulNodeList);
				if (node2.type == NodeType.WarpDoor)
				{
					node2.aiValue -= 1f;
					node2.aiValue += node2.aiStrength;
				}
				aiData.targetList.Add(node2);
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparsionAIValue));
		this.CalcTarget(t);
		for (int k = 0; k < aiData.senderList.Count; k++)
		{
			Node node3 = aiData.senderList[k];
			for (int l = 0; l < aiData.targetList.Count; l++)
			{
				Node node4 = aiData.targetList[l];
				if (node3 != node4 && node3.AICanLink(node4, t) && node4.aiValue <= node3.aiValue)
				{
					int num = node3.TeamStrength(t.team, false);
					int num2 = (int)(this.GetLengthInTowerRange(t, node3, node4) * 3.3f + 0.5f);
					num += num2;
					if (num2 <= 0 || (float)node3.TeamStrength(t.team, false) >= (float)num2 * 0.5f)
					{
						node3.AIMoveTo(t.team, node4, num);
						node3.ResetAiTimer(t.team, aiData.aiTimeInterval);
						return true;
					}
				}
			}
		}
		return false;
	}

	public float GetLengthInTowerRange(Team t, Node arg0, Node arg1)
	{
		float num = 0f;
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		int num2 = 1;
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node.team != t.team)
			{
				if (node.type == NodeType.Tower || node.type == NodeType.Castle)
				{
					if (!node.IsOurNode(t.team) && node.team != TEAM.Neutral)
					{
						Vector3 position = arg0.GetPosition();
						Vector3 position2 = arg1.GetPosition();
						Vector3 position3 = node.GetPosition();
						this.LineIntersectCircle(t, position, position2, position3, node.GetWidth() * node.GetAttackRage());
						AIData aiData = base.GetAiData(t, false);
						if (aiData.resultIntersects)
						{
							if (aiData.resultEnter != Vector3.forward && aiData.resultExit != Vector3.forward)
							{
								num += Vector3.Distance(aiData.resultEnter, aiData.resultExit) * (float)num2;
							}
							else if (aiData.resultEnter != Vector3.forward && aiData.resultExit == Vector3.forward)
							{
								num += Vector3.Distance(aiData.resultEnter, position2) * (float)num2;
							}
							else if (aiData.resultEnter == Vector3.forward && aiData.resultExit != Vector3.forward)
							{
								num += Vector3.Distance(position, aiData.resultExit) * (float)num2;
							}
							else
							{
								num += Vector3.Distance(position, position2) * (float)num2;
							}
							num2++;
						}
						else if (aiData.resultInside)
						{
							num += Vector3.Distance(position, position2) * (float)num2;
							num2++;
						}
					}
				}
			}
		}
		return num;
	}

	public int GetLengthInTowerRangeV1(Team t, Node arg0, Node arg1)
	{
		int num = 0;
		if (arg0.type == NodeType.WarpDoor)
		{
			return 0;
		}
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node.team != t.team)
			{
				if (node.unitLost > 0)
				{
					if (!node.IsOurNode(t.team) && node.team != TEAM.Neutral)
					{
						Vector3 position = arg0.GetPosition();
						Vector3 position2 = arg1.GetPosition();
						Vector3 position3 = node.GetPosition();
						this.LineIntersectCircle(t, position, position2, position3, node.GetWidth() * node.GetAttackRage());
						AIData aiData = base.GetAiData(t, false);
						if (aiData.resultIntersects || aiData.resultInside)
						{
							num += node.unitLost;
						}
					}
				}
			}
		}
		return num;
	}

	public void LineIntersectCircle(Team t, Vector3 A, Vector3 B, Vector3 C, float r)
	{
		AIData aiData = base.GetAiData(t, false);
		aiData.resultInside = false;
		aiData.resultIntersects = false;
		aiData.resultEnter = Vector3.forward;
		aiData.resultExit = Vector3.forward;
		float num = (B.x - A.x) * (B.x - A.x) + (B.y - A.y) * (B.y - A.y);
		float num2 = 2f * ((B.x - A.x) * (A.x - C.x) + (B.y - A.y) * (A.y - C.y));
		float num3 = C.x * C.x + C.y * C.y + A.x * A.x + A.y * A.y - 2f * (C.x * A.x + C.y * A.y) - r * r;
		float num4 = num2 * num2 - 4f * num * num3;
		if (num4 <= 0f)
		{
			aiData.resultInside = false;
		}
		else
		{
			float num5 = Mathf.Sqrt(num4);
			float num6 = (-num2 + num5) / (2f * num);
			float num7 = (-num2 - num5) / (2f * num);
			if ((num6 < 0f || num6 > 1f) && (num7 < 0f || num7 > 1f))
			{
				if ((num6 < 0f && num7 < 0f) || (num6 > 1f && num7 > 1f))
				{
					aiData.resultInside = false;
				}
				else
				{
					aiData.resultInside = true;
				}
			}
			else
			{
				if (0f <= num7 && num7 <= 1f)
				{
					aiData.resultEnter = Vector3.Lerp(A, B, 1f - num7);
				}
				if (0f <= num6 && num6 <= 1f)
				{
					aiData.resultExit = Vector3.Lerp(A, B, 1f - num6);
				}
				aiData.resultIntersects = true;
			}
		}
	}

	private bool DefendV1(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				float num = (float)node.TeamStrength(t.team, false);
				if (num >= 5f && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)))
				{
					node.aiStrength = -num;
					aiData.senderList.Add(node);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
				if (node3.type != NodeType.Barrier && node3.type != NodeType.Curse && node2 != node3 && node3.IsOurNode(t.team) && node3.currentTeam != null && node2.AICanLink(node3, t))
				{
					int num2 = node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true);
					int num3 = node3.OppStrength(t.team);
					if (num3 != 0 && (num2 <= 5 || num2 <= num3 * 2))
					{
						float num4 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
						if (node2.type == NodeType.WarpDoor)
						{
							num4 /= 100f;
						}
						float num5 = this.CalcOccupiedTime(node3, t, num2 - num3);
						node3.aiValue = (num4 + num5) / (float)node3.weight;
						aiData.targetList.Add(node3);
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, true) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num6 = node4.OppStrength(t.team) * 2 - node4.TeamStrength(t.team, true);
						int num7 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
						if (num7 <= 0 || node2.TeamStrength(t.team, false) >= num7)
						{
							num6 += num7;
							if (num6 < 5)
							{
								num6 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num6);
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		return false;
	}

	private bool AttackV1(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0 && node.IsOurNode(t.team))
			{
				int num = node.OppStrength(t.team);
				if ((num != 0 || node.state != NodeState.Capturing) && node.TeamStrength(t.team, false) >= 5)
				{
					int num2 = node.TeamStrength(t.team, true);
					if (num <= 0 || num2 <= num)
					{
						node.aiStrength = -(float)node.TeamStrength(t.team, false);
						aiData.senderList.Add(node);
					}
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && !node3.IsOurNode(t.team) && node2.AICanLink(node3, t))
				{
					LoggerSystem.CodeComments("代码修改-24.12.05-余音回响1.0.2.3-疑似影响AI对中立天体的进攻--  <=5 --> <=45");
					if (node3.team != TEAM.Neutral || node3.TeamStrength(t.team, true) <= 45 || node3.TeamStrength(t.team, true) <= node3.OppStrength(t.team) * 2)
					{
						float num3 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
						if (node2.type == NodeType.WarpDoor)
						{
							num3 /= 100f;
						}
						float num4 = this.CalcOccupiedTime(node3, t, node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true) - node3.OppStrength(t.team));
						node3.aiValue = (num3 + num4) / (float)node3.weight;
						aiData.targetList.Add(node3);
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num5 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f + 0.5f);
						int num6 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
						if (num6 <= 0 || node2.TeamStrength(t.team, false) >= num6)
						{
							num5 += num6;
							if (node2.OppStrength(t.team) == 0 || node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
							{
								num5 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num5);
							node2.ResetAICalculateCache();
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		return false;
	}

	private bool DefendV2(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				float num = (float)node.TeamStrength(t.team, false);
				if (num >= 5f && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)))
				{
					node.aiStrength = -num;
					aiData.senderList.Add(node);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
				if (node3.type != NodeType.Barrier && node3.type != NodeType.Curse && node2 != node3 && node3.IsOurNode(t.team) && node3.currentTeam != null && node2.AICanLink(node3, t))
				{
					int num2 = node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true);
					int num3 = node3.OppStrength(t.team);
					if (num3 != 0 && (num2 <= 5 || num2 <= num3 * 2))
					{
						int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
						if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
						{
							float num4 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
							if (node2.type == NodeType.WarpDoor)
							{
								num4 /= 100f;
							}
							float num5 = this.CalcOccupiedTime(node3, t, num2 - num3 - lengthInTowerRangeV);
							node3.aiValue = (num4 + num5) / (float)node3.weight;
							aiData.targetList.Add(node3);
						}
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, true) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num6 = node4.OppStrength(t.team) * 2 - node4.TeamStrength(t.team, true);
						int num7 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
						if (num7 <= 0 || node2.TeamStrength(t.team, false) >= num7)
						{
							num6 += num7;
							if (num6 < 5)
							{
								num6 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num6);
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		return false;
	}

	private bool AttackV2(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0 && node.IsOurNode(t.team))
			{
				int num = node.OppStrength(t.team);
				if ((num != 0 || node.state != NodeState.Capturing) && node.TeamStrength(t.team, false) >= 5)
				{
					int num2 = node.TeamStrength(t.team, true);
					if (num <= 0 || num2 <= num)
					{
						node.aiStrength = -(float)node.TeamStrength(t.team, false);
						aiData.senderList.Add(node);
					}
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && !node3.IsOurNode(t.team) && node2.AICanLink(node3, t))
				{
					LoggerSystem.CodeComments("代码修改-24.12.05-余音回响1.0.2.3-疑似影响AI对中立天体的占领--  <=5 --> <=45");
					if (node3.team != TEAM.Neutral || node3.TeamStrength(t.team, true) <= 45 || node3.TeamStrength(t.team, true) <= node3.OppStrength(t.team) * 2)
					{
						int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
						if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
						{
							float num3 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
							if (node2.type == NodeType.WarpDoor)
							{
								num3 /= 100f;
							}
							float num4 = this.CalcOccupiedTime(node3, t, node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true) - node3.OppStrength(t.team) - lengthInTowerRangeV);
							node3.aiValue = (num3 + num4) / (float)node3.weight;
							aiData.targetList.Add(node3);
						}
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num5 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f + 0.5f);
						int num6 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
						if (num6 <= 0 || node2.TeamStrength(t.team, false) >= num6)
						{
							num5 += num6;
							if (node2.OppStrength(t.team) == 0 || node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
							{
								num5 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num5);
							node2.ResetAICalculateCache();
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		return false;
	}

	private bool GatherV1(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null))
			{
				LoggerSystem.CodeComments("代码修改-24.12.05-余音回响1.0.2.3-疑似影响AI聚兵飞船下限，同时影响AI的撤退--  >=10 --> >=5");
				if (node.TeamStrength(t.team, false) >= 5 && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && (node.OppStrength(t.team) <= 0 || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
				{
					node.aiStrength = -(float)node.TeamStrength(t.team, false) - (float)node.OppStrength(t.team);
					node.aiValue = -(float)node.GetOppLinks(t, usefulNodeList);
					if (node.type == NodeType.WarpDoor)
					{
						node.aiValue -= 1f;
						node.aiValue += node.aiStrength;
					}
					aiData.senderList.Add(node);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		aiData.targetList.Clear();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node2 = usefulNodeList[j];
                        if (node2.CanBeAITarget(t.team) && node2.IsOurNode(t.team))
			{
				node2.aiStrength = -(float)node2.TeamStrength(t.team, false) - (float)node2.OppStrength(t.team);
				node2.aiValue = -(float)node2.GetOppLinks(t, usefulNodeList);
				if (node2.type == NodeType.WarpDoor)
				{
					node2.aiValue -= 1f;
					node2.aiValue += node2.aiStrength;
				}
				aiData.targetList.Add(node2);
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparsionAIValue));
		this.CalcTarget(t);
		for (int k = 0; k < aiData.senderList.Count; k++)
		{
			Node node3 = aiData.senderList[k];
			for (int l = 0; l < aiData.targetList.Count; l++)
			{
				Node node4 = aiData.targetList[l];
				if (node3 != node4 && node3.AICanLink(node4, t) && node4.aiValue <= node3.aiValue)
				{
					int num = node3.TeamStrength(t.team, false);
					int num2 = (int)((float)this.GetLengthInTowerRangeV1(t, node3, node4) * 2f);
					num += num2;
					if (num2 <= 0 || (float)node3.TeamStrength(t.team, false) >= (float)num2 * 0.5f)
					{
						node3.AIMoveTo(t.team, node4, num);
						node3.ResetAiTimer(t.team, aiData.aiTimeInterval);
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool DefendV3(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				float num = (float)node.TeamStrength(t.team, false);
				if (num >= 1f && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)))
				{
					node.aiStrength = -num;
					aiData.senderList.Add(node);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int current = node2.currentTeam.current;
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
				if (node3.type != NodeType.Barrier && node3.type != NodeType.Curse && node2 != node3 && node3.IsOurNode(t.team) && node3.currentTeam != null && node2.AICanLink(node3, t))
				{
					int num2 = node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true);
					int num3 = node3.OppStrength(t.team);
					if (num3 != 0 && (num2 <= 5 || num2 <= num3 * 2))
					{
						int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
						if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
						{
							int current2 = node3.currentTeam.current;
							float num4 = 1f;
							if (current2 > current)
							{
								num4 = (float)current2 * 2f / (float)current;
							}
							float num5 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
							if (node2.type == NodeType.WarpDoor)
							{
								num5 /= 100f;
							}
							float num6 = this.CalcOccupiedTime(node3, t, num2 - num3 - lengthInTowerRangeV);
							node3.aiValue = (num5 + num6) / (float)node3.weight / num4;
							aiData.targetList.Add(node3);
						}
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, true) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num7 = node4.OppStrength(t.team) * 2 - node4.TeamStrength(t.team, true);
						int num8 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
						if (num8 <= 0 || node2.TeamStrength(t.team, false) >= num8)
						{
							num7 += num8;
							if (num7 < 5)
							{
								num7 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num7);
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		return false;
	}

	private bool AttackV3(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0 && node.IsOurNode(t.team))
			{
				int num = node.OppStrength(t.team);
				if ((num != 0 || node.state != NodeState.Capturing) && node.TeamStrength(t.team, false) >= 1)
				{
					int num2 = node.TeamStrength(t.team, true);
					if (num <= 0 || num2 <= num)
					{
						node.aiStrength = -(float)node.TeamStrength(t.team, false);
						aiData.senderList.Add(node);
					}
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int current = node2.currentTeam.current;
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && (!node3.IsOurNode(t.team) || node3.state == NodeState.Capturing) && node2.AICanLink(node3, t))
				{
					LoggerSystem.CodeComments("代码修改-24.12.05-余音回响1.0.2.3-疑似影响AI对中立天体的占领--  <=5 --> <=45");
					if (node3.team != TEAM.Neutral || node3.TeamStrength(t.team, true) <= 45 || node3.TeamStrength(t.team, true) <= node3.OppStrength(t.team) * 2)
					{
						int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
						if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
						{
							int current2 = node3.currentTeam.current;
							float num3 = 1f;
							if (current2 > current)
							{
								num3 = (float)current2 * 2f / (float)current;
							}
							float num4 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
							if (node2.type == NodeType.WarpDoor)
							{
								num4 /= 100f;
							}
							float num5 = this.CalcOccupiedTime(node3, t, node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true) - node3.OppStrength(t.team) - lengthInTowerRangeV);
							node3.aiValue = (num4 + num5) / (float)node3.weight / num3;
							aiData.targetList.Add(node3);
						}
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num6 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f + 0.5f);
						int num7 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
						if (num7 <= 0 || node2.TeamStrength(t.team, false) >= num7)
						{
							num6 += num7;
							if (node2.OppStrength(t.team) == 0 || node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
							{
								num6 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num6);
							node2.ResetAICalculateCache();
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		return false;
	}

	private bool GatherV2(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null))
			{
				LoggerSystem.CodeComments("代码修改-24.12.05-余音回响1.0.2.3-疑似影响AI聚兵飞船下限，同时影响AI的撤退--  >=10 --> >=5");
				if (node.TeamStrength(t.team, false) >= 5 && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && (node.OppStrength(t.team) <= 0 || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
				{
					node.aiStrength = -(float)node.TeamStrength(t.team, false) - (float)node.OppStrength(t.team);
					node.aiValue = -(float)node.GetOppLinks(t, usefulNodeList);
					if (node.type == NodeType.WarpDoor)
					{
						node.aiValue -= 1f;
						node.aiValue += node.aiStrength;
					}
					aiData.senderList.Add(node);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int attributeBaseInt = node2.currentTeam.GetAttributeBaseInt(TeamAttr.Population);
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && (!node3.IsOurNode(t.team) || node3.state == NodeState.Capturing) && node2.AICanLink(node3, t) && (node3.team != TEAM.Neutral || node3.TeamStrength(t.team, true) <= 5 || node3.TeamStrength(t.team, true) <= node3.OppStrength(t.team) * 2))
				{
					int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
					if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
					{
						int attributeBaseInt2 = node3.currentTeam.GetAttributeBaseInt(TeamAttr.Population);
						float num = 1f;
						if (attributeBaseInt2 > attributeBaseInt)
						{
							num = (float)attributeBaseInt2 / (float)attributeBaseInt;
						}
						float num2 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
						if (node2.type == NodeType.WarpDoor)
						{
							num2 /= 100f;
						}
						float num3 = this.CalcOccupiedTime(node3, t, node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true) - node3.OppStrength(t.team) - lengthInTowerRangeV);
						node3.aiValue = (num2 + num3) / (float)node3.weight / num;
						aiData.targetList.Add(node3);
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num4 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f);
						int num5 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f);
						if (num5 <= 0 || node2.TeamStrength(t.team, false) >= num5)
						{
							num4 += num5;
							if (node2.OppStrength(t.team) == 0 || node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
							{
								num4 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num4);
							node2.ResetAICalculateCache();
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		aiData.targetList.Clear();
		for (int m = 0; m < usefulNodeList.Count; m++)
		{
			Node node5 = usefulNodeList[m];
                        if (node5.CanBeAITarget(t.team) && node5.IsOurNode(t.team))
			{
				node5.aiStrength = -(float)node5.TeamStrength(t.team, false) - (float)node5.OppStrength(t.team);
				node5.aiValue = -(float)node5.GetOppLinks(t, usefulNodeList);
				if (node5.type == NodeType.WarpDoor)
				{
					node5.aiValue -= 1f;
					node5.aiValue += node5.aiStrength;
				}
				aiData.targetList.Add(node5);
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparsionAIValue));
		this.CalcTarget(t);
		for (int n = 0; n < aiData.senderList.Count; n++)
		{
			Node node6 = aiData.senderList[n];
			for (int num6 = 0; num6 < aiData.targetList.Count; num6++)
			{
				Node node7 = aiData.targetList[num6];
				if (node6 != node7 && node6.AICanLink(node7, t) && node7.aiValue <= node6.aiValue)
				{
					int num7 = node6.TeamStrength(t.team, false);
					int num8 = (int)((float)this.GetLengthInTowerRangeV1(t, node6, node7) * 2f);
					num7 += num8;
					if (num8 <= 0 || (float)node6.TeamStrength(t.team, false) >= (float)num8 * 0.5f)
					{
						node6.AIMoveTo(t.team, node7, num7);
						node6.ResetAiTimer(t.team, aiData.aiTimeInterval);
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool Attack_Low(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.targetList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
                        if (node.CanBeAITarget(t.team) && !node.IsOurNode(t.team) && (node.team != TEAM.Neutral || node.TeamStrength(t.team, true) <= 5 || node.TeamStrength(t.team, true) <= node.OppStrength(t.team) * 2))
			{
				float magnitude = (node.GetPosition() - b).magnitude;
				float num = (float)(node.OppStrength(t.team) - node.TeamStrength(t.team, true));
				node.aiValue = magnitude + num;
				aiData.targetList.Add(node);
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
		aiData.senderList.Clear();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node2 = usefulNodeList[j];
			if ((so == null || node2 == so) && node2.aiTimers[(int)t.team] <= 0f && (node2.aistrategy < 0 || so != null) && node2.GetAttributeInt(NodeAttr.Ice) <= 0 && node2.IsOurNode(t.team))
			{
				int num2 = node2.OppStrength(t.team);
				if ((num2 != 0 || node2.state != NodeState.Capturing) && node2.TeamStrength(t.team, false) >= 5)
				{
					int num3 = node2.TeamStrength(t.team, true);
					if (num2 <= 0 || num3 <= num2)
					{
						node2.aiStrength = -(float)node2.TeamStrength(t.team, false);
						aiData.senderList.Add(node2);
					}
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		this.CalcTarget(t);
		bool flag = false;
		int num4 = 0;
		List<int> list = new List<int>();
		for (int k = 0; k < aiData.targetList.Count; k++)
		{
			Node node3 = aiData.targetList[k];
			for (int l = 0; l < aiData.senderList.Count; l++)
			{
				Node node4 = aiData.senderList[l];
				if (node4 != node3 && node4.AICanLink(node3, t))
				{
					if (node4.TeamStrength(t.team, false) + node3.TeamStrength(t.team, true) > node3.OppStrength(t.team))
					{
						int num5 = (int)((float)(node3.OppStrength(t.team) * 2) - ((float)node3.TeamStrength(t.team, true) * 0.5f + 0.5f));
						int num6 = (int)(this.GetLengthInTowerRange(t, node4, node3) * 3.3f + 0.5f);
						if (num6 <= 0 || node4.TeamStrength(t.team, false) >= num6)
						{
							num5 += num6;
							if (node4.OppStrength(t.team) == 0 || node4.OppStrength(t.team) > node4.TeamStrength(t.team, true))
							{
								num5 = node4.TeamStrength(t.team, false);
							}
							node4.AIMoveTo(t.team, node3, num5);
							node4.ResetAICalculateCache();
							node4.ResetAiTimer(t.team, aiData.aiTimeInterval);
							return true;
						}
					}
					else
					{
						num4 += node4.TeamStrength(t.team, false);
						list.Add(l);
					}
				}
			}
		}
		if (flag)
		{
			return true;
		}
		for (int m = 0; m < aiData.targetList.Count; m++)
		{
			Node node5 = aiData.targetList[m];
			if (num4 + node5.TeamStrength(t.team, true) > node5.OppStrength(t.team))
			{
				for (int n = 0; n < list.Count; n++)
				{
					Node node6 = aiData.senderList[list[n]];
					if (node6 != node5 && node6.AICanLink(node5, t))
					{
						int num7 = (int)((float)(node5.OppStrength(t.team) * 2) - ((float)node5.TeamStrength(t.team, true) * 0.5f + 0.5f));
						int num8 = (int)(this.GetLengthInTowerRange(t, node6, node5) * 3.3f + 0.5f);
						if (num8 <= 0 || node6.TeamStrength(t.team, false) >= num8)
						{
							num7 += num8;
							if (node6.OppStrength(t.team) == 0 || node6.OppStrength(t.team) > node6.TeamStrength(t.team, true))
							{
								num7 = node6.TeamStrength(t.team, false);
							}
							node6.AIMoveTo(t.team, node5, num7);
							node6.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
				return true;
			}
		}
		return false;
	}

	private bool DefendV4(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				float num = (float)node.TeamStrength(t.team, false);
				if (num >= 1f && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)))
				{
					node.aiStrength = -num;
					aiData.senderList.Add(node);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int current = node2.currentTeam.current;
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
				if (node3.type != NodeType.Barrier && node3.type != NodeType.Curse && node2 != node3 && node3.IsOurNode(t.team) && node3.currentTeam != null && node2.AICanLink(node3, t))
				{
					int num2 = node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true);
					int num3 = node3.OppStrength(t.team);
					if (num3 != 0 && (num2 <= 5 || num2 <= num3 * 2))
					{
						int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
						if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
						{
							int current2 = node3.currentTeam.current;
							float num4 = 1f;
							if (current2 > current)
							{
								num4 = (float)current2 * 2f / (float)current;
							}
							float num5 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
							if (node2.type == NodeType.WarpDoor)
							{
								num5 /= 100f;
							}
							float num6 = this.CalcOccupiedTime(node3, t, num2 - num3 - lengthInTowerRangeV);
							node3.aiValue = (num5 + num6) / (float)node3.weight / num4;
							aiData.targetList.Add(node3);
						}
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				int num7 = 0;
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (num7 >= 5)
					{
						break;
					}
					if (node2.TeamStrength(t.team, true) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						int num8 = node4.OppStrength(t.team) * 2 - node4.TeamStrength(t.team, true);
						int num9 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
						if (num9 <= 0 || node2.TeamStrength(t.team, false) >= num9)
						{
							num8 += num9;
							if (num8 < 0)
							{
								num8 = node2.TeamStrength(t.team, false);
							}
							node2.AIMoveTo(t.team, node4, num8);
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
							num7++;
						}
					}
				}
			}
		}
		return false;
	}

	private bool AttackV4(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0 && (node.IsOurNode(t.team) || node.OppStrength(t.team) >= node.TeamStrength(t.team, true)))
			{
				int num = node.OppStrength(t.team);
				if ((num != 0 || node.state != NodeState.Capturing) && node.TeamStrength(t.team, false) >= 1)
				{
					int num2 = node.TeamStrength(t.team, true);
					if (num <= 0 || num2 <= num)
					{
						node.aiStrength = -(float)node.TeamStrength(t.team, false);
						aiData.senderList.Add(node);
					}
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int current = node2.currentTeam.current;
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && (!node3.IsOurNode(t.team) || node3.state == NodeState.Capturing) && node2.AICanLink(node3, t) && (!node3.readyBomb || Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(1, 10) <= 1) && (!this.CrossCurseRange(node2, node3) || Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(1, 10) <= 1))
				{
					int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
					if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
					{
						int current2 = node3.currentTeam.current;
						float num3 = 1f;
						if (current2 > current)
						{
							num3 = (float)current2 * 2f / (float)current;
						}
						float num4 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
						if (node2.type == NodeType.WarpDoor)
						{
							num4 /= 100f;
						}
						float num5 = this.CalcOccupiedTime(node3, t, node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true) - node3.OppStrength(t.team) - lengthInTowerRangeV);
						node3.aiValue = (num4 + num5) / (float)node3.weight / num3;
						aiData.targetList.Add(node3);
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				int num6 = 0;
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (num6 >= 5)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						if (node2.type == NodeType.Diffusion)
						{
							int num7 = 0;
							if (node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
							{
								num7 = node2.TeamStrength(t.team, false);
							}
							else
							{
								int attackCount = node2.GetAttackCount();
								int num8 = node2.TeamStrength(t.team, false);
								if (t.current + num8 * (attackCount - 1) <= t.currentMax)
								{
									goto IL_5D3;
								}
								int num9 = (t.currentMax - t.current + (attackCount - 1)) / attackCount;
								int num10 = num8 - num9;
								if (num10 > 0)
								{
									num7 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f + 0.5f);
									int num11 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
									if (num11 > 0 && node2.TeamStrength(t.team, false) < num11)
									{
										goto IL_5D3;
									}
									num7 += num11;
									if (node2.OppStrength(t.team) == 0)
									{
										num7 = num10;
									}
									if (num7 > num10)
									{
										num7 = 0;
									}
								}
							}
							if (num7 > 0)
							{
								node2.AIMoveTo(t.team, node4, num7);
								node2.ResetAICalculateCache();
								node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
								num6++;
							}
						}
						else
						{
							int num12 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f + 0.5f);
							int num13 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
							if (num13 <= 0 || node2.TeamStrength(t.team, false) >= num13)
							{
								num12 += num13;
								if (node2.OppStrength(t.team) == 0 || node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
								{
									num12 = node2.TeamStrength(t.team, false);
								}
								node2.AIMoveTo(t.team, node4, num12);
								node2.ResetAICalculateCache();
								node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
								num6++;
							}
						}
					}
					IL_5D3:;
				}
			}
		}
		return false;
	}

	private bool GatherV3(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.TeamStrength(t.team, false) >= 10 && (node.IsOurNode(t.team) || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && (node.OppStrength(t.team) <= 0 || node.TeamStrength(t.team, true) <= node.OppStrength(t.team)) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				node.aiStrength = -(float)node.TeamStrength(t.team, false) - (float)node.OppStrength(t.team);
				node.aiValue = -(float)node.GetOppLinks(t, usefulNodeList);
				if (node.type == NodeType.WarpDoor)
				{
					node.aiValue -= 1f;
					node.aiValue += node.aiStrength;
				}
				aiData.senderList.Add(node);
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int attributeBaseInt = node2.currentTeam.GetAttributeBaseInt(TeamAttr.Population);
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && (!node3.IsOurNode(t.team) || node3.state == NodeState.Capturing) && node2.AICanLink(node3, t))
				{
					int lengthInTowerRangeV = this.GetLengthInTowerRangeV1(t, node2, node3);
					if (node2.TeamStrength(t.team, false) - node3.OppStrength(t.team) - lengthInTowerRangeV > 0)
					{
						int attributeBaseInt2 = node3.currentTeam.GetAttributeBaseInt(TeamAttr.Population);
						float num = 1f;
						if (attributeBaseInt2 > attributeBaseInt)
						{
							num = (float)attributeBaseInt2 / (float)attributeBaseInt;
						}
						float num2 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
						if (node2.type == NodeType.WarpDoor)
						{
							num2 /= 100f;
						}
						float num3 = this.CalcOccupiedTime(node3, t, node2.TeamStrength(t.team, true) + node3.TeamStrength(t.team, true) - node3.OppStrength(t.team) - lengthInTowerRangeV);
						node3.aiValue = (num2 + num3) / (float)node3.weight / num;
						aiData.targetList.Add(node3);
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				int num4 = 0;
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (num4 >= 5)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.TeamStrength(t.team, true) > node4.OppStrength(t.team))
					{
						if (node2.type == NodeType.Diffusion)
						{
							int num5 = 0;
							if (node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
							{
								num5 = node2.TeamStrength(t.team, false);
							}
							else
							{
								int attackCount = node2.GetAttackCount();
								int num6 = node2.TeamStrength(t.team, false);
								if (t.current + num6 * (attackCount - 1) <= t.currentMax)
								{
									goto IL_600;
								}
								int num7 = (t.currentMax - t.current + (attackCount - 1)) / attackCount;
								int num8 = num6 - num7;
								if (num8 > 0)
								{
									num5 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f + 0.5f);
									int num9 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
									if (num9 > 0 && node2.TeamStrength(t.team, false) < num9)
									{
										goto IL_600;
									}
									num5 += num9;
									if (node2.OppStrength(t.team) == 0)
									{
										num5 = num8;
									}
									if (num5 > num8)
									{
										num5 = 0;
									}
								}
							}
							if (num5 > 0)
							{
								node2.AIMoveTo(t.team, node4, num5);
								node2.ResetAICalculateCache();
								node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
								num4++;
							}
						}
						else
						{
							int num10 = (int)((float)(node4.OppStrength(t.team) * 2) - (float)node4.TeamStrength(t.team, true) * 0.5f + 0.5f);
							int num11 = (int)(this.GetLengthInTowerRange(t, node2, node4) * 3.3f + 0.5f);
							if (num11 <= 0 || node2.TeamStrength(t.team, false) >= num11)
							{
								num10 += num11;
								if (node2.OppStrength(t.team) == 0 || node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
								{
									num10 = node2.TeamStrength(t.team, false);
								}
								if (node2.OppStrength(t.team) > node2.TeamStrength(t.team, true))
								{
									num10 = node2.TeamStrength(t.team, false);
								}
								node2.AIMoveTo(t.team, node4, num10);
								node2.ResetAICalculateCache();
								node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
								num4++;
							}
						}
					}
					IL_600:;
				}
			}
		}
		aiData.targetList.Clear();
		for (int m = 0; m < usefulNodeList.Count; m++)
		{
			Node node5 = usefulNodeList[m];
                        if (node5.CanBeAITarget(t.team) && node5.IsOurNode(t.team))
			{
				node5.aiStrength = -(float)node5.TeamStrength(t.team, false) - (float)node5.OppStrength(t.team);
				node5.aiValue = -(float)node5.GetOppLinks(t, usefulNodeList);
				if (node5.type == NodeType.WarpDoor)
				{
					node5.aiValue -= 1f;
					node5.aiValue += node5.aiStrength;
				}
				aiData.targetList.Add(node5);
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparsionAIValue));
		this.CalcTarget(t);
		for (int n = 0; n < aiData.senderList.Count; n++)
		{
			Node node6 = aiData.senderList[n];
			for (int num12 = 0; num12 < aiData.targetList.Count; num12++)
			{
				Node node7 = aiData.targetList[num12];
				if (node6 != node7 && node6.AICanLink(node7, t) && node7.aiValue <= node6.aiValue)
				{
					int num13 = node6.TeamStrength(t.team, false);
					int num14 = (int)((float)this.GetLengthInTowerRangeV1(t, node6, node7) * 2f);
					num13 += num14;
					if (num14 <= 0 || (float)node6.TeamStrength(t.team, false) >= (float)num14 * 0.5f)
					{
						node6.AIMoveTo(t.team, node7, num13);
						node6.ResetAiTimer(t.team, aiData.aiTimeInterval);
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool CrossCurseRange(Node from, Node to)
	{
		List<Node> globalNodeList = base.sceneManager.nodeManager.GetGlobalNodeList();
		if (globalNodeList.Count == 0)
		{
			return false;
		}
		foreach (Node node in globalNodeList)
		{
			if (node.type == NodeType.Curse)
			{
				CurseNode curseNode = (CurseNode)node;
				if (curseNode.useSkill)
				{
					float num = 0f;
					float num2 = 0f;
					if (from.GetPosition().x - to.GetPosition().x > 1E-05f)
					{
						num = (from.GetPosition().y - to.GetPosition().y) / (from.GetPosition().x - to.GetPosition().x);
						num2 = from.GetPosition().y - num * from.GetPosition().x;
					}
					if (num == curseNode.skillK)
					{
						return false;
					}
					float num3 = 0f;
					float num4;
					if (num == 0f && num2 == 0f)
					{
						num4 = curseNode.skillB;
					}
					else
					{
						num3 = (num2 - curseNode.skillB) / (curseNode.skillK - num);
						num4 = num * num3 + num2;
					}
					float num5 = (from.GetPosition().x <= to.GetPosition().x) ? to.GetPosition().x : from.GetPosition().x;
					float num6 = (from.GetPosition().x >= to.GetPosition().x) ? to.GetPosition().x : from.GetPosition().x;
					float num7 = (from.GetPosition().y <= to.GetPosition().y) ? to.GetPosition().y : from.GetPosition().y;
					float num8 = (from.GetPosition().y >= to.GetPosition().y) ? to.GetPosition().y : from.GetPosition().y;
					if (num3 > num6 && num3 < num5 && num4 > num8 && num4 < num7)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void LineIntersectCircleV1(Team t, Vector3 A, Vector3 B, Vector3 C, float r)
	{
		AIData aiData = base.GetAiData(t, false);
		aiData.resultInside = false;
		aiData.resultIntersects = false;
		aiData.resultEnter = Vector3.forward;
		aiData.resultExit = Vector3.forward;
		float num = (B.x - A.x) * (B.x - A.x) + (B.y - A.y) * (B.y - A.y);
		float num2 = 2f * ((B.x - A.x) * (A.x - C.x) + (B.y - A.y) * (A.y - C.y));
		float num3 = C.x * C.x + C.y * C.y + A.x * A.x + A.y * A.y - 2f * (C.x * A.x + C.y * A.y) - r * r;
		float num4 = num2 * num2 - 4f * num * num3;
		if (num4 <= 0f)
		{
			aiData.resultInside = false;
			return;
		}
		float num5 = Mathf.Sqrt(num4);
		float num6 = (-num2 + num5) / (2f * num);
		float num7 = (-num2 - num5) / (2f * num);
		if ((num6 >= 0f && num6 <= 1f) || (num7 >= 0f && num7 <= 1f))
		{
			if (0f <= num6 && num6 <= 1f)
			{
				aiData.resultEnter = Vector3.Lerp(A, B, 1f - num6);
			}
			if (0f <= num7 && num7 <= 1f)
			{
				aiData.resultExit = Vector3.Lerp(A, B, 1f - num7);
			}
			aiData.resultIntersects = true;
			return;
		}
		if ((num6 < 0f && num7 < 0f) || (num6 > 1f && num7 > 1f))
		{
			aiData.resultInside = false;
			return;
		}
		aiData.resultInside = true;
	}

	public int GetLostInTowerRange(Team t, Node arg0, Node arg1)
	{
		float num = 0f;
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if (node.team != t.team && node.unitLost > 0 && !node.IsOurNode(t.team) && node.team != TEAM.Neutral)
			{
				Vector3 position = arg0.GetPosition();
				Vector3 b = arg1.GetPosition();
				Vector3 position2 = node.GetPosition();
				if (arg1.revoType != RevolutionType.RT_None)
				{
					float runTime = Vector3.Distance(position, b) / t.speed;
					b = arg1.GetNodeRunPosition(runTime);
					runTime = Vector3.Distance(position, b) / t.speed;
					b = arg1.GetNodeRunPosition(runTime);
				}
				this.LineIntersectCircleV1(t, position, b, position2, node.GetWidth() * node.GetAttackRage());
				float num2 = node.GetWidth() * node.GetAttackRage();
				AIData aiData = base.GetAiData(t, false);
				if (aiData.resultIntersects)
				{
					if (aiData.resultEnter != Vector3.forward && aiData.resultExit != Vector3.forward)
					{
						if (node.nodeType == NodeType.Defense)
						{
							float num3 = Vector3.Distance(position, position2) / t.speed;
							float num4 = node.GetWidth() * node.GetAttackRage() / t.speed;
							if ((node.isactive && node.act + num3 - num4 < 5f) || (!node.isactive && num3 + node.blackHoleCD >= 7.5f))
							{
								num += 10000f;
							}
						}
						num += Vector3.Distance(aiData.resultEnter, aiData.resultExit) / num2 * (float)node.unitLost;
					}
					else if (aiData.resultEnter != Vector3.forward && aiData.resultExit == Vector3.forward)
					{
						if (node.nodeType == NodeType.Defense)
						{
							float num5 = Vector3.Distance(position, position2) / t.speed;
							float num6 = node.GetWidth() * node.GetAttackRage() / t.speed;
							if ((node.isactive && node.act + num5 - num6 < 5f) || (!node.isactive && num5 + node.blackHoleCD >= 7.5f))
							{
								num += 10000f;
							}
						}
						num += Vector3.Distance(aiData.resultEnter, b) / num2 * (float)node.unitLost;
					}
					else if (aiData.resultEnter == Vector3.forward && aiData.resultExit != Vector3.forward)
					{
						if (node.nodeType == NodeType.Defense)
						{
							float num7 = Vector3.Distance(position, position2) / t.speed;
							float num8 = node.GetWidth() * node.GetAttackRage() / t.speed;
                            float cdValue = (float)typeof(Node)
                                .GetField("cd", BindingFlags.NonPublic | BindingFlags.Instance)
                                .GetValue(node);
                            if ((node.isactive && node.act + num7 - num8 < 5f) || (!node.isactive && num7 + cdValue >= 7.5f))
							{
								num += 10000f;
							}
						}
						num += Vector3.Distance(position, aiData.resultExit) / num2 * (float)node.unitLost;
					}
					else
					{
						if (node.nodeType == NodeType.Defense)
						{
							float num9 = Vector3.Distance(position, position2) / t.speed;
							float num10 = node.GetWidth() * node.GetAttackRage() / t.speed;
                            float cdValue = (float)typeof(Node)
								.GetField("cd", BindingFlags.NonPublic | BindingFlags.Instance)
								.GetValue(node);
                            if ((node.isactive && node.act + num9 - num10 < 5f) || (!node.isactive && num9 + cdValue >= 7.5f))
							{
								num += 10000f;
							}
						}
						num += Vector3.Distance(position, b) / num2 * (float)node.unitLost;
					}
				}
				else if (aiData.resultInside)
				{
					num += Vector3.Distance(position, b) / num2 * (float)node.unitLost;
					if (node.nodeType == NodeType.Defense)
					{
						float num11 = Vector3.Distance(position, position2) / t.speed;
						float num12 = node.GetWidth() * node.GetAttackRage() / t.speed;
                        float cdValue = (float)typeof(Node)
							.GetField("cd", BindingFlags.NonPublic | BindingFlags.Instance)
							.GetValue(node);
                        if ((node.isactive && node.act + num11 - num12 < 5f) || (!node.isactive && num11 + cdValue >= 7.5f))
						{
							num += 10000f;
						}
					}
				}
			}
		}
		if (arg1.revoType != RevolutionType.RT_None)
		{
			return (int)(1.35f * num);
		}
		return (int)(1.2f * num);
	}

	private bool vt1_DefendAdd(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.targetList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			node.GetTransitShips(t);
			if (node.type != NodeType.Barrier && node.IsOurNode(t.team))
			{
				int num = node.PredictedTeamStrength(t.team, true);
				int num2 = node.PredictedOppStrength(t.team, true);
				if (num2 != 0 && (num <= 5 || num <= num2 * 2))
				{
					float magnitude = (node.GetPosition() - b).magnitude;
					float num3 = (float)(num2 - num);
					node.aiValue = magnitude + num3;
					aiData.targetList.Add(node);
				}
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
		aiData.senderList.Clear();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node2 = usefulNodeList[j];
			if ((so == null || node2 == so) && node2.aiTimers[(int)t.team] <= 0f && (node2.aistrategy < 0 || so != null) && node2.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				float num4 = (float)node2.TeamStrength(t.team, false);
				if (num4 >= 2f && node2.OppStrength(t.team) < 1)
				{
					node2.aiStrength = -num4;
					aiData.senderList.Add(node2);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		this.CalcTarget(t);
		for (int k = 0; k < aiData.targetList.Count; k++)
		{
			Node node3 = aiData.targetList[k];
			for (int l = 0; l < aiData.senderList.Count; l++)
			{
				Node node4 = aiData.senderList[l];
				if (node4 != node3 && node4.AICanLink(node3, t) && node4.PredictedTeamStrength(t.team, true) + node3.PredictedTeamStrength(t.team, true) > node3.PredictedOppStrength(t.team, true))
				{
					int num5 = (int)((double)node3.PredictedOppStrength(t.team, true) * 1.4 - (double)node3.PredictedTeamStrength(t.team, true) + 2.0);
					int lostInTowerRange = this.GetLostInTowerRange(t, node4, node3);
					if ((node4.PredictedTeamStrength(t.team, true) - node4.PredictedOppStrength(t.team, true) - lostInTowerRange >= num5 || lostInTowerRange <= 0) && (node4.IsOurNode(t.team) || (node3.PredictedOppStrength(t.team, true) <= 13 && node3.PredictedTeamStrength(t.team, true) < node3.PredictedOppStrength(t.team, true) && node4.PredictedOppStrength(t.team, true) < 10 && lostInTowerRange < 5)))
					{
						num5 += lostInTowerRange;
						if (num5 < 10 && lostInTowerRange < 3)
						{
							num5 = node3.PredictedOppStrength(t.team, true) + lostInTowerRange + 1;
						}
						if (num5 > 250 && node4.PredictedOppStrength(t.team, true) < 150)
						{
							num5 = 100;
						}
						node4.AIMoveTo(t.team, node3, num5);
						node4.ResetAICalculateCache();
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool vt1_DefendAdd_Beta(Team t, float dt, Node so = null)
	{
		int num = (t.GetAttributeInt(TeamAttr.PopulationMax) <= t.GetAttributeInt(TeamAttr.Population)) ? 1 : 10;
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.targetList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			node.GetTransitShips(t);
			int num2 = (node.type == NodeType.Diffusion) ? num : 0;
			if (node.type != NodeType.Barrier && node.IsOurNode(t.team))
			{
				int num3 = node.PredictedTeamStrength(t.team, true);
				int num4 = node.PredictedOppStrength(t.team, true) + num2;
				if (num4 != 0 && (num3 <= 5 || num3 <= num4 * 2))
				{
					float magnitude = (node.GetPosition() - b).magnitude;
					float num5 = (float)(num4 - num3);
					node.aiValue = magnitude + num5;
					aiData.targetList.Add(node);
				}
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
		aiData.senderList.Clear();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node2 = usefulNodeList[j];
			if ((so == null || node2 == so) && node2.aiTimers[(int)t.team] <= 0f && (node2.aistrategy < 0 || so != null) && node2.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				float num6 = (float)node2.TeamStrength(t.team, false);
				if (num6 >= 2f && node2.OppStrength(t.team) < 1)
				{
					node2.aiStrength = -num6;
					aiData.senderList.Add(node2);
				}
			}
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		this.CalcTarget(t);
		for (int k = 0; k < aiData.targetList.Count; k++)
		{
			Node node3 = aiData.targetList[k];
			int num7 = (node3.type == NodeType.Diffusion) ? num : 0;
			for (int l = 0; l < aiData.senderList.Count; l++)
			{
				Node node4 = aiData.senderList[l];
				if (node4 != node3 && node4.AICanLink(node3, t) && node4.PredictedTeamStrength(t.team, true) + node3.PredictedTeamStrength(t.team, true) > node3.PredictedOppStrength(t.team, true) + num7)
				{
					int num8 = (int)((double)(node3.PredictedOppStrength(t.team, true) + num7) * 1.4 - (double)node3.PredictedTeamStrength(t.team, true) + 2.0);
					int lostInTowerRange = this.GetLostInTowerRange(t, node4, node3);
					if ((node4.PredictedTeamStrength(t.team, true) - node4.PredictedOppStrength(t.team, true) - lostInTowerRange >= num8 || lostInTowerRange <= 0) && (node4.IsOurNode(t.team) || (node3.PredictedOppStrength(t.team, true) + num7 <= 13 && node3.PredictedTeamStrength(t.team, true) < node3.PredictedOppStrength(t.team, true) + num7 && node4.PredictedOppStrength(t.team, true) < 10 && lostInTowerRange < 5)))
					{
						num8 += lostInTowerRange;
						if (num8 < 10 && lostInTowerRange < 3)
						{
							num8 = node3.PredictedOppStrength(t.team, true) + num7 + lostInTowerRange + 1;
						}
						if (num8 > 250 && node4.PredictedOppStrength(t.team, true) < 150)
						{
							num8 = 100;
						}
						node4.AIMoveTo(t.team, node3, num8);
						node4.ResetAICalculateCache();
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool vt1_GatherV3(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		aiData.senderList.Clear();
		int num = 45;
		bool flag = false;
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			if ((node.nodeType == NodeType.Tower || node.nodeType == NodeType.Twist || node.nodeType == NodeType.Gunturret || node.nodeType == NodeType.Lasergun) && node.currentTeam.team == TEAM.Neutral)
			{
				num += 5;
			}
			else if ((node.nodeType == NodeType.Tower || node.nodeType == NodeType.Twist || node.nodeType == NodeType.Gunturret || node.nodeType == NodeType.Lasergun) && node.currentTeam.team != t.team)
			{
				num += 10;
			}
			else if (node.nodeType == NodeType.Castle && node.team == TEAM.Neutral)
			{
				num += 8;
			}
			else if (node.nodeType == NodeType.Castle && node.team != t.team)
			{
				num += 15;
			}
			if (node.nodeType != NodeType.Planet && node.nodeType != NodeType.Barrier && node.nodeType != NodeType.BarrierLine)
			{
				flag = true;
			}
			node.GetTransitShips(t);
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.TeamStrength(t.team, false) >= 1 && (node.IsOurNode(t.team) || node.PredictedTeamStrength(t.team, true) <= node.PredictedOppStrength(t.team, true)) && (node.PredictedOppStrength(t.team, true) <= 0 || node.PredictedTeamStrength(t.team, true) <= node.PredictedOppStrength(t.team, true)) && node.GetAttributeInt(NodeAttr.Ice) <= 0)
			{
				node.aiStrength = -(float)node.TeamStrength(t.team, false) - (float)node.OppStrength(t.team);
				node.aiValue = -(float)node.GetOppLinks(t, usefulNodeList);
				if (node.type == NodeType.WarpDoor && node.IsOurNode(t.team))
				{
					node.aiValue -= 1f;
					node.aiValue += node.aiStrength;
				}
				int num2 = node.OppStrength(t.team);
				int num3 = (int)((float)node.PredictedOppStrength(t.team, true) * 1.5f);
				int num4 = node.PredictedTeamStrength(t.team, true);
				if (num3 <= 0 || num4 <= num2 || 2 * num4 < node.PredictedOppStrength(t.team, true))
				{
					aiData.senderList.Add(node);
				}
			}
		}
		if (!flag)
		{
			num = 30;
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int attributeBaseInt = node2.currentTeam.GetAttributeBaseInt(TeamAttr.Population);
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && !node3.IsOurNode(t.team) && node2.AICanLink(node3, t) && ((node3.team != TEAM.Neutral && node3.PredictedTeamStrength(t.team, true) <= 2 * num) || (node3.PredictedTeamStrength(t.team, true) <= num && node3.team == TEAM.Neutral) || node3.PredictedTeamStrength(t.team, true) <= node3.PredictedOppStrength(t.team, true) * 2))
				{
					int lostInTowerRange = this.GetLostInTowerRange(t, node2, node3);
					if (node2.TeamStrength(t.team, false) - node3.PredictedOppStrength(t.team, true) - lostInTowerRange > 0)
					{
						int attributeBaseInt2 = node3.currentTeam.GetAttributeBaseInt(TeamAttr.Population);
						float num5 = 1f;
						if (attributeBaseInt2 > attributeBaseInt)
						{
							num5 = (float)attributeBaseInt2 / (float)attributeBaseInt;
						}
						float num6 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
						if (node2.type == NodeType.WarpDoor && node2.IsOurNode(t.team))
						{
							num6 /= 100f;
						}
						float num7 = this.CalcOccupiedTime(node3, t, node2.PredictedTeamStrength(t.team, true) + node3.PredictedTeamStrength(t.team, true) - node3.PredictedOppStrength(t.team, true) - lostInTowerRange);
						node3.aiValue = (num6 + num7) / (float)node3.weight / num5;
						aiData.targetList.Add(node3);
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.PredictedTeamStrength(t.team, true) > node4.PredictedOppStrength(t.team, true))
					{
						int num8 = (int)((float)(node4.PredictedOppStrength(t.team, true) * 2) - (float)node4.PredictedTeamStrength(t.team, true) * 0.5f);
						int lostInTowerRange2 = this.GetLostInTowerRange(t, node2, node4);
						if (lostInTowerRange2 <= 0 || (node2.TeamStrength(t.team, false) > (int)(1.2f * (float)lostInTowerRange2) && (node2.nodeType != NodeType.Diffusion || node2.PredictedTeamStrength(t.team, true) > 16)))
						{
							num8 += lostInTowerRange2;
							if (node2.PredictedOppStrength(t.team, true) == 0 || node2.PredictedOppStrength(t.team, true) > node2.PredictedTeamStrength(t.team, true))
							{
								if (node2.PredictedTeamStrength(t.team, true) > num && node4.PredictedTeamStrength(t.team, true) < node2.PredictedTeamStrength(t.team, true) - num)
								{
									num8 = node2.TeamStrength(t.team, false) - num;
									if (node2.PredictedTeamStrength(t.team, true) >= num * 2)
									{
										num8 = (int)((float)node2.PredictedTeamStrength(t.team, true) * Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(0.4f, 0.55f));
									}
								}
								else
								{
									num8 = node2.TeamStrength(t.team, false);
								}
								if (node2.PredictedOppStrength(t.team, true) > node2.PredictedTeamStrength(t.team, true))
								{
									num8 = node2.TeamStrength(t.team, false);
								}
								if (node2.PredictedTeamStrength(t.team, true) > 12 && node4.PredictedOppStrength(t.team, true) <= node2.PredictedTeamStrength(t.team, true) - 12 && node2.type == NodeType.Diffusion)
								{
									num8 = node2.TeamStrength(t.team, false) - 12;
								}
								if (node2.PredictedTeamStrength(t.team, true) <= 13 && node2.type == NodeType.Diffusion)
								{
									num8 = 0;
								}
							}
							if (num8 != 0 && node2.type != NodeType.Diffusion)
							{
								num8 += lostInTowerRange2;
							}
							if (num8 <= lostInTowerRange2)
							{
								num8 = 0;
							}
							node2.AIMoveTo(t.team, node4, num8);
							node2.ResetAICalculateCache();
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		aiData.targetList.Clear();
		for (int m = 0; m < usefulNodeList.Count; m++)
		{
			Node node5 = usefulNodeList[m];
                        if (node5.CanBeAITarget(t.team) && node5.IsOurNode(t.team))
			{
				node5.aiStrength = -(float)node5.TeamStrength(t.team, false) - (float)node5.OppStrength(t.team);
				node5.aiValue = -(float)node5.GetOppLinks(t, usefulNodeList);
				if (node5.type == NodeType.WarpDoor)
				{
					node5.aiValue -= 1f;
					node5.aiValue += node5.aiStrength;
				}
				aiData.targetList.Add(node5);
			}
		}
		if (aiData.targetList.Count == 0)
		{
			return false;
		}
		aiData.targetList.Sort(new Comparison<Node>(base.ComparsionAIValue));
		this.CalcTarget(t);
		for (int n = 0; n < aiData.senderList.Count; n++)
		{
			Node node6 = aiData.senderList[n];
			for (int num9 = 0; num9 < aiData.targetList.Count; num9++)
			{
				Node node7 = aiData.targetList[num9];
				if (node6 != node7 && node6.AICanLink(node7, t) && node7.aiValue <= node6.aiValue)
				{
					int num10 = node6.TeamStrength(t.team, false);
					int lostInTowerRange3 = this.GetLostInTowerRange(t, node6, node7);
					num10 += lostInTowerRange3;
					if (((lostInTowerRange3 <= 0 || (float)node6.TeamStrength(t.team, false) >= (float)lostInTowerRange3 * 3f) && node6.PredictedOppStrength(t.team, true) == 0) || node6.PredictedOppStrength(t.team, true) >= node6.PredictedTeamStrength(t.team, true))
					{
						if (node6.PredictedTeamStrength(t.team, true) > 12 && node7.PredictedOppStrength(t.team, true) <= node6.PredictedTeamStrength(t.team, true) - 12 && node6.type == NodeType.Diffusion)
						{
							num10 = node6.TeamStrength(t.team, false) - 12;
						}
						if (num10 > num * 2 && node6.PredictedOppStrength(t.team, true) == 0)
						{
							num10 = num;
						}
						if (node6.nodeType == NodeType.Diffusion && node6.TeamStrength(t.team, false) <= 13)
						{
							num10 = 0;
						}
						if (num10 < lostInTowerRange3)
						{
							num10 = 0;
						}
						node6.AIMoveTo(t.team, node7, num10);
						node6.ResetAICalculateCache();
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool vt1_AttackV4(Team t, float dt, Node so = null)
	{
		AIData aiData = base.GetAiData(t, false);
		Vector3 b = base.TraversalAllNodeCenter(t);
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		int num = 45;
		bool flag = false;
		aiData.senderList.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			node.GetTransitShips(t);
			if ((node.nodeType == NodeType.Tower || node.nodeType == NodeType.Twist) && node.currentTeam.team == TEAM.Neutral)
			{
				num += 5;
			}
			else if ((node.nodeType == NodeType.Tower || node.nodeType == NodeType.Twist) && node.currentTeam.team != t.team)
			{
				num += 10;
			}
			else if (node.nodeType == NodeType.Castle && node.team == TEAM.Neutral)
			{
				num += 8;
			}
			else if (node.nodeType == NodeType.Castle && node.team != t.team)
			{
				num += 15;
			}
			if (node.nodeType != NodeType.Planet && node.nodeType != NodeType.Barrier && node.nodeType != NodeType.BarrierLine)
			{
				flag = true;
			}
			if ((so == null || node == so) && node.aiTimers[(int)t.team] <= 0f && (node.aistrategy < 0 || so != null) && node.GetAttributeInt(NodeAttr.Ice) <= 0 && node.IsOurNode(t.team))
			{
				int num2 = node.OppStrength(t.team);
				int num3 = (int)((float)node.PredictedOppStrength(t.team, true) * 1.8f);
				if ((num2 != 0 || node.state != NodeState.Capturing) && node.TeamStrength(t.team, false) >= 1)
				{
					int num4 = node.PredictedTeamStrength(t.team, true);
					if (num3 <= 0 || num4 < num2 || num4 >= num3 || 2 * num4 < node.PredictedOppStrength(t.team, true))
					{
						node.aiStrength = -(float)node.TeamStrength(t.team, false);
						aiData.senderList.Add(node);
					}
				}
			}
			if (node.nodeType == NodeType.WarpDoor && node.TeamStrength(t.team, true) < node.OppStrength(t.team) && node.team != t.team && node.aiTimers[(int)t.team] <= 0f)
			{
				node.aiStrength = -(float)node.TeamStrength(t.team, false);
				aiData.senderList.Add(node);
			}
		}
		if (!flag)
		{
			num = 30;
		}
		if (aiData.senderList.Count == 0)
		{
			return false;
		}
		aiData.senderList.Sort(new Comparison<Node>(base.ComparsionAIStrength));
		for (int j = 0; j < aiData.senderList.Count; j++)
		{
			Node node2 = aiData.senderList[j];
			int current = node2.currentTeam.current;
			aiData.targetList.Clear();
			for (int k = 0; k < usefulNodeList.Count; k++)
			{
				Node node3 = usefulNodeList[k];
                                if (node3.CanBeAITarget(t.team) && node3 != node2 && !node3.IsOurNode(t.team) && node2.AICanLink(node3, t) && ((node3.team != TEAM.Neutral && node3.PredictedTeamStrength(t.team, true) <= 2 * num) || (node3.PredictedTeamStrength(t.team, true) <= num && node3.team == TEAM.Neutral) || node3.PredictedTeamStrength(t.team, true) <= node3.PredictedOppStrength(t.team, true) * 2))
				{
					int lostInTowerRange = this.GetLostInTowerRange(t, node2, node3);
					if (node2.TeamStrength(t.team, false) - node3.PredictedOppStrength(t.team, true) - lostInTowerRange > 0)
					{
						int current2 = node3.currentTeam.current;
						float num5 = 1f;
						if (current2 > current)
						{
							num5 = (float)current2 * 2f / (float)current;
						}
						float num6 = (node3.GetPosition() - b).magnitude / t.GetAttributeFloat(TeamAttr.Speed);
						if (node2.type == NodeType.WarpDoor && node2.IsOurNode(t.team))
						{
							num6 /= 100f;
						}
						float num7 = this.CalcOccupiedTime(node3, t, node2.PredictedTeamStrength(t.team, true) + node3.PredictedTeamStrength(t.team, true) - node3.PredictedOppStrength(t.team, true) - lostInTowerRange);
						node3.aiValue = (num6 + num7) / (float)node3.weight / num5;
						aiData.targetList.Add(node3);
					}
				}
			}
			if (aiData.targetList.Count != 0)
			{
				aiData.targetList.Sort(new Comparison<Node>(base.ComparisonAIValue));
				this.CalcTargetV1(t);
				for (int l = 0; l < aiData.targetList.Count; l++)
				{
					Node node4 = aiData.targetList[l];
					if (l >= 3)
					{
						break;
					}
					if (node2.TeamStrength(t.team, false) + node4.PredictedTeamStrength(t.team, true) > node4.PredictedOppStrength(t.team, true))
					{
						int num8 = (int)((float)(node4.PredictedOppStrength(t.team, true) * 2) - (float)node4.PredictedTeamStrength(t.team, true) * 0.5f + 0.5f);
						int lostInTowerRange2 = this.GetLostInTowerRange(t, node2, node4);
						if (lostInTowerRange2 <= 0 || node2.TeamStrength(t.team, false) - lostInTowerRange2 >= num8)
						{
							num8 += lostInTowerRange2;
							if (node2.OppStrength(t.team) == 0)
							{
								if (node2.PredictedTeamStrength(t.team, true) > num && node4.PredictedOppStrength(t.team, true) <= node2.PredictedTeamStrength(t.team, true) - num)
								{
									num8 = node2.TeamStrength(t.team, false) - num;
									if (node2.PredictedTeamStrength(t.team, true) >= num * 2)
									{
										num8 = (int)((float)node2.PredictedTeamStrength(t.team, true) * Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(0.4f, 0.55f));
										if (node4.nodeType == NodeType.Tower || node4.nodeType == NodeType.Twist || node4.nodeType == NodeType.Castle)
										{
											num8 = (int)((float)node2.PredictedTeamStrength(t.team, true) * Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.Range(0.65f, 1f));
										}
									}
								}
								else if (node2.PredictedOppStrength(t.team, true) < node2.PredictedTeamStrength(t.team, true) && node2.TeamStrength(t.team, false) - node2.PredictedOppStrength(t.team, true) > (int)((float)(node4.PredictedOppStrength(t.team, true) - node4.PredictedTeamStrength(t.team, true)) * 1.3f))
								{
									num8 = node2.TeamStrength(t.team, false) - node2.PredictedOppStrength(t.team, true) - 2;
								}
								if (node2.PredictedTeamStrength(t.team, true) > 12 && node4.PredictedOppStrength(t.team, true) <= node2.PredictedTeamStrength(t.team, true) - 12 && node2.type == NodeType.Diffusion)
								{
									num8 = node2.TeamStrength(t.team, false) - 12;
								}
								if (node2.TeamStrength(t.team, false) <= 12 && node2.nodeType == NodeType.Diffusion && node2.PredictedOppStrength(t.team, true) == 0)
								{
									num8 = 0;
								}
								if (node2.PredictedOppStrength(t.team, true) >= node2.PredictedTeamStrength(t.team, true))
								{
									num8 = node2.TeamStrength(t.team, false);
								}
							}
							if (node2.TeamStrength(t.team, true) <= 3 && node2.PredictedOppStrength(t.team, true) < 1 && node2.nodeType != NodeType.Diffusion)
							{
								num8 = node2.TeamStrength(t.team, true);
							}
							if (node2.IsOurNode(t.team) && node2.TeamStrength(t.team, true) > node2.OppStrength(t.team) && node2.TeamStrength(t.team, true) <= (int)((float)node2.OppStrength(t.team) * 1.8f))
							{
								num8 = 0;
							}
							if (num8 != 0 && node2.nodeType != NodeType.Diffusion)
							{
								num8 += lostInTowerRange2;
							}
							if (num8 <= lostInTowerRange2)
							{
								num8 = 0;
							}
							node2.AIMoveTo(t.team, node4, num8);
							node2.ResetAICalculateCache();
							node2.ResetAiTimer(t.team, aiData.aiTimeInterval);
						}
					}
				}
			}
		}
		return false;
	}
}
