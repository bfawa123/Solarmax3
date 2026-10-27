using System;
using Plugin;
using Solarmax;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CannonEffect : EffectNode
{
	public float speed { get; set; }

	public float recycleTime { get; set; }

	public float missleTime { get; set; }

	public Node node { get; set; }

	public Node attackNode { get; set; }

	private ParticleSystem[] effectList { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		if (!this.pause && !this.isBreak)
		{
			if (this.missleTime > 0f)
			{
				if (this.attackNode == null)
				{
					this.missleTime = 0f;
					this.recycleTime = 0f;
					return;
				}
				this.missleTime -= interval * this.speed;
				Vector3 vector = this.BezierPoint();
				base.go.transform.position = new Vector3(vector.x, vector.y, -1f);
				base.go.transform.localEulerAngles = new Vector3(0f, 0f, -90f + vector.z);
			}
			else
			{
				this.missileImage.SetActive(false);
			}
			this.delta += interval * this.speed;
			if (this.recycleTime > 0f)
			{
				this.recycleTime -= interval * this.speed;
			}
			else
			{
				base.Recycle(this);
			}
		}
	}

	public override void Destroy()
	{
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		base.Destroy();
	}

	public override void OnRecycle()
	{
		this.SetEffectStop(this.effectList);
		base.OnRecycle();
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("EFF_XJ_Nuclear");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.effectList = base.go.GetComponentsInChildren<ParticleSystem>();
			this.missileImage = base.go.transform.Find("image").transform.gameObject;
		}
		this.missileImage.SetActive(true);
		base.go.transform.SetParent(this.node.GetGO().transform.parent);
		base.go.transform.position = new Vector3(this.node.GetPosition().x, this.node.GetPosition().y, -1f);
		base.go.SetActive(true);
		this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		this.lastPoint = base.go.transform.position;
		base.go.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.delta = 0f;
		this.lastDelta = 0f;
		this.isBreak = false;
		this.CD = this.missleTime;
		this.SetColor();
		this.SetMidPoint();
		this.SetAttackPoint();
		this.PlayEffect(this.effectList);
	}

	private void SetColor()
	{
		this.effectList[0].gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(this.node.currentTeam.color.r, this.node.currentTeam.color.g, this.node.currentTeam.color.b, 0.5f));
		this.effectList[1].gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(this.node.currentTeam.color.r, this.node.currentTeam.color.g, this.node.currentTeam.color.b, 1f));
		Transform transform = base.go.transform.Find("image");
		if (transform != null)
		{
			transform.GetComponent<SpriteRenderer>().color = new Color(this.node.currentTeam.color.r, this.node.currentTeam.color.g, this.node.currentTeam.color.b, 1f);
		}
	}

	private void SetAttackPoint()
	{
		if (this.attackNode.revoType == RevolutionType.RT_None)
		{
			this.attackPoint = this.attackNode.GetPosition();
		}
		else
		{
			this.attackPoint = this.attackNode.GetNodeRunPosition(this.missleTime);
		}
	}

	private void SetMidPoint()
	{
		float num = 0.1f;
		float num2 = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.Range(2f, 4f);
		if (Mathf.Abs(this.node.GetPosition().x - this.attackNode.GetPosition().x) < 1f)
		{
			num = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.Range(-3f, 3f);
		}
		this.midPoint = new Vector2(this.node.GetPosition().x + num, this.node.GetPosition().y + num2);
	}

	private void SetPause(bool pause)
	{
		if (base.go != null && this.effectList != null)
		{
			if (pause)
			{
				this.pause = pause;
				this.SetEffectPause(this.effectList);
			}
			else
			{
				this.pause = pause;
				this.SetEffectDelay(this.effectList);
				this.SetEffectPlay(this.effectList);
			}
		}
	}

	private void SetEffectSpeed(float speed)
	{
		this.speed = speed;
		if (base.go != null && this.effectList != null)
		{
			foreach (ParticleSystem particleSystem in this.effectList)
			{
				MainModule mm = particleSystem.main;
				mm.simulationSpeed = speed;
			}
		}
	}

	private void SetEffectPause(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Pause(false);
			}
		}
	}

	private void PlayEffect(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Simulate(0f, false, true);
				particleSystem.Play(false);
			}
		}
	}

	private void SetEffectPlay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Play(false);
			}
		}
	}

	private void SetEffectStop(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Stop(false);
			}
		}
	}

	private void SetEffectDelay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (!(particleSystem == null))
			{
				if (!particleSystem.isPlaying)
				{
					ParticleSystem.MainModule main = particleSystem.main;
					float num = main.startDelay.constant;
					if (num - this.delta > 0f)
					{
						num -= this.delta;
						main.startDelay = num;
					}
				}
			}
		}
		this.lastDelta = this.delta;
	}

	private Vector3 BezierPoint()
	{
		float num = this.delta / this.CD;
		Vector2 to = (1f - num) * (1f - num) * (Vector2)this.node.GetPosition() + 2f * num * (1f - num) * this.midPoint + num * num * this.attackPoint;
		float angle360BetweenVector = UtilTools.GetAngle360BetweenVector2(this.lastPoint, to);
		this.lastPoint = to;
		return new Vector3(to.x, to.y, angle360BetweenVector);
	}

	private const string EFF_NAME = "EFF_XJ_Nuclear";

	public const float effectTime = 10f;

	public float lastDelta;

	private bool pause;

	private float delta;

	private bool isBreak;

	private float shrinkTime;

	private float CD;

	private Vector2 midPoint;

	private Vector2 lastPoint;

	private Vector2 attackPoint;

	private GameObject missileImage;
}
