using System;
using System.Collections;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ReconnectWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.RequestUserResult);
		base.RegisterEvent(EventId.CreateUserResult);
		base.RegisterEvent(EventId.NetworkStatus);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.Reconnect();
		this.waitBeginTime = Time.realtimeSinceStartup;
		this.tips.text = LanguageDataProvider.GetValue(501);
		base.InvokeRepeating("UpdateTips", 1f, 1f);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.RequestUserResult)
		{
			ErrCode errCode = (ErrCode)args[0];
			Debug.Log(errCode.ToString());
			if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<NetSystem>.Instance.SendCache2Net();
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.ReconnectResult, new object[0]);
				Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
			}
			else if (errCode == ErrCode.EC_NoExist)
			{
				this.CreateDefaultUser();
				Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
			}
			else if (errCode == ErrCode.EC_NeedResume)
			{
				base.gameObject.SetActive(false);
				Solarmax.Singleton<NetSystem>.Instance.IsReconnecting = false;
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
				{
					2,
					LanguageDataProvider.GetValue(20),
					new EventDelegate(new EventDelegate.Callback(this.ReconnectResume)),
					new EventDelegate(new EventDelegate.Callback(this.ReconnectGiveup))
				});
			}
			else if (errCode == ErrCode.EC_SysUnknown)
			{
				this.connectedNum++;
				Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
				Solarmax.Singleton<NetSystem>.Instance.Close();
			}
			else if (errCode == ErrCode.EC_NeedUpdate)
			{
				string message = string.Format(LanguageDataProvider.GetValue(2170), this.connectedNum);
				Tips.Make(Tips.TipsType.FlowUp, message, 3f);
				Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
			}
		}
		else if (eventId == EventId.CreateUserResult)
		{
			ErrCode errCode2 = (ErrCode)args[0];
			if (errCode2 == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.ReconnectResult, new object[0]);
				Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
			}
		}
	}

	public void ReconnectResume()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.ReconnectResume(-2);
		Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
	}

	public void ReconnectGiveup()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(-1);
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitBattle(-1);
		Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
	}

	public override void OnHide()
	{
		global::Coroutine.StopCoroutines();
		this.bVisbledCommonDialog = false;
		Solarmax.Singleton<NetSystem>.Instance.IsReconnecting = false;
	}

	public void UpdateTips()
	{
		if (this.connectedNum <= 10)
		{
			this.tips.text = string.Format(LanguageDataProvider.GetValue(502), this.connectedNum);
		}
		else
		{
			this.tips.text = string.Empty;
			if (!this.bVisbledCommonDialog)
			{
				this.EndBattle();
			}
		}
	}

	private void Reconnect()
	{
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

	private void CreateDefaultUser()
	{
		int index = UnityEngine.Random.Range(0, 10);
		string icon = SelectIconWindow.GetIcon(index);
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.CreateUser(string.Empty, icon);
		}
	}

	private void EndBattle()
	{
		this.bVisbledCommonDialog = true;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
		{
			2,
			LanguageDataProvider.GetValue(22),
			new EventDelegate(new EventDelegate.Callback(this.ReconnectCommit)),
			new EventDelegate(new EventDelegate.Callback(this.ReconnectCancle))
		});
	}

	public void ReconnectCommit()
	{
		this.bVisbledCommonDialog = false;
		base.CancelInvoke("Reconnect");
		base.Invoke("Reconnect", 0.5f);
		this.connectedNum = 0;
		this.waitBeginTime = Time.realtimeSinceStartup;
	}

	public void ReconnectCancle()
	{
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		if (battleData.gameType == GameType.PVP || battleData.gameType == GameType.League)
		{
			Solarmax.Singleton<BattleSystem>.Instance.Reset();
		}
		if (!Solarmax.Singleton<UISystem>.Get().IsWindowVisible("HomeWindow"))
		{
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
		}
		else
		{
			Solarmax.Singleton<UISystem>.Instance.HideWindow("ReconnectWindow");
			this.bVisbledCommonDialog = false;
			Solarmax.Singleton<NetSystem>.Instance.IsReconnecting = false;
		}
	}

	public UILabel tips;

	private float waitBeginTime;

	private int connectedNum;

	private bool bVisbledCommonDialog;
}
