using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class MailWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnMailGetItemResponse);
		base.RegisterEvent(EventId.OnMailReadResponse);
		base.RegisterEvent(EventId.OnMailListResponse);
		base.RegisterEvent(EventId.OnMailDelResponse);
		this.mailModel = Solarmax.Singleton<MailModel>.Get();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.UpdateMailList();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		switch (eventId)
		{
		case EventId.OnMailListResponse:
			this.UpdateMailList();
			break;
		case EventId.OnMailReadResponse:
			this.currentMailItemBh.config.Read = true;
			this.currentMailItemBh.RefreshView();
			break;
		case EventId.OnMailGetItemResponse:
			this.currentMailItemBh.config.Get = true;
			this.UpdateMailInfo(this.currentMailItemBh.config, this.currentMailItemBh.languageMail);
			Tips.Make(LanguageDataProvider.GetValue(2333));
			break;
		case EventId.OnMailDelResponse:
			this.UpdateMailList();
			break;
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	public void OnTakeAttachmentClick()
	{
		Mail config = this.currentMailItemBh.GetComponent<MailItemBh>().config;
		Solarmax.Singleton<NetSystem>.Get().helper.GetMailItems(config.mailId);
	}

	public void OnDeleteMailClick()
	{
		Mail config = this.currentMailItemBh.GetComponent<MailItemBh>().config;
		if (config.mailItems.Count > 0 && !config.Get)
		{
			Tips.Make(LanguageDataProvider.GetValue(2334));
			return;
		}
		Solarmax.Singleton<NetSystem>.Get().helper.DeleteMail(config.mailId);
		Solarmax.Singleton<MailModel>.Get().DeleteMail(config.mailId);
	}

	public void OnMailItemClicked(GameObject go)
	{
		this.currentMailItemBh = go.GetComponent<MailItemBh>();
		go.GetComponent<UIToggle>().Set(true, true);
		Mail config = go.GetComponent<MailItemBh>().config;
		if (!config.Read)
		{
			Solarmax.Singleton<NetSystem>.Get().helper.ReadMail(config.mailId);
		}
		config.Read = true;
		this.currentMailItemBh.RefreshView();
		this.UpdateMailInfo(config, this.currentMailItemBh.languageMail);
	}

	private void UpdateMailInfo(Mail mail, MailLanguage mailLanguage)
	{
		for (int i = 0; i < this.attackments.Length; i++)
		{
			this.attackments[i].SetActive(false);
		}
		if (mail == null)
		{
			this.mailTitleLabel.text = string.Empty;
			this.mailContentLabel.text = string.Empty;
			this.takeBtn.SetActive(false);
			this.tokeLabel.gameObject.SetActive(false);
			this.deleteBtn.SetActive(false);
			return;
		}
		this.mailTitleLabel.text = mailLanguage.title;
		this.mailContentLabel.text = mailLanguage.content;
		for (int j = 0; j < mail.mailItems.Count; j++)
		{
			this.attackments[j].SetActive(true);
			this.attackments[j].GetComponent<MaitAttachBh>().SetAttachmentInfo(mail.mailItems[j]);
		}
		string text = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds((double)mail.sendTime).ToString("yyyy.MM.dd");
		this.mailDateLabel.text = text;
		if (!mail.Get && mail.mailItems.Count > 0)
		{
			this.takeBtn.SetActive(true);
			this.tokeLabel.gameObject.SetActive(false);
		}
		else
		{
			this.takeBtn.SetActive(false);
			this.tokeLabel.gameObject.SetActive(true);
		}
		this.deleteBtn.SetActive(true);
	}

	private void UpdateMailList()
	{
		this.UpdateMailInfo(null, null);
		this.mailItemsTable.transform.DestroyChildren();
		if (Solarmax.Singleton<MailModel>.Get().mailList == null)
		{
			return;
		}
		List<Mail> mail = Solarmax.Singleton<MailModel>.Get().mailList.mail;
		for (int i = 0; i < mail.Count; i++)
		{
			GameObject gameObject = this.mailItemsTable.gameObject.AddChild(this.mailItemTemplate);
			gameObject.SetActive(true);
			UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.OnMailItemClicked);
			MailItemBh component = gameObject.GetComponent<MailItemBh>();
			component.SetData(mail[i]);
			if (i == 0)
			{
				this.OnMailItemClicked(gameObject);
			}
		}
		this.mailItemsTable.Reposition();
		UIScrollView componentInParent = this.mailItemsTable.gameObject.GetComponentInParent<UIScrollView>();
		componentInParent.ResetPosition();
	}

	public UITable mailItemsTable;

	public UILabel mailTitleLabel;

	public UILabel mailContentLabel;

	public GameObject takeBtn;

	public UILabel tokeLabel;

	public GameObject deleteBtn;

	public UILabel attackmentLabel;

	public UILabel mailDateLabel;

	public GameObject[] attackments;

	public GameObject mailItemTemplate;

	private MailModel mailModel;

	public MailItemBh currentMailItemBh;
}
