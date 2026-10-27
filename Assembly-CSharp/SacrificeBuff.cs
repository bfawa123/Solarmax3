using System;
using UnityEngine;

public class SacrificeBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.targetNode.BombShipNum(this.targetTeam.team, this.sendTeam.team, int.Parse(this.config.arg0));
		Debug.Log(string.Concat(new string[]
		{
			"SacrificeBuff:",
			this.targetTeam.team.ToString(),
			",",
			this.sendTeam.team.ToString(),
			",",
			this.config.arg0
		}));
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
