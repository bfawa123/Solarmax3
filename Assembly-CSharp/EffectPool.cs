using System;
using Solarmax;

public class EffectPool : SimplePool<EffectNode>, Lifecycle2
{
	public virtual bool Init()
	{
		return true;
	}

	public virtual void Tick(int frame, float interval)
	{
		for (int i = this.mBusyObjects.Count - 1; i >= 0; i--)
		{
			EffectNode effectNode = this.mBusyObjects[i];
			if (effectNode != null)
			{
				effectNode.Tick(frame, interval);
			}
		}
		if (this.clearCd < 1f)
		{
			this.clearCd += interval;
		}
		else
		{
			this.clearCd = 0f;
			for (int j = 0; j < this.mBusyObjects.Count; j++)
			{
				if (this.mBusyObjects[j] == null)
				{
					this.mBusyObjects.RemoveAt(j);
					break;
				}
			}
		}
	}

	public virtual void Destroy()
	{
		for (int i = this.mBusyObjects.Count - 1; i >= 0; i--)
		{
			EffectNode t = this.mBusyObjects[i];
			this.Recycle(t);
		}
		EffectNode[] array = this.mFreeObjects.ToArray();
		int j = 0;
		int num = array.Length;
		while (j < num)
		{
			EffectNode effectNode = array[j];
			effectNode.Destroy();
			j++;
		}
		base.Clear();
	}

	private float clearCd;
}
