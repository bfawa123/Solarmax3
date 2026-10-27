using System;
using System.Collections.Generic;
using System.Globalization;
using Solarmax;
using UnityEngine;

public class NodeAttribute : MonoBehaviour
{
	public static int GetSize(int type, float scale)
	{
		List<MapNodeConfig> allData = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetAllData();
		foreach (MapNodeConfig mapNodeConfig in allData)
		{
			if (mapNodeConfig.typeEnum == type)
			{
				if (mapNodeConfig.size == scale)
				{
					return mapNodeConfig.sizeType;
				}
			}
		}
		return 0;
	}

	public static float GetScale(int type, int size)
	{
		List<MapNodeConfig> allData = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetAllData();
		foreach (MapNodeConfig mapNodeConfig in allData)
		{
			if (mapNodeConfig.typeEnum == type)
			{
				if (mapNodeConfig.sizeType == size)
				{
					return mapNodeConfig.size;
				}
			}
		}
		return 0f;
	}

	public void SetNode(Node node)
	{
		if (node == null)
		{
			return;
		}
		if (NodeAttribute.current != null && NodeAttribute.current.entity.GetGO() != null)
		{
			Solarmax.Singleton<EffectManager>.Get().HideSelectEffect(NodeAttribute.current);
			NodeAttribute.current.ShowRange(false);
		}
		NodeAttribute.current = null;
		float scale = node.GetScale();
		int size = NodeAttribute.GetSize((int)node.type, scale);
		List<MapNodeConfig> allData = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetAllData();
		this.popSize.Clear();
		foreach (MapNodeConfig mapNodeConfig in allData)
		{
			if (mapNodeConfig.typeEnum == (int)node.type)
			{
				this.popSize.AddItem(mapNodeConfig.sizeType.ToString());
			}
		}
		this.inTag.value = node.tag;
		this.popCamp.value = node.team.ToString();
		this.popSize.value = size.ToString();
		this.inX.value = node.GetPosition().x.ToString();
		this.inY.value = node.GetPosition().y.ToString();
		this.inShipNum.value = node.GetShipCountEdit((int)node.team).ToString();
		if (node is BarrierNode)
		{
			this.sprite.height = 580;
			this.barrierRoot.SetActive(true);
			this.angleRoot.SetActive(false);
			this.transformIDRoot.SetActive(false);
			this.lasergunRoot.SetActive(false);
			this.barrierpointRoot.SetActive(false);
			this.curseDelayRoot.SetActive(false);
			BoxCollider component = this.sprite.GetComponent<BoxCollider>();
			component.size = new Vector3(300f, 580f, 1f);
			component.center = new Vector3(0f, -290f, 0f);
			MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
			this.popBarrier.Clear();
			if (currentTable != null && currentTable.mlcList != null)
			{
				foreach (MapLineConfig mapLineConfig in currentTable.mlcList)
				{
					if (!(mapLineConfig.point1 != node.tag))
					{
						this.popBarrier.AddItem(mapLineConfig.point2);
					}
				}
			}
			if (this.popBarrier.itemData != null && this.popBarrier.items.Count >= 1)
			{
				this.popBarrier.value = this.popBarrier.items[0];
			}
			else
			{
				this.popBarrier.value = string.Empty;
			}
			this.inBarrier.value = string.Empty;
		}
		else if (node.nodeType == NodeType.UnknownStar)
		{
			this.barrierRoot.SetActive(false);
			this.angleRoot.SetActive(false);
			this.transformIDRoot.SetActive(true);
			this.lasergunRoot.SetActive(false);
			this.barrierpointRoot.SetActive(false);
			this.curseDelayRoot.SetActive(false);
			this.sprite.height = 440;
		}
		else if (node.nodeType == NodeType.BarrierPoint)
		{
			this.barrierRoot.SetActive(false);
			this.angleRoot.SetActive(false);
			this.transformIDRoot.SetActive(false);
			this.lasergunRoot.SetActive(false);
			this.barrierpointRoot.SetActive(true);
			this.curseDelayRoot.SetActive(false);
			this.sprite.height = 460;
		}
		else if (node is LasergunNode)
		{
			this.barrierRoot.SetActive(false);
			this.angleRoot.SetActive(false);
			this.transformIDRoot.SetActive(false);
			this.lasergunRoot.SetActive(true);
			this.barrierpointRoot.SetActive(false);
			this.curseDelayRoot.SetActive(false);
			this.sprite.height = 580;
		}
		else if (node.nodeType == NodeType.Curse)
		{
			this.barrierRoot.SetActive(false);
			this.angleRoot.SetActive(false);
			this.transformIDRoot.SetActive(false);
			this.lasergunRoot.SetActive(false);
			this.barrierpointRoot.SetActive(false);
			this.curseDelayRoot.SetActive(true);
			this.sprite.height = 440;
		}
		else
		{
			this.sprite.height = 440;
			this.barrierRoot.SetActive(false);
			this.transformIDRoot.SetActive(false);
			this.lasergunRoot.SetActive(false);
			this.barrierpointRoot.SetActive(false);
			this.curseDelayRoot.SetActive(false);
			if (node.nodeType == NodeType.FixedWarpDoor || node.nodeType == NodeType.Mirror)
			{
				this.angleRoot.SetActive(true);
				this.sprite.height = 460;
				this.angle.value = node.nodeAngle.ToString();
			}
			else
			{
				this.angleRoot.SetActive(false);
			}
			BoxCollider component2 = this.sprite.GetComponent<BoxCollider>();
			component2.size = new Vector3(300f, 440f, 1f);
			component2.center = new Vector3(0f, -220f, 0f);
		}
		string[] names = Enum.GetNames(typeof(RevolutionType));
		this.popOrbit.Clear();
		foreach (string text in names)
		{
			this.popOrbit.AddItem(text);
		}
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == node.tag);
		this.popOrbit.value = Enum.GetName(typeof(RevolutionType), (RevolutionType)mapBuildingConfig.orbit);
		this.orbitX.value = mapBuildingConfig.orbitParam1;
		this.orbitY.value = mapBuildingConfig.orbitParam2;
		this.orbitCW.value = mapBuildingConfig.orbitClockWise;
		this.transformBuildingIds.value = mapBuildingConfig.transformBulidingID;
		this.angle.value = mapBuildingConfig.fAngle.ToString();
		this.popLasergunAngle.value = mapBuildingConfig.lasergunAngle.ToString();
		this.popLasergunRange.value = mapBuildingConfig.lasergunRange.ToString();
		this.popLasergunRotateSkip.value = mapBuildingConfig.lasergunRotateSkip.ToString();
		this.curseDelay.value = mapBuildingConfig.curseDelay.ToString();
		if (!string.IsNullOrEmpty(mapBuildingConfig.aistrategy))
		{
			this.AIStrategy.value = mapBuildingConfig.aistrategy.ToString();
		}
		this.popBarrierPointRange.value = mapBuildingConfig.fbpRange.ToString();
		Solarmax.Singleton<EffectManager>.Get().ShowSelectEffect(node);
		node.ShowRange(true);
		NodeAttribute.current = node;
		this.clickTime = Time.realtimeSinceStartup;
	}

	public void AddBarrier()
	{
		if (string.IsNullOrEmpty(this.inBarrier.value))
		{
			return;
		}
		if (NodeAttribute.current.tag == this.inBarrier.value)
		{
			return;
		}
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		if (currentTable == null)
		{
			return;
		}
		MapBuildingConfig mapBuildingConfig = currentTable.mbcList.Find((MapBuildingConfig b) => b.tag == this.inBarrier.value);
		if (mapBuildingConfig == null)
		{
			return;
		}
		if (mapBuildingConfig.type != "barrier")
		{
			return;
		}
		if (currentTable.mlcList == null)
		{
			currentTable.mlcList = new List<MapLineConfig>();
		}
		MapLineConfig mapLineConfig = currentTable.mlcList.Find((MapLineConfig l) => l.point1 == NodeAttribute.current.tag && l.point2 == this.inBarrier.value);
		if (mapLineConfig != null)
		{
			return;
		}
		mapLineConfig = new MapLineConfig();
		mapLineConfig.point1 = NodeAttribute.current.tag;
		mapLineConfig.point2 = this.inBarrier.value;
		currentTable.mlcList.Add(mapLineConfig);
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.AddBarrierLines(NodeAttribute.current.tag, this.inBarrier.value);
		this.popBarrier.AddItem(this.inBarrier.value);
		this.popBarrier.value = this.inBarrier.value;
		this.inBarrier.value = string.Empty;
	}

	public void DeleteCurrentBarrier()
	{
		if (string.IsNullOrEmpty(this.popBarrier.value))
		{
			return;
		}
		string tag = this.popBarrier.value;
		this.popBarrier.RemoveItem(tag);
		if (this.popBarrier.itemData != null && this.popBarrier.items.Count >= 1)
		{
			this.popBarrier.value = this.popBarrier.items[0];
		}
		else
		{
			this.popBarrier.value = string.Empty;
		}
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		if (currentTable.mlcList != null)
		{
			MapLineConfig mapLineConfig = currentTable.mlcList.Find((MapLineConfig l) => l.point1 == NodeAttribute.current.tag && l.point2 == tag);
			if (mapLineConfig != null)
			{
				currentTable.mlcList.Remove(mapLineConfig);
				this.RefreshBarrierLine();
			}
		}
	}

	public void UpdatePosition(Node node)
	{
		this.inX.value = node.GetPosition().x.ToString();
		this.inY.value = node.GetPosition().y.ToString();
		RevolutionType type = (RevolutionType)Enum.Parse(typeof(RevolutionType), this.popOrbit.value);
		this.UpdateOrbitLine(node, type, this.orbitX.value, this.orbitY.value);
	}

	public void ChangeTeam()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		TEAM team = (TEAM)Enum.Parse(typeof(TEAM), this.popCamp.value);
		NodeAttribute.current.currentTeam = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team);
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		MapBuildingConfig mapBuildingConfig = currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.camption = (int)NodeAttribute.current.team;
		this.ChangeShipNum();
	}

	public void ChangeSize()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		int size = int.Parse(this.popSize.value);
		float scale = NodeAttribute.GetScale((int)NodeAttribute.current.type, size);
		NodeAttribute.current.SetScale(scale);
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		MapBuildingConfig mapBuildingConfig = currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.size = size;
	}

	public void ChangePosition()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		float x;
		float y = x = 0f;
		if (this.inX.value == "-" || this.inY.value == "-")
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.inX.value))
		{
			x = float.Parse(this.inX.value);
		}
		if (!string.IsNullOrEmpty(this.inY.value))
		{
			y = float.Parse(this.inY.value);
		}
		NodeAttribute.current.SetPosition(x, y);
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		MapBuildingConfig mapBuildingConfig = currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.x = x;
		mapBuildingConfig.y = y;
		if (NodeAttribute.current.type == NodeType.Barrier)
		{
			this.RefreshBarrierLine();
		}
	}

	public void RefreshBarrierLine()
	{
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.DestroyBarrierLineNode();
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.CreateLines(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mlcList);
	}

	public void ChangeShipNum()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		int num = 0;
		if (!string.IsNullOrEmpty(this.inShipNum.value))
		{
			num = int.Parse(this.inShipNum.value);
		}
		NodeAttribute.current.RemoveAllShip();
		NodeAttribute.current.AddShip((int)NodeAttribute.current.team, num, true, true);
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		if (currentTable.mpcList == null)
		{
			currentTable.mpcList = new List<MapPlayerConfig>();
		}
		MapPlayerConfig mapPlayerConfig = currentTable.mpcList.Find((MapPlayerConfig i) => i.tag == NodeAttribute.current.tag);
		if (mapPlayerConfig == null)
		{
			mapPlayerConfig = new MapPlayerConfig();
			currentTable.mpcList.Add(mapPlayerConfig);
		}
		mapPlayerConfig.camption = (int)NodeAttribute.current.team;
		mapPlayerConfig.ship = num;
		mapPlayerConfig.tag = NodeAttribute.current.tag;
	}

	public void ChangeOrbitXYAndType()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		RevolutionType revolutionType = (RevolutionType)Enum.Parse(typeof(RevolutionType), this.popOrbit.value);
		string value = this.orbitX.value;
		string value2 = this.orbitY.value;
		if (revolutionType != RevolutionType.RT_Custom)
		{
			if (!string.IsNullOrEmpty(value) && !Converter.CanConvertVector3D(value))
			{
				Debug.Log("Orbit Param1 error!");
				return;
			}
			if (revolutionType == RevolutionType.RT_Ellipse)
			{
				if (string.IsNullOrEmpty(value2) || !Converter.CanConvertVector3D(value2))
				{
					Debug.Log("Ellipse Revolution Param error!");
					return;
				}
			}
			else if (!string.IsNullOrEmpty(value2) && !Converter.CanConvertVector3D(value2))
			{
				Debug.Log("Orbit Param2 error!");
				return;
			}
		}
		NodeAttribute.current.SetRevolution((int)revolutionType, value, value2, string.Empty, string.Empty, this.orbitCW.value);
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.orbit = (int)revolutionType;
		mapBuildingConfig.orbitParam1 = value;
		mapBuildingConfig.orbitParam2 = value2;
		mapBuildingConfig.orbitClockWise = this.orbitCW.value;
		this.UpdateOrbitLine(NodeAttribute.current, revolutionType, value, value2);
	}

	public void ChangeNodeAngle()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string value = this.angle.value;
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.fAngle = float.Parse(value);
		NodeAttribute.current.RotateNodeImage(-NodeAttribute.current.GetNodeImageAngle());
		NodeAttribute.current.RotateNodeImage(mapBuildingConfig.fAngle);
	}

	public void UpdateOrbitLine(Node node, RevolutionType type, string p1, string p2)
	{
		if (type == RevolutionType.RT_None)
		{
			return;
		}
		LineRenderer lineRenderer = node.GetGO().GetComponent<LineRenderer>();
		if (lineRenderer == null)
		{
			lineRenderer = node.GetGO().AddComponent<LineRenderer>();
		}
		Color color = new Color32(204, 204, 204, 102);
		LineRenderer lineRenderer2 = lineRenderer;
		float num = 0.02f;
		lineRenderer.endWidth = num;
		lineRenderer2.startWidth = num;
		LineRenderer lineRenderer3 = lineRenderer;
		Color color2 = color;
		lineRenderer.endColor = color2;
		lineRenderer3.startColor = color2;
		List<Vector3> list = new List<Vector3>();
		if (type == RevolutionType.RT_Custom)
		{
			Func<float, float> func;
			float num2;
			float num3;
			if (EquationFunction.TryCreate(p1, out func) && NodeAttribute.TryParseOrbitRange(p2, out num2, out num3))
			{
				int num4 = NodeAttribute.CalcCustomOrbitSampleCount(num2, num3);
				for (int i = 0; i < num4; i++)
				{
					float t = (num4 <= 1) ? 0f : ((float)i / (float)(num4 - 1));
					float num5 = Mathf.Lerp(num2, num3, t);
					float y = func(num5);
					list.Add(new Vector3(num5, y, -1f));
				}
				for (int j = num4 - 2; j > 0; j--)
				{
					float t2 = (num4 <= 1) ? 0f : ((float)j / (float)(num4 - 1));
					float num6 = Mathf.Lerp(num2, num3, t2);
					float y2 = func(num6);
					list.Add(new Vector3(num6, y2, -1f));
				}
				if (list.Count > 0)
				{
					list.Add(list[0]);
				}
			}
		}
		else
		{
			Vector3 vector = Converter.ConvertVector3D(p1);
			Vector3 vector2 = Converter.ConvertVector3D(p2);
			Vector3 vector3 = Converter.ConvertVector3D(node.revoParam3);
			Vector3 vector4 = Converter.ConvertVector3D(node.revoParam4);
		if (type == RevolutionType.RT_Circular)
		{
			list = this.CalculatePositions(node.GetPosition(), new Vector3(vector.x, vector.y, -1f), this.CalcDragPoint(type));
		}
		else if (type == RevolutionType.RT_Triangle)
		{
			bool flag = !string.IsNullOrEmpty(p1) && (p1.IndexOf(',') >= 0);
			bool flag2 = !string.IsNullOrEmpty(p2) && (p2.IndexOf(',') >= 0);
			bool flag3 = !string.IsNullOrEmpty(node.revoParam3) && (node.revoParam3.IndexOf(',') >= 0);
			bool flag4 = !string.IsNullOrEmpty(node.revoParam4) && (node.revoParam4.IndexOf(',') >= 0);
			if (flag && flag2 && flag3 && !flag4)
			{
				list.Add(new Vector3(vector.x, vector.y, -1f));
				list.Add(new Vector3(vector2.x, vector2.y, -1f));
				list.Add(new Vector3(vector3.x, vector3.y, -1f));
				list.Add(new Vector3(vector.x, vector.y, -1f));
			}
			else if (flag3 && flag4)
			{
				Vector3 position = node.GetPosition();
				position.z = -1f;
				list.Add(position);
				list.Add(new Vector3(vector3.x, vector3.y, -1f));
				list.Add(new Vector3(vector4.x, vector4.y, -1f));
				list.Add(position);
			}
		}
		else if (type == RevolutionType.RT_Quadrilateral)
		{
			bool flag5 = !string.IsNullOrEmpty(p1) && (p1.IndexOf(',') >= 0);
			bool flag6 = !string.IsNullOrEmpty(p2) && (p2.IndexOf(',') >= 0);
			bool flag7 = !string.IsNullOrEmpty(node.revoParam3) && (node.revoParam3.IndexOf(',') >= 0);
			bool flag8 = !string.IsNullOrEmpty(node.revoParam4) && (node.revoParam4.IndexOf(',') >= 0);
			if (flag5 && flag6 && flag7 && flag8)
			{
				list.Add(new Vector3(vector.x, vector.y, -1f));
				list.Add(new Vector3(vector2.x, vector2.y, -1f));
				list.Add(new Vector3(vector3.x, vector3.y, -1f));
				list.Add(new Vector3(vector4.x, vector4.y, -1f));
				list.Add(new Vector3(vector.x, vector.y, -1f));
			}
			else if (flag7 && flag8)
			{
				Vector3 position2 = node.GetPosition();
				position2.z = -1f;
				Vector3 vector5 = new Vector3(vector3.x + vector4.x - position2.x, vector3.y + vector4.y - position2.y, -1f);
				list.Add(position2);
				list.Add(new Vector3(vector3.x, vector3.y, -1f));
				list.Add(vector5);
				list.Add(new Vector3(vector4.x, vector4.y, -1f));
				list.Add(position2);
			}
		}
		else if (type == RevolutionType.RT_Ellipse)
		{
			list = this.CalculatePositionsEllipse(node.GetPosition(), new Vector3(vector.x, vector.y, -1f), new Vector3(vector2.x, vector2.y, -1f), this.CalcDragPoint(type));
		}
		}
		lineRenderer.useWorldSpace = true;
		lineRenderer.SetVertexCount(list.Count);
		lineRenderer.SetPositions(list.ToArray());
	}

	private static bool TryParseOrbitRange(string text, out float x1, out float x2)
	{
		x1 = 0f;
		x2 = 0f;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		string[] array = text.Split(new char[]
		{
			','
		});
		if (array == null || array.Length < 2)
		{
			return false;
		}
		return float.TryParse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x1) && float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out x2);
	}

	private static int CalcCustomOrbitSampleCount(float x1, float x2)
	{
		float num = Mathf.Abs(x2 - x1);
		int num2 = Mathf.RoundToInt(num * 20f) + 2;
		return Mathf.Clamp(num2, 10, 200);
	}

	private int CalcDragPoint(RevolutionType type)
	{
		switch (type)
		{
		case RevolutionType.RT_Circular:
			return 128;
		case RevolutionType.RT_Triangle:
			return 3;
		case RevolutionType.RT_Quadrilateral:
			return 4;
		case RevolutionType.RT_Ellipse:
			return 128;
		default:
			return 0;
		}
	}

	private List<Vector3> CalculatePositions(Vector3 basePos, Vector3 centerPos, int count)
	{
		List<Vector3> list = new List<Vector3>();
		basePos.z = -1f;
		basePos -= centerPos;
		list.Add(basePos);
		float num = 360f / (float)count;
		Quaternion rotation = Quaternion.AngleAxis(num, Vector3.forward);
		for (int i = 1; i < count; i++)
		{
			Vector3 item = rotation * list[i - 1];
			list.Add(item);
		}
		list.Add(basePos);
		for (int j = 0; j < list.Count; j++)
		{
			List<Vector3> list2 = list;
			int index = j;
			(list2 = list)[index = j] = list2[index] + centerPos;
		}
		return list;
	}

	private List<Vector3> CalculatePositionsEllipse(Vector3 basePos, Vector3 pos1, Vector3 pos2, int count)
	{
		basePos.z = -1f;
		pos1.z = -1f;
		pos2.z = -1f;
		List<Vector3> list = new List<Vector3>();
		Vector3 b = (pos1 + pos2) / 2f;
		float num = Vector3.Distance(basePos, pos1) + Vector3.Distance(basePos, pos2);
		float num2 = Vector3.Distance(pos1, pos2);
		float num3 = num / 2f;
		float f = num2 / 2f;
		float num4 = Mathf.Sqrt(Mathf.Pow(num3, 2f) - Mathf.Pow(f, 2f));
		float num5 = 6.2831855f / (float)count;
		for (int i = 0; i <= count; i++)
		{
			float f2 = num5 * (float)i;
			Vector3 vector = Vector3.zero;
			vector.x = num3 * Mathf.Cos(f2);
			vector.y = num4 * Mathf.Sin(f2);
			vector.z = basePos.z;
			vector += b;
			list.Add(vector);
		}
		return list;
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed && base.enabled)
		{
			this.modify = base.gameObject.transform.position - UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!base.enabled)
		{
			return;
		}
		Vector2 b = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
		base.gameObject.transform.position = this.modify + b;
	}

	public void ChangeTransformBuildingID()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string transformBulidingID = string.Empty;
		if (!string.IsNullOrEmpty(this.transformBuildingIds.value))
		{
			transformBulidingID = this.transformBuildingIds.value;
		}
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		MapBuildingConfig mapBuildingConfig = currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.transformBulidingID = transformBulidingID;
	}

	public void ChangeLasergunAngle()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string value = this.popLasergunAngle.value;
		float num;
		for (num = float.Parse(value); num > 360f; num -= 360f)
		{
		}
		while (num < 0f)
		{
			num += 360f;
		}
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.lasergunAngle = num;
		NodeAttribute.current.RotateNodeImage(-NodeAttribute.current.GetNodeImageAngle());
		NodeAttribute.current.RotateNodeImage(num);
		NodeAttribute.current.ResetLasergunShowRange();
	}

	public void ChangeLasergunRange()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string value = this.popLasergunRange.value;
		float num;
		for (num = float.Parse(value); num > 360f; num -= 360f)
		{
		}
		while (num < 0f)
		{
			num += 360f;
		}
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.lasergunRange = num;
		NodeAttribute.current.ResetLasergunShowRange();
	}

	public void ChangeLasergunRotateSkip()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string value = this.popLasergunRotateSkip.value;
		float num;
		for (num = float.Parse(value); num > 360f; num -= 360f)
		{
		}
		while (num < 0f)
		{
			num += 360f;
		}
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.lasergunRotateSkip = num;
		NodeAttribute.current.ResetLasergunShowRange();
	}

	public void ChangeAIStrategy()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string value = this.AIStrategy.value;
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.aistrategy = value;
	}

	public void ChangeBarrierPointRange()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string value = this.popBarrierPointRange.value;
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.fbpRange = float.Parse(value);
	}

	public void ChangeCurseDelay()
	{
		if (NodeAttribute.current == null)
		{
			return;
		}
		string s = string.Empty;
		if (!string.IsNullOrEmpty(this.curseDelay.value))
		{
			s = this.curseDelay.value;
		}
		float num = float.Parse(s);
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		MapBuildingConfig mapBuildingConfig = currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == NodeAttribute.current.tag);
		mapBuildingConfig.curseDelay = num;
	}

	public UIInput inTag;

	public UIPopupList popSize;

	public UIPopupList popCamp;

	public UIInput inShipNum;

	public UIInput inX;

	public UIInput inY;

	public UISprite sprite;

	public GameObject barrierRoot;

	public GameObject angleRoot;

	public GameObject transformIDRoot;

	public GameObject lasergunRoot;

	public GameObject barrierpointRoot;

	public GameObject curseDelayRoot;

	public UIInput inBarrier;

	public UIPopupList popBarrier;

	public UIPopupList popOrbit;

	public UIInput orbitX;

	public UIInput orbitY;

	public UIToggle orbitCW;

	public UIInput angle;

	public UIInput transformBuildingIds;

	public UIInput popLasergunAngle;

	public UIInput popLasergunRange;

	public UIInput popLasergunRotateSkip;

	public UIInput AIStrategy;

	public UIInput popBarrierPointRange;

	public UIInput curseDelay;

	public static Node current;

	public float clickTime;

	private Vector2 modify;
}
