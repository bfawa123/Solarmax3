using System;
using Solarmax;
using UnityEngine;

public class LevelStarCell : MonoBehaviour
{
	private void Start()
	{
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnButtonClick);
		if (this.difficultLabel != null)
		{
			UIEventListener.Get(this.difficultLabel.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnDiffcultClick);
		}
	}

	private void OnDestroy()
	{
		if (this.difficultLabel != null)
		{
			UIEventListener.Get(this.difficultLabel.gameObject).onClick = null;
		}
	}

	private void OnButtonClick(GameObject go)
	{
		Solarmax.Singleton<EventSystem>.Get().FireEvent(EventId.OnNumSelectClicked, new object[]
		{
			go
		});
	}

	private void OnDiffcultClick(GameObject go)
	{
		this.showDifficult = !this.showDifficult;
		Solarmax.Singleton<EventSystem>.Get().FireEvent(EventId.OnDifficultSelectClicked, new object[]
		{
			this.showDifficult
		});
	}

	public void SetShowDifficult(bool show)
	{
		this.showDifficult = show;
	}

	public void SetDifficult(int difficult, bool show)
	{
		this.difficultLabel.gameObject.SetActive(show);
		if (!show)
		{
			return;
		}
		if (difficult != 0)
		{
			if (difficult != 1)
			{
				if (difficult == 2)
				{
					this.difficultLabel.text = LanguageDataProvider.GetValue(2106);
				}
			}
			else
			{
				this.difficultLabel.text = LanguageDataProvider.GetValue(2105);
			}
		}
		else
		{
			this.difficultLabel.text = LanguageDataProvider.GetValue(2104);
		}
	}

	public void SetLevelCell(bool hide = false)
	{
		int num = this.starList.Length;
		if (hide)
		{
			for (int i = 0; i < num; i++)
			{
				this.starList[i].gameObject.SetActive(false);
			}
			return;
		}
		if (!this.IsLevel)
		{
			for (int j = 0; j < num; j++)
			{
				this.starList[j].gameObject.SetActive(false);
			}
			return;
		}
		if (!this.unLock)
		{
			for (int k = 0; k < num; k++)
			{
				this.starList[k].gameObject.SetActive(false);
			}
		}
		else
		{
			if (this.MaxStar == 4)
			{
				this.starList[0].gameObject.transform.localPosition = new Vector3(-36f, -40f, 0f);
				this.starList[1].gameObject.transform.localPosition = new Vector3(-12f, -40f, 0f);
				this.starList[2].gameObject.transform.localPosition = new Vector3(12f, -40f, 0f);
				this.starList[3].gameObject.transform.localPosition = new Vector3(36f, -40f, 0f);
			}
			else if (this.MaxStar == 3)
			{
				this.starList[0].gameObject.transform.localPosition = new Vector3(-24f, -40f, 0f);
				this.starList[1].gameObject.transform.localPosition = new Vector3(0f, -40f, 0f);
				this.starList[2].gameObject.transform.localPosition = new Vector3(24f, -40f, 0f);
			}
			if (this.MaxStar == 2)
			{
				this.starList[0].gameObject.transform.localPosition = new Vector3(-12f, -40f, 0f);
				this.starList[1].gameObject.transform.localPosition = new Vector3(12f, -40f, 0f);
			}
			else if (this.MaxStar == 1)
			{
				this.starList[0].gameObject.transform.localPosition = new Vector3(0f, -40f, 0f);
			}
			for (int l = 0; l < 4; l++)
			{
				if (l < this.MaxStar)
				{
					this.starList[l].gameObject.SetActive(true);
					Transform transform = this.starList[l].transform.Find("Star");
					transform.gameObject.SetActive(l < this.nStar);
				}
				else
				{
					this.starList[l].gameObject.SetActive(false);
					Transform transform2 = this.starList[l].transform.Find("Star");
					transform2.gameObject.SetActive(false);
				}
			}
		}
		if (this.nStar == 3 && this.MaxStar == 3)
		{
			this.star3Effect.SetActive(true);
			this.animator = this.star3Effect.transform.Find("Plane").GetComponent<Animator>();
			this.animator.Play("EFF_XJ_SaoXing");
			this.star4Effect.SetActive(false);
		}
		else if (this.nStar == 4 && this.MaxStar == 4)
		{
			this.star3Effect.SetActive(false);
			this.star4Effect.SetActive(true);
		}
		else
		{
			this.star3Effect.SetActive(false);
			this.star4Effect.SetActive(false);
		}
	}

	public void SetChapterStar()
	{
		if (!this.unLock)
		{
			for (int i = 0; i < 4; i++)
			{
				this.starList[i].gameObject.SetActive(false);
			}
		}
		else
		{
			this.starList[0].gameObject.SetActive(true);
			if (this.label != null)
			{
				this.label.gameObject.SetActive(true);
				this.label.text = string.Format("{0}/{1}", this.achievedStars, this.maxStars);
			}
			for (int j = 1; j < 4; j++)
			{
				this.starList[j].gameObject.SetActive(false);
			}
		}
	}

	public UISprite[] starList;

	public int nStar;

	public bool unLock;

	public bool IsLevel;

	public int MaxStar = 4;

	public UILabel label;

	public UILabel difficultLabel;

	public GameObject star3Effect;

	public GameObject star4Effect;

	public int achievedStars;

	public int maxStars;

	public int number;

	private bool showDifficult;

	private Animator animator;
}
