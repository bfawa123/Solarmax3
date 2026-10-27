using System;
using Solarmax;
using UnityEngine;

public class CooperationAssistCell : MonoBehaviour
{
	public void SetInfo(ChapterAssistConfig chapterInfo, bool unLock, int nStart = 0)
	{
		if (chapterInfo != null)
		{
			this.Config = chapterInfo;
			this.chapterName.text = LanguageDataProvider.GetValue(this.Config.name);
			if (unLock)
			{
				this.selected.SetActive(false);
				this.unSelected.SetActive(true);
			}
			else
			{
				this.selected.SetActive(false);
				this.unSelected.SetActive(false);
			}
			this.UpdateUI(unLock);
		}
	}

	public void RefreshBtnStats(string levelAssistID, bool unLock, int nStart)
	{
		if (this.Config != null)
		{
			if (unLock)
			{
				this.LuckIcon.SetActive(false);
				if (this.Config.id.Equals(levelAssistID))
				{
					this.selected.SetActive(true);
					this.unSelected.SetActive(false);
				}
				else
				{
					this.selected.SetActive(false);
					this.unSelected.SetActive(true);
				}
			}
			else
			{
				this.LuckIcon.SetActive(true);
				if (this.Config.id.Equals(levelAssistID))
				{
					this.selected.SetActive(true);
					this.unSelected.SetActive(false);
				}
				else
				{
					this.selected.SetActive(false);
					this.unSelected.SetActive(true);
				}
			}
			this.UpdateUI(unLock);
			for (int i = 0; i < this.start.Length; i++)
			{
				if (i < nStart)
				{
					this.start[i].SetActive(true);
				}
				else
				{
					this.start[i].SetActive(false);
				}
			}
		}
	}

	private void UpdateUI(bool unLock)
	{
		this.unLock = unLock;
		this.LuckIcon.SetActive(false);
		if (unLock)
		{
			this.bg.alpha = 1f;
			this.chapterName.alpha = 1f;
			foreach (UISprite uisprite in this.starBorder)
			{
				uisprite.alpha = 1f;
			}
		}
		else
		{
			this.bg.alpha = 0.3f;
			this.chapterName.alpha = 0.3f;
			foreach (UISprite uisprite2 in this.starBorder)
			{
				uisprite2.alpha = 0.3f;
			}
		}
	}

	public void OnClickEvent(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		if (this.Config == null)
		{
			return;
		}
		if (!this.unLock)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2113), 1f);
			return;
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.SelectCooperationLevel, new object[]
		{
			this.Config.id
		});
	}

	public UITexture chapterBG;

	public UILabel chapterName;

	public UIEventListener buyListener;

	public GameObject selected;

	public GameObject unSelected;

	public GameObject LuckIcon;

	public GameObject[] start;

	public UISprite[] starBorder;

	public UISprite bg;

	[NonSerialized]
	public ChapterAssistConfig Config;

	private bool unLock;
}
