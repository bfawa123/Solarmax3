using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class CommonDialogWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnCommonDialog);
		this.EnsureFourthButton();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.EnsureFourthButton();
		base.gameObject.SetActive(false);
		Solarmax.Singleton<BattleSystem>.Instance.canOperation = false;
		if (this.closeBtn != null)
		{
			this.closeBtn.onClick.Clear();
			EventDelegate.Add(this.closeBtn.onClick, new EventDelegate(this.OnClose));
		}
		else
		{
			UIButton[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIButton>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != this.yesBtn1 && componentsInChildren[i] != this.yesBtn2 && componentsInChildren[i] != this.noBtn2)
				{
					this.closeBtn = componentsInChildren[i];
					this.closeBtn.onClick.Clear();
					EventDelegate.Add(this.closeBtn.onClick, new EventDelegate(this.OnClose));
					break;
				}
			}
		}
	}

	public override void OnHide()
	{
		this.onYes = null;
		this.onAlt = null;
		this.onNo = null;
		this.onExtra = null;
		this.optionDelegates = null;
		this.optionTexts = null;
		this.yesBtn1.gameObject.SetActive(false);
		this.yesBtn2.gameObject.SetActive(false);
		this.noBtn2.gameObject.SetActive(false);
		if (this.extraBtn != null)
		{
			this.extraBtn.gameObject.SetActive(false);
		}
		this.HideOptionScrollRoot();
		this.ClearDynamicOptionButtons();
		this.RestoreButtonLayout();
		Solarmax.Singleton<BattleSystem>.Instance.canOperation = true;
	}

	private void InvokeYes2CountDown()
	{
		if (this.yesCountDown <= 0)
		{
			this.yesBtnLabel2.text = LanguageDataProvider.GetValue(16);
			this.yesBtn2.isEnabled = true;
			base.CancelInvoke("InvokeYes2CountDown");
			return;
		}
		this.yesBtnLabel2.text = string.Format(LanguageDataProvider.GetValue(17), this.yesCountDown);
		this.yesCountDown--;
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnCommonDialog)
		{
			this.type = (int)args[0];
			string str = (string)args[1];
			if (this.type == 1)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
			}
			else if (this.type == 2)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
				if (args.Length > 3)
				{
					this.onNo = (EventDelegate)args[3];
				}
			}
			else if (this.type == 3)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
				if (args.Length > 3)
				{
					this.onNo = (EventDelegate)args[3];
				}
				if (args.Length > 4)
				{
					this.yesCountDown = (int)args[4];
				}
			}
			else if (this.type == 4)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
			}
			else if (this.type == 5)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
				if (args.Length > 3)
				{
					this.onNo = (EventDelegate)args[3];
				}
			}
			else if (this.type == 6)
			{
				if (args.Length > 2)
				{
					this.onAlt = args[2] as EventDelegate;
				}
				if (args.Length > 3)
				{
					this.onYes = args[3] as EventDelegate;
				}
				if (args.Length > 4)
				{
					this.onNo = args[4] as EventDelegate;
				}
				if (args.Length > 5)
				{
					this.altBtnText = args[5] as string;
				}
				if (args.Length > 6)
				{
					this.yesBtnText = args[6] as string;
				}
				if (args.Length > 7)
				{
					this.noBtnText = args[7] as string;
				}
			}
			else if (this.type == 7)
			{
				if (args.Length > 3 && args[2] is EventDelegate[] && args[3] is string[])
				{
					this.optionDelegates = (EventDelegate[])args[2];
					this.optionTexts = (string[])args[3];
				}
				else
				{
					List<EventDelegate> list = new List<EventDelegate>();
					List<string> list2 = new List<string>();
					if (args.Length > 2 && args[2] is EventDelegate)
					{
						list.Add(args[2] as EventDelegate);
					}
					if (args.Length > 3 && args[3] is EventDelegate)
					{
						list.Add(args[3] as EventDelegate);
					}
					if (args.Length > 4 && args[4] is EventDelegate)
					{
						list.Add(args[4] as EventDelegate);
					}
					if (args.Length > 5 && args[5] is EventDelegate)
					{
						list.Add(args[5] as EventDelegate);
					}
					if (args.Length > 6 && args[6] is string)
					{
						list2.Add(args[6] as string);
					}
					if (args.Length > 7 && args[7] is string)
					{
						list2.Add(args[7] as string);
					}
					if (args.Length > 8 && args[8] is string)
					{
						list2.Add(args[8] as string);
					}
					if (args.Length > 9 && args[9] is string)
					{
						list2.Add(args[9] as string);
					}
					this.optionDelegates = list.ToArray();
					this.optionTexts = list2.ToArray();
				}
			}
			this.SetInfo(this.type, str);
		}
	}

	private void EnsureFourthButton()
	{
		if (this.extraBtn != null || this.noBtn2 == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(this.noBtn2.gameObject) as GameObject;
		if (gameObject == null)
		{
			return;
		}
		gameObject.name = "extraBtn";
		gameObject.transform.SetParent(this.noBtn2.transform.parent, false);
		gameObject.transform.localScale = this.noBtn2.transform.localScale;
		gameObject.transform.localPosition = this.noBtn2.transform.localPosition;
		this.extraBtn = gameObject.GetComponent<UIButton>();
		if (this.extraBtn == null)
		{
			this.extraBtn = gameObject.GetComponentInChildren<UIButton>(true);
		}
		this.extraBtnLabel = gameObject.GetComponentInChildren<UILabel>(true);
		if (this.extraBtn != null)
		{
			this.extraBtn.onClick.Clear();
			EventDelegate.Add(this.extraBtn.onClick, new EventDelegate(this.OnExtraClick));
			this.extraBtn.gameObject.SetActive(false);
		}
	}

	private void EnsureOptionScrollRoot()
	{
		if (this.optionScrollRoot != null || this.yesBtn1 == null)
		{
			return;
		}
		Transform parent = this.yesBtn1.transform.parent;
		if (parent == null)
		{
			return;
		}
		this.optionScrollRoot = new GameObject("type7ScrollRoot");
		this.optionScrollRoot.layer = base.gameObject.layer;
		this.optionScrollRoot.transform.SetParent(parent, false);
		this.optionScrollRoot.transform.localPosition = new Vector3(0f, -225f, 0f);
		this.optionScrollRoot.transform.localScale = Vector3.one;
		this.optionPanel = this.optionScrollRoot.AddComponent<UIPanel>();
		this.optionPanel.depth = 1;
		this.optionPanel.clipping = UIDrawCall.Clipping.SoftClip;
		this.optionPanel.baseClipRegion = new Vector4(0f, 0f, 760f, 120f);
		this.optionPanel.clipSoftness = new Vector2(14f, 0f);
		this.optionScrollView = this.optionScrollRoot.AddComponent<UIScrollView>();
		this.optionScrollView.movement = UIScrollView.Movement.Horizontal;
		this.optionScrollView.restrictWithinPanel = true;
		this.optionScrollView.disableDragIfFits = true;
		this.optionScrollView.dragEffect = UIScrollView.DragEffect.MomentumAndSpring;
		this.optionScrollView.smoothDragStart = true;
		this.optionContentRoot = new GameObject("type7ScrollContent").transform;
		this.optionContentRoot.gameObject.layer = base.gameObject.layer;
		this.optionContentRoot.SetParent(this.optionScrollRoot.transform, false);
		this.optionContentRoot.localPosition = Vector3.zero;
		this.optionContentRoot.localScale = Vector3.one;
		this.optionScrollRoot.SetActive(false);
	}

	private void HideOptionScrollRoot()
	{
		if (this.optionScrollRoot != null)
		{
			this.optionScrollRoot.SetActive(false);
		}
	}

	private void ClearDynamicOptionButtons()
	{
		for (int i = 0; i < this.dynamicOptionButtons.Count; i++)
		{
			if (this.dynamicOptionButtons[i] != null)
			{
				UnityEngine.Object.Destroy(this.dynamicOptionButtons[i]);
			}
		}
		this.dynamicOptionButtons.Clear();
	}

	private void BuildDynamicOptionButtons()
	{
		this.EnsureOptionScrollRoot();
		this.ClearDynamicOptionButtons();
		if (this.optionScrollRoot == null || this.optionContentRoot == null || this.optionTexts == null || this.optionTexts.Length == 0)
		{
			return;
		}
		UIButton uibutton = this.yesBtn1;
		if (uibutton == null)
		{
			UIButton[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIButton>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UIButton uibutton2 = componentsInChildren[i];
				if (uibutton2 != null && uibutton2 != this.yesBtn2 && uibutton2 != this.noBtn2 && uibutton2 != this.extraBtn)
				{
					uibutton = uibutton2;
					break;
				}
			}
		}
		if (uibutton == null)
		{
			return;
		}
		this.optionScrollRoot.SetActive(true);
		GameObject gameObject = uibutton.gameObject;
		UIWidget primaryWidget = CommonDialogWindow.GetPrimaryWidget(uibutton);
		float num = (primaryWidget != null) ? ((float)primaryWidget.width) : 260f;
		float num2 = (primaryWidget != null) ? ((float)primaryWidget.height) : 80f;
		float num3 = num * 1.04f;
		float num4 = num2 * 1.04f;
		float num5 = 28f;
		float num6 = 760f;
		float num7 = (float)this.optionTexts.Length * num3 + (float)Mathf.Max(0, this.optionTexts.Length - 1) * num5;
		float num8 = (num7 <= num6) ? (0f - num7 * 0.5f + num3 * 0.5f) : (0f - num6 * 0.5f + num3 * 0.5f + 20f);
		for (int i = 0; i < this.optionTexts.Length; i++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
			if (gameObject2 == null)
			{
				continue;
			}
			gameObject2.name = "type7Option_" + i;
			gameObject2.layer = base.gameObject.layer;
			gameObject2.transform.SetParent(this.optionContentRoot, false);
			gameObject2.transform.localScale = gameObject.transform.localScale;
			gameObject2.transform.localPosition = new Vector3(num8 + (num3 + num5) * (float)i, 0f, 0f);
			UIWidget primaryWidget2 = CommonDialogWindow.GetPrimaryWidget(gameObject2.GetComponent<UIButton>());
			if (primaryWidget2 != null)
			{
				primaryWidget2.SetDimensions(Mathf.RoundToInt(num3), Mathf.RoundToInt(num4));
				BoxCollider component = primaryWidget2.GetComponent<BoxCollider>();
				if (component != null)
				{
					component.size = new Vector3(num3, num4, component.size.z);
					component.center = Vector3.zero;
				}
			}
			UILabel componentInChildren = gameObject2.GetComponentInChildren<UILabel>(true);
			if (componentInChildren != null)
			{
				componentInChildren.mKey = -1;
				componentInChildren.text = this.optionTexts[i];
			}
			UIButton component2 = gameObject2.GetComponent<UIButton>();
			if (component2 != null)
			{
				component2.onClick.Clear();
			}
			UIDragScrollView uIDragScrollView = gameObject2.GetComponent<UIDragScrollView>();
			if (uIDragScrollView == null)
			{
				uIDragScrollView = gameObject2.AddComponent<UIDragScrollView>();
			}
			uIDragScrollView.scrollView = this.optionScrollView;
			UIEventListener uIEventListener = gameObject2.GetComponent<UIEventListener>();
			if (uIEventListener == null)
			{
				uIEventListener = gameObject2.AddComponent<UIEventListener>();
			}
			int index = i;
			uIEventListener.onClick = delegate(GameObject go)
			{
				this.OnDynamicOptionClick(index);
			};
			this.dynamicOptionButtons.Add(gameObject2);
		}
		if (this.optionScrollView != null)
		{
			this.optionScrollView.ResetPosition();
		}
	}

	private void OnDynamicOptionClick(int index)
	{
		if (this.optionDelegates != null && index >= 0 && index < this.optionDelegates.Length && this.optionDelegates[index] != null)
		{
			this.optionDelegates[index].Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
	}

	private void EnsureButtonLayoutSnapshot()
	{
		this.EnsureFourthButton();
		if (this.layoutSnapshot)
		{
			return;
		}
		if (this.yesBtn1 != null)
		{
			this.yesBtn1OriginalPos = this.yesBtn1.transform.localPosition;
		}
		if (this.yesBtn2 != null)
		{
			this.yesBtn2OriginalPos = this.yesBtn2.transform.localPosition;
		}
		if (this.noBtn2 != null)
		{
			this.noBtn2OriginalPos = this.noBtn2.transform.localPosition;
		}
		if (this.extraBtn != null)
		{
			this.extraBtnOriginalPos = this.extraBtn.transform.localPosition;
		}
		this.layoutSnapshot = true;
	}

	private void RestoreButtonLayout()
	{
		if (!this.layoutSnapshot)
		{
			return;
		}
		if (this.yesBtn1 != null)
		{
			this.yesBtn1.transform.localPosition = this.yesBtn1OriginalPos;
			this.yesBtn1.onClick.Clear();
			EventDelegate.Add(this.yesBtn1.onClick, new EventDelegate(this.OnYesClick));
		}
		if (this.yesBtn2 != null)
		{
			this.yesBtn2.transform.localPosition = this.yesBtn2OriginalPos;
			this.yesBtn2.onClick.Clear();
			EventDelegate.Add(this.yesBtn2.onClick, new EventDelegate(this.OnYesClick));
		}
		if (this.noBtn2 != null)
		{
			this.noBtn2.transform.localPosition = this.noBtn2OriginalPos;
			this.noBtn2.onClick.Clear();
			EventDelegate.Add(this.noBtn2.onClick, new EventDelegate(this.OnNoClick));
		}
		if (this.extraBtn != null)
		{
			this.extraBtn.transform.localPosition = this.extraBtnOriginalPos;
			this.extraBtn.onClick.Clear();
			EventDelegate.Add(this.extraBtn.onClick, new EventDelegate(this.OnExtraClick));
		}
	}

	private void SetInfo(int type, string str)
	{
		this.EnsureButtonLayoutSnapshot();
		this.RestoreButtonLayout();
		this.yesBtn1.gameObject.SetActive(false);
		this.yesBtn2.gameObject.SetActive(false);
		this.noBtn2.gameObject.SetActive(false);
		this.HideOptionScrollRoot();
		this.ClearDynamicOptionButtons();
		if (this.extraBtn != null)
		{
			this.extraBtn.gameObject.SetActive(false);
		}
		if (type == 1)
		{
			this.yesBtn1.gameObject.SetActive(true);
		}
		else if (type == 2)
		{
			this.yesBtn2.gameObject.SetActive(true);
			this.noBtn2.gameObject.SetActive(true);
		}
		else if (type == 3)
		{
			this.yesBtn2.gameObject.SetActive(true);
			this.noBtn2.gameObject.SetActive(true);
			if (this.yesCountDown > 0)
			{
				this.yesBtn2.isEnabled = false;
				base.InvokeRepeating("InvokeYes2CountDown", 0f, 1f);
			}
		}
		else if (type == 4)
		{
			this.yesBtn1.gameObject.SetActive(true);
		}
		else if (type == 5)
		{
			this.yesBtn2.gameObject.SetActive(true);
			this.noBtn2.gameObject.SetActive(true);
			this.yesBtnLabel2.text = LanguageDataProvider.GetValue(2179);
			this.noBtnLabel2.text = LanguageDataProvider.GetValue(2145);
		}
		else if (type == 6)
		{
			this.yesBtn1.gameObject.SetActive(true);
			this.yesBtn2.gameObject.SetActive(true);
			this.noBtn2.gameObject.SetActive(true);
			this.ApplyType6Labels();
			this.yesBtn1.onClick.Clear();
			EventDelegate.Add(this.yesBtn1.onClick, new EventDelegate(this.OnAltClick));
			this.yesBtn2.onClick.Clear();
			EventDelegate.Add(this.yesBtn2.onClick, new EventDelegate(this.OnYesClick));
			this.noBtn2.onClick.Clear();
			EventDelegate.Add(this.noBtn2.onClick, new EventDelegate(this.OnNoClick));
			this.NormalizeThreeButtonSizes();
			this.LayoutThreeButtons();
		}
		else if (this.type == 7)
		{
			this.yesBtn1.gameObject.SetActive(true);
			this.yesBtn2.gameObject.SetActive(true);
			this.noBtn2.gameObject.SetActive(true);
			this.extraBtn.gameObject.SetActive(true);
			this.yesBtn1.onClick.Clear();
			this.yesBtn2.onClick.Clear();
			this.noBtn2.onClick.Clear();
			this.extraBtn.onClick.Clear();
			if (this.optionDelegates != null)
			{
				if (this.optionDelegates.Length > 0 && this.optionDelegates[0] != null)
				{
					EventDelegate.Add(this.yesBtn1.onClick, this.optionDelegates[0]);
				}
				if (this.optionDelegates.Length > 1 && this.optionDelegates[1] != null)
				{
					EventDelegate.Add(this.yesBtn2.onClick, this.optionDelegates[1]);
				}
				if (this.optionDelegates.Length > 2 && this.optionDelegates[2] != null)
				{
					EventDelegate.Add(this.noBtn2.onClick, this.optionDelegates[2]);
				}
				if (this.optionDelegates.Length > 3 && this.optionDelegates[3] != null)
				{
					EventDelegate.Add(this.extraBtn.onClick, this.optionDelegates[3]);
				}
			}
			this.ApplyType7Labels();
			this.NormalizeFourButtonSizes();
			this.LayoutFourButtons();
		}
		this.tips.text = str;
		base.gameObject.SetActive(true);
		if (type == 6)
		{
			global::Coroutine.DelayDo(0f, new EventDelegate(new EventDelegate.Callback(this.ApplyType6Labels)));
		}
		else if (type == 7)
		{
			global::Coroutine.DelayDo(0f, new EventDelegate(new EventDelegate.Callback(this.ApplyType7Labels)));
		}
	}

	private void ApplyType6Labels()
	{
		if (!string.IsNullOrEmpty(this.altBtnText) && this.yesBtnLabel1 != null)
		{
			this.yesBtnLabel1.mKey = -1;
			this.yesBtnLabel1.text = this.altBtnText;
		}
		if (!string.IsNullOrEmpty(this.yesBtnText) && this.yesBtnLabel2 != null)
		{
			this.yesBtnLabel2.mKey = -1;
			this.yesBtnLabel2.text = this.yesBtnText;
		}
		if (!string.IsNullOrEmpty(this.noBtnText) && this.noBtnLabel2 != null)
		{
			this.noBtnLabel2.mKey = -1;
			this.noBtnLabel2.text = this.noBtnText;
		}
		if (!string.IsNullOrEmpty(this.extraBtnText) && this.extraBtnLabel != null)
		{
			this.extraBtnLabel.mKey = -1;
			this.extraBtnLabel.text = this.extraBtnText;
		}
		this.OverrideButtonLabelDeep(this.yesBtn1, this.altBtnText);
		this.OverrideButtonLabelDeep(this.yesBtn2, this.yesBtnText);
		this.OverrideButtonLabelDeep(this.noBtn2, this.noBtnText);
		this.OverrideButtonLabelDeep(this.extraBtn, this.extraBtnText);
	}

	private void ApplyType7Labels()
	{
		if (this.optionTexts == null || this.optionTexts.Length < 4)
		{
			return;
		}
		UILabel[] componentsInChildren = this.yesBtn1.GetComponentsInChildren<UILabel>(true);
		if (componentsInChildren.Length > 0)
		{
			componentsInChildren[0].mKey = -1;
			componentsInChildren[0].text = this.optionTexts[0];
		}
		UILabel[] componentsInChildren2 = this.yesBtn2.GetComponentsInChildren<UILabel>(true);
		if (componentsInChildren2.Length > 0)
		{
			componentsInChildren2[0].mKey = -1;
			componentsInChildren2[0].text = this.optionTexts[1];
		}
		UILabel[] componentsInChildren3 = this.noBtn2.GetComponentsInChildren<UILabel>(true);
		if (componentsInChildren3.Length > 0)
		{
			componentsInChildren3[0].mKey = -1;
			componentsInChildren3[0].text = this.optionTexts[2];
		}
		if (this.extraBtn != null)
		{
			UILabel[] componentsInChildren4 = this.extraBtn.GetComponentsInChildren<UILabel>(true);
			if (componentsInChildren4.Length > 0)
			{
				componentsInChildren4[0].mKey = -1;
				componentsInChildren4[0].text = this.optionTexts[3];
			}
		}
		this.OverrideButtonLabelDeep(this.yesBtn1, this.optionTexts[0]);
		this.OverrideButtonLabelDeep(this.yesBtn2, this.optionTexts[1]);
		this.OverrideButtonLabelDeep(this.noBtn2, this.optionTexts[2]);
		this.OverrideButtonLabelDeep(this.extraBtn, this.optionTexts[3]);
	}

	private void OverrideButtonLabelDeep(UIButton button, string text)
	{
		if (button == null || string.IsNullOrEmpty(text))
		{
			return;
		}
		UILabel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UILabel>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UILabel uilabel = componentsInChildren[i];
			if (uilabel != null)
			{
				UIButton componentInParent = uilabel.GetComponentInParent<UIButton>();
				if (componentInParent == button)
				{
					uilabel.mKey = -1;
					uilabel.text = text;
				}
			}
		}
	}

	private static UIWidget GetPrimaryWidget(UIButton button)
	{
		if (button == null)
		{
			return null;
		}
		UIWidget component = button.GetComponent<UIWidget>();
		if (component != null)
		{
			return component;
		}
		return button.GetComponentInChildren<UIWidget>(true);
	}

	private static void CopyWidgetSize(UIWidget src, UIWidget dst)
	{
		if (src == null || dst == null)
		{
			return;
		}
		dst.SetDimensions(src.width, src.height);
		BoxCollider component = dst.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.size = new Vector3((float)src.width, (float)src.height, component.size.z);
			component.center = Vector3.zero;
		}
	}

	private void NormalizeThreeButtonSizes()
	{
		UIWidget primaryWidget = CommonDialogWindow.GetPrimaryWidget(this.yesBtn1);
		UIWidget primaryWidget2 = CommonDialogWindow.GetPrimaryWidget(this.yesBtn2);
		UIWidget primaryWidget3 = CommonDialogWindow.GetPrimaryWidget(this.noBtn2);
		CommonDialogWindow.CopyWidgetSize(primaryWidget, primaryWidget2);
		CommonDialogWindow.CopyWidgetSize(primaryWidget, primaryWidget3);
	}

	private void NormalizeFourButtonSizes()
	{
		UIWidget primaryWidget = CommonDialogWindow.GetPrimaryWidget(this.yesBtn1);
		UIWidget primaryWidget2 = CommonDialogWindow.GetPrimaryWidget(this.yesBtn2);
		UIWidget primaryWidget3 = CommonDialogWindow.GetPrimaryWidget(this.noBtn2);
		UIWidget primaryWidget4 = CommonDialogWindow.GetPrimaryWidget(this.extraBtn);
		if (primaryWidget != null)
		{
			int num = primaryWidget.width;
			int num2 = Mathf.RoundToInt((float)primaryWidget.height * 1.04f);
			primaryWidget.SetDimensions(num, num2);
			BoxCollider component = primaryWidget.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.size = new Vector3((float)num, (float)num2, component.size.z);
				component.center = Vector3.zero;
			}
		}
		CommonDialogWindow.CopyWidgetSize(primaryWidget, primaryWidget2);
		CommonDialogWindow.CopyWidgetSize(primaryWidget, primaryWidget3);
		CommonDialogWindow.CopyWidgetSize(primaryWidget, primaryWidget4);
	}

	private void LayoutThreeButtons()
	{
		if (this.yesBtn1 == null || this.yesBtn2 == null || this.noBtn2 == null)
		{
			return;
		}
		Transform transform = this.yesBtn2.transform.parent;
		if (transform == null)
		{
			return;
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform, this.yesBtn2.transform, true, true);
		float num = bounds.size.x;
		if (num <= 1f)
		{
			num = 260f;
		}
		float num2 = num * 0.12f;
		Vector3 localPosition = this.yesBtn2.transform.localPosition;
		Vector3 localPosition2 = this.yesBtn1.transform.localPosition;
		Vector3 localPosition3 = this.noBtn2.transform.localPosition;
		localPosition2.x = -num - num2;
		localPosition.x = 0f;
		localPosition3.x = num + num2;
		localPosition2.y = localPosition.y;
		localPosition3.y = localPosition.y;
		this.yesBtn1.transform.localPosition = localPosition2;
		this.yesBtn2.transform.localPosition = localPosition;
		this.noBtn2.transform.localPosition = localPosition3;
	}

	private void LayoutFourButtons()
	{
		if (this.yesBtn1 == null || this.yesBtn2 == null || this.noBtn2 == null || this.extraBtn == null)
		{
			return;
		}
		Transform transform = this.yesBtn2.transform.parent;
		if (transform == null)
		{
			return;
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform, this.yesBtn2.transform, true, true);
		float num = bounds.size.x;
		float num2 = bounds.size.y;
		if (num <= 1f)
		{
			num = 260f;
		}
		if (num2 <= 1f)
		{
			num2 = 90f;
		}
		float num3 = num2 * 0.24f;
		float num4 = num2 + num3;
		Vector3 localPosition = this.yesBtn2.transform.localPosition;
		float num5 = localPosition.z;
		float num6 = localPosition.y;
		float num7 = (float)CommonDialogWindow.GetPrimaryWidget(this.yesBtn1).width + 10;
		this.yesBtn1.transform.localPosition = new Vector3(-num7 * 1.5f, num6, num5);
		this.yesBtn2.transform.localPosition = new Vector3(-num7 * 0.5f, num6, num5);
		this.noBtn2.transform.localPosition = new Vector3(num7 * 0.5f, num6, num5);
		this.extraBtn.transform.localPosition = new Vector3(num7 * 1.5f, num6, num5);
	}

	public void OnYesClick()
	{
		if (this.onYes != null)
		{
			this.onYes.Execute();
		}
		if (this.type != 4 && this.type != 5)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
		}
	}

	public void OnNoClick()
	{
		if (this.onNo != null)
		{
			this.onNo.Execute();
		}
		if (this.type != 5)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
	}

	public void OnAltClick()
	{
		if (this.onAlt != null)
		{
			this.onAlt.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
	}

	public void OnExtraClick()
	{
		if (this.onExtra != null)
		{
			this.onExtra.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
	}

	public UIButton closeBtn;

	public UIButton yesBtn1;

	public UILabel yesBtnLabel1;

	public UIButton yesBtn2;

	public UILabel yesBtnLabel2;

	public UIButton noBtn2;

	public UILabel noBtnLabel2;

	public UILabel tips;

	private int type;

	private EventDelegate onYes;

	private EventDelegate onAlt;

	private EventDelegate onNo;

	private EventDelegate onExtra;

	private int yesCountDown;

	private bool layoutSnapshot;

	private Vector3 yesBtn1OriginalPos;

	private Vector3 yesBtn2OriginalPos;

	private Vector3 noBtn2OriginalPos;

	private Vector3 extraBtnOriginalPos;

	private string altBtnText;

	private string yesBtnText;

	private string noBtnText;

	private string extraBtnText;

	private EventDelegate[] optionDelegates;

	private string[] optionTexts;

	private GameObject optionScrollRoot;

	private UIPanel optionPanel;

	private UIScrollView optionScrollView;

	private Transform optionContentRoot;

	private List<GameObject> dynamicOptionButtons = new List<GameObject>();

	private UIButton extraBtn;

	private UILabel extraBtnLabel;
}
