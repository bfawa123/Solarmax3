using System;
using System.Collections.Generic;
using Solarmax;

public class CloudWindow : BaseWindow
{
	private void Awake()
	{
	}

	private void Start()
	{
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnSyncuserAndChaptersResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.tipLabel.text = string.Empty;
		if (Solarmax.Singleton<UserSyncSysteam>.Get().curLoadStruct != null)
		{
			long regtimeSaveFile = Solarmax.Singleton<LocalAccountStorage>.Get().regtimeSaveFile;
			if (regtimeSaveFile > 0L)
			{
				DateTime dateTime = new DateTime(1970, 1, 1);
				dateTime = dateTime.AddSeconds((double)regtimeSaveFile);
				this.localLabel.text = dateTime.ToString();
			}
			else
			{
				this.localLabel.text = LanguageDataProvider.GetValue(2101);
			}
			long regtimeSaveFile2 = Solarmax.Singleton<UserSyncSysteam>.Get().curLoadStruct.regtimeSaveFile;
			DateTime dateTime2 = new DateTime(1970, 1, 1);
			dateTime2 = dateTime2.AddSeconds((double)regtimeSaveFile2);
			this.cloudLabel.text = dateTime2.ToString();
		}
	}

	public override void OnHide()
	{
		base.CancelInvoke("OnClose");
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnSyncuserAndChaptersResult)
		{
			int num = (int)args[0];
			int num2 = (int)args[1];
			if (num == 1)
			{
				if (num2 == 1)
				{
					Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1119), 2f);
				}
				else
				{
					Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1122), 2f);
				}
				base.Invoke("OnClose", 1f);
			}
		}
	}

	public void OnUpLoadChapterAndUser()
	{
		long regtimeSaveFile = Solarmax.Singleton<LocalAccountStorage>.Get().regtimeSaveFile;
		if (regtimeSaveFile <= 0L)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2101), 2f);
			return;
		}
		string text = Solarmax.Singleton<LocalAccountStorage>.Get().account + ".txt";
		string file = MonoSingleton<UpdateSystem>.Instance.saveRoot + text;
		Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl(text, "PUT", "text/plain", file, 117);
	}

	public void OnLoadChapterAndUser()
	{
		if (Solarmax.Singleton<UserSyncSysteam>.Get().curLoadStruct != null)
		{
			SyncMessageStruct curLoadStruct = Solarmax.Singleton<UserSyncSysteam>.Get().curLoadStruct;
			if (curLoadStruct != null)
			{
				Solarmax.Singleton<LocalPlayer>.Get().playerData.name = curLoadStruct.PlayerName;
				Solarmax.Singleton<LocalPlayer>.Get().playerData.icon = curLoadStruct.PlayerIcon;
				Solarmax.Singleton<LocalAccountStorage>.Get().regtimeSaveFile = curLoadStruct.regtimeSaveFile;
				Solarmax.Singleton<LocalPlayer>.Get().playerData.RegisteredtimeStamp = curLoadStruct.PlayerFrist;
				foreach (KeyValuePair<string, string> keyValuePair in curLoadStruct.listChapters)
				{
					string key = keyValuePair.Key;
					string value = keyValuePair.Value;
				}
			}
			Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalAccount(false);
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1120), 2f);
		}
		base.Invoke("OnClose", 1f);
	}

	public void OnClose()
	{
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChaptersView, new object[]
		{
			1
		});
	}

	public UILabel cloudLabel;

	public UILabel localLabel;

	public UILabel tipLabel;
}
