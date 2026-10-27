using System;
using System.Collections.Generic;
using Solarmax;

public class AIManager : Lifecycle2
{
	public AIManager(SceneManager sceneManager)
	{
		this.sceneMagr = sceneManager;
		this.aiLogic = new FriendSmartAILogic
		{
			sceneManager = this.sceneMagr
		};
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			this.aiData[i] = new AIData();
			this.aiData[i].Reset();
		}
	}

	private SceneManager sceneMagr { get; set; }

	public bool Init()
	{
		return true;
	}

	public void Tick(int frame, float interval)
	{
		if (!this.start)
		{
			return;
		}
		if (this.countDownTime > 0f)
		{
			this.countDownTime -= interval;
			return;
		}
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.sceneMagr.teamManager.GetTeam((TEAM)i);
			if (team.aiEnable)
			{
				this.aiLogic.Tick(team, frame, interval);
			}
		}
		List<Node> usefulNodeList = this.sceneMagr.nodeManager.GetUsefulNodeList();
		for (int j = 0; j < usefulNodeList.Count; j++)
		{
			Node node = usefulNodeList[j];
			if (node.aistrategy >= 0 && node.currentTeam != null)
			{
				this.aiLogic.NodeTick(node, frame, interval);
			}
		}
	}

	public void Destroy()
	{
		this.start = false;
		for (int i = 0; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.sceneMagr.teamManager.GetTeam((TEAM)i);
			if (team.aiEnable)
			{
				this.aiLogic.Release(team);
			}
		}
	}

	public void AddAI(Team t, int aiStrategy, int level, int Difficulty, float Tick = 1f)
	{
		this.aiLogic.Init(t, aiStrategy, level, Difficulty, Tick);
	}

	public void Start(float countTie)
	{
		this.start = true;
		this.countDownTime = countTie;
	}

	public static string GetAIName(int userId)
	{
		int localLanguage = Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage;
		Random random = new Random(userId);
		List<NameConfig> firstNameList = Solarmax.Singleton<NameConfigProvider>.Instance.GetFirstNameList();
		int index = random.Next(0, firstNameList.Count - 1);
		List<NameConfig> lastNameList = Solarmax.Singleton<NameConfigProvider>.Instance.GetLastNameList();
		int index2 = random.Next(0, lastNameList.Count - 1);
		string result;
		if (localLanguage == 40 || localLanguage == 6)
		{
			result = firstNameList[index].chinese + "·" + lastNameList[index2].chinese;
		}
		else if (localLanguage == 41)
		{
			result = firstNameList[index].chinese + "·" + lastNameList[index2].chinese;
		}
		else
		{
			result = firstNameList[index].english + "·" + lastNameList[index2].english;
		}
		return result;
	}

	public static string GetAIIcon(int userId)
	{
		return SelectIconWindow.GetIcon(new Random(userId).Next(0, 10));
	}

	private FriendSmartAILogic aiLogic;

	public AIData[] aiData = new AIData[LocalPlayer.MaxTeamNum];

	private bool start;

	private float countDownTime;
}
