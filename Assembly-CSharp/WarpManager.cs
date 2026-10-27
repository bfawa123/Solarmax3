using System;
using System.Collections.Generic;
using Solarmax;

public class WarpManager : Lifecycle2
{
	public bool Init()
	{
		return true;
	}

	public void Tick(int frame, float interval)
	{
		int i = 0;
		while (i < this.list.Count)
		{
			this.list[i].time -= interval;
			if (this.list[i].time <= 0f)
			{
				this.list[i].Warp();
				this.list.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	public void Destroy()
	{
		this.list.Clear();
	}

	public void AddWarpItem(Node from, Node to, TEAM team, float rate, int num, bool warp)
	{
		WarpItem warpItem = new WarpItem();
		warpItem.from = from;
		warpItem.to = to;
		warpItem.team = team;
		warpItem.rate = rate;
		warpItem.num = num;
		warpItem.time = 0.5f;
		warpItem.bwarp = warp;
		this.list.Add(warpItem);
	}

	private List<WarpItem> list = new List<WarpItem>();
}
