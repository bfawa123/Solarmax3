using System;

public class PlanetNode : Node
{
	public PlanetNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Planet;
		}
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
		base.UpdateNodeSkill(frame, interval);
		base.UpdateOrbit(frame, interval);
		base.UpdateState(frame, interval);
		this.UpdateOccupied(frame, interval);
		base.UpdateBattle(frame, interval);
		base.UpdateProduce(frame, interval, true);
		this.UpdateCapturing(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
