using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class EffectManager : Solarmax.Singleton<EffectManager>, Lifecycle2
{
	private EffectPool pulsePool { get; set; }

	private EffectPool bombPool { get; set; }

	private EffectPool makePool { get; set; }

	private EffectPool glowPool { get; set; }

	private EffectPool haloPool { get; set; }

	private EffectPool laserLinePool { get; set; }

	private EffectPool laserGunPool { get; set; }

	private EffectPool twistPool { get; set; }

	private EffectPool darkPulsePool { get; set; }

	private EffectPool groundPool { get; set; }

	private EffectPool aniPool { get; set; }

	private EffectPool guidePool { get; set; }

	private SkillEffectPool skillPool { get; set; }

	private EffectPool drifteffectPool { get; set; }

	private EffectPool syncdrifteffectPool { get; set; }

	private EffectPool blackHoleffectPool { get; set; }

	private EffectPool unknowEffectPool { get; set; }

	private EffectPool clonePool { get; set; }

	private EffectPool blackholeHitPool { get; set; }

	private EffectPool curseEffectPool { get; set; }

	private EffectPool diffusionEffectPool { get; set; }

	private EffectPool cannonEffectPool { get; set; }

	private EffectPool cannonBombEffectPool { get; set; }

	public bool Init()
	{
		this.selectGrounds = new Dictionary<string, GroundNode>();
		this.pulsePool = new EffectPool();
		this.haloPool = new EffectPool();
		this.glowPool = new EffectPool();
		this.laserLinePool = new EffectPool();
		this.darkPulsePool = new EffectPool();
		this.groundPool = new EffectPool();
		this.aniPool = new EffectPool();
		this.guidePool = new EffectPool();
		this.bombPool = new EffectPool();
		this.makePool = new EffectPool();
		this.skillPool = new SkillEffectPool();
		this.drifteffectPool = new EffectPool();
		this.syncdrifteffectPool = new EffectPool();
		this.laserGunPool = new EffectPool();
		this.blackHoleffectPool = new EffectPool();
		this.unknowEffectPool = new EffectPool();
		this.twistPool = new EffectPool();
		this.clonePool = new EffectPool();
		this.blackholeHitPool = new EffectPool();
		this.curseEffectPool = new EffectPool();
		this.diffusionEffectPool = new EffectPool();
		this.cannonEffectPool = new EffectPool();
		this.cannonBombEffectPool = new EffectPool();
		return true;
	}

	public void InitPrev()
	{
	}

	public void Tick(int frame, float interval)
	{
		this.pulsePool.Tick(frame, interval);
		this.haloPool.Tick(frame, interval);
		this.glowPool.Tick(frame, interval);
		this.laserLinePool.Tick(frame, interval);
		this.darkPulsePool.Tick(frame, interval);
		this.groundPool.Tick(frame, interval);
		this.aniPool.Tick(frame, interval);
		this.guidePool.Tick(frame, interval);
		this.bombPool.Tick(frame, interval);
		this.makePool.Tick(frame, interval);
		this.skillPool.Tick(frame, interval);
		this.drifteffectPool.Tick(frame, interval);
		this.syncdrifteffectPool.Tick(frame, interval);
		this.blackHoleffectPool.Tick(frame, interval);
		this.unknowEffectPool.Tick(frame, interval);
		this.twistPool.Tick(frame, interval);
		this.clonePool.Tick(frame, interval);
		this.blackholeHitPool.Tick(frame, interval);
		this.curseEffectPool.Tick(frame, interval);
		this.diffusionEffectPool.Tick(frame, interval);
		this.cannonEffectPool.Tick(frame, interval);
		this.cannonBombEffectPool.Tick(frame, interval);
	}

	public void LaserGunTick(int frame, float interval)
	{
		this.laserGunPool.Tick(frame, interval);
	}

	public void Destroy()
	{
		this.pulsePool.Destroy();
		this.haloPool.Destroy();
		this.glowPool.Destroy();
		this.laserLinePool.Destroy();
		this.darkPulsePool.Destroy();
		this.groundPool.Destroy();
		this.aniPool.Destroy();
		this.guidePool.Destroy();
		this.bombPool.Destroy();
		this.makePool.Destroy();
		this.skillPool.Destroy();
		this.drifteffectPool.Destroy();
		this.syncdrifteffectPool.Destroy();
		this.laserGunPool.Destroy();
		this.blackHoleffectPool.Destroy();
		this.unknowEffectPool.Destroy();
		this.twistPool.Destroy();
		this.clonePool.Destroy();
		this.blackholeHitPool.Destroy();
		this.curseEffectPool.Destroy();
		this.diffusionEffectPool.Destroy();
		this.cannonEffectPool.Destroy();
		this.cannonBombEffectPool.Destroy();
	}

	public void PreloadEffect()
	{
		for (int i = 0; i < 6; i++)
		{
			this.AddBomber(null, Vector3.zero, Color.white, false, true);
		}
		for (int j = 0; j < 6; j++)
		{
			this.AddMakeEffect(null, Vector3.zero, Color.white, false, true);
		}
		for (int k = 0; k < 6; k++)
		{
			this.AddMakeEffect(null, Vector3.zero, Color.white, false, true);
		}
		for (int l = 0; l < 6; l++)
		{
			this.AddBlackholeHitEffect(Vector3.zero, Color.white, false, true);
		}
		for (int m = 0; m < 11; m++)
		{
			this.AddHalo(null, Color.white, false, true);
		}
		for (int n = 0; n < 10; n++)
		{
			this.AddLaserLine(Vector3.zero, Vector3.zero, Color.white, false, true);
		}
	}

	public void AddBomber(Node node, Vector3 position, Color color, bool anim = true, bool preload = false)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		if (!preload && !Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Bomber))
		{
			return;
		}
		BombEffect bombEffect = this.bombPool.Alloc<BombEffect>();
		bombEffect.node = node;
		bombEffect.bombPosition = position;
		bombEffect.color = color;
		bombEffect.InitEffectNode(anim);
		if (anim)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayExlposion(position);
		}
	}

	public BlackholeHitEffect AddBlackholeHitEffect(Vector3 position, Color color, bool anim = true, bool preload = false)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return null;
		}
		if (!preload && !Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Bomber))
		{
			return null;
		}
		BlackholeHitEffect blackholeHitEffect = this.blackholeHitPool.Alloc<BlackholeHitEffect>();
		blackholeHitEffect.bombPosition = position;
		blackholeHitEffect.recycleTime = 1.5f;
		color.a = 1f;
		blackholeHitEffect.selfColor = color;
		blackholeHitEffect.InitEffectNode(anim);
		if (anim)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayExlposion(position);
		}
		return blackholeHitEffect;
	}

	public void AddBomberNoLimit(Node node, Vector3 position, Color color, bool anim = true, bool preload = false)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		BombEffect bombEffect = this.bombPool.Alloc<BombEffect>();
		bombEffect.node = node;
		bombEffect.bombPosition = position;
		bombEffect.color = color;
		bombEffect.InitEffectNode(anim);
		if (!preload && !Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Bomber))
		{
			return;
		}
		if (anim)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayExlposion(position);
		}
	}

	public void AddMakeEffect(Node node, Vector3 position, Color color, bool anim = true, bool preload = false)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		if (!preload && !Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Maker))
		{
			return;
		}
		MakeEffect makeEffect = this.makePool.Alloc<MakeEffect>();
		makeEffect.node = node;
		makeEffect.makePosition = position;
		makeEffect.color = color;
		makeEffect.InitEffectNode(anim);
	}

	public void AddHalo(Node building, Color color, bool anim = true, bool preload = false)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		if (building != null)
		{
			HaloNode haloNode = this.haloPool.Alloc<HaloNode>();
			haloNode.position = building.GetPosition();
			haloNode.scale = Vector3.one * building.GetScale();
			haloNode.color = color;
			haloNode.CurNode = building;
			haloNode.InitEffectNode(anim);
		}
		else
		{
			HaloNode haloNode2 = this.haloPool.Alloc<HaloNode>();
			haloNode2.position = Vector3.zero;
			haloNode2.scale = Vector3.one * 1f;
			haloNode2.color = color;
			haloNode2.CurNode = null;
			haloNode2.InitEffectNode(anim);
		}
	}

	public void AddGlow(Node node, Color color)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		GlowNode glowNode = this.glowPool.Alloc<GlowNode>();
		glowNode.node = node;
		glowNode.color = color;
		glowNode.InitEffectNode(true);
	}

	public void AddLaserLine(Vector3 beginPos, Vector3 endPos, Color color, bool anim = true, bool preload = false)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		LaserLineNode laserLineNode = this.laserLinePool.Alloc<LaserLineNode>();
		laserLineNode.beginPosition = beginPos;
		laserLineNode.endPosition = endPos;
		laserLineNode.color = color;
		laserLineNode.InitEffectNode(anim);
	}

	public void AddWarpPulse(Node from, Node to, TEAM team, float rate)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		DarkPulse darkPulse = this.darkPulsePool.Alloc<DarkPulse>();
		darkPulse.from = from;
		darkPulse.to = to;
		darkPulse.team = team;
		darkPulse.moveRate = rate;
		darkPulse.position = from.GetPosition();
		darkPulse.scale = Vector3.one * from.GetScale();
		darkPulse.color = from.currentTeam.color;
		darkPulse.type = WarpType.WarpPulse;
		darkPulse.InitEffectNode(true);
	}

	public void AddWarpArrive(Node from, Color color)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		DarkPulse darkPulse = this.darkPulsePool.Alloc<DarkPulse>();
		darkPulse.from = from;
		darkPulse.position = from.GetPosition();
		darkPulse.scale = Vector3.one * from.GetScale();
		darkPulse.color = color;
		darkPulse.type = WarpType.WarpArrive;
		darkPulse.InitEffectNode(true);
	}

	public void ShowSelectEffect(Node node)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		if (this.selectGrounds.ContainsKey(node.tag))
		{
			return;
		}
		GroundNode groundNode = this.groundPool.Alloc<GroundNode>();
		groundNode.owner = node;
		groundNode.color = node.currentTeam.color;
		groundNode.liftTime = -1f;
		groundNode.InitEffectNode(true);
		this.selectGrounds.Add(node.tag, groundNode);
	}

	public void ShowGuideEffect(Node node, bool isSelf)
	{
		GuideEffect guideEffect = this.guidePool.Alloc<GuideEffect>();
		if (isSelf)
		{
			guideEffect.effectName = "Effect_Birth";
		}
		else
		{
			guideEffect.effectName = "Effect_Birth_Other";
		}
		guideEffect.animationName = "Effect_Birth_in";
		guideEffect.homeNode = node;
		guideEffect.flickDuring = 0.5f;
		guideEffect.flickInterval = 0.5f;
		guideEffect.InitEffectNode(true);
	}

	public void HideGuideEffect()
	{
		this.guidePool.Destroy();
	}

	public void HideSelectEffect(Node node)
	{
		if (this.selectGrounds.ContainsKey(node.tag))
		{
			GroundNode groundNode = this.selectGrounds[node.tag];
			groundNode.liftTime = 0.5f;
			this.selectGrounds.Remove(node.tag);
		}
	}

	public SkillEffect PlaySkillEffect(Node node, string effectName, string animationName, float time, float scale = 1f)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return null;
		}
		if (string.IsNullOrEmpty(animationName))
		{
			animationName = effectName;
		}
		SkillEffect skillEffect = this.skillPool.GetSameEffect(node, effectName, animationName);
		if (skillEffect != null)
		{
			SkillEffect.SkillEffectRecycleType skillEffectRecycleType;
			if (time > 1E-45f)
			{
				skillEffectRecycleType = SkillEffect.SkillEffectRecycleType.BuffTime;
			}
			else
			{
				skillEffectRecycleType = SkillEffect.SkillEffectRecycleType.AnimateTime;
			}
			if (skillEffect.startFrame != Solarmax.Singleton<TimeSystem>.Instance.GetFrame() && (skillEffectRecycleType != SkillEffect.SkillEffectRecycleType.BuffTime || skillEffect.recycleType != SkillEffect.SkillEffectRecycleType.BuffTime))
			{
				skillEffect = null;
			}
		}
		if (skillEffect == null)
		{
			skillEffect = this.skillPool.Alloc<SkillEffect>();
			skillEffect.effectName = effectName;
			skillEffect.animationName = animationName;
			skillEffect.hoodEntity = node;
			skillEffect.lifeTime = time;
			if (time > 1E-45f)
			{
				skillEffect.recycleType = SkillEffect.SkillEffectRecycleType.BuffTime;
			}
			else
			{
				skillEffect.recycleType = SkillEffect.SkillEffectRecycleType.AnimateTime;
			}
			skillEffect.scale = scale;
			skillEffect.InitEffectNode(true);
			this.skillPool.AddReLifeEffect(skillEffect);
		}
		else
		{
			Debug.LogWarningFormat("ReLife effect, node:{0}, en:{1}, an:{2}  lasttime:{3}, nowtime:{4}", new object[]
			{
				node.tag,
				effectName,
				animationName,
				skillEffect.lifeTime,
				time
			});
			skillEffect.lifeTime = time;
		}
		return skillEffect;
	}

	public void PlayParticleEffect(Node node, string effectName)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(effectName);
		GameObject gameObject = UnityEngine.Object.Instantiate(resources) as GameObject;
		ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
		gameObject.transform.position = node.GetPosition();
		component.Play();
	}

	public GameObject PlayEffect(Node node, string effectName)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return null;
		}
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(effectName);
		GameObject gameObject = UnityEngine.Object.Instantiate(resources) as GameObject;
		gameObject.transform.SetParent(node.GetGO().transform);
		gameObject.transform.localPosition = Vector3.zero;
		return gameObject;
	}

	public BlackHoleEffect PlayBlackHoleEffect(Node node)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return null;
		}
		BlackHoleEffect blackHoleEffect = this.blackHoleffectPool.Alloc<BlackHoleEffect>();
		blackHoleEffect.node = node;
		blackHoleEffect.recycleTime = 20f;
		blackHoleEffect.InitEffectNode(true);
		return blackHoleEffect;
	}

	public void PlayUnknownStarEffect(Node node, float scale)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		UnknownStarEffect unknownStarEffect = this.unknowEffectPool.Alloc<UnknownStarEffect>();
		unknownStarEffect.node = node;
		unknownStarEffect.scale = scale;
		unknownStarEffect.recycleTime = 2f;
		unknownStarEffect.InitEffectNode(true);
	}

	public TwistEffect PlayTwistEffect(Node node, Vector3 beginPos, Vector3 endPos, Color selfColor, Color enemyColor, bool isMax)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return null;
		}
		TwistEffect twistEffect = this.twistPool.Alloc<TwistEffect>();
		twistEffect.node = node;
		twistEffect.recycleTime = 1.3f;
		twistEffect.beginPosition = beginPos;
		twistEffect.endPosition = endPos;
		twistEffect.selfColor = selfColor;
		twistEffect.isMax = isMax;
		enemyColor.a = 1f;
		twistEffect.enemyColor = enemyColor;
		twistEffect.InitEffectNode(true);
		return twistEffect;
	}

	public CloneEffect PlayCloneEffect(Node node, Vector3 beginPos, Vector3 endPos, Color selfColor)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return null;
		}
		CloneEffect cloneEffect = this.clonePool.Alloc<CloneEffect>();
		cloneEffect.node = node;
		cloneEffect.recycleTime = 1.3f;
		cloneEffect.beginPosition = beginPos;
		cloneEffect.endPosition = endPos;
		cloneEffect.selfColor = selfColor;
		cloneEffect.InitEffectNode(true);
		return cloneEffect;
	}

	public void PlayDriftSkillEffect(Node node, string effectName, float time, float scale = 1f)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		DriftSkillEffect driftSkillEffect = this.drifteffectPool.Alloc<DriftSkillEffect>();
		driftSkillEffect.effectName = effectName;
		driftSkillEffect.hoodEntity = node;
		driftSkillEffect.lifeTime = time;
		driftSkillEffect.recycleType = DriftSkillEffect.SkillEffectRecycleType.AnimateTime;
		driftSkillEffect.scale = scale;
		driftSkillEffect.InitEffectNode(true);
	}

	public void PlaySyncEffect(Node node, string effectName, float time, float scale = 1f)
	{
		if (effectName == "EFF_XJ_Boom_1")
		{
			node.readyBomb = true;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		bool flag = false;
		List<EffectNode> allObjects = this.syncdrifteffectPool.GetAllObjects();
		foreach (EffectNode effectNode in allObjects)
		{
			DriftSkillEffect driftSkillEffect = (DriftSkillEffect)effectNode;
			if (driftSkillEffect.hoodEntity == node && driftSkillEffect.effectName == effectName)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		DriftSkillEffect driftSkillEffect2 = this.syncdrifteffectPool.Alloc<DriftSkillEffect>();
		driftSkillEffect2.effectName = effectName;
		driftSkillEffect2.hoodEntity = node;
		driftSkillEffect2.lifeTime = time;
		driftSkillEffect2.recycleType = DriftSkillEffect.SkillEffectRecycleType.AnimateTime;
		driftSkillEffect2.scale = scale;
		driftSkillEffect2.InitEffectNode(true);
	}

	public void AddLasergun(Node node, float time, int count = 0, bool reflex = true, float length = 3f, float speed = 10f, float width = 0f)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		LasergunEffectNode lasergunEffectNode = this.laserGunPool.Alloc<LasergunEffectNode>();
		lasergunEffectNode.hoodEntity = node;
		lasergunEffectNode.team = node.team;
		lasergunEffectNode.lifeTime = time;
		lasergunEffectNode.count = count;
		lasergunEffectNode.reflex = reflex;
		lasergunEffectNode.totalLength = length;
		lasergunEffectNode.speedRatio = speed;
		lasergunEffectNode.setwidth = width;
		lasergunEffectNode.color = node.entity.GetColor();
		lasergunEffectNode.beginPosition = node.GetPosition();
		lasergunEffectNode.InitEffectNode(true);
	}

	public void PlayCurseEffect(Node node)
	{
		CurseEffect curseEffect = this.curseEffectPool.Alloc<CurseEffect>();
		curseEffect.recycleTime = 31f;
		curseEffect.node = node;
		curseEffect.InitEffectNode(true);
	}

	public void PlayDiffusionEffect(Node node, Vector3 beginPos, Vector3 endPos)
	{
		if (node == null || node.GetGO() == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("[DiffusionEffect]{0}", new object[]
			{
				"星球为空"
			});
			return;
		}
		float angle360BetweenVector = UtilTools.GetAngle360BetweenVector2(beginPos, endPos);
		float num = Vector2.Angle(Vector3.zero, new Vector3(1f, 1f, 0f));
		DiffusionEffect diffusionEffect = this.diffusionEffectPool.Alloc<DiffusionEffect>();
		diffusionEffect.scale = 10f / (endPos - beginPos).magnitude;
		diffusionEffect.angle = angle360BetweenVector;
		diffusionEffect.recycleTime = 0.5f;
		diffusionEffect.node = node;
		diffusionEffect.radius = (endPos - beginPos).magnitude;
		diffusionEffect.radian = angle360BetweenVector * 3.1415927f / 180f;
		diffusionEffect.InitEffectNode(true);
	}

	public void PlayCannonEffect(Node node, Node attack)
	{
		CannonEffect cannonEffect = this.cannonEffectPool.Alloc<CannonEffect>();
		cannonEffect.recycleTime = node.AttackSpeed - 1f;
		cannonEffect.missleTime = node.AttackSpeed - 5f;
		cannonEffect.node = node;
		cannonEffect.attackNode = attack;
		cannonEffect.InitEffectNode(true);
	}

	public void PlayCannonBombEffect(Node node)
	{
		CannonBombEffect cannonBombEffect = this.cannonBombEffectPool.Alloc<CannonBombEffect>();
		cannonBombEffect.recycleTime = 5.5f;
		cannonBombEffect.node = node;
		cannonBombEffect.InitEffectNode(true);
	}

	private Dictionary<string, GroundNode> selectGrounds;

	private List<GameObject> baozhaEffect = new List<GameObject>();

	public float fPlayAniSpeed = 1f;
}
