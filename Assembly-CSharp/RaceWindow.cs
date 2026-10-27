using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class RaceWindow : BaseWindow
{
	private void Awake()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			Transform transform = this.items[i].transform.Find("kuang");
			UIEventListener component = transform.gameObject.GetComponent<UIEventListener>();
			component.onClick = new UIEventListener.VoidDelegate(this.OnItemClick);
		}
		UIEventListener component2 = this.backBtn.GetComponent<UIEventListener>();
		component2.onClick = new UIEventListener.VoidDelegate(this.OnBackClick);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnGetRaceData);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < this.items.Length; i++)
		{
			this.items[i].SetActive(false);
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestRaceData();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	private void OnItemClick(GameObject go)
	{
		if (this.raceList == null)
		{
			return;
		}
		string name = go.transform.parent.name;
		int index = int.Parse(name.Substring(4)) - 1;
		RaceData raceData = this.raceList[index];
		Solarmax.Singleton<UISystem>.Get().ShowWindow("RaceInfoWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRaceWindowSelect, new object[]
		{
			raceData
		});
		Solarmax.Singleton<UISystem>.Get().HideWindow("RaceWindow");
	}

	private void OnBackClick(GameObject go)
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("RaceWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomSelectWindowNew");
	}

	public GameObject[] items;

	public GameObject backBtn;

	private List<RaceData> raceList = new List<RaceData>();

	public GameObject cup;

	public GameObject gold;

	public GameObject diamond;
}
