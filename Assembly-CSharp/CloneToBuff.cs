using System;
using System.Collections.Generic;
using Solarmax;

public class CloneToBuff : BaseNewBuff
{
	protected override void Apply()
	{
		float hp = this.targetNode.hp;
		int num = 0;
		List<int> list = new List<int>();
		TEAM team = TEAM.Team_1;
		while ((int)team < LocalPlayer.MaxTeamNum)
		{
			int shipFactCount = this.targetNode.GetShipFactCount((int)team);
			list.Add(shipFactCount);
			if (shipFactCount > 0)
			{
				this.targetNode.BombShip(team, TEAM.Neutral, 1f);
				num += shipFactCount;
			}
			team += 1;
		}
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
		MapBuildingConfig mapBuildingConfig = null;
		MapBuildingConfig mapBuildingConfig2 = null;
		if (data != null)
		{
			foreach (MapBuildingConfig mapBuildingConfig3 in data.mbcList)
			{
				if (mapBuildingConfig3.tag == this.config.arg0)
				{
					mapBuildingConfig = mapBuildingConfig3;
				}
				else if (mapBuildingConfig3.tag == this.targetNode.tag)
				{
					mapBuildingConfig2 = mapBuildingConfig3;
				}
			}
		}
		if (mapBuildingConfig != null && mapBuildingConfig2 != null)
		{
			MapNodeConfig data2 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(mapBuildingConfig.type, mapBuildingConfig.size);
			this.sceneManager.nodeManager.RemoveNode(this.targetNode.tag);
			Node node = this.sceneManager.AddNode(data2.id, (int)this.targetTeam.team, data2.typeEnum, data2.aiWeight, data2.aiUnitLost, mapBuildingConfig2.x, mapBuildingConfig2.y, data2.size, mapBuildingConfig2.tag, data2.createshipnum, data2.createship, (float)data2.hp, data2.food, data2.attackrange, data2.attackspeed, data2.attackpower, 0f, mapBuildingConfig.orbit, mapBuildingConfig.orbitParam1, mapBuildingConfig.orbitParam2, mapBuildingConfig.orbitParam3, mapBuildingConfig.orbitParam4, mapBuildingConfig.orbitClockWise, data2.nodesize, string.Empty, data2.skills, mapBuildingConfig.fAngle, int.Parse(mapBuildingConfig.aistrategy), 0f, 0f);
			node.shipProduceOverride = mapBuildingConfig.shipProduceOverride;
			node.RevoSpeed = (float)mapBuildingConfig.orbitRevoSpeed;
			if (node != null && node.entity != null)
			{
				node.entity.FadeEntity(true, 0.3f);
				node.hp = hp;
				if (num > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] > 0)
						{
							node.AddShip(i + 1, list[i], true, true);
						}
					}
				}
				for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
				{
					List<Ship> flyShip = this.sceneManager.shipManager.GetFlyShip((TEAM)j);
					for (int k = 0; k < flyShip.Count; k++)
					{
						if (flyShip[k] != null && flyShip[k].entity != null && flyShip[k].entity.targetNode.tag == node.tag)
						{
							flyShip[k].entity.targetNode = node;
						}
					}
				}
			}
		}
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		base.Disable();
	}
}
