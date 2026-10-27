using System;
using UnityEngine;

public class UnlockView : MonoBehaviour
{
	public void Show()
	{
		this.unlock.alpha = 1f;
		this.unlock.spriteName = "lock_CopyUI_UnopenState";
		base.Invoke("Unlock", 0.8f);
	}

	private void Unlock()
	{
		this.unlock.spriteName = "lock_CopyUI_OpenState";
		this.fadeOut = true;
	}

	private void Update()
	{
		if (this.fadeOut)
		{
			this.delta += Time.deltaTime;
			if (this.delta > 0.05f)
			{
				this.delta = 0f;
				this.unlock.alpha = this.unlock.alpha - 0.05f;
				if (this.unlock.alpha <= 1E-45f)
				{
					this.fadeOut = false;
				}
			}
		}
	}

	private const string LOCK_ICON = "lock_CopyUI_UnopenState";

	private const string UNLOCK_ICON = "lock_CopyUI_OpenState";

	public UISprite unlock;

	private bool fadeOut;

	private float delta;
}
