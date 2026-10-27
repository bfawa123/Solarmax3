using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class CooperationSelectWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		this.CreateBackground();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.CreateButtons();
	}

	public override void OnHide()
	{
		base.OnHide();
		this.ClearButtons();
	}

	private void CreateBackground()
	{
		if (this.background != null)
		{
			return;
		}
		this.background = new GameObject("Background");
		this.background.transform.SetParent(base.gameObject.transform, false);
		UIPanel panel = this.background.AddComponent<UIPanel>();
		panel.depth = 0;
		UIWidget widget = this.background.AddComponent<UIWidget>();
		widget.width = 800;
		widget.height = 600;
		widget.pivot = UIWidget.Pivot.Center;
		this.background.transform.localPosition = Vector3.zero;
		UISprite bgSprite = this.background.AddComponent<UISprite>();
		bgSprite.type = UIBasicSprite.Type.Sliced;
		bgSprite.width = 800;
		bgSprite.height = 600;
		bgSprite.depth = 0;
		bgSprite.color = new Color(0f, 0f, 0f, 0.7f);
		BoxCollider box = this.background.AddComponent<BoxCollider>();
		box.size = new Vector3(800f, 600f, 0f);
		UIEventListener uieventListener = this.background.AddComponent<UIEventListener>();
		uieventListener.onClick = delegate(GameObject go)
		{
			this.OnCloseClick();
		};
	}

	private void CreateButtons()
	{
		if (this.buttonRoot == null)
		{
			this.buttonRoot = new GameObject("ButtonRoot");
			this.buttonRoot.transform.SetParent(base.gameObject.transform, false);
			UIWidget widget = this.buttonRoot.AddComponent<UIWidget>();
			widget.width = 600;
			widget.height = 200;
			widget.pivot = UIWidget.Pivot.Center;
			this.buttonRoot.transform.localPosition = Vector3.zero;
			UIPanel panel = this.buttonRoot.AddComponent<UIPanel>();
			panel.depth = 1;
		}
		this.ClearButtons();
		string[] array = new string[]
		{
			LanguageDataProvider.GetValue(7019),
			LanguageDataProvider.GetValue(7020),
			LanguageDataProvider.GetValue(7021),
			LanguageDataProvider.GetValue(7023)
		};
		Action<int>[] array2 = new Action<int>[]
		{
			delegate(int a)
			{
				this.OnSelectCooperation(2);
			},
			delegate(int a)
			{
				this.OnSelectCooperation(3);
			},
			delegate(int a)
			{
				this.OnSelectCooperation(4);
			},
			delegate(int a)
			{
				this.OnSelectCooperation(5);
			}
		};
		for (int i = 0; i < 4; i++)
		{
			GameObject gameObject = this.CreateButton(array[i], i);
			UIEventListener uieventListener = gameObject.GetComponent<UIEventListener>();
			if (uieventListener == null)
			{
				uieventListener = gameObject.AddComponent<UIEventListener>();
			}
			int index = i;
			uieventListener.onClick = delegate(GameObject go)
			{
				array2[index](index);
			};
		}
	}

	private GameObject CreateButton(string text, int index)
	{
		GameObject gameObject = new GameObject("CooperationBtn_" + index, new Type[]
		{
			typeof(UIWidget)
		});
		UIWidget component = gameObject.GetComponent<UIWidget>();
		component.width = 260;
		component.height = 80;
		component.anchor = this.buttonRoot.GetComponent<UIWidget>();
		component.pivot = UIWidget.Pivot.Center;
		gameObject.transform.parent = this.buttonRoot.transform;
		float num = index % 2 == 0 ? -140f : 140f;
		float num2 = index < 2 ? 50f : -50f;
		gameObject.transform.localPosition = new Vector3(num, num2, 0f);
		gameObject.transform.localScale = Vector3.one;
		UIButton uibutton = gameObject.AddComponent<UIButton>();
		uibutton.tweenTarget = gameObject;
		uibutton.hover = new Color(0.78431374f, 0.78431374f, 0.78431374f);
		uibutton.pressed = new Color(0.7058824f, 0.7058824f, 0.7058824f);
		uibutton.disabledColor = new Color(0.5f, 0.5f, 0.5f);
		UISprite uisprite = gameObject.AddComponent<UISprite>();
		uisprite.type = UIBasicSprite.Type.Sliced;
		uisprite.width = 260;
		uisprite.height = 80;
		uisprite.depth = 5;
		gameObject.AddComponent<BoxCollider>().size = new Vector3(260f, 80f, 0f);
		GameObject gameObject2 = new GameObject("Label", new Type[]
		{
			typeof(UILabel)
		});
		UILabel uilabel = gameObject2.GetComponent<UILabel>();
		uilabel.text = text;
		uilabel.fontSize = 28;
		uilabel.alignment = NGUIText.Alignment.Center;
		uilabel.color = new Color(1f, 1f, 1f);
		uilabel.width = 260;
		uilabel.height = 80;
		uilabel.pivot = UIWidget.Pivot.Center;
		uilabel.depth = 6;
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localScale = Vector3.one;
		return gameObject;
	}

	private void ClearButtons()
	{
		if (this.buttonRoot == null)
		{
			return;
		}
		for (int i = this.buttonRoot.transform.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(this.buttonRoot.transform.GetChild(i).gameObject);
		}
	}

	private void OnSelectCooperation(int playerNum)
	{
		this.OpenCooperationInternal(playerNum);
		Solarmax.Singleton<UISystem>.Get().HideWindow("CooperationSelectWindow");
	}

	private void OpenCooperationInternal(int playerNum)
	{
		Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(playerNum);
		string backWindow = Solarmax.Singleton<LocalPlayer>.Get().HomeWindow;
		if (string.IsNullOrEmpty(backWindow))
		{
			backWindow = "StartWindowPVP";
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationRoomWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBackWindowName, new object[]
		{
			backWindow
		});
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CooperationSelectWindow");
	}

	public GameObject buttonRoot;

	public GameObject background;

	public UILabel titleLabel;

	public GameObject closeBtn;
}