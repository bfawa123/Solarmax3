using System;
using UnityEngine;

public class ShipFadeManager : Solarmax.Singleton<ShipFadeManager>
{
	public void SetFadeType(ShipFadeManager.FADETYPE eType, float duration)
	{
		this.mShipsFadeType = eType;
		if (eType == ShipFadeManager.FADETYPE.IN)
		{
			this.mfadeProcess = 0f;
		}
		if (eType == ShipFadeManager.FADETYPE.OUT)
		{
			this.mfadeProcess = 1f;
		}
		this.mfDuration = duration;
	}

	public void BackShipMaterial(GameObject ship, SpriteRenderer render)
	{
		this.mShipGameObj = ship;
		if (this.mShipGameObj != null)
		{
			Material material = render.material;
			render.material = material;
			GameObject gameObject = ship.transform.Find("scale").gameObject;
			if (gameObject != null)
			{
				SpriteRenderer component = gameObject.transform.Find("trail").GetComponent<SpriteRenderer>();
				if (component != null)
				{
					component.material = material;
				}
				SpriteRenderer component2 = gameObject.transform.Find("ship_defense").GetComponent<SpriteRenderer>();
				if (component2 != null)
				{
					component2.material = material;
				}
				SpriteRenderer component3 = component.transform.Find("ship_accelerate").GetComponent<SpriteRenderer>();
				if (component3 != null)
				{
					component3.material = material;
				}
				SpriteRenderer component4 = component.transform.Find("ship_decelera").GetComponent<SpriteRenderer>();
				if (component4 != null)
				{
					component4.material = material;
				}
			}
			this.fsm_mat = render.sharedMaterial;
		}
	}

	public void SetShipAlpha(float fAlpha)
	{
		if (this.fsm_mat != null)
		{
			this.fsm_mat.SetFloat("_alphavalue", fAlpha);
		}
	}

	public void UpdateFadeInOut(float delta)
	{
		if (this.mShipsFadeType == ShipFadeManager.FADETYPE.NULL)
		{
			return;
		}
		float num = 1f / this.mfDuration;
		if (this.mShipsFadeType == ShipFadeManager.FADETYPE.IN)
		{
			this.mfadeProcess += num * delta;
			if (this.mfadeProcess > 1f)
			{
				this.mfadeProcess = 1f;
				this.mShipsFadeType = ShipFadeManager.FADETYPE.NULL;
				return;
			}
		}
		else if (this.mShipsFadeType == ShipFadeManager.FADETYPE.OUT)
		{
			this.mfadeProcess -= num * delta;
			if (this.mfadeProcess < 0f)
			{
				this.mfadeProcess = 0.01f;
				this.mShipsFadeType = ShipFadeManager.FADETYPE.NULL;
			}
		}
		if (this.fsm_mat != null)
		{
			this.fsm_mat.SetFloat("_alphavalue", this.mfadeProcess);
		}
	}

	public Material fsm_mat;

	private Material mShipmat;

	public GameObject mShipGameObj;

	private float mfadeProcess = 1f;

	private ShipFadeManager.FADETYPE mShipsFadeType;

	private float mfDuration = 1f;

	public enum FADETYPE
	{
		NULL,
		IN,
		OUT
	}
}
