using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ResultPlayerInfo : MonoBehaviour
{
	public void Init(Team team, string image, string name, int score, string destroy, int nMoney, Color color, int rank, bool isSeason)
	{
		this.team = team;
		this.avatar.picUrl = image;
		this.nameLabel.text = name;
		this.changeScore = score;
		this.destroyLabel.text = destroy;
		if (!isSeason)
		{
			this.scoreLabel.text = team.playerData.score.ToString();
		}
		else if (score < 0)
		{
			this.scoreLabel.text = team.playerData.score.ToString() + "[-][FF0000]" + score.ToString() + "[-]";
		}
		else if (score > 0)
		{
			this.scoreLabel.text = team.playerData.score.ToString() + "[-][00FF00]+" + score.ToString() + "[-]";
		}
		else
		{
			this.scoreLabel.text = team.playerData.score.ToString() + "+" + score.ToString();
		}
		if (team.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			this.MEbg.gameObject.SetActive(true);
		}
		else
		{
			this.MEbg.gameObject.SetActive(false);
		}
		if (nMoney > 0)
		{
			this.money.SetActive(true);
			if (team.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
			{
				int nCurAccumulMoney = Solarmax.Singleton<LocalPlayer>.Get().nCurAccumulMoney;
				int nMaxAccumulMoney = Solarmax.Singleton<LocalPlayer>.Get().nMaxAccumulMoney;
				int num = nCurAccumulMoney + nMoney;
				if (nMoney + num < nMaxAccumulMoney)
				{
					this.moneyLabel.text = "+" + nMoney.ToString();
				}
				else
				{
					this.moneyLabel.text = "+" + (nMaxAccumulMoney - nCurAccumulMoney).ToString();
				}
			}
			else
			{
				this.moneyLabel.text = "+" + nMoney.ToString();
			}
		}
		else
		{
			this.money.SetActive(false);
		}
		this.headBg.color = color;
		this.headBg.alpha = 1f;
		if (team.playerData.userId <= 0 || team.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId || Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(team.playerData.userId))
		{
			this.attenBtn.SetActive(false);
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
		{
			this.rankLabel.gameObject.SetActive(false);
		}
		else if (team.resultRank >= 0)
		{
			this.rankLabel.gameObject.SetActive(true);
			string[] array = LanguageDataProvider.GetValue(107).Split(new char[]
			{
				','
			});
			if (rank < array.Length)
			{
				this.rankLabel.text = LanguageDataProvider.Format(108, new object[]
				{
					array[rank]
				});
			}
		}
		else
		{
			this.rankLabel.gameObject.SetActive(false);
		}
	}

	public void FakeInit()
	{
		this.leftGo.SetActive(false);
		this.destroyGo.SetActive(false);
		this.attenBtn.gameObject.SetActive(false);
	}

	public void ChangeScore()
	{
		this.scoreLabel.text = string.Format("{0}[-][FF0000]{1}[-]", this.team.playerData.score.ToString(), this.changeScore / 2);
	}

	public void OnClickAttention()
	{
		if (this.addCd)
		{
			return;
		}
		this.addCd = true;
		global::Coroutine.DelayDo(1f, new EventDelegate(delegate()
		{
			this.addCd = false;
		}));
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(this.team.playerData.userId, true);
	}

	public void OnAddFriendSuccess(bool follow)
	{
		this.attenBtn.gameObject.SetActive(!follow);
	}

	public NetTexture avatar;

	public UILabel nameLabel;

	public UILabel scoreLabel;

	public GameObject money;

	public UILabel moneyLabel;

	public UILabel rankLabel;

	public UILabel destroyLabel;

	public UISprite bg;

	public UISprite MEbg;

	public UISprite headBg;

	public GameObject attenBtn;

	public UISprite attenBtnBg;

	public GameObject leftGo;

	public GameObject destroyGo;

	private Team team;

	private bool addCd;

	private int changeScore;
}
