using System;
using Solarmax;
using UnityEngine;

public class FriendWindowCell : MonoBehaviour
{
	public void SetRecommendInfo(SimplePlayerData data, bool cared)
	{
		this.cellType = 1;
		this.playerData = new SimplePlayerData(data);
		this.icon.picUrl = this.playerData.icon;
		this.nameLabel.text = this.playerData.name;
		if (data.onStats == 0)
		{
			this.onlineLabel.text = LanguageDataProvider.GetValue(412);
			this.onlineLabel.color = new Color(0.7294118f, 0.7294118f, 0.7294118f, 1f);
		}
		else if (data.onStats == 1)
		{
			this.onlineLabel.text = LanguageDataProvider.GetValue(1146);
			this.onlineLabel.color = new Color(1f, 0.3529412f, 0.3529412f, 1f);
		}
		else if (data.onStats == 2)
		{
			this.onlineLabel.text = LanguageDataProvider.GetValue(804);
			this.onlineLabel.color = new Color(0.43137255f, 1f, 0.3529412f, 1f);
		}
		this.scoreLabel.text = this.playerData.score.ToString();
		if (cared)
		{
			this.careBtn.isEnabled = false;
			this.careBtnLabel.text = LanguageDataProvider.GetValue(807);
		}
		else
		{
			this.careBtn.isEnabled = true;
			this.careBtnLabel.text = LanguageDataProvider.GetValue(806);
		}
	}

	public void SetFollowerInfo(SimplePlayerData data, UIScrollView view)
	{
		this.cellType = 2;
		this.playerData = new SimplePlayerData(data);
		this.icon.scroll = view;
		this.icon.picUrl = this.playerData.icon;
		this.nameLabel.text = this.playerData.name;
		this.scoreLabel.text = this.playerData.score.ToString();
		if (Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(this.playerData.userId))
		{
			this.addFriend.SetActive(false);
			this.deleteFriend.SetActive(true);
			this.onlineLabel.gameObject.SetActive(true);
			if (data.onStats == 0)
			{
				this.onlineLabel.text = LanguageDataProvider.GetValue(412);
				this.onlineLabel.color = new Color(0.7294118f, 0.7294118f, 0.7294118f, 1f);
			}
			else if (data.onStats == 1)
			{
				this.onlineLabel.text = LanguageDataProvider.GetValue(1146);
				this.onlineLabel.color = new Color(1f, 0.3529412f, 0.3529412f, 1f);
			}
			else if (data.onStats == 2)
			{
				this.onlineLabel.text = LanguageDataProvider.GetValue(804);
				this.onlineLabel.color = new Color(0.43137255f, 1f, 0.3529412f, 1f);
			}
		}
		else if (this.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			this.addFriend.SetActive(false);
			this.deleteFriend.SetActive(false);
			this.onlineLabel.gameObject.SetActive(true);
			this.onlineLabel.text = LanguageDataProvider.GetValue(804);
			this.onlineLabel.color = new Color(0.43137255f, 1f, 0.3529412f, 1f);
		}
		else
		{
			this.addFriend.SetActive(true);
			this.deleteFriend.SetActive(false);
			this.onlineLabel.gameObject.SetActive(false);
		}
	}

	public void OnShowPlayerInfoClick()
	{
		if (this.playerData != null)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendSearch(string.Empty, this.playerData.userId, 1);
		}
	}

	public void OnDeleteClick()
	{
		if (this.playerData != null)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
			Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
			{
				2,
				LanguageDataProvider.GetValue(1134),
				new EventDelegate(new EventDelegate.Callback(this.OnDeleteConfirm))
			});
		}
	}

	public void OnDeleteConfirm()
	{
		if (this.playerData != null)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.playerData.userId, false);
		}
	}

	public void OnInfoClick(GameObject go)
	{
	}

	public void OnCareClick()
	{
		if (this.playerData != null)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.playerData.userId, true);
		}
	}

	public void OnCareResult(bool cared)
	{
		if (this.cellType == 1)
		{
			if (cared)
			{
				this.careBtn.isEnabled = false;
				this.careBtnLabel.text = LanguageDataProvider.GetValue(808);
			}
		}
		else if (this.cellType == 2)
		{
			if (cared)
			{
				this.careBtn.isEnabled = false;
				this.careBtnLabel.text = LanguageDataProvider.GetValue(808);
			}
		}
		else if (this.cellType == 3 && !cared)
		{
			this.careBtn.isEnabled = false;
			this.careBtnLabel.text = LanguageDataProvider.GetValue(811);
		}
	}

	public NetTexture icon;

	public UILabel nameLabel;

	public UISprite onlineBg;

	public UILabel onlineLabel;

	public UILabel scoreLabel;

	public UIButton careBtn;

	public UILabel careBtnLabel;

	public PortraitTemplate portrait;

	public GameObject addFriend;

	public GameObject deleteFriend;

	public SimplePlayerData playerData;

	private int cellType;
}
