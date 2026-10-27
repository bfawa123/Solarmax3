using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public abstract class Entity : Lifecycle2
{
	public Entity(string name, bool silent)
	{
		this.tag = name;
		this.silent = silent;
	}

	public string tag { get; set; }

	public string perfab { get; set; }

	protected GameObject go { get; set; }

	protected SpriteRenderer image { get; set; }

	protected float dist { get; set; }

	protected float width { get; set; }

	public float nodesize { get; set; }

	public virtual bool Init()
	{
		this.InitGameObject();
		return true;
	}

	public virtual void Tick(int frame, float interval)
	{
	}

	public virtual void Destroy()
	{
		this.silent = false;
		if (this.go != null)
		{
			UnityEngine.Object.Destroy(this.go);
		}
		this.go = null;
	}

	public void SetPosition(float x, float y)
	{
		this.SetPosition(new Vector3(x, y, 0f));
	}

	protected virtual void InitGameObject()
	{
		this.scale = 1f;
		this.go = this.CreateGameObject();
		this.transform = this.go.transform;
		this.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		this.transform.localScale = Vector3.one;
		this.image = this.transform.Find("image").GetComponent<SpriteRenderer>();
		this.ImageTransform = this.image.transform;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.ImageTransform.Rotate(Vector3.forward, 90f);
			this.imageAngle = 90f;
		}
		this.color = this.image.color;
		this.eulerAngles = this.transform.localEulerAngles;
	}

	protected virtual GameObject CreateGameObject()
	{
		return null;
	}

	public virtual void SetPosition(Vector3 pos)
	{
		this.position = pos;
		if (this.silent)
		{
			return;
		}
		this.transform.position = pos;
	}

	public Vector3 GetPosition()
	{
		return this.position;
	}

	public void SetScale(float scale)
	{
		this.scale = scale;
		this.dist = scale * 0.25f * 0.5625f;
		this.width = this.dist;
		if (this.silent)
		{
			return;
		}
		if (this.go == null)
		{
			return;
		}
		this.transform.localScale = Vector3.one * scale;
		this.dist = scale * 0.25f * 0.5625f;
		this.width = this.dist;
	}

	public float GetScale()
	{
		return this.scale;
	}

	public float GetDist()
	{
		return this.GetScale();
	}

	public float GetWidth()
	{
		return this.GetScale();
	}

	public float GetHalfNodeSize()
	{
		return this.nodesize;
	}

	public virtual void SetColor(Color color)
	{
		this.color = color;
		if (this.silent)
		{
			return;
		}
		if (this.image == null)
		{
			return;
		}
		this.image.color = color;
	}

	public virtual void SetHaloColor(Color color)
	{
	}

	public Color GetColor()
	{
		return this.color;
	}

	public virtual void AddPoint(List<Vector3> ls)
	{
	}

	public virtual void SetAlpha(float alpha)
	{
		this.color.a = alpha;
		if (this.silent)
		{
			return;
		}
		if (this.image == null)
		{
			return;
		}
		this.image.color = this.color;
	}

	public void SetRotation(Vector3 r3)
	{
		this.eulerAngles = r3;
		if (this.silent)
		{
			return;
		}
		if (this.go != null)
		{
			this.transform.localEulerAngles = r3;
		}
	}

	public void RotateImage(float angle)
	{
		if (this.silent)
		{
			return;
		}
		if (this.image == null)
		{
			return;
		}
		this.ImageTransform.Rotate(Vector3.forward, angle);
		if (this.go != null)
		{
			if (this.glowTransform == null)
			{
				this.glowTransform = this.go.transform.Find("shape").transform;
			}
			if (this.glowTransform != null)
			{
				this.glowTransform.Rotate(Vector3.forward, angle);
			}
		}
		this.imageAngle += angle;
	}

	public void TweenRotate(Vector3 from, Vector3 to, float duration, float angle)
	{
		TweenRotation tr = this.image.GetComponent<TweenRotation>();
		if (tr == null)
		{
			return;
		}
		tr.ResetToBeginning();
		tr.from = from;
		tr.to = to;
		tr.duration = duration;
		tr.onFinished.Add(new EventDelegate(delegate()
		{
			tr.onFinished.Clear();
			this.imageAngle = to.z;
		}));
		tr.Play(true);
	}

	public float GetImageRotation()
	{
		return this.imageAngle;
	}

	public GameObject GetGO()
	{
		return this.go;
	}

	public void SetParent(Transform transform)
	{
		if (this.go == null)
		{
			return;
		}
		transform.SetParent(transform);
	}

	public void ReName(string name)
	{
		this.go.name = name;
	}

	public virtual void FadeEntity(bool bFadeIn, float duration)
	{
	}

	public virtual void CalcGlowShape(Color color)
	{
	}

	public virtual void SilentMode(bool status)
	{
		bool flag = this.silent;
		this.silent = status;
		if (flag && !this.silent)
		{
			this.Resuming();
		}
	}

	public virtual void Resuming()
	{
		this.SetColor(this.color);
		this.SetPosition(this.position);
		this.SetScale(this.scale);
		this.SetRotation(this.eulerAngles);
	}

	protected Transform transform;

	protected Transform ImageTransform;

	private float imageAngle;

	protected float scale;

	protected string name = "entity";

	protected Vector3 position = Vector3.zero;

	protected Color color = Color.white;

	protected Vector3 eulerAngles = Vector3.zero;

	public bool silent;

	private Transform glowTransform;
}
