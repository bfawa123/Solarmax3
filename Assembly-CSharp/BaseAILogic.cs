using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public abstract class BaseAILogic : Lifecycle2
{
	public BaseAILogic()
	{
	}

	public SceneManager sceneManager { get; set; }

	public void RegisterCallback(AIStatus status, BaseAILogic.OnAIStatusCallback callback)
	{
		if (status == AIStatus.Unknow)
		{
			return;
		}
		this.aiStatusCallbacks[(int)status] = callback;
	}

	public bool Init()
	{
		return true;
	}

	public void Tick(int frame, float interval)
	{
	}

	public void Tick(Team t, int frame, float interval)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript)
		{
			return;
		}
		if (t.current <= 0)
		{
			return;
		}
		AIData aiData = this.GetAiData(t, false);
		aiData.actionTime += interval;
		if (aiData.actionTime < this.actionTick)
		{
			return;
		}
		aiData.actionTime = 0f;
		for (int i = 0; i < this.teamActions[(int)t.team].Count; i++)
		{
			this.aiStatusCallbacks[this.teamActions[(int)t.team][i]](t, interval, null);
		}
	}

	public void NodeTick(Node n, int frame, float interval)
	{
		if (n == null)
		{
			return;
		}
		if (n.currentTeam == null)
		{
			return;
		}
		if (n.currentTeam.current <= 0)
		{
			return;
		}
		if (n.aiActions == null)
		{
			return;
		}
		if (n.aiActions.Count == 0)
		{
			return;
		}
		if (this.GetAiData(n.currentTeam, false).actionTime > 0f)
		{
			return;
		}
		for (int i = 0; i < n.aiActions.Count; i++)
		{
			this.aiStatusCallbacks[n.aiActions[i]](n.currentTeam, interval, n);
		}
	}

	public void Destroy()
	{
	}

	public abstract void Init(Team t, int aiStrategy, int level, int Difficulty, float Tick);

	public abstract void Release(Team t);

	protected int ComparsionAIValue(Node arg0, Node arg1)
	{
		return arg0.aiValue.CompareTo(arg1.aiValue);
	}

	protected int ComparisonAIValue(Node arg0, Node arg1)
	{
		int num = arg0.aiValue.CompareTo(arg1.aiValue);
		if (num == 0)
		{
			return arg0.tag.CompareTo(arg1.tag);
		}
		return num;
	}

	protected int ComparsionAIStrength(Node arg0, Node arg1)
	{
		return arg0.aiStrength.CompareTo(arg1.aiStrength);
	}

	protected void InsertSort(List<Node> list, Comparison<Node> comparsion)
	{
		if (list == null || list.Count == 0)
		{
			throw new ArgumentNullException("list");
		}
		if (comparsion == null)
		{
			throw new ArgumentNullException("comparsion");
		}
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			Node node = list[i];
			int num = i - 1;
			while (num >= 0 && comparsion(list[num], node) > 0)
			{
				list[num + 1] = list[num];
				num--;
			}
			list[num + 1] = node;
		}
	}

	protected Vector3 TraversalAllNodeCenter(Team t)
	{
		Vector3 vector = Vector3.zero;
		int num = 0;
		List<Node> usefulNodeList = this.sceneManager.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			Node node = usefulNodeList[i];
			node.ResetAICalculateCache();
			node.GetTransitShips(t);
			if (node.IsOurNode(t.team))
			{
				vector += node.GetPosition();
				num++;
			}
		}
		if (num > 0)
		{
			vector /= (float)num;
		}
		return vector;
	}

	public AIData GetAiData(Team t, bool release = false)
	{
		AIData aidata = this.sceneManager.aiManager.aiData[(int)t.team];
		if (release)
		{
			aidata.Reset();
		}
		return aidata;
	}

	protected BaseAILogic.OnAIStatusCallback[] aiStatusCallbacks = new BaseAILogic.OnAIStatusCallback[100];

	public float actionTick = 1f;

	public List<int>[] teamActions = new List<int>[LocalPlayer.MaxTeamNum];

	public delegate bool OnAIStatusCallback(Team t, float dt, Node o = null);
}
