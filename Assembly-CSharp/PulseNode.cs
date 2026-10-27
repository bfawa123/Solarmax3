using System;
using Solarmax;
using UnityEngine;

public class PulseNode : EffectNode
{
	public PulseType type { get; set; }

	public Vector3 position { get; set; }

	public Node currentNode { get; set; }

	public int depth { get; set; }

	public Color color { get; set; }

	private PulseNode.BomberState bombSteate { get; set; }

	private SpriteRenderer pulse { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateLogic(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		switch (this.type)
		{
		case PulseType.Bomb:
			if (this.currentNode != null)
			{
				this.currentNode.bomber--;
			}
			break;
		}
		this.type = PulseType.None;
		base.isActive = false;
		if (base.go != null)
		{
			base.go.SetActive(false);
		}
		if (base.go != null)
		{
			base.go.transform.position = Vector3.one * 1000f;
		}
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.currentNode != null && this.currentNode.nodeManager != null)
		{
			Node node = this.currentNode.nodeManager.GetNode(this.currentNode.tag);
			if (node != null && node != this.currentNode)
			{
				this.currentNode = node;
			}
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Pulse");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.pulse = base.go.GetComponentInChildren<SpriteRenderer>();
			base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			base.go.transform.position = Vector3.one * 1000f;
			switch (this.type)
			{
			case PulseType.MakeShip:
				base.go.transform.localScale = Vector3.one;
				break;
			case PulseType.Bomb:
				base.go.transform.localScale = Vector3.one * 0.5f;
				this.bombSteate = PulseNode.BomberState.Glow;
				if (this.currentNode != null && anim)
				{
					this.currentNode.bomber++;
				}
				break;
			}
		}
		else
		{
			switch (this.type)
			{
			case PulseType.MakeShip:
				base.go.transform.localScale = Vector3.one;
				break;
			case PulseType.Bomb:
				base.go.transform.localScale = Vector3.one * 0.5f;
				break;
			}
			base.go.transform.position = this.position;
			this.bombSteate = PulseNode.BomberState.Glow;
		}
		this.pulse.color = this.color;
		Color color = this.pulse.color;
		switch (this.type)
		{
		case PulseType.MakeShip:
			color.a = 0f;
			this.pulse.color = color;
			break;
		case PulseType.Bomb:
			color.a = 1f;
			this.pulse.color = color;
			break;
		}
		base.go.SetActive(anim);
		base.isActive = anim;
	}

	public void UpdateLogic(int frame, float dt)
	{
		PulseType type = this.type;
		if (type != PulseType.Bomb)
		{
			if (type == PulseType.MakeShip)
			{
				this.UpdateBuildShip(frame, dt);
			}
		}
		else
		{
			this.UpdateBomb(frame, dt);
		}
	}

	private void UpdateBuildShip(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		Vector3 localScale = base.go.transform.localScale;
		Color color = this.pulse.color;
		if (localScale.x > 0f)
		{
			color.a += dt * 0.25f;
			localScale.x = (localScale.y -= dt * 0.5f);
			if (color.a > 0.7f)
			{
				color.a = 0.7f;
			}
			this.pulse.color = color;
			base.go.transform.localScale = localScale;
		}
		else
		{
			base.Recycle(this);
		}
	}

	private void UpdateBomb(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		switch (this.bombSteate)
		{
		case PulseNode.BomberState.None:
			base.Recycle(this);
			break;
		case PulseNode.BomberState.Glow:
		{
			Vector3 localScale = base.go.transform.localScale;
			float num = localScale.x;
			num += Time.deltaTime * 2f;
			if (num > 0.6f)
			{
				num = 0.6f;
			}
			localScale.x = (localScale.y = num);
			base.go.transform.localScale = localScale;
			if (num >= 0.6f)
			{
				this.bombSteate = PulseNode.BomberState.Shrink;
			}
			break;
		}
		case PulseNode.BomberState.Shrink:
		{
			Vector3 localScale2 = base.go.transform.localScale;
			float num2 = localScale2.x;
			num2 += Time.deltaTime * 0.5f;
			if (num2 > 1.5f)
			{
				num2 = 1.5f;
			}
			localScale2.x = (localScale2.y = num2);
			Color color = this.pulse.color;
			color.a -= Time.deltaTime;
			this.pulse.color = color;
			base.go.transform.localScale = localScale2;
			if (num2 >= 1.5f)
			{
				this.bombSteate = PulseNode.BomberState.None;
			}
			break;
		}
		}
	}

	private enum BomberState
	{
		None,
		Glow,
		Shrink
	}
}
