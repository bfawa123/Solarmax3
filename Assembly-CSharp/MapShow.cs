using System;
using System.Collections.Generic;
using System.Globalization;
using Solarmax;
using UnityEngine;

public class MapShow : MonoBehaviour
{
	private void Awake()
	{
		if (this.blackSelectEffect != null)
		{
			this.blackSelectScales = new List<Vector3>();
			foreach (Transform transform in this.blackSelectEffect.transform.GetComponentsInChildren<Transform>(true))
			{
				this.blackSelectScales.Add(transform.localScale);
			}
		}
	}

	private void OnEnable()
	{
		if (this.chapterName != null)
		{
			this.chapterName.gameObject.SetActive(true);
			ChapterLevelGroup levelByIndex = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex);
			if (levelByIndex != null)
			{
				LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(levelByIndex.displayID);
				if (data != null)
				{
					this.chapterName.text = LanguageDataProvider.GetValue(data.levelName);
					this.levelName.text = string.Format(LanguageDataProvider.GetValue(2207), Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex + 1, Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.levelList.Count);
				}
			}
		}
		LevelDataHandler levelDataHandler = Solarmax.Singleton<LevelDataHandler>.Get();
		levelDataHandler.onLevelScore = (LevelDataHandler.OnLevelScore)Delegate.Combine(levelDataHandler.onLevelScore, new LevelDataHandler.OnLevelScore(this.RefreshScore));
	}

	private void OnDisable()
	{
		LevelDataHandler levelDataHandler = Solarmax.Singleton<LevelDataHandler>.Get();
		levelDataHandler.onLevelScore = (LevelDataHandler.OnLevelScore)Delegate.Remove(levelDataHandler.onLevelScore, new LevelDataHandler.OnLevelScore(this.RefreshScore));
	}

	private void SetZeroPoint()
	{
		this.zeroPointInSky = this.skyGo.transform.worldToLocalMatrix.MultiplyPoint(Vector2.zero);
	}

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
			for (int j = 0; j < this.tweenAlphaList.Count; j++)
			{
				TweenAlpha tweenAlpha = this.tweenAlphaList[j];
				float value = tweenAlpha.value;
				tweenAlpha.ResetToBeginning();
				tweenAlpha.from = value;
				tweenAlpha.to = 1f;
				tweenAlpha.duration = 0.5f;
				tweenAlpha.Play(true);
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
			TweenScale tweenScale = null;
			for (int i = 0; i < this.tweenList.Count; i++)
			{
				tweenScale = this.tweenList[i];
				Vector3 localScale = tweenScale.gameObject.transform.localScale;
				tweenScale.ResetToBeginning();
				tweenScale.from = ((!continueFade) ? Vector3.one : localScale);
				tweenScale.to = Vector3.zero;
				tweenScale.duration = (1f - tweenScale.from.magnitude / Vector3.one.magnitude) * 0.5f;
				tweenScale.duration += 0.01f;
				tweenScale.Play(true);
			}
			if (tweenScale != null)
			{
				global::Coroutine.DelayDo(tweenScale.delay, new EventDelegate(delegate()
				{
					this.OnFinished();
				}));
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

	public void AlphaFadeOut(float duration, EventDelegate ed = null)
	{
		for (int i = 0; i < this.tweenOrbitsList.Count; i++)
		{
			TweenScale tweenScale = this.tweenOrbitsList[i];
			tweenScale.enabled = true;
			Vector3 localScale = tweenScale.gameObject.transform.localScale;
			tweenScale.ResetToBeginning();
			tweenScale.from = localScale;
			tweenScale.to = Vector3.zero;
			tweenScale.duration = duration;
			tweenScale.Play(true);
		}
	}

	public void AlphaFadeIn(float duration, EventDelegate ed = null)
	{
		for (int i = 0; i < this.tweenOrbitsList.Count; i++)
		{
			TweenScale tweenScale = this.tweenOrbitsList[i];
			tweenScale.enabled = true;
			Vector3 localScale = tweenScale.gameObject.transform.localScale;
			tweenScale.ResetToBeginning();
			tweenScale.from = localScale;
			tweenScale.to = Vector3.one;
			tweenScale.duration = duration;
			tweenScale.Play(true);
		}
	}

	public void MapFadeOut(float duration)
	{
		if (this.chapterName != null)
		{
			this.chapterName.gameObject.SetActive(false);
		}
		this.StroceEnable(true);
		for (int i = 0; i < this.tweenAlphaList.Count; i++)
		{
			TweenAlpha tweenAlpha = this.tweenAlphaList[i];
			tweenAlpha.enabled = true;
			tweenAlpha.gameObject.SetActive(true);
			float value = tweenAlpha.value;
			tweenAlpha.ResetToBeginning();
			tweenAlpha.from = value;
			tweenAlpha.to = 0f;
			tweenAlpha.duration = duration;
			tweenAlpha.Play(true);
		}
		for (int j = 0; j < this.tweenList.Count; j++)
		{
			TweenScale tweenScale = this.tweenList[j];
			tweenScale.enabled = true;
			tweenScale.gameObject.SetActive(true);
			Vector3 localScale = tweenScale.gameObject.transform.localScale;
			tweenScale.ResetToBeginning();
			tweenScale.from = localScale;
			tweenScale.to = Vector3.zero;
			tweenScale.duration = duration;
			tweenScale.Play(true);
		}
	}

	public void StroceEnable(bool enable)
	{
		this.stroceEnable = enable;
		if (!this.stroceEnable)
		{
			this.lastV = 0f;
		}
	}

	public void MapFadeIn(float duration)
	{
		this.StroceEnable(true);
		for (int i = 0; i < this.tweenAlphaList.Count; i++)
		{
			TweenAlpha tweenAlpha = this.tweenAlphaList[i];
			float value = tweenAlpha.value;
			tweenAlpha.ResetToBeginning();
			tweenAlpha.from = value;
			tweenAlpha.to = 1f;
			tweenAlpha.duration = duration;
			tweenAlpha.Play(true);
		}
		for (int j = 0; j < this.tweenList.Count; j++)
		{
			TweenScale tweenScale = this.tweenList[j];
			Vector3 localScale = tweenScale.gameObject.transform.localScale;
			tweenScale.ResetToBeginning();
			tweenScale.from = localScale;
			tweenScale.to = Vector3.one;
			tweenScale.duration = duration;
			tweenScale.Play(true);
		}
	}

	public void ManualFade(float v)
	{
		if (this.lastV != v && !this.stroceEnable)
		{
			if (base.gameObject.activeSelf == (v == 0f))
			{
				base.gameObject.SetActive(v != 0f);
			}
			this.lastV = v;
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
		if (this.blackSelectEffect != null && this.blackSelectEffect.activeSelf)
		{
			int num = 0;
			foreach (Transform transform in this.blackSelectEffect.transform.GetComponentsInChildren<Transform>(true))
			{
				transform.localScale = this.blackSelectScales[num++] * v;
			}
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
				this.DrawMap(this.showSceneId);
				this.FadeIn(false);
			}
			else
			{
				base.SendMessageUpwards("OnMapShowFadeOut", SendMessageOptions.RequireReceiver);
			}
		}
	}

	public void SelectMap()
	{
		this.FadeOut(false);
	}

	public void Switch(string sceneId, bool imme = false)
	{
		this.immediately = imme;
		if (!this.showSceneId.Equals(sceneId))
		{
			this.isSwitch = true;
			this.showSceneId = sceneId;
			this.FadeOut(false);
		}
		else
		{
			if (this.skyGo != null && this.skyGo.transform.childCount == 0)
			{
				this.DrawMap(sceneId);
			}
			this.FadeIn(true);
		}
	}

	private void DrawMap(string sceneId)
	{
		this.sceneId = sceneId;
		this.RealDrawMap();
	}

	private void RealDrawMap()
	{
		this.SetZeroPoint();
		this.tweenList.Clear();
		this.tweenAlphaList.Clear();
		this.tweenOrbitsList.Clear();
		this.orbitsBuildingItem.Clear();
		if (!string.IsNullOrEmpty(this.sceneId))
		{
			this.RefreshScore(Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID());
			MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.sceneId);
			if (data == null)
			{
				data = new MapConfig(this.sceneId);
				data.Load();
			}
			if (data != null)
			{
				if (data.mbcList != null)
				{
					for (int i = 0; i < data.mbcList.Count; i++)
					{
						MapBuildingConfig mapBuildingConfig = data.mbcList[i];
						MapNodeConfig data2 = Solarmax.Singleton<MapNodeConfigProvider>.Instance.GetData(mapBuildingConfig.type, mapBuildingConfig.size);
						if (data2 != null)
						{
							this.CreateOne(mapBuildingConfig.tag, data2.typeEnum, data2.size, new Vector2(mapBuildingConfig.x, mapBuildingConfig.y), mapBuildingConfig.camption, false, default(Vector2), default(Vector2));
							if (mapBuildingConfig.orbit != 0 && !this.CheckSameOrbits(mapBuildingConfig))
							{
								this.CreateOrbits(data2.typeEnum, mapBuildingConfig.orbit, mapBuildingConfig.camption, new Vector2(mapBuildingConfig.x, mapBuildingConfig.y), mapBuildingConfig);
							}
						}
					}
				}
				if (data.mlcList != null)
				{
					MapBuildingConfig mapBuildingConfig2 = null;
					for (int j = 0; j < data.mlcList.Count; j++)
					{
						MapLineConfig mapLineConfig = data.mlcList[j];
						MapBuildingConfig mapBuildingConfig4;
						MapBuildingConfig mapBuildingConfig3 = mapBuildingConfig4 = null;
						for (int k = 0; k < data.mbcList.Count; k++)
						{
							mapBuildingConfig2 = data.mbcList[k];
							if (mapBuildingConfig2.tag.Equals(mapLineConfig.point1))
							{
								mapBuildingConfig4 = mapBuildingConfig2;
							}
							if (mapBuildingConfig2.tag.Equals(mapLineConfig.point2))
							{
								mapBuildingConfig3 = mapBuildingConfig2;
							}
						}
						if (mapBuildingConfig4 != null && mapBuildingConfig3 != null)
						{
							this.CreateOne(mapBuildingConfig2.tag, 6, 1f, Vector2.zero, 0, false, new Vector2(mapBuildingConfig4.x, mapBuildingConfig4.y), new Vector2(mapBuildingConfig3.x, mapBuildingConfig3.y));
						}
					}
				}
			}
		}
		if (this.chapterName != null)
		{
			ChapterLevelGroup levelByIndex = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex);
			if (levelByIndex == null)
			{
				return;
			}
			LevelConfig data3 = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(levelByIndex.displayID);
			this.chapterName.text = LanguageDataProvider.GetValue(data3.levelName);
			this.levelName.text = string.Format(LanguageDataProvider.GetValue(2207), Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex + 1, Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.levelList.Count);
		}
	}

	private void RefreshScore(string groupId)
	{
		if (groupId != Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID())
		{
			return;
		}
		if (string.IsNullOrEmpty(this.sceneId))
		{
			return;
		}
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		ChapterLevelInfo level = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevel(this.sceneId);
		if (level != null)
		{
			LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(level.id);
			if (data == null)
			{
				return;
			}
			ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Instance.FindGroupLevel(data.levelGroup);
			if (chapterLevelGroup == null)
			{
				return;
			}
			string format = "{0}";
			if (this.levelScore != null)
			{
				this.levelScore.text = string.Format(format, chapterLevelGroup.Score);
			}
		}
	}

	private bool CheckSameOrbits(MapBuildingConfig bi)
	{
		if (bi.orbit == (int)RevolutionType.RT_Custom)
		{
			string text = string.IsNullOrEmpty(bi.equation) ? bi.orbitParam1 : bi.equation;
			string text2 = string.IsNullOrEmpty(bi.orbitXChoose) ? bi.orbitParam2 : bi.orbitXChoose;
			for (int i = 0; i < this.orbitsBuildingItem.Count; i++)
			{
				MapBuildingConfig mapBuildingConfig = this.orbitsBuildingItem[i];
				if (mapBuildingConfig != null && mapBuildingConfig.orbit == bi.orbit)
				{
					string text3 = string.IsNullOrEmpty(mapBuildingConfig.equation) ? mapBuildingConfig.orbitParam1 : mapBuildingConfig.equation;
					string text4 = string.IsNullOrEmpty(mapBuildingConfig.orbitXChoose) ? mapBuildingConfig.orbitParam2 : mapBuildingConfig.orbitXChoose;
					if (text3 == text && text4 == text2)
					{
						return true;
					}
				}
			}
			this.orbitsBuildingItem.Add(bi);
			return false;
		}
		Vector3 vector = Converter.ConvertVector3D(bi.orbitParam1);
		Vector3 vector2 = Converter.ConvertVector3D(bi.orbitParam2);
		float num = 0f;
		if (bi.orbit == 1)
		{
			num = Mathf.Sqrt(Mathf.Pow(bi.x - vector.x, 2f) + Mathf.Pow(bi.y - vector.y, 2f));
		}
		else if (bi.orbit == 4)
		{
			num = Mathf.Sqrt(Mathf.Pow(bi.x - vector.x, 2f) + Mathf.Pow(bi.y - vector.y, 2f)) + Mathf.Sqrt(Mathf.Pow(bi.x - vector2.x, 2f) + Mathf.Pow(bi.y - vector2.y, 2f));
		}
		float num2 = 0f;
		for (int i = 0; i < this.orbitsBuildingItem.Count; i++)
		{
			MapBuildingConfig mapBuildingConfig = this.orbitsBuildingItem[i];
			if (mapBuildingConfig.orbit == bi.orbit && mapBuildingConfig.orbitParam1.Equals(bi.orbitParam1) && mapBuildingConfig.orbitParam2.Equals(bi.orbitParam2))
			{
				if (bi.orbit == 2 || bi.orbit == 3)
				{
					if (mapBuildingConfig.orbitParam3.Equals(bi.orbitParam3) && mapBuildingConfig.orbitParam4.Equals(bi.orbitParam4))
					{
						return true;
					}
				}
				Vector3 vector3 = Converter.ConvertVector3D(mapBuildingConfig.orbitParam1);
				Vector3 vector4 = Converter.ConvertVector3D(mapBuildingConfig.orbitParam2);
				if (bi.orbit == 1)
				{
					num2 = Mathf.Sqrt(Mathf.Pow(mapBuildingConfig.x - vector3.x, 2f) + Mathf.Pow(mapBuildingConfig.y - vector3.y, 2f));
				}
				else if (bi.orbit == 4)
				{
					num2 = Mathf.Sqrt(Mathf.Pow(mapBuildingConfig.x - vector3.x, 2f) + Mathf.Pow(mapBuildingConfig.y - vector3.y, 2f)) + Mathf.Sqrt(Mathf.Pow(mapBuildingConfig.x - vector4.x, 2f) + Mathf.Pow(mapBuildingConfig.y - vector4.y, 2f));
				}
				if (Math.Abs(num2 - num) < 0.01f)
				{
					return true;
				}
			}
		}
		this.orbitsBuildingItem.Add(bi);
		return false;
	}

	private void CreateOne(string id, int nodeType, float scale, Vector2 position, int camp, bool vertical = false, Vector2 from = default(Vector2), Vector2 to = default(Vector2))
	{
		position = this.ChangePosToLocal(position) + this.offdistance;
		from = this.ChangePosToLocal(from) + this.offdistance;
		to = this.ChangePosToLocal(to) + this.offdistance;
		GameObject gameObject = this.skyGo.AddChild(this.copyGo);
		gameObject.SetActive(true);
		gameObject.name = "build-" + id;
		Transform transform = gameObject.transform.Find("effectroot");
		if (transform != null)
		{
			transform.localScale = scale * Vector3.one;
		}
		GameObject gameObject2 = gameObject.transform.Find("line").gameObject;
		gameObject2.SetActive(false);
		gameObject2 = gameObject.transform.Find("image").gameObject;
		gameObject2.SetActive(true);
		UISprite component = gameObject2.GetComponent<UISprite>();
		string text = string.Empty;
		switch (nodeType)
		{
		case 1:
			text = "planet_shape";
			break;
		case 2:
			text = "starbase_shape";
			break;
		case 3:
			text = "warp_shape";
			break;
		case 4:
			text = "tower_shape";
			break;
		case 5:
			text = "barrier_new_shape";
			break;
		case 6:
			text = "map_line";
			break;
		case 7:
			text = "master_shape";
			break;
		case 8:
			text = "magicstar_shape";
			break;
		case 9:
			text = "power_shape";
			break;
		case 10:
			text = "blackhole";
			break;
		case 11:
			text = "House_shape";
			break;
		case 12:
			text = "Arsenal_shape";
			break;
		case 13:
			text = "AircraftCarrier_shape";
			break;
		case 14:
			text = "Lasercannon_new_shape";
			break;
		case 15:
			text = "Attackship_shape";
			break;
		case 16:
			text = "Lifeship_shape";
			break;
		case 17:
			text = "Speedship_shape";
			break;
		case 18:
			text = "Captureship_shape";
			break;
		case 19:
			text = "AntiAttackship_shape";
			break;
		case 20:
			text = "AntiLifeship_shape";
			break;
		case 21:
			text = "AntiSpeedship_shape";
			break;
		case 22:
			text = "AntiCaptureship_shape";
			break;
		case 23:
			text = "magicstar_shape";
			break;
		case 24:
			text = "hiddenstar_shape";
			break;
		case 25:
			text = "fixedwarpdoor_shape";
			break;
		case 26:
			text = "planet_shape";
			break;
		case 27:
			text = "master_shape";
			break;
		case 28:
			text = "Captureship_shape";
			break;
		case 29:
			text = "AntiCaptureship_shape";
			break;
		case 30:
			text = "master_shape";
			break;
		case 31:
			text = "master_shape";
			break;
		case 32:
			text = "master_shape";
			break;
		case 33:
			text = "master_shape";
			break;
		case 34:
			text = "planetRandom_shape";
			break;
		case 35:
			text = "Speedship_shape";
			break;
		case 36:
			text = "mirror_shape";
			break;
		case 37:
			text = "planet_shape";
			break;
		case 38:
			text = "Speedship_shape";
			break;
		case 39:
			text = "UnstablePortal_shape";
			break;
		case 41:
			text = "NuclearBomb_shape";
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		component.spriteName = text;
		component.MakePixelPerfect();
		if (nodeType == 6)
		{
			Vector3 vector = to - from;
			float num = (float)component.height / 15f / 2.5f;
			num /= 2f;
			component.height = 8;
			component.width = (int)(vector.magnitude * 0.9f / num);
			gameObject2.transform.localScale = num * scale * Vector3.one * this.scaleRatio;
			gameObject.transform.localPosition = (from + to) / 2f;
			Vector3 normalized = vector.normalized;
			gameObject.transform.Rotate(Vector3.forward, Mathf.Atan2(normalized.y, normalized.x) * 180f / 3.1415927f);
		}
		else
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)camp);
			component.color = team.color;
			component.alpha = ((!(component.color == Color.clear)) ? 0.4f : 0.8f);
			if (this.isFadeOut)
			{
				component.alpha = 0f;
			}
			float num2 = 12f / (float)(component.width / 2);
			gameObject2.transform.localScale = num2 * scale * Vector3.one;
			gameObject.transform.localPosition = position;
			gameObject.transform.eulerAngles = Vector3.zero;
			if (vertical)
			{
				gameObject.transform.Rotate(Vector3.forward, 90f);
			}
		}
		gameObject.transform.localPosition *= this.scaleRatio;
		gameObject.transform.localScale = Vector3.zero;
		this.tweenList.Add(gameObject.GetComponent<TweenScale>());
		this.tweenAlphaList.Add(gameObject.GetComponent<TweenAlpha>());
	}

	private void CreateOrbits(int nodeType, int orbitType, int camp, Vector2 nowPos, MapBuildingConfig bi)
	{
		LoggerSystem.CodeComments("天体轨道计算注释by天凌喵---在这里创建轨道线的相关方法");
		Vector3 pos = Vector3.zero;
		Vector3 pos2 = Vector3.zero;
		Vector3 pos3 = Vector3.zero;
		Vector3 pos4 = Vector3.zero;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (orbitType != (int)RevolutionType.RT_Custom)
		{
			try
			{
				pos = Converter.ConvertVector3D(bi.orbitParam1);
				pos2 = Converter.ConvertVector3D(bi.orbitParam2);
				pos3 = Converter.ConvertVector3D(bi.orbitParam3);
				pos4 = Converter.ConvertVector3D(bi.orbitParam4);
			}
			catch
			{
				return;
			}
			flag = !string.IsNullOrEmpty(bi.orbitParam1) && (bi.orbitParam1.IndexOf(',') >= 0);
			flag2 = !string.IsNullOrEmpty(bi.orbitParam2) && (bi.orbitParam2.IndexOf(',') >= 0);
			flag3 = !string.IsNullOrEmpty(bi.orbitParam3) && (bi.orbitParam3.IndexOf(',') >= 0);
			flag4 = !string.IsNullOrEmpty(bi.orbitParam4) && (bi.orbitParam4.IndexOf(',') >= 0);
		}
		bool flag5 = orbitType == 2 && flag && flag2 && flag3 && !flag4;
		bool flag6 = orbitType == 3 && flag && flag2 && flag3 && flag4;
		Vector2 vector12 = nowPos;
		nowPos = this.ChangePosToLocal(nowPos);
		nowPos *= this.scaleRatio;
		Vector2 vector = Vector2.zero;
		Vector3 vector2 = Vector3.zero;
		Vector3 vector3 = Vector3.zero;
		if (orbitType == 1)
		{
			vector = this.ChangePosToLocal(new Vector2(pos.x, pos.y));
			vector *= this.scaleRatio;
		}
		else if (orbitType == 4)
		{
			vector = (new Vector2(pos.x, pos.y) + new Vector2(pos2.x, pos2.y)) / 2f;
			vector = this.ChangePosToLocal(vector);
			vector *= this.scaleRatio;
			Vector2 a = this.ChangePosToLocal(new Vector2(pos.x, pos.y)) * this.scaleRatio;
			Vector2 a2 = this.ChangePosToLocal(new Vector2(pos2.x, pos2.y)) * this.scaleRatio;
			vector2.x = a.x - vector.x;
			vector2.y = a.y - vector.y;
			vector3.x = a2.x - vector.x;
			vector3.y = a2.y - vector.y;
		}
		else if (orbitType == (int)RevolutionType.RT_Custom)
		{
			vector = Vector2.zero;
		}
		else if ((orbitType == 2 || orbitType == 3) && !flag5 && !flag6 && flag)
		{
			vector = this.ChangePosToLocal(new Vector2(pos.x, pos.y));
			vector *= this.scaleRatio;
		}
		GameObject gameObject = this.skyGo.AddChild(this.copyGo);
		gameObject.SetActive(true);
		gameObject.name = "orbits-" + nodeType;
		gameObject.transform.localPosition = vector;
		nowPos -= vector;
		Vector2 vector4 = Vector2.zero;
		gameObject.transform.Find("image").gameObject.SetActive(false);
		GameObject gameObject2 = gameObject.transform.Find("line").gameObject;
		gameObject2.SetActive(true);
		LineRenderer component = gameObject2.GetComponent<LineRenderer>();
		Color color = new Color32(204, 204, 204, 102);
		LineRenderer lineRenderer = component;
		float num = 0.005f;
		component.startWidth = num;
		lineRenderer.endWidth = num;
		LineRenderer lineRenderer2 = component;
		Color color2 = color;
		component.startColor = color2;
		lineRenderer2.endColor = color2;
		List<Vector3> list = new List<Vector3>();
		switch (orbitType)
		{
		case 1:
			this.CalculatePositions(new Vector3(nowPos.x, nowPos.y, -1f), new Vector3(vector4.x, vector4.y, -1f), 128, list);
			break;
		case 2:
			if (flag5)
			{
				Vector2 vector5 = this.ChangePosToLocal(new Vector2(pos.x, pos.y)) * this.scaleRatio - vector;
				Vector2 vector6 = this.ChangePosToLocal(new Vector2(pos2.x, pos2.y)) * this.scaleRatio - vector;
				Vector2 vector7 = this.ChangePosToLocal(new Vector2(pos3.x, pos3.y)) * this.scaleRatio - vector;
				list.Add(new Vector3(vector5.x, vector5.y, -1f));
				list.Add(new Vector3(vector6.x, vector6.y, -1f));
				list.Add(new Vector3(vector7.x, vector7.y, -1f));
				list.Add(new Vector3(vector5.x, vector5.y, -1f));
			}
			else
			{
				this.CalculatePositions(new Vector3(nowPos.x, nowPos.y, -1f), new Vector3(vector4.x, vector4.y, -1f), 3, list);
			}
			break;
		case 3:
			if (flag6)
			{
				Vector2 vector8 = this.ChangePosToLocal(new Vector2(pos.x, pos.y)) * this.scaleRatio - vector;
				Vector2 vector9 = this.ChangePosToLocal(new Vector2(pos2.x, pos2.y)) * this.scaleRatio - vector;
				Vector2 vector10 = this.ChangePosToLocal(new Vector2(pos3.x, pos3.y)) * this.scaleRatio - vector;
				Vector2 vector11 = this.ChangePosToLocal(new Vector2(pos4.x, pos4.y)) * this.scaleRatio - vector;
				list.Add(new Vector3(vector8.x, vector8.y, -1f));
				list.Add(new Vector3(vector9.x, vector9.y, -1f));
				list.Add(new Vector3(vector10.x, vector10.y, -1f));
				list.Add(new Vector3(vector11.x, vector11.y, -1f));
				list.Add(new Vector3(vector8.x, vector8.y, -1f));
			}
			else
			{
				this.CalculatePositions(new Vector3(nowPos.x, nowPos.y, -1f), new Vector3(vector4.x, vector4.y, -1f), 4, list);
			}
			break;
		case 4:
			this.CalculatePositionsEllipse(new Vector3(nowPos.x, nowPos.y, -1f), new Vector3(vector2.x, vector2.y, -1f), new Vector3(vector3.x, vector3.y, -1f), 128, list);
			break;
		case 5:
		{
			string text = string.IsNullOrEmpty(bi.equation) ? bi.orbitParam1 : bi.equation;
			string text2 = string.IsNullOrEmpty(bi.orbitXChoose) ? bi.orbitParam2 : bi.orbitXChoose;
			string text3 = string.IsNullOrEmpty(bi.equationX) ? bi.orbitParam1 : bi.equationX;
			string text4 = string.IsNullOrEmpty(bi.equationY) ? bi.orbitParam3 : bi.equationY;
			string text5 = string.IsNullOrEmpty(bi.orbitTChoose) ? bi.orbitParam2 : bi.orbitTChoose;
			string text6 = string.IsNullOrEmpty(bi.orbitBox) ? bi.orbitParam2 : bi.orbitBox;
			if (!string.IsNullOrEmpty(text4))
			{
				Func<float, float> func;
				Func<float, float> func2;
				if (!EquationFunction.TryCreateParametric(text3, text4, out func, out func2))
				{
					break;
				}
				float num3 = 0f;
				float num4 = 6.2831855f;
				if (!string.IsNullOrEmpty(text5))
				{
					string[] array = text5.Split(new char[]
					{
						','
					});
					if (array != null && array.Length >= 2)
					{
						float tmp2;
						float tmp3;
						if (float.TryParse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture, out tmp2) && float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out tmp3))
						{
							num3 = tmp2;
							num4 = tmp3;
						}
					}
				}
				int num6 = 200;
				bool paramHasBounds = false;
				float paramMinX = 0f;
				float paramMaxX = 0f;
				float paramMinY = 0f;
				float paramMaxY = 0f;
				for (int i = 0; i < num6; i++)
				{
					float t = (num6 <= 1) ? 0f : ((float)i / (float)(num6 - 1));
					float paramT = Mathf.Lerp(num3, num4, t);
					float px = func(paramT);
					float py = func2(paramT);
					if (float.IsNaN(px) || float.IsInfinity(px) || float.IsNaN(py) || float.IsInfinity(py))
					{
						continue;
					}
					Vector2 vector6 = new Vector2(px, py);
					Vector2 vector7 = this.ChangePosToLocal(vector6) * this.scaleRatio - vector;
					list.Add(new Vector3(vector7.x, vector7.y, -1f));
					if (!paramHasBounds)
					{
						paramHasBounds = true;
						paramMinX = vector7.x;
						paramMaxX = vector7.x;
						paramMinY = vector7.y;
						paramMaxY = vector7.y;
					}
					else
					{
						if (vector7.x < paramMinX)
						{
							paramMinX = vector7.x;
						}
						if (vector7.x > paramMaxX)
						{
							paramMaxX = vector7.x;
						}
						if (vector7.y < paramMinY)
						{
							paramMinY = vector7.y;
						}
						if (vector7.y > paramMaxY)
						{
							paramMaxY = vector7.y;
						}
					}
				}
				if (list.Count >= 2)
				{
					float paramEps = Mathf.Min(paramMaxX - paramMinX, paramMaxY - paramMinY) * 0.2f;
					if (paramEps <= 0.0001f)
					{
						paramEps = 0.01f;
					}
					bool paramIsClosed = Vector3.Distance(list[0], list[list.Count - 1]) <= paramEps;
					if (paramIsClosed)
					{
						list.Add(list[0]);
					}
					else
					{
						for (int j = list.Count - 2; j > 0; j--)
						{
							list.Add(list[j]);
						}
						if (list.Count > 0)
						{
							list.Add(list[0]);
						}
					}
				}
				break;
			}
			string[] array2 = (string.IsNullOrEmpty(text6) ? null : text6.Split(new char[]
			{
				','
			}));
			if (array2 != null && array2.Length >= 4)
			{
				List<Vector3> list2;
				if (!EquationFunction.TrySampleImplicit(text, text6, vector12.x, vector12.y, out list2))
				{
					break;
				}
				float xmin;
				float xmax;
				float ymin;
				float ymax;
				float num10 = 0.001f;
				if (array2.Length >= 4 && float.TryParse(array2[0], NumberStyles.Float, CultureInfo.InvariantCulture, out xmin) && float.TryParse(array2[1], NumberStyles.Float, CultureInfo.InvariantCulture, out xmax) && float.TryParse(array2[2], NumberStyles.Float, CultureInfo.InvariantCulture, out ymin) && float.TryParse(array2[3], NumberStyles.Float, CultureInfo.InvariantCulture, out ymax))
				{
					if (xmin > xmax)
					{
						float tmp4 = xmin;
						xmin = xmax;
						xmax = tmp4;
					}
					if (ymin > ymax)
					{
						float tmp5 = ymin;
						ymin = ymax;
						ymax = tmp5;
					}
					num10 = Mathf.Min((xmax - xmin) / 80f, (ymax - ymin) / 80f) * 20f;
					if (num10 <= 0.0001f)
					{
						num10 = 0.001f;
					}
				}
				bool flag7 = list2.Count >= 2 && Vector3.Distance(list2[0], list2[list2.Count - 1]) <= num10;
				int count = list2.Count;
				for (int j = 0; j < count; j++)
				{
					Vector3 vector8 = list2[j];
					if (float.IsNaN(vector8.x) || float.IsInfinity(vector8.x) || float.IsNaN(vector8.y) || float.IsInfinity(vector8.y))
					{
						continue;
					}
					Vector2 vector9 = this.ChangePosToLocal(new Vector2(vector8.x, vector8.y)) * this.scaleRatio - vector;
					list.Add(new Vector3(vector9.x, vector9.y, -1f));
				}
				if (!flag7)
				{
					for (int k = count - 2; k > 0; k--)
					{
						Vector3 vector10 = list2[k];
						if (float.IsNaN(vector10.x) || float.IsInfinity(vector10.x) || float.IsNaN(vector10.y) || float.IsInfinity(vector10.y))
						{
							continue;
						}
						Vector2 vector11 = this.ChangePosToLocal(new Vector2(vector10.x, vector10.y)) * this.scaleRatio - vector;
						list.Add(new Vector3(vector11.x, vector11.y, -1f));
					}
				}
				if (list.Count > 0)
				{
					list.Add(list[0]);
				}
				break;
			}
			Func<float, float> func3;
			if (!EquationFunction.TryCreate(text, out func3))
			{
				break;
			}
			string[] array3 = (string.IsNullOrEmpty(text2) ? null : text2.Split(new char[]
			{
				','
			}));
			if (array3 == null || array3.Length < 2)
			{
				break;
			}
			float num11;
			float num12;
			if (!float.TryParse(array3[0], NumberStyles.Float, CultureInfo.InvariantCulture, out num11) || !float.TryParse(array3[1], NumberStyles.Float, CultureInfo.InvariantCulture, out num12))
			{
				break;
			}
			if (num11 > num12)
			{
				float tmp6 = num11;
				num11 = num12;
				num12 = tmp6;
			}
			float rangeAbs = Mathf.Abs(num12 - num11);
			int num13 = Mathf.Clamp(Mathf.RoundToInt(rangeAbs * 20f) + 2, 10, 200);
			for (int l = 0; l < num13; l++)
			{
				float t2 = (num13 <= 1) ? 0f : ((float)l / (float)(num13 - 1));
				float num14 = Mathf.Lerp(num11, num12, t2);
				float num15 = func3(num14);
				if (float.IsNaN(num15) || float.IsInfinity(num15))
				{
					continue;
				}
				Vector2 vector13 = new Vector2(num14, num15);
				Vector2 vector14 = this.ChangePosToLocal(vector13) * this.scaleRatio - vector;
				list.Add(new Vector3(vector14.x, vector14.y, -1f));
			}
			for (int m = num13 - 2; m > 0; m--)
			{
				float t3 = (num13 <= 1) ? 0f : ((float)m / (float)(num13 - 1));
				float num16 = Mathf.Lerp(num11, num12, t3);
				float num17 = func3(num16);
				if (float.IsNaN(num17) || float.IsInfinity(num17))
				{
					continue;
				}
				Vector2 vector15 = new Vector2(num16, num17);
				Vector2 vector16 = this.ChangePosToLocal(vector15) * this.scaleRatio - vector;
				list.Add(new Vector3(vector16.x, vector16.y, -1f));
			}
			if (list.Count > 0)
			{
				list.Add(list[0]);
			}
			break;
		}
		}
		if (list.Count <= 1)
		{
			return;
		}
		component.useWorldSpace = false;
		component.SetVertexCount(list.Count);
		component.SetPositions(list.ToArray());
		this.tweenList.Add(gameObject.GetComponent<TweenScale>());
		this.tweenAlphaList.Add(gameObject.GetComponent<TweenAlpha>());
		this.tweenOrbitsList.Add(gameObject.GetComponent<TweenScale>());
	}

	private void CalculatePositions(Vector3 basePos, Vector3 centerPos, int count, List<Vector3> positions)
	{
		basePos -= centerPos;
		positions.Add(basePos);
		float angle = 360f / (float)count;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		for (int i = 1; i < count; i++)
		{
			Vector3 item = rotation * positions[i - 1];
			positions.Add(item);
		}
		positions.Add(basePos);
		for (int j = 0; j < positions.Count; j++)
		{
			int index = j;
			positions[index = j] = positions[index] + centerPos;
		}
	}

	private void CalculatePositionsEllipse(Vector3 basePos, Vector3 pos1, Vector3 pos2, int count, List<Vector3> positions)
	{
		Vector3 b = (pos1 + pos2) / 2f;
		Vector3 vector = pos2 - pos1;
		Vector3 forward = Vector3.forward;
		float angle = Mathf.Atan2(vector.y, vector.x);
		float num = (Vector3.Distance(basePos, pos1) + Vector3.Distance(basePos, pos2)) / 2f;
		float num2 = Vector3.Distance(pos1, pos2) / 2f;
		float num3 = Mathf.Sqrt(num * num - num2 * num2);
		float num4 = 6.2831855f / (float)count;
		for (int i = 0; i <= count; i++)
		{
			float f = num4 * (float)i;
			Vector3 vector2 = Vector3.zero;
			vector2.x = num * Mathf.Cos(f);
			vector2.y = num3 * Mathf.Sin(f);
			vector2.z = basePos.z;
			vector2 = vector2.RotateAround(Vector3.zero, forward, angle);
			vector2 += b;
			positions.Add(vector2);
		}
	}

	public GameObject GetEffectRoot(string id)
	{
		GameObject result = null;
		Transform transform = this.skyGo.transform.Find("build-" + id);
		if (transform != null)
		{
			transform = transform.Find("effectroot");
		}
		if (transform != null)
		{
			result = transform.gameObject;
		}
		return result;
	}

	private Vector2 ChangePosToLocal(Vector2 world)
	{
		Vector3 position = Camera.main.WorldToScreenPoint(new Vector3(world.x, world.y, 0f));
		position.z = 0f;
		Vector3 point = UICamera.currentCamera.ScreenToWorldPoint(position);
		world = this.skyGo.transform.worldToLocalMatrix.MultiplyPoint(point);
		return world;
	}

	public void Clear()
	{
		this.SetZeroPoint();
		this.tweenList.Clear();
		this.tweenAlphaList.Clear();
		this.tweenOrbitsList.Clear();
		this.orbitsBuildingItem.Clear();
	}

	public GameObject copyGo;

	public GameObject skyGo;

	public UILabel levelScore;

	public UITable ScoreTable;

	public TweenScale selectNumCircle;

	public TweenScale selectNumCircleEffect;

	public GameObject blackSelectEffect;

	public float positionScale = 1f;

	public UILabel chapterName;

	public UILabel levelName;

	private string showSceneId = string.Empty;

	private List<TweenScale> tweenList = new List<TweenScale>();

	private List<TweenAlpha> tweenAlphaList = new List<TweenAlpha>();

	private List<TweenScale> tweenOrbitsList = new List<TweenScale>();

	private List<MapBuildingConfig> orbitsBuildingItem = new List<MapBuildingConfig>();

	private bool isFadeOut;

	private bool isSwitch;

	private bool immediately;

	private Vector2 zeroPointInSky;

	private string sceneId;

	private float scaleRatio = 0.66f;

	private List<Vector3> blackSelectScales;

	private EventDelegate eventHandle;

	private bool stroceEnable;

	private float lastV;

	private Vector2 offdistance = new Vector2(0f, 0f);
}
