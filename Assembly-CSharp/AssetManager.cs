using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : Solarmax.Singleton<AssetManager>
{
	public void Init()
	{
	}

	public void LoadBattleResources()
	{
		this.AddSprite("Entity_Castle");
		this.AddSprite("Entity_Planet");
		this.AddSprite("Entity_Ship");
		this.AddSprite("Entity_Tower");
		this.AddSprite("Entity_Door");
		this.AddSprite("Entity_Barrier");
		this.AddSprite("Entity_Master");
		this.AddSprite("Entity_Defense");
		this.AddSprite("Entity_Power");
		this.AddSprite("Entity_Dilator");
		this.AddSprite("Entity_BlackHole");
		this.AddSprite("Entity_House");
		this.AddSprite("Entity_Arsenal");
		this.AddSprite("Entity_AircraftCarrier");
		this.AddSprite("Entity_Attackship");
		this.AddSprite("Entity_AntiAttackship");
		this.AddSprite("Entity_AntiCaptureship");
		this.AddSprite("Entity_AntiLifeship");
		this.AddSprite("Entity_AntiSpeedship");
		this.AddSprite("Entity_Lifeship");
		this.AddSprite("Entity_Speedship");
		this.AddSprite("Entity_Lasercannon");
		this.AddSprite("Entity_Captureship");
		this.AddSprite("Entity_Magicstar");
		this.AddSprite("Entity_Hiddenstar");
		this.AddSprite("Entity_FixedWarpDoor");
		this.AddSprite("Entity_Clouds");
		this.AddSprite("Entity_UnknownStar");
		this.AddSprite("Entity_Mirror");
		this.AddSprite("Entity_Curse");
		this.AddSprite("Entity_Pulse");
		this.AddSprite("Entity_Halo");
		this.AddSprite("Entity_LaserLine");
		this.AddSprite("Entity_LaserLineNew");
		this.AddSprite("Entity_Linerender");
		this.AddSprite("Entity_LaserLineNew0.1");
		this.AddSprite("Entity_BarrierLine");
		this.AddSprite("entity_warp");
		this.AddSprite("Entity_Select");
		this.AddSprite("Entity_Warning");
		this.AddSprite("Effect_Door");
		this.AddSprite("Effect_Birth");
		this.AddSprite("Effect_Birth_Other");
		this.AddSprite("Effect_Explosion");
		this.AddSprite("Effect_ShipBirth");
		this.AddSprite("Entity_UI_Halo");
		this.AddSprite("Entity_Selected");
		this.AddSprite("Entity_Line");
		this.AddSprite("Skill_ChongJiBo");
		this.AddSprite("Effect_Aims");
		this.AddSprite("Entity_Nuclear");
		this.AddSound("explosion01");
		this.AddSound("explosion02");
		this.AddSound("explosion03");
		this.AddSound("explosion04");
		this.AddSound("explosion05");
		this.AddSound("explosion06");
		this.AddSound("explosion07");
		this.AddSound("explosion08");
		this.AddSound("jumpCharge");
		this.AddSound("jumpStart");
		this.AddSound("jumpEnd");
		this.AddSound("capture");
		this.AddSound("laser");
		this.AddSound("warp_charge");
		this.AddSound("warp");
		this.AddSound("Swish05");
		this.AddSound("TrailerHit05");
		this.AddSound("TrailerHit14");
		this.AddSound("onPVPdefeated");
		this.AddSound("onPVPvictory");
		this.AddSound("onClick");
		this.AddSound("LevelEntry");
		this.AddSound("LevelExit");
		this.AddSound("onMyRace");
		this.AddSound("moveClick");
		this.AddSound("unlock");
		this.AddSound("starSound");
		this.AddSound("startBattle");
		this.AddSound("click_down");
		this.AddSound("onOpen");
		this.AddSound("click");
		this.AddSound("PlanetExplosion");
		this.AddSound("onClose");
		this.AddSound("Gold");
		this.AddSound("T_Twist_tra");
		this.AddSound("T_Clone_tra");
		this.AddSound("emission_1");
		this.AddSound("emission_2");
		this.AddSound("HitLightning2");
		this.AddUI("TXT");
		this.AddUI("HomeWindow");
		this.AddUI("LobbyWindowView");
		this.AddParticle("shifang_fab");
		this.AddParticle("shifang_1_fab");
		this.AddEffect("Effect_Jiguangpao");
		this.AddEffect("EFF_XJ_Boom_1");
		this.AddEffect("Eff_XJ_Djs");
		this.AddEffect("EFF_XJ_XiShou");
		this.AddEffect("EFF_XJ_XiShou_1");
		this.AddEffect("EFF_XJ_XiShou_3");
		this.AddEffect("XJ_Glow_2");
		this.AddEffect("EFF_XJ_BianHuan");
		this.AddEffect("tongdao");
		this.AddEffect("tongdao2");
		this.AddEffect("EFF_XJ_XiShou");
		this.AddEffect("EFF_XJ_SaoGuang_B");
		this.AddEffect("EFF_XJ_Lightning1");
		this.AddEffect("EFF_select_head_101");
		this.AddEffect("EFF_select_head_102");
		this.AddEffect("EFF_select_head_103");
		this.AddEffect("EFF_XJ_Nuclear");
		this.AddEffect("EFF_XJ_BaoZha2");
	}

	public void LoadShipAudio()
	{
		this.AddSound("emission_0");
	}

	public void UnLoadBattleResources()
	{
		this.RemoveResources("Entity_Castle");
		this.RemoveResources("Entity_Planet");
		this.RemoveResources("Entity_Ship");
		this.RemoveResources("Entity_Tower");
		this.RemoveResources("Entity_Door");
		this.RemoveResources("Entity_Barrier");
		this.RemoveResources("Entity_Master");
		this.RemoveResources("Entity_Defense");
		this.RemoveResources("Entity_Power");
		this.RemoveResources("Entity_BlackHole");
		this.RemoveResources("Entity_Dilator");
		this.RemoveResources("Entity_House");
		this.RemoveResources("Entity_Arsenal");
		this.RemoveResources("Entity_AircraftCarrier");
		this.RemoveResources("Entity_Attackship");
		this.RemoveResources("Entity_AntiAttackship");
		this.RemoveResources("Entity_AntiCaptureship");
		this.RemoveResources("Entity_AntiLifeship");
		this.RemoveResources("Entity_AntiSpeedship");
		this.RemoveResources("Entity_Lifeship");
		this.RemoveResources("Entity_Speedship");
		this.RemoveResources("Entity_Lasercannon");
		this.RemoveResources("Entity_Captureship");
		this.RemoveResources("Entity_Magicstar");
		this.RemoveResources("Entity_Hiddenstar");
		this.RemoveResources("Entity_FixedWarpDoor");
		this.RemoveResources("Entity_Clouds");
		this.RemoveResources("Entity_UnknownStar");
		this.RemoveResources("Entity_Mirror");
		this.RemoveResources("Entity_Curse");
		this.RemoveResources("Entity_Nuclear");
		this.RemoveResources("Entity_Pulse");
		this.RemoveResources("Entity_Halo");
		this.RemoveResources("Entity_LaserLine");
		this.RemoveResources("Entity_Linerender");
		this.RemoveResources("Entity_LaserLineNew0.1");
		this.RemoveResources("Entity_BarrierLine");
		this.RemoveResources("Entity_Warp");
		this.RemoveResources("Entity_Select");
		this.RemoveResources("Entity_Warning");
		this.RemoveResources("Effect_Door");
		this.RemoveResources("Effect_Birth");
		this.RemoveResources("Effect_Birth_Other");
		this.RemoveResources("Effect_Explosion");
		this.RemoveResources("Effect_ShipBirth");
		this.RemoveResources("Entity_UI_Halo");
		this.RemoveResources("Entity_Selected");
		this.RemoveResources("Entity_Line");
		this.RemoveResources("Skill_ChongJiBo");
		this.RemoveResources("EFF_XJ_Boom_1");
		this.RemoveResources("Eff_XJ_Djs");
		this.RemoveResources("Effect_Aims");
		this.RemoveResources("tongdao");
		this.RemoveResources("tongdao2");
		this.RemoveResources("EFF_XJ_XiShou");
		this.RemoveResources("explosion01");
		this.RemoveResources("explosion02");
		this.RemoveResources("explosion03");
		this.RemoveResources("explosion04");
		this.RemoveResources("explosion05");
		this.RemoveResources("explosion06");
		this.RemoveResources("explosion07");
		this.RemoveResources("explosion08");
		this.RemoveResources("jumpCharge");
		this.RemoveResources("jumpStart");
		this.RemoveResources("jumpEnd");
		this.RemoveResources("capture");
		this.RemoveResources("laser");
		this.RemoveResources("warp_charge");
		this.RemoveResources("warp");
		this.RemoveResources("onClick");
		this.RemoveResources("LevelEntry");
		this.RemoveResources("LevelExit");
		this.RemoveResources("onMyRace");
		this.RemoveResources("moveClick");
		this.RemoveResources("unlock");
		this.RemoveResources("starSound");
		this.RemoveResources("startBattle");
		this.RemoveResources("click_down");
		this.RemoveResources("onOpen");
		this.RemoveResources("click");
		this.RemoveResources("PlanetExplosion");
		this.RemoveResources("onClose");
		this.RemoveResources("Gold");
		this.RemoveResources("T_Twist_tra");
		this.RemoveResources("T_Clone_tra");
		this.RemoveResources("emission_0");
		this.RemoveResources("emission_1");
		this.RemoveResources("emission_2");
		this.RemoveResources("HitLightning2");
		this.RemoveResources("shifang_fab");
		this.RemoveResources("shifang_1_fab");
		this.RemoveResources("Effect_Jiguangpao");
		this.RemoveResources("EFF_XJ_XiShou");
		this.RemoveResources("EFF_XJ_XiShou_1");
		this.RemoveResources("EFF_XJ_XiShou_2");
		this.RemoveResources("EFF_XJ_XiShou_3");
		this.RemoveResources("XJ_Glow_2");
		this.RemoveResources("EFF_XJ_BianHuan");
		this.RemoveResources("EFF_XJ_SaoGuang_B");
		this.RemoveResources("EFF_XJ_Lightning1");
		this.RemoveResources("EFF_select_head_101");
		this.RemoveResources("EFF_select_head_102");
		this.RemoveResources("EFF_select_head_103");
		this.RemoveResources("EFF_XJ_Nuclear");
		this.RemoveResources("EFF_XJ_BaoZha2");
		this.RemoveResources("TXT");
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void FakeLoadBattleResources()
	{
		this.AddSprite("Entity_Planet");
		this.AddSprite("Entity_Ship");
		this.AddSprite("Entity_Door");
		this.AddSprite("Entity_Warp");
		this.AddSprite("Entity_Select");
		this.AddSprite("Effect_Door");
		this.AddSprite("Entity_Selected");
		this.AddSprite("Entity_Line");
		this.AddSprite("Effect_Explosion");
		this.AddSprite("Effect_ShipBirth");
		this.AddSprite("Entity_UI_Halo");
		this.AddSound("jumpCharge");
		this.AddSound("jumpStart");
		this.AddSound("jumpEnd");
		this.AddSound("warp_charge");
		this.AddSound("warp");
		this.AddUI("TXT");
	}

	private void RemoveResources(string key)
	{
		if (this.resources.ContainsKey(key))
		{
			this.resources.Remove(key);
		}
	}

	public UnityEngine.Object GetResources(string key)
	{
		UnityEngine.Object result = null;
		this.resources.TryGetValue(key, out result);
		return result;
	}

	public void AddSprite(string key)
	{
		if (this.resources.ContainsKey(key))
		{
			return;
		}
		this.resources.Add(key, this.LoadResource("gameres/sprites/" + key + ".prefab"));
	}

	public void AddSound(string key)
	{
		if (this.resources.ContainsKey(key))
		{
			return;
		}
		this.resources.Add(key, this.LoadSounds("gameres/sounds/" + key + ".mp3"));
	}

	public void AddUI(string key)
	{
		if (this.resources.ContainsKey(key))
		{
			return;
		}
		this.resources.Add(key, this.LoadResource("gameres/ui/" + key + ".prefab"));
	}

	public void AddParticle(string key)
	{
		if (this.resources.ContainsKey(key))
		{
			return;
		}
		this.resources.Add(key, this.LoadResource("gameres/particle/" + key + ".prefab"));
	}

	public void AddEffect(string key)
	{
		if (this.resources.ContainsKey(key))
		{
			return;
		}
		this.resources.Add(key, this.LoadResource("gameres/effect/prefab/" + key + ".prefab"));
	}

	public UnityEngine.Object LoadResource(string path)
	{
		object obj = null;
		if (obj == null)
		{
			obj = LoadResManager.LoadRes(path.ToLower());
		}
		return (UnityEngine.Object)obj;
	}

	public UnityEngine.Object LoadSounds(string path)
	{
		object obj = null;
		if (obj == null)
		{
			obj = LoadResManager.LoadSound(path.ToLower());
		}
		return (UnityEngine.Object)obj;
	}

	private const string SOUND_PATH = "gameres/sounds/";

	private const string SPRITE_PATH = "gameres/sprites/";

	private const string UI_PATH = "gameres/ui/";

	private const string PARTICLE_PATH = "gameres/particle/";

	private const string EFFECT_PATH = "gameres/effect/prefab/";

	private Dictionary<string, UnityEngine.Object> resources = new Dictionary<string, UnityEngine.Object>();
}
