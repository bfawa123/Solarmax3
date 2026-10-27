using System;
using Solarmax;
using UnityEngine;

public class CommonDialogWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnCommonDialog);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
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
		this.yesBtn1.gameObject.SetActive(false);
		this.yesBtn2.gameObject.SetActive(false);
		this.noBtn2.gameObject.SetActive(false);
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
			this.SetInfo(this.type, str);
		}
	}

	private void EnsureButtonLayoutSnapshot()
	{
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
	}

	private void SetInfo(int type, string str)
	{
		this.EnsureButtonLayoutSnapshot();
		this.RestoreButtonLayout();
		this.yesBtn1.gameObject.SetActive(false);
		this.yesBtn2.gameObject.SetActive(false);
		this.noBtn2.gameObject.SetActive(false);
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
		this.tips.text = str;
		base.gameObject.SetActive(true);
		if (type == 6)
		{
			global::Coroutine.DelayDo(0f, new EventDelegate(new EventDelegate.Callback(this.ApplyType6Labels)));
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
		this.OverrideButtonLabelDeep(this.yesBtn1, this.altBtnText);
		this.OverrideButtonLabelDeep(this.yesBtn2, this.yesBtnText);
		this.OverrideButtonLabelDeep(this.noBtn2, this.noBtnText);
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

	private int yesCountDown;

	private bool layoutSnapshot;

	private Vector3 yesBtn1OriginalPos;

	private Vector3 yesBtn2OriginalPos;

	private Vector3 noBtn2OriginalPos;

	private string altBtnText;

	private string yesBtnText;

	private string noBtnText;
}
