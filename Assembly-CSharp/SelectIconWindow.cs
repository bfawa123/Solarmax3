using System;
using Solarmax;
using UnityEngine;

public class SelectIconWindow : BaseWindow
{
	public void Awake()
	{
		for (int i = 0; i < this.icons.Length; i++)
		{
			GameObject gameObject = this.icons[i];
			UIEventListener component = gameObject.GetComponent<UIEventListener>();
			component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnIconClick));
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < this.icons.Length; i++)
		{
			GameObject gameObject = this.icons[i];
			gameObject.transform.Find("block").GetComponent<UISprite>().color = this.unselectColor;
		}
		string icon = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
		if (!string.IsNullOrEmpty(icon) && !icon.StartsWith("http"))
		{
			int num = icon.LastIndexOf('_');
			string s = icon.Substring(num + 1);
			int index = int.Parse(s) - 1;
			this.Select(index);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	private void Select(int index)
	{
		if (this.selectGo != null)
		{
			this.selectGo.transform.Find("block").GetComponent<UISprite>().color = this.unselectColor;
		}
		this.selectGo = this.icons[index];
		this.selectGo.transform.Find("block").GetComponent<UISprite>().color = this.selectColor1;
		this.selectIndex = index;
	}

	private void OnIconClick(GameObject go)
	{
		int num = go.name.LastIndexOf('_');
		string s = go.name.Substring(num + 1);
		int index = int.Parse(s) - 1;
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("click_down");
		this.Select(index);
	}

	public void OnCloseClick()
	{
		if (this.selectIndex != -1)
		{
			string icon = SelectIconWindow.GetIcon(this.selectIndex);
			if (!Solarmax.Singleton<LocalPlayer>.Get().playerData.icon.Equals(icon))
			{
				Solarmax.Singleton<LocalPlayer>.Get().playerData.icon = icon;
				Solarmax.Singleton<NetSystem>.Instance.helper.ChangeIcon(icon);
			}
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("SelectHeadWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("LobbyWindowView");
	}

	public static string GetIcon(int index)
	{
		return string.Format("select_head_{0}", index + 1);
	}

	public GameObject[] icons;

	public Color unselectColor = new Color(1f, 1f, 1f, 0.6f);

	public Color selectColor1 = new Color(0.196f, 0.929f, 0.525f, 1f);

	private GameObject selectGo;

	private int selectIndex = -1;
}
