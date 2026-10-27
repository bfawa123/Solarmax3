using System;
using Solarmax;

public class GMWindow : BaseWindow
{
	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	public void OnGoldClick()
	{
		this.SendCMD("add 1 10000");
	}

	public void OnJewelClick()
	{
		this.SendCMD("add 2 100");
	}

	public void OnCMDClick()
	{
		if (string.IsNullOrEmpty(this.input.value))
		{
			return;
		}
		this.SendCMD(this.input.value);
	}

	public void OnStopAIClick()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.useAI = false;
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatch3(false);
		this.OnCloseClick();
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("GMWindow");
	}

	private void SendCMD(string cmd)
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestGMCommand(cmd);
	}

	public UIInput input;
}
