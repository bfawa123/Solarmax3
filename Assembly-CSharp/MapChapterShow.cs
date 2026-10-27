using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class MapChapterShow : MonoBehaviour
{
	private void FadeIn(bool continueFade = false)
	{
		base.gameObject.SetActive(true);
		if (!this.immediately)
		{
			for (int i = 0; i < this.tweenList.Count; i++)
			{
				TweenScale tweenScale = this.tweenList[i];
				Vector3 localScale = tweenScale.gameObject.transform.localScale;
				tweenScale.ResetToBeginning();
				tweenScale.from = ((!continueFade) ? Vector3.zero : localScale);
				tweenScale.to = Vector3.one;
				tweenScale.duration = (1f - tweenScale.from.magnitude / Vector3.one.magnitude) * 0.5f;
				tweenScale.duration += 0.01f;
				tweenScale.Play(true);
			}
		}
		else
		{
			this.ManualFade(1f);
		}
		if (this.tweenList.Count == 0 || this.immediately)
		{
			this.OnFinished();
		}
	}

	private void FadeOut(bool continueFade = false)
	{
		this.isFadeOut = true;
		base.gameObject.SetActive(true);
		if (!this.immediately)
		{
			for (int i = 0; i < this.tweenList.Count; i++)
			{
				TweenScale tweenScale = this.tweenList[i];
				Vector3 localScale = tweenScale.gameObject.transform.localScale;
				tweenScale.ResetToBeginning();
				tweenScale.from = ((!continueFade) ? Vector3.one : localScale);
				tweenScale.to = Vector3.zero;
				tweenScale.duration = (1f - tweenScale.from.magnitude / Vector3.one.magnitude) * 0.5f;
				tweenScale.duration += 0.01f;
				tweenScale.Play(true);
			}
		}
		else
		{
			this.ManualFade(0f);
		}
		if (this.tweenList.Count == 0 || this.immediately)
		{
			this.OnFinished();
		}
	}

	public void ManualFade(float v)
	{
		if (base.gameObject.activeSelf == (v == 0f))
		{
			base.gameObject.SetActive(v != 0f);
		}
		if (!this.isPlayingTransferAnimation)
		{
			for (int i = 0; i < this.tweenList.Count; i++)
			{
				TweenScale tweenScale = this.tweenList[i];
				if (tweenScale.enabled)
				{
					tweenScale.enabled = false;
				}
				tweenScale.value = v * Vector3.one;
			}
			for (int j = 0; j < this.tweenAlphaList.Count; j++)
			{
				TweenAlpha tweenAlpha = this.tweenAlphaList[j];
				if (tweenAlpha.enabled)
				{
					tweenAlpha.enabled = false;
				}
				tweenAlpha.value = v;
			}
		}
		if (this.selectNumCircle != null)
		{
			TweenScale tweenScale = this.selectNumCircle;
			if (tweenScale.enabled)
			{
				tweenScale.enabled = false;
			}
			tweenScale.value = v * Vector3.one;
		}
		if (this.selectNumCircleEffect != null)
		{
			TweenScale tweenScale = this.selectNumCircleEffect;
			if (tweenScale.enabled)
			{
				tweenScale.enabled = false;
			}
			tweenScale.value = v * Vector3.one;
		}
	}

	public void OnFinished()
	{
		if (this.isFadeOut)
		{
			this.isFadeOut = false;
			if (this.isSwitch)
			{
				this.isSwitch = false;
				if (this.skyGo.transform.childCount > 0)
				{
					this.skyGo.transform.DestroyChildren();
				}
				this.DrawMap(this.showChapterId);
				this.FadeIn(false);
			}
			else
			{
				base.SendMessageUpwards("OnMapShowFadeOut", SendMessageOptions.RequireReceiver);
			}
		}
	}

	public void Switch(string chapterid, bool imme = false)
	{
		this.immediately = imme;
		if (!this.showChapterId.Equals(chapterid))
		{
			this.isSwitch = true;
			this.showChapterId = chapterid;
			this.FadeOut(false);
		}
		else
		{
			this.FadeIn(true);
		}
	}

	private void DrawMap(string chapterID)
	{
		this.chapterId = chapterID;
		this.tweenList.Clear();
		this.tweenAlphaList.Clear();
		this.lineTweenColor.Clear();
		if (!string.IsNullOrEmpty(this.chapterId))
		{
			ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(this.chapterId);
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterId);
			if (chapterInfo != null && data != null)
			{
				bool unLock = !chapterInfo.isWattingUnlock && chapterInfo.unLock;
				this.CreateBG(data, unLock);
				foreach (KeyValuePair<string, OneChapterPointConfig> pair in data.oneChapterPointConfigs)
				{
					LevelConfig data2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(pair.Value.level);
					Vector3 pos = new Vector3(pair.Value.x, pair.Value.y, pair.Value.z);
					Vector3 scale = new Vector3(pair.Value.size, pair.Value.size, 1f);
					int levelStar = this.GetLevelStar(pair.Value.level, chapterInfo);
					bool bSecLine = false;
					if (data2 != null)
					{
						bSecLine = (data2.mainLine <= 0);
					}
					this.CreateOne(data2, levelStar, bSecLine, pos, scale, pair, chapterInfo.unLock);
				}
				foreach (OneChapterLineConfig oneChapterLineConfig in data.oneChapterLineConfigs)
				{
					OneChapterPointConfig point = data.GetPoint(oneChapterLineConfig.point1);
					OneChapterPointConfig point2 = data.GetPoint(oneChapterLineConfig.point2);
					Vector3 startPos = new Vector3(point.x, point.y, point.z);
					Vector3 endPos = new Vector3(point2.x, point2.y, point2.z);
					int levelStar2 = this.GetLevelStar(point.level, chapterInfo);
					int levelStar3 = this.GetLevelStar(point2.level, chapterInfo);
					string spriteName = "Line_CopyUI_UnopenState1";
					this.CreateLine(startPos, endPos, spriteName);
				}
				this.SetChapterInfo();
			}
		}
		this.galaxyTweenAlpa.Play(true);
	}

	public void ShowBuyBtn()
	{
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterId);
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(this.chapterId);
		if (data.costGold > 0 && !Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(data.id))
		{
			if (this.buyBtnInstance == null)
			{
				this.buyBtnInstance = this.skyGo.AddChild(this.buyBtn);
			}
			this.buyBtnInstance.transform.localPosition = new Vector3(675.68f, -224.6f, 0f);
			this.buyBtnInstance.SetActive(true);
			Transform transform = this.buyBtnInstance.transform.Find("TableBuyBtn/NUM");
			UILabel component = transform.gameObject.GetComponent<UILabel>();
			component.text = data.costGold.ToString();
		}
	}

	public void ShowChapterInfo(bool visible)
	{
		this.star.SetActive(visible);
		if (visible)
		{
			ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(this.chapterId);
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterId);
			this.starLabel.text = string.Format("{0}/{1}", chapterInfo.star, chapterInfo.allstar);
			this.chapterName.text = LanguageDataProvider.GetValue(data.name);
		}
		else
		{
			this.starLabel.text = string.Empty;
			this.chapterName.text = string.Empty;
		}
	}

	public bool NeedBuy()
	{
		return this.needBuy;
	}

	public int GetLevelStar(string levelId, ChapterInfo chapterInfo)
	{
		int result = 0;
		foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
		{
			if (levelId == chapterLevelGroup.GetLevel(0).id)
			{
				result = AchievementModel.GetCompletedStars(chapterLevelGroup.groupID);
				return result;
			}
		}
		return result;
	}

	private void CreateBG(ChapterConfig config, bool unLock = true)
	{
		GameObject gameObject = this.skyGo.AddChild(this.galaxyBg);
		gameObject.SetActive(unLock);
		this.chaptergalaxyBG = gameObject;
		this.galaxyTweenColor = gameObject.GetComponent<TweenColor>();
		this.galaxyBgTweenAlpha = gameObject.GetComponent<TweenAlpha>();
		gameObject.gameObject.GetComponent<UITexture>().color = Vector4.one;
		this.galaxyName = "gameres/texture_4/galaxyicon/" + config.starChart;
		ResourcesUtil.SetStarChartTexture(gameObject.GetComponent<UITexture>(), config.starChart);
		this.tweenAlphaList.Add(gameObject.GetComponent<TweenAlpha>());
		if (unLock)
		{
		}
	}

	private void CreateOne(LevelConfig level, int levelStar, bool bSecLine, Vector3 pos, Vector3 scale, KeyValuePair<string, OneChapterPointConfig> pair, bool unLock = true)
	{
		GameObject gameObject = this.skyGo.AddChild(this.startsItemTemplate);
		if (level != null)
		{
			gameObject.name = level.id;
		}
		gameObject.SetActive(true);
		gameObject.transform.localPosition = pos;
		gameObject.transform.localScale = scale;
		LobbyChapterWindowCellStarsItemTemPlate component = gameObject.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
		if (component != null)
		{
			component.SetLevel(bSecLine, levelStar, scale, unLock);
		}
		if (pair.Value.greekLetter != string.Empty && unLock)
		{
			Vector3 greekPos = new Vector3(pair.Value.greekPosX, pair.Value.greekPosY, 0f);
			component.SetGreekLetter(greekPos, pair.Value.greekLetter);
		}
		if (pair.Value.starName > 0 && unLock)
		{
			Vector3 starNamePos = new Vector3(pair.Value.starNamePosX, pair.Value.starNamePosY, 0f);
			if (pair.Value.greekLetter != string.Empty)
			{
				component.SetStarName(starNamePos, pair.Value.starName, pair.Value.greekPosX);
			}
			else
			{
				component.SetStarName(starNamePos, pair.Value.starName);
			}
		}
		this.tweenList.Add(gameObject.GetComponent<TweenScale>());
	}

	public void ReflushLanguange()
	{
		for (int i = 0; i < this.tweenList.Count; i++)
		{
			LobbyChapterWindowCellStarsItemTemPlate component = this.tweenList[i].gameObject.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
			if (component != null)
			{
				component.SetStarName();
			}
		}
		if (this.buyBtnInstance != null)
		{
			Transform transform = this.buyBtnInstance.transform.Find("TableBuyBtn/Label");
			UILabel component2 = transform.gameObject.GetComponent<UILabel>();
			component2.text = LanguageDataProvider.GetValue(2053);
		}
	}

	private void CreateLine(Vector3 StartPos, Vector3 EndPos, string spriteName)
	{
		GameObject gameObject = this.skyGo.AddChild(this.lineTemplate);
		gameObject.SetActive(true);
		UISprite component = gameObject.GetComponent<UISprite>();
		if (component == null)
		{
			return;
		}
		gameObject.transform.localScale = new Vector3(1f, 0.2f, 1f);
		Vector3 localPosition = (StartPos + EndPos) * 0.5f;
		gameObject.transform.localPosition = localPosition;
		float num = Vector3.Distance(StartPos, EndPos);
		component.width = (int)num;
		component.spriteName = spriteName;
		Vector3 zero = Vector3.zero;
		Vector3 vector = EndPos - StartPos;
		zero.z = Mathf.Atan2(vector.y, vector.x) * 180f / 3.1415927f;
		gameObject.transform.eulerAngles = zero;
		this.tweenAlphaList.Add(gameObject.GetComponent<TweenAlpha>());
		this.lineTweenColor.Add(gameObject.GetComponent<TweenColor>());
	}

	public void moveSelectEffectPoint(string groupId, float dt)
	{
		string text = groupId + 0;
		if (!string.IsNullOrEmpty(text))
		{
			Transform transform = this.skyGo.transform.Find(text);
			if (transform == null)
			{
				return;
			}
			Vector3 localPosition = transform.localPosition;
			Vector3 localPosition2 = this.effectPoint.transform.localPosition;
			this.effectPoint.transform.localPosition = Vector3.Lerp(localPosition2, localPosition, dt);
		}
	}

	public void refreshLevelPointEffect()
	{
		string id = Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel.id;
		if (!string.IsNullOrEmpty(id))
		{
			Transform transform = this.skyGo.transform.Find(id);
			if (transform == null)
			{
				return;
			}
			LobbyChapterWindowCellStarsItemTemPlate component = transform.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
			if (component != null)
			{
				int levelStar = this.GetLevelStar(id, Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter);
				component.RefreshPoint(levelStar);
			}
		}
	}

	public void ChangeGalaxyPointColor(string levelId)
	{
		if (!string.IsNullOrEmpty(levelId))
		{
			Transform transform = this.skyGo.transform.Find(levelId);
			if (transform == null)
			{
				return;
			}
			LobbyChapterWindowCellStarsItemTemPlate component = transform.gameObject.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
			if (component != null)
			{
				component.SetPoointColor();
			}
		}
	}

	public void EnableEffect(bool enable)
	{
		this.effectPoint.SetActive(enable);
	}

	public void SetTransferAnimation(bool play)
	{
		this.isPlayingTransferAnimation = play;
	}

	public void StartUnLockAnimation()
	{
		this.unLockDelta = 0f;
		this.starIndex = 0;
		this.effectShowDelta = 0f;
		this.bUnLockChapter = true;
		this.widthList.Clear();
		this.goList1.Clear();
		this.UnLockChapterAnimation();
		this.galaxyTweenAlpa.enabled = true;
	}

	public void StartScaleAnimation()
	{
		this.delta = 0f;
		this.scaleDelta = 0f;
		this.starIndex = 0;
		this.effectShowDelta = 0.2f;
		this.startEffect = true;
		this.startScale = false;
		this.smallScale = false;
		this.bUnLockChapter = false;
		this.waittinFireEvent = false;
		this.isFadeout = true;
		this.blink = false;
		this.isPlayingTransferAnimation = true;
		this.firstBackg = false;
		this.secondBackg = false;
		this.orderList.Clear();
		for (int i = 0; i < this.tweenList.Count; i++)
		{
			this.orderList.Add(i);
		}
		this.orderList = this.ListRandom(this.orderList);
		this.goList.Clear();
		this.galaxyBackList.Clear();
		this.galaxyTweenAlpa.enabled = false;
	}

	public void OnClickBuyBtn()
	{
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(this.chapterId);
		if (!chapterInfo.unLock)
		{
			Tips.Make(LanguageDataProvider.GetValue(1140));
			return;
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnShowBuyChapterEvent, new object[0]);
	}

	public void RefreshChapterAfterBuySuccess()
	{
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(this.chapterId);
		this.needBuy = false;
		this.unlockTip.SetActive(false);
		this.star.SetActive(true);
		this.unlockCondition1.text = string.Empty;
		this.unlockCondition2.text = string.Empty;
		this.starLabel.text = string.Format("{0}/{1}", chapterInfo.star, chapterInfo.allstar);
		UnityEngine.Object.Destroy(this.buyBtnInstance);
	}

	private void Update()
	{
		if (this.startEffect)
		{
			this.delta += Time.deltaTime;
			if (this.delta < 5f)
			{
				this.effectShowDelta += Time.deltaTime;
				if (this.effectShowDelta > 0.2f)
				{
					this.effectShowDelta = 0f;
					if (this.starIndex < this.tweenList.Count)
					{
						if (this.starIndex < this.orderList.Count && this.orderList[this.starIndex] < this.tweenList.Count)
						{
							LobbyChapterWindowCellStarsItemTemPlate component = this.tweenList[this.orderList[this.starIndex]].gameObject.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
							if (component != null)
							{
								component.StartEfect(true);
							}
						}
						if (this.orderList[this.starIndex] < this.lineTweenColor.Count)
						{
							TweenColor tweenColor = this.lineTweenColor[this.orderList[this.starIndex]];
							LineFillAnimationBehaviour component2 = tweenColor.gameObject.GetComponent<LineFillAnimationBehaviour>();
							component2.Fill(false);
						}
						this.starIndex++;
					}
					else
					{
						this.delta = 5f;
						this.firstBackg = true;
					}
				}
			}
			else if (this.firstBackg && this.delta > 5.2f)
			{
				this.firstBackg = false;
				this.secondBackg = true;
				GameObject gameObject = this.skyGo.gameObject.AddChild(this.galaxyBg);
				gameObject.SetActive(true);
				gameObject.transform.localPosition = this.galaxyTweenColor.gameObject.transform.localPosition;
				gameObject.GetComponent<UITexture>().mainTexture = ResourcesUtil.GetUITexture(this.galaxyName);
				this.galaxyBackList.Add(gameObject);
				GalaxyBgBehaviour component3 = gameObject.gameObject.GetComponent<GalaxyBgBehaviour>();
				component3.StartAnimation();
			}
			else if (this.secondBackg && this.delta > 5.35f)
			{
				this.secondBackg = false;
				GameObject gameObject2 = this.skyGo.gameObject.AddChild(this.galaxyBg);
				gameObject2.SetActive(true);
				gameObject2.transform.localPosition = this.galaxyTweenColor.gameObject.transform.localPosition;
				gameObject2.GetComponent<UITexture>().mainTexture = ResourcesUtil.GetUITexture(this.galaxyName);
				this.galaxyBackList.Add(gameObject2);
				GalaxyBgBehaviour component4 = gameObject2.gameObject.GetComponent<GalaxyBgBehaviour>();
				component4.StartAnimation();
			}
			else if (this.delta >= 6.1f)
			{
				this.blink = true;
				this.delta = 0f;
				this.startEffect = false;
			}
		}
		if (this.blink)
		{
			this.delta += Time.deltaTime;
			if (this.delta < 0f)
			{
				this.delta = 0.1f;
			}
			else
			{
				this.blink = false;
				this.delta = 0f;
				this.blinkFadeOut = true;
			}
		}
		if (this.blinkFadeOut)
		{
			this.delta += Time.deltaTime;
			if (this.delta < 0.2f)
			{
				this.delta = 0.2f;
			}
			else if ((double)this.delta > 0.22)
			{
				this.blinkFadeOut = false;
				this.blink = false;
				this.delta = 0f;
				this.starIndex = 0;
				this.effectShowDelta = 0f;
				this.startEffect = false;
				this.startScale = true;
			}
		}
		if (this.startScale)
		{
			this.delta += Time.deltaTime;
			this.scaleDelta += Time.deltaTime;
			if (this.delta <= 0f)
			{
				if (this.delta <= 0.15f && this.scaleDelta >= 0.03f)
				{
					this.scaleDelta = 0f;
					this.skyGo.transform.localScale += new Vector3(0.007f, 0.007f, 0f);
				}
			}
			else
			{
				foreach (GameObject obj in this.galaxyBackList)
				{
					UnityEngine.Object.Destroy(obj);
				}
				this.galaxyBackList.Clear();
				this.delta = 0f;
				this.scaleDelta = 0f;
				this.startScale = false;
				this.smallScale = true;
				for (int i = 0; i < this.lineTweenColor.Count; i++)
				{
					TweenColor tweenColor2 = this.lineTweenColor[i];
					LineFillAnimationBehaviour component5 = tweenColor2.gameObject.GetComponent<LineFillAnimationBehaviour>();
					component5.Fade(false);
				}
				for (int j = 0; j < this.tweenList.Count; j++)
				{
					LobbyChapterWindowCellStarsItemTemPlate component6 = this.tweenList[j].gameObject.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
					if (component6 != null)
					{
						component6.StarBGFadeOut();
					}
				}
			}
		}
		if (this.smallScale)
		{
			this.delta += Time.deltaTime;
			this.scaleDelta += Time.deltaTime;
			if (this.delta <= 0.4f)
			{
				if (this.delta <= 0.4f && this.scaleDelta >= 0.04f)
				{
					this.scaleDelta = 0f;
					Vector3 vector = this.galaxyTweenAlpa.gameObject.transform.localScale;
					vector += new Vector3(-0.016f, -0.016f, -0.016f);
					if (vector.x < 1f)
					{
						vector = Vector3.one;
					}
					this.galaxyTweenAlpa.gameObject.transform.localScale = vector;
				}
			}
			else
			{
				this.smallScale = false;
				this.delta = 0f;
				this.scaleDelta = 0f;
				this.waittinFireEvent = true;
				for (int k = 0; k < this.tweenList.Count; k++)
				{
					TweenColor component7 = this.tweenList[k].gameObject.transform.GetChild(0).GetComponent<TweenColor>();
					component7.gameObject.GetComponent<UISprite>().spriteName = "Btn_CopyUI_MainCopyUnopen1";
				}
			}
		}
		if (this.waittinFireEvent)
		{
			this.delta += Time.deltaTime;
			if (this.delta > 0.6f)
			{
				this.delta = 0f;
				this.waittinFireEvent = false;
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.AfterTransferChapter, null);
				foreach (GameObject gameObject3 in this.goList)
				{
					if (gameObject3 != null)
					{
						UnityEngine.Object.Destroy(gameObject3);
					}
				}
			}
		}
		if (this.bUnLockChapter)
		{
			this.effectShowDelta += Time.deltaTime;
			if (this.effectShowDelta > 0.1f)
			{
				this.effectShowDelta = 0f;
				if (this.unlockIndex < this.tweenList.Count)
				{
					LobbyChapterWindowCellStarsItemTemPlate component8 = this.tweenList[this.unlockIndex].gameObject.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
					component8.StartEfect(false);
					this.unlockIndex++;
				}
			}
			this.unLockDelta += Time.deltaTime;
			if (this.unLockDelta > (float)this.tweenList.Count * 0.12f)
			{
				this.delta = 0f;
				this.unlockIndex = 0;
				this.unLockDelta = 0f;
				this.bUnLockChapter = false;
				this.unlockLineAnimator = true;
			}
		}
		if (this.unlockLineAnimator)
		{
			if (this.delta > 0.05f)
			{
				if (this.unlockIndex < this.lineTweenColor.Count)
				{
					TweenColor tweenColor3 = this.lineTweenColor[this.unlockIndex++];
					LineFillAnimationBehaviour component9 = tweenColor3.gameObject.GetComponent<LineFillAnimationBehaviour>();
					component9.Fill(true);
				}
				else
				{
					this.delta = 0f;
					this.unlockIndex = 0;
					this.unlockLineAnimator = false;
					this.beforeUnlockBackgAnimator = true;
				}
			}
			else
			{
				this.delta += Time.deltaTime;
			}
		}
		if (this.beforeUnlockBackgAnimator)
		{
			this.delta += Time.deltaTime;
			if (this.delta > 0.6f)
			{
				this.delta = 0f;
				this.chaptergalaxyBG.SetActive(true);
				this.chaptergalaxyBG.GetComponent<Animator>().enabled = true;
				this.chaptergalaxyBG.GetComponent<Animator>().Play("ChapterBGAlpha");
				this.beforeUnlockBackgAnimator = false;
				this.unlockBackgAnimator = true;
				this.firstUnlockBackg = true;
			}
		}
		if (this.unlockBackgAnimator)
		{
			this.delta += Time.deltaTime;
			if (this.delta > 0.4f && this.firstUnlockBackg)
			{
				this.firstUnlockBackg = false;
				GameObject gameObject4 = this.skyGo.gameObject.AddChild(this.galaxyBg);
				gameObject4.SetActive(true);
				gameObject4.transform.localPosition = this.galaxyTweenColor.gameObject.transform.localPosition;
				gameObject4.GetComponent<UITexture>().mainTexture = ResourcesUtil.GetUITexture(this.galaxyName);
				this.galaxyBackList.Add(gameObject4);
				GalaxyBgBehaviour component10 = gameObject4.gameObject.GetComponent<GalaxyBgBehaviour>();
				component10.StartUnlockAnimation();
				this.secondUnlockBackg = true;
			}
			if (this.delta > 0.5f && this.secondUnlockBackg)
			{
				this.secondUnlockBackg = false;
				GameObject gameObject5 = this.skyGo.gameObject.AddChild(this.galaxyBg);
				gameObject5.SetActive(true);
				gameObject5.transform.localPosition = this.galaxyTweenColor.gameObject.transform.localPosition;
				gameObject5.GetComponent<UITexture>().mainTexture = ResourcesUtil.GetUITexture(this.galaxyName);
				this.galaxyBackList.Add(gameObject5);
				GalaxyBgBehaviour component11 = gameObject5.gameObject.GetComponent<GalaxyBgBehaviour>();
				component11.StartUnlockAnimation();
			}
			if (this.delta > 1.5f)
			{
				this.delta = 0f;
				this.firstUnlockBackg = false;
				this.secondUnlockBackg = false;
				this.unlockBackgAnimator = false;
				foreach (GameObject gameObject6 in this.goList1)
				{
					if (gameObject6 != null)
					{
						UnityEngine.Object.Destroy(gameObject6);
					}
				}
				this.isPlayingTransferAnimation = false;
				this.chaptergalaxyBG.GetComponent<Animator>().enabled = false;
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.ChapterUnLockFinish, null);
			}
		}
	}

	private List<int> ListRandom(List<int> myList)
	{
		System.Random random = new System.Random();
		List<int> list = new List<int>();
		for (int i = 0; i < myList.Count; i++)
		{
			int num = random.Next(0, myList.Count - 1);
			if (num != i)
			{
				int value = myList[i];
				myList[i] = myList[num];
				myList[num] = value;
			}
		}
		return myList;
	}

	private void UnLockChapterAnimation()
	{
		this.unlockIndex = 0;
		for (int i = 0; i < this.tweenAlphaList.Count; i++)
		{
			GameObject gameObject = this.tweenAlphaList[i].gameObject;
			UISprite component = gameObject.GetComponent<UISprite>();
			if (component == null)
			{
				this.widthList.Add(0);
			}
			else
			{
				int width = component.width;
				this.widthList.Add(width);
			}
		}
		this.isPlayingTransferAnimation = true;
	}

	private void UnlockTween()
	{
		TweenAlpha tweenAlpha = this.tweenAlphaList[this.unlockIndex];
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0.1f;
		tweenAlpha.duration = 0.5f;
		tweenAlpha.Play(true);
		GameObject gameObject = this.tweenAlphaList[this.unlockIndex++].gameObject;
		UISprite component = gameObject.GetComponent<UISprite>();
		if (component == null)
		{
			this.widthList.Add(0);
			return;
		}
		int width = component.width;
		this.widthList.Add(width);
	}

	public void ChangeLanguage()
	{
		this.SetChapterInfo();
	}

	public void SetChapterInfo()
	{
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterId);
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(this.chapterId);
		bool flag = chapterInfo.unLock && (data.costGold <= 0 || Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(data.id));
		this.chapterName.text = LanguageDataProvider.GetValue(data.name);
		this.unlockTip.SetActive(!flag);
		this.star.SetActive(flag);
		string arg = string.Empty;
		if (Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data.dependChapter) != null)
		{
			arg = LanguageDataProvider.GetValue(Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data.dependChapter).name);
		}
		if (data.costGold > 0 && !Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(data.id))
		{
			this.unlockCondition1.text = string.Format("·{0}{1}", arg, LanguageDataProvider.GetValue(206));
			this.unlockCondition2.text = string.Format("·{0}", string.Format(LanguageDataProvider.GetValue(2184), data.costGold));
			if (chapterInfo.unLock)
			{
				this.unlockCondition1.text = string.Empty;
				this.ShowBuyBtn();
				this.needBuy = true;
			}
		}
		else
		{
			if (chapterInfo.unLock)
			{
				this.unlockCondition1.text = string.Empty;
				this.unlockCondition2.text = string.Empty;
				this.starLabel.text = string.Format("{0}/{1}", chapterInfo.star, chapterInfo.allstar);
			}
			else
			{
				this.unlockCondition1.text = string.Format("·{0}{1}", arg, LanguageDataProvider.GetValue(206));
				this.unlockCondition2.text = string.Empty;
				this.starLabel.text = string.Format("{0}/{1}", chapterInfo.star, chapterInfo.allstar);
			}
			this.needBuy = false;
		}
	}

	public GameObject skyGo;

	public GameObject galaxyBg;

	public GameObject startsItemTemplate;

	public GameObject lineTemplate;

	public GameObject effectPoint;

	public GameObject solarEffect;

	public GameObject chapterUnlockEffectPoint;

	public GameObject buyBtn;

	public UILabel chapterName;

	public GameObject unlockTip;

	public UILabel unlockCondition1;

	public UILabel unlockCondition2;

	public GameObject star;

	public UILabel starLabel;

	private GameObject buyBtnInstance;

	public TweenScale selectNumCircle;

	public TweenScale selectNumCircleEffect;

	public float positionScale = 1f;

	public TweenAlpha galaxyTweenAlpa;

	private TweenColor galaxyTweenColor;

	private TweenAlpha galaxyBgTweenAlpha;

	private string showChapterId = string.Empty;

	private List<TweenScale> tweenList = new List<TweenScale>();

	private List<TweenAlpha> tweenAlphaList = new List<TweenAlpha>();

	private bool isFadeOut;

	private bool isSwitch;

	private bool immediately;

	private Vector2 zeroPointInSky;

	private string chapterId;

	private const string GALAXY_TEXTURE_PATH = "gameres/texture_4/galaxyicon/";

	private GameObject background;

	private int starIndex;

	private float delta;

	private float scaleDelta;

	private float effectShowDelta;

	private bool startScale;

	private bool smallScale;

	private bool startEffect;

	private bool waittinFireEvent;

	private List<GameObject> goList = new List<GameObject>();

	private List<GameObject> goList1 = new List<GameObject>();

	private List<TweenColor> lineTweenColor = new List<TweenColor>();

	private bool isPlayingTransferAnimation;

	private bool blink;

	private float galaxyTweenTime;

	private TweenColor lineTc;

	private bool isFadeout = true;

	private bool blinkFadeOut;

	private List<GameObject> galaxyBackList = new List<GameObject>();

	private GameObject chaptergalaxyBG;

	private bool needBuy;

	private string galaxyName = string.Empty;

	private float unLockDelta = float.MaxValue;

	private bool bUnLockChapter;

	private List<int> orderList = new List<int>();

	private bool beforeFirstBackg;

	private bool firstBackg;

	private bool secondBackg;

	private bool firstUnlockBackg = true;

	private bool secondUnlockBackg = true;

	private bool unlockBackgAnimator;

	private bool unlockLineAnimator;

	private bool beforeUnlockBackgAnimator;

	private List<int> widthList = new List<int>();

	private int unlockIndex;

	private bool unlockChapterTween;
}
