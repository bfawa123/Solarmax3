using System;
using Solarmax;
using UnityEngine;

public class CurseSkill : MonoBehaviour
{
	private float k
	{
		get
		{
			return this.kValue;
		}
		set
		{
			this.kValue = value;
			if (this.node != null)
			{
				this.node.skillK = this.kValue;
			}
		}
	}

	private float b
	{
		get
		{
			return this.bValue;
		}
		set
		{
			this.bValue = value;
			if (this.node != null)
			{
				this.node.skillB = this.bValue;
			}
		}
	}

	private void Awake()
	{
		this.animator = base.transform.gameObject.GetComponent<Animator>();
	}

	private void OnEnable()
	{
	}

	public void EnsureInit(CurseNode node, CurseEffect effect)
	{
		this.node = node;
		this.k = Mathf.Tan(this.center.eulerAngles.z * 3.1415927f / 180f);
		this.b = this.center.position.y - this.k * this.center.position.x;
		this.curseEffect = effect;
	}

	private void OnDisable()
	{
	}

	public void SetAnimatorTime(float time)
	{
	}

	public void EndReady()
	{
		if (this.curseEffect == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("[CurseEffect]{0}", new object[]
			{
				"伽马射线特效为空"
			});
			return;
		}
		this.curseEffect.PlayFireEffect();
	}

	public void StartFire()
	{
	}

	public void EndFire()
	{
	}

	public Transform center;

	public GameObject weapon;

	private Animator animator;

	private CurseNode node;

	private const float RADIUS = 0.5f;

	private float kValue;

	private float bValue;

	private CurseEffect curseEffect;
}
