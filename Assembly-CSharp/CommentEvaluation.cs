using System;
using Solarmax;
using UnityEngine;

public class CommentEvaluation : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void EnsureInit()
	{
		this.averageNumber.text = Solarmax.Singleton<CommentModel>.Get().averageScore.ToString();
		if (Solarmax.Singleton<CommentModel>.Get().canEvaluate)
		{
			foreach (UISprite uisprite in this.likeBtns)
			{
				UIEventListener.Get(uisprite.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLikeButton);
			}
		}
		else
		{
			for (int j = 0; j < 5; j++)
			{
				this.likeBtns[j].GetComponent<BoxCollider>().enabled = false;
				if (j < Solarmax.Singleton<CommentModel>.Get().levelScore)
				{
					this.likeBtns[j].spriteName = "Btn_comment_likeAC";
				}
				else
				{
					this.likeBtns[j].spriteName = "Btn_comment_likeA";
				}
			}
			Solarmax.Singleton<CommentModel>.Get().PullLevelScore(delegate(bool result)
			{
				if (result)
				{
				}
			});
			Solarmax.Singleton<CommentModel>.Get().PullAverageLevelScore(delegate(bool result)
			{
				if (result)
				{
					this.averageNumber.text = Solarmax.Singleton<CommentModel>.Get().averageScore.ToString();
				}
				else
				{
					this.averageNumber.text = Solarmax.Singleton<CommentModel>.Get().averageScore.ToString();
				}
			});
		}
	}

	public void EnsureDestroy()
	{
		foreach (UISprite uisprite in this.likeBtns)
		{
			UIEventListener.Get(uisprite.gameObject).onClick = null;
		}
	}

	public void OnClickLikeButton(GameObject go)
	{
		int num = 0;
		if (go.name.EndsWith("1"))
		{
			num = 1;
		}
		else if (go.name.EndsWith("2"))
		{
			num = 2;
		}
		else if (go.name.EndsWith("3"))
		{
			num = 3;
		}
		else if (go.name.EndsWith("4"))
		{
			num = 4;
		}
		else if (go.name.EndsWith("5"))
		{
			num = 5;
		}
		for (int i = 0; i < 5; i++)
		{
			if (i < num)
			{
				this.likeBtns[i].spriteName = "Btn_comment_likeAC";
			}
			else
			{
				this.likeBtns[i].spriteName = "Btn_comment_likeA";
			}
		}
		Solarmax.Singleton<CommentModel>.Get().SendLevelEvalution(num, delegate(bool result)
		{
			if (result)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("----评星成功----", new object[0]);
				this.tips.SetActive(true);
				int num2 = 100;
				this.rewardNumber.text = num2.ToString();
				this.EnsureInit();
			}
			else
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("----评星失败----", new object[0]);
			}
		});
	}

	private void UpdateUI()
	{
		this.EnsureInit();
	}

	public GameObject tips;

	public UILabel rewardNumber;

	public UILabel averageNumber;

	public UISprite[] likeBtns = new UISprite[5];

	private const string LIKE_BTN = "LikeBtn{0}";

	private const string LIKE_NAME = "Btn_comment_likeAC";

	private const string UNLIKE_NAME = "Btn_comment_likeA";
}
