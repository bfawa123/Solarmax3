using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class MailItemBh : MonoBehaviour
{
	public void SetData(Mail mail)
	{
		this.config = mail;
		SystemLanguage language = Solarmax.Singleton<LanguageDataProvider>.Instance.GetLanguage();
		LanguageType languageType;
		if (language != SystemLanguage.ChineseSimplified)
		{
			if (language == SystemLanguage.ChineseTraditional)
			{
				languageType = LanguageType.TraditionalChinese;
				goto IL_45;
			}
			if (language != SystemLanguage.Chinese)
			{
				languageType = LanguageType.English;
				goto IL_45;
			}
		}
		languageType = LanguageType.Chinese;
		IL_45:
		foreach (MailLanguage mailLanguage in this.config.mailLanguages)
		{
			if (mailLanguage.language == languageType)
			{
				this.languageMail = mailLanguage;
			}
		}
		this.RefreshView();
	}

	public void RefreshView()
	{
		this.titleLabel.text = this.languageMail.title;
		string text = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds((double)this.config.sendTime).ToString("yyyy.MM.dd");
		this.dateLabel.text = text;
		if (this.config.Read)
		{
			this.mailUnCheckIcon.SetActive(false);
			this.mailCheckIcon.SetActive(true);
			this.redDot.SetActive(false);
		}
		else
		{
			this.mailUnCheckIcon.SetActive(true);
			this.mailCheckIcon.SetActive(false);
			this.redDot.SetActive(true);
		}
	}

	public GameObject redDot;

	public UILabel titleLabel;

	public UILabel dateLabel;

	public GameObject mailUnCheckIcon;

	public GameObject mailCheckIcon;

	public Mail config;

	public MailLanguage languageMail;
}
