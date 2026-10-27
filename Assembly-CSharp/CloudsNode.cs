using System;
using System.Collections.Generic;
using Solarmax;

public class CloudsNode : Node
{
	public CloudsNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Clouds;
		}
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		if (this.currStatus == CloudsNode.CloudsStatus.CS_RandomTarget)
		{
			this.CalcTargetPosition();
		}
		else if (this.currStatus == CloudsNode.CloudsStatus.CS_FadeIn)
		{
			this.curAlpha += 2.5f * interval;
			if (this.curAlpha > 1f)
			{
				this.currStatus = CloudsNode.CloudsStatus.CS_None;
				this.curAlpha = 1f;
			}
			base.entity.SetPosition(this.currNode.GetPosition());
			base.entity.SetAlpha(this.curAlpha);
		}
		else if (this.currStatus == CloudsNode.CloudsStatus.CS_FadeOut)
		{
			this.curAlpha -= 2.5f * interval;
			if (this.curAlpha < 0f)
			{
				this.currStatus = CloudsNode.CloudsStatus.CS_FadeIn;
				this.curAlpha = 0f;
			}
			base.entity.SetAlpha(this.curAlpha);
		}
		this.curGrandtotal += interval;
		if (this.curGrandtotal >= 10f)
		{
			this.curGrandtotal = 0f;
			this.currStatus = CloudsNode.CloudsStatus.CS_RandomTarget;
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void CalcTargetPosition()
	{
		List<Node> usefulNodeList = base.sceneManager.nodeManager.GetUsefulNodeList();
		int count = usefulNodeList.Count;
		if (count < 1)
		{
			return;
		}
		this.nodelist.Clear();
		for (int i = 0; i < usefulNodeList.Count; i++)
		{
			if (usefulNodeList[i].CanBeTarget())
			{
				this.nodelist.Add(usefulNodeList[i]);
			}
		}
		if (this.currNode != null)
		{
			this.nodelist.Remove(this.currNode);
		}
		count = this.nodelist.Count;
		if (count < 1)
		{
			return;
		}
		int index = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.Range(0, count);
		this.curAlpha = 1f;
		this.currNode = this.nodelist[index];
		this.currStatus = CloudsNode.CloudsStatus.CS_FadeOut;
	}

	public override bool CanBeTarget()
	{
		return false;
	}

	private float curAlpha = 1f;

	private float curGrandtotal;

	private Node currNode;

	private CloudsNode.CloudsStatus currStatus;

	private List<Node> nodelist = new List<Node>();

	public enum CloudsStatus
	{
		CS_None,
		CS_RandomTarget,
		CS_FadeIn,
		CS_FadeOut
	}
}
