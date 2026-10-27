using System;
using Solarmax;
using UnityEngine;

public class GuideNode
{
	public void LogicNode()
	{
		if (this.m_fsm == GuideNode.GuideFSM.delay)
		{
			this.fDelayTime -= Time.deltaTime;
			if (this.fDelayTime <= 0f)
			{
				this.m_fsm = GuideNode.GuideFSM.ani;
			}
		}
		else if (this.m_fsm == GuideNode.GuideFSM.ani)
		{
			this.AnimationGuide();
		}
	}

	public int GetConfigID()
	{
		return this.mCurGuideConfig.id;
	}

	public void InitGuideNode(CTagGuideConfig config, GameObject CtrlGameObject)
	{
		this.mCtrlObject = CtrlGameObject;
		this.mCurGuideConfig = config;
		this.mSrcPosition = new Vector3(this.mCurGuideConfig.srcX, this.mCurGuideConfig.srcY, 0f);
		this.m_fsm = GuideNode.GuideFSM.delay;
		this.fDelayTime = this.mCurGuideConfig.duration;
		this.mCtrlObject.transform.localPosition = this.mSrcPosition;
		if (this.mCurGuideConfig.OcclusionBG > 0)
		{
			Solarmax.Singleton<BattleSystem>.Instance.canOperation = false;
		}
		if (this.mCurGuideConfig.type == GuideType.Ani)
		{
			this.mCtrlObject.transform.rotation = Quaternion.Euler(0f, 0f, this.mCurGuideConfig.angle);
			Transform transform = this.mCtrlObject.transform.Find("label");
			if (transform != null)
			{
				UILabel component = transform.GetComponent<UILabel>();
				if (component != null && component.mKey > 0)
				{
					string value = LanguageDataProvider.GetValue(component.mKey);
					component.text = value.Replace("\\n", "\n");
				}
			}
		}
		else if (this.mCurGuideConfig.type == GuideType.Tip)
		{
			UILabel component2 = this.mCtrlObject.transform.Find("desc").GetComponent<UILabel>();
			if (component2 != null)
			{
				component2.text = LanguageDataProvider.GetValue(this.mCurGuideConfig.tipID);
			}
		}
		else if (this.mCurGuideConfig.type == GuideType.Window)
		{
			GuideTipCell component3 = this.mCtrlObject.GetComponent<GuideTipCell>();
			string value2 = LanguageDataProvider.GetValue(this.mCurGuideConfig.tipID);
			component3.SetLevelCell(this.mCurGuideConfig.ctrlname, value2);
		}
	}

	public bool Completed(GuildEndEvent endCondition)
	{
		if (endCondition == GuildEndEvent.animend)
		{
			return this.IsPlayedAnimator();
		}
		return this.mCurGuideConfig != null && (this.mCurGuideConfig.endCondition1 == endCondition || this.mCurGuideConfig.endCondition2 == endCondition);
	}

	public void EndGuideNode()
	{
		Solarmax.Singleton<BattleSystem>.Instance.canOperation = true;
		this.mCtrlObject.SetActive(false);
		this.mRootObject = null;
		this.animator = null;
	}

	private void AnimationGuide()
	{
		if (this.mCtrlObject != null && !this.mCtrlObject.activeSelf)
		{
			this.mCtrlObject.SetActive(true);
		}
		if (this.mCurGuideConfig.moveType == BtnMoveType.BMT_Null)
		{
			return;
		}
		if (this.animator != null && !string.IsNullOrEmpty(this.mCurGuideConfig.aniName))
		{
			this.animator.Play(this.mCurGuideConfig.aniName);
			this.bAnimatorPlayed = true;
		}
		this.m_fsm = GuideNode.GuideFSM.Idle;
	}

	public bool IsPlayedAnimator()
	{
		if (this.animator == null || !this.bAnimatorPlayed)
		{
			return false;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return currentAnimatorStateInfo.IsName(this.mCurGuideConfig.aniName) && currentAnimatorStateInfo.normalizedTime > 1f;
	}

	private void FadeGuideButton(bool bFadeIn)
	{
	}

	private Vector3 ChangeWorldPos2UILocal(Vector3 pos)
	{
		Vector3 position = Camera.main.WorldToScreenPoint(pos);
		position.z = 0f;
		Vector3 point = UICamera.currentCamera.ScreenToWorldPoint(position);
		return this.mRootObject.transform.worldToLocalMatrix.MultiplyPoint(point);
	}

	public CTagGuideConfig mCurGuideConfig;

	public GameObject mRootObject;

	private GameObject mCtrlObject;

	public Animator animator;

	public GuideNode.GuideFSM m_fsm;

	private Vector3 mSrcPosition = Vector3.zero;

	private float fDelayTime;

	private bool bAnimatorPlayed;

	public enum GuideFSM
	{
		Idle,
		delay,
		fadein,
		ani,
		fadeout
	}
}
