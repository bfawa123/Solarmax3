﻿using System;
using Solarmax;
using UnityEngine;

public class LoadingWindow : BaseWindow
{
	private void Awake()
	{
	}

	private void Start()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LoadingWindow Start", new object[0]);
		this.OnShow();
		this.progressGo.SetActive(false);
		this.progressLabel.text = string.Empty;
		base.Invoke("DelayStart", 0.01f);
		base.RegisterEvent(EventId.OnABDownloadingFinished, "loadingwndow");
		base.RegisterEvent(EventId.OnStartDownLoad, "loadingwndow");
		base.RegisterEvent(EventId.OnUpdateDownLoad, "loadingwndow");
		Solarmax.Singleton<UISystem>.Get().DirectAdd(this);
	}

	private void DelayStart()
	{
		MonoSingleton<UpdateSystem>.Instance.CheckUpgrade(false);
	}

	public override bool Init()
	{
		base.Init();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.loadingTip.text = LanguageDataProvider.GetValue(2223);
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnABDownloadingFinished)
		{
			Game.game.Init();
			base.Invoke("HideDownLoad", 0.2f);
		}
		else if (eventId == EventId.OnStartDownLoad)
		{
			this.progressGo.SetActive(true);
			this.nCurProgress = (long)args[0];
			this.nMaxProgress = (long)args[1];
			this.UpdateProgress(this.nCurProgress);
		}
		else if (eventId == EventId.OnUpdateDownLoad)
		{
			long nAddVuale = (long)args[0];
			this.UpdateProgress(nAddVuale);
		}
	}

	private void UpdateProgress(long nAddVuale)
	{
		this.nCurProgress += nAddVuale;
		float num = (float)this.nCurProgress / ((float)this.nMaxProgress * 1f);
		this.curSlider.value = num;
		this.progressLabel.text = string.Format("{0}%", (int)(num * 100f));
		if (num >= 1f)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1120), 1f);
		}
		string value = LanguageDataProvider.GetValue(2222);
		string text = string.Format(value, this.Cover2String(this.nCurProgress), this.Cover2String(this.nMaxProgress));
		this.loadingTip.text = text;
	}

	private void HideDownLoad()
	{
		Solarmax.Singleton<UISystem>.Get().DirectDel(this);
		Solarmax.Singleton<UISystem>.Get().ShowWindow("LogoWindow");
	}

	private string Cover2String(long nSize)
	{
		string arg = ((double)nSize / 1024.0 / 1024.0).ToString("0.00");
		return string.Format("{0} MB", arg);
	}

	public GameObject progressGo;

	public UILabel progressLabel;

	public UILabel loadingTip;

	public UISlider curSlider;

	private long nMaxProgress = 1L;

	private long nCurProgress;
}
