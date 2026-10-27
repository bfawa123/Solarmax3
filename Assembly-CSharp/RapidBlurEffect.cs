using System;
using Solarmax;
using UnityEngine;

[ExecuteInEditMode]
public class RapidBlurEffect : MonoBehaviour
{
	private Material material
	{
		get
		{
			if (this.CurMaterial == null)
			{
				this.CurMaterial = new Material(this.CurShader);
				this.CurMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return this.CurMaterial;
		}
	}

	private void Start()
	{
		RapidBlurEffect.ChangeValue = this.DownSampleNum;
		RapidBlurEffect.ChangeValue2 = this.BlurSpreadSize;
		RapidBlurEffect.ChangeValue3 = this.BlurIterations;
		this.CurShader = Shader.Find(this.ShaderName);
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
			return;
		}
		this.mCamera = base.GetComponent<Camera>();
		if (null == this.mCamera)
		{
			base.enabled = false;
			return;
		}
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (this.CurShader != null)
		{
			float num = 1f / (1f * (float)(1 << this.DownSampleNum));
			this.material.SetFloat("_DownSampleValue", this.BlurSpreadSize * num);
			sourceTexture.filterMode = FilterMode.Bilinear;
			int width = sourceTexture.width >> this.DownSampleNum;
			int height = sourceTexture.height >> this.DownSampleNum;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, sourceTexture.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(sourceTexture, renderTexture);
			int num2 = 0;
			while ((float)num2 < this.BlurIterations)
			{
				float num3 = (float)num2 * 1f;
				this.material.SetFloat("_DownSampleValue", this.BlurSpreadSize * num + num3);
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, sourceTexture.format);
				Graphics.Blit(renderTexture, temporary, this.material, 1);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, sourceTexture.format);
				Graphics.Blit(renderTexture, temporary, this.CurMaterial, 2);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				num2++;
			}
			Graphics.Blit(renderTexture, destTexture);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void OnValidate()
	{
		RapidBlurEffect.ChangeValue = this.DownSampleNum;
		RapidBlurEffect.ChangeValue2 = this.BlurSpreadSize;
		RapidBlurEffect.ChangeValue3 = this.BlurIterations;
	}

	private void Update()
	{
		if (null == this.mCamera)
		{
			return;
		}
		if (this.blurEffect)
		{
			this.delta += Time.deltaTime;
			if (this.addDelta <= this.BLUR_TIME / 2f)
			{
				this.addDelta += Time.deltaTime;
				this.everyDelta += Time.deltaTime;
				if (this.everyDelta > 0.04f)
				{
					this.BlurIterations += this.BLUR_MAX1 / this.everyUICameraValue;
					this.BlurSpreadSize += this.BLUR_MAX2 / this.everyUICameraValue;
					this.everyDelta = 0f;
				}
			}
			else if (this.subDelta <= this.BLUR_TIME / 2f)
			{
				this.subDelta += Time.deltaTime;
				this.everyDelta += Time.deltaTime;
				if (this.everyDelta > 0.04f)
				{
					this.BlurIterations -= this.BLUR_MAX1 / this.everyUICameraValue;
					this.BlurSpreadSize -= this.BLUR_MAX2 / this.everyUICameraValue;
					this.everyDelta = 0f;
				}
			}
			else
			{
				this.BlurIterations = 0f;
				this.BlurSpreadSize = 0f;
				this.everyDelta = 0f;
				this.blurEffect = false;
				this.addDelta = 0f;
				this.subDelta = 0f;
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.AfterUICameraBlurEffect, null);
			}
		}
		if (this.mainCameraBlurEffect)
		{
			this.delta += Time.deltaTime;
			if (this.addDelta <= this.MAIN_BLUR_TIME / 2f)
			{
				this.addDelta += Time.deltaTime;
				this.everyDelta += Time.deltaTime;
				if (this.everyDelta > 0.04f)
				{
					this.BlurIterations += this.BLUR_MAX1 / this.everyMainCameraValue * 2f;
					this.BlurSpreadSize += this.BLUR_MAX2 / this.everyMainCameraValue * 2f;
					this.everyDelta = 0f;
				}
			}
			else if (this.subDelta <= this.BLUR_TIME / 2f)
			{
				this.subDelta += Time.deltaTime;
				this.everyDelta += Time.deltaTime;
				if (this.everyDelta > 0.04f)
				{
					this.BlurIterations -= this.BLUR_MAX1 / this.everyMainCameraValue * 2f;
					this.BlurSpreadSize -= this.BLUR_MAX2 / this.everyMainCameraValue * 2f;
					this.everyDelta = 0f;
				}
			}
			else
			{
				this.BlurIterations = 0f;
				this.BlurSpreadSize = 0f;
				this.everyDelta = 0f;
				this.mainCameraBlurEffect = false;
				this.addDelta = 0f;
				this.subDelta = 0f;
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.AfterMainCameraBlurEffect, null);
			}
		}
		if (this.mainBgScale)
		{
			if (this.mainBgScaleDelta < this.MAIN_BG_SCALE_TIME * this.MAIN_BG_SCALE)
			{
				this.mainBgScaleDelta += Time.deltaTime;
				if (this.mainBgScaleLarge)
				{
					this.mCamera.orthographicSize -= this.EVERY_CAMERA_MOVE_SIZE;
					if (this.mCamera.orthographicSize < this.CAMERA_MOVE_SIZE)
					{
						this.mCamera.orthographicSize = this.CAMERA_MOVE_SIZE;
					}
				}
				else
				{
					this.mCamera.orthographicSize += this.EVERY_CAMERA_MOVE_SIZE;
					if (this.mCamera.orthographicSize > this.CAMERA_MOVE_SIZE)
					{
						this.mCamera.orthographicSize = this.CAMERA_MOVE_SIZE;
					}
				}
			}
			else
			{
				this.mainBgScale = false;
				this.mCamera.orthographicSize = this.CAMERA_MOVE_SIZE;
			}
		}
	}

	private void OnDisable()
	{
		if (this.CurMaterial)
		{
			UnityEngine.Object.DestroyImmediate(this.CurMaterial);
		}
	}

	public void Blur()
	{
		this.blurEffect = true;
		this.delta = 0f;
		this.addDelta = 0f;
		this.subDelta = 0f;
		this.everyUICameraValue = this.BLUR_TIME / 2f / 0.04f;
	}

	public void MainCameraBlur(bool into, float time, float everyCameraSize, float cameraTo)
	{
		this.MAIN_BLUR_TIME = time;
		this.mainCameraBlurEffect = true;
		this.delta = 0f;
		this.addDelta = 0f;
		this.subDelta = 0f;
		this.BlurIterations = 0f;
		this.BlurSpreadSize = 0f;
		this.MainBgScale(into, cameraTo, everyCameraSize);
		this.everyMainCameraValue = this.MAIN_BLUR_TIME / 2f / 0.04f;
	}

	public void MainBgScale(bool large, float cameraTo, float everyCameraSize = 0.015f)
	{
		this.CAMERA_MOVE_SIZE = cameraTo;
		this.EVERY_CAMERA_MOVE_SIZE = everyCameraSize;
		this.mainBgScale = true;
		this.mainBgScaleLarge = large;
		this.mainBgScaleDelta = 0f;
		this.BlurIterations = 0f;
		this.BlurSpreadSize = 0f;
	}

	public GameObject mainBg;

	public GameObject mainCamera;

	private string ShaderName = "Custom/RapidBlurEffect";

	public Shader CurShader;

	private Material CurMaterial;

	public static int ChangeValue;

	public static float ChangeValue2;

	public static float ChangeValue3;

	[Range(0f, 6f)]
	[Tooltip("[降采样次数]向下采样的次数。此值越大,则采样间隔越大,需要处理的像素点越少,运行速度越快。")]
	public int DownSampleNum;

	[Range(0f, 20f)]
	[Tooltip("[模糊扩散度]进行高斯模糊时，相邻像素点的间隔。此值越大相邻像素间隔越远，图像越模糊。但过大的值会导致失真。")]
	public float BlurSpreadSize;

	[Range(0f, 8f)]
	[Tooltip("[迭代次数]此值越大,则模糊操作的迭代次数越多，模糊效果越好，但消耗越大。")]
	public float BlurIterations;

	private bool mainCameraBlurEffect;

	private bool blurEffect;

	private float delta;

	private float addDelta;

	private float subDelta;

	private float everyDelta;

	private bool mainBgScale;

	private bool mainBgScaleLarge;

	private float mainBgScaleDelta;

	private float MAIN_BG_SCALE_TIME = 1f;

	private float BLUR_TIME = 0.6f;

	private float BLUR_MAX1 = 1f;

	private float BLUR_MAX2 = 5f;

	private float BLUR_MAX3;

	private float MAIN_BLUR_TIME = 0.5f;

	private float CAMERA_MOVE_SIZE = 5.5f;

	private float EVERY_CAMERA_MOVE_SIZE = 0.015f;

	private float MAIN_BG_SCALE = 1f;

	private Camera mCamera;

	private float everyUICameraValue;

	private float everyMainCameraValue;
}
