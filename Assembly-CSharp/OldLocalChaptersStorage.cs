using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;

public class OldLocalChaptersStorage : Solarmax.Singleton<OldLocalChaptersStorage>, OldILocalStorage
{
	public string Name()
	{
		return "LocalChapter";
	}

	public void Save(OldLocalStorageSystem manager)
	{
	}

	public void Clear(OldLocalStorageSystem manager)
	{
	}

	public void LevelIdConvert()
	{
		this.levels.Add("100106", "100104");
		this.levels.Add("100108", "100105");
		this.levels.Add("100109", "100106");
		this.levels.Add("100110", "100107");
		this.levels.Add("200201", "100211");
		this.levels.Add("200301", "100311");
		this.levels.Add("200302", "100312");
		this.levels.Add("200401", "100415");
		this.levels.Add("200402", "100416");
		this.levels.Add("100511", "100508");
		this.levels.Add("100508", "100509");
		this.levels.Add("100509", "100510");
		this.levels.Add("100510", "100511");
		this.levels.Add("200501", "100512");
		this.levels.Add("200502", "100513");
		this.levels.Add("200503", "100514");
		this.levels.Add("100611", "100601");
		this.levels.Add("100612", "100602");
		this.levels.Add("100613", "100603");
		this.levels.Add("100608", "100605");
		this.levels.Add("100609", "100606");
		this.levels.Add("100601", "100607");
		this.levels.Add("100602", "100608");
		this.levels.Add("100603", "100609");
		this.levels.Add("100607", "100610");
		this.levels.Add("100606", "100611");
		this.levels.Add("100605", "100612");
		this.levels.Add("200601", "100613");
		this.levels.Add("200602", "100614");
		this.levels.Add("200603", "100615");
		this.levels.Add("100711", "100701");
		this.levels.Add("100712", "100702");
		this.levels.Add("100701", "100703");
		this.levels.Add("100702", "100704");
		this.levels.Add("100703", "100705");
		this.levels.Add("100707", "100706");
		this.levels.Add("100708", "100707");
		this.levels.Add("100704", "100708");
		this.levels.Add("100705", "100709");
		this.levels.Add("100706", "100710");
		this.levels.Add("200701", "100711");
		this.levels.Add("200702", "100712");
	}

	public void Load(OldLocalStorageSystem manager)
	{
		this.LevelIdConvert();
		int @int = manager.GetInt();
		for (int i = 0; i < @int; i++)
		{
			string @string = manager.GetString();
			int int2 = manager.GetInt();
			string string2 = manager.GetString();
			bool bought = manager.GetInt() == 1;
			bool passed = manager.GetInt() == 1;
			UploadOldVersionChapter uploadOldVersionChapter = this.LocalStorageToOneChapter(@string, int2, string2, true, bought, passed);
			if (uploadOldVersionChapter != null)
			{
				OldLocalStorageSystem.oldData.chapters.Add(uploadOldVersionChapter);
			}
		}
	}

	public UploadOldVersionChapter LocalStorageToOneChapter(string chapterId, int curSelectLevel, string LevelInfos, bool b = false, bool bought = false, bool passed = false)
	{
		if (string.IsNullOrEmpty(chapterId) || string.IsNullOrEmpty(LevelInfos))
		{
			return null;
		}
		int num = 0;
		UploadOldVersionChapter uploadOldVersionChapter = new UploadOldVersionChapter();
		uploadOldVersionChapter.chapterId = chapterId;
		string[] array = LevelInfos.Split(new char[]
		{
			';'
		});
		int num2 = array.Length;
		for (int i = 0; i < num2; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			int num3 = array2.Length;
			string text = (num3 <= 1) ? string.Empty : array2[0];
			if (string.IsNullOrEmpty(text))
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format(OldLocalStorageSystem.OLD_LOCAL_SAVE_TAG, "关卡组id为空"), new object[0]);
			}
			else
			{
				if (this.levels.ContainsKey(text))
				{
					text = this.levels[text];
				}
				ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Get().FindGroupLevel(text);
				if (chapterLevelGroup != null)
				{
					int num4 = (num3 < 2) ? 0 : int.Parse(array2[1]);
					int num5 = (num3 < 3) ? 0 : int.Parse(array2[2]);
					if (Solarmax.Singleton<AchievementListProvider>.Get().dataList.ContainsKey(text))
					{
						UploadOldVersionAchieve uploadOldVersionAchieve = new UploadOldVersionAchieve();
						uploadOldVersionAchieve.levelGroupId = text;
						AchievementListConfig achievementListConfig = Solarmax.Singleton<AchievementListProvider>.Get().dataList[text];
						int num6 = (achievementListConfig.achieveList.Length <= num4) ? achievementListConfig.achieveList.Length : num4;
						for (int j = 0; j < num6; j++)
						{
							uploadOldVersionAchieve.achieveId.Add(achievementListConfig.achieveList[j]);
						}
						uploadOldVersionChapter.achieves.Add(uploadOldVersionAchieve);
						num4 = num6;
					}
					else
					{
						num4 = 1;
					}
					if (num4 == 0 || num5 == 0)
					{
					}
					else
					{
						num += num4;
					}
					foreach (ChapterLevelInfo chapterLevelInfo in chapterLevelGroup.mapList)
					{
						UploadOldVersionLevel uploadOldVersionLevel = new UploadOldVersionLevel();
						uploadOldVersionLevel.levelId = chapterLevelInfo.id;
						uploadOldVersionLevel.star = num4;
						uploadOldVersionLevel.score = num5;
						uploadOldVersionChapter.levels.Add(uploadOldVersionLevel);
					}
				}
			}
		}
		if (num == 0)
		{
			return null;
		}
		return uploadOldVersionChapter;
	}

	private Dictionary<string, string> levels = new Dictionary<string, string>();
}
