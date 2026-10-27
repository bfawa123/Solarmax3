using System;
using System.Collections;
using System.IO;
using Solarmax;
using UnityEngine;

public class BGView : MonoBehaviour
{
	private void Awake()
	{
		this.scroll = this.table.transform.parent.gameObject.GetComponent<UIScrollView>();
	}

	public void EnsureInit()
	{
		this.model = Solarmax.Singleton<CollectionModel>.Get();
		CollectionModel collectionModel = Solarmax.Singleton<CollectionModel>.Get();
		collectionModel.onAvatarChanged = (CollectionModel.OnAvatarChanged)Delegate.Combine(collectionModel.onAvatarChanged, new CollectionModel.OnAvatarChanged(this.OnBGChanged));
		base.StartCoroutine(this.UpdateUI());
	}

	public void EnsureDestroy()
	{
		CollectionModel collectionModel = Solarmax.Singleton<CollectionModel>.Get();
		collectionModel.onAvatarChanged = (CollectionModel.OnAvatarChanged)Delegate.Remove(collectionModel.onAvatarChanged, new CollectionModel.OnAvatarChanged(this.OnBGChanged));
		if (Solarmax.Singleton<CollectionModel>.Get().NeedSyncAvatar())
		{
		}
		this.ClearTable();
	}

	private IEnumerator UpdateUI()
	{
		this.scroll.ResetPosition();
		yield return null;
		if (BGManager.Inst.CurrentSkinConfig() != null)
		{
			SkinConfig skinConfig = BGManager.Inst.CurrentSkinConfig();
			string[] array = skinConfig.bgImage.Split(new char[]
			{
				','
			});
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string fileName = MonoSingleton<UpdateSystem>.Instance.saveSkin + array[i] + ".ab";
					FileInfo fileInfo = new FileInfo(fileName);
					if (skinConfig.url.StartsWith("http") && !fileInfo.Exists)
					{
						BGManager.Inst.EmptySkinConfig();
						break;
					}
				}
			}
		}
		foreach (SkinConfig config in this.model.bgConfigs)
		{
			GameObject gameObject = this.table.gameObject.AddChild(this.template);
			gameObject.SetActive(true);
			BGTemplate component = gameObject.GetComponent<BGTemplate>();
			component.EnsureInit(config, false);
		}
		this.table.Reposition();
		this.scroll.ResetPosition();
		yield break;
	}

	private void ClearTable()
	{
		this.table.transform.DestroyChildren();
	}

	private void AfterChoosedAvatar()
	{
	}

	private void OnBGChanged()
	{
		if (Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("Choosed avatar error", new object[0]);
			return;
		}
	}

	public void UpdateProgress(int nAddVuale)
	{
		for (int i = 0; i < this.table.transform.childCount; i++)
		{
			BGTemplate component = this.table.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.UpdateProgress(nAddVuale);
		}
	}

	public void StartDownLoad()
	{
		for (int i = 0; i < this.table.transform.childCount; i++)
		{
			BGTemplate component = this.table.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.StartDownLoad();
		}
	}

	public void UpdateSkinStatus()
	{
		for (int i = 0; i < this.table.transform.childCount; i++)
		{
			BGTemplate component = this.table.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.UpdataSkinStatus();
		}
	}

	public void OnClickPreview()
	{
		Solarmax.Singleton<EventSystem>.Get().FireEvent(EventId.OnBgPreviewShow, null);
		this.EnsureDestroy();
	}

	public GameObject template;

	public UITable table;

	public GameObject preview;

	private CollectionModel model;

	private UIScrollView scroll;
}
