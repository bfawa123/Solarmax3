using System;
using System.Collections.Generic;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class NodeHUD
{
	public NodeHUD()
	{
		this.m_clr = new List<Color>();
		this.m_p = new List<float>();
		this.m_Halo = new BattleUIHalo();
		this.m_RangeHalo = new BattleWarningHalo();
		this.m_LaserLineHalo = new BattleLaserLineHalo();
		this.mHudPoses = new List<Vector3>();
		this.mHudLabel = new List<UILabel>();
		this.mLastShowHudCount = 0;
	}

	private bool IsFivePlayerCoop()
	{
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		if (battleData == null)
		{
			return false;
		}
		if (battleData.matchPlayerNum != 5)
		{
			return false;
		}
		return battleData.battleSubType == CooperationType.CT_2vPC || battleData.battleSubType == CooperationType.CT_3vPC || battleData.battleSubType == CooperationType.CT_4vPC;
	}

	public void SetNode(Node node)
	{
		if (node != null)
		{
			this.hostNode = node;
			this.m_Halo.InitHalo(node);
			this.m_RangeHalo.InitHalo(node, this.hostNode.GetAttackRage());
			this.m_LaserLineHalo.InitHalo(node);
			this.initHUDPos();
			this.mHudRoot = new GameObject();
			this.mHudRoot.name = "HudRoot";
			this.mHudRoot.transform.SetParent(this.hostNode.GetGO().transform);
			this.mHudRoot.layer = this.hostNode.GetGO().layer;
			this.mHudRoot.transform.localPosition = Vector3.zero;
			Vector3 vector = this.mHudRoot.transform.parent.localScale * 80f;
			this.mHudRoot.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
			{
				this.mHudRoot.transform.Rotate(0f, 0f, 90f);
			}
		}
	}

	public void ResetHost(Node node)
	{
		this.hostNode = node;
		this.m_Halo.ResetHost(this.hostNode);
		this.m_RangeHalo.ResetHost(this.hostNode);
		this.m_LaserLineHalo.ResetHost(this.hostNode);
		this.mHudRoot.transform.SetParent(this.hostNode.GetGO().transform);
		this.mHudRoot.layer = this.hostNode.GetGO().layer;
		this.mHudRoot.transform.localPosition = Vector3.zero;
		Vector3 vector = this.mHudRoot.transform.parent.localScale * 80f;
		this.mHudRoot.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
	}

	private void initHUDPos()
	{
		if (this.hostNode != null)
		{
			Vector3 position = this.hostNode.GetPosition();
			this.m_PosDown = (this.m_PosLeft = (this.m_PosRight = (this.m_PosUp = position)));
			this.m_PosDown.y = this.m_PosDown.y - this.hostNode.GetWidth();
			this.m_PosRight.x = this.m_PosRight.x + this.hostNode.GetWidth() * 2.5f;
			this.m_PosLeft.x = this.m_PosLeft.x - this.hostNode.GetWidth() * 3.5f;
			this.m_PosUp.y = this.m_PosUp.y + this.hostNode.GetWidth() * 1.5f;
		}
	}

	public void ShowProgress(List<Team> teamArray, List<float> HPArray)
	{
		int count = teamArray.Count;
		int count2 = HPArray.Count;
		if (count != count2)
		{
			Debug.LogError("teamArr != HPArr");
			return;
		}
		if (count == 1)
		{
			Color color = teamArray[0].color;
			color.a = 0.2f;
			this.m_clr.Add(color);
			color.a = 1f;
			this.m_clr.Add(color);
			this.m_p.Add(1f);
			this.m_p.Add(HPArray[0] / this.hostNode.hpMax);
			this.m_Halo.SetColor(this.m_clr, this.m_p);
			if (HPArray[0] > 100f)
			{
				this.m_Halo.ShutHalo();
			}
		}
		else if (2 <= count)
		{
			float num = 0f;
			for (int i = 0; i < count2; i++)
			{
				num += HPArray[i];
			}
			if (num == 0f)
			{
				this.m_Halo.ShutHalo();
			}
			for (int j = 0; j < count2; j++)
			{
				if (HPArray[j] > 0f)
				{
					Color item = teamArray[j].color;
					if (teamArray[j].team == TEAM.Neutral || ((teamArray[j].team == TEAM.Team_5 || teamArray[j].team == TEAM.Team_6) && !this.IsFivePlayerCoop()))
					{
						item = this.TempTeamColor;
					}
					item.a = 1f;
					this.m_clr.Add(item);
					this.m_p.Add(HPArray[j] / num);
					if (HPArray[j] / num == 1f)
					{
						this.m_Halo.ShutHalo();
					}
				}
			}
			this.m_Halo.SetColor(this.m_clr, this.m_p);
		}
		this.m_clr.Clear();
		this.m_p.Clear();
	}

	public void ShowProgress(Team[] teamArray, float[] HPArray)
	{
		int num = teamArray.Length;
		int num2 = HPArray.Length;
		if (num != num2)
		{
			Debug.LogError("teamArr != HPArr");
			return;
		}
		if (num == 1)
		{
			Color color = teamArray[0].color;
			color.a = 0.2f;
			this.m_clr.Add(color);
			color.a = 1f;
			this.m_clr.Add(color);
			this.m_p.Add(1f);
			this.m_p.Add(HPArray[0] / this.hostNode.hpMax);
			this.m_Halo.SetColor(this.m_clr, this.m_p);
			if (HPArray[0] > 100f)
			{
				this.m_Halo.ShutHalo();
			}
		}
		else if (2 <= num)
		{
			float num3 = 0f;
			for (int i = 0; i < num2; i++)
			{
				num3 += HPArray[i];
			}
			if (num3 == 0f)
			{
				this.m_Halo.ShutHalo();
			}
			for (int j = 0; j < num2; j++)
			{
				if (HPArray[j] > 0f)
				{
					Color item = teamArray[j].color;
					if (teamArray[j].team == TEAM.Neutral || ((teamArray[j].team == TEAM.Team_5 || teamArray[j].team == TEAM.Team_6) && !this.IsFivePlayerCoop()))
					{
						item = this.TempTeamColor;
					}
					item.a = 1f;
					this.m_clr.Add(item);
					this.m_p.Add(HPArray[j] / num3);
					if (HPArray[j] / num3 == 1f)
					{
						this.m_Halo.ShutHalo();
					}
				}
			}
			this.m_Halo.SetColor(this.m_clr, this.m_p);
		}
		this.m_clr.Clear();
		this.m_p.Clear();
	}

	public void ShowPopulation(List<Team> teamArray, List<float> HPArray)
	{
		this.ShowPopu(teamArray, HPArray);
	}

	public void ShutHUD()
	{
		if (this.m_Halo != null)
		{
			this.m_Halo.ShutHalo();
		}
	}

	public void Destroy()
	{
		if (this.m_LaserLineHalo != null)
		{
			this.m_LaserLineHalo.Destroy();
		}
	}

	public void ResetLabelPos(List<Team> teamArray, List<float> hpArray)
	{
		this.CalculateHudPoses(teamArray.Count);
		bool flag = true;
		this.mLastShowHudCount = teamArray.Count;
		if (flag && Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.mHudRoot.transform.Rotate(0f, 0f, -90f);
		}
		for (int i = this.mHudLabel.Count; i < teamArray.Count; i++)
		{
			GameObject gameObject = Solarmax.Singleton<AssetManager>.Get().GetResources("TXT") as GameObject;
			gameObject = this.mHudRoot.AddChild(gameObject);
			this.mHudLabel.Add(gameObject.GetComponent<UILabel>());
		}
		for (int j = 0; j < this.mHudLabel.Count; j++)
		{
			if (!(this.mHudLabel[j] == null))
			{
				this.mHudLabel[j].gameObject.SetActive(j < teamArray.Count);
				if (flag && j < teamArray.Count)
				{
					this.mHudLabel[j].transform.position = this.mHudPoses[j];
				}
			}
		}
		Color color = Color.blue;
		for (int k = 0; k < teamArray.Count; k++)
		{
			Team team = teamArray[k];
			color = team.color;
			if (teamArray[k].team == TEAM.Neutral || ((teamArray[k].team == TEAM.Team_5 || teamArray[k].team == TEAM.Team_6) && !this.IsFivePlayerCoop()))
			{
				color = this.TempTeamColor;
			}
			color.a = 1f;
			if (team.hidePopution && team.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
			{
				this.mHudLabel[k].text = string.Empty;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(hpArray[k]);
				this.mHudLabel[k].text = stringBuilder.ToString();
			}
			this.mHudLabel[k].color = color;
		}
		if (flag && Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.mHudRoot.transform.Rotate(0f, 0f, 90f);
		}
	}

	private void ShowPopu(List<Team> teamArray, List<float> hpArray)
	{
		bool flag = false;
		if (teamArray.Count != this.mHudLabel.Count || teamArray.Count != this.mLastShowHudCount)
		{
			this.CalculateHudPoses(teamArray.Count);
			flag = true;
			this.mLastShowHudCount = teamArray.Count;
		}
		if (flag && Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.mHudRoot.transform.Rotate(0f, 0f, -90f);
		}
		for (int i = this.mHudLabel.Count; i < teamArray.Count; i++)
		{
			GameObject gameObject = Solarmax.Singleton<AssetManager>.Get().GetResources("TXT") as GameObject;
			gameObject = this.mHudRoot.AddChild(gameObject);
			this.mHudLabel.Add(gameObject.GetComponent<UILabel>());
		}
		for (int j = 0; j < this.mHudLabel.Count; j++)
		{
			if (!(this.mHudLabel[j] == null))
			{
				this.mHudLabel[j].gameObject.SetActive(j < teamArray.Count);
				if (flag && j < teamArray.Count)
				{
					this.mHudLabel[j].transform.position = this.mHudPoses[j];
				}
			}
		}
		Color color = Color.blue;
		for (int k = 0; k < teamArray.Count; k++)
		{
			Team team = teamArray[k];
			color = team.color;
			if (teamArray[k].team == TEAM.Neutral)
			{
				color = this.TempTeamColor;
			}
			color.a = 1f;
			if (team.hidePopution && team.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
			{
				this.mHudLabel[k].text = string.Empty;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(hpArray[k]);
				this.mHudLabel[k].text = stringBuilder.ToString();
			}
			this.mHudLabel[k].color = color;
		}
		if (flag && Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.mHudRoot.transform.Rotate(0f, 0f, 90f);
		}
	}

	private void CalculateHudPoses(int teamCount)
	{
		if (this.hostNode != null)
		{
			for (int i = this.mHudPoses.Count; i < teamCount; i++)
			{
				this.mHudPoses.Add(Vector3.zero);
			}
			Vector3 position = this.hostNode.GetPosition();
			if (teamCount == 1)
			{
				this.mHudPoses[0] = new Vector3(position.x, position.y - this.hostNode.GetWidth() * 1.6f, position.z);
			}
			else if (teamCount == 2)
			{
				this.mHudPoses[0] = new Vector3(position.x, position.y - this.hostNode.GetWidth() * 3f, position.z);
				this.mHudPoses[1] = new Vector3(position.x, position.y + this.hostNode.GetWidth() * 3f, position.z);
			}
			else
			{
				float num = this.hostNode.GetWidth() * 3f;
				float num2 = 6.2831855f / (float)teamCount;
				float num3 = 1.5707964f;
				for (int j = 0; j < teamCount; j++)
				{
					float f = num3 + num2 * (float)j;
					this.mHudPoses[j] = new Vector3(position.x + Mathf.Cos(f) * num, position.y + Mathf.Sin(f) * num, position.z);
				}
			}
		}
	}

	public void ShowWarningRange(bool bWarning)
	{
		if (bWarning)
		{
			if (this.hostNode != null)
			{
				Color color = this.hostNode.currentTeam.color;
				this.m_RangeHalo.Show(color);
			}
		}
		else
		{
			this.m_RangeHalo.Hide();
		}
	}

	public void ShowLaserLine(bool bWarning)
	{
		if (bWarning)
		{
			if (this.hostNode != null)
			{
				Color white = Color.white;
				this.m_LaserLineHalo.Show(white);
			}
		}
		else
		{
			this.m_LaserLineHalo.Hide();
		}
	}

	private Node hostNode;

	private List<Color> m_clr;

	private List<float> m_p;

	private Vector3 m_PosDown = Vector3.zero;

	private Vector3 m_PosLeft = Vector3.zero;

	private Vector3 m_PosRight = Vector3.zero;

	private Vector3 m_PosUp = Vector3.zero;

	private BattleUIHalo m_Halo;

	private BattleWarningHalo m_RangeHalo;

	private BattleLaserLineHalo m_LaserLineHalo;

	private List<Vector3> mHudPoses;

	private List<UILabel> mHudLabel;

	private GameObject mHudRoot;

	private int mLastShowHudCount;

	private Color TempTeamColor = new Color32(153, 153, 153, 0);
}
