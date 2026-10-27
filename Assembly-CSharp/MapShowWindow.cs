using System;

public class MapShowWindow : BaseWindow
{
	private void Awake()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		this.mapShowObj.Switch("1", false);
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	public void OnMapClick()
	{
	}

	public MapShow mapShowObj;
}
