using System;
using System.IO;
using Solarmax;
using UnityEngine;

public class BGTemplate : MonoBehaviour
{
	public void EnsureInit(SkinConfig config, bool preview = false)
	{
		if (config == null)
		{
			return;
		}
		this.config = config;
		this.isPreview = preview;
		this.progressLabel.gameObject.SetActive(false);
		if (this.isPreview)
		{
			this.labelApply.gameObject.SetActive(false);
			this.curProgress.SetActive(false);
			this.moneyIcon.SetActive(false);
			this.moneyNum.gameObject.SetActive(false);
			if ((BGManager.Inst.CurrentSkinConfig() == null && config.id == BGManager.DEFAULT_BG_ID) || BGManager.Inst.CurrentSkinConfig() == config)
			{
				UIToggle component = base.transform.gameObject.GetComponent<UIToggle>();
				if (component != null)
				{
					component.value = true;
				}
				Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig = config;
				if (config.unlock)
				{
					this.applyLabel.SetActive(true);
					this.buyButton.SetActive(false);
				}
				else
				{
					this.applyLabel.SetActive(false);
					this.buyButton.SetActive(true);
					this.buyLabel.text = config.goodValue.ToString();
				}
			}
		}
		else
		{
			this.labelApply.gameObject.SetActive(config.unlock);
			this.curProgress.SetActive(false);
			this.moneyIcon.SetActive(!config.unlock);
			this.moneyNum.gameObject.SetActive(!config.unlock);
			this.moneyNum.text = config.goodValue.ToString();
			if (!config.unlock)
			{
				this.select.enabled = false;
			}
		}
		this.bApllyBg = this.IsAppluBG();
		this.UpdateAvatar();
		string[] array = config.bgImage.Split(new char[]
		{
			','
		});
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string fileName = MonoSingleton<UpdateSystem>.Instance.saveSkin + array[i] + ".ab";
				FileInfo fileInfo = new FileInfo(fileName);
				if (config.url.StartsWith("http") && !fileInfo.Exists)
				{
					this.downloadComplete = false;
					break;
				}
				this.downloadComplete = true;
			}
		}
		if (this.NeedDownloadBG())
		{
			this.curSlider.value = 1f;
			this.curProgress.SetActive(true);
		}
		else
		{
			this.curSlider.value = 0f;
			this.curProgress.SetActive(false);
		}
	}

	public void OnClicked()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BGTemplate OnClicked", new object[0]);
		if (this.isPreview)
		{
			if (Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig == this.config)
			{
				return;
			}
			if (MonoSingleton<UpdateSystem>.Instance.IsDownLoadIng())
			{
				return;
			}
			this.bBuyBG = true;
			Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig = this.config;
			if (this.config.unlock)
			{
				this.applyLabel.SetActive(true);
				this.buyButton.SetActive(false);
			}
			else
			{
				this.applyLabel.SetActive(false);
				this.buyButton.SetActive(true);
				this.buyLabel.text = this.config.goodValue.ToString();
			}
			if (BGManager.Inst.DownloadBG(this.config))
			{
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateSkinStatus, new object[0]);
				return;
			}
			this.curProgress.SetActive(false);
			this.labelApply.gameObject.SetActive(false);
			BGManager.Inst.ApplySkinConfig(this.config, false);
			return;
		}
		else
		{
			if ((BGManager.Inst.CurrentSkinConfig() == null && this.config.id == BGManager.DEFAULT_BG_ID) || BGManager.Inst.CurrentSkinConfig() == this.config)
			{
				this.select.startsActive = true;
				Solarmax.Singleton<LocalSettingStorage>.Get().bg = this.config.id;
				Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
				Solarmax.Singleton<LoggerSystem>.Instance.Info("背景相同，不要更换", new object[0]);
				return;
			}
			CollectionWindow.choosedBg = null;
			if (!this.config.unlock)
			{
				this.BuySkin();
				return;
			}
			this.bBuyBG = true;
			if (MonoSingleton<UpdateSystem>.Instance.IsDownLoadIng())
			{
				return;
			}
			BGManager.Inst.ApplySkinConfig(this.config, false);
			CollectionWindow.choosedBg = this.config;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateSkinStatus, new object[0]);
			return;
		}
	}

	public void BuySkin()
	{
		if (this.isPreview && Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig != this.config)
		{
			return;
		}
		if (MonoSingleton<UpdateSystem>.Instance.IsDownLoadIng())
		{
			Tips.Make(LanguageDataProvider.GetValue(2161));
			return;
		}
		this.bBuyBG = true;
		if ((double)Solarmax.Singleton<LocalPlayer>.Get().playerData.money < this.config.goodValue)
		{
			Tips.Make(LanguageDataProvider.GetValue(1102));
			return;
		}
		CollectionBuyModel collectionBuyModel = new CollectionBuyModel
		{
			buyType = 1,
			buyId = this.config.id,
			url = this.config.skinImageName,
			gold = this.config.goodValue.ToString()
		};
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CollectionBuyWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnShowBuyWindowEvent, new object[]
		{
			collectionBuyModel
		});
	}

	private void UpdateAvatar()
	{
		string skinImageName = this.config.skinImageName;
		this.portrait.Load(skinImageName, null, null);
		if (!this.isPreview)
		{
			if ((BGManager.Inst.CurrentSkinConfig() == null && this.config.id == BGManager.DEFAULT_BG_ID) || BGManager.Inst.CurrentSkinConfig() == this.config)
			{
				this.select.startsActive = true;
				Solarmax.Singleton<LocalSettingStorage>.Get().bg = this.config.id;
				Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
			}
			else if (this.config.id == Solarmax.Singleton<LocalSettingStorage>.Get().bg)
			{
				this.select.startsActive = true;
			}
			this.UpdataSkinStatus();
		}
	}

	public void UpdataSkinStatus()
	{
		if (this.isPreview)
		{
			if (this.bApllyBg && Solarmax.Singleton<LocalSettingStorage>.Get().bg != this.config.id)
			{
				this.labelApply.text = string.Empty;
			}
			else if (this.isDownloading)
			{
				this.labelApply.text = LanguageDataProvider.GetValue(2087);
			}
			else if (this.config.url.StartsWith("http") && !this.downloadComplete)
			{
				this.labelApply.text = LanguageDataProvider.GetValue(2162);
			}
			else
			{
				this.labelApply.text = string.Empty;
			}
		}
		else if (this.bApllyBg && Solarmax.Singleton<LocalSettingStorage>.Get().bg != this.config.id)
		{
			this.labelApply.text = string.Empty;
		}
		else if (this.isDownloading)
		{
			this.labelApply.text = LanguageDataProvider.GetValue(2087);
		}
		else if (Solarmax.Singleton<LocalSettingStorage>.Get().bg == this.config.id)
		{
			this.isDownloading = false;
			this.labelApply.text = LanguageDataProvider.GetValue(2163);
		}
		else if (this.config.url.StartsWith("http") && !this.downloadComplete)
		{
			this.labelApply.text = LanguageDataProvider.GetValue(2162);
		}
		else
		{
			this.labelApply.text = string.Empty;
		}
	}

	public void StartDownLoad()
	{
		if (this.isPreview)
		{
			if (!this.bBuyBG || this.downloadComplete)
			{
				return;
			}
			this.progressLabel.gameObject.SetActive(true);
			this.labelApply.gameObject.SetActive(true);
			this.isDownloading = true;
			this.nCurProgress = 0;
			this.nMaxProgress = 3;
			this.curSlider.value = 1f;
			this.curProgress.SetActive(true);
			this.UpdataSkinStatus();
		}
		else
		{
			if (!this.bBuyBG || this.downloadComplete)
			{
				return;
			}
			this.progressLabel.gameObject.SetActive(true);
			this.labelApply.gameObject.SetActive(this.config.unlock);
			this.isDownloading = true;
			this.nCurProgress = 0;
			this.nMaxProgress = 3;
			this.curSlider.value = 1f;
			this.curProgress.SetActive(true);
			this.UpdataSkinStatus();
		}
	}

	public void UpdateProgress(int nAddVuale)
	{
		if (this.isPreview)
		{
			if (!this.bBuyBG || this.downloadComplete)
			{
				return;
			}
			this.nCurProgress += nAddVuale;
			float num = (float)this.nCurProgress / ((float)this.nMaxProgress * 1f);
			this.curSlider.value = 1f - num;
			if (this.nCurProgress >= this.nMaxProgress)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1120), 1f);
				this.isDownloading = false;
				this.downloadComplete = true;
				this.HideDownLoad();
			}
		}
		else
		{
			if (!this.bBuyBG || this.downloadComplete)
			{
				return;
			}
			this.nCurProgress += nAddVuale;
			float num2 = (float)this.nCurProgress / ((float)this.nMaxProgress * 1f);
			this.curSlider.value = 1f - num2;
			if (this.nCurProgress >= this.nMaxProgress)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1120), 1f);
				this.isDownloading = false;
				this.downloadComplete = true;
				this.HideDownLoad();
			}
		}
	}

	private void HideDownLoad()
	{
		if (this.isPreview)
		{
			BGManager.Inst.ApplySkinConfig(this.config, true);
			this.progressLabel.gameObject.SetActive(false);
			CollectionWindow.choosedBg = this.config;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateSkinStatus, new object[0]);
			this.bBuyBG = false;
			if (this.config != null)
			{
			}
			this.curProgress.SetActive(false);
			this.select.Set(true, true);
			this.labelApply.gameObject.SetActive(false);
		}
		else
		{
			BGManager.Inst.ApplySkinConfig(this.config, true);
			this.progressLabel.gameObject.SetActive(false);
			CollectionWindow.choosedBg = this.config;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateSkinStatus, new object[0]);
			this.bBuyBG = false;
			if (this.config != null)
			{
				this.labelApply.text = LanguageDataProvider.GetValue(2163);
			}
			this.curProgress.SetActive(false);
			this.select.Set(true, true);
		}
	}

	private bool IsAppluBG()
	{
		if (!this.config.url.StartsWith("http"))
		{
			return false;
		}
		string[] array = this.config.bgImage.Split(new char[]
		{
			','
		});
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string fileName = MonoSingleton<UpdateSystem>.Instance.saveSkin + array[i] + ".ab";
				FileInfo fileInfo = new FileInfo(fileName);
				if (!fileInfo.Exists)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void OnBuyResponse()
	{
		if (!this.isPreview || Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig != this.config)
		{
			return;
		}
		if (this.config.unlock)
		{
			this.applyLabel.SetActive(true);
			this.buyButton.SetActive(false);
		}
		else
		{
			this.applyLabel.SetActive(false);
			this.buyButton.SetActive(true);
			this.buyLabel.text = this.config.goodValue.ToString();
		}
	}

	private void Update()
	{
		if (this.progressLabel.gameObject.activeSelf)
		{
			this.interval += Time.deltaTime;
			if (this.interval >= this.cd)
			{
				this.interval = 0f;
				int num = (this.nCurProgress + 1) * 33;
				if (this.nCurProgress == 3)
				{
					this.curSlider.value = 0f;
					this.progressLabel.text = string.Format("100%", new object[0]);
					return;
				}
				if (this.progressValue < num)
				{
					this.progressValue++;
				}
				else
				{
					this.progressValue = num;
				}
				this.curSlider.value = 1f - (float)this.progressValue * 0.01f;
				this.progressLabel.text = string.Format("{0}%", this.progressValue);
			}
		}
	}

	public bool NeedDownloadBG()
	{
		if (this.config == null)
		{
			return false;
		}
		string[] array = this.config.bgImage.Split(new char[]
		{
			','
		});
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string fileName = MonoSingleton<UpdateSystem>.Instance.saveSkin + array[i] + ".ab";
				FileInfo fileInfo = new FileInfo(fileName);
				if (this.config.url.StartsWith("http") && !fileInfo.Exists)
				{
					return true;
				}
			}
		}
		return false;
	}

	public GameObject lockIcon;

	public SkinConfig config;

	public PortraitTemplate portrait;

	public UIToggle select;

	public UILabel labelApply;

	public GameObject moneyIcon;

	public UILabel moneyNum;

	public GameObject curProgress;

	public UISlider curSlider;

	public GameObject applyLabel;

	public GameObject buyButton;

	public UILabel buyLabel;

	public UILabel progressLabel;

	private int nMaxProgress = 3;

	private int nCurProgress;

	private bool bApllyBg;

	private bool bBuyBG;

	private bool isDownloading;

	private bool downloadComplete;

	private bool isPreview;

	private float cd = 0.04f;

	private float interval;

	private int progressValue;
}
