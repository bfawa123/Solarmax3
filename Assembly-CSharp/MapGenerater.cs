using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class MapGenerater : MonoBehaviour
{
	private void Start()
	{
		this.nodeNumInputs.Clear();
		foreach (string text in Enum.GetNames(typeof(NodeType)))
		{
			if (!text.ToLower().Equals("none"))
			{
				if (!text.ToLower().Equals("barrierline"))
				{
					int key = (int)Enum.Parse(typeof(NodeType), text);
					GameObject gameObject = this.nodeGrid.gameObject.AddChild(this.nodeTemplate);
					gameObject.SetActive(true);
					UILabel component = gameObject.transform.Find("typeLabel").GetComponent<UILabel>();
					component.text = text;
					UIInput componentInChildren = gameObject.GetComponentInChildren<UIInput>();
					componentInChildren.value = "0";
					this.nodeNumInputs.Add(key, componentInChildren);
				}
			}
		}
		this.nodeGrid.Reposition();
		this.nodeScrollview.ResetPosition();
	}

	public void ShowMap()
	{
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable = this.generatedMap;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.id;
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.CreateScene(true, Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable, null, false);
	}

	public void OnGenerateBtnClick()
	{
		try
		{
			this.generateSuccessed = false;
			this.generatedMap = new MapConfig("randmap1");
			this.generatedMap.vertical = true;
			Solarmax.Singleton<BattleSystem>.Instance.Reset();
			Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable = this.generatedMap;
			this.nodeNums.Clear();
			this.nodeTag = 'A';
			this.playerHostNode.Clear();
			foreach (KeyValuePair<int, UIInput> keyValuePair in this.nodeNumInputs)
			{
				int num = 0;
				int.TryParse(keyValuePair.Value.value, out num);
				if (num > 0)
				{
					this.nodeNums.Add(keyValuePair.Key, num);
				}
			}
			if (this.nodeNums.Count == 0)
			{
				this.mapEdit.ShowTips("新球配置为空!");
			}
			else
			{
				int num2 = 0;
				int.TryParse(this.playerNumPopuplist.value, out num2);
				if (num2 == 2)
				{
					this.Generate2();
				}
				else if (num2 == 3)
				{
					this.Generate3();
				}
				else
				{
					if (num2 != 4)
					{
						this.mapEdit.ShowTips("请输入玩家数量!");
						return;
					}
					this.Generate4();
				}
				int i;
				for (i = 0; i < this.playerHostNode.Count; i++)
				{
					if (this.generatedMap.mpcList == null)
					{
						this.generatedMap.mpcList = new List<MapPlayerConfig>();
					}
					MapPlayerConfig mapPlayerConfig = new MapPlayerConfig();
					mapPlayerConfig.camption = i + 1;
					mapPlayerConfig.ship = 30;
					mapPlayerConfig.tag = this.playerHostNode[i];
					this.generatedMap.mpcList.Add(mapPlayerConfig);
					MapBuildingConfig mapBuildingConfig = this.generatedMap.mbcList.Find((MapBuildingConfig b) => this.playerHostNode[i].Equals(b.tag));
					mapBuildingConfig.camption = i + 1;
				}
				this.ShowMap();
				this.generateSuccessed = true;
			}
		}
		catch (Exception ex)
		{
			this.mapEdit.ShowTips("生成地图  失败！请重试！");
		}
	}

	public void OnSaveBtnClick()
	{
		if (!this.generateSuccessed || this.generatedMap == null)
		{
			this.mapEdit.ShowTips("生成地图出错，无法保存！");
		}
		if (string.IsNullOrEmpty(this.mapNameInput.value))
		{
			this.mapEdit.ShowTips("地图名不能为空！");
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = this.mapNameInput.value;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.currentPlayers = int.Parse(this.playerNumPopuplist.value);
		this.mapEdit.SaveMap();
	}

	private void Generate2()
	{
		int num = this.symmetricalType2++ % 14;
		JRandom random = new JRandom((long)Time.frameCount);
		List<Node> list = new List<Node>();
		foreach (KeyValuePair<int, int> keyValuePair in this.nodeNums)
		{
			list.Clear();
			NodeType key = (NodeType)keyValuePair.Key;
			string type = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType(keyValuePair.Key);
			MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(type);
			int num2 = 0;
			int num3 = keyValuePair.Value / 2;
			int num4 = num3;
			num2 = keyValuePair.Value % 2;
			if (num3 > 0)
			{
				int i = num3;
				while (i > 0)
				{
					float xmin = 0f;
					float xmax = 0f;
					float ymin = 0f;
					float ymax = 0f;
					MapGenerater.Calculation calculationYmin = null;
					MapGenerater.Calculation calculationYmax = null;
					if (num == 0 || num == 1)
					{
						xmin = -7.5f;
						xmax = 0f;
						ymin = -4f;
						ymax = 4f;
					}
					else if (num == 2 || num == 3)
					{
						xmin = -7.5f;
						xmax = 7.5f;
						ymin = 0f;
						ymax = 4f;
					}
					else if (num == 4)
					{
						xmin = -7.5f;
						xmax = 7.5f;
						calculationYmin = ((float x) => -0.53333336f * x);
						ymax = 4f;
					}
					else if (num == 5)
					{
						xmin = -7.5f;
						xmax = 7.5f;
						calculationYmin = ((float x) => 0.53333336f * x);
						ymax = 4f;
					}
					else if (num == 6 || num == 7)
					{
						xmin = -7.5f;
						xmax = 7.5f;
						calculationYmin = ((float x) => Math.Min(-1.4906832f * x + 7.1801243f, 4f));
						calculationYmax = ((float x) => -0.53333336f * x);
					}
					else if (num == 8 || num == 9)
					{
						xmin = -7.5f;
						xmax = 7.5f;
						calculationYmin = ((float x) => 0.53333336f * x);
						calculationYmax = ((float x) => Math.Min(1.4906832f * x + 7.1801243f, 4f));
					}
					else if (num == 10 || num == 11)
					{
						xmin = -4f;
						xmax = 4f;
						calculationYmin = ((float x) => -1f * x);
						ymax = 4f;
					}
					else if (num == 12 || num == 13)
					{
						xmin = -4f;
						xmax = 4f;
						calculationYmin = ((float x) => x);
						ymax = 4f;
					}
					Node node;
					if (!this.ProduceOnePlanet(random, xmin, xmax, ymin, ymax, key, data, out node, calculationYmin, calculationYmax))
					{
						return;
					}
					i--;
					list.Add(node);
					this.AddPlayerToMap(1, node);
				}
			}
			if (num4 > 0)
			{
				foreach (Node node2 in list)
				{
					MapNodeConfig data2 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType((int)node2.nodeType));
					Vector3 position = node2.GetPosition();
					float x2 = 0f;
					float y = 0f;
					if (num == 0)
					{
						x2 = -position.x;
						y = position.y;
					}
					else if (num == 1 || num == 3 || num == 4 || num == 5 || num == 7 || num == 9 || num == 11 || num == 13)
					{
						x2 = -position.x;
						y = -position.y;
					}
					else if (num == 2)
					{
						x2 = position.x;
						y = -position.y;
					}
					else if (num == 6)
					{
						x2 = position.x * 161f / 289f - position.y * 240f / 289f;
						y = position.x * -0.8304498f - position.y * 161f / 289f;
					}
					else if (num == 8)
					{
						x2 = position.x * 161f / 289f + position.y * 240f / 289f;
						y = position.x * 240f / 289f - position.y * 161f / 289f;
					}
					else if (num == 10)
					{
						x2 = -position.y;
						y = -position.x;
					}
					else if (num == 12)
					{
						x2 = position.y;
						y = position.x;
					}
					Node node3 = this.AddNodeToScene(key, x2, y, data);
					this.AddPlayerToMap(2, node3);
				}
			}
			if (num2 > 0)
			{
				int i = num2;
				bool flag = false;
				while (i > 0)
				{
					float ymin2 = 0f;
					float ymax2 = 0f;
					MapGenerater.Calculation calculationYmax2 = null;
					if (num == 0 || num == 1)
					{
						flag = this.ProduceOnePlanet(random, 0f, 0f, -4f, 4f, key, data);
					}
					else if (num == 2 || num == 3)
					{
						flag = this.ProduceOnePlanet(random, -7.5f, 7.5f, 0f, 0f, key, data);
					}
					else if (num == 4 || num == 6 || num == 7)
					{
						float xmin2 = -7.5f;
						float xmax2 = 7.5f;
						MapGenerater.Calculation calculationYmin2 = (float x) => -0.53333336f * x;
						calculationYmin2 = ((float x) => -0.53333336f * x);
						Node node4;
						flag = this.ProduceOnePlanet(random, xmin2, xmax2, ymin2, ymax2, key, data, out node4, calculationYmin2, calculationYmax2);
					}
					else if (num == 5 || num == 8 || num == 9)
					{
						float xmin2 = -7.5f;
						float xmax2 = 7.5f;
						MapGenerater.Calculation calculationYmin2 = (float x) => 0.53333336f * x;
						calculationYmin2 = ((float x) => 0.53333336f * x);
						Node node4;
						flag = this.ProduceOnePlanet(random, xmin2, xmax2, ymin2, ymax2, key, data, out node4, calculationYmin2, calculationYmax2);
					}
					else if (num == 10 || num == 11)
					{
						float xmin2 = -4f;
						float xmax2 = 4f;
						MapGenerater.Calculation calculationYmin2 = (float x) => -1f * x;
						calculationYmin2 = ((float x) => -1f * x);
						Node node4;
						flag = this.ProduceOnePlanet(random, xmin2, xmax2, ymin2, ymax2, key, data, out node4, calculationYmin2, calculationYmax2);
					}
					else if (num == 12 || num == 13)
					{
						float xmin2 = -4f;
						float xmax2 = 4f;
						MapGenerater.Calculation calculationYmin2 = (float x) => x;
						calculationYmin2 = ((float x) => x);
						Node node4;
						flag = this.ProduceOnePlanet(random, xmin2, xmax2, ymin2, ymax2, key, data, out node4, calculationYmin2, calculationYmax2);
					}
					if (!flag)
					{
						return;
					}
					i--;
				}
			}
		}
	}

	private void Generate3()
	{
		int num = this.symmetricalType3++ % 2;
		JRandom random = new JRandom((long)Time.frameCount);
		List<Node> list = new List<Node>();
		foreach (KeyValuePair<int, int> keyValuePair in this.nodeNums)
		{
			list.Clear();
			NodeType key = (NodeType)keyValuePair.Key;
			string type = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType(keyValuePair.Key);
			MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(type);
			int num2 = 0;
			int num3 = 0;
			int num4 = keyValuePair.Value / 3;
			int num5;
			num2 = (num5 = num4);
			num3 = keyValuePair.Value % 3;
			if (num4 > 0)
			{
				int i = num4;
				while (i > 0)
				{
					float xmin = 0f;
					float xmax = 0f;
					float ymin = 0f;
					float ymax = 0f;
					MapGenerater.Calculation calculationYmin = null;
					MapGenerater.Calculation calculationYmax = null;
					if (num == 0)
					{
						xmin = -4f;
						xmax = 4f;
						calculationYmin = ((float x) => Mathf.Abs(x));
						ymax = 4f;
					}
					else if (num == 1)
					{
						xmin = -4f;
						xmax = 4f;
						ymin = -4f;
						calculationYmax = ((float x) => Mathf.Abs(x));
					}
					Node node;
					if (!this.ProduceOnePlanet(random, xmin, xmax, ymin, ymax, key, data, out node, calculationYmin, calculationYmax))
					{
						return;
					}
					i--;
					list.Add(node);
					this.AddPlayerToMap(1, node);
				}
			}
			if (num5 > 0)
			{
				foreach (Node node2 in list)
				{
					MapNodeConfig data2 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType((int)node2.nodeType));
					Vector3 position = node2.GetPosition();
					float f = 2.0943952f;
					float x3 = position.x * Mathf.Cos(f) - position.y * Mathf.Sin(f);
					float y = position.x * Mathf.Sin(f) + position.y * Mathf.Cos(f);
					Node node3 = this.AddNodeToScene(key, x3, y, data);
					this.AddPlayerToMap(2, node3);
				}
			}
			if (num2 > 0)
			{
				foreach (Node node4 in list)
				{
					MapNodeConfig data3 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType((int)node4.nodeType));
					Vector3 position2 = node4.GetPosition();
					float f2 = 4.1887903f;
					float x2 = position2.x * Mathf.Cos(f2) - position2.y * Mathf.Sin(f2);
					float y2 = position2.x * Mathf.Sin(f2) + position2.y * Mathf.Cos(f2);
					Node node5 = this.AddNodeToScene(key, x2, y2, data);
					this.AddPlayerToMap(3, node5);
				}
			}
			if (num3 > 0)
			{
				int i = num3;
				bool flag = true;
				while (i > 0)
				{
					if (i == 1)
					{
						flag &= this.ProduceOnePlanet(random, 0f, 0f, 0f, 0f, key, data);
						i--;
					}
					else if (i == 2)
					{
						MapGenerater.Calculation calculation = (float x) => x * 4f / 7.5f;
						MapGenerater.Calculation calculation2 = (float x) => x * -4f / 7.5f;
						if (num == 0)
						{
							Node node6 = null;
							flag &= this.ProduceOnePlanet(random, -7.5f, 0f, 0f, 0f, key, data, out node6, calculation2, calculation2);
							Vector3 position3 = node6.GetPosition();
							this.AddNodeToScene(key, -position3.x, position3.y, data);
						}
						else if (num == 1)
						{
							Node node7 = null;
							flag &= this.ProduceOnePlanet(random, -7.5f, 0f, 0f, 0f, key, data, out node7, calculation, calculation);
							Vector3 position4 = node7.GetPosition();
							this.AddNodeToScene(key, -position4.x, position4.y, data);
						}
						i -= 2;
					}
					if (!flag)
					{
						return;
					}
				}
			}
		}
	}

	private void Generate4()
	{
		int num = this.symmetricalType4 % 2;
		JRandom jrandom = new JRandom((long)Time.frameCount);
		List<Node> list = new List<Node>();
		foreach (KeyValuePair<int, int> keyValuePair in this.nodeNums)
		{
			list.Clear();
			NodeType key = (NodeType)keyValuePair.Key;
			string type = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType(keyValuePair.Key);
			MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(type);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = keyValuePair.Value / 4;
			int num6;
			num2 = (num6 = (num3 = num5));
			num4 = keyValuePair.Value % 4;
			if (num5 > 0)
			{
				int i = num5;
				while (i > 0)
				{
					float xmin = 0f;
					float xmax = 0f;
					float ymin = 0f;
					float ymax = 0f;
					if (num == 0)
					{
						xmin = -7.5f;
						xmax = 0f;
						ymin = 0f;
						ymax = 4f;
					}
					else if (num == 1)
					{
					}
					Node node;
					if (!this.ProduceOnePlanet(jrandom, xmin, xmax, ymin, ymax, key, data, out node, null, null))
					{
						return;
					}
					i--;
					list.Add(node);
					this.AddPlayerToMap(1, node);
				}
			}
			if (num6 > 0)
			{
				foreach (Node node2 in list)
				{
					MapNodeConfig data2 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType((int)node2.nodeType));
					Vector3 position = node2.GetPosition();
					float x = 0f;
					float y = 0f;
					if (num == 0)
					{
						x = position.x;
						y = -position.y;
					}
					else if (num == 1)
					{
					}
					Node node3 = this.AddNodeToScene(key, x, y, data);
					this.AddPlayerToMap(2, node3);
				}
			}
			if (num2 > 0)
			{
				foreach (Node node4 in list)
				{
					MapNodeConfig data3 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType((int)node4.nodeType));
					Vector3 position2 = node4.GetPosition();
					float x2 = 0f;
					float y2 = 0f;
					if (num == 0)
					{
						x2 = -position2.x;
						y2 = -position2.y;
					}
					else if (num == 1)
					{
					}
					Node node5 = this.AddNodeToScene(key, x2, y2, data);
					this.AddPlayerToMap(3, node5);
				}
			}
			if (num3 > 0)
			{
				foreach (Node node6 in list)
				{
					MapNodeConfig data4 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType((int)node6.nodeType));
					Vector3 position3 = node6.GetPosition();
					float x3 = 0f;
					float y3 = 0f;
					if (num == 0)
					{
						x3 = -position3.x;
						y3 = position3.y;
					}
					else if (num == 1)
					{
					}
					Node node7 = this.AddNodeToScene(key, x3, y3, data);
					this.AddPlayerToMap(4, node7);
				}
			}
			if (num4 > 0)
			{
				int i = num4;
				bool flag = true;
				while (i > 0)
				{
					if (i == 1 || i == 3)
					{
						flag &= this.ProduceOnePlanet(jrandom, 0f, 0f, 0f, 0f, key, data);
						i--;
					}
					else if (i == 2)
					{
						int num7 = jrandom.nextInt();
						if (num7 % 2 == 0)
						{
							Node node8 = null;
							flag &= this.ProduceOnePlanet(jrandom, -7.5f, 0f, 0f, 0f, key, data, out node8, null, null);
							Vector3 position4 = node8.GetPosition();
							this.AddNodeToScene(key, -position4.x, position4.y, data);
						}
						else
						{
							Node node9 = null;
							flag &= this.ProduceOnePlanet(jrandom, 0f, 0f, 0f, 4f, key, data, out node9, null, null);
							Vector3 position5 = node9.GetPosition();
							this.AddNodeToScene(key, position5.x, -position5.y, data);
						}
						i -= 2;
					}
					if (!flag)
					{
						return;
					}
				}
			}
		}
	}

	private bool ProduceOnePlanet(JRandom random, float xmin, float xmax, float ymin, float ymax, NodeType nodeType, MapNodeConfig nodeConfig)
	{
		Node node = null;
		return this.ProduceOnePlanet(random, xmin, xmax, ymin, ymax, nodeType, nodeConfig, out node, null, null);
	}

	private bool ProduceOnePlanet(JRandom random, float xmin, float xmax, float ymin, float ymax, NodeType nodeType, MapNodeConfig nodeConfig, out Node outNode, MapGenerater.Calculation calculationYmin = null, MapGenerater.Calculation calculationYmax = null)
	{
		outNode = null;
		bool flag = false;
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		while (!flag)
		{
			flag = true;
			num = random.nextFloat() * (xmax - xmin) + xmin;
			if (calculationYmin != null)
			{
				ymin = calculationYmin(num);
			}
			if (calculationYmax != null)
			{
				ymax = calculationYmax(num);
			}
			num2 = random.nextFloat() * (ymax - ymin) + ymin;
			float size = nodeConfig.size;
			List<Node> list = new List<Node>();
			list.AddRange(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetUsefulNodeList());
			list.AddRange(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetBarrierNodeList());
			foreach (Node node in list)
			{
				MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetType((int)node.nodeType));
				Vector3 position = node.GetPosition();
				float num4 = Mathf.Sqrt((num - position.x) * (num - position.x) + (num2 - position.y) * (num2 - position.y));
				if (num4 < size * 0.6f + data.size * 0.6f)
				{
					flag = false;
					break;
				}
			}
			num3++;
			if (!flag && num3 >= 1000)
			{
				Debug.LogFormat("无法在 {0} 循环中增加一个星球", new object[]
				{
					num3
				});
				throw new Exception("增加新球失败");
			}
		}
		if (flag)
		{
			outNode = this.AddNodeToScene(nodeType, num, num2, nodeConfig);
		}
		return flag;
	}

	private Node AddNodeToScene(NodeType nodeType, float x, float y, MapNodeConfig config)
	{
		Node result = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.AddNode(this.nodeTag.ToString(), (int)nodeType, x, y, config.perfab);
		this.nodeTag += '\u0001';
		return result;
	}

	private void AddPlayerToMap(int team, Node node)
	{
		if (node.nodeType != NodeType.Planet)
		{
			return;
		}
		if (this.playerHostNode.Count == team - 1)
		{
			this.playerHostNode.Add(node.tag);
		}
	}

	public UIPopupList playerNumPopuplist;

	public UIScrollView nodeScrollview;

	public UIGrid nodeGrid;

	public GameObject nodeTemplate;

	public UIInput mapNameInput;

	public MapEdit mapEdit;

	private Dictionary<int, UIInput> nodeNumInputs = new Dictionary<int, UIInput>();

	private Dictionary<int, int> nodeNums = new Dictionary<int, int>();

	private List<string> playerHostNode = new List<string>();

	private char nodeTag;

	private MapConfig generatedMap;

	private bool generateSuccessed;

	private int symmetricalType2;

	private int symmetricalType3;

	private int symmetricalType4;

	private delegate float Calculation(float x);
}
