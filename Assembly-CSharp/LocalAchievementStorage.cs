using System;
using System.Collections.Generic;
using System.Linq;
using Solarmax;
using UnityEngine;

public class LocalAchievementStorage : Solarmax.Singleton<LocalAchievementStorage>, ILocalStorage
{
	public string Name()
	{
		return "ACHIEVE_RECORD1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		manager.PutString("ACHIEVE", this.DicToString());
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		AchievementModel achievementModel = Solarmax.Singleton<AchievementModel>.Get();
		achievementModel.onAchieveSuccess = (AchievementModel.OnAchieveSuccess)Delegate.Combine(achievementModel.onAchieveSuccess, new AchievementModel.OnAchieveSuccess(this.OnSuccessChanged));
		string @string = manager.GetString("ACHIEVE", string.Empty);
		this.StringToDic(@string);
	}

	public void Clear(LocalStorageSystem manager)
	{
		AchievementModel achievementModel = Solarmax.Singleton<AchievementModel>.Get();
		achievementModel.onAchieveSuccess = (AchievementModel.OnAchieveSuccess)Delegate.Remove(achievementModel.onAchieveSuccess, new AchievementModel.OnAchieveSuccess(this.OnSuccessChanged));
	}

	public void OnSuccessChanged(string id, bool success)
	{
		if (this.dicAchievement.ContainsKey(id) && this.dicAchievement[id] == success)
		{
			return;
		}
		this.dicAchievement[id] = success;
	}

	public bool GetStatus(string id)
	{
		return this.dicAchievement.ContainsKey(id) && this.dicAchievement[id];
	}

	public void CheckSyncData(Dictionary<string, Dictionary<string, bool>> dic)
	{
		if (dic == null || dic.Count == 0)
		{
			return;
		}
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		foreach (KeyValuePair<string, AchievementGroup> keyValuePair in Solarmax.Singleton<AchievementModel>.Get().achievementGroups)
		{
			foreach (Achievement achievement in keyValuePair.Value.achievements)
			{
				if (this.dicAchievement[achievement.id] && (!dic.ContainsKey(keyValuePair.Key) || !dic[keyValuePair.Key].ContainsKey(achievement.id)))
				{
					string groupId = Solarmax.Singleton<AchievementModel>.Get().dicAchievements[achievement.id].groupId;
					if (!dictionary.ContainsKey(groupId))
					{
						dictionary[groupId] = new List<string>();
					}
					dictionary[groupId].Add(achievement.id);
				}
			}
		}
		int num = 0;
		if (dictionary.Count > 0)
		{
			Debug.Log("------------本地数据异常，需要上传服务器----------");
		}
		using (Dictionary<string, List<string>>.Enumerator enumerator3 = dictionary.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				KeyValuePair<string, List<string>> elemet = enumerator3.Current;
				global::Coroutine.DelayDo((float)num++, new EventDelegate(delegate()
				{
					Solarmax.Singleton<NetSystem>.Instance.helper.SendAchievementSet(elemet.Key, elemet.Value);
				}));
			}
		}
	}

	public void CheckSyncData(string gId, Dictionary<string, bool> sAchievemets)
	{
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[gId].achievements)
		{
			if (this.dicAchievement[achievement.id] && !sAchievemets.ContainsKey(achievement.id))
			{
				string groupId = Solarmax.Singleton<AchievementModel>.Get().dicAchievements[achievement.id].groupId;
				if (!dictionary.ContainsKey(groupId))
				{
					dictionary[groupId] = new List<string>();
				}
				dictionary[groupId].Add(achievement.id);
			}
		}
		foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary)
		{
			Debug.LogError("------------本地数据异常，需要上传服务器----------");
			Solarmax.Singleton<NetSystem>.Instance.helper.SendAchievementSet(keyValuePair.Key, keyValuePair.Value);
		}
	}

	private string DicToString()
	{
		return string.Join(";", (from x in this.dicAchievement
		select x.Key + "=" + x.Value).ToArray<string>());
	}

	private void StringToDic(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			this.dicAchievement.Clear();
			return;
		}
		string[] array = str.Split(new char[]
		{
			';'
		});
		foreach (string text in array)
		{
			string[] array3 = text.Split(new char[]
			{
				'='
			});
			string key = array3[0];
			string text2 = array3[1];
			if (text2.Equals("True"))
			{
				this.dicAchievement[key] = true;
			}
			else
			{
				this.dicAchievement[key] = false;
			}
		}
	}

	public int ver = 1;

	public string achieve = string.Empty;

	public Dictionary<string, bool> dicAchievement = new Dictionary<string, bool>();
}
