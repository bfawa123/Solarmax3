using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class NewSkillManager : Lifecycle2
{
	public NewSkillManager(SceneManager manager)
	{
		this.sceneManager = manager;
		this.skillLists = new List<BaseNewSkill>[LocalPlayer.MaxTeamNum];
		int i = 0;
		int num = this.skillLists.Length;
		while (i < num)
		{
			this.skillLists[i] = new List<BaseNewSkill>();
			i++;
		}
		this.skillTypeMap = new Dictionary<int, Type>();
		this.buffTypeMap = new Dictionary<int, Type>();
	}

	public bool Init()
	{
		this.RegistSkillType(SkillLogicId.BornSkill, typeof(BornSkill));
		this.RegistSkillType(SkillLogicId.OccupiedSkill, typeof(OccupiedSkill));
		this.RegistSkillType(SkillLogicId.CapturedSkill, typeof(CapturedSkill));
		this.RegistSkillType(SkillLogicId.BeCapturedSkill, typeof(BeCapturedSkill));
		this.RegistSkillType(SkillLogicId.TeamInitiativeSkill, typeof(TeamInitiativeSkill));
		this.RegistSkillType(SkillLogicId.JinGuSkill, typeof(JinGuSkill));
		this.RegistSkillType(SkillLogicId.TianShangSkill, typeof(TianShangSkill));
		this.RegistSkillType(SkillLogicId.DuWuSkill, typeof(DuWuSkill));
		this.RegistSkillType(SkillLogicId.FengBaoSkill, typeof(FengBaoSkill));
		this.RegistSkillType(SkillLogicId.SiShiSkill, typeof(SiShiSkill));
		this.RegistSkillType(SkillLogicId.ChouXinSkill, typeof(ChouXinSkill));
		this.RegistSkillType(SkillLogicId.NiePanSkill, typeof(NiePanSkill));
		this.RegistSkillType(SkillLogicId.ChongWangSkill, typeof(ChongWangSkill));
		this.RegistSkillType(SkillLogicId.ChongDongSkill, typeof(ChongDongSkill));
		this.RegistSkillType(SkillLogicId.QiXiSkill, typeof(QiXiSkill));
		this.RegistSkillType(SkillLogicId.YiHuaSkill, typeof(YiHuaSkill));
		this.RegistSkillType(SkillLogicId.WenYiSkill, typeof(WenYiSkill));
		this.RegistSkillType(SkillLogicId.IntensifyAttackSkill, typeof(IntensifyTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.WeakenAttackSkill, typeof(WeakenTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.IntensifySpeedSkill, typeof(IntensifyTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.WeakenSpeedSkill, typeof(WeakenTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.IntensifyOccupidSkill, typeof(IntensifyTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.WeakenOccupidSkill, typeof(WeakenTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.IntensifyHPSkill, typeof(IntensifyTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.WeakenHPSkill, typeof(WeakenTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.HideStartSkill, typeof(HideStartTeamAttrSkill));
		this.RegistSkillType(SkillLogicId.LasercannonSkill, typeof(LasercannonSkill));
		this.RegistSkillType(SkillLogicId.InhibitSkill, typeof(InhibitSkill));
		this.RegistSkillType(SkillLogicId.SacrificeSkill, typeof(SacrificeSkill));
		this.RegistSkillType(SkillLogicId.OverKillSkill, typeof(OverKillSkill));
		this.RegistSkillType(SkillLogicId.CloneToSkill, typeof(CloneToSkill));
		this.RegistSkillType(SkillLogicId.TreasureSkill, typeof(TreasureSkill));
		this.RegistSkillType(SkillLogicId.UnknownStarSkill, typeof(UnknownStarSkill));
		this.RegistSkillType(SkillLogicId.RotateSkill, typeof(RotateSkill));
		this.RegistSkillType(SkillLogicId.MirrorSkill, typeof(MirrorSkill));
		this.RegistSkillType(SkillLogicId.DiffusionSkill, typeof(DiffusionSkill));
		this.RegistSkillType(SkillLogicId.CurseSkill, typeof(CurseWrapperSkill));
		this.RegistSkillType(SkillLogicId.CannonSkill, typeof(CannonSkill));
		this.RegistBuffType(SkillBuffLogicId.PlanetShipNumBuff, typeof(PlanetShipNumBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamShipSpeedBuff, typeof(TeamShipSpeedBuff));
		this.RegistBuffType(SkillBuffLogicId.IceBuff, typeof(IceBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamOccupiedSpeedBuff, typeof(TeamOccupiedSpeedBuff));
		this.RegistBuffType(SkillBuffLogicId.NodeOccupiedSpeedBuff, typeof(NodeOccupiedSpeedBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamHideBuff, typeof(TeamHideBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamPopulationMaxBuff, typeof(TeamPopulationMaxBuff));
		this.RegistBuffType(SkillBuffLogicId.NodePopulationBuff, typeof(NodePopulationBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamProduceSpeedBuff, typeof(TeamProduceSpeedBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamBeCapturedSpeedBuff, typeof(TeamBeCapturedSpeedBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamStopCaptureBuff, typeof(TeamStopBeCaptureBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamQuickMoveBuff, typeof(TeamQuickMoveBuff));
		this.RegistBuffType(SkillBuffLogicId.PlanetShipNumNoLimitBuff, typeof(PlanetShipNumNoLimitBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamPopulationMaxFakeBuff, typeof(TeamPopulationMaxFakeBuff));
		this.RegistBuffType(SkillBuffLogicId.PlanetShipNumAllEnemyBuff, typeof(PlanetShipNumAllEnemyBuff));
		this.RegistBuffType(SkillBuffLogicId.NodeShipReliveBuff, typeof(NodeShipReliveBuff));
		this.RegistBuffType(SkillBuffLogicId.NodeShipCallBuff, typeof(NodeShipCallBuff));
		this.RegistBuffType(SkillBuffLogicId.ChongWangBuff, typeof(ChongWangBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamShipAttackBuff, typeof(TeamShipAttakBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamShipHPBuff, typeof(TeamShipHPBuff));
		this.RegistBuffType(SkillBuffLogicId.TeamHideStatr, typeof(TeamHideStatrBuff));
		this.RegistBuffType(SkillBuffLogicId.NodeProduceSpeedBuff, typeof(NodeProduceSpeedBuff));
		this.RegistBuffType(SkillBuffLogicId.NodePopulationMaxBuff, typeof(NodePopulationMaxBuff));
		this.RegistBuffType(SkillBuffLogicId.SacrificeBuff, typeof(SacrificeBuff));
		this.RegistBuffType(SkillBuffLogicId.OverKillBuff, typeof(OverKillBuff));
		this.RegistBuffType(SkillBuffLogicId.CloneToBuff, typeof(CloneToBuff));
		this.RegistBuffType(SkillBuffLogicId.TreasureBuff, typeof(TreasureBuff));
		return true;
	}

	public void Tick(int frame, float interval)
	{
		int i = 0;
		int num = this.skillLists.Length;
		while (i < num)
		{
			int j = 0;
			int count = this.skillLists[i].Count;
			while (j < count)
			{
				this.skillLists[i][j].Tick(frame, interval);
				j++;
			}
			i++;
		}
	}

	public void Destroy()
	{
		int i = 0;
		int num = this.skillLists.Length;
		while (i < num)
		{
			this.skillLists[i].Clear();
			i++;
		}
	}

	private void RegistSkillType(SkillLogicId id, Type type)
	{
		this.skillTypeMap.Add((int)id, type);
	}

	private void RegistBuffType(SkillBuffLogicId id, Type type)
	{
		this.buffTypeMap.Add((int)id, type);
	}

	public void AddSkill(Team t, int skillId)
	{
		NewSkillConfig data = Solarmax.Singleton<NewSkillConfigProvider>.Instance.GetData(skillId);
		if (data == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format("读取技能配置错误，技能id : {0}", skillId), new object[0]);
			return;
		}
		Type type = null;
		if (!this.skillTypeMap.TryGetValue(data.logicId, out type))
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format("获取技能类型错误，技能id : {0}", skillId), new object[0]);
			return;
		}
		BaseNewSkill baseNewSkill = Activator.CreateInstance(type) as BaseNewSkill;
		baseNewSkill.SetInfo(t, data, this.sceneManager, this);
		List<BaseNewSkill> list = this.skillLists[(int)t.team];
		list.Add(baseNewSkill);
		List<int> list2 = new List<int>();
		if (data.effectId > 0)
		{
			list2.Add(data.effectId);
		}
		int i = 0;
		int count = data.buffIds.Count;
		while (i < count)
		{
			NewSkillBuffConfig data2 = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(data.buffIds[i]);
			if (data2.effectId > 0)
			{
				list2.Add(data2.effectId);
			}
			i++;
		}
		int j = 0;
		int count2 = list2.Count;
		while (j < count2)
		{
			NewSkillEffectConfig data3 = Solarmax.Singleton<NewSkillEffectConfigProvider>.Instance.GetData(list2[j]);
			if (data3 != null)
			{
				Solarmax.Singleton<AssetManager>.Get().AddSprite(data3.effectName);
				Solarmax.Singleton<AssetManager>.Get().AddSound(data3.audio);
			}
			j++;
		}
	}

	public BaseNewSkill AddSkillEX(int skillId, Team t)
	{
		NewSkillConfig data = Solarmax.Singleton<NewSkillConfigProvider>.Instance.GetData(skillId);
		if (data == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format("读取技能配置错误，技能id : {0}", skillId), new object[0]);
			return null;
		}
		Type type = null;
		if (!this.skillTypeMap.TryGetValue(data.logicId, out type))
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format("获取技能类型错误，技能id : {0}", skillId), new object[0]);
			return null;
		}
		BaseNewSkill baseNewSkill = Activator.CreateInstance(type) as BaseNewSkill;
		baseNewSkill.SetInfo(t, data, this.sceneManager, this);
		List<int> list = new List<int>();
		if (data.effectId > 0)
		{
			list.Add(data.effectId);
		}
		int i = 0;
		int count = data.buffIds.Count;
		while (i < count)
		{
			NewSkillBuffConfig data2 = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(data.buffIds[i]);
			if (data2.effectId > 0)
			{
				list.Add(data2.effectId);
			}
			i++;
		}
		int j = 0;
		int count2 = list.Count;
		while (j < count2)
		{
			NewSkillEffectConfig data3 = Solarmax.Singleton<NewSkillEffectConfigProvider>.Instance.GetData(list[j]);
			if (data3 != null)
			{
				Solarmax.Singleton<AssetManager>.Get().AddSprite(data3.effectName);
				Solarmax.Singleton<AssetManager>.Get().AddSound(data3.audio);
			}
			j++;
		}
		return baseNewSkill;
	}

	public BaseNewBuff NewBuff(int buffid)
	{
		Type type = null;
		if (!this.buffTypeMap.TryGetValue(buffid, out type))
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format("获取buff类型错误，buffid : {0}", buffid), new object[0]);
			return null;
		}
		return Activator.CreateInstance(type) as BaseNewBuff;
	}

	public void OnBorn(Team t)
	{
		List<BaseNewSkill> list = this.skillLists[(int)t.team];
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			list[i].OnBorn(t);
			i++;
		}
	}

	public void OnCaptured(Node node, Team before, Team now)
	{
		List<BaseNewSkill> list = this.skillLists[(int)before.team];
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			list[i].OnBeCaptured(node, before, now);
			i++;
		}
		list = this.skillLists[(int)now.team];
		int j = 0;
		int count2 = list.Count;
		while (j < count2)
		{
			list[j].OnCaptured(node, before, now);
			j++;
		}
	}

	public void OnOccupied(Node node, Team now)
	{
		List<BaseNewSkill> list = this.skillLists[(int)now.team];
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			list[i].OnOccupied(node, now);
			i++;
		}
	}

	public void OnCast(SkillPacket packet)
	{
		int skillID = packet.skillID;
		TEAM from = packet.from;
		Team team = this.sceneManager.teamManager.GetTeam(from);
		List<BaseNewSkill> list = this.skillLists[(int)from];
		BaseNewSkill baseNewSkill = null;
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			if (list[i].config.skillId == skillID)
			{
				baseNewSkill = list[i];
				break;
			}
			i++;
		}
		try
		{
			if (baseNewSkill != null)
			{
				baseNewSkill.OnCast(team);
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Cast skill error, Team:{0}, SkillId:{1}", new object[]
			{
				from,
				skillID
			});
			Debug.LogError(ex.Message);
		}
	}

	public BaseNewSkill GetSkill(Team team, int skillId)
	{
		List<BaseNewSkill> list = this.skillLists[(int)team.team];
		BaseNewSkill result = null;
		int i = 0;
		int count = list.Count;
		while (i < count)
		{
			if (list[i].config.skillId == skillId)
			{
				result = list[i];
				break;
			}
			i++;
		}
		return result;
	}

	private SceneManager sceneManager;

	private List<BaseNewSkill>[] skillLists;

	private Dictionary<int, Type> skillTypeMap;

	private Dictionary<int, Type> buffTypeMap;
}
