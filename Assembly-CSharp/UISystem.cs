using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class UISystem : Solarmax.Singleton<UISystem>, Lifecycle
{
	public UISystem()
	{
		this.mManagedWindows = new List<BaseWindow>();
		this.mShowingStack = new List<ShowWindowParams>();
		this.mUIRoot = null;
		this.mUICamera = null;
	}

	public BaseWindow GetWindow(string name)
	{
		BaseWindow result = null;
		for (int i = 0; i < this.mManagedWindows.Count; i++)
		{
			if (this.mManagedWindows[i] != null && this.mManagedWindows[i].GetName().Equals(name))
			{
				result = this.mManagedWindows[i];
			}
		}
		return result;
	}

	private void HideCurrentWindowsByStack(ShowWindowParams param)
	{
		UIWindowConfig data = Solarmax.Singleton<UIWindowConfigProvider>.Instance.GetData(param.WindowName);
		if (data == null || string.IsNullOrEmpty(data.mPrefabPath))
		{
			Debug.LogErrorFormat("UISystem_ShowWindow : {0}'s prefab config is null!", new object[]
			{
				param.WindowName
			});
			return;
		}
		int count = this.mShowingStack.Count;
		if (!data.mOverlapDisplay && data.mUseShowingStack && count > 0)
		{
			for (int i = count - 1; i >= 0; i--)
			{
				BaseWindow window = this.GetWindow(this.mShowingStack[i].WindowName);
				UIWindowConfig data2 = Solarmax.Singleton<UIWindowConfigProvider>.Instance.GetData(param.WindowName);
				if (data2 == null || !data2.mIsGlobalDisplay)
				{
					if (this.IsWindowVisible(window))
					{
						this.HideWindowImpl(window);
					}
				}
			}
		}
	}

	private void PushShowingStack(ShowWindowParams param, UIWindowConfig cfg)
	{
		int count = this.mShowingStack.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			ShowWindowParams value = this.mShowingStack[i];
			if (value.WindowName == param.WindowName)
			{
				this.mShowingStack[i] = this.mShowingStack[count - 1];
				this.mShowingStack[count - 1] = value;
				return;
			}
		}
		this.mShowingStack.Add(param);
	}

	private void PopShowingStack(string windowName, UIWindowConfig cfg)
	{
		int num = this.mShowingStack.Count;
		for (int i = num - 1; i >= 0; i--)
		{
			if (this.mShowingStack[i].WindowName == windowName)
			{
				this.mShowingStack.RemoveAt(i);
				num--;
				break;
			}
		}
		if (cfg.mUseShowingStack)
		{
			for (int j = num - 1; j >= 0; j--)
			{
				UIWindowConfig data = Solarmax.Singleton<UIWindowConfigProvider>.Instance.GetData(this.mShowingStack[j].WindowName);
				if (data != null)
				{
					if (!this.IsWindowVisible(this.mShowingStack[j].WindowName))
					{
						this.ShowWindowImpl(this.mShowingStack[j]);
					}
					if (!data.mOverlapDisplay)
					{
						break;
					}
				}
			}
		}
	}

	public bool Init()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("UISystem    init  begin", new object[0]);
		GameObject gameObject = GameObject.Find("UI Root");
		if (gameObject == null)
		{
			return false;
		}
		this.mUIRoot = gameObject.GetComponent<UIRoot>();
		gameObject = this.mUIRoot.transform.Find("Camera").gameObject;
		if (gameObject == null)
		{
			return false;
		}
		this.mUICamera = gameObject.GetComponent<UICamera>();
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("UISystem    init  end", new object[0]);
		return true;
	}

	public void Tick(float interval)
	{
	}

	public void Destroy()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("UISystem    destroy  begin", new object[0]);
		this.HideAllWindow();
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("UISystem    destroy  end", new object[0]);
	}

	public UIRoot GetNGUIRoot()
	{
		return this.mUIRoot;
	}

	private bool IsWindowVisible(BaseWindow window)
	{
		return window != null && window.gameObject.activeSelf;
	}

	public bool IsWindowVisible(string name)
	{
		BaseWindow window = this.GetWindow(name);
		return this.IsWindowVisible(window);
	}

	public void ShowWindow(string name)
	{
		this.ShowWindow(new ShowWindowParams(name, EventId.Undefine, new object[0]));
	}

	public void ShowWindow(ShowWindowParams param)
	{
		this.HideCurrentWindowsByStack(param);
		BaseWindow baseWindow = this.ShowWindowImpl(param);
		if (null == baseWindow)
		{
			return;
		}
		this.PushShowingStack(param, baseWindow.GetConfigData());
	}

	public void SetInitEvent(string windowName, EventId eventId, params object[] eventArgs)
	{
		int count = this.mShowingStack.Count;
		if (count < 1)
		{
			return;
		}
		ShowWindowParams value = this.mShowingStack[count - 1];
		if (value.WindowName != windowName)
		{
			return;
		}
		value.InitEventId = eventId;
		value.InitEventArgs = eventArgs;
		this.mShowingStack[count - 1] = value;
	}

	private BaseWindow ShowWindowImpl(ShowWindowParams param)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("UISystem ShowWindowImpl ", new object[0]);
		BaseWindow baseWindow = this.GetWindow(param.WindowName);
		TweenAlpha ta;
		if (baseWindow != null)
		{
			if (!this.IsWindowVisible(baseWindow))
			{
				baseWindow.gameObject.SetActive(true);
				baseWindow.OnShow();
				if (param.InitEventId != EventId.Undefine)
				{
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(param.InitEventId, param.InitEventArgs);
				}
			}
			ta = baseWindow.gameObject.GetComponent<TweenAlpha>();
			if (ta == null)
			{
				ta = baseWindow.gameObject.AddComponent<TweenAlpha>();
			}
			ta.enabled = true;
			ta.ResetToBeginning();
			ta.from = 0f;
			ta.to = 1f;
			ta.duration = 0.5f;
			ta.SetOnFinished(delegate()
			{
				ta.value = 1f;
				ta.enabled = false;
			});
			ta.Play(true);
			return baseWindow;
		}
		UIWindowConfig data = Solarmax.Singleton<UIWindowConfigProvider>.Instance.GetData(param.WindowName);
		if (data == null || string.IsNullOrEmpty(data.mPrefabPath))
		{
			BaseWindow codeWindow = this.CreateWindowByType(param.WindowName);
			if (codeWindow != null)
			{
				codeWindow.gameObject.name = param.WindowName;
				ta = codeWindow.gameObject.GetComponent<TweenAlpha>();
				if (ta == null)
				{
					ta = codeWindow.gameObject.AddComponent<TweenAlpha>();
				}
				ta.enabled = true;
				ta.ResetToBeginning();
				ta.from = 0f;
				ta.to = 1f;
				ta.duration = 0.5f;
				ta.SetOnFinished(delegate()
				{
					ta.value = 1f;
					ta.enabled = false;
				});
				ta.Play(true);
				this.mManagedWindows.Add(codeWindow);
				codeWindow.OnShow();
				if (param.InitEventId != EventId.Undefine)
				{
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(param.InitEventId, param.InitEventArgs);
				}
				return codeWindow;
			}
			Debug.LogErrorFormat("UISystem_ShowWindow : {0}'s prefab config is null!", new object[]
			{
				param.WindowName
			});
			return null;
		}
		GameObject gameObject = Solarmax.Singleton<AssetManager>.Get().LoadResource("gameres/" + data.mPrefabPath.ToLower() + ".prefab") as GameObject;
		if (null == gameObject)
		{
			Debug.LogErrorFormat("UISystem_ShowWindow : {0}'s prefab cannot Load!", new object[]
			{
				param.WindowName
			});
			return null;
		}
		gameObject = UnityTools.AddChild(this.mUICamera.gameObject, gameObject);
		baseWindow = gameObject.GetComponent<BaseWindow>();
		if (baseWindow == null)
		{
			Debug.LogErrorFormat("UISystem_ShowWindow : {0} can't get BaseWindow component from prefab!", new object[]
			{
				param.WindowName
			});
			return null;
		}
		baseWindow.SetConfigData(data);
		if (!baseWindow.Init())
		{
			Debug.LogErrorFormat("UISystem_ShowWindow : {0}'s BaseWindow init failed!", new object[]
			{
				param.WindowName
			});
			return null;
		}
		ta = baseWindow.gameObject.GetComponent<TweenAlpha>();
		if (ta == null)
		{
			ta = baseWindow.gameObject.AddComponent<TweenAlpha>();
		}
		ta.enabled = true;
		ta.ResetToBeginning();
		ta.from = 0f;
		ta.to = 1f;
		ta.duration = 0.5f;
		ta.SetOnFinished(delegate()
		{
			ta.value = 1f;
			ta.enabled = false;
		});
		ta.Play(true);
		this.mManagedWindows.Add(baseWindow);
		baseWindow.OnShow();
		if (param.InitEventId != EventId.Undefine)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(param.InitEventId, param.InitEventArgs);
		}
		return baseWindow;
	}

	private BaseWindow CreateWindowByType(string windowName)
	{
		Type windowType = Type.GetType(windowName);
		if (windowType == null || !typeof(BaseWindow).IsAssignableFrom(windowType))
		{
			return null;
		}
		GameObject gameObject = new GameObject(windowName);
		gameObject.layer = this.mUICamera.gameObject.layer;
		gameObject.transform.SetParent(this.mUICamera.transform, false);
		UIPanel uipanel = gameObject.AddComponent<UIPanel>();
		uipanel.depth = 10;
		BaseWindow baseWindow = (BaseWindow)gameObject.AddComponent(windowType);
		UIWindowConfig uiwindowConfig = new UIWindowConfig();
		uiwindowConfig.mID = 99999;
		uiwindowConfig.mName = windowName;
		uiwindowConfig.mPrefabPath = string.Empty;
		uiwindowConfig.mHideWithDestroy = true;
		uiwindowConfig.mUseShowingStack = true;
		uiwindowConfig.mOverlapDisplay = false;
		uiwindowConfig.mIsGlobalDisplay = false;
		baseWindow.SetConfigData(uiwindowConfig);
		if (!baseWindow.Init())
		{
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		return baseWindow;
	}

	public void DirectAdd(BaseWindow bw)
	{
		this.loadingwindow = bw;
	}

	public void DirectDel(BaseWindow bw)
	{
		this.HideWindowImpl(this.loadingwindow);
		this.loadingwindow = null;
	}

	private void HideWindowImpl(BaseWindow bw)
	{
		if (null == bw)
		{
			return;
		}
		bw.OnHide();
		if (bw.GetConfigData() != null && bw.GetConfigData().mHideWithDestroy)
		{
			bw.Release();
			this.mManagedWindows.Remove(bw);
			UnityEngine.Object.Destroy(bw.gameObject);
		}
		else
		{
			bw.gameObject.SetActive(false);
		}
	}

	public void HideWindow(string name)
	{
		BaseWindow window = this.GetWindow(name);
		if (!this.IsWindowVisible(window))
		{
			Debug.LogFormat("UISystem_HideWindow : {0} don't showed or invisible, don't need hide.", new object[]
			{
				name
			});
			return;
		}
		UIWindowConfig configData = window.GetConfigData();
		this.HideWindowImpl(window);
		if (!name.Equals("FriendRanking") && !name.Equals("SingleClearWindow"))
		{
			GuideManager.ClearGuideData();
		}
		this.PopShowingStack(name, configData);
	}

	public void FadeOutWindow2(string name)
	{
		BaseWindow window = this.GetWindow(name);
		if (window == null)
		{
			return;
		}
		TweenAlpha tweenAlpha = window.gameObject.GetComponent<TweenAlpha>();
		float num = 1f;
		float num2 = 0.25f;
		if (tweenAlpha == null)
		{
			tweenAlpha = window.gameObject.AddComponent<TweenAlpha>();
		}
		else
		{
			num = tweenAlpha.value;
			num2 *= num;
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = num;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = num2;
		tweenAlpha.SetOnFinished(delegate()
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow(name);
		});
		tweenAlpha.Play(true);
	}

	public void HideAllWindow()
	{
		for (int i = this.mManagedWindows.Count - 1; i >= 0; i--)
		{
			if (i < this.mManagedWindows.Count)
			{
				BaseWindow baseWindow = this.mManagedWindows[i];
				if (baseWindow != null && this.IsWindowVisible(baseWindow))
				{
					this.HideWindowImpl(baseWindow);
				}
			}
		}
		this.mShowingStack.Clear();
	}

	public void OnEventHandler(int eventId, object window, params object[] args)
	{
		BaseWindow window2 = this.GetWindow((string)window);
		if (window2 != null)
		{
			window2.OnUIEventHandler((EventId)eventId, args);
		}
		if (this.loadingwindow != null)
		{
			this.loadingwindow.OnUIEventHandler((EventId)eventId, args);
		}
	}

	public void ShowDialogWindow(int type, string tipLog)
	{
		this.ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
		{
			type,
			tipLog
		});
	}

	public static void DirectShowPrefab(string path)
	{
		GameObject prefab = LoadResManager.LoadRes("gameres/" + path);
		GameObject parent = GameObject.Find("UI Root/Camera");
		prefab = UnityTools.AddChild(parent, prefab);
	}

	public void FadeOutWindow(string name)
	{
		if (!this.IsWindowVisible(name))
		{
			return;
		}
		this.ShowWindow("UnTouchWindow");
		GameObject gameObject = this.GetWindow(name).gameObject;
		TweenAlpha tweenAlpha = gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 0.5f;
		tweenAlpha.SetOnFinished(delegate()
		{
			this.HideWindow(name);
			this.HideWindow("UnTouchWindow");
		});
	}

	public void FadeBattle(bool fadeIn, EventDelegate ed = null)
	{
		if (fadeIn)
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.FadePlanet(fadeIn, 0.5f);
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.FadePlanet(fadeIn, 0.5f);
		}
		GameObject battle = Solarmax.Singleton<BattleSystem>.Instance.battleData.root;
		battle.SetActive(true);
		TweenScale tweenScale = battle.GetComponent<TweenScale>();
		if (tweenScale == null)
		{
			tweenScale = battle.AddComponent<TweenScale>();
		}
		tweenScale.ResetToBeginning();
		if (fadeIn)
		{
			tweenScale.from = Vector3.one * 0.7f;
			tweenScale.to = Vector3.one;
		}
		else
		{
			tweenScale.from = Vector3.one;
			tweenScale.to = Vector3.one * 0.7f;
		}
		tweenScale.duration = 0.5f;
		tweenScale.SetOnFinished(delegate()
		{
			if (fadeIn)
			{
				Solarmax.Singleton<ShipFadeManager>.Get().SetFadeType(ShipFadeManager.FADETYPE.IN, 0.25f);
			}
			battle.transform.localScale = Vector3.one;
			if (ed != null)
			{
				ed.Execute();
			}
		});
		tweenScale.Play(true);
	}

	public void FadeOutBattle(bool fadeIn, EventDelegate ed = null)
	{
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.FadePlanet(false, 0.15f);
		Solarmax.Singleton<ShipFadeManager>.Get().SetShipAlpha(0f);
		GameObject root = Solarmax.Singleton<BattleSystem>.Instance.battleData.root;
		root.SetActive(true);
		TweenAlpha tweenAlpha = root.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = root.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 0.1f;
		tweenAlpha.SetOnFinished(delegate()
		{
			if (ed != null)
			{
				ed.Execute();
			}
		});
		tweenAlpha.Play(true);
	}

	private List<BaseWindow> mManagedWindows;

	private List<ShowWindowParams> mShowingStack;

	private BaseWindow loadingwindow;

	private UIRoot mUIRoot;

	private UICamera mUICamera;
}
