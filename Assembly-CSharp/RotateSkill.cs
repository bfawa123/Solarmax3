using System;
using Solarmax;
using UnityEngine;

public class RotateSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		if (this.owerNode.IsHaveAnyShip())
		{
			return false;
		}
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == this.owerNode.tag);
		if (mapBuildingConfig == null)
		{
			return false;
		}
		if (this.owerNode.imageRoate)
		{
			float nodeImageAngle = this.owerNode.GetNodeImageAngle();
			if (nodeImageAngle + mapBuildingConfig.lasergunRotateSkip <= mapBuildingConfig.lasergunAngle + mapBuildingConfig.lasergunRange)
			{
				Vector3 vector = new Vector3(0f, 0f, nodeImageAngle);
				Vector3 vector2 = new Vector3(0f, 0f, nodeImageAngle + mapBuildingConfig.lasergunRotateSkip);
				this.owerNode.RotateNodeImage(mapBuildingConfig.lasergunRotateSkip);
			}
			else
			{
				this.owerNode.imageRoate = false;
				if (nodeImageAngle - mapBuildingConfig.lasergunRotateSkip >= mapBuildingConfig.lasergunAngle)
				{
					Vector3 vector3 = new Vector3(0f, 0f, nodeImageAngle);
					Vector3 vector4 = new Vector3(0f, 0f, nodeImageAngle - mapBuildingConfig.lasergunRotateSkip);
					this.owerNode.RotateNodeImage(-mapBuildingConfig.lasergunRotateSkip);
				}
			}
		}
		else
		{
			float nodeImageAngle2 = this.owerNode.GetNodeImageAngle();
			if (nodeImageAngle2 - mapBuildingConfig.lasergunRotateSkip >= mapBuildingConfig.lasergunAngle)
			{
				Vector3 vector5 = new Vector3(0f, 0f, nodeImageAngle2);
				Vector3 vector6 = new Vector3(0f, 0f, nodeImageAngle2 - mapBuildingConfig.lasergunRotateSkip);
				this.owerNode.RotateNodeImage(-mapBuildingConfig.lasergunRotateSkip);
			}
			else
			{
				this.owerNode.imageRoate = true;
				if (nodeImageAngle2 + mapBuildingConfig.lasergunRotateSkip <= mapBuildingConfig.lasergunAngle + mapBuildingConfig.lasergunRange)
				{
					Vector3 vector7 = new Vector3(0f, 0f, nodeImageAngle2);
					Vector3 vector8 = new Vector3(0f, 0f, nodeImageAngle2 + mapBuildingConfig.lasergunRotateSkip);
					this.owerNode.RotateNodeImage(mapBuildingConfig.lasergunRotateSkip);
				}
			}
		}
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		return false;
	}
}
