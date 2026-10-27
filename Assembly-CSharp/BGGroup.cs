using System;
using UnityEngine;

public class BGGroup : MonoBehaviour
{
	public int ImageCount
	{
		get
		{
			if (this.mImages == null)
			{
				return 0;
			}
			return this.mImages.Length;
		}
	}

	public void Init()
	{
		this.mGo = base.gameObject;
		this.mTrans = base.transform;
		int childCount = this.mTrans.childCount;
		this.mImages = new BGGroup.Image[childCount];
		this.mImageTotalLength = 0f;
		for (int i = 0; i < childCount; i++)
		{
			BGGroup.Image image = new BGGroup.Image();
			Transform child = this.mTrans.GetChild(i);
			image.Init(child, i);
			this.mImages[i] = image;
			this.mImageTotalLength += image.ImageLength;
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < this.mImages.Length; i++)
		{
			this.mImages[i].Destroy();
			this.mImages[i] = null;
		}
		this.mImages = null;
	}

	private void Update()
	{
		this.CheckScroll();
	}

	private void UpdatePosRange()
	{
		if (null == this.mCamera)
		{
			return;
		}
		float num = this.mCamera.orthographicSize * this.mCamera.aspect;
		if (Mathf.Abs(this.mHalfCameraLength - num) < 0.001f)
		{
			return;
		}
		this.mHalfCameraLength = num;
		float x = this.mTrans.localScale.x;
		this.mPosRange.x = -this.mHalfCameraLength + this.mImages[0].ImageLength * 0.5f * x;
		this.mPosRange.y = this.mHalfCameraLength - (this.mImageTotalLength - this.mImages[0].ImageLength * 0.5f) * x;
	}

	public void Scroll(float delta)
	{
		Vector3 position = this.mTrans.position;
		position.x += delta;
		this.mTrans.position = position;
		this.CheckScroll();
	}

	private bool CheckScroll()
	{
		if (this.mImages.Length < 2)
		{
			return false;
		}
		if (null == this.mCamera)
		{
			this.mCamera = Camera.main;
		}
		if (null == this.mCamera)
		{
			return false;
		}
		this.UpdatePosRange();
		Vector3 position = this.mTrans.position;
		float x = this.mTrans.localScale.x;
		if (position.x > this.mPosRange.x)
		{
			BGGroup.Image img = this.mImages[this.mImages.Length - 1];
			for (int i = this.mImages.Length - 1; i > 0; i--)
			{
				this.SetImage(i, this.mImages[i - 1]);
			}
			this.SetImage(0, img);
			position.x -= this.mImages[0].ImageLength * x;
			this.mTrans.position = position;
			return true;
		}
		if (position.x < this.mPosRange.y)
		{
			BGGroup.Image img2 = this.mImages[0];
			for (int j = 0; j < this.mImages.Length - 1; j++)
			{
				this.SetImage(j, this.mImages[j + 1]);
			}
			this.SetImage(this.mImages.Length - 1, img2);
			position.x += this.mImages[this.mImages.Length - 1].ImageLength * x;
			this.mTrans.position = position;
			return true;
		}
		return false;
	}

	private void SetImage(int index, BGGroup.Image img)
	{
		img.SetIndex(index);
		this.mImages[index] = img;
	}

	public void SetTexture(int imageIndex, Texture2D tex)
	{
		if (imageIndex < 0 || imageIndex >= this.mImages.Length)
		{
			Debug.LogErrorFormat("Invalid image index {0}!", new object[]
			{
				imageIndex
			});
			return;
		}
		this.mImages[imageIndex].SetTexture(tex);
	}

	public void SetTextureAll(Texture2D tex)
	{
		if (this.mImages == null)
		{
			return;
		}
		for (int i = 0; i < this.mImages.Length; i++)
		{
			this.mImages[i].SetTexture(tex);
		}
	}

	public void SetVisible(bool b)
	{
		this.mGo.SetActive(b);
	}

	public void SetMotionVectorLength(float f)
	{
		for (int i = 0; i < this.mImages.Length; i++)
		{
			this.mImages[i].SetMotionVectorLength(f);
		}
	}

	private GameObject mGo;

	private Transform mTrans;

	private BGGroup.Image[] mImages;

	private float mImageTotalLength;

	private float mHalfCameraLength;

	private Vector2 mPosRange;

	private Camera mCamera;

	private class Image
	{
		public float ImageLength { get; private set; }

		public bool Init(Transform trans, int index)
		{
			this.mGo = trans.gameObject;
			this.mRenderer = this.mGo.GetComponent<SpriteRenderer>();
			if (null == this.mRenderer)
			{
				this.mGo = null;
				return false;
			}
			this.mTrans = trans;
			this.ImageLength = this.mRenderer.sprite.bounds.size.x;
			this.SetIndex(index);
			return true;
		}

		public void Destroy()
		{
			this.mGo = null;
			this.mTrans = null;
			this.mRenderer = null;
		}

		public void SetTexture(Texture2D tex)
		{
			if (null == this.mRenderer)
			{
				return;
			}
			Sprite sprite = this.mRenderer.sprite;
			if (null != sprite && sprite.texture == tex)
			{
				Rect textureRect = sprite.textureRect;
				if (Mathf.Abs(textureRect.width - (float)tex.width) < 0.001f && Mathf.Abs(textureRect.height - (float)tex.height) < 0.001f)
				{
					return;
				}
			}
			float pixelsPerUnit = 100f;
			if (sprite != null)
			{
				pixelsPerUnit = sprite.pixelsPerUnit;
			}
			Rect rect = new Rect(0f, 0f, (float)tex.width, (float)tex.height);
			this.mRenderer.sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
			if (sprite != null && sprite.texture != null && sprite.texture != tex)
			{
				Resources.UnloadAsset(sprite.texture);
			}
		}

		public void SetIndex(int index)
		{
			this.mIndex = index;
			if (null == this.mTrans)
			{
				return;
			}
			Vector3 localPosition = this.mTrans.localPosition;
			localPosition.x = this.ImageLength * (float)this.mIndex;
			this.mTrans.localPosition = localPosition;
		}

		public void SetMotionVectorLength(float f)
		{
			if (null == this.mMaterial)
			{
				this.mMaterial = this.mRenderer.material;
			}
			this.mMaterial.SetFloat(BGManager.SPIdMotionVectorLength, f);
		}

		private GameObject mGo;

		private Transform mTrans;

		private SpriteRenderer mRenderer;

		private Material mMaterial;

		private int mIndex;
	}
}
