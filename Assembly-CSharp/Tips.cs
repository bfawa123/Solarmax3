using System;
using System.Collections.Generic;
using UnityEngine;

public class Tips : MonoBehaviour
{
	private void Start()
	{
		Tips.tips = this;
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		this.clipWidth = component.width;
		this.clipHeight = component.height;
		base.Invoke("UpdateDelete", this.TipsClearInterval);
	}

	private void Update()
	{
		this.UpdateData();
	}

	private void UpdateData()
	{
		if (this.tipsShowingCount > this.TipsShowMax)
		{
			this.tipsShowingCount--;
			if (this.tipsItemList.Count > 0)
			{
				TipsItem tipsItem = this.tipsItemList[this.tipsItemList.Count - 1];
				this.tipsItemList.Remove(tipsItem);
				UnityEngine.Object.Destroy(tipsItem.gameObject);
			}
			if (this.tipsDataList.Count > 0)
			{
				this.tipsDataList.RemoveRange(0, 1);
			}
		}
		if (this.tipsDataList.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.tipsDataList.Count; i++)
		{
			TipsData tipsData = this.tipsDataList[i];
			if (tipsData != null && tipsData.IsNew())
			{
				if (!tipsData.IsImportantNotice())
				{
					this.MakeToast(tipsData);
					break;
				}
				bool flag = false;
				for (int j = 0; j < this.tipsDataList.Count; j++)
				{
					if (this.tipsDataList[j].IsImportantNotice() && this.tipsDataList[j].IsShow())
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.MakeToast(tipsData);
					break;
				}
			}
		}
	}

	private void UpdateDelete()
	{
		if (this.tipsItemList.Count > 0)
		{
			TipsItem tipsItem = this.tipsItemList[this.tipsItemList.Count - 1];
			if (tipsItem != null && tipsItem.UnUse())
			{
				this.tipsItemList.Remove(tipsItem);
				UnityEngine.Object.Destroy(tipsItem.gameObject);
			}
		}
		base.Invoke("UpdateDelete", this.TipsClearInterval);
	}

	public void ShowToast(Tips.TipsType type, string message, float seconds)
	{
		TipsData tipsData = null;
		for (int i = 0; i < this.tipsDataList.Count; i++)
		{
			if (this.tipsDataList[i] != null && this.tipsDataList[i].UnUse())
			{
				tipsData = this.tipsDataList[i];
				break;
			}
		}
		if (tipsData == null)
		{
			tipsData = new TipsData();
			this.tipsDataList.Add(tipsData);
		}
		tipsData.Init(type, message, seconds);
	}

	private void MakeToast(TipsData data)
	{
		TipsItem tipsItem = null;
		GameObject gameObject = null;
		for (int i = 0; i < this.tipsItemList.Count; i++)
		{
			if (this.tipsItemList[i].UnUse())
			{
				gameObject = this.tipsItemList[i].gameObject;
				tipsItem = this.tipsItemList[i];
			}
		}
		if (gameObject == null && tipsItem == null)
		{
			Tips.TipsType mType = data.mType;
			if (mType != Tips.TipsType.FlowLeft)
			{
				if (mType != Tips.TipsType.FlowUp)
				{
					gameObject = this.topParrent.AddChild(this.itemTemplate);
				}
				else
				{
					gameObject = this.flowUpParent.AddChild(this.itemTemplate);
				}
			}
			else
			{
				gameObject = this.flowLeftParent.AddChild(this.itemTemplate);
			}
			if (gameObject == null)
			{
				return;
			}
			tipsItem = gameObject.GetComponent<TipsItem>();
			this.tipsItemList.Add(tipsItem);
		}
		Tips.TipsType mType2 = data.mType;
		if (mType2 != Tips.TipsType.FlowLeft)
		{
			if (mType2 != Tips.TipsType.FlowUp)
			{
				gameObject.transform.SetParent(this.topParrent.transform);
			}
			else
			{
				gameObject.transform.SetParent(this.flowUpParent.transform);
			}
		}
		else
		{
			gameObject.transform.SetParent(this.flowLeftParent.transform);
		}
		tipsItem.Show(data, this.clipWidth, this.clipHeight, this);
	}

	public void OnShowUIToast(Tips.TipsType type)
	{
		this.tipsShowingCount++;
		if (type == Tips.TipsType.FlowLeft)
		{
			this.flowLeftBackground.SetActive(true);
		}
	}

	public void OnRecycleUIToast(Tips.TipsType type)
	{
		this.tipsShowingCount--;
		if (type == Tips.TipsType.FlowLeft)
		{
			bool flag = false;
			for (int i = 0; i < this.tipsDataList.Count; i++)
			{
				if (this.tipsDataList[i].IsImportantNotice() && this.tipsDataList[i].IsShow())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.flowLeftBackground.SetActive(false);
			}
		}
	}

	public static void Make(string message)
	{
		Tips.tips.ShowToast(Tips.TipsType.Top, message, 2f);
	}

	public static void Make(string message, float seconds)
	{
		if (seconds < 2f)
		{
			seconds = 2f;
		}
		Tips.tips.ShowToast(Tips.TipsType.Top, message, seconds);
	}

	public static void Make(Tips.TipsType type, string message, float seconds)
	{
		if (seconds < 2f)
		{
			seconds = 2f;
		}
		Tips.tips.ShowToast(Tips.TipsType.Top, message, seconds);
	}

	private List<TipsData> tipsDataList = new List<TipsData>();

	private List<TipsItem> tipsItemList = new List<TipsItem>();

	public GameObject flowLeftParent;

	public GameObject flowLeftBackground;

	public GameObject flowUpParent;

	public GameObject bottomParent;

	public GameObject itemTemplate;

	public GameObject topParrent;

	private int tipsShowingCount;

	public int TipsShowMax = 1;

	public float TipsClearInterval = 20f;

	private float clipWidth;

	private float clipHeight;

	private static Tips tips;

	public enum TipsType
	{
		Default,
		FlowLeft,
		FlowUp,
		Bottom,
		Debug,
		Top
	}
}
