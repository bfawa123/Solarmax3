using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LasergunEffectNode : EffectNode
{
	public Node hoodEntity { get; set; }

	public float lifeTime { get; set; }

	private LineRenderer lineRender { get; set; }

	private Node mirrorNode { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateHalo1(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		this.color = Color.white;
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.hoodEntity == null)
		{
			return;
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_LaserLineNew0.1");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			this.lineRender = base.go.GetComponentInChildren<LineRenderer>();
		}
		base.go.SetActive(true);
		this.width = 0.2f;
		if (this.count > 59)
		{
			this.width = 0.6f;
		}
		else if (this.count > 29)
		{
			this.width = 0.4f;
		}
		this.power = this.width * 300f * LasergunEffectNode.powerFactor;
		if (this.setwidth <= 0f)
		{
			this.setwidth = this.width;
		}
		this.endPosition = Vector3.zero;
		this.currLength = 0.02f;
		this.headangle = 3.1415927f * this.hoodEntity.GetNodeImageAngle() / 180f;
		this.tailangle = this.headangle;
		this.endPosition.x = this.beginPosition.x + this.currLength * Mathf.Cos(this.headangle);
		this.endPosition.y = this.beginPosition.y + this.currLength * Mathf.Sin(this.headangle);
		if (this.lineRender != null)
		{
			this.lineRender.enabled = true;
			this.lineRender.sortingLayerID = SortingLayer.NameToID("Node");
			this.lineRender.sortingOrder = 1;
			this.lineRender.positionCount = 2;
			this.lineRender.SetPosition(0, this.beginPosition);
			this.lineRender.SetPosition(1, this.endPosition);
			this.lineRender.startWidth = this.setwidth;
			this.lineRender.endWidth = this.setwidth;
		}
		base.isActive = true;
		this.mirrorNode = null;
	}

	private float GetCross(Vector3 p1, Vector3 p2, Vector3 p)
	{
		return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
	}

	private bool IsPointInMatrix(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p)
	{
		LoggerSystem.CodeComments("代码注释--判断点是否位于由p1,2,3,4四个点围成的四边形内，四个点的排列顺序为：从左上角开始顺时针为1，2，3，4");
		return (p.x >= p1.x || p.x >= p4.x) && (p.x <= p2.x || p.x <= p3.x) && (p.y <= p1.y || p.y <= p2.y) && (p.y >= p3.y || p.y >= p4.y) && this.GetCross(p1, p2, p) * this.GetCross(p3, p4, p) >= 0f && this.GetCross(p2, p3, p) * this.GetCross(p4, p1, p) >= 0f;
	}

	private float pointToLine(Vector3 x, Vector3 y, Vector3 po)
	{
		float num = Mathf.Sqrt((x.x - y.x) * (x.x - y.x) + (x.y - y.y) * (x.y - y.y));
		float num2 = Mathf.Sqrt((x.x - po.x) * (x.x - po.x) + (x.y - po.y) * (x.y - po.y));
		float num3 = Mathf.Sqrt((y.x - po.x) * (y.x - po.x) + (y.y - po.y) * (y.y - po.y));
		if ((double)num3 <= 1E-06 || (double)num2 <= 1E-06)
		{
			return 0f;
		}
		if ((double)num <= 1E-06)
		{
			return num2;
		}
		if (num3 * num3 >= num * num + num2 * num2)
		{
			return num2;
		}
		if (num2 * num2 >= num * num + num3 * num3)
		{
			return num3;
		}
		float num4 = (num + num2 + num3) / 2f;
		float num5 = Mathf.Sqrt(num4 * (num4 - num) * (num4 - num2) * (num4 - num3));
		return 2f * num5 / num;
	}

	private bool IsSegmentInCircle(Vector3 p1, Vector3 p2, Vector3 circle, float radius)
	{
		Vector3 vector = p1 - circle;
		Vector3 vector2 = p2 - circle;
		return vector.magnitude < radius || vector2.magnitude < radius || (((p1.x < circle.x && p2.x > circle.x) || (p1.x > circle.x && p2.x < circle.x)) && this.pointToLine(p1, p2, circle) < radius);
	}

	public bool IsSegmentIntersectionWithSegment(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, out Vector3 p)
	{
		p = Vector3.zero;
		float num = p1.x - p0.x;
		float num2 = p1.y - p0.y;
		float num3 = p3.x - p2.x;
		float num4 = p3.y - p2.y;
		float num5 = num * num4 - num3 * num2;
		if (num5 == 0f)
		{
			return false;
		}
		bool flag = num5 > 0f;
		float num6 = p0.x - p2.x;
		float num7 = p0.y - p2.y;
		float num8 = num * num7 - num2 * num6;
		if (num8 < 0f == flag)
		{
			return false;
		}
		float num9 = num3 * num7 - num4 * num6;
		if (num9 < 0f == flag)
		{
			return false;
		}
		if (Mathf.Abs(num8) > Mathf.Abs(num5) || Mathf.Abs(num9) > Mathf.Abs(num5))
		{
			return false;
		}
		float num10 = num9 / num5;
		p.x = p0.x + num10 * num;
		p.y = p0.y + num10 * num2;
		return true;
	}

	private void UpdateHalo(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		this.lifeTime -= dt;
		if (this.lifeTime <= 0f)
		{
			this.beginLength += dt * this.speedRatio;
			if (this.beginLength >= this.currLength)
			{
				base.isActive = false;
				base.Recycle(this);
				return;
			}
			zero.x = this.beginPosition.x + this.beginLength * Mathf.Cos(this.headangle);
			zero.y = this.beginPosition.y + this.beginLength * Mathf.Sin(this.headangle);
		}
		else
		{
			this.currLength += dt * this.speedRatio;
			zero = this.beginPosition;
			if (this.currLength > this.totalLength)
			{
				this.beginLength = this.currLength - this.totalLength;
				zero.x = this.beginPosition.x + this.beginLength * Mathf.Cos(this.headangle);
				zero.y = this.beginPosition.y + this.beginLength * Mathf.Sin(this.headangle);
			}
		}
		zero2.x = this.beginPosition.x + this.currLength * Mathf.Cos(this.headangle);
		zero2.y = this.beginPosition.y + this.currLength * Mathf.Sin(this.headangle);
		this.lineRender.SetPosition(0, zero);
		this.lineRender.SetPosition(1, zero2);
		LineRenderer lineRender = this.lineRender;
		Color color = this.color;
		this.lineRender.endColor = color;
		lineRender.startColor = color;
		float num = this.setwidth / 2f;
		Vector3 p4;
		Vector3 p3;
		Vector3 p2;
		Vector3 p = p2 = (p3 = (p4 = Vector3.zero));
		if (zero.x < zero2.x)
		{
			p = (p2 = zero);
			p3 = (p4 = zero2);
			p.y += num;
			p2.y -= num;
			p3.y += num;
			p4.y -= num;
		}
		else if (zero.x > zero2.x)
		{
			p = (p2 = zero2);
			p3 = (p4 = zero);
			p.y += num;
			p2.y -= num;
			p3.y += num;
			p4.y -= num;
		}
		else
		{
			p.x = (p2.x = zero.x - num);
			p3.x = (p4.x = zero.x + num);
			p.y = (p3.y = ((zero.y <= zero2.y) ? zero2.y : zero.y));
			p2.y = (p4.y = ((zero.y >= zero2.y) ? zero2.y : zero.y));
		}
		if (this.hoodEntity != null && this.power > 0f && this.team != TEAM.Neutral)
		{
			float num2 = this.power;
			new List<Ship>();
			for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
			{
				if ((int)this.team != i)
				{
					Team team = this.hoodEntity.sceneManager.teamManager.GetTeam((TEAM)i);
					if (!this.hoodEntity.sceneManager.teamManager.GetTeam(this.team).IsFriend(team.groupID))
					{
						int j = 0;
						while (num2 > 0f)
						{
							List<Ship> flyShip = this.hoodEntity.sceneManager.shipManager.GetFlyShip((TEAM)i);
							while (j < flyShip.Count)
							{
								if (this.IsPointInMatrix(p, p3, p4, p2, flyShip[j].GetPosition()))
								{
									if (num2 >= flyShip[j].hp)
									{
										num2 -= flyShip[j].hp;
										flyShip[j].hp = 0f;
										flyShip[j].Bomb(NodeType.None);
										this.hoodEntity.nodeManager.sceneManager.teamManager.AddDestory((TEAM)i);
										this.hoodEntity.nodeManager.sceneManager.teamManager.AddHitShip(this.team);
										break;
									}
									flyShip[j].hp -= num2;
									num2 = 0f;
									break;
								}
								else
								{
									j++;
								}
							}
							if (j >= flyShip.Count)
							{
								break;
							}
						}
						if (num2 <= 0f)
						{
							return;
						}
					}
				}
			}
			if (num2 > 0f)
			{
				List<Node> usefulNodeList = this.hoodEntity.sceneManager.nodeManager.GetUsefulNodeList();
				List<Node> list = new List<Node>();
				for (int k = 0; k < usefulNodeList.Count; k++)
				{
					Node node = usefulNodeList[k];
					if (node.CanBeTarget() && node.type != NodeType.Barrier && node.type != NodeType.BarrierLine && node.type != NodeType.Curse && node != this.hoodEntity && this.IsSegmentInCircle(zero, zero2, node.GetPosition(), node.GetWidth()))
					{
						list.Add(node);
					}
				}
				if (list.Count > 0)
				{
					for (int l = 0; l < list.Count; l++)
					{
						Node node2 = list[l];
						float dmg = num2 / (float)list.Count / (float)node2.GetTeams(this.team);
						for (int m = 1; m < LocalPlayer.MaxTeamNum; m++)
						{
							if ((int)this.team != m)
							{
								Team team2 = this.hoodEntity.sceneManager.teamManager.GetTeam((TEAM)m);
								if (!this.hoodEntity.sceneManager.teamManager.GetTeam(this.team).IsFriend(team2.groupID))
								{
									node2.DamageToShip(dmg, m, (int)this.team);
								}
							}
						}
					}
				}
			}
		}
	}

	private void UpdateHalo1(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		this.lifeTime -= dt;
		dt *= this.speedRatio;
		Vector3 zero;
		Vector3 p3;
		Vector3 p2;
		Vector3 p = p2 = (p3 = (zero = Vector3.zero));
		if (this.lifeTime > 0f)
		{
			p2.x = this.beginPosition.x + dt * Mathf.Cos(this.headangle);
			p2.y = this.beginPosition.y + dt * Mathf.Sin(this.headangle);
			Vector3 vector = this.beginPosition;
			int i = 0;
			float num = 0f;
			this.mirrorNode = null;
			if (this.reflex)
			{
				List<Node> usefulNodeList = this.hoodEntity.sceneManager.nodeManager.GetUsefulNodeList();
				while (i < usefulNodeList.Count)
				{
					Node node = usefulNodeList[i];
					if (node.type == NodeType.Mirror)
					{
						num = 3.1415927f * node.GetNodeImageAngle() / 180f;
						Vector3 position = node.GetPosition();
						float num2 = node.GetWidth() / 2f;
						p.x = position.x + num2 * Mathf.Cos(num);
						p.y = position.y + num2 * Mathf.Sin(num);
						p3.x = position.x - num2 * Mathf.Cos(num);
						p3.y = position.y - num2 * Mathf.Sin(num);
						if (this.IsSegmentIntersectionWithSegment(p2, vector, p, p3, out zero))
						{
							this.mirrorNode = node;
							break;
						}
					}
					i++;
				}
			}
			if (this.mirrorNode != null)
			{
				this.isReflex = true;
				if (this.team == this.mirrorNode.currentTeam.team || this.hoodEntity.sceneManager.teamManager.GetTeam(this.team).IsFriend(this.mirrorNode.currentTeam.groupID))
				{
					LasergunEffectNode.middlePoint item;
					item.tag = this.mirrorNode.tag;
					item.angle = num;
					item.position = zero;
					this.middlePoints.Add(item);
					this.headangle = num + num - this.headangle;
					while (this.headangle > 6.2831855f)
					{
						this.headangle -= 6.2831855f;
					}
					while (this.headangle < 0f)
					{
						this.headangle += 6.2831855f;
					}
					float num3 = dt - (zero - vector).magnitude;
					if (num3 > 0f)
					{
						p2.x = zero.x + num3 * Mathf.Cos(this.headangle);
						p2.y = zero.y + num3 * Mathf.Sin(this.headangle);
					}
					else
					{
						p2.x = zero.x + 0.001f * Mathf.Cos(this.headangle);
						p2.y = zero.y + 0.001f * Mathf.Sin(this.headangle);
					}
				}
			}
			this.beginPosition = p2;
			this.currLength += dt;
			if (this.currLength > this.totalLength)
			{
				float num4 = this.currLength - this.totalLength;
				this.currLength = this.totalLength;
				while (this.middlePoints.Count > 0)
				{
					Vector3 vector2 = this.endPosition - this.middlePoints[0].position;
					if (vector2.magnitude > num4)
					{
						break;
					}
					num4 -= vector2.magnitude;
					this.endPosition = this.middlePoints[0].position;
					this.tailangle = this.middlePoints[0].angle + this.middlePoints[0].angle - this.tailangle;
					while (this.tailangle > 6.2831855f)
					{
						this.tailangle -= 6.2831855f;
					}
					while (this.tailangle < 0f)
					{
						this.tailangle += 6.2831855f;
					}
					this.middlePoints.Remove(this.middlePoints[0]);
				}
				this.endPosition.x = this.endPosition.x + num4 * Mathf.Cos(this.tailangle);
				this.endPosition.y = this.endPosition.y + num4 * Mathf.Sin(this.tailangle);
			}
		}
		else
		{
			this.currLength -= dt;
			if (this.currLength <= 0f)
			{
				base.isActive = false;
				this.isReflex = false;
				base.Recycle(this);
				return;
			}
			while (this.middlePoints.Count > 0)
			{
				Vector3 vector3 = this.endPosition - this.middlePoints[0].position;
				if (vector3.magnitude > dt)
				{
					break;
				}
				dt -= vector3.magnitude;
				this.endPosition = this.middlePoints[0].position;
				this.tailangle = this.middlePoints[0].angle + this.middlePoints[0].angle - this.tailangle;
				while (this.tailangle > 6.2831855f)
				{
					this.tailangle -= 6.2831855f;
				}
				while (this.tailangle < 0f)
				{
					this.tailangle += 6.2831855f;
				}
				this.middlePoints.Remove(this.middlePoints[0]);
			}
			this.endPosition.x = this.endPosition.x + dt * Mathf.Cos(this.tailangle);
			this.endPosition.y = this.endPosition.y + dt * Mathf.Sin(this.tailangle);
		}
		this.lineRender.positionCount = 2 + this.middlePoints.Count;
		this.lineRender.SetPosition(0, this.endPosition);
		for (int j = 0; j < this.middlePoints.Count; j++)
		{
			this.lineRender.SetPosition(j + 1, this.middlePoints[j].position);
		}
		this.lineRender.SetPosition(1 + this.middlePoints.Count, this.beginPosition);
		LineRenderer lineRender = this.lineRender;
		Color color = this.color;
		this.lineRender.endColor = color;
		lineRender.startColor = color;
		if (this.hoodEntity != null && this.power > 0f && this.team != TEAM.Neutral)
		{
			List<LasergunEffectNode.middlePoint> list = new List<LasergunEffectNode.middlePoint>();
			LasergunEffectNode.middlePoint middlePoint;
			middlePoint.tag = string.Empty;
			middlePoint.position = this.endPosition;
			middlePoint.angle = 0f;
			LasergunEffectNode.middlePoint item2 = middlePoint;
			item2.position = this.beginPosition;
			list.Add(middlePoint);
			for (int k = 0; k < this.middlePoints.Count; k++)
			{
				list.Add(this.middlePoints[k]);
			}
			list.Add(item2);
			float num5 = this.power;
			LoggerSystem.CodeComments("代码注释--激光的伤害计算-位置判断部分：list中存储着激光一路上的中心点坐标，以下向量计算坐标并且判断点是否在四边形内以施加伤害");
			for (int l = 0; l < list.Count - 1; l++)
			{
				float num6 = LasergunEffectNode.scopeFactor * this.setwidth / 2f;
				Vector3 p7;
				Vector3 p6;
				Vector3 p5;
				Vector3 p4 = p5 = (p6 = (p7 = Vector3.zero));
				if (list[l].position.x < list[l + 1].position.x)
				{
					p4 = (p5 = list[l].position);
					p7 = (p6 = list[l + 1].position);
					p5.y += num6;
					p4.y -= num6;
					p6.y += num6;
					p7.y -= num6;
				}
				else if (list[l].position.x > list[l + 1].position.x)
				{
					p4 = (p5 = list[l + 1].position);
					p7 = (p6 = list[l].position);
					p5.y += num6;
					p4.y -= num6;
					p6.y += num6;
					p7.y -= num6;
				}
				else
				{
					p5.x = (p4.x = list[l].position.x - num6);
					p6.x = (p7.x = list[l].position.x + num6);
					p5.y = (p6.y = ((list[l].position.y <= list[l + 1].position.y) ? list[l + 1].position.y : list[l].position.y));
					p4.y = (p7.y = ((list[l].position.y >= list[l + 1].position.y) ? list[l + 1].position.y : list[l].position.y));
				}
				new List<Ship>();
				for (int m = 1; m < LocalPlayer.MaxTeamNum; m++)
				{
					if ((int)this.team != m)
					{
						Team team = this.hoodEntity.sceneManager.teamManager.GetTeam((TEAM)m);
						if (!this.hoodEntity.sceneManager.teamManager.GetTeam(this.team).IsFriend(team.groupID))
						{
							int n = 0;
							while (num5 > 0f)
							{
								List<Ship> flyShip = this.hoodEntity.sceneManager.shipManager.GetFlyShip((TEAM)m);
								while (n < flyShip.Count)
								{
									if (this.IsPointInMatrix(p5, p6, p7, p4, flyShip[n].GetPosition()))
									{
										if (num5 >= flyShip[n].hp)
										{
											num5 -= flyShip[n].hp;
											flyShip[n].hp = 0f;
											flyShip[n].Bomb(NodeType.None);
											this.hoodEntity.nodeManager.sceneManager.teamManager.AddDestory((TEAM)m);
											this.hoodEntity.nodeManager.sceneManager.teamManager.AddHitShip(this.team);
											break;
										}
										flyShip[n].hp -= num5;
										num5 = 0f;
										break;
									}
									else
									{
										n++;
									}
								}
								if (n >= flyShip.Count)
								{
									break;
								}
							}
							if (num5 <= 0f)
							{
								return;
							}
						}
					}
				}
				if (num5 > 0f)
				{
					List<Node> usefulNodeList2 = this.hoodEntity.sceneManager.nodeManager.GetUsefulNodeList();
					List<Node> list2 = new List<Node>();
					for (int num7 = 0; num7 < usefulNodeList2.Count; num7++)
					{
						Node node2 = usefulNodeList2[num7];
						if (node2.CanBeTarget() && node2.type != NodeType.Barrier && node2.type != NodeType.BarrierLine && node2.type != NodeType.Curse && (node2 != this.hoodEntity || this.isReflex) && this.IsSegmentInCircle(list[l].position, list[l + 1].position, node2.GetPosition(), node2.GetWidth()))
						{
							list2.Add(node2);
						}
					}
					if (list2.Count > 0)
					{
						for (int num8 = 0; num8 < list2.Count; num8++)
						{
							Node node3 = list2[num8];
							float dmg = num5 / (float)list2.Count / (float)node3.GetTeams(this.team);
							for (int num9 = 1; num9 < LocalPlayer.MaxTeamNum; num9++)
							{
								if ((int)this.team != num9)
								{
									Team team2 = this.hoodEntity.sceneManager.teamManager.GetTeam((TEAM)num9);
									if (!this.hoodEntity.sceneManager.teamManager.GetTeam(this.team).IsFriend(team2.groupID))
									{
										node3.DamageToShip(dmg, num9, (int)this.team);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public TEAM team;

	public Color color = Color.white;

	public Vector3 beginPosition;

	public Vector3 endPosition;

	public int count = 1;

	private float power = 8f;

	private float width = 0.8f;

	public float setwidth;

	public float totalLength = 3f;

	private float currLength;

	private float beginLength;

	private float headangle;

	private float tailangle;

	public float speedRatio = 10f;

	private List<LasergunEffectNode.middlePoint> middlePoints = new List<LasergunEffectNode.middlePoint>();

	public bool reflex = true;

	private bool isReflex;

	private static float powerFactor = 2f;

	private static float scopeFactor = 3f;

	private struct middlePoint
	{
		public string tag;

		public Vector3 position;

		public float angle;
	}
}
