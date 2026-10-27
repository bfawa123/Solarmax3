using System;
using System.Collections.Generic;
using System.Linq;
using Solarmax;

public class LocalLevelScoreStorage : Solarmax.Singleton<LocalLevelScoreStorage>, ILocalStorage
{
	public string Name()
	{
		return "LocalLevelScore1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		manager.PutString("LevelScore", this.DicToString());
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		this.StringToDic(manager.GetString("LevelScore", string.Empty));
		Solarmax.Singleton<LevelDataHandler>.Get().SetLocalScore();
	}

	public void Clear(LocalStorageSystem manager)
	{
	}

	private string DicToString()
	{
		return string.Join(";", (from x in this.levelScore
		select x.Key + "=" + x.Value).ToArray<string>());
	}

	private void StringToDic(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			this.levelScore.Clear();
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
			string s = array3[1];
			int value = 0;
			if (int.TryParse(s, out value))
			{
				this.levelScore[key] = value;
			}
		}
	}

	public int ver = 1;

	public Dictionary<string, int> levelScore = new Dictionary<string, int>();
}
