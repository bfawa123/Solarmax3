using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ResourcesUtil
{
	private static bool ShouldUseFixedAspectForStarChart(string starChartOrPath)
	{
		if (string.IsNullOrEmpty(starChartOrPath))
		{
			return true;
		}
		string text = starChartOrPath.Replace('\\', '/');
		if (text.StartsWith("/"))
		{
			text = text.Substring(1);
		}
		int num = text.LastIndexOf('.');
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		return !string.Equals(text, "Btn_CopyUI_22", StringComparison.OrdinalIgnoreCase) && !string.Equals(text, "data/image/Btn_CopyUI_22", StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsExplicitDataImageRef(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		string text = value;
		if (text.StartsWith("/"))
		{
			text = text.Substring(1);
		}
		return text.StartsWith("data/", StringComparison.OrdinalIgnoreCase) || text.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
	}

	private static string NormalizeDataImagePath(string starChart)
	{
		if (string.IsNullOrEmpty(starChart))
		{
			return string.Empty;
		}
		string text = starChart;
		if (text.StartsWith("/"))
		{
			text = text.Substring(1);
		}
		if (text.StartsWith("data/", StringComparison.OrdinalIgnoreCase))
		{
			return text;
		}
		if (text.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
		{
			return "data/" + text;
		}
		return "data/image/" + text;
	}

	private static IEnumerator LoadStreamingTexture(string relativePath, UITexture target)
	{
		if (string.IsNullOrEmpty(relativePath))
		{
			yield break;
		}
		Texture2D texture2D;
		if (ResourcesUtil.streamingTextureCache != null && ResourcesUtil.streamingTextureCache.TryGetValue(relativePath, out texture2D) && texture2D != null)
		{
			if (target != null)
			{
				target.mainTexture = texture2D;
				target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(relativePath);
			}
			yield break;
		}
		texture2D = ResourcesUtil.TryLoadTextureFromAndroidAssets(relativePath);
		if (texture2D != null)
		{
			ResourcesUtil.streamingTextureCache[relativePath] = texture2D;
			if (target != null)
			{
				target.mainTexture = texture2D;
				target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(relativePath);
			}
			yield break;
		}
#if UNITY_EDITOR
		texture2D = ResourcesUtil.TryLoadTextureFromEditorResMirror(relativePath);
		if (texture2D != null)
		{
			ResourcesUtil.streamingTextureCache[relativePath] = texture2D;
			if (target != null)
			{
				target.mainTexture = texture2D;
				target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(relativePath);
			}
			yield break;
		}
#endif
		string url = Application.streamingAssetsPath + "/res/" + relativePath;
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			Texture2D texture = www.texture;
			if (texture != null)
			{
				ResourcesUtil.streamingTextureCache[relativePath] = texture;
				if (target != null)
				{
					target.mainTexture = texture;
					target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(relativePath);
				}
			}
		}
		www.Dispose();
		yield break;
	}

	private static IEnumerator LoadStreamingTextureToBG(string relativePath, BackgroundType backgroundType, int imageIndex)
	{
		if (string.IsNullOrEmpty(relativePath) || BGManager.Inst == null)
		{
			yield break;
		}
		Texture2D texture2D;
		if (ResourcesUtil.streamingTextureCache != null && ResourcesUtil.streamingTextureCache.TryGetValue(relativePath, out texture2D) && texture2D != null)
		{
			BGManager.Inst.SetTextureAll(backgroundType, texture2D);
			yield break;
		}
		texture2D = ResourcesUtil.TryLoadTextureFromAndroidAssets(relativePath);
		if (texture2D != null)
		{
			ResourcesUtil.streamingTextureCache[relativePath] = texture2D;
			if (BGManager.Inst != null)
			{
				BGManager.Inst.SetTextureAll(backgroundType, texture2D);
			}
			yield break;
		}
#if UNITY_EDITOR
		texture2D = ResourcesUtil.TryLoadTextureFromEditorResMirror(relativePath);
		if (texture2D != null)
		{
			ResourcesUtil.streamingTextureCache[relativePath] = texture2D;
			if (BGManager.Inst != null)
			{
				BGManager.Inst.SetTextureAll(backgroundType, texture2D);
			}
			yield break;
		}
#endif
		string url = Application.streamingAssetsPath + "/res/" + relativePath;
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			Texture2D texture = www.texture;
			if (texture != null)
			{
				ResourcesUtil.streamingTextureCache[relativePath] = texture;
				if (BGManager.Inst != null)
				{
					BGManager.Inst.SetTextureAll(backgroundType, texture);
				}
			}
		}
		www.Dispose();
		yield break;
	}

	private static Texture2D TryLoadTextureFromAndroidAssets(string relativePath)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		if (string.IsNullOrEmpty(relativePath))
		{
			return null;
		}
		try
		{
			string text = "res/" + relativePath.ToLowerInvariant().TrimStart(new char[]
			{
				'/'
			});
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getAssets", new object[0]);
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("open", new object[]
			{
				text
			});
			AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.io.ByteArrayOutputStream", new object[0]);
			byte[] array = new byte[16384];
			int num;
			while ((num = androidJavaObject2.Call<int>("read", new object[]
			{
				array
			})) > 0)
			{
				androidJavaObject3.Call("write", new object[]
				{
					array,
					0,
					num
				});
			}
			androidJavaObject2.Call("close", new object[0]);
			byte[] data = androidJavaObject3.Call<byte[]>("toByteArray", new object[0]);
			androidJavaObject3.Call("close", new object[0]);
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			if (!texture2D.LoadImage(data))
			{
				return null;
			}
			return texture2D;
		}
		catch
		{
			return null;
		}
#else
		return null;
#endif
	}

#if UNITY_EDITOR
	private static Texture2D TryLoadTextureFromEditorResMirror(string relativePath)
	{
		if (string.IsNullOrEmpty(relativePath))
		{
			return null;
		}
		try
		{
			string text = relativePath.TrimStart(new char[]
			{
				'/'
			}).Replace('/', Path.DirectorySeparatorChar);
			string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Solarmax3vt3_killer", "assets", "res", text));
			if (!File.Exists(path))
			{
				return null;
			}
			byte[] data = File.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			if (!texture2D.LoadImage(data))
			{
				return null;
			}
			return texture2D;
		}
		catch
		{
			return null;
		}
	}
#endif

	public static Texture2D GetUITexture(string resource)
	{
		return LoadResManager.LoadTex(resource.ToLower());
	}

	public static void SetChapterBgTexture(BackgroundType backgroundType, int imageIndex, string chapterBg)
	{
		if (BGManager.Inst == null || string.IsNullOrEmpty(chapterBg))
		{
			return;
		}
		if (!ResourcesUtil.IsExplicitDataImageRef(chapterBg))
		{
			string resource = "gameres/texture_4/" + chapterBg;
			Texture2D texture2D = GetUITexture(resource);
			if (texture2D != null)
			{
				BGManager.Inst.SetTextureAll(backgroundType, texture2D);
				return;
			}
		}
		string relativePath = ResourcesUtil.NormalizeDataImagePath(chapterBg);
		try
		{
			global::Coroutine.Start(ResourcesUtil.LoadStreamingTextureToBG(relativePath, backgroundType, imageIndex));
		}
		catch
		{
		}
	}

	public static void SetStarChartTexture(UITexture target, string starChart)
	{
		if (target == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(starChart))
		{
			target.mainTexture = null;
			return;
		}
		string text = ResourcesUtil.NormalizeDataImagePath(starChart);
		Texture2D texture2D;
		if (ResourcesUtil.streamingTextureCache != null && ResourcesUtil.streamingTextureCache.TryGetValue(text, out texture2D) && texture2D != null)
		{
			target.mainTexture = texture2D;
			target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(starChart);
			return;
		}
		texture2D = null;
		if (!ResourcesUtil.IsExplicitDataImageRef(starChart))
		{
			string resource = "gameres/texture_4/galaxyicon/" + starChart;
			texture2D = GetUITexture(resource);
			if (texture2D != null)
			{
				target.mainTexture = texture2D;
				target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(starChart);
				return;
			}
		}
		texture2D = LoadResManager.LoadTex(text.ToLower());
		if (texture2D != null)
		{
			target.mainTexture = texture2D;
			target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(text);
			return;
		}
		try
		{
			global::Coroutine.Start(ResourcesUtil.LoadStreamingTexture(text, target));
		}
		catch
		{
		}
		if (target.mainTexture != null)
		{
			target.fixedAspect = ResourcesUtil.ShouldUseFixedAspectForStarChart(text);
		}
	}

	private static readonly Dictionary<string, Texture2D> streamingTextureCache = new Dictionary<string, Texture2D>();
}
