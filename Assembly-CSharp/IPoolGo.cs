using System;

public class IPoolGo
{
	public virtual void Init(Action recycle, Action destroy)
	{
		this.recyle = recycle;
		this.destroy = destroy;
	}

	public virtual void Recycle()
	{
		if (this.recyle != null)
		{
			this.recyle();
		}
	}

	public virtual void Destory()
	{
		if (this.destroy != null)
		{
			this.destroy();
		}
	}

	public virtual void Tick(float delta)
	{
	}

	private Action recyle;

	private Action destroy;
}
