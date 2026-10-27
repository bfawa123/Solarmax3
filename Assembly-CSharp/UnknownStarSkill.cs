using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class UnknownStarSkill : BaseNewSkill
{
	public override void ReadySkill(Team castTeam, Node node, bool playEffect)
	{
		base.ReadySkill(castTeam, node, playEffect);
		Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(node.id);
		MapBuildingConfig mapBuildingConfig = null;
		for (int i = 0; i < Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Count; i++)
		{
			MapBuildingConfig mapBuildingConfig2 = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList[i];
			if (mapBuildingConfig2.tag == node.tag)
			{
				mapBuildingConfig = mapBuildingConfig2;
				break;
			}
		}
		if (mapBuildingConfig == null || mapBuildingConfig.buildIds == null || mapBuildingConfig.buildIds.Count == 0)
		{
			return;
		}
		if (Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(mapBuildingConfig.type, mapBuildingConfig.size) == null)
		{
			return;
		}
		int num = 0;
		for (int j = 0; j < mapBuildingConfig.buildIds.Count; j++)
		{
			if (mapBuildingConfig.buildIds[j] == node.id)
			{
				num = j + 1;
				break;
			}
		}
		if (num >= mapBuildingConfig.buildIds.Count)
		{
			num = 0;
		}
		int id = mapBuildingConfig.buildIds[num];
		MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(id);
		if (data == null)
		{
			return;
		}
		if (playEffect)
		{
			Solarmax.Singleton<EffectManager>.Get().PlayUnknownStarEffect(node, node.entity.nodesize / 0.38f);
		}
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_UnknownStar");
		GameObject gameObject = UnityEngine.Object.Instantiate(resources) as GameObject;
		gameObject.transform.SetParent(node.entity.GetGO().transform);
		if (node.state != NodeState.Idle)
		{
			gameObject.SetActive(false);
		}
		UnknownStarBehivor component = gameObject.GetComponent<UnknownStarBehivor>();
		string cImageShape = string.Format("{0}_shape", data.imageName);
		if (data.typeEnum == 1)
		{
			cImageShape = "planet_shape";
		}
		float num2 = data.nodesize / 0.38f / (node.entity.nodesize / 0.38f) * 0.025f;
		if (this.isFirst)
		{
			num2 = data.nodesize / node.entity.nodesize * 0.038f;
		}
		gameObject.transform.localScale = new Vector3(num2, num2, 1f);
		component.ChangeImage(data.imageName, cImageShape, this.isFirst, node);
	}

	public override bool OnCast(Team castTeam)
	{
		if (this.owerNode.state != NodeState.Idle && this.owerNode.type != NodeType.UnknownStar)
		{
			return false;
		}
		MapBuildingConfig mapBuildingConfig = null;
		for (int i = 0; i < Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Count; i++)
		{
			MapBuildingConfig mapBuildingConfig2 = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList[i];
			if (mapBuildingConfig2.tag == this.owerNode.tag)
			{
				mapBuildingConfig = mapBuildingConfig2;
				break;
			}
		}
		if (this.scriptMisc == null && (mapBuildingConfig == null || mapBuildingConfig.buildIds == null || mapBuildingConfig.buildIds.Count == 0))
		{
			return false;
		}
		MapNodeConfig data = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(mapBuildingConfig.type, mapBuildingConfig.size);
		if (data == null)
		{
			return false;
		}
		MapNodeConfig data2;
		if (this.scriptMisc == null)
		{
			int num = 0;
			for (int j = 0; j < mapBuildingConfig.buildIds.Count; j++)
			{
				if (mapBuildingConfig.buildIds[j] == this.owerNode.id)
				{
					num = j + 1;
					break;
				}
			}
			if (num >= mapBuildingConfig.buildIds.Count)
			{
				num = 0;
			}
			int id = mapBuildingConfig.buildIds[num];
			data2 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(id);
		}
		else
		{
			data2 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(int.Parse(this.scriptMisc));
		}
		if (data2 == null)
		{
			return false;
		}
		float hp = this.owerNode.hp;
		NodeState state = this.owerNode.state;
		NODESTATE node_state = this.owerNode.node_state;
		TEAM occupiedTeam = this.owerNode.occupiedTeam;
		int num2 = 0;
		List<int> list = new List<int>();
		TEAM team = TEAM.Team_1;
		while ((int)team < LocalPlayer.MaxTeamNum)
		{
			int shipFactCount = this.owerNode.GetShipFactCount((int)team);
			list.Add(shipFactCount);
			if (shipFactCount > 0)
			{
				this.owerNode.BombShipNumWithoutScaling(team, shipFactCount);
				num2 += shipFactCount;
			}
			team += 1;
		}
		string text = data2.skills;
		if (text == "NULL")
		{
			text = data.skills;
		}
		else
		{
			text = text + "," + data.skills;
		}
		string text2 = data2.perfab;
		if (text2 == "NULL")
		{
			text2 = string.Empty;
		}
		float x = this.owerNode.GetPosition().x;
		float y = this.owerNode.GetPosition().y;
		int revoType = (int)this.owerNode.revoType;
		string revoParam = this.owerNode.revoParam1;
		string revoParam2 = this.owerNode.revoParam2;
		string revoParam3 = this.owerNode.revoParam3;
		string revoParam4 = this.owerNode.revoParam4;
		bool isClockWise = this.owerNode.isClockWise;
		if (TouchHandler.currentNode == this.owerNode)
		{
			TouchHandler.HideOperater();
		}
		this.sceneManager.nodeManager.RemoveNode(this.owerNode.tag);
		Node node = this.sceneManager.AddNode(data2.id, 0, data2.typeEnum, data2.aiWeight, data2.aiUnitLost, x, y, data2.size, mapBuildingConfig.tag, data2.createshipnum, data2.createship, (float)data2.hp, data2.food, data2.attackrange, data2.attackspeed, data2.attackpower, data.attackspeed, revoType, revoParam, revoParam2, revoParam3, revoParam4, isClockWise, data2.nodesize, text2, text, mapBuildingConfig.fAngle, int.Parse(mapBuildingConfig.aistrategy), 0f, 0f);
		node.shipProduceOverride = mapBuildingConfig.shipProduceOverride;
		node.RevoSpeed = (float)mapBuildingConfig.orbitRevoSpeed;
		if (node != null && node.entity != null)
		{
			node.entity.FadeEntity(true, 0.5f);
			node.hp = hp;
			node.state = state;
			node.node_state = node_state;
			node.occupiedTeam = occupiedTeam;
			if (num2 > 0)
			{
				for (int k = 0; k < list.Count; k++)
				{
					if (list[k] > 0)
					{
						node.AddShip(k + 1, list[k], true, true);
					}
				}
			}
			this.sceneManager.shipManager.ChangeTargetNode(node);
		}
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		this.ReadySkill(castTeam, node, true);
		return base.OnCast(castTeam);
	}
}
