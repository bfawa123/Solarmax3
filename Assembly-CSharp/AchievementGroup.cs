using System;
using System.Collections.Generic;

public class AchievementGroup
{
	public List<Achievement> GetAchievementByDifficult(AchievementDifficult difficult, bool containPass = true)
	{
		List<Achievement> list = new List<Achievement>();
		if (difficult != AchievementDifficult.Simple)
		{
			if (difficult != AchievementDifficult.Hard)
			{
				if (difficult == AchievementDifficult.Hell)
				{
					foreach (Achievement achievement in this.dicAchievements[AchievementDifficult.Hell])
					{
						if (achievement.types[0] != AchievementType.PassDiffcult || containPass)
						{
							if (!achievement.success)
							{
								list.Add(achievement);
							}
						}
					}
				}
			}
			else
			{
				foreach (Achievement achievement2 in this.dicAchievements[AchievementDifficult.Hard])
				{
					if (achievement2.types[0] != AchievementType.PassDiffcult || containPass)
					{
						if (!achievement2.success)
						{
							list.Add(achievement2);
						}
					}
				}
			}
		}
		else
		{
			foreach (Achievement achievement3 in this.dicAchievements[AchievementDifficult.Simple])
			{
				if (achievement3.types[0] != AchievementType.PassDiffcult || containPass)
				{
					if (!achievement3.success)
					{
						list.Add(achievement3);
					}
				}
			}
		}
		return list;
	}

	public string id;

	public string groupid;

	public List<Achievement> achievements;

	public Dictionary<AchievementDifficult, List<Achievement>> dicAchievements;

	public Dictionary<string, Achievement> idAchievements;
}
