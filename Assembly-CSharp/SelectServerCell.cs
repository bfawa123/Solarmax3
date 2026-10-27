using System;
using Solarmax;
using UnityEngine;

public class SelectServerCell : MonoBehaviour
{
	public void SetInfoLocal(ServerListItemConfig serverCfg, GameObject parent)
	{
		this.selectPanel = parent;
		this.config = serverCfg;
		if (this.nameLabel != null)
		{
			this.nameLabel.text = ((this.config != null) ? this.config.Name : string.Empty);
		}
		if (this.descLabel != null)
		{
			string text = string.Empty;
			if (this.config != null)
			{
				if (!string.IsNullOrEmpty(this.config.IPAddress))
				{
					text = this.config.IPAddress;
				}
				else if (!string.IsNullOrEmpty(this.config.Url))
				{
					text = this.config.Url;
				}
			}
			this.descLabel.text = text;
		}
	}

	public void OnSelectServer()
	{
		if (this.config == null)
		{
			return;
		}
		string name = this.config.Name;
		this.selectPanel.SetActive(false);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSelectServerList, new object[]
		{
			this.config
		});
		MonoSingleton<FlurryAnalytis>.Instance.FlurrySelectServer(name);
		MiGameAnalytics.MiAnalyticsSelectServer(name);
	}

	private GameObject selectPanel;

	public UILabel nameLabel;

	public UILabel descLabel;

	private ServerListItemConfig config;
}
