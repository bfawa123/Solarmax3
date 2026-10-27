using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class JJCPreviewWindow : BaseWindow
{
	public override void OnShow()
	{
		base.OnShow();
		this.posTemplate.SetActive(false);
		this.uiGrid.transform.DestroyChildren();
		List<LadderConfig> allData = Solarmax.Singleton<LadderConfigProvider>.Instance.GetAllData();
		for (int i = 0; i < allData.Count; i++)
		{
			this.AddElement(allData[i]);
		}
		this.uiGrid.Reposition();
		this.scrollView.ResetPosition();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	private void AddElement(LadderConfig config)
	{
		GameObject gameObject = this.posTemplate;
		string[] array = config.itemgather.Split(new char[]
		{
			','
		});
		gameObject = this.uiGrid.gameObject.AddChild(gameObject);
		gameObject.SetActive(true);
		Transform transform = gameObject.transform.Find("public");
		Transform transform2 = gameObject.transform.Find("item");
		UILabel component = transform.Find("name").GetComponent<UILabel>();
		UILabel component2 = transform.Find("label").GetComponent<UILabel>();
		component.text = string.Format(LanguageDataProvider.GetValue(721), config.ladderlevel);
		component2.text = config.laddername;
		for (int i = 0; i < array.Length; i++)
		{
			int id = int.Parse(array[i]);
			ChestConfig data = Solarmax.Singleton<ChestConfigProvider>.Instance.GetData(id);
			GameObject gameObject2 = transform2.Find(string.Format("card{0}", i)).gameObject;
			UISprite component3 = gameObject2.transform.Find("icon").GetComponent<UISprite>();
			UILabel component4 = gameObject2.transform.Find("money").GetComponent<UILabel>();
			UILabel component5 = gameObject2.transform.Find("count").GetComponent<UILabel>();
			if (!string.IsNullOrEmpty(data.icon))
			{
				component3.spriteName = data.icon;
			}
			component4.text = string.Format("{0}-{1}", data.mincoin, data.maxcoin);
			component5.text = data.itemnum.ToString();
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("JJCPreviewWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomSelectWindowNew");
	}

	public void OnChange(float per)
	{
		int num = 0;
		if (per < 0.01f)
		{
			num = 0;
		}
		else if (per < 0.12f)
		{
			num = 1;
		}
		else if (per < 0.23f)
		{
			num = 2;
		}
		else if (per < 0.34f)
		{
			num = 3;
		}
		else if (per < 0.45f)
		{
			num = 4;
		}
		else if (per < 0.56f)
		{
			num = 5;
		}
		else if (per < 0.67f)
		{
			num = 6;
		}
		else if (per < 0.78f)
		{
			num = 7;
		}
		else if (per < 0.89f)
		{
			num = 8;
		}
		else if (per < 1f)
		{
			num = 9;
		}
		if (this.curIndex == 9 && num == 0)
		{
			return;
		}
		this.curIndex = num;
		this.point.transform.localPosition = this.listpoint[this.curIndex].transform.localPosition;
	}

	public UIScrollView scrollView;

	public UIGrid uiGrid;

	public GameObject posTemplate;

	public GameObject[] listpoint;

	public UISprite point;

	private int curIndex;
}
