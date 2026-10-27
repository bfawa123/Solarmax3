using System;
using Solarmax;
using UnityEngine;

public class changeSpriteWidth : MonoBehaviour
{
	private void Start()
	{
		int num = 0;
		foreach (Node node in Solarmax.Singleton<BattleSystem>.Get().sceneManager.nodeManager.GetUsefulNodeList())
		{
			if (num != 0)
			{
				if (num == 1)
				{
					this.endPos = Camera.main.WorldToScreenPoint(node.GetPosition());
					this.endPos = UICamera.currentCamera.ScreenToWorldPoint(this.endPos);
					this.endPos = base.transform.parent.InverseTransformPoint(this.endPos);
				}
			}
			else
			{
				this.startPos = Camera.main.WorldToScreenPoint(node.GetPosition());
				this.startPos = UICamera.currentCamera.ScreenToWorldPoint(this.startPos);
				this.startPos = base.transform.parent.InverseTransformPoint(this.startPos);
			}
			num++;
		}
		this.limit = (int)(Vector3.Distance(this.startPos, this.endPos) * 2f);
		Vector3 to = this.endPos - this.startPos;
		base.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, -Vector3.Angle(new Vector3(-1f, 0f, 0f), to));
		this.speed = (int)((float)this.limit * 1f * Time.fixedDeltaTime);
		this.Btn.transform.localPosition = this.startPos;
		base.gameObject.transform.localPosition = this.startPos;
		base.gameObject.GetComponent<UISprite>().width = 0;
	}

	private void FixedUpdate()
	{
		if (this.playTime >= 0.8333333f && this.playTime <= 1.8333334f)
		{
			if (base.gameObject.GetComponent<UISprite>().width <= this.limit)
			{
				base.gameObject.GetComponent<UISprite>().width += this.speed;
				this.Btn.transform.localPosition = Vector3.Lerp(this.startPos, this.endPos, (this.playTime * 60f - 50f) / 60f);
			}
		}
		else if (this.playTime >= 1.9f && this.playTime <= 2.15f && !this.hideLine)
		{
			base.gameObject.GetComponent<UISprite>().width = 0;
			this.hideLine = true;
		}
		if (this.playTime <= 0.48333332f)
		{
			this.btnSprite.color = new Color(255f, 255f, 255f, 0.465f * this.playTime * 60f / 29f);
		}
		else if (this.playTime >= 1.8333334f)
		{
			this.btnSprite.color = new Color(255f, 255f, 255f, 0.465f - 0.465f * (this.playTime - 1.8333334f) * 60f / 19f);
		}
		if (this.playTime >= 0.48333332f && this.playTime <= 0.8333333f)
		{
			this.ringSprite.color = new Color(255f, 255f, 255f, 0.746f * (this.playTime - 0.48333332f) * 60f / 21f);
		}
		else if (this.playTime >= 1.8333334f)
		{
			this.ringSprite.color = new Color(255f, 255f, 255f, 0.746f - 0.746f * (this.playTime - 1.8333334f) * 60f / 19f);
		}
		if (this.playTime >= 0.48333332f && this.playTime <= 0.8333333f)
		{
			this.Btn.transform.localScale = new Vector3(1.3f - 0.3f * (this.playTime - 0.48333332f) * 60f / 21f, 1.3f - 0.3f * (this.playTime - 0.48333332f) * 60f / 21f, 1.3f - 0.3f * (this.playTime - 0.48333332f) * 60f / 21f);
		}
		if (this.playTime > 2.15f)
		{
			this.playTime = 0f;
			this.hideLine = false;
			base.gameObject.GetComponent<UISprite>().width = 0;
			this.btnSprite.color = new Color(255f, 255f, 255f, 0f);
			this.ringSprite.color = new Color(255f, 255f, 255f, 0f);
			this.Btn.transform.localPosition = this.startPos;
			base.gameObject.transform.localPosition = this.startPos;
			this.Btn.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
		}
		else
		{
			this.playTime += Time.deltaTime;
		}
	}

	private int speed;

	private Vector3 btnSpeed = Vector3.zero;

	public GameObject Btn;

	public UISprite btnSprite;

	public UISprite ringSprite;

	public bool hideLine;

	private float playTime;

	private int limit = 1750;

	private Vector3 startPos = Vector3.zero;

	private Vector3 endPos = Vector3.zero;
}
