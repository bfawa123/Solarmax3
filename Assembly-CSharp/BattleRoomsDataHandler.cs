using System;
using Solarmax;

public class BattleRoomsDataHandler : Solarmax.Singleton<BattleRoomsDataHandler>, IDataHandler, Lifecycle
{
	public bool Init()
	{
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnStorageLoaded, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnGetRaceData, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		return true;
	}

	public void Destroy()
	{
	}

	public void Tick(float interval)
	{
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
		if (eventId != 87)
		{
			if (eventId == 64)
			{
			}
		}
	}
}
