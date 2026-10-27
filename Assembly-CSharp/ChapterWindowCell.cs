using System;
using Solarmax;
using UnityEngine;

public class ChapterWindowCell : MonoBehaviour
{
	public void SetInfo(ChapterInfo info)
	{
		this.chapter = info;
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(info.id);
		if (data == null)
		{
			Debug.LogError("error read config " + info.id);
			return;
		}
		if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(this.chapter.id))
		{
			this.money.SetActive(false);
		}
		else
		{
			this.money.SetActive(true);
			this.needMoney = data.costGold;
			this.chapterMoney.text = data.costGold.ToString();
		}
		if (!string.IsNullOrEmpty(data.starChart))
		{
			ResourcesUtil.SetStarChartTexture(this.galaxyBg, data.starChart);
		}
		if (info.nPromotionPrice < data.costGold)
		{
			this.oldPrice.gameObject.SetActive(true);
			this.chapterMoney.text = info.nPromotionPrice.ToString();
			this.chapterMoney.color = Color.red;
			this.oldPrice.text = " (" + data.costGold.ToString() + ")";
		}
		else
		{
			this.oldPrice.gameObject.SetActive(false);
		}
		this.chapterDesc.text = LanguageDataProvider.GetValue(data.describe);
		this.chapterName.text = LanguageDataProvider.GetValue(data.name);
		if (Solarmax.Singleton<LevelDataHandler>.Instance.IsHotChapter(data.id))
		{
			this.hot.SetActive(true);
		}
		else
		{
			this.hot.SetActive(false);
		}
		if (this.chapter.ftotalStart > 0f)
		{
			this.score.text = string.Format("{0:F1}", this.chapter.ftotalStart);
			this.Fraction.SetActive(true);
		}
		else
		{
			this.score.text = string.Empty;
			this.Fraction.SetActive(false);
		}
	}

	public void OnClickBuy(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.chapter == null)
		{
			return;
		}
		Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectChapter(this.chapter.id);
	}

	public UILabel chapterMoney;

	public UILabel chapterDesc;

	public UILabel chapterName;

	public GameObject money;

	public UITexture galaxyBg;

	public GameObject hot;

	public UILabel score;

	public UILabel oldPrice;

	public GameObject Fraction;

	private ChapterInfo chapter;

	private int needMoney;

	private const string GALAXY_TEXTURE_PATH = "gameres/texture_4/galaxyicon/";
}
