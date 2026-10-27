using System;
using System.Collections.Generic;
using Solarmax;

public class LocalChaptersStorage : Solarmax.Singleton<LocalChaptersStorage>, ILocalStorage
{
	public string Name()
	{
		return "LocalChapter1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		List<ChapterInfo> chapterList = Solarmax.Singleton<LevelDataHandler>.Get().chapterList;
		int count = Solarmax.Singleton<LevelDataHandler>.Get().chapterList.Count;
		for (int i = 0; i < count; i++)
		{
			ChapterInfo chapterInfo = chapterList[i];
			manager.PutBool(chapterInfo.id, chapterInfo.trackChallenge);
		}
	}

	public void Clear(LocalStorageSystem manager)
	{
		Solarmax.Singleton<LevelDataHandler>.Get().Reset();
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		List<ChapterInfo> chapterList = Solarmax.Singleton<LevelDataHandler>.Get().chapterList;
		int count = Solarmax.Singleton<LevelDataHandler>.Get().chapterList.Count;
		for (int i = 0; i < count; i++)
		{
			ChapterInfo chapterInfo = chapterList[i];
			chapterInfo.trackChallenge = manager.GetBool(chapterInfo.id, true);
		}
	}

	public int ver = 1;
}
