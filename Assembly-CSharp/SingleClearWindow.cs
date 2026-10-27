using System;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class SingleClearWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnRename);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		string[] array = base.gameObject.name.Split(new char[]
		{
			'('
		});
		if (!string.IsNullOrEmpty(array[0]))
		{
			GuideManager.StartGuide(GuildCondition.GC_Ui, array[0], base.gameObject);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnRename)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("Rename with new name " + this.inputField.value, new object[0]);
				Solarmax.Singleton<LocalPlayer>.Get().playerData.name = this.inputField.value;
				Solarmax.Singleton<LocalAccountStorage>.Get().name = this.inputField.value;
				Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalAccount(false);
				Solarmax.Singleton<UISystem>.Get().HideWindow("SingleClearWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUpdateName, new object[0]);
				return;
			}
			if (errCode == ErrCode.EC_NameExist)
			{
				Tips.Make(LanguageDataProvider.GetValue(4));
				return;
			}
			if (errCode == ErrCode.EC_InvalidName)
			{
				Tips.Make(LanguageDataProvider.GetValue(5));
				return;
			}
			if (errCode == ErrCode.EC_AccountExist)
			{
				Tips.Make(LanguageDataProvider.GetValue(6));
				return;
			}
			Tips.Make(LanguageDataProvider.GetValue(7));
		}
	}

	public void OnEnterNameValueChanged()
	{
		string text = this.inputField.value.Trim();
		text = text.Replace('\r', ' ');
		text = text.Replace('\t', ' ');
		text = text.Replace('\n', ' ');
		while (this.EncodingTextLength(text) > 20)
		{
			text = text.Substring(0, text.Length - 1);
		}
		this.inputField.value = text;
	}

	private int EncodingTextLength(string s)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(s.Substring(i, 1));
			num += ((bytes.Length <= 1) ? 1 : 2);
		}
		return num;
	}

	public void OnRandNameClick()
	{
		this.inputField.value = AIManager.GetAIName(Time.frameCount);
		GuideManager.TriggerGuidecompleted(GuildEndEvent.rename);
	}

	public void OnConfirmClick()
	{
		string value = this.inputField.value;
		if (string.IsNullOrEmpty(value))
		{
			Tips.Make(LanguageDataProvider.GetValue(10));
			return;
		}
		bool flag = Solarmax.Singleton<NameFilterConfigProvider>.Instance.nameCheck(value);
		if (flag)
		{
			Tips.Make(LanguageDataProvider.GetValue(1114));
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.ChangeName(value);
	}

	public void OnCloseClick()
	{
	}

	public UIInput inputField;
}
