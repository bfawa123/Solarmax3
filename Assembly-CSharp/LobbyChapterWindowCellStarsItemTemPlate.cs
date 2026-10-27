using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LobbyChapterWindowCellStarsItemTemPlate : MonoBehaviour
{
	public void setStarsPos(int star, bool unLock, int maxStars = 3)
	{
		this.IsUnLock = unLock;
		if (unLock)
		{
			GameObject gameObject;
			if (maxStars == 3)
			{
				this.threeStarts.SetActive(false);
				this.fourStarts.SetActive(false);
				gameObject = this.threeStarts;
			}
			else
			{
				this.threeStarts.SetActive(false);
				this.fourStarts.SetActive(false);
				gameObject = this.fourStarts;
			}
			for (int i = 1; i <= star; i++)
			{
				string name = "Star" + i;
				GameObject gameObject2 = gameObject.transform.Find(name).gameObject;
				gameObject2.GetComponent<UISprite>().spriteName = "icon_CopyUI_CopyUnopenStart";
			}
		}
		else
		{
			this.threeStarts.SetActive(false);
			this.fourStarts.SetActive(false);
		}
	}

	public void SetGreekLetter(Vector3 greekPos, string name)
	{
		this.greekLetter.SetActive(true);
		this.greekLetter.transform.localPosition = greekPos;
		this.greekLetter.GetComponent<UILabel>().text = name;
	}

	public void SetStarName(Vector3 starNamePos, int nKey)
	{
		this.starName.SetActive(true);
		this.nameKey = nKey;
		this.starName.transform.localPosition = starNamePos;
		this.starName.GetComponent<UILabel>().text = LanguageDataProvider.GetValue(this.nameKey);
	}

	public void SetStarName(Vector3 starNamePos, int nKey, float greekNamePosX)
	{
		this.starName.SetActive(true);
		this.nameKey = nKey;
		if (starNamePos.x <= greekNamePosX)
		{
			this.starName.GetComponent<UILabel>().pivot = UIWidget.Pivot.Right;
			this.starName.GetComponent<UILabel>().alignment = NGUIText.Alignment.Right;
		}
		else
		{
			this.starName.GetComponent<UILabel>().pivot = UIWidget.Pivot.Left;
			this.starName.GetComponent<UILabel>().alignment = NGUIText.Alignment.Left;
		}
		this.starName.transform.localPosition = starNamePos;
		this.starName.GetComponent<UILabel>().text = LanguageDataProvider.GetValue(this.nameKey);
	}

	public void SetStarName()
	{
		this.starName.GetComponent<UILabel>().text = LanguageDataProvider.GetValue(this.nameKey);
	}

	public void SetLevel(bool bNor, int star, Vector3 scale, bool unLock = true)
	{
		if (bNor)
		{
			this.bosBG.SetActive(true);
			this.bosBG.transform.localScale = scale;
			this.norBG.SetActive(false);
		}
		else
		{
			this.bosBG.SetActive(false);
			this.norBG.SetActive(true);
			this.norBG.transform.localScale = scale;
		}
		UISprite component = this.norBG.GetComponent<UISprite>();
		UISprite component2 = this.bosBG.GetComponent<UISprite>();
		if (star != 0)
		{
			component.spriteName = "Btn_CopyUI_MainCopyOpen1";
			component2.spriteName = "Btn_CopyUI_MainCopyOpen1";
		}
		else
		{
			component.spriteName = "Btn_CopyUI_MainCopyUnopen1";
			component2.spriteName = "Btn_CopyUI_MainCopyUnopen1";
		}
	}

	public void RefreshPoint(int star)
	{
		UISprite component = this.norBG.GetComponent<UISprite>();
		UISprite component2 = this.bosBG.GetComponent<UISprite>();
		if (star != 0)
		{
			component.spriteName = "Btn_CopyUI_MainCopyOpen1";
			component2.spriteName = "Btn_CopyUI_MainCopyOpen1";
		}
		else
		{
			component.spriteName = "Btn_CopyUI_MainCopyUnopen1";
			component2.spriteName = "Btn_CopyUI_MainCopyUnopen1";
		}
	}

	public void SetPoointColor()
	{
		UISprite component = this.norBG.GetComponent<UISprite>();
		UISprite component2 = this.bosBG.GetComponent<UISprite>();
		component.spriteName = "Btn_CopyUI_MainCopyOpen1";
		component2.spriteName = "Btn_CopyUI_MainCopyOpen1";
	}

	public void InitStars(LevelConfig conf = null)
	{
		if (conf != null)
		{
			this.levelId = conf.id;
			this.starLevelCOnfig = conf;
		}
	}

	public void StartEfect(bool color = true)
	{
		this.needChangeColor = color;
		this.needScale = ((!color) ? 1.05f : 1.2f);
		GameObject gameObject = base.gameObject.AddChild(this.effectXJ);
		gameObject.SetActive(true);
		gameObject.transform.localScale = new Vector3(this.needScale, this.needScale, this.needScale);
		this.effectXJList.Add(gameObject);
		base.Invoke("ReAddEffect", 0.2f);
	}

	public void ReAddEffect()
	{
		GameObject gameObject = base.gameObject.AddChild(this.effectXJ);
		gameObject.SetActive(true);
		gameObject.transform.localScale = new Vector3(this.needScale, this.needScale, this.needScale);
		this.effectXJList.Add(gameObject);
		base.Invoke("ReAddEffectAgain", 0.2f);
	}

	public void ReAddEffectAgain()
	{
		GameObject gameObject = base.gameObject.AddChild(this.effectXJ);
		gameObject.SetActive(true);
		gameObject.transform.localScale = new Vector3(this.needScale, this.needScale, this.needScale);
		this.effectXJList.Add(gameObject);
		Solarmax.Singleton<AudioManger>.Get().PlayCapture(base.gameObject.transform.localPosition);
		base.Invoke("ReAddEffectAgainAndAgain", 0.2f);
	}

	public void ReAddEffectAgainAndAgain()
	{
		if (this.needChangeColor)
		{
			this.starBG.SetActive(this.needChangeColor);
			GameObject gameObject = base.gameObject.AddChild(this.effectGuangquan);
			gameObject.SetActive(true);
		}
		foreach (GameObject obj in this.effectXJList)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.effectXJList.Clear();
	}

	public void StarBGFadeOut()
	{
	}

	private void Update()
	{
		if (this.starBgFade)
		{
			this.fadeDelta -= Time.deltaTime;
			if (this.fadeDelta > 0f)
			{
				if (this.fadeEveryDelta > 0.03f)
				{
					this.starBG.GetComponent<UISprite>().color -= new Color(0f, 0f, 0f, 0.05f);
				}
			}
			else
			{
				this.fadeDelta = 0.6f;
				this.fadeEveryDelta = 0f;
				this.starBgFade = false;
				this.starBG.SetActive(false);
			}
		}
	}

	public GameObject threeStarts;

	public GameObject fourStarts;

	public GameObject norBG;

	public GameObject bosBG;

	private const int DEFAULT_STARS = 3;

	private int nameKey;

	public string levelId;

	public LevelConfig starLevelCOnfig;

	public bool IsUnLock;

	public GameObject greekLetter;

	public GameObject starName;

	public GameObject effectXJ;

	public GameObject effectGuangquan;

	public GameObject starBG;

	private bool needChangeColor = true;

	private float needScale = 1.2f;

	private List<GameObject> effectXJList = new List<GameObject>();

	private bool starBgFade;

	private float fadeDelta = 0.6f;

	private float fadeEveryDelta;
}
