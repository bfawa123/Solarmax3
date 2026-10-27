using System;
using Solarmax;
using UnityEngine;

public class FriendRankingCell : MonoBehaviour
{
	public void SetInfoLocal(FriendSimplePlayer data, int rank, UIScrollView view)
	{
		this.friend = data;
		if (this.playerIcon != null && !string.IsNullOrEmpty(data.icon) && !data.icon.StartsWith("http") && !data.icon.EndsWith(".png") && !data.icon.EndsWith(".jpg") && !data.icon.EndsWith(".jpeg"))
		{
			this.playerIcon.spriteName = data.icon;
		}
		this.playerRank.text = rank.ToString();
		this.playerName.text = data.name;
		this.playerScore.text = data.score.ToString();
		this.playerPortrait.Load(data.icon, view, null);
		if (this.playerPlay != null)
		{
			this.playerPlay.onClick = new UIEventListener.VoidDelegate(this.OnPlayClick);
		}
	}

	public void SetInfoLocal(SimplePlayerData data, int rank, UIScrollView view)
	{
		this.userID = data.userId;
		this.playerPortrait.Load(data.icon, view, null);
		if (this.playerIcon != null && !string.IsNullOrEmpty(data.icon) && !data.icon.StartsWith("http") && !data.icon.EndsWith(".png") && !data.icon.EndsWith(".jpg") && !data.icon.EndsWith(".jpeg"))
		{
			this.playerIcon.spriteName = data.icon;
		}
		this.playerName.text = data.name;
		this.playerScore.text = data.score.ToString();
		if (data.online)
		{
			this.playerStatus.gameObject.SetActive(true);
		}
		else
		{
			this.playerStatus.gameObject.SetActive(false);
		}
		if (this.playerPlay != null)
		{
			this.playerPlay.onClick = new UIEventListener.VoidDelegate(this.OnPlayClick);
		}
	}

	public void SetInviteInfo(SimplePlayerData data, int rank, UIScrollView view)
	{
		this.userID = data.userId;
		this.playerPortrait.Load(data.icon, view, null);
		if (this.playerIcon != null && !string.IsNullOrEmpty(data.icon) && !data.icon.StartsWith("http") && !data.icon.EndsWith(".png") && !data.icon.EndsWith(".jpg") && !data.icon.EndsWith(".jpeg"))
		{
			this.playerIcon.spriteName = data.icon;
		}
		this.playerName.text = data.name;
		this.playerScore.text = ((data.level > 0) ? data.level.ToString() : data.score.ToString());
		this.playerName.mKey = -1;
		this.playerScore.mKey = -1;
		string text = string.Empty;
		if (data.onStats == 0)
		{
			text = LanguageDataProvider.GetValue(412);
			this.playerStatus.color = new Color(0.7294118f, 0.7294118f, 0.7294118f, 1f);
		}
		else if (data.onStats == 1)
		{
			text = LanguageDataProvider.GetValue(1146);
			this.playerStatus.color = new Color(1f, 0.3529412f, 0.3529412f, 1f);
		}
		else if (data.onStats == 2)
		{
			text = LanguageDataProvider.GetValue(804);
			this.playerStatus.color = new Color(0.43137255f, 1f, 0.3529412f, 1f);
		}
		if (data.onStats == 2)
		{
			this.playerPlay.gameObject.SetActive(true);
		}
		else
		{
			this.playerPlay.gameObject.SetActive(false);
		}
		this.playerStatus.text = text;
		if (this.playerPlay != null)
		{
			this.playerPlay.onClick = new UIEventListener.VoidDelegate(this.OnInviteClick);
		}
	}

	public void OnPlayClick(GameObject go)
	{
		Solarmax.Singleton<UserSyncSysteam>.Get().GenPveDownloadUrl(this.friend.account, Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel.groupId, this.friend.score.ToString());
	}

	public void OnInviteClick(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.userID <= 0)
		{
			return;
		}
		DateTime? dateTime = this.searchFriendTime;
		if (dateTime != null)
		{
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchFriendTime.Value;
			if (timeSpan.TotalSeconds < 5.0)
			{
				string message = string.Format(LanguageDataProvider.GetValue(1147), 5 - timeSpan.Seconds);
				Tips.Make(message);
				return;
			}
		}
		this.searchFriendTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchInvite(this.userID);
	}

	public UILabel playerRank;

	public UISprite playerIcon;

	public UILabel playerName;

	public UILabel playerScore;

	public UIEventListener playerPlay;

	public UILabel playerStatus;

	public PortraitTemplate playerPortrait;

	private int userID;

	private FriendSimplePlayer friend;

	private DateTime? searchFriendTime;
}
