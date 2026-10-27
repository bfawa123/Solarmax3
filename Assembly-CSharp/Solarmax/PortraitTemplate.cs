using System;
using System.Collections;
using UnityEngine;

namespace Solarmax
{
	public class PortraitTemplate : MonoBehaviour
	{
		public void OnEnable()
		{
		}

		public void OnDisable()
		{
		}

		public void Load(string url, UIScrollView view = null, UIPanel panel = null)
		{
			if (string.IsNullOrEmpty(url))
			{
				return;
			}
			NetTexture component = this.avatar.GetComponent<NetTexture>();
			if (component != null)
			{
				component.picPanel = null;
				component.scroll = view;
				component.picUrl = url;
				return;
			}
			this.scroll = view;
			if (!url.StartsWith("http"))
			{
				if (!url.EndsWith(".png"))
				{
					url += ".png";
				}
				Texture2D texture2D = Solarmax.Singleton<PortraitManager>.Get().GetTexture2D(url);
				if (texture2D == null)
				{
					texture2D = Solarmax.Singleton<PortraitManager>.Get().GetTexture2D("select_head_1.png");
				}
				this.avatar.mainTexture = texture2D;
				this.url = url;
				if (Solarmax.Singleton<SkinConfigProvider>.Get().avatarDic.ContainsKey(url) && Solarmax.Singleton<SkinConfigProvider>.Get().avatarDic[url].skinType == 1)
				{
					int width = this.avatar.width;
					int height = this.avatar.height;
					SkinConfig skinConfig = Solarmax.Singleton<SkinConfigProvider>.Get().avatarDic[url];
					if (this.effect == null)
					{
						UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(skinConfig.bgImage);
						this.effect = (UnityEngine.Object.Instantiate(resources) as GameObject);
					}
					if (this.effect != null)
					{
						string path = "gameres/effect/texture/materials/flare08.mat";
						if (this.scroll != null)
						{
							path = "gameres/effect/texture/materials/flare07.mat";
						}
						this.effect.GetComponent<Renderer>().material = null;
						Material material = LoadResManager.LoadMat(path);
						if (material != null)
						{
							this.effect.GetComponent<Renderer>().material = material;
						}
						this.effect.transform.parent = this.avatar.transform;
						this.effect.transform.localPosition = Vector3.zero;
						this.effect.transform.localScale = new Vector3((float)width * 2.5f, (float)height * 2.5f, (float)width * 2.5f);
						this.effect.SetActive(true);
					}
					else if (this.effect != null)
					{
						this.effect.SetActive(false);
					}
				}
				else if (this.effect != null)
				{
					this.effect.SetActive(false);
				}
			}
			else
			{
				this.url = url;
				base.StartCoroutine(this.UpdateAvatar());
			}
			this.ScrollViewPanel();
		}

		private void ScrollViewPanel()
		{
			if (this.scroll == null)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Debug("NetTexure scrollview为空", new object[0]);
				return;
			}
			UIPanel component = this.scroll.GetComponent<UIPanel>();
			Vector3 vector = UICamera.mainCamera.WorldToViewportPoint(component.worldCorners[0]);
			Vector3 vector2 = UICamera.mainCamera.WorldToViewportPoint(component.worldCorners[2]);
			Vector4 value = new Vector4(vector.x * 2f - 1f, vector.y * 2f - 1f, vector2.x * 2f - 1f, vector2.y * 2f - 1f);
			Shader.SetGlobalVector("effectClipRange", value);
		}

		public void OnClicked()
		{
		}

		private IEnumerator UpdateAvatar()
		{
			if (!string.IsNullOrEmpty(this.url))
			{
				if (this.url.StartsWith("http"))
				{
					WWW www = new WWW(this.url);
					yield return www;
					Texture2D tex = www.texture;
					if (tex != null)
					{
						this.avatar.mainTexture = tex;
					}
                    Solarmax.Singleton<PortraitManager>.Get().AddTexture2D(this.url, tex);
					www.Dispose();
				}
				else
				{
					Texture2D texture2D = Solarmax.Singleton<PortraitManager>.Get().GetTexture2D(this.url);
					if (texture2D != null)
					{
						this.avatar.mainTexture = texture2D;
					}
				}
			}
			yield break;
		}

		public UITexture avatar;

		private string url;

		private const string DEFAULT_ICON = "select_head_1.png";

		private GameObject effect;

		private UIScrollView scroll;
	}
}
