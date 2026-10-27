using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class MaitAttachBh : MonoBehaviour
{
	public void SetAttachmentInfo(MailItemList info)
	{
		this.fragMark.SetActive(false);
		if (info.itemType == 1)
		{
			this.attachmentSprite.spriteName = "icon_currency";
		}
		else
		{
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(info.itemId);
			if (data != null)
			{
				this.attachmentSprite.spriteName = data.icon;
				if (data.needCount > 1)
				{
					this.fragMark.SetActive(true);
				}
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
		this.numLabel.text = info.itemCount.ToString();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public UISprite attachmentSprite;

	public GameObject fragMark;

	public UILabel numLabel;
}
