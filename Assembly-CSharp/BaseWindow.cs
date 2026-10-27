using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public abstract class BaseWindow : MonoBehaviour
{
	public BaseWindow()
	{
		this.mRegisteredEvents = new List<EventId>();
		this.langList = new List<UILabel>();
		this.mConfigData = null;
	}

	protected void RegisterEvent(EventId eventId)
	{
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(eventId, this, this.mConfigData.mName, new Callback<int, object, object[]>(Solarmax.Singleton<UISystem>.Instance.OnEventHandler));
		this.mRegisteredEvents.Add(eventId);
	}

	protected void RegisterEvent(EventId eventId, string name)
	{
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(eventId, this, name, new Callback<int, object, object[]>(Solarmax.Singleton<UISystem>.Instance.OnEventHandler));
		this.mRegisteredEvents.Add(eventId);
	}

	protected void UnRegisterEvent(EventId eventId)
	{
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(eventId, this);
		this.mRegisteredEvents.Remove(eventId);
	}

	private void UnRegisterAllEvent()
	{
		for (int i = this.mRegisteredEvents.Count - 1; i >= 0; i--)
		{
			this.UnRegisterEvent(this.mRegisteredEvents[i]);
		}
	}

	public void SetConfigData(UIWindowConfig data)
	{
		this.mConfigData = data;
	}

	public UIWindowConfig GetConfigData()
	{
		return this.mConfigData;
	}

	public string GetName()
	{
		return (this.mConfigData != null) ? this.mConfigData.mName : string.Empty;
	}

	public virtual bool Init()
	{
		UILabel[] componentsInChildren = base.transform.GetComponentsInChildren<UILabel>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].mKey > 0)
			{
				this.langList.Add(componentsInChildren[i]);
			}
		}
		return true;
	}

	public virtual void Release()
	{
		this.langList.Clear();
		this.UnRegisterAllEvent();
	}

	public virtual void OnShow()
	{
		this.ReFreshLanguage();
	}

	public abstract void OnHide();

	public abstract void OnUIEventHandler(EventId eventId, params object[] args);

	public void ReFreshLanguage()
	{
		for (int i = 0; i < this.langList.Count; i++)
		{
			this.langList[i].text = LanguageDataProvider.GetValue(this.langList[i].mKey);
		}
		this.OnLanguageChanged();
	}

	public virtual void OnLanguageChanged()
	{
	}

	private List<EventId> mRegisteredEvents;

	private List<UILabel> langList;

	private UIWindowConfig mConfigData;
}
