using System;

public class WarpItem
{
	public void Warp()
	{
		if (this.num == 0)
		{
			this.from.MoveTo(this.team, this.to, this.rate, this.bwarp);
		}
		else
		{
			this.from.MoveTo(this.team, this.to, this.num, this.bwarp);
		}
	}

	public Node from;

	public Node to;

	public TEAM team;

	public float rate;

	public int num;

	public float time;

	public bool bwarp;
}
