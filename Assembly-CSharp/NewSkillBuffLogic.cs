using System;
using System.Collections.Generic;
using Solarmax;

public class NewSkillBuffLogic : Lifecycle2
{
	public NewSkillBuffLogic()
	{
		this.buffList = new List<BaseNewBuff>();
		this.deleteTemp = new List<BaseNewBuff>();
	}

	public bool Init()
	{
		this.buffList.Clear();
		return true;
	}

	public void Tick(int frame, float interval)
	{
		int i = 0;
		int count = this.buffList.Count;
		while (i < count)
		{
			BaseNewBuff baseNewBuff = this.buffList[i];
			baseNewBuff.Tick(frame, interval);
			if (!baseNewBuff.IsActive())
			{
				baseNewBuff.Destroy();
				this.deleteTemp.Add(baseNewBuff);
			}
			i++;
		}
		int j = 0;
		int count2 = this.deleteTemp.Count;
		while (j < count2)
		{
			this.buffList.Remove(this.deleteTemp[j]);
			j++;
		}
		this.deleteTemp.Clear();
	}

	public void Destroy()
	{
		int i = 0;
		int count = this.buffList.Count;
		while (i < count)
		{
			this.buffList[i].Destroy();
			i++;
		}
		this.buffList.Clear();
	}

	public void Add(BaseNewBuff buff)
	{
		this.buffList.Add(buff);
		this.buffList.Sort(new Comparison<BaseNewBuff>(this.ComparisonOrder));
	}

	private int ComparisonOrder(BaseNewBuff arg0, BaseNewBuff arg1)
	{
		int num = arg0.config.order.CompareTo(arg1.config.order);
		return (num != 0) ? num : arg0.startTime.CompareTo(arg1.startTime);
	}

	private List<BaseNewBuff> buffList;

	private List<BaseNewBuff> deleteTemp;
}
