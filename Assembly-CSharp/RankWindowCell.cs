using System;
using Solarmax;
using UnityEngine;

public class RankWindowCell : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetCell(int rankid, int userId, string rankname, int rankscore, string sprite, int type, UIScrollView view)
	{
		this.userId = userId;
		this.id.text = rankid.ToString();
		this.nameLabel.text = rankname;
		this.score.text = rankscore.ToString();
		if (rankid <= 3)
		{
			this.id.color = this.top3Color;
			this.idBg.gameObject.SetActive(true);
			this.nameLabel.color = this.top3Color;
			this.score.color = this.top3Color;
			if (rankid == 1)
			{
				this.idBg.color = new Color(1f, 0.80784315f, 0f, 1f);
			}
			else if (rankid == 2)
			{
				this.idBg.color = new Color(0.93333334f, 0.96862745f, 0.99215686f, 1f);
			}
			else if (rankid == 3)
			{
				this.idBg.color = new Color(1f, 0.49411765f, 0.26666668f, 1f);
			}
		}
		else
		{
			this.id.color = this.otherColor;
			this.idBg.gameObject.SetActive(false);
			this.nameLabel.color = this.otherColor;
			this.score.color = this.otherColor;
		}
		if (!string.IsNullOrEmpty(sprite))
		{
			string text = sprite;
			if (!text.EndsWith(".png"))
			{
				text += ".png";
			}
			this.icon.scroll = view;
			this.icon.picUrl = text;
		}
		this.line.gameObject.SetActive(true);
		if (rankid % 2 == 1)
		{
		}
		this.addFriendGo.SetActive(!Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(userId) && Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != userId);
		foreach (GameObject gameObject in this.scorePic)
		{
			gameObject.SetActive(false);
		}
		this.scorePic[type].SetActive(true);
	}

	public void OnAddFriendClicked()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.userId, true);
	}

	public void OnAddFriendSuccess()
	{
		this.addFriendGo.SetActive(!Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(this.userId) && Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != this.userId);
	}

	public UISprite bg;

	public UILabel id;

	public UISprite idBg;

	public NetTexture icon;

	public UILabel nameLabel;

	public UILabel score;

	public UISprite line;

	public GameObject[] scorePic;

	public GameObject addFriendGo;

	private int userId;

	private Color top3Color = new Color(1f, 1f, 1f, 1f);

	private Color otherColor = new Color(1f, 1f, 1f, 0.5f);

	private string[] SCORE_ICON = new string[]
	{
		"icon_jiangbei",
		"icon_CopyUI_CopyOpenStart1",
		"button_challenge"
	};
}
