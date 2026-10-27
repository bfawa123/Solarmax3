using System;
using System.Collections;
using Solarmax;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class NetTexture : MonoBehaviour
{
	public string picUrl
	{
		get
		{
			return this.url;
		}
		set
		{
			this.url = value;
			if (this.url.StartsWith("http"))
			{
				base.StartCoroutine(this.SetUrl());
			}
			else
			{
				this.SetLocalIcon();
			}
		}
	}

	public UIScrollView scroll
	{
		get
		{
			return this.scrollView;
		}
		set
		{
			this.scrollView = value;
		}
	}

	public UIPanel picPanel
	{
		get
		{
			return this.panel;
		}
		set
		{
			this.panel = value;
		}
	}

	public Color picColor
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
			if (this.uiTexture != null)
			{
				this.uiTexture.color = this.color;
			}
		}
	}

	private void Awake()
	{
		this.uiTexture = base.GetComponent<UITexture>();
	}

	private void Start()
	{
		if (this.url.StartsWith("http"))
		{
			base.StartCoroutine(this.SetUrl());
		}
		else
		{
			this.SetLocalIcon();
		}
	}

	private void OnDestroy()
	{
		if (this.effect != null && this.effect.go != null && this.effect.go.transform.parent == base.transform)
		{
			this.effect.Destory();
			this.effect = null;
		}
	}

	private void SetLocalIcon()
	{
		if (string.IsNullOrEmpty(this.url) || this.uiTexture == null)
		{
			return;
		}
		if (!this.url.EndsWith(".png"))
		{
			this.url += ".png";
		}
		string text = string.Format("gameres/atlas/skin/{0}", this.url);
		Texture2D texture2D = LoadResManager.LoadTex(text.ToLower());
		if (texture2D != null && texture2D != null)
		{
			this.uiTexture.mainTexture = texture2D;
			if (this.pixelPerfect)
			{
				this.uiTexture.MakePixelPerfect();
			}
		}
		if (Solarmax.Singleton<SkinConfigProvider>.Get().avatarDic.ContainsKey(this.url) && Solarmax.Singleton<SkinConfigProvider>.Get().avatarDic[this.url].skinType == 1)
		{
			int width = this.uiTexture.width;
			int height = this.uiTexture.height;
			SkinConfig skinConfig = Solarmax.Singleton<SkinConfigProvider>.Get().avatarDic[this.url];
			if (this.effect == null || this.currentEffect != skinConfig.bgImage || this.effect.go == null)
			{
				if (this.effect != null)
				{
					this.effect.Recycle();
					this.effect = null;
				}
				AnyPool<IPoolGo> effectPool = HeadEffectManager.GetEffectPool(skinConfig.bgImage);
				this.currentEffect = skinConfig.bgImage;
				this.effect = (HeadEffect)effectPool.Alloc();
				while (this.effect.go == null)
				{
					this.effect.Destory();
					this.effect = null;
					this.effect = (HeadEffect)effectPool.Alloc();
				}
			}
			if (this.effect != null && this.uiTexture != null)
			{
				Shader shader = Shader.Find("ACTFX/Additive");
				if (this.scroll != null)
				{
					shader = Shader.Find("Unlit/EffectSoftClip");
				}
				Renderer[] componentsInChildren = this.effect.go.GetComponentsInChildren<Renderer>();
				if (componentsInChildren != null)
				{
					foreach (Renderer renderer in componentsInChildren)
					{
						renderer.material.shader = shader;
					}
				}
				if (this.panel != null)
				{
					Renderer component = this.effect.go.GetComponent<Renderer>();
					if (component != null)
					{
						this.effect.go.GetComponent<Renderer>().sortingOrder = this.panel.sortingOrder + 1;
					}
				}
				Renderer[] componentsInChildren2 = this.effect.go.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer2 in componentsInChildren2)
				{
					if (this.panel != null)
					{
						renderer2.sortingOrder = this.panel.sortingOrder + 1;
					}
					renderer2.material.shader = shader;
				}
				this.effect.go.transform.parent = this.uiTexture.transform;
				if (this.panel != null)
				{
					this.effect.go.transform.localPosition = new Vector3(0f, 0f, -100f);
				}
				else
				{
					this.effect.go.transform.localPosition = Vector3.zero;
				}
				Vector3 vector = base.transform.parent.localScale / 0.4f;
				string bgImage = skinConfig.bgImage;
				if (bgImage != null)
				{
					if (!(bgImage == "EFF_select_head_101"))
					{
						if (!(bgImage == "EFF_select_head_102"))
						{
							if (bgImage == "EFF_select_head_103")
							{
								this.effect.go.transform.localScale = new Vector3((float)width / 170f, (float)height / 170f, (float)height / 170f);
							}
						}
						else
						{
							this.effect.go.transform.localScale = new Vector3((float)width / 170f, (float)height / 170f, (float)height / 170f);
						}
					}
					else
					{
						this.effect.go.transform.localScale = new Vector3((float)width * 2.3f, (float)height * 2.3f, (float)width * 2.3f);
					}
				}
				this.effect.go.SetActive(true);
			}
			else if (this.effect != null && this.effect.go != null)
			{
				this.effect.Recycle();
				this.effect = null;
			}
		}
		else if (this.effect != null && this.effect.go != null && this.effect.go.transform.parent == base.transform)
		{
			this.effect.Recycle();
			this.effect = null;
		}
		this.ScrollViewPanel();
	}

	private IEnumerator SetUrl()
	{
		if (!string.IsNullOrEmpty(this.url) && this.uiTexture != null)
		{
			WWW www = new WWW(this.url);
			yield return www;
			Texture2D tex = www.texture;
			if (tex != null)
			{
				this.uiTexture.mainTexture = tex;
				if (this.pixelPerfect)
				{
					this.uiTexture.MakePixelPerfect();
				}
			}
			www.Dispose();
		}
		yield break;
	}

	private void ScrollViewPanel()
	{
		if (this.scrollView == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Debug("NetTexure scrollview为空", new object[0]);
			return;
		}
		UIPanel component = this.scrollView.GetComponent<UIPanel>();
		Vector3 vector = UICamera.mainCamera.WorldToViewportPoint(component.worldCorners[0]);
		Vector3 vector2 = UICamera.mainCamera.WorldToViewportPoint(component.worldCorners[2]);
		Vector4 value = new Vector4(-5f, -5f, 5f, 5f);
		if (this.scrollView.movement == UIScrollView.Movement.Horizontal)
		{
			value = new Vector4(vector.x * 2f - 1f, -5f, vector2.x * 2f - 1f, 5f);
		}
		else if (this.scrollView.movement == UIScrollView.Movement.Vertical)
		{
			value = new Vector4(-5f, vector.y * 2f - 1f, 5f, vector2.y * 2f - 1f);
		}
		Shader.SetGlobalVector("effectClipRange", value);
	}

	public bool pixelPerfect;

	private string url = string.Empty;

	private Color color = Color.white;

	private UITexture uiTexture;

	private UIPanel panel;

	private UIScrollView scrollView;

	private HeadEffect effect;

	public string currentEffect = string.Empty;
}
