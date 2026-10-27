using System;
using UnityEngine;

public class TipsItem : MonoBehaviour
{
	private void Awake()
	{
		this.bornPosition = base.gameObject.transform.localPosition;
		TipsItem.labelBornDepth = this.label.depth;
	}

	private void Start()
	{
	}

	private void Update()
	{
		this.UpdateShow();
		if (this.tipsData.IsEnd())
		{
			this.HideAndRecycle();
		}
	}

	public void Show(TipsData data, float cw, float ch, Tips root)
	{
		this.tipsData = data;
		this.clipWidth = cw;
		this.clipHeight = ch;
		this.mRoot = root;
		this.tipsData.OnShow();
		base.gameObject.SetActive(true);
		this.label.width = this.label.minWidth;
		this.label.height = this.label.minHeight;
		this.label.text = this.tipsData.mMessage;
		this.label.MakePixelPerfect();
		this.label.gameObject.SetActive(true);
		this.background.gameObject.SetActive(true);
		this.background.depth = TipsItem.labelBornDepth++;
		this.label.depth = TipsItem.labelBornDepth++;
		base.gameObject.transform.localPosition = this.bornPosition;
		TweenPosition component = base.gameObject.GetComponent<TweenPosition>();
		component.ResetToBeginning();
		TweenAlpha component2 = base.gameObject.GetComponent<TweenAlpha>();
		component2.ResetToBeginning();
		if (this.tipsData.mType == Tips.TipsType.FlowLeft)
		{
			this.background.gameObject.SetActive(false);
			float num = (float)(this.label.width / 2) + this.clipWidth / 2f;
			Vector3 vector = new Vector3(this.bornPosition.x + num, this.bornPosition.y, this.bornPosition.z);
			Vector3 to = new Vector3(this.bornPosition.x - num, this.bornPosition.y, this.bornPosition.z);
			base.gameObject.transform.localPosition = vector;
			component.from = vector;
			component.to = to;
			component.duration = (to.x - vector.x) / (this.clipWidth / 3f);
			component.enabled = true;
			component2.to = 0f;
			component2.duration = 0.6f;
			component2.enabled = false;
		}
		else if (this.tipsData.mType == Tips.TipsType.FlowUp)
		{
			component.from = this.bornPosition;
			component.to = new Vector3(this.bornPosition.x, this.clipHeight / 2f, this.bornPosition.z);
			component.duration = 2.5f;
			component.enabled = true;
			component2.to = 0f;
			component2.duration = 1.5f;
			component2.enabled = true;
		}
		this.mRoot.OnShowUIToast(this.tipsData.mType);
	}

	public void OnEnd()
	{
		this.tipsData.OnEnd();
	}

	public void OnTimeout()
	{
		this.tipsData.OnTimeout();
	}

	private void HideAndRecycle()
	{
		this.mRoot.OnRecycleUIToast(this.tipsData.mType);
		this.tipsData.OnRecycle();
		this.tipsData = null;
		base.gameObject.SetActive(false);
	}

	public bool UnUse()
	{
		return this.tipsData == null;
	}

	public void UpdateShow()
	{
		this.tipsData.mSeconds -= Time.deltaTime;
		if ((this.tipsData.mType == Tips.TipsType.Default || this.tipsData.mType == Tips.TipsType.Bottom || this.tipsData.mType == Tips.TipsType.Top) && this.tipsData.mSeconds <= 0f)
		{
			this.OnTimeout();
		}
	}

	public UILabel label;

	public UISprite background;

	public TipsData tipsData;

	private Vector3 bornPosition;

	private static int labelBornDepth;

	private float clipWidth;

	private float clipHeight;

	private Tips mRoot;
}
