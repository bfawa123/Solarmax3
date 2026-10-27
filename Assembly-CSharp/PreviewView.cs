using System;
using System.Collections;
using Solarmax;
using UnityEngine;

public class PreviewView : MonoBehaviour
{
	private void Awake()
	{
		this.scroll = this.grid.transform.parent.gameObject.GetComponent<UIScrollView>();
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
		this.ClearTable();
	}

	private IEnumerator UpdateUI()
	{
		yield return null;
		foreach (SkinConfig config in this.model.bgConfigs)
		{
			GameObject gameObject = this.grid.gameObject.AddChild(this.template);
			gameObject.SetActive(true);
			BGTemplate component = gameObject.GetComponent<BGTemplate>();
			component.EnsureInit(config, true);
		}
		this.grid.Reposition();
		yield break;
	}

	private void ClearTable()
	{
		this.grid.transform.DestroyChildren();
		this.grid.Reposition();
		this.scroll.ResetPosition();
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
		for (int i = 0; i < this.grid.transform.childCount; i++)
		{
			BGTemplate component = this.grid.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.UpdateProgress(nAddVuale);
		}
	}

	public void StartDownLoad()
	{
		for (int i = 0; i < this.grid.transform.childCount; i++)
		{
			BGTemplate component = this.grid.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.StartDownLoad();
		}
	}

	public void UpdateSkinStatus()
	{
		for (int i = 0; i < this.grid.transform.childCount; i++)
		{
			BGTemplate component = this.grid.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.UpdataSkinStatus();
		}
	}

	public void BuySkin()
	{
		for (int i = 0; i < this.grid.transform.childCount; i++)
		{
			BGTemplate component = this.grid.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.BuySkin();
		}
	}

	public void OnBuySkinResponse()
	{
		for (int i = 0; i < this.grid.transform.childCount; i++)
		{
			BGTemplate component = this.grid.transform.GetChild(i).gameObject.GetComponent<BGTemplate>();
			component.OnBuyResponse();
		}
	}

	public GameObject template;

	public UIGrid grid;

	public UIScrollView scroll;

	private CollectionModel model;
}
