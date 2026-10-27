using System;

public class AttributeObject
{
	public void Reset()
	{
		this.baseNum = 0f;
		this.addPercent = 0f;
		this.addNum = 0f;
		this.fixedNum = 0f;
	}

	public void Calculate()
	{
		this.fixedNum = this.baseNum * (1f + this.addPercent) + this.addNum;
		if (this.fixedNum < 0f)
		{
			this.fixedNum = 0f;
		}
	}

	public float baseNum;

	public float addPercent;

	public float addNum;

	public float fixedNum;
}
