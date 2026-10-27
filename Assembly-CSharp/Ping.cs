using System;
using Solarmax;
using UnityEngine;

public class Ping : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("RegistEvent", 0.5f);
		this.SetPicAndValue();
	}

	private void RegistEvent()
	{
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.NetworkStatus, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.PingRefresh, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
		if (eventId == 82)
		{
			bool flag = (bool)args[0];
			this.SetPicAndValue();
		}
		else if (eventId == 83)
		{
			this.SetPicAndValue();
		}
	}

	public void OnClickPic(GameObject go)
	{
		this.SetPicAndValue();
	}

	public void AutoHidePingValue()
	{
	}

	private void SetPicAndValue()
	{
		string spriteName;
		Color color;
		Solarmax.Singleton<NetSystem>.Instance.ping.GetNetPic(out spriteName, out color);
		this.pingPic.spriteName = spriteName;
		this.pingPic.color = color;
		float lastPingTime = Solarmax.Singleton<NetSystem>.Instance.ping.lastPingTime;
		if (this.pingValue == null)
		{
			return;
		}
		this.pingValue.text = string.Format(LanguageDataProvider.GetValue(504), Mathf.RoundToInt(lastPingTime));
		this.pingValue.color = color;
	}

	public UISprite pingPic;

	public UILabel pingValue;

	private float appPauseBeginTime;
}
