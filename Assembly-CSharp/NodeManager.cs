using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class NodeManager : Lifecycle2
{
	public NodeManager(SceneManager magr)
	{
		this.sceneManager = magr;
		this.nodeMap = new Dictionary<string, Node>();
		this.usefulNodeList = new List<Node>();
		this.barrierNodeList = new List<Node>();
		this.globalNodeList = new List<Node>();
	}

	public void AddFixedPortal(Vector3 pos, float fAngleY, float radius)
	{
		NodeManager.FixedPortal fixedPortal = new NodeManager.FixedPortal();
		fixedPortal.keyPos = pos;
		fixedPortal.fAngle = fAngleY;
		fixedPortal.fRadius = radius;
		this.mapPortal.Add(fixedPortal);
	}

	public void AddBarrierLines(string barrierX, string barrierY)
	{
		Node node = this.GetNode(barrierX);
		Node node2 = this.GetNode(barrierY);
		if (node == null || node2 == null)
		{
			return;
		}
		if (node.type != NodeType.Barrier || node2.type != NodeType.Barrier)
		{
			return;
		}
		NodeManager.BarrierPair barrierPair = new NodeManager.BarrierPair();
		barrierPair.keyPos = node.GetPosition();
		barrierPair.keyLocalPos = node.GetPosition();
		barrierPair.valuePos = node2.GetPosition();
		barrierPair.valuedLocalPos = node2.GetPosition();
		barrierPair.barrierX = barrierX;
		barrierPair.barrierY = barrierY;
		barrierPair.barrierTeam = node.team;
		barrierPair.dynamic = false;
		barrierPair.show = true;
		this.BarriersKY.Add(barrierPair);
		this.calculatePoints(barrierPair.keyPos, barrierPair.valuePos, barrierPair, true);
	}

	public void AddDynamicBarrierLines(string barrierX, string barrierY, bool show)
	{
		Node node = this.GetNode(barrierX);
		Node node2 = this.GetNode(barrierY);
		if (node == null || node2 == null)
		{
			return;
		}
		if (node.type != NodeType.BarrierPoint || node2.type != NodeType.BarrierPoint)
		{
			return;
		}
		Vector3 position = node.GetPosition();
		Vector3 position2 = node2.GetPosition();
		float f = Mathf.Atan((position2.y - position.y) / (position2.x - position.x)) + 3.1415927f;
		Vector3 zero;
		Vector3 vector = zero = Vector3.zero;
		zero.x = position.x + node.GetWidth() * Mathf.Cos(f) / 2f;
		zero.y = position.y + node.GetWidth() * Mathf.Sin(f) / 2f;
		vector.x = position2.x - node2.GetWidth() * Mathf.Cos(f) / 2f;
		vector.y = position2.y - node2.GetWidth() * Mathf.Sin(f) / 2f;
		NodeManager.BarrierPair barrierPair = new NodeManager.BarrierPair();
		barrierPair.keyPos = zero;
		barrierPair.keyLocalPos = zero;
		barrierPair.valuePos = vector;
		barrierPair.valuedLocalPos = vector;
		barrierPair.barrierX = barrierX;
		barrierPair.barrierY = barrierY;
		barrierPair.barrierTeam = node.team;
		barrierPair.dynamic = true;
		barrierPair.show = show;
		this.BarriersKY.Add(barrierPair);
		this.calculatePoints(barrierPair.keyPos, barrierPair.valuePos, barrierPair, show);
	}

	public void ResetDynamicBarrierLines(string barrier)
	{
		foreach (NodeManager.BarrierPair barrierPair in this.BarriersKY)
		{
			if (barrierPair.barrierX == barrier || barrierPair.barrierY == barrier)
			{
				if (barrierPair.dynamic)
				{
					Node node = this.GetNode(barrierPair.barrierX);
					Node node2 = this.GetNode(barrierPair.barrierY);
					if (node != null && node2 != null)
					{
						if (node is BarrierPointNode && node2 is BarrierPointNode)
						{
							if (barrierPair.show && ((BarrierPointNode)node).haveShip != ((BarrierPointNode)node2).haveShip)
							{
								foreach (int num in barrierPair.barrierLinetags)
								{
									Node node3 = this.GetNode(num.ToString());
									if (node3 != null)
									{
										Vector3 position = node3.GetPosition();
										position.y += 100f;
										node3.SetPosition(position);
									}
								}
								barrierPair.show = false;
							}
							else if (!barrierPair.show && ((BarrierPointNode)node).haveShip == ((BarrierPointNode)node2).haveShip)
							{
								foreach (int num2 in barrierPair.barrierLinetags)
								{
									Node node4 = this.GetNode(num2.ToString());
									if (node4 != null)
									{
										Vector3 position2 = node4.GetPosition();
										position2.y -= 100f;
										node4.SetPosition(position2);
									}
								}
								barrierPair.show = true;
							}
						}
					}
				}
			}
		}
	}

	public void MoveOutBarrierLines(int i)
	{
		if (i < 0 || i >= this.BarriersKY.Count)
		{
			return;
		}
		NodeManager.BarrierPair barrierPair = this.BarriersKY[i];
		foreach (int num in barrierPair.barrierLinetags)
		{
			Node node = this.GetNode(num.ToString());
			if (node != null)
			{
				Vector3 position = node.GetPosition();
				position.y += 100f;
				node.SetPosition(position);
			}
		}
		barrierPair.show = false;
	}

	public void MoveOutAllBarrierLines()
	{
		foreach (NodeManager.BarrierPair barrierPair in this.BarriersKY)
		{
			foreach (int num in barrierPair.barrierLinetags)
			{
				Node node = this.GetNode(num.ToString());
				if (node != null)
				{
					Vector3 position = node.GetPosition();
					position.y += 100f;
					node.SetPosition(position);
				}
			}
			barrierPair.show = false;
		}
	}

	private void calculatePoints(Vector3 startPos, Vector3 endPos, NodeManager.BarrierPair pair, bool show = true)
	{
		Vector3 vector = endPos - startPos;
		vector.Normalize();
		Vector3 vector2 = startPos;
		pair.barrierLinetags.Add(this.BarrierLinetag);
		this.draw(vector2, vector, pair.barrierTeam, show);
		while (Vector3.Distance(endPos, vector2) > 0.1f)
		{
			vector2 += vector * 0.1f;
			pair.barrierLinetags.Add(this.BarrierLinetag);
			this.draw(vector2, vector, pair.barrierTeam, show);
		}
	}

	private void draw(Vector3 pos, Vector3 dir, TEAM barrierTEAM, bool show = true)
	{
		this.AddNode(0, (int)barrierTEAM, 6, 1, 0, pos.x, pos.y, 0.5f, this.BarrierLinetag.ToString(), 0, 0f, 0f, 0, 0f, 0f, 0f, 0f, this.sceneManager.battleData.mapEdit, 0, string.Empty, string.Empty, string.Empty, string.Empty, false, 1f, string.Empty, string.Empty, 0f, 0);
		float num = Vector3.Angle(dir, Vector3.right);
		if (dir.y < 0f)
		{
			num = 360f - num;
		}
		Node node = this.GetNode(this.BarrierLinetag.ToString());
		node.SetRotation(new Vector3(0f, 0f, num));
		if (!show)
		{
			Vector3 position = node.GetPosition();
			position.y += 100f;
			node.SetPosition(position);
		}
		this.BarrierLinetag++;
	}

	private void draw2(Vector3 pos, Vector3 end, TEAM barrierTEAM)
	{
		this.AddNode(0, (int)barrierTEAM, 6, 1, 0, pos.x, pos.y, 0.5f, this.BarrierLinetag.ToString(), 0, 0f, 0f, 0, 0f, 0f, 0f, 0f, this.sceneManager.battleData.mapEdit, 0, string.Empty, string.Empty, string.Empty, string.Empty, false, 1f, string.Empty, string.Empty, 0f, 0);
		List<Vector3> list = new List<Vector3>();
		list.Add(pos);
		list.Add(end);
		Node node = this.GetNode(this.BarrierLinetag.ToString());
		node.entity.AddPoint(list);
		list.Clear();
		this.BarrierLinetag++;
	}

	public void DelBarrierLines(string barrierX, string barrierY)
	{
		foreach (NodeManager.BarrierPair barrierPair in this.BarriersKY)
		{
			if (barrierX.CompareTo(barrierPair.barrierX) == 0 && barrierY.CompareTo(barrierPair.barrierY) == 0)
			{
				foreach (int num in barrierPair.barrierLinetags)
				{
					this.RemoveNode(num.ToString());
				}
				this.BarriersKY.Remove(barrierPair);
				break;
			}
			if (barrierX.CompareTo(barrierPair.barrierY) == 0 && barrierY.CompareTo(barrierPair.barrierX) == 0)
			{
				foreach (int num2 in barrierPair.barrierLinetags)
				{
					this.RemoveNode(num2.ToString());
				}
				this.BarriersKY.Remove(barrierPair);
				break;
			}
		}
	}

	public void DelBarrierLines(string barrier)
	{
		List<NodeManager.BarrierPair> list = new List<NodeManager.BarrierPair>();
		foreach (NodeManager.BarrierPair barrierPair in this.BarriersKY)
		{
			if (barrier.CompareTo(barrierPair.barrierX) == 0 || barrier.CompareTo(barrierPair.barrierY) == 0)
			{
				foreach (int num in barrierPair.barrierLinetags)
				{
					this.RemoveNode(num.ToString());
				}
				list.Add(barrierPair);
			}
		}
		foreach (NodeManager.BarrierPair item in list)
		{
			this.BarriersKY.Remove(item);
		}
		list.Clear();
	}

	public bool IntersectBarrierLien(Vector3 startPos, Vector3 endPos)
	{
		foreach (NodeManager.BarrierPair barrierPair in this.BarriersKY)
		{
			if (barrierPair.show)
			{
				if (this.detectIntersect(barrierPair.keyPos, barrierPair.valuePos, startPos, endPos))
				{
					return true;
				}
			}
		}
		return false;
	}

	public int IntersectBarrierLine(Vector3 startPos, Vector3 endPos)
	{
		for (int i = 0; i < this.BarriersKY.Count; i++)
		{
			NodeManager.BarrierPair barrierPair = this.BarriersKY[i];
			if (barrierPair.show)
			{
				if (this.detectIntersect(barrierPair.keyPos, barrierPair.valuePos, startPos, endPos))
				{
					return i;
				}
			}
		}
		return -1;
	}

	public bool IntersectCircle(Vector3 vBegin, Vector3 vEnd, Vector3 center, float radius)
	{
		float num;
		float num2;
		float num3;
		if (vBegin.x == vEnd.x)
		{
			num = 1f;
			num2 = 0f;
			num3 = -vBegin.x;
		}
		else if (vBegin.y == vEnd.y)
		{
			num = 0f;
			num2 = 1f;
			num3 = -vBegin.y;
		}
		else
		{
			num = vBegin.y - vEnd.y;
			num2 = vEnd.x - vBegin.x;
			num3 = vBegin.x * vEnd.y - vBegin.y * vEnd.x;
		}
		float num4 = num * center.x + num2 * center.y + num3;
		num4 *= num4;
		float num5 = (num * num + num2 * num2) * radius * radius;
		if (num4 > num5)
		{
			return false;
		}
		float num6 = (center.x - vBegin.x) * (vEnd.x - vBegin.x) + (center.y - vBegin.y) * (vEnd.y - vBegin.y);
		float num7 = (center.x - vEnd.x) * (vBegin.x - vEnd.x) + (center.y - vEnd.y) * (vBegin.y - vEnd.y);
		return num6 > 0f && num7 > 0f;
	}

	private float VectorAngle(Vector2 from, Vector2 to)
	{
		Vector3 vector = Vector3.Cross(from, to);
		float num = Vector2.Angle(from, to);
		return (vector.z <= 0f) ? num : (-num);
	}

	public bool IsFixedPortal(Vector3 vBegin, Vector3 vEnd)
	{
		foreach (NodeManager.FixedPortal fixedPortal in this.mapPortal)
		{
			if (this.IntersectCircle(vBegin, vEnd, fixedPortal.keyPos, fixedPortal.fRadius))
			{
				float num = this.VectorAngle(vBegin - vEnd, Vector3.up);
				if (Mathf.Abs(num - fixedPortal.fAngle) < 5f)
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	private bool between(float a, float X0, float X1)
	{
		float num = a - X0;
		float num2 = a - X1;
		return ((double)num < 1E-08 && (double)num2 > -1E-08) || ((double)num2 < 1E-06 && (double)num > -1E-08);
	}

	private bool detectIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
	{
		if ((double)Mathf.Abs(p1.x - p2.x) < 1E-06 && (double)Mathf.Abs(p3.x - p4.x) < 1E-06)
		{
			return false;
		}
		if ((double)Mathf.Abs(p1.x - p2.x) < 1E-06)
		{
			if (this.between(p1.x, p3.x, p4.x))
			{
				float num = (p4.y - p3.y) / (p4.x - p3.x);
				float num2 = p1.x;
				float a = num * (num2 - p3.x) + p3.y;
				return this.between(a, p1.y, p2.y);
			}
			return false;
		}
		else if ((double)Mathf.Abs(p3.x - p4.x) < 1E-06)
		{
			if (this.between(p3.x, p1.x, p2.x))
			{
				float num3 = (p2.y - p1.y) / (p2.x - p1.x);
				float num2 = p3.x;
				float a = num3 * (num2 - p2.x) + p2.y;
				return this.between(a, p3.y, p4.y);
			}
			return false;
		}
		else
		{
			float num4 = (p2.y - p1.y) / (p2.x - p1.x);
			float num5 = (p4.y - p3.y) / (p4.x - p3.x);
			if ((double)Mathf.Abs(num4 - num5) < 1E-06)
			{
				return false;
			}
			float num2 = (p3.y - p1.y - (num5 * p3.x - num4 * p1.x)) / (num4 - num5);
			float a = num4 * (num2 - p1.x) + p1.y;
			return this.between(num2, p1.x, p2.x) && this.between(num2, p3.x, p4.x);
		}
	}

	public SceneManager sceneManager { get; set; }

	public bool Init()
	{
		return true;
	}

	public void Tick(int frame, float interval)
	{
		int count = this.usefulNodeList.Count;
		for (int i = 0; i < count; i++)
		{
			Node node = this.usefulNodeList[i];
			node.Tick(frame, interval);
		}
		int j = 0;
		int count2 = this.globalNodeList.Count;
		while (j < count2)
		{
			Node node2 = this.globalNodeList[j];
			node2.Tick(frame, interval);
			j++;
		}
	}

	public void Destroy()
	{
		int i = 0;
		int count = this.usefulNodeList.Count;
		while (i < count)
		{
			Node node = this.usefulNodeList[i];
			node.Destroy();
			i++;
		}
		int j = 0;
		int count2 = this.barrierNodeList.Count;
		while (j < count2)
		{
			Node node2 = this.barrierNodeList[j];
			node2.Destroy();
			j++;
		}
		this.nodeMap.Clear();
		this.usefulNodeList.Clear();
		this.barrierNodeList.Clear();
		this.BarriersKY.Clear();
		this.globalNodeList.Clear();
	}

	public void DestroyBarrierLineNode()
	{
		int i = 0;
		int count = this.barrierNodeList.Count;
		while (i < count)
		{
			Node node = this.barrierNodeList[i];
			if (node.type == NodeType.BarrierLine)
			{
				node.Destroy();
			}
			i++;
		}
	}

	public Node AddNode(int id, int team, int kind, int weight, int unitlost, float x, float y, float scale, string tag, int buildNum, float buildTimer, float hpMax, int pop, float rage, float speed, float power, float skillspeed, bool mapEdit, int orbit, string orbitParam1, string orbitParam2, string orbitParam3, string orbitParam4, bool orbitclockwise, float nodesize, string perfab, string skills, float fAngle, int aistrategy)
	{
		if (this.nodeMap.ContainsKey(tag))
		{
			return null;
		}
		Vector3 v3 = Vector3.zero;
		v3.z = fAngle;
		Node node = this.CreateNode((NodeType)kind, tag, perfab);
		node.nodeManager = this;
		if (team == 0)
		{
			node.hp = 0f;
		}
		else
		{
			node.hp = hpMax;
		}
		node.id = id;
		node.SetAttributeBase(NodeAttr.HpMax, hpMax);
		node.SetAttributeBase(NodeAttr.Poplation, (float)pop);
		node.produceNum = buildNum;
		node.produceFrame = (int)(25f * buildTimer);
		node.weight = weight;
		node.unitLost = unitlost;
		node.SetScale(scale);
		node.SetPosition(x, y);
		node.SetRevolution(orbit, orbitParam1, orbitParam2, orbitParam3, orbitParam4, orbitclockwise);
		node.SetRealTeam(this.sceneManager.teamManager.GetTeam((TEAM)team), true);
		node.initTEAM = (TEAM)team;
		node.SetAttributeBase(NodeAttr.AttackRange, rage);
		node.SetAttributeBase(NodeAttr.AttackSpeed, speed);
		if (skillspeed > 0f)
		{
			node.SetAttributeBase(NodeAttr.SkillSpeed, skillspeed);
		}
		else
		{
			node.SetAttributeBase(NodeAttr.SkillSpeed, speed);
		}
		node.SetAttributeBase(NodeAttr.AttackPower, power);
		node.SetNodeSize(nodesize);
		node.aistrategy = aistrategy;
		if (mapEdit)
		{
			node.SetMapEditModel();
		}
		this.nodeMap.Add(node.tag, node);
		if (node.type == NodeType.Barrier || node.type == NodeType.BarrierLine || node.type == NodeType.FixedWarpDoor)
		{
			this.barrierNodeList.Add(node);
		}
		else if (node.type == NodeType.Curse)
		{
			this.globalNodeList.Add(node);
			this.barrierNodeList.Add(node);
		}
		else
		{
			this.usefulNodeList.Add(node);
		}
		MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType(kind));
		if (data != null)
		{
			node.SetAttributeBase(NodeAttr.AttackRange, data.attackrange);
			node.SetShowRange(data.isShowRange);
		}
		return node;
	}

	public Node AddNode(string tag, int kind, float dx, float dy, float scale, bool mapedit = false, string perfab = "NULL")
	{
		if (this.nodeMap.ContainsKey(tag))
		{
			return null;
		}
		Node node = this.CreateNode((NodeType)kind, tag, perfab);
		node.nodeManager = this;
		node.SetScale(scale);
		node.SetPosition(dx, dy);
		node.currentTeam = this.sceneManager.teamManager.GetTeam(TEAM.Neutral);
		MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType(kind));
		if (data != null)
		{
			node.SetAttributeBase(NodeAttr.AttackRange, data.attackrange);
			node.SetShowRange(data.isShowRange);
		}
		if (mapedit)
		{
			node.SetMapEditModel();
		}
		this.nodeMap.Add(node.tag, node);
		if (node.type == NodeType.Barrier || node.type == NodeType.BarrierLine || node.type == NodeType.FixedWarpDoor)
		{
			this.barrierNodeList.Add(node);
		}
		else if (node.type == NodeType.Curse)
		{
			this.globalNodeList.Add(node);
			this.barrierNodeList.Add(node);
		}
		else
		{
			this.usefulNodeList.Add(node);
		}
		return node;
	}

	public void RemoveNode(string tag)
	{
		Node node = this.GetNode(tag);
		if (node == null)
		{
			return;
		}
		node.Destroy();
		this.nodeMap.Remove(tag);
		if (node.type == NodeType.Barrier || node.type == NodeType.BarrierLine || node.type == NodeType.Clouds)
		{
			this.barrierNodeList.Remove(node);
		}
		else if (node.type == NodeType.Curse)
		{
			this.globalNodeList.Remove(node);
			this.barrierNodeList.Remove(node);
		}
		else
		{
			this.usefulNodeList.Remove(node);
		}
	}

	private Node CreateNode(NodeType nodeType, string name, string perfab)
	{
		Node node = null;
		switch (nodeType)
		{
		case NodeType.Planet:
			node = new PlanetNode(name);
			break;
		case NodeType.Castle:
			node = new CastleNode(name);
			break;
		case NodeType.WarpDoor:
			node = new WarpDoorNode(name);
			break;
		case NodeType.Tower:
			node = new TowerNode(name);
			break;
		case NodeType.Barrier:
			node = new BarrierNode(name);
			break;
		case NodeType.BarrierLine:
			node = new BarrierLineNode(name);
			break;
		case NodeType.Master:
			node = new MasterNode(name);
			break;
		case NodeType.Defense:
			node = new DefenseNode(name);
			break;
		case NodeType.Power:
			node = new PowerNode(name);
			break;
		case NodeType.BlackHole:
			node = new CreatureNode(name);
			break;
		case NodeType.House:
			node = new HouseNode(name);
			break;
		case NodeType.Arsenal:
			node = new ArsenalNode(name);
			break;
		case NodeType.AircraftCarrier:
			node = new AircraftCarrierNode(name);
			break;
		case NodeType.Lasercannon:
		case NodeType.Attackship:
		case NodeType.Lifeship:
		case NodeType.Speedship:
		case NodeType.Captureship:
		case NodeType.AntiAttackship:
		case NodeType.AntiLifeship:
		case NodeType.AntiSpeedship:
		case NodeType.AntiCaptureship:
		case NodeType.Magicstar:
		case NodeType.Hiddenstar:
		case NodeType.Inhibit:
		case NodeType.Sacrifice:
		case NodeType.OverKill:
		case NodeType.CloneTo:
		case NodeType.Treasure:
		case NodeType.UnknownStar:
		case NodeType.Mirror:
			node = new CreatureNode(name);
			break;
		case NodeType.FixedWarpDoor:
			node = new FixedWarpDoorNode(name);
			break;
		case NodeType.Clouds:
			node = new CloudsNode(name);
			break;
		case NodeType.Twist:
			node = new TwistNode(name);
			break;
		case NodeType.AddTower:
			node = new AddTowerNode(name);
			break;
		case NodeType.Lasergun:
			node = new LasergunNode(name);
			break;
		case NodeType.BarrierPoint:
			node = new BarrierPointNode(name);
			break;
		case NodeType.Gunturret:
			node = new GunturretNode(name);
			break;
		case NodeType.Diffusion:
			node = new DiffusionNode(name);
			break;
		case NodeType.Curse:
			node = new CurseNode(name);
			break;
		case NodeType.Cannon:
			node = new CannonNode(name);
			break;
		case NodeType.Heart:
			node = new HeartNode(name);
			break;
		}
		if (node != null)
		{
			node.perfab = perfab;
			node.nodeType = nodeType;
			node.Init();
		}
		return node;
	}

	public void MoveTo(MovePacket packet, TEAM team)
	{
		int count = packet.tags.Count;
		if (count == 0)
		{
			Node node = this.GetNode(packet.from);
			Node node2 = this.GetNode(packet.to);
			if (node == null)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("移动星球为空，from tag：{0}", new object[]
				{
					packet.from
				});
				return;
			}
			if (node2 == null)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("移动星球为空, to tag：{0}", new object[]
				{
					packet.to
				});
				return;
			}
			if (packet.optype == 1)
			{
				this.MoveTo(node, node2, team, 0f, (int)packet.rate);
			}
			else
			{
				this.MoveTo(node, node2, team, packet.rate, 0);
			}
		}
		else
		{
			Node node3 = null;
			List<Node> list = new List<Node>();
			for (int i = 0; i < count; i++)
			{
				Node node4 = this.GetNode(packet.tags[i]);
				if (i == 0)
				{
					node3 = node4;
				}
				else
				{
					list.Add(node4);
				}
			}
			if (packet.optype == 1)
			{
				node3.MoveTo(team, list.ToArray(), (int)packet.rate);
			}
			else
			{
				node3.MoveTo(team, list.ToArray(), packet.rate);
			}
		}
	}

	public void MoveTo(Node from, Node to, TEAM team, float rate = 0f, int num = 0)
	{
		if (from == null || to == null || team == TEAM.Neutral)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("飞船移动，星球不能为空", new object[0]);
			return;
		}
		Team team2 = this.sceneManager.teamManager.GetTeam(from.team);
		if ((from.team == team || this.sceneManager.teamManager.GetTeam(team).IsFriend(from.currentTeam.groupID)) && from.type == NodeType.WarpDoor)
		{
			Solarmax.Singleton<EffectManager>.Get().AddWarpPulse(from, to, team, rate);
			Solarmax.Singleton<AudioManger>.Get().PlayWarpCharge(from.GetPosition());
			this.sceneManager.warpManager.AddWarpItem(from, to, team, rate, num, true);
			return;
		}
		if (from.team == team || this.sceneManager.teamManager.GetTeam(team).IsFriend(from.currentTeam.groupID))
		{
			if (this.sceneManager.IsFixedPortal(from.GetPosition(), to.GetPosition()))
			{
				Solarmax.Singleton<EffectManager>.Get().AddWarpPulse(from, to, team, rate);
				Solarmax.Singleton<AudioManger>.Get().PlayWarpCharge(from.GetPosition());
				this.sceneManager.warpManager.AddWarpItem(from, to, team, rate, num, true);
				return;
			}
		}
		else if (team2.GetAttributeInt(TeamAttr.QuickMove) > 0)
		{
			Solarmax.Singleton<EffectManager>.Get().AddWarpPulse(from, to, team, rate);
			Solarmax.Singleton<AudioManger>.Get().PlayWarpCharge(from.GetPosition());
			this.sceneManager.warpManager.AddWarpItem(from, to, team, rate, num, true);
			return;
		}
		if (num == 0)
		{
			from.MoveTo(team, to, rate, false);
		}
		else
		{
			from.MoveTo(team, to, num, false);
		}
	}

	public Node GetNode(string tag)
	{
		Node result = null;
		this.nodeMap.TryGetValue(tag, out result);
		return result;
	}

	public List<Node> GetUsefulNodeList()
	{
		return this.usefulNodeList;
	}

	public List<Node> GetBarrierNodeList()
	{
		return this.barrierNodeList;
	}

	public List<Node> GetGlobalNodeList()
	{
		return this.globalNodeList;
	}

	public int GetNodeCount()
	{
		return this.nodeMap.Count;
	}

	public bool OnlyTeam(int eTEAM)
	{
		int groupID = this.sceneManager.teamManager.GetTeam((TEAM)eTEAM).groupID;
		for (int i = 0; i < this.usefulNodeList.Count; i++)
		{
			Node node = this.usefulNodeList[i];
			if (node.type != NodeType.BarrierLine && (int)node.team != eTEAM && node.team != TEAM.Neutral && !node.currentTeam.IsFriend(groupID))
			{
				return false;
			}
		}
		return true;
	}

	public bool AllOccupied(int eTEAM)
	{
		int groupID = this.sceneManager.teamManager.GetTeam((TEAM)eTEAM).groupID;
		for (int i = 0; i < this.usefulNodeList.Count; i++)
		{
			Node node = this.usefulNodeList[i];
			if ((int)node.team != eTEAM && !node.currentTeam.IsFriend(groupID))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsOccupiedPlanet(int eTEAM, string tag)
	{
		if (tag == string.Empty)
		{
			return false;
		}
		for (int i = 0; i < this.usefulNodeList.Count; i++)
		{
			Node node = this.usefulNodeList[i];
			if ((int)node.team == eTEAM && node.tag == tag)
			{
				return true;
			}
		}
		return false;
	}

	public int OccupiedSomeone(int eTEAM, string tag1, string tag2)
	{
		if (tag1 == string.Empty && tag2 == string.Empty)
		{
			return -1;
		}
		if (eTEAM > 0)
		{
			bool flag = false;
			bool flag2 = true;
			if (tag2 != string.Empty)
			{
				flag2 = false;
			}
			int groupID = this.sceneManager.teamManager.GetTeam((TEAM)eTEAM).groupID;
			for (int i = 0; i < this.usefulNodeList.Count; i++)
			{
				Node node = this.usefulNodeList[i];
				if ((int)node.team == eTEAM || node.currentTeam.IsFriend(groupID))
				{
					if (node.tag == tag1)
					{
						flag = true;
					}
					if (node.tag == tag2)
					{
						flag2 = true;
					}
				}
				if (flag && flag2)
				{
					break;
				}
			}
			if (flag && flag2)
			{
				return eTEAM;
			}
		}
		else
		{
			Node node2 = null;
			Node node3 = null;
			for (int j = 0; j < this.usefulNodeList.Count; j++)
			{
				Node node4 = this.usefulNodeList[j];
				if (node4.tag == tag1)
				{
					node2 = node4;
				}
				if (tag2 != string.Empty && node4.tag == tag2)
				{
					node3 = node4;
				}
			}
			if (node2 == null)
			{
				return -1;
			}
			if (tag2 == string.Empty)
			{
				return (int)node2.team;
			}
			if (node3 == null)
			{
				return -1;
			}
			int groupID2 = this.sceneManager.teamManager.GetTeam(node2.team).groupID;
			if (node3.currentTeam.IsFriend(groupID2))
			{
				return (int)node2.team;
			}
		}
		return -1;
	}

	public bool CheckHaveNode(int eTEAM)
	{
		for (int i = 0; i < this.usefulNodeList.Count; i++)
		{
			Node node = this.usefulNodeList[i];
			if ((int)node.team == eTEAM)
			{
				return true;
			}
		}
		return false;
	}

	public int CheckHaveNodeCount(int eTEAM)
	{
		int num = 0;
		for (int i = 0; i < this.usefulNodeList.Count; i++)
		{
			Node node = this.usefulNodeList[i];
			if ((int)node.team == eTEAM)
			{
				num++;
			}
		}
		return num;
	}

	public bool CheckHaveProduceNode(int eTEAM)
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < this.usefulNodeList.Count; i++)
		{
			Node node = this.usefulNodeList[i];
			if ((int)node.team == eTEAM)
			{
				NodeType type = node.type;
				if (type != NodeType.Planet && type != NodeType.Castle)
				{
					if (type == NodeType.Twist)
					{
						flag2 = true;
						goto IL_71;
					}
					if (type == NodeType.AddTower)
					{
						flag = true;
						goto IL_71;
					}
					if (type != NodeType.Diffusion)
					{
						goto IL_71;
					}
				}
				return true;
			}
			IL_71:;
		}
		return flag2 && flag;
	}

	public void SilentMode(bool status)
	{
		foreach (KeyValuePair<string, Node> keyValuePair in this.nodeMap)
		{
			keyValuePair.Value.entity.SilentMode(status);
		}
	}

	/// <summary>
	/// 移除一个障碍点(Barrier/BarrierPoint)并连带清除所有以它为端点的障碍线(BarrierLine)与 BarrierPair 记录。
	/// </summary>
	public void RemoveBarrierAndLines(string tag)
	{
		for (int i = this.BarriersKY.Count - 1; i >= 0; i--)
		{
			NodeManager.BarrierPair pair = this.BarriersKY[i];
			if (pair.barrierX == tag || pair.barrierY == tag)
			{
				for (int j = 0; j < pair.barrierLinetags.Count; j++)
				{
					this.RemoveNode(pair.barrierLinetags[j].ToString());
				}
				this.BarriersKY.RemoveAt(i);
			}
		}
		this.RemoveNode(tag);
	}

	/// <summary>
	/// 摧毁线漂移：让障碍线的碰撞/选中判定数据（BarrierPair.keyPos/valuePos 及固定传送门 keyPos）
	/// 随星图一起平移，否则障碍线视觉在动、寻路/点选判定还停留在原位。
	/// </summary>
	public void ApplyDestroyLineDrift(Vector2 delta)
	{
		Vector3 d = new Vector3(delta.x, delta.y, 0f);
		for (int i = 0; i < this.BarriersKY.Count; i++)
		{
			NodeManager.BarrierPair pair = this.BarriersKY[i];
			pair.keyPos += d;
			pair.valuePos += d;
			pair.keyLocalPos += d;
			pair.valuedLocalPos += d;
		}
		for (int i = 0; i < this.mapPortal.Count; i++)
		{
			NodeManager.FixedPortal portal = this.mapPortal[i];
			portal.keyPos += d;
		}
	}

	public void RemoveUnusedBarrier()
	{
		foreach (Node node in this.usefulNodeList)
		{
			bool flag = false;
			int num = 0;
			while (!flag)
			{
				int num2 = -1;
				foreach (Node node2 in this.usefulNodeList)
				{
					if (node != node2)
					{
						if (node.type == NodeType.WarpDoor)
						{
							flag = true;
							break;
						}
						num2 = this.IntersectBarrierLine(node.GetPosition(), node2.GetPosition());
						if (num2 == -1)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					this.MoveOutBarrierLines(num2);
				}
				num++;
				if (num >= 5)
				{
					break;
				}
			}
			if (num >= 5)
			{
				this.MoveOutAllBarrierLines();
				break;
			}
		}
	}

	public void RemoveALLBarriers()
	{
		this.MoveOutAllBarrierLines();
	}

	private int BarrierLinetag = 1024;

	private List<NodeManager.BarrierPair> BarriersKY = new List<NodeManager.BarrierPair>();

	public List<MapBuildingConfig> dynamicBarriers = new List<MapBuildingConfig>();

	private List<NodeManager.FixedPortal> mapPortal = new List<NodeManager.FixedPortal>();

	private Dictionary<string, Node> nodeMap;

	private List<Node> usefulNodeList;

	private List<Node> barrierNodeList;

	private List<Node> globalNodeList;

	private class BarrierPair
	{
		public Vector3 keyPos;

		public Vector3 keyLocalPos;

		public Vector3 valuePos;

		public Vector3 valuedLocalPos;

		public List<int> barrierLinetags = new List<int>();

		public string barrierX;

		public string barrierY;

		public TEAM barrierTeam;

		public bool dynamic;

		public bool show;
	}

	private class FixedPortal
	{
		public Vector3 keyPos;

		public float fAngle;

		public float fRadius;
	}
}
