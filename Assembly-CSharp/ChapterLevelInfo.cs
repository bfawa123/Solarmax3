using System;
using Solarmax;

public class ChapterLevelInfo
{
	public ChapterLevelInfo()
	{
		this.id = string.Empty;
		this.star = 0;
		this.isMain = false;
		LoggerSystem.CodeComments("Set Level Unlock here");
		this.unLock = LocalPlayer.LocalUnlock;
		this.maxStar = 0;
		this.difficult = -1;
	}

	public void Reset()
	{
		this.star = 0;
		this.unLock = false;
	}

	public string id;

	public string chapterId;

	public string groupId;

	public int star;

	public bool isMain;

	public bool unLock;

	public int Score;

	public int maxStar;

	public int difficult;
}
