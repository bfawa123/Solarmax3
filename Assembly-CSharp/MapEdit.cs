using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Solarmax;
using UnityEngine;

public class MapEdit : MonoBehaviour
{
	private void Start()
	{
		MapEdit.Instance = this;
		Solarmax.Singleton<Framework>.Instance.SetWritableRootDir(Application.temporaryCachePath);
		LoggerSystem instance = Solarmax.Singleton<LoggerSystem>.Instance;
		if (MapEdit.tmp1 == null)
		{
			MapEdit.tmp1 = new Callback<string>(Debug.Log);
		}
		instance.SetConsoleLogger(new Solarmax.Logger(MapEdit.tmp1));
		Solarmax.Singleton<LoadResManager>.Get().Init();
		Solarmax.Singleton<Framework>.Instance.InitLanguageAndPing();
		if (Solarmax.Singleton<Framework>.Instance.Init())
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("系统启动！", new object[0]);
			this.IsInited = true;
		}
		else
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("系统启动失败！", new object[0]);
		}
		this.ch = 'A';
		this.tagIndex = 1;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.root = this.root;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.mapEdit = true;
		this.bulidsgrid.Reposition();
		this.bulidsView.ResetPosition();
		this.CreateItemList();
		this.NewMap();
		this.currentSelect = null;
		Solarmax.Singleton<AssetManager>.Get().LoadBattleResources();
		this.StartConnectServer();
	}

	private void OnDestroy()
	{
		Solarmax.Singleton<UserSyncSysteam>.Get().DestroyThread();
	}

	private void CreateItemList()
	{
		float value = this.view.verticalScrollBar.value;
		Dictionary<string, MapConfig> allData = Solarmax.Singleton<MapConfigProvider>.Instance.GetAllData();
		this.itemList.ForEach(delegate(GameObject g)
		{
			UnityEngine.Object.Destroy(g);
		});
		this.itemList.Clear();
		foreach (KeyValuePair<string, MapConfig> keyValuePair in allData)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemRoot);
			gameObject.transform.SetParent(this.grid.gameObject.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(true);
			UIEventListener componentInChildren = gameObject.GetComponentInChildren<UIEventListener>();
			componentInChildren.onClick = new UIEventListener.VoidDelegate(this.OnClickButton);
			UILabel componentInChildren2 = gameObject.GetComponentInChildren<UILabel>();
			componentInChildren2.text = keyValuePair.Key;
			this.itemList.Add(gameObject);
		}
		this.grid.Reposition();
		this.view.ResetPosition();
		this.view.verticalScrollBar.value = value;
	}

	private void OnClickButton(GameObject go)
	{
		UILabel componentInChildren = go.GetComponentInChildren<UILabel>();
		string text = componentInChildren.text;
		if (text == Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = text;
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.CreateScene(null, true, false);
		this.mapName.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		this.playerCount.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentPlayers.ToString();
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(text);
		this.mapAudio.value = data.audio;
		this.vertical.value = true;
		this.defaultAI.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.defaultAI;
		this.teamAI.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.teamAI;
		List<Node> usefulNodeList = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetUsefulNodeList();
		foreach (Node node in usefulNodeList)
		{
			this.nodeAttribute.UpdateOrbitLine(node, node.revoType, node.revoParam1, node.revoParam2);
		}
	}

	private void Update()
	{
		if (!this.IsInited)
		{
			return;
		}
		Solarmax.Singleton<Framework>.Instance.Tick(Time.deltaTime);
		if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Delete)))
		{
			if (NodeAttribute.current == null)
			{
				return;
			}
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.RemoveNode(NodeAttribute.current.tag);
		}
		this.ProcessNeedUpLoadMap();
		Solarmax.Singleton<EffectManager>.Get().Tick(Time.frameCount, Time.time);
	}

	public void HideUI()
	{
		this.menuRoot.SetActive(!this.menuRoot.activeSelf);
	}

	public void SelectNode(UILabel lable)
	{
		int num = 0;
		string text = lable.text;
		switch (text)
		{
		case "star":
			num = 1;
			break;
		case "teleport":
			num = 3;
			break;
		case "tower":
			num = 4;
			break;
		case "castle":
			num = 2;
			break;
		case "barrier":
			num = 5;
			break;
		case "master":
			num = 7;
			break;
		case "defense":
			num = 8;
			break;
		case "power":
			num = 9;
			break;
		case "blackhole":
			num = 10;
			break;
		case "House":
			num = 11;
			break;
		case "Arsenal":
			num = 12;
			break;
		case "AircraftCarrier":
			num = 13;
			break;
		case "Lasercannon":
			num = 14;
			break;
		case "Attackship":
			num = 15;
			break;
		case "Lifeship":
			num = 16;
			break;
		case "Speedship":
			num = 17;
			break;
		case "Captureship":
			num = 18;
			break;
		case "AntiAttackship":
			num = 19;
			break;
		case "AntiLifeship":
			num = 20;
			break;
		case "AntiSpeedship":
			num = 21;
			break;
		case "AntiCaptureship":
			num = 22;
			break;
		case "Magicstar":
			num = 23;
			break;
		case "Hiddenstar":
			num = 24;
			break;
		case "FixedWarpDoor":
			num = 25;
			break;
		case "Clouds":
			num = 26;
			break;
		case "Inhibit":
			num = 27;
			break;
		case "Twist":
			num = 28;
			break;
		case "AddTower":
			num = 29;
			break;
		case "Sacrifice":
			num = 30;
			break;
		case "OverKill":
			num = 31;
			break;
		case "CloneTo":
			num = 32;
			break;
		case "Treasure":
			num = 33;
			break;
		case "UnknownStar":
			num = 34;
			break;
		case "Lasergun":
			num = 35;
			break;
		case "Mirror":
			num = 36;
			break;
		case "BarrierPoint":
			num = 37;
			break;
		case "Gunturret":
			num = 38;
			break;
		case "Diffusion":
			num = 39;
			break;
		case "Curse":
			num = 40;
			break;
		case "Cannon":
			num = 41;
			break;
		}
		if (num == 0)
		{
			return;
		}
		Vector3 position = Input.mousePosition;
		position.z = 0f;
		position = this.uiCamera.ScreenToWorldPoint(position);
		for (;;)
		{
			if (this.ch > 'z')
			{
				if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetNode(this.tagIndex.ToString()) == null)
				{
					break;
				}
				this.tagIndex++;
			}
			else
			{
				if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetNode(this.ch.ToString()) == null)
				{
					break;
				}
				this.ch += '\u0001';
			}
		}
		MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(lable.text);
		Node node;
		if (data != null && this.ch > 'z')
		{
			node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.AddNode(this.tagIndex.ToString(), num, position.x, position.y, data.perfab);
		}
		else
		{
			node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.AddNode(this.ch.ToString(), num, position.x, position.y, data.perfab);
		}
		if (node != null)
		{
			this.nodeAttribute.SetNode(node);
		}
		this.currentSelect = node;
		this.UpdateMenu(position);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable == null)
		{
			return;
		}
		this.rightMenu.SetActive(false);
	}

	public void UpdateMenu(Vector3 position)
	{
		this.menuRoot.transform.localPosition = new Vector3(830f, -240f, 0f);
	}

	public void ChangeMapName()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = this.mapName.value;
	}

	public void ChangePlayerCount()
	{
		if (string.IsNullOrEmpty(this.playerCount.value))
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.currentPlayers = 0;
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.currentPlayers = int.Parse(this.playerCount.value);
		}
	}

	public void ChangeMapAudio()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.mapAudio = this.mapAudio.value;
	}

	public void ChangeDefaultAI()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.defaultAI = this.defaultAI.value;
	}

	public void ChangeteamAI()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.teamAI = this.teamAI.value;
	}

	public void ChangeVertical()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical = true;
	}

	public void NewMap()
	{
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable = new MapConfig("newmap1");
		this.mapName.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		this.playerCount.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentPlayers.ToString();
		this.mapAudio.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.mapAudio;
		this.vertical.value = true;
		this.defaultAI.value = Solarmax.Singleton<BattleSystem>.Instance.battleData.defaultAI;
		this.ch = 'A';
	}

	public void DeleteMap()
	{
		MapConfig currentTable = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		Solarmax.Singleton<MapConfigProvider>.Instance.Delete(currentTable.name);
		this.CreateMatchJson();
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		this.CreateItemList();
		GameObject gameObject = this.itemList[0];
		UIEventListener componentInChildren = gameObject.GetComponentInChildren<UIEventListener>();
		componentInChildren.onClick(gameObject);
		Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
		this.ShowTips("生成map.txt, match.json成功");
	}

	public void ShowTips(string message)
	{
		UISprite componentInChildren = this.tips.GetComponentInChildren<UISprite>();
		componentInChildren.alpha = 0f;
		componentInChildren.color = Color.white;
		UILabel componentInChildren2 = this.tips.GetComponentInChildren<UILabel>();
		componentInChildren2.text = message;
		this.tips.transform.localPosition = Vector3.zero;
		this.tips.SetActive(true);
		TweenPosition tweenPosition = TweenPosition.Begin(this.tips, 0.5f, new Vector3(0f, 200f, 0f));
		TweenAlpha.Begin(this.tips, 0.5f, 1f);
		EventDelegate.Add(tweenPosition.onFinished, delegate()
		{
			TweenAlpha.Begin(this.tips, 1.5f, 0f);
		}, true);
	}

	public void HidePanel()
	{
		TweenPosition.Begin(this.tweener.gameObject, 0.5f, new Vector3(600f, 0f, 0f));
	}

	public void ShowPanel()
	{
		TweenPosition.Begin(this.tweener.gameObject, 0.5f, new Vector3(0f, 0f, 0f));
	}

	public void OnBackBtnClick()
	{
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
	}

	public void SaveMap()
	{
		if (string.IsNullOrEmpty(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId))
		{
			this.ShowTips("地图名字不可为空");
			return;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.currentPlayers == 0)
		{
			this.ShowTips("参与玩家人数不得少于1");
			return;
		}
		List<int> list = new List<int>();
		using (List<MapPlayerConfig>.Enumerator enumerator = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mpcList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MapPlayerConfig item = enumerator.Current;
				if (item != null && item.camption != 0 && item.camption != list.Find((int n) => n == item.camption))
				{
					list.Add(item.camption);
				}
			}
		}
		if (list.Count != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentPlayers)
		{
			this.ShowTips("参与玩家人数与出生队伍不一致");
			return;
		}
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
		if (data != null && data != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable)
		{
			this.ShowTips("地图名字重复");
			return;
		}
		if (!Solarmax.Singleton<MapConfigProvider>.Instance.ModifymapVersion(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId))
		{
			this.ShowTips("地图版本管理错误");
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.name = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		Solarmax.Singleton<MapConfigProvider>.Instance.Save(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable);
		this.CreateItemList();
		this.ShowTips("保存" + Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId + ".xml成功");
	}

	private void CreateMatchJson()
	{
		List<MatchTable> list = new List<MatchTable>();
		foreach (MapConfig mapConfig in Solarmax.Singleton<MapConfigProvider>.Instance.GetAllData().Values)
		{
			list.Add(new MatchTable
			{
				id = mapConfig.id,
				players = mapConfig.player_count,
				vertical = true
			});
		}
		string s = JsonHelper.SerializeObject(list);
		string text = string.Format("{0}/StreamingAssets/data/match.json", Application.dataPath);
		string text2 = string.Format("{0}/StreamingAssets/data/match_bak.json", Application.dataPath);
		if (File.Exists(text))
		{
			File.Move(text, text2);
		}
		using (FileStream fileStream = new FileStream(text, FileMode.CreateNew))
		{
			byte[] bytes = Encoding.Default.GetBytes(s);
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Flush();
			fileStream.Close();
		}
		if (File.Exists(text2))
		{
			File.Delete(text2);
		}
	}

	private void SaveText(string text, string name)
	{
		string text2 = string.Format("{0}/StreamingAssets/data/{1}.txt", Application.dataPath, name);
		string text3 = string.Format("{0}/StreamingAssets/data/{1}_bak.txt", Application.dataPath, name);
		File.Move(text2, text3);
		using (FileStream fileStream = new FileStream(text2, FileMode.CreateNew))
		{
			byte[] bytes = Encoding.Default.GetBytes(text);
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Flush();
			fileStream.Close();
		}
		File.Delete(text3);
	}

	public void UploadFile()
	{
		this.slider.gameObject.SetActive(true);
		this.slider.value = 0f;
		this.modifyList.Clear();
		Dictionary<string, MapListConfig> mapVersion = Solarmax.Singleton<MapConfigProvider>.Instance.mapVersion;
		foreach (KeyValuePair<string, MapListConfig> keyValuePair in mapVersion)
		{
			MapListConfig value = keyValuePair.Value;
			if (value.nAdd > 0)
			{
				this.modifyList.Push(value.mapID);
			}
		}
		int count = this.modifyList.Count;
		if (count <= 0 || count == 1)
		{
			this.scaleProcess = 1f;
		}
		else
		{
			this.scaleProcess = 1f / (float)count;
		}
		this.StarUpLoad = true;
	}

	public void StartConnectServer()
	{
		base.StartCoroutine(this.ConnectServer());
	}

	private IEnumerator ConnectServer()
	{
		yield return new WaitForSeconds(0.2f);
		yield return Solarmax.Singleton<NetSystem>.Instance.helper.ConnectServer(true);
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
		{
			yield return new WaitForSeconds(0.2f);
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestUser();
		}
		else
		{
			yield return new WaitForSeconds(3f);
			base.StartCoroutine(this.ConnectServer());
		}
		yield break;
	}

	private void ProcessNeedUpLoadMap()
	{
		int count = this.modifyList.Count;
		if (count <= 0 && this.slider.gameObject.activeSelf)
		{
			this.slider.gameObject.SetActive(false);
			return;
		}
		if (this.IsUpLoading)
		{
			return;
		}
		if (count <= 0)
		{
			return;
		}
		string text = this.modifyList.Pop();
		if (!string.IsNullOrEmpty(text))
		{
			this.curUploadMap = text;
			string file = string.Format("{0}/StreamingAssets/data/maplist/{1}.xml", Application.dataPath, text);
			Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl(text + ".xml", "PUT", "text/plain", file, 113);
			this.IsUpLoading = true;
		}
		if (this.StarUpLoad && this.modifyList.Count == 0)
		{
			this.StarUpLoad = false;
			Solarmax.Singleton<MapConfigProvider>.Instance.WriteMapList(true);
			string file2 = string.Format("{0}/StreamingAssets/data/MapList.xml", Application.dataPath, text);
			Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl("MapList.xml", "PUT", "text/plain", file2, 113);
		}
	}

	public void OnCompleteUpLoad()
	{
		float num = this.slider.value;
		num += this.scaleProcess;
		this.slider.value = num;
		this.IsUpLoading = false;
		Solarmax.Singleton<MapConfigProvider>.Instance.SyncmapVersion(this.curUploadMap);
		this.curUploadMap = string.Empty;
	}

	public Camera uiCamera;

	public GameObject root;

	public GameObject menuRoot;

	public GameObject rightMenu;

	public NodeAttribute nodeAttribute;

	public UIInput mapName;

	public UIToggle vertical;

	public UIInput playerCount;

	public UIInput mapAudio;

	public UIInput defaultAI;

	public UIInput teamAI;

	public TweenPosition tweener;

	private char ch;

	private int tagIndex;

	public UIScrollView view;

	public UIScrollView bulidsView;

	public UIGrid grid;

	public UIGrid bulidsgrid;

	public GameObject itemRoot;

	public GameObject tips;

	public UISlider slider;

	private List<GameObject> itemList = new List<GameObject>();

	public Node currentSelect;

	public static MapEdit Instance;

	private bool IsInited;

	private string curUploadMap = string.Empty;

	private bool StarUpLoad;

	private bool IsUpLoading;

	private float scaleProcess = 1f;

	private Stack<string> modifyList = new Stack<string>();

	[CompilerGenerated]
	private static Callback<string> tmp1;
}
