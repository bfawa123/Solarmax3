using System;
using System.Collections;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ReconnectHandler : Solarmax.Singleton<ReconnectHandler>, IDataHandler, Lifecycle
{
	public bool Init()
	{
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.RequestUserResult, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		return true;
	}

	public void Tick(float interval)
	{
	}

	public void Destroy()
	{
		this.Release();
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.RequestUserResult, this);
	}

	public void Release()
	{
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
		if (eventId == 4)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<NetSystem>.Instance.SendCache2Net();
			}
			else if (errCode != ErrCode.EC_NoExist)
			{
				if (errCode != ErrCode.EC_NeedResume)
				{
					if (errCode == ErrCode.EC_SysUnknown)
					{
					}
				}
			}
		}
	}

	public void StartReconnect()
	{
		this.connectedNum = 0;
		global::Coroutine.Start(this.LoginServer());
	}

	private IEnumerator LoginServer()
	{
		this.connectedNum++;
		yield return Solarmax.Singleton<NetSystem>.Instance.helper.ConnectServer(false);
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestUser();
		}
		else
		{
			yield return new WaitForSeconds(3f);
			if (this.connectedNum <= 10)
			{
				global::Coroutine.Start(this.LoginServer());
			}
		}
		yield break;
	}

	private const int MAX_CONNECT_NUM = 10;

	private int connectedNum;
}
