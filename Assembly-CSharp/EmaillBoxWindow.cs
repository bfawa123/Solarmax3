using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class EmaillBoxWindow : BaseWindow
{
	private void Awake()
	{
		this.emaillboxList = new List<SimplePlayerData>();
	}

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

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("EmaillBoxWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendWindow");
	}

	private void RefreshScrollView(List<SimplePlayerData> data)
	{
		this.grid.transform.DestroyChildren();
		for (int i = 0; i < data.Count; i++)
		{
			GameObject gameObject = this.grid.gameObject.AddChild(this.infoTemplate);
			gameObject.name = "infomation_" + data[i].userId;
			gameObject.SetActive(true);
			EmaillBoxWindowCell component = gameObject.GetComponent<EmaillBoxWindowCell>();
			component.SetEamilBoxInfo(data[i]);
		}
		this.grid.Reposition();
		this.scrollView.ResetPosition();
	}

	public GameObject infoTemplate;

	public UIScrollView scrollView;

	public UIGrid grid;

	private List<SimplePlayerData> emaillboxList;
}
