using System;

public class BarrierPointNode : Node
{
	public BarrierPointNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.BarrierPoint;
		}
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
		base.UpdateOrbit(frame, interval);
		base.UpdateState(frame, interval);
		base.UpdateOccupied(frame, interval);
		base.UpdateBattle(frame, interval);
		base.UpdateCapturing(frame, interval);
		this.UpdateBarrierLines();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void UpdateBarrierLines()
	{
		if (this.haveShip == -1)
		{
			if (base.IsHaveAnyShip())
			{
				this.haveShip = 1;
			}
			else
			{
				this.haveShip = 0;
			}
			return;
		}
		if (this.haveShip == 0 && base.IsHaveAnyShip())
		{
			this.haveShip = 1;
			base.nodeManager.ResetDynamicBarrierLines(base.tag);
		}
		else if (this.haveShip == 1 && !base.IsHaveAnyShip())
		{
			this.haveShip = 0;
			base.nodeManager.ResetDynamicBarrierLines(base.tag);
		}
	}

	public int haveShip = -1;
}
