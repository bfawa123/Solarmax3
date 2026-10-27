using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LobbyAchieveTemplate : MonoBehaviour
{
	public void Init(TaskConfig config)
	{
		this.task = config;
		TaskModel taskModel = Solarmax.Singleton<TaskModel>.Get();
		taskModel.onRequestTaskOk = (TaskModel.OnRequestTaskOk)Delegate.Combine(taskModel.onRequestTaskOk, new TaskModel.OnRequestTaskOk(this.OnRecieveResponsed));
		this.UpdateUI();
	}

	public void Destroy()
	{
		TaskModel taskModel = Solarmax.Singleton<TaskModel>.Get();
		taskModel.onRequestTaskOk = (TaskModel.OnRequestTaskOk)Delegate.Remove(taskModel.onRequestTaskOk, new TaskModel.OnRequestTaskOk(this.OnRecieveResponsed));
	}

	public void OnclickRecieveButton()
	{
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED || Solarmax.Singleton<EngineSystem>.Get().GetNetworkRechability() == 0)
		{
			Tips.Make(LanguageDataProvider.GetValue(18));
			return;
		}
		Solarmax.Singleton<TaskModel>.Get().ClaimReward(this.task.id, null, 1);
	}

	private void OnRecieveResponsed(List<string> successes, List<string> failures)
	{
		foreach (string a in successes)
		{
			if (a == this.task.id)
			{
				this.UpdateUI();
				break;
			}
		}
		foreach (string a2 in failures)
		{
			if (a2 == this.task.id)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("成就奖励领取失败", new object[0]);
				break;
			}
		}
	}

	private void UpdateUI()
	{
		UIButton component = this.recieveGo.GetComponent<UIButton>();
		UILabel recieveLabel = this.recieveGo.GetComponent<UILabel>();
		TaskStatus status = this.task.status;
		if (status != TaskStatus.Completed)
		{
			if (status != TaskStatus.Received)
			{
				if (status == TaskStatus.Unfinished)
				{
					component.enabled = false;
					global::Coroutine.DelayDo(0.02f, new EventDelegate(delegate()
					{
						recieveLabel.text = LanguageDataProvider.GetValue(2080);
						recieveLabel.color = new Color(1f, 1f, 1f, 0.5f);
					}));
				}
			}
			else
			{
				component.enabled = false;
				global::Coroutine.DelayDo(0.02f, new EventDelegate(delegate()
				{
					recieveLabel.text = LanguageDataProvider.GetValue(2081);
					recieveLabel.color = new Color(1f, 1f, 1f, 1f);
				}));
			}
		}
		else
		{
			component.enabled = true;
			global::Coroutine.DelayDo(0.02f, new EventDelegate(delegate()
			{
				recieveLabel.text = LanguageDataProvider.GetValue(2077);
				recieveLabel.color = new Color(0f, 1f, 0f, 1f);
			}));
		}
	}

	public GameObject recieveGo;

	private TaskConfig task;
}
