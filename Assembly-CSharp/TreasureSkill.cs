using System;
using System.Collections.Generic;

public class TreasureSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		TEAM team = TEAM.Team_1;
		while ((int)team < LocalPlayer.MaxTeamNum)
		{
			if (this.owerNode.GetShipFactCount((int)team) > 0)
			{
				this.owerNode.BombShip(team, TEAM.Neutral, 1f);
			}
			team += 1;
		}
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			List<Ship> flyShip = this.sceneManager.shipManager.GetFlyShip((TEAM)i);
			for (int j = 0; j < flyShip.Count; j++)
			{
				if (flyShip[j] != null && flyShip[j].entity != null && flyShip[j].entity.targetNode.tag == this.owerNode.tag)
				{
					flyShip[j].Bomb(NodeType.None);
				}
			}
		}
		this.sceneManager.nodeManager.RemoveNode(this.owerNode.tag);
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		return base.OnCast(castTeam);
	}
}
