using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LevelDiffuseCell : MonoBehaviour
{
	public void SetInfoLocal(int nDiffuse, string groupID, int length)
	{
		if (length < 0)
		{
			return;
		}
		if (nDiffuse < 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			if (i > length)
			{
				this.diffGo[i].SetActive(false);
			}
			else
			{
				this.diffGo[i].SetActive(true);
			}
		}
		List<string> list = new List<string>();
		AchievementGroup achievementGroup = Solarmax.Singleton<AchievementModel>.Get().achievementGroups[groupID];
		for (int j = 0; j < achievementGroup.achievements.Count; j++)
		{
			Achievement achievement = achievementGroup.achievements[j];
			TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(achievement.taskId);
			if (task == null)
			{
				if (achievement.types[0] == AchievementType.PassDiffcult)
				{
					list.Add("0");
				}
			}
			else if (achievement.types[0] == AchievementType.PassDiffcult)
			{
				list.Add(task.rewardValue.ToString());
			}
		}
		for (int k = 0; k <= length; k++)
		{
			this.display[k].alpha = 0.2f;
			this.displayBtn[k].ResetDefaultColor(new Color(1f, 1f, 1f, 0.2f));
			this.reward[k].text = list[k];
		}
		if (Solarmax.Singleton<LevelDataHandler>.Get().FindGroupLevel(groupID).unLock)
		{
			this.display[nDiffuse].alpha = 1f;
			this.displayBtn[nDiffuse].ResetDefaultColor(new Color(1f, 1f, 1f, 1f));
		}
		for (int l = 0; l <= length; l++)
		{
			this.star[l].SetActive(false);
			this.unStar[l].SetActive(false);
			this.lockIcon[l].SetActive(true);
		}
		ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Get().FindGroupLevel(groupID);
		if (chapterLevelGroup == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("关卡组为空", new object[0]);
		}
		if (chapterLevelGroup.unLock)
		{
			int num = 0;
			int num2 = 0;
			AchievementModel.GetDiffcultStar(groupID, out num, out num2);
			for (int m = 0; m < num; m++)
			{
				if (m < num2)
				{
					this.star[m].SetActive(true);
					this.unStar[m].SetActive(true);
					this.lockIcon[m].SetActive(false);
				}
				else if (m == num2 || (m > num2 && LocalPlayer.LocalUnlock))
				{
					this.star[m].SetActive(false);
					this.unStar[m].SetActive(true);
					this.lockIcon[m].SetActive(false);
				}
				else if (m > num2)
				{
					this.star[m].SetActive(false);
					this.unStar[m].SetActive(false);
					this.lockIcon[m].SetActive(true);
				}
			}
		}
	}

	public UIWidget[] display;

	public UILabel[] reward;

	public UIButton[] displayBtn;

	public GameObject[] diffGo;

	public GameObject[] star;

	public GameObject[] unStar;

	public GameObject[] lockIcon;
}
