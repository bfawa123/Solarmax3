using System;
using Plugin;
using Solarmax;
using UnityEngine;

public class UnknownStarBehivor : MonoBehaviour
{
	private void Start()
	{
		Node.onTeamValid = (Node.OnTeamValid)Delegate.Combine(Node.onTeamValid, new Node.OnTeamValid(this.OnTeamValid));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetSpeed));
	}

	private void Update()
	{
		if (this.ani.speed > 0f)
		{
			this.life -= Time.deltaTime;
			if (this.life <= 0f)
			{
				UnityEngine.Object.Destroy(this);
			}
		}
	}

	private void OnDestroy()
	{
		Node.onTeamValid = (Node.OnTeamValid)Delegate.Remove(Node.onTeamValid, new Node.OnTeamValid(this.OnTeamValid));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetSpeed));
	}

	private void OnTeamValid(bool visible, Node node)
	{
		if (this.isFirst || node != this.node || this.node.id == 40)
		{
			return;
		}
		base.gameObject.SetActive(visible);
	}

	private void SetSpeed(float speed)
	{
		this.ani.speed = speed;
	}

	private void SetPause(bool pause)
	{
		this.ani.speed = ((!pause) ? Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed : 0f);
	}

	public void ChangeImage(string cImage, string cImageShape, bool first, Node node)
	{
		base.gameObject.transform.localPosition = Vector3.zero;
		this.changeImage.spriteName = cImage;
		this.changeImageShape.spriteName = cImageShape;
		this.ani.enabled = true;
		this.ani.Play("Entity_Master");
		this.isFirst = first;
		this.node = node;
		this.ani.speed = Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed;
	}

	public Animator ani;

	public UISprite variety;

	public UISprite changeImage;

	public UISprite changeImageShape;

	private bool isFirst;

	private Node node;

	private float life = 6f;
}
