using System;
using Solarmax;
using UnityEngine;

public class BaseNewSkill : Lifecycle2
{
	public virtual bool Init()
	{
		return true;
	}

	public virtual void Tick(int frame, float interval)
	{
		if (this.enable && !this.canUse)
		{
			this.cd -= interval;
			if (this.cd <= 0f)
			{
				this.canUse = true;
				this.cd = 0f;
			}
		}
	}

	public virtual void Destroy()
	{
		this.owerNode = null;
	}

	public void SetInfo(Team host, NewSkillConfig skillconfig, SceneManager sceneMger, NewSkillManager skillMger)
	{
		this.sceneManager = sceneMger;
		this.skillManager = skillMger;
		this.hostTeam = host;
		this.config = skillconfig;
		if (this.config.type == 0)
		{
			this.cd = 0f;
			this.cdTotal = 0f;
			this.canUse = true;
			this.enable = true;
		}
		else if (this.config.type == 1)
		{
			this.cd = this.config.firstcd;
			this.cdTotal = this.cd;
			this.canUse = false;
			this.enable = true;
		}
	}

	public void ResetSkillForNode(Team host)
	{
		this.hostTeam = host;
		this.cd = this.config.firstcd;
		this.cdTotal = this.cd;
		this.canUse = false;
		this.enable = true;
	}

	public virtual void OnBorn(Team t)
	{
	}

	public virtual void OnCaptured(Node node, Team before, Team now)
	{
	}

	public virtual void OnBeCaptured(Node Node, Team before, Team now)
	{
	}

	public virtual void OnOccupied(Node node, Team now)
	{
	}

	protected void PlaySkillAudio()
	{
		NewSkillEffectConfig data = Solarmax.Singleton<NewSkillEffectConfigProvider>.Instance.GetData(this.config.effectId);
		if (data == null)
		{
			return;
		}
		Solarmax.Singleton<AudioManger>.Get().PlayEffect(data.audio);
	}

	protected void ShowToasts(params object[] args)
	{
		string text = this.config.tips;
		int tipsdir = this.config.tipsdir;
		bool tipsallshow = this.config.tipsallshow;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (text.Contains("{0}"))
		{
			if (args.Length == 0)
			{
				Debug.LogErrorFormat("Skill show toast need args.   skillId:{0}", new object[]
				{
					this.config.skillId
				});
			}
			text = string.Format(text, args);
		}
		Vector3 zero = Vector3.zero;
		if (tipsallshow)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSkillPop, new object[]
			{
				tipsdir,
				text,
				zero
			});
		}
	}

	public virtual void ReadySkill(Team castTeam, Node node, bool playEffect)
	{
	}

	public virtual bool OnCast(Team castTeam)
	{
		if (this.enable && this.canUse)
		{
			this.canUse = false;
			if (this.config.reuse)
			{
				this.cd = this.config.cd;
				this.cdTotal = this.cd;
			}
			else
			{
				this.cdTotal = 0f;
			}
			return true;
		}
		return false;
	}

	public float GetCD()
	{
		return this.cd;
	}

	public float GetCDTotal()
	{
		return this.cdTotal;
	}

	public bool CanUse()
	{
		return this.canUse;
	}

	public bool Enable()
	{
		return this.enable;
	}

	protected void AddBuff(Node targetNode, Team targetTeam, BaseNewBuff buff, NewSkillBuffConfig buffConfig)
	{
		if (buffConfig.targetType == 0)
		{
			targetNode.skillBuffLogic.Add(buff);
		}
		else if (buffConfig.targetType == 2)
		{
			targetTeam.skillBuffLogic.Add(buff);
		}
		else if (buffConfig.targetType == 1)
		{
		}
	}

	protected SceneManager sceneManager;

	protected NewSkillManager skillManager;

	public NewSkillConfig config;

	protected Team hostTeam;

	public Node owerNode;

	public bool isFirst;

	protected float cd;

	protected float cdTotal;

	protected bool canUse;

	protected bool enable;

	public string scriptMisc;
}
