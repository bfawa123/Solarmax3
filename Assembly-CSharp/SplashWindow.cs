using System;
using UnityEngine;

public class SplashWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		return true;
	}

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

	private void Awake()
	{
		this.logoPage.gameObject.SetActive(false);
		this.warningPage.gameObject.SetActive(false);
		base.Invoke("ShowWarning", 0.5f);
		base.Invoke("ShowLogo", 0.5f);
	}

	private void Update()
	{
	}

	private void ShowLogo()
	{
		this.logoPage.gameObject.SetActive(true);
		this.logoPage.value = 0f;
		this.logoPage.ResetToBeginning();
		this.logoPage.from = 0f;
		this.logoPage.to = 1f;
		this.logoPage.duration = 0.5f;
		this.logoPage.enabled = true;
		this.logoPage.PlayForward();
	}

	private void HideLogo()
	{
		this.logoPage.gameObject.SetActive(true);
		this.logoPage.ResetToBeginning();
		this.logoPage.from = 1f;
		this.logoPage.to = 0f;
		this.logoPage.duration = 0.5f;
		this.logoPage.enabled = true;
		this.logoPage.PlayForward();
		base.Invoke("ShowWarning", 0.8f);
	}

	private void ShowWarning()
	{
		this.warningPage.gameObject.SetActive(true);
		this.warningPage.value = 0f;
		this.warningPage.ResetToBeginning();
		this.warningPage.from = 0f;
		this.warningPage.to = 1f;
		this.warningPage.duration = 0.5f;
		this.warningPage.enabled = true;
		this.warningPage.PlayForward();
		base.Invoke("HideWarning", 1.5f);
	}

	private void HideWarning()
	{
		this.warningPage.gameObject.SetActive(true);
		this.warningPage.ResetToBeginning();
		this.warningPage.from = 1f;
		this.warningPage.to = 0f;
		this.warningPage.duration = 1f;
		this.warningPage.enabled = true;
		this.warningPage.PlayForward();
		base.Invoke("Close", 0.8f);
	}

	private void Close()
	{
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 1.2f;
		tweenAlpha.SetOnFinished(delegate()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		});
		base.Invoke("showLogoWindow", 0.8f);
	}

	private void showLogoWindow()
	{
		UISystem.DirectShowPrefab("gameres/ui/logowindow_h3");
	}

	public TweenAlpha logoPage;

	public TweenAlpha warningPage;
}
