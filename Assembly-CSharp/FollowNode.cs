using System;
using Solarmax;
using UnityEngine;

public class FollowNode : MonoBehaviour
{
	private void Awake()
	{
		this.mTrans = base.transform;
	}

	public void Follow(string targetTag, Vector3 offset)
	{
		this.mTargetTag = targetTag;
		this.mOffset = offset;
		this.UpdatePos();
	}

	public void ClearFollow()
	{
		this.mTargetTag = null;
		this.mOffset = Vector3.zero;
	}

	private void LateUpdate()
	{
		this.UpdatePos();
	}

	private void UpdatePos()
	{
		if (this.mTargetTag == null)
		{
			return;
		}
		Node node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(this.mTargetTag);
		if (node == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.mTrans.localPosition = node.GetPosition() + this.mOffset;
	}

	public bool IsTarget(string notTag)
	{
		return this.mTargetTag == notTag;
	}

	private Vector3 mOffset;

	private string mTargetTag;

	private Transform mTrans;
}
