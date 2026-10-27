using System;
using Solarmax;
using UnityEngine;

public class SeasonRewardTemplate : MonoBehaviour
{
	public void UpdateUI(string id)
	{
		this.rewardId = id;
		if (!Solarmax.Singleton<SeasonRewardModel>.Get().rewardStatus.ContainsKey(this.rewardId))
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("赛季奖励错误id:" + this.rewardId, new object[0]);
			return;
		}
		SeasonRewardModel seasonRewardModel = Solarmax.Singleton<SeasonRewardModel>.Get();
		int seasonId = Solarmax.Singleton<SeasonRewardModel>.Get().seasonId;
		SeasonRewardConfig seasonRewardConfig = Solarmax.Singleton<SeasonRewardProvider>.Get().dataList[seasonId.ToString()];
		this.desc.text = string.Format(seasonRewardConfig.GetDesc(this.rewardId), seasonRewardConfig.ladderScore[this.rewardId]);
		this.score.text = string.Format("{0}/{1}", seasonRewardModel.seasonMaxScore, seasonRewardConfig.ladderScore[this.rewardId]);
		bool flag = seasonRewardModel.rewardStatus[this.rewardId];
		this.check.SetActive(flag);
		if (!flag)
		{
			if (seasonRewardModel.seasonMaxScore >= seasonRewardConfig.ladderScore[this.rewardId])
			{
				this.claimBtn.enabled = true;
				this.claimBtn.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 1f);
				this.claimBtn.ResetDefaultColor(new Color(1f, 1f, 1f, 1f));
				this.claimLabel.text = LanguageDataProvider.GetValue(2077);
			}
			else
			{
				this.claimBtn.enabled = false;
				this.claimBtn.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.2f);
				this.claimLabel.text = LanguageDataProvider.GetValue(2080);
			}
		}
		else
		{
			this.claimBtn.enabled = false;
			this.claimBtn.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.4f);
			this.claimBtn.ResetDefaultColor(new Color(1f, 1f, 1f, 0.4f));
			this.claimLabel.text = LanguageDataProvider.GetValue(2237);
		}
		if (seasonRewardConfig.rewardType[this.rewardId] == 1)
		{
			this.moneyIcon.SetActive(true);
			this.portraitIcon.SetActive(false);
			this.moneyLabel.text = seasonRewardConfig.misc[this.rewardId].ToString();
		}
		else if (seasonRewardConfig.rewardType[this.rewardId] == 2)
		{
			this.moneyIcon.SetActive(false);
			this.portraitIcon.SetActive(true);
			int id2 = 0;
			if (int.TryParse(seasonRewardConfig.misc[this.rewardId].ToString(), out id2))
			{
				SkinConfig avatarData = Solarmax.Singleton<SkinConfigProvider>.Get().GetAvatarData(id2);
				if (avatarData != null)
				{
					this.portrait.Load(avatarData.skinImageName, null, null);
				}
			}
		}
	}

	public void RefreshUI()
	{
		this.UpdateUI(this.rewardId);
	}

	public void OnClickClaimReward()
	{
		int type = -1;
		if (int.TryParse(this.rewardId, out type))
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.ClaimSeasonReward(type);
		}
		else
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("领取赛季奖励错误：" + this.rewardId, new object[0]);
		}
	}

	public UILabel desc;

	public UILabel score;

	public GameObject check;

	public UIButton claimBtn;

	public UILabel claimLabel;

	public GameObject portraitIcon;

	public GameObject moneyIcon;

	public UILabel moneyLabel;

	public PortraitTemplate portrait;

	private string rewardId;
}
