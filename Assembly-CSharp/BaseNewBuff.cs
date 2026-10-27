using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class BaseNewBuff : Lifecycle2
{
	public BaseNewBuff()
	{
		this.toastMsg = string.Empty;
		this.toastDir = 0;
		this.toastAllShow = false;
		this.linkSkill = null;
		this.linkSkillEffectId = 0;
		this.affectNode = new List<Node>();
	}

	public virtual bool Init()
	{
		if (this.config == null)
		{
			return false;
		}
		this.lastTime = this.config.lastTime;
		this.actInterval = this.config.actInterval;
		this.allNode = this.sceneManager.nodeManager.GetUsefulNodeList();
		this.Enable();
		return true;
	}

	public virtual void Tick(int frame, float interval)
	{
		if (!UtilTools.IsZero(this.actInterval))
		{
			this.actInterval -= interval;
			if (this.actInterval <= 0f)
			{
				this.Apply();
				this.actInterval = this.config.actInterval;
			}
		}
		if (this.lastTime > 0f)
		{
			this.lastTime -= interval;
		}
		if (this.config.targetType == 2 && this.targetNode == null && this.config.lastTime > 0f)
		{
			for (int i = 0; i < this.allNode.Count; i++)
			{
				Node node = this.allNode[i];
				if (node.currentTeam == this.targetTeam && !this.affectNode.Contains(node))
				{
					this.PlaySkillEffect(node);
					this.affectNode.Add(node);
				}
			}
		}
	}

	public virtual void Destroy()
	{
		this.Disable();
	}

	public void SetInfo(Team sendteam, Team targetteam, Node targetnode, NewSkillBuffConfig buffconfig, BaseNewSkill skill, SceneManager manager)
	{
		this.sendTeam = sendteam;
		this.targetTeam = targetteam;
		this.targetNode = targetnode;
		this.config = buffconfig;
		this.sceneManager = manager;
		this.linkSkill = skill;
		this.linkSkillEffectId = this.linkSkill.config.effectId;
		this.startTime = this.sceneManager.GetBattleTime();
	}

	public void SetToasts()
	{
		if (this.linkSkill != null)
		{
			this.toastMsg = this.linkSkill.config.tips;
			this.toastDir = this.linkSkill.config.tipsdir;
			this.toastAllShow = this.linkSkill.config.tipsallshow;
		}
	}

	protected virtual void Apply()
	{
		this.PlayBuffEffect(this.targetNode);
	}

	protected virtual void Enable()
	{
		this.Apply();
		this.PlaySkillEffect(this.targetNode);
	}

	protected virtual void Disable()
	{
	}

	protected void PlayBuffEffect(Node node)
	{
		NewSkillEffectConfig data = Solarmax.Singleton<NewSkillEffectConfigProvider>.Instance.GetData(this.config.effectId);
		if (data == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(data.audio))
		{
			Solarmax.Singleton<AudioManger>.Get().PlayEffect(data.audio);
		}
		if (node != null && !string.IsNullOrEmpty(data.effectName))
		{
			Solarmax.Singleton<EffectManager>.Get().PlaySkillEffect(node, data.effectName, data.animationName, this.lastTime, 1f);
		}
	}

	protected void PlaySkillEffect(Node node)
	{
		if (this.linkSkillEffectId <= 0)
		{
			return;
		}
		NewSkillEffectConfig data = Solarmax.Singleton<NewSkillEffectConfigProvider>.Instance.GetData(this.linkSkillEffectId);
		if (data == null)
		{
			return;
		}
		if (node != null && !string.IsNullOrEmpty(data.effectName))
		{
			Solarmax.Singleton<EffectManager>.Get().PlaySkillEffect(node, data.effectName, data.animationName, this.lastTime, 1f);
		}
	}

	protected void ShowToasts(params object[] args)
	{
		if (string.IsNullOrEmpty(this.toastMsg))
		{
			return;
		}
		if (this.toastMsg.Contains("{0}"))
		{
			if (args.Length == 0)
			{
				Debug.LogErrorFormat("Buff show toast need args.   skillId:{0}, buffId:{1}", new object[]
				{
					this.linkSkill.config.skillId,
					this.config.buffId
				});
			}
			this.toastMsg = string.Format(this.toastMsg, args);
		}
		Vector3 vector = Vector3.zero;
		if (this.targetNode != null)
		{
			vector = this.targetNode.GetPosition();
		}
		if (this.toastAllShow)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSkillPop, new object[]
			{
				this.toastDir,
				this.toastMsg,
				vector
			});
		}
		else if (this.sendTeam.team == this.sceneManager.battleData.currentTeam)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSkillPop, new object[]
			{
				this.toastDir,
				this.toastMsg,
				vector
			});
		}
	}

	public bool IsActive()
	{
		return this.config.lastTime < 0f || this.lastTime > 0f;
	}

	public NewSkillBuffConfig config;

	protected Team sendTeam;

	protected BaseNewSkill linkSkill;

	protected SceneManager sceneManager;

	protected Node targetNode;

	protected Team targetTeam;

	protected float actInterval;

	protected float lastTime;

	public float startTime;

	protected string toastMsg;

	protected int toastDir;

	protected bool toastAllShow;

	protected int linkSkillEffectId;

	protected List<Node> affectNode;

	protected List<Node> allNode;
}
