using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class CooperationWidnowCell : MonoBehaviour
{
	private void ApplyCooperationModeLabel(int playerNum)
	{
		int id = (playerNum == 2) ? 7019 : ((playerNum == 3) ? 7020 : ((playerNum == 5) ? 7023 : 7021));
		string text = LanguageDataProvider.GetValue(id);
		string value = LanguageDataProvider.GetValue(2210);
		UILabel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UILabel>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UILabel uilabel = componentsInChildren[i];
			if (!(uilabel == null) && (uilabel.mKey == 2210 || (!string.IsNullOrEmpty(uilabel.text) && uilabel.text == value)))
			{
				uilabel.mKey = -1;
				uilabel.text = text;
			}
		}
	}

	public void SetInfo(ChapterInfo chapterInfo)
	{
		if (chapterInfo != null)
		{
			this.chapter = chapterInfo;
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapter.id);
			if (data == null)
			{
				Debug.LogError("error read config " + this.chapter.id);
				return;
			}
			if (!this.chapter.unLock)
			{
				this.bgBorder.height = 366;
				this.btnBuy.SetActive(true);
			}
			else
			{
				this.btnBuy.SetActive(false);
			}
			if (this.chapterBG != null)
			{
				ResourcesUtil.SetStarChartTexture(this.chapterBG, data.starChart);
			}
			this.ApplyCooperationModeLabel(Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetCoopPlayerNum());
			this.chapterMoney.text = data.costGold.ToString();
			this.chapterName.text = LanguageDataProvider.GetValue(data.name);
			this.chapterDesc.text = LanguageDataProvider.GetValue(data.describe);
			this.buyListener.onClick = new UIEventListener.VoidDelegate(this.OnClickBuy);
		}
	}

	public void OnClickBuy(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.chapter == null)
		{
			return;
		}
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapter.id);
		if (data == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		if (this.chapter.unLock)
		{
			Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter = this.chapter;
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("CooperationLevelWindow", EventId.Undefine, new object[0]));
			Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectChapter(this.chapter.id);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChapterWindow, new object[]
			{
				1,
				Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.id
			});
		}
		else
		{
			if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < data.costGold)
			{
				global::Coroutine.DelayDo(0.2f, new EventDelegate(delegate()
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow("GoldTipsWindow");
				}));
				return;
			}
			string value = LanguageDataProvider.GetValue(data.name);
			string value2 = LanguageDataProvider.GetValue(data.describe);
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogVIPWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
			{
				1,
				LanguageDataProvider.GetValue(1100),
				new EventDelegate(new EventDelegate.Callback(this.BuyChapter)),
				data.costGold,
				value2,
				value
			});
		}
	}

	private void BuyChapter()
	{
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapter.id);
		if (data == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < data.costGold)
		{
			global::Coroutine.DelayDo(0.2f, new EventDelegate(delegate()
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("GoldTipsWindow");
			}));
		}
		else
		{
			CSBuyChapter csbuyChapter = new CSBuyChapter();
			csbuyChapter.chapter = this.chapter.id;
			Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSBuyChapter>(219, csbuyChapter);
		}
	}

	public UITexture chapterBG;

	public UILabel chapterName;

	public UILabel chapterDesc;

	public UIEventListener buyListener;

	public GameObject btnBuy;

	public GameObject btnOwer;

	public UILabel chapterMoney;

	public GameObject infoContainer;

	public UISprite bgBorder;

	private ChapterInfo chapter;

	private const string GALAXY_TEXTURE_PATH = "gameres/texture_4/galaxyicon/";
}
