using System;
using Plugin;
using Solarmax;
using UnityEngine;

public class EntityShip : Entity
{
	public EntityShip(string name, bool silent) : base(name, silent)
	{
		this.nodeIsHide = false;
	}

	public ShipState shipState { get; set; }

	private new GameObject scale { get; set; }

	public Node targetNode { get; set; }

	private RunLockStepLogic[] handler { get; set; }

	private Ship ship { get; set; }

	private bool vertical { get; set; }

	public override bool Init()
	{
		base.Init();
		this.shipState = ShipState.ORBIT;
		this.handler = new RunLockStepLogic[4];
		this.handler[0] = new RunLockStepLogic(this.UpdateOrbit);
		this.handler[1] = new RunLockStepLogic(this.UpdatePreJump1);
		this.handler[2] = new RunLockStepLogic(this.UpdatePreJump2);
		this.handler[3] = new RunLockStepLogic(this.UpdateJumping);
		return true;
	}

	public override void Tick(int frame, float interval)
	{
		this.handler[(int)this.shipState](frame, interval);
	}

	public void UpdateRender(float interval)
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public void OnRecycle()
	{
		this.fromWarp = false;
		this.shipState = ShipState.ORBIT;
		this.targetFlag = string.Empty;
		base.image.gameObject.layer = LayerDefine.Invisible;
		this.trail.gameObject.layer = LayerDefine.Invisible;
		base.SetScale(1f);
	}

	public void SetForceHide(bool hide)
	{
		if (hide)
		{
			if (base.image != null)
			{
				base.image.gameObject.layer = LayerDefine.Invisible;
			}
			if (this.trailObject != null)
			{
				this.trailObject.layer = LayerDefine.Invisible;
			}
			this.SetColorImage(this.imageColor, 0f);
			this.SetColorTrail(this.trailColor, 0f);
			return;
		}
		if (base.image != null)
		{
			base.image.gameObject.layer = LayerDefine.Default;
		}
		if (this.trailObject != null)
		{
			this.trailObject.layer = LayerDefine.Invisible;
		}
		this.SetColorTrail(this.trailColor, 0f);
	}

	protected override GameObject CreateGameObject()
	{
		if (base.go != null)
		{
			return base.go;
		}
		if (Solarmax.Singleton<ShipFadeManager>.Get().mShipGameObj == null)
		{
			base.go = (Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Ship") as GameObject);
			base.go = UnityEngine.Object.Instantiate<GameObject>(base.go);
			SpriteRenderer component = base.go.transform.Find("scale/image").GetComponent<SpriteRenderer>();
			Solarmax.Singleton<ShipFadeManager>.Get().BackShipMaterial(base.go, component);
			base.go.transform.position = Vector3.one * 1000f;
		}
		return UnityEngine.Object.Instantiate<GameObject>(Solarmax.Singleton<ShipFadeManager>.Get().mShipGameObj);
	}

	protected override void InitGameObject()
	{
		base.go = this.CreateGameObject();
		this.transform = base.go.transform;
		this.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		if (this.scale == null)
		{
			this.scale = base.go.transform.Find("scale").gameObject;
		}
		base.image = this.scale.transform.Find("image").GetComponent<SpriteRenderer>();
		this.ImageTransform = base.image.transform;
		this.imageColor = base.image.color;
		this.imageScale = this.ImageTransform.localScale;
		if (this.trailObject == null)
		{
			this.trailObject = this.scale.transform.Find("trail").gameObject;
			this.trailTransform = this.trailObject.transform;
			this.trail = this.trailObject.GetComponent<SpriteRenderer>();
			this.trailColor = this.trail.color;
			this.trailScale = this.trailTransform.localScale;
		}
		if (this.accelerate == null)
		{
			this.accelerate = this.trailTransform.Find("ship_accelerate").GetComponent<SpriteRenderer>();
		}
		if (this.decelera == null)
		{
			this.decelera = this.trailTransform.Find("ship_decelera").GetComponent<SpriteRenderer>();
		}
		this.trailObject.layer = LayerDefine.Invisible;
		base.SetScale(1f);
		this.SetPosition(Vector3.one * 1000f);
	}

	public void InitShipEntity(Ship ship, bool vertical, bool noAnim)
	{
		this.ship = ship;
		base.image.gameObject.layer = LayerDefine.Default;
		this.SetColorImage(this.ship.currentTeam.color, (!noAnim) ? 0f : 1f);
		this.SetColorTrail(this.ship.currentTeam.color, 0f);
		this.HideEffect();
		base.SetScale(1f);
		this.vertical = vertical;
		this.CalcOrbitArgs();
		this.shipState = ShipState.ORBIT;
	}

	private float GetNodeDist()
	{
		return this.ship.currentNode.GetDist();
	}

	private float GetNodeWidth()
	{
		return this.ship.currentNode.GetWidth();
	}

	private void CalcOrbitArgs()
	{
		this.orbitRand = (float)Math.Round((double)this.ship.sceneManager.battleData.rand.Range(0f, 1f), 2);
		this.orbitDist = (float)Math.Round((double)(this.GetNodeDist() + this.orbitRand * this.GetNodeWidth()), 2) + this.fOffsetRadious;
		this.orbitSpeed = (float)Math.Round((double)this.ship.sceneManager.battleData.rand.Range(0.1f, 0.4f), 2);
		this.orbitSpeed *= this.ship.sceneManager.GetbattleScaleSpeed();
		float radAngle = (float)Math.Round((double)this.ship.sceneManager.battleData.rand.Range(1f, 360f), 2);
		Vector3 position = this.ship.currentNode.GetPosition();
		this.rotateRadius = this.orbitDist;
		this.AnglerPerCircle = 6.2831855f / (float)MyCircle.Positions.Length;
		this.SetPosition(this.SimpleRotate(radAngle, position));
		this.orbitAngle = radAngle;
		this.UpdateLayer();
		this.relativePos = base.GetPosition() - position;
	}

	private void UpdateLayer()
	{
		if (this.orbitAngle <= -6.2831855f)
		{
			this.orbitAngle -= -6.2831855f;
		}
		if (this.orbitAngle >= 6.2831855f)
		{
			this.orbitAngle -= 6.2831855f;
		}
		if (this.position.z > 0f)
		{
			this.SetOrderLayerBack();
		}
		else
		{
			this.SetOrderLayerFront();
		}
	}

	public override void SetPosition(Vector3 pos)
	{
		this.position = pos;
		if (this.silent)
		{
			return;
		}
		this.transform.position = pos;
	}

	private void SetColorImage(Color color, float alpha)
	{
		color.a = alpha;
		this.imageColor = color;
		if (this.silent)
		{
			return;
		}
		if (base.image == null)
		{
			return;
		}
		base.image.color = color;
	}

	public override void SetColor(Color color)
	{
		base.SetColor(color);
		if (base.image != null)
		{
			this.imageColor = base.image.color;
		}
		this.SetColorTrail(color, this.trail.color.a);
	}

	private void SetColorTrail(Color color, float alpha)
	{
		color.a = alpha;
		this.trailColor = color;
		if (this.silent)
		{
			return;
		}
		if (this.trail == null)
		{
			return;
		}
		this.trail.color = color;
	}

	private void SetScaleImage(Vector3 s)
	{
		this.imageScale = s;
		if (this.silent)
		{
			return;
		}
		if (this.ImageTransform == null)
		{
			return;
		}
		this.ImageTransform.localScale = s;
	}

	private void SetScaleTrail(Vector3 s)
	{
		this.trailScale = s;
		if (this.silent)
		{
			return;
		}
		if (this.trailTransform == null)
		{
			return;
		}
		this.trailTransform.localScale = s;
	}

	private void UpdateOrbit(int frame, float dt)
	{
		this.orbitAngle += this.orbitSpeed * dt;
		Vector3 position = this.SimpleRotate(this.orbitAngle, this.ship.currentNode.GetPosition());
		this.SetPosition(position);
		if (!this.ship.currentTeam.hideFly)
		{
			float num = this.imageColor.a;
			if (num < 1f)
			{
				num += dt * 2.5f;
				if (num >= 1f)
				{
					num = 1f;
				}
				this.SetColorImage(this.imageColor, num);
			}
		}
		Vector3 scaleImage = this.imageScale;
		if (scaleImage.x != 1f)
		{
			scaleImage.x = 1f;
			scaleImage.y = 1f;
			this.SetScaleImage(scaleImage);
		}
		float num2 = this.trailColor.a;
		if (num2 > 0f)
		{
			if (this.fromWarp)
			{
				num2 -= dt * 80f;
				Vector3 scaleTrail = this.trailScale;
				scaleTrail.x -= dt * 40f;
				if (scaleTrail.x < 0f)
				{
					scaleTrail.x = 0f;
				}
				this.SetScaleTrail(scaleTrail);
				if (scaleTrail.x <= 0f)
				{
					this.fromWarp = false;
					this.trailObject.layer = LayerDefine.Invisible;
					this.trailTransform.eulerAngles = Vector3.zero;
				}
			}
			else
			{
				num2 -= dt * 2.5f;
				Vector3 scaleTrail2 = this.trailScale;
				scaleTrail2.x -= dt * 1f;
				if (scaleTrail2.x < 0f)
				{
					scaleTrail2.x = 0f;
				}
				this.SetScaleTrail(scaleTrail2);
				if (num2 <= 0f)
				{
					this.trailObject.layer = LayerDefine.Invisible;
					this.trailTransform.eulerAngles = Vector3.zero;
				}
			}
		}
		this.UpdateLayer();
	}

	private void UpdatePreJump1(int frame, float dt)
	{
		Vector3 scaleImage = this.imageScale;
		scaleImage.x += dt * this.chargeRate * 2f * 1f;
		if (scaleImage.x > 6f)
		{
			scaleImage.x = 6f;
		}
		scaleImage.y = 1f - scaleImage.x / 6f * 1f * 0.25f;
		this.SetScaleImage(scaleImage);
		Transform transform = this.trailTransform;
		Vector3 eulerAngles = new Vector3(0f, 0f, this.jumpAngle * EntityShip.STATIC_ANGLE);
		this.ImageTransform.eulerAngles = eulerAngles;
		transform.eulerAngles = eulerAngles;
		this.shipState = ((scaleImage.x == 6f) ? ShipState.PREJUMP2 : ShipState.PREJUMP1);
	}

	private void UpdatePreJump2(int frame, float dt)
	{
		Transform imageTransform = this.ImageTransform;
		Vector3 eulerAngles = new Vector3(0f, 0f, this.jumpAngle * EntityShip.STATIC_ANGLE);
		this.trailTransform.eulerAngles = eulerAngles;
		imageTransform.eulerAngles = eulerAngles;
		Vector3 scaleImage = this.imageScale;
		scaleImage.x -= dt * 40f * 1f;
		if (scaleImage.x <= 2f)
		{
			scaleImage.x = 2f;
		}
		scaleImage.y = 1f - scaleImage.x / 6f * 1f * 0.25f;
		if (scaleImage.x != 2f)
		{
			this.SetScaleImage(scaleImage);
			return;
		}
		scaleImage.y = 0.5f;
		this.SetScaleImage(scaleImage);
		if (!this.warping)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayJumpStart(base.GetPosition());
			this.shipState = ShipState.JUMPING;
			return;
		}
		if (this.targetNode == null && this.ship != null)
		{
			this.ship.Bomb(NodeType.None);
			return;
		}
		Vector3 targetPosition = this.GetTargetPosition();
		this.SetPosition(targetPosition);
		this.ship.EnterNode(this.targetNode, true);
		this.shipState = ShipState.ORBIT;
		Solarmax.Singleton<AudioManger>.Get().PlayWarp(base.GetPosition());
	}

	public void SetWarpTrailEffect(Node from, Node to)
	{
		this.fromWarp = true;
		this.SetColorImage(this.imageColor, 1f);
		this.SetColorTrail(this.trailColor, 1f);
		this.SetScaleTrail(new Vector3(2f, 1f, 1f));
		this.trailObject.layer = LayerDefine.Default;
		Transform transform = this.trailTransform;
		Vector3 eulerAngles = new Vector3(0f, 0f, UtilTools.GetAngle360BetweenVector2(from.GetPosition(), to.GetPosition()));
		this.ImageTransform.eulerAngles = eulerAngles;
		transform.eulerAngles = eulerAngles;
	}

	private void UpdateJumping(int frame, float dt)
	{
		Vector3 vector = this.GetTargetPosition();
		float num = this.ship.currentTeam.speed * this.ship.sceneManager.GetbattleScaleSpeed();
		num *= this.gravitySpeedFix;
		float num2 = (float)Math.Round((double)(num * dt), 2);
		Vector3 position = base.GetPosition();
		Vector3 scaleTrail = this.trailScale;
		if (scaleTrail.x < 0.4f)
		{
			scaleTrail.x += dt * 0.1f * (this.ship.currentTeam.speed / 1.2f);
			if (scaleTrail.x > 0.4f)
			{
				scaleTrail.x = 0.4f;
			}
			this.SetScaleTrail(scaleTrail);
		}
		if (!this.nodeIsHide)
		{
			this.SetColorTrail(this.trailColor, 0.4f);
		}
		float num3 = (float)Math.Round((double)Vector3.Distance(position, vector), 2);
		if (num3 < 0.5f)
		{
			float num4 = this.trail.color.a;
			if (num4 > 0f)
			{
				num4 -= dt * 0.5f;
				Vector3 scaleTrail2 = this.trailScale;
				scaleTrail2.x -= dt * 1f;
				this.SetScaleTrail(scaleTrail2);
				if (num4 <= 0f)
				{
				}
			}
		}
		if (num3 > num2)
		{
			Vector3 nodeTarget;
			if (this.targetNode.revoType != RevolutionType.RT_None)
			{
				float runTime = num3 / this.ship.currentTeam.speed;
				nodeTarget = this.targetNode.GetNodeRunPosition(runTime);
			}
			else
			{
				nodeTarget = this.targetNode.GetPosition();
			}
			nodeTarget.x = (float)Math.Round((double)nodeTarget.x, 2);
			nodeTarget.y = (float)Math.Round((double)nodeTarget.y, 2);
			nodeTarget.z = (float)Math.Round((double)nodeTarget.z, 2);
			this.targetNodePos = nodeTarget;
			vector = this.targetNodePos + this.relativePos;
			float x = vector.x - this.curNodePos.x;
			float y = vector.y - this.curNodePos.y;
			this.jumpAngle = Mathf.Atan2(y, x);
			this.SetPosition(Vector3.MoveTowards(position, vector, num2));
		}
		else
		{
			this.SetPosition(vector);
			this.fromWarp = false;
			this.ship.EnterNode(this.targetNode, false);
			this.shipState = ShipState.ORBIT;
			this.HideEffect();
			Solarmax.Singleton<AudioManger>.Get().PlayJumpEnd(position);
		}
		this.totalJumpDist += num2;
	}

	private void HideEffect()
	{
		this.accelerate.gameObject.layer = LayerDefine.Invisible;
		this.decelera.gameObject.layer = LayerDefine.Invisible;
	}

	public void MoveTo(Node node, bool warp)
	{
		this.targetNode = node;
		this.targetFlag = node.tag;
		this.warping = warp;
		if (this.trailObject)
		{
			this.trailObject.layer = LayerDefine.Default;
		}
		float num = this.orbitDist;
		this.orbitDist = (float)Math.Round((double)(this.targetNode.GetDist() + this.orbitRand * this.targetNode.GetWidth()), 2) + this.fOffsetRadious;
		this.targetNodePos = this.targetNode.GetPosition();
		this.curNodePos = this.ship.currentNode.GetPosition();
		this.targetNodePos.x = (float)Math.Round((double)this.targetNodePos.x, 2);
		this.targetNodePos.y = (float)Math.Round((double)this.targetNodePos.y, 2);
		this.targetNodePos.z = (float)Math.Round((double)this.targetNodePos.z, 2);
		if (node.revoType != RevolutionType.RT_None && !this.warping)
		{
			Vector3 position = base.GetPosition();
			position.x = (float)Math.Round((double)position.x, 2);
			position.y = (float)Math.Round((double)position.y, 2);
			position.z = (float)Math.Round((double)position.z, 2);
			float runTime = Vector3.Distance(position, this.targetNodePos) / this.ship.currentTeam.speed;
			Vector3 nodeRunPosition = this.targetNode.GetNodeRunPosition(runTime);
			nodeRunPosition.x = (float)Math.Round((double)nodeRunPosition.x, 2);
			nodeRunPosition.y = (float)Math.Round((double)nodeRunPosition.y, 2);
			nodeRunPosition.z = (float)Math.Round((double)nodeRunPosition.z, 2);
			this.targetNodePos = nodeRunPosition;
		}
		this.rotateRadius = this.orbitDist;
		this.relativePos = this.orbitDist / num * (base.GetPosition() - this.curNodePos);
		this.relativePos.x = (float)Math.Round((double)this.relativePos.x, 2);
		this.relativePos.y = (float)Math.Round((double)this.relativePos.y, 2);
		this.relativePos.z = (float)Math.Round((double)this.relativePos.z, 2);
		this.chargeRate = (float)Math.Round((double)(this.ship.sceneManager.battleData.rand.Range(0f, 1f) * 2f + 6f), 2);
		float x = this.targetNodePos.x - this.curNodePos.x;
		float y = this.targetNodePos.y - this.curNodePos.y;
		this.jumpAngle = Mathf.Atan2(y, x);
		this.totalJumpDist = 0f;
		this.CalculateGravitySpeedFix();
		this.shipState = ShipState.PREJUMP1;
		if (!this.ship.currentTeam.hideFly)
		{
			this.SetScaleTrail(new Vector3(0f, 1f, 1f));
			this.SetColorImage(this.imageColor, 1f);
			this.SetColorTrail(this.trailColor, 0f);
		}
		Solarmax.Singleton<AudioManger>.Get().PlayJumpCharge(base.GetPosition());
	}

	private Vector3 GetTargetPosition()
	{
		return this.targetNodePos + this.relativePos;
	}

	public void ReSize(float size)
	{
		if (this.scale == null)
		{
			return;
		}
	}

	public Vector3 SimpleRotate(float radAngle, Vector3 centerPos)
	{
		int num = (int)(radAngle / this.AnglerPerCircle);
		float num2 = radAngle % this.AnglerPerCircle;
		int num3 = MyCircle.Positions.Length;
		if (num > num3 - 1)
		{
			num %= num3;
		}
		if (num < 0)
		{
			num = 0;
		}
		int num4 = num + 1;
		if (num4 > num3 - 1)
		{
			num4 = 0;
		}
		float t = num2 / this.AnglerPerCircle;
		this.mNewPos = Vector3.Lerp(MyCircle.Positions[num], MyCircle.Positions[num4], t);
		this.mNewPos = this.mNewPos * this.rotateRadius + centerPos;
		return this.mNewPos;
	}

	public override void Resuming()
	{
		this.SetColorImage(this.imageColor, this.imageColor.a);
		this.SetColorTrail(this.trailColor, this.trailColor.a);
		this.SetPosition(this.position);
		this.SetScaleImage(this.imageScale);
		this.SetScaleTrail(this.trailScale);
		base.SetScale(base.scale);
		base.SetRotation(this.eulerAngles);
	}

	private void CalculateGravitySpeedFix()
	{
		float value = this.targetNode.GetHalfNodeSize() - this.ship.currentNode.GetHalfNodeSize();
		value = Mathf.Clamp(value, -1f, 1f);
		this.gravitySpeedFix = 1f;
	}

	private void SetOrderLayerFront()
	{
		if (this.curOrderLayer == 15)
		{
			return;
		}
		base.image.sortingOrder = 15;
		this.trail.sortingOrder = 15;
		this.accelerate.sortingOrder = 15;
		this.decelera.sortingOrder = 15;
		this.curOrderLayer = 15;
	}

	private void SetOrderLayerBack()
	{
		if (this.curOrderLayer == 0)
		{
			return;
		}
		base.image.sortingOrder = 0;
		this.trail.sortingOrder = 0;
		this.accelerate.sortingOrder = 0;
		this.decelera.sortingOrder = 0;
		this.curOrderLayer = 0;
	}

	private const float SCALE_MAX = 1f;

	private SpriteRenderer trail;

	private GameObject trailObject;

	private SpriteRenderer accelerate;

	private SpriteRenderer decelera;

	private bool nodeIsHide;

	public string targetFlag = string.Empty;

	private Vector3 relativePos;

	private Vector3 targetNodePos;

	private Vector3 curNodePos;

	private float orbitRand;

	private float orbitDist;

	private float orbitSpeed;

	private float orbitAngle;

	private float fOffsetRadious = 0.15f;

	private Vector3 roatVertical = Vector3.Normalize(new Vector3(1f, -0.2f, 0f));

	private Vector3 roatHorizontal = Vector3.Normalize(new Vector3(0f, 1f, -0.2f));

	private static float STATIC_ANGLE = 57.295776f;

	private float chargeRate;

	private float jumpAngle;

	private bool warping;

	public float totalJumpDist;

	private Color imageColor;

	private Color trailColor;

	private Vector3 imageScale;

	private Vector3 trailScale;

	private bool[] flag = new bool[5];

	private float gravitySpeedFix;

	private bool fromWarp;

	private float warpDistance;

	private Transform trailTransform;

	private int curOrderLayer = -1;

	private float rotateRadius;

	private float AnglerPerCircle;

	private Vector3 mNewPos = Vector3.zero;
}
