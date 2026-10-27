using System;
using Solarmax;
using UnityEngine;

[Serializable]
public class ActiveChapterObject
{
	public void SetInfo(ChapterInfo chapter)
	{
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapter.id);
		if (data != null)
		{
			this.chpaterName.text = LanguageDataProvider.GetValue(data.name);
		}
		this.dif1Value.text = string.Format("{0:F1}", chapter.fInterestStart);
		this.dif2Value.text = string.Format("{0:F1}", chapter.fStrategyStart);
		this.dif3Value.text = string.Format("{0:F1}", chapter.ftotalStart);
		if (!string.IsNullOrEmpty(data.starChart))
		{
			ResourcesUtil.SetStarChartTexture(this.iconGB, data.starChart);
		}
		if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(chapter.id))
		{
			this.btnLable.text = LanguageDataProvider.GetValue(2272);
			this.orgPrice.text = string.Empty;
			this.newPrice.text = string.Empty;
			this.icon.SetActive(false);
			this.line.SetActive(false);
			this.btnLable.transform.localPosition = Vector3.zero;
		}
		else if (data.costGold <= chapter.nPromotionPrice)
		{
			this.orgPrice.gameObject.SetActive(false);
			this.newPrice.text = chapter.nPromotionPrice.ToString();
			this.newPrice.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);
			this.icon.SetActive(true);
			this.line.SetActive(false);
		}
		else
		{
			this.orgPrice.gameObject.SetActive(true);
			this.btnLable.text = LanguageDataProvider.GetValue(2268);
			this.orgPrice.text = data.costGold.ToString();
			this.newPrice.text = chapter.nPromotionPrice.ToString();
			this.newPrice.color = Color.red;
			this.icon.SetActive(true);
			this.line.SetActive(true);
		}
	}

	public GameObject go;

	public UITexture iconGB;

	public UILabel chpaterName;

	public UILabel dif1Value;

	public UILabel dif2Value;

	public UILabel dif3Value;

	public UILabel orgPrice;

	public UILabel newPrice;

	public UILabel btnLable;

	public GameObject icon;

	public GameObject line;
}
