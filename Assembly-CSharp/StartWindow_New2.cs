using System;
using Solarmax;
using UnityEngine;

public class StartWindow_New2 : BaseWindow
{
	private void Awake()
	{
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnChestNotify);
		base.RegisterEvent(EventId.OnCoinSync);
		base.RegisterEvent(EventId.OnUpdateNoticy);
		base.RegisterEvent(EventId.OnFriendSearchResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.SetPlayerInfo();
		this.SetJJCInfo();
		this.testLevel.SetActive(true);
		this.SetChestsInfo();
		this.noticeImage.SetActive(false);
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		Solarmax.Singleton<NetSystem>.Instance.helper.LoadClientStorage();
		this.isShowNotice = true;
		this.PlayAnimation("StartWindow_h4");
	}

	private void OnTriggerGuide()
	{
	}

	private void OnNoticeImage()
	{
	}

	public override void OnHide()
	{
		GuideManager.ClearGuideData();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId != EventId.OnCoinSync)
		{
			if (eventId != EventId.OnChestNotify)
			{
				if (eventId != EventId.OnFriendSearchResult)
				{
					if (eventId == EventId.OnUpdateNoticy)
					{
						if (!this.isShowNotice)
						{
							this.OnTriggerGuide();
							this.OnNoticeImage();
						}
					}
				}
				else
				{
					SimplePlayerData simplePlayerData = args[0] as SimplePlayerData;
					bool flag = (bool)args[1];
					int num = (int)args[2];
					int num2 = (int)args[3];
					int num3 = (int)args[4];
					int num4 = (int)args[5];
					int num5 = (int)args[6];
					if (simplePlayerData != null)
					{
						Solarmax.Singleton<UISystem>.Get().HideAllWindow();
						Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendFindWindow");
						Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendSearchResultShow, new object[]
						{
							simplePlayerData,
							flag,
							num,
							num2,
							num3,
							num4,
							num5,
							true
						});
					}
				}
			}
			else
			{
				this.OnHideChestClicked();
				this.SetChestsInfo();
			}
		}
		else
		{
			this.SetCoinInfo();
		}
	}

	private void SetPlayerInfo()
	{
		this.userNameLabel.text = string.Format("Hi, {0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.name);
		if (string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().playerData.icon))
		{
			this.userIconTexture.spriteName = "select_head_head_1";
		}
		else
		{
			this.userIconTexture.spriteName = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
		}
	}

	private void SetJJCInfo()
	{
	}

	public void SetCoinInfo()
	{
	}

	private void PlayAnimation(string state)
	{
		this.aniPlayer.clipName = state;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	public void JoinGameOnClicked()
	{
	}

	public void OnSettingClick()
	{
	}

	public void OnJJCPreviewClick()
	{
	}

	public void OnRaceClick()
	{
	}

	public void OnRewardClick()
	{
	}

	public void OnRankClick()
	{
	}

	public void OnRecordClick()
	{
	}

	public void OnRoomClick()
	{
	}

	public void OnGMClick()
	{
	}

	public void OnCustomPlayerHead()
	{
		int userId = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId;
		if (userId > 0)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendSearch(string.Empty, userId, 0);
		}
	}

	public void OnChest0Clicked()
	{
		Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(602), 1f);
	}

	public void OnBreakThroughMode()
	{
	}

	public void OnChest1Clicked()
	{
		Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(602), 1f);
	}

	public void OnChest2Clicked()
	{
		Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(602), 1f);
	}

	public void OnTestLevelClicked()
	{
	}

	public void OnChestOpenClicked()
	{
		if (this.curSelectIndex == this.curIndexComper)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.OpenBox(this.curSelectIndex, true);
		}
		else if (this.hasUnlockChest)
		{
			if (this.curUnlockcount > this.curUnlockfinish)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.OpenBox(this.curSelectIndex, true);
			}
			else
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartBox(this.curSelectIndex);
			}
		}
		else
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.StartBox(this.curSelectIndex);
		}
	}

	public void OnHideChestClicked()
	{
		this.chestframeObj.SetActive(false);
		GuideManager.ClearGuideData();
	}

	private void SetChestsInfo()
	{
		this.curIndexComper = -1;
		this.curUnlockcount = 0;
		this.curUnlockfinish = 0;
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		this.chests = playerData.chesses;
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			if (this.chests[i] != null && this.chests[i].timeout > 0L)
			{
				flag = true;
			}
			this.frashChest(i, this.chests[i], this.chestObjs[i]);
		}
		this.hasUnlockChest = flag;
		if (!this.hasUnlockChest)
		{
			this.chestTimeObj.SetActive(false);
		}
		int timechestid = playerData.timechestid;
		int battlechestid = playerData.battlechestid;
		ChestConfig data = Solarmax.Singleton<ChestConfigProvider>.Instance.GetData(timechestid);
		if (data != null)
		{
			this.timechesticon.spriteName = data.icon;
		}
		ChestConfig data2 = Solarmax.Singleton<ChestConfigProvider>.Instance.GetData(battlechestid);
		if (data2 != null)
		{
			this.battlechesticon.spriteName = data2.icon;
		}
		if (playerData.timechest > 0L)
		{
			DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
			DateTime dateTime = new DateTime(1970, 1, 1);
			DateTime d = dateTime.AddSeconds((double)playerData.timechest).AddSeconds((double)playerData.timechestcost);
			if (d.CompareTo(serverTime) > 0)
			{
				TimeSpan timeSpan = d - serverTime;
				if (timeSpan.Days > 0)
				{
					this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(603), timeSpan.Days, timeSpan.Hours);
				}
				else if (timeSpan.Hours > 0)
				{
					this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(604), timeSpan.Hours, timeSpan.Minutes);
				}
				else if (timeSpan.Minutes > 0)
				{
					this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(605), timeSpan.Minutes, timeSpan.Seconds);
				}
				else
				{
					this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(606), timeSpan.Seconds);
				}
				this.timechestbg.spriteName = "all_frame_double";
				this.timechestbg.GetComponent<UIButton>().normalSprite = "all_frame_double";
				this.freshchest = true;
			}
			else
			{
				this.timechestlabel.text = LanguageDataProvider.GetValue(607);
				this.timechestbg.spriteName = "pvp_freebox_open_bg_yellow";
				this.timechestbg.GetComponent<UIButton>().normalSprite = "pvp_freebox_open_bg_yellow";
			}
		}
		if (playerData.curbattlechest < playerData.maxbattlechest)
		{
			this.battlechestlabel.text = string.Format("{0}/{1}", playerData.curbattlechest, playerData.maxbattlechest);
			this.battlechestbg.spriteName = "all_frame_double";
			this.battlechestbg.GetComponent<UIButton>().normalSprite = "all_frame_double";
		}
		else
		{
			this.battlechestlabel.text = LanguageDataProvider.GetValue(607);
			this.battlechestbg.spriteName = "pvp_freebox_open_bg_yellow";
			this.battlechestbg.GetComponent<UIButton>().normalSprite = "pvp_freebox_open_bg_yellow";
		}
		this.timechestbg.spriteName = "all_frame_double";
		this.timechestbg.GetComponent<UIButton>().normalSprite = "all_frame_double";
		this.battlechestbg.spriteName = "all_frame_double";
		this.battlechestbg.GetComponent<UIButton>().normalSprite = "all_frame_double";
		this.timechestlabel.text = string.Empty;
		this.battlechestlabel.text = string.Empty;
	}

	private void ReflashTime()
	{
		base.Invoke("ReflashTime", 1f);
		if (this.freshchest)
		{
			PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
			if (playerData.timechest > 0L)
			{
				DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
				DateTime dateTime = new DateTime(1970, 1, 1);
				DateTime d = dateTime.AddSeconds((double)playerData.timechest).AddSeconds((double)playerData.timechestcost);
				if (d.CompareTo(serverTime) > 0)
				{
					TimeSpan timeSpan = d - serverTime;
					if (timeSpan.Days > 0)
					{
						this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(603), timeSpan.Days, timeSpan.Hours);
					}
					else if (timeSpan.Hours > 0)
					{
						this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(604), timeSpan.Hours, timeSpan.Minutes);
					}
					else if (timeSpan.Minutes > 0)
					{
						this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(605), timeSpan.Minutes, timeSpan.Seconds);
					}
					else
					{
						this.timechestlabel.text = string.Format(LanguageDataProvider.GetValue(606), timeSpan.Seconds);
					}
				}
				else
				{
					this.timechestlabel.text = LanguageDataProvider.GetValue(607);
					this.timechestbg.spriteName = "pvp_freebox_open_bg_yellow";
					this.timechestbg.GetComponent<UIButton>().normalSprite = "pvp_freebox_open_bg_yellow";
					this.freshchest = false;
				}
			}
		}
		if (this.curIndex < 0 || this.curIndex > 2)
		{
			return;
		}
		ChessItem chessItem = this.chests[this.curIndex];
		if (chessItem != null && chessItem.timeout > 0L)
		{
			DateTime serverTime2 = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
			if (chessItem.timefinish.CompareTo(serverTime2) > 0)
			{
				TimeSpan timeSpan2 = chessItem.timefinish - serverTime2;
				if (timeSpan2.Days > 0)
				{
					this.chestLastTime.text = string.Format("{0:T}", timeSpan2);
				}
				else if (timeSpan2.Hours > 0)
				{
					this.chestLastTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan2.Hours, timeSpan2.Minutes, timeSpan2.Seconds);
				}
				else if (timeSpan2.Minutes > 0)
				{
					this.chestLastTime.text = string.Format("00:{0:D2}:{1:D2}", timeSpan2.Minutes, timeSpan2.Seconds);
				}
				else
				{
					this.chestLastTime.text = string.Format("00:00:{0:D2}", timeSpan2.Seconds);
				}
			}
			else
			{
				this.curIndex = -1;
				this.SetChestsInfo();
			}
		}
		else
		{
			this.curIndex = -1;
		}
	}

	private void frashChest(int index, ChessItem chest, GameObject chestobj)
	{
		this.chestLabel[index].enabled = false;
		this.chestLock[index].enabled = false;
		this.chestTime[index].enabled = false;
		this.chestFrom[index].enabled = false;
		this.chestOpen[index].enabled = false;
		this.chestNow[index].enabled = false;
		this.chestCost[index].enabled = false;
		this.chestCostSpr[index].enabled = false;
		this.chestFree[index].enabled = false;
		this.chestIcon[index].enabled = false;
		this.chestLabel[index].enabled = true;
		this.chestBg[index].spriteName = "all_frame_double";
		base.transform.Find("Anchor/right/reword1/line").gameObject.SetActive(false);
		base.transform.Find("Anchor/right/reword2/line").gameObject.SetActive(false);
		base.transform.Find("Anchor/right/reword3/line").gameObject.SetActive(false);
	}

	private void ShowChestObject()
	{
	}

	public void TimeBoxClicked()
	{
	}

	public void BattleBoxClicked()
	{
	}

	public void ShowJJC()
	{
	}

	public void OnPlayerFAniamionEnd()
	{
		if (this.isShowNotice)
		{
			this.isShowNotice = false;
			this.OnNoticeImage();
		}
	}

	public UIPlayAnimation aniPlayer;

	public GameObject testLevel;

	public UISprite userIconTexture;

	public UILabel userNameLabel;

	public UILabel scoreLabel;

	public UILabel jewelLabel;

	public UILabel goldLabel;

	public UITexture jjcIcon;

	public UILabel jjcName;

	public UILabel jjcLevel;

	public GameObject noticeImage;

	private string waitToShowWindow;

	private int oldGold;

	private int oldJewel;

	private bool isShowNotice;

	private static float fLastrequestFriendTime;

	public GameObject[] chestObjs;

	public UISprite[] chestBg;

	public UISprite[] chestIcon;

	public GameObject[] chestLine;

	public UILabel[] chestLabel;

	public UILabel[] chestLock;

	public UILabel[] chestTime;

	public UILabel[] chestFrom;

	public UILabel[] chestOpen;

	public UILabel[] chestNow;

	public UILabel[] chestCost;

	public UILabel[] chestFree;

	public UISprite[] chestCostSpr;

	public GameObject chestTimeObj;

	public UILabel chestLastTime;

	public GameObject chestframeObj;

	public UILabel chestNameLabel;

	public UILabel chestJJCLevelLabel;

	public UILabel chestGoldGift;

	public UILabel chestCardGift;

	public UILabel chestSkillGift;

	public GameObject[] chestdowns;

	public UILabel timechestlabel;

	public UILabel battlechestlabel;

	public UISprite timechestbg;

	public UISprite battlechestbg;

	public UISprite timechesticon;

	public UISprite battlechesticon;

	private ChessItem[] chests;

	private bool hasUnlockChest;

	private int curUnlockcount;

	private int curUnlockfinish;

	private int curIndex = -1;

	private int curIndexComper = -1;

	private int curSelectIndex = -1;

	private bool freshchest;
}
