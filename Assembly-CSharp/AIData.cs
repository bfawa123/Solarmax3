using System;
using System.Collections.Generic;
using UnityEngine;

public class AIData
{
	public AIData()
	{
		this.targetList = new List<Node>();
		this.senderList = new List<Node>();
	}

	public virtual void Reset()
	{
		this.aiStrategy = 1;
		this.aiStatus = AIStatus.Idle;
		this.actionTime = 2f;
		this.aiTimeInterval = 2f;
		this.actionDelayDefend = 0f;
		this.actionDelayAttack = 0f;
		this.actionDelayGather = 0f;
		this.aiTimeInterval = 1f;
		this.resultInside = false;
		this.resultIntersects = false;
		this.resultEnter = Vector3.zero;
		this.resultExit = Vector3.zero;
		this.targetList.Clear();
		this.senderList.Clear();
	}

	public int aiStrategy;

	public AIStatus aiStatus;

	public float actionTime;

	public float actionDelayDefend;

	public float actionDelayAttack;

	public float actionDelayGather;

	public float aiTimeInterval;

	public bool resultInside;

	public bool resultIntersects;

	public Vector3 resultEnter;

	public Vector3 resultExit;

	public List<Node> targetList;

	public List<Node> senderList;
}
