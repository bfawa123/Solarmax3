using System;
using Solarmax;
using UnityEngine;

public class PersonalView : MonoBehaviour
{
	public void EnsureInit()
	{
		this.UpdateUI();
	}

	public void EnsureDestroy()
	{
	}

	private void UpdateUI()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		if (playerData != null)
		{
			this.scoreLabel.text = playerData.score.ToString();
			this.nameLabel.text = playerData.name;
			this.userId.text = Solarmax.Singleton<LanguageDataProvider>.Instance.GetData(2146) + playerData.userId.ToString();
			this.starLabel.text = Solarmax.Singleton<LevelDataHandler>.Instance.allStars.ToString();
			this.challenge.text = Solarmax.Singleton<LevelDataHandler>.Instance.GetChapterCompletedChallenges().ToString();
			this.starTabel.Reposition();
			this.scoreTabel.Reposition();
			this.challengeTabel.Reposition();
			if (!playerData.icon.EndsWith(".png"))
			{
				PlayerData playerData2 = playerData;
				playerData2.icon += ".png";
			}
			this.icon.picUrl = playerData.icon;
			if (playerData.battle_count <= 0)
			{
				this.shenglv.text = "0";
				this.changci.text = "0";
				this.Mvp.text = "0";
			}
			else
			{
				this.changci.text = playerData.battle_count.ToString();
				this.Mvp.text = playerData.mvp_count.ToString();
				double value = (double)playerData.mvp_count / ((double)playerData.battle_count * 1.0) * 100.0;
				this.shenglv.text = Math.Round(value, 1).ToString() + "%";
			}
		}
	}

	private void ClearTable()
	{
	}

	public NetTexture icon;

	public UILabel nameLabel;

	public UILabel scoreLabel;

	public UILabel starLabel;

	public UILabel changci;

	public UILabel Mvp;

	public UILabel shenglv;

	public UILabel userId;

	public UILabel challenge;

	public UITable starTabel;

	public UITable scoreTabel;

	public UITable challengeTabel;
}
