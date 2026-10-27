using System;
using System.Collections.Generic;
using NetMessage;
using Plugin;
using Solarmax;
using UnityEngine;

public class BattleSystem : Solarmax.Singleton<BattleSystem>, Lifecycle
{
	public BattleSystem()
	{
		this.battleData = new BattleData();
		this.lockStep = new LockStep();
		this.sceneManager = new SceneManager(this.battleData);
		this.battleController = null;
		this.pvpBattleController = new PVPBattleController(this.battleData, this);
		this.singleBattleController = new SingleBattleController(this.battleData, this);
		this.replayManager = new ReplayManager(this.battleData);
	}

	public int currentFrameCount
	{
		get
		{
			return (this.lockStep != null) ? this.lockStep.messageCount : 0;
		}
	}

	public int currentFps
	{
		get
		{
			return (this.lockStep != null) ? this.lockStep.FPS : 0;
		}
	}

	public bool Init()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("BattleSystem    init  begin", new object[0]);
		this.lockStep.frameThreshold = 3;
		this.lockStep.AddListennerLogic(new RunLockStepLogic(this.FrameTick));
		this.lockStep.AddListennerPacket(new RunPacketHandler(this.FramePacketRun));
		string data = Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(3);
		string[] array = data.Split(new char[]
		{
			';'
		});
		for (int i = 0; i < array.Length; i++)
		{
			List<float> list = Converter.ConvertNumberList<float>(array[i]);
			this.sceneManager.AddProduce(list[0], list[1]);
		}
		this.battleData.Init();
		this.sceneManager.Init();
		this.replayManager.Init();
		Solarmax.Singleton<AssetManager>.Get().Init();
		Solarmax.Singleton<EffectManager>.Get().Init();
		Solarmax.Singleton<GameTimeManager>.Get().Init();
		this.pause = false;
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("BattleSystem    init  end", new object[0]);
		return true;
	}

	public void FrameTick(int frame, float interval)
	{
		if (this.battleController != null)
		{
			this.battleController.Tick(frame, interval);
		}
		this.battleData.Tick(frame, interval);
		this.sceneManager.Tick(frame, interval);
		if (this.battleData.battleType == BattlePlayType.Normalize)
		{
			ReplayCollectManager.Get().Tick(frame, interval);
		}
		this.replayManager.Tick(frame, interval);

		// 反同步校验（临时调试）：每帧记录随机状态 + 状态哈希，用于定位联机帧同步不同步
		if (this.battleData.gameType == GameType.PVP)
		{
			Debug.Log(string.Format("[SyncCheck] f={0} r={1} a={2} h={3}", frame, this.battleData.rand.seed, this.battleData.aiRand.seed, this.sceneManager.ComputeSyncHash()));
		}
	}

	public void FramePacketRun(FrameNode frameNode)
	{
		if (this.battleController != null)
		{
			this.battleController.OnRunFramePacket(frameNode);
		}
	}

	public void Tick(float interval)
	{
		if (this.pause)
		{
			return;
		}
		this.lockStep.Tick(interval);
		float num = this.sceneManager.GetbattleScaleSpeed();
		float interval2 = interval * num;
		Solarmax.Singleton<EffectManager>.Get().fPlayAniSpeed = num;
		Solarmax.Singleton<EffectManager>.Get().Tick(Time.frameCount, interval2);
		Solarmax.Singleton<ShipFadeManager>.Get().UpdateFadeInOut(interval);
	}

	public void UpdateRender(float interval)
	{
		if (this.pause)
		{
			return;
		}
		this.battleData.silent = false;
		this.sceneManager.UpdateRender(interval);
	}

	public void Destroy()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("BattleSystem    destroy  begin", new object[0]);
		this.Reset();
		if (this.battleController != null)
		{
			this.battleController.Destroy();
		}
		this.lockStep.StopLockStep(true);
		Solarmax.Singleton<AssetManager>.Get().UnLoadBattleResources();
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("BattleSystem    destroy  end", new object[0]);
	}

	public void Reset()
	{
		this.pause = false;
		this.bStartBattle = false;
		this.BeginFadeOut();
		this.battleData.Init();
		if (this.battleController != null)
		{
			this.battleController.Reset();
		}
		this.lockStep.StopLockStep(false);
		this.sceneManager.Destroy();
		this.battleData.Destroy();
		this.replayManager.Destroy();
		TouchHandler.Clean();
		Solarmax.Singleton<GameTimeManager>.Get().Release();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void BeginFadeOut()
	{
		Solarmax.Singleton<EffectManager>.Instance.Destroy();
	}

	public void SetPlayMode(bool pvp, bool single)
	{
		if (pvp)
		{
			this.battleController = this.pvpBattleController;
		}
		else if (single)
		{
			this.battleController = this.singleBattleController;
		}
		else
		{
			this.battleController = null;
			Solarmax.Singleton<LoggerSystem>.Instance.Error("PlayMode error!", new object[0]);
		}
		Solarmax.Singleton<LoggerSystem>.Instance.Info("设置战斗模式为：pvp:{0}, single:{1}", new object[]
		{
			pvp,
			single
		});
	}

	public void OnPlayerMove(Node from, Node to)
	{
		this.battleController.OnPlayerMove(from, to);
	}

	public void OnRecievedFramePacket(SCFrame frame)
	{
		this.battleController.OnRecievedFramePacket(frame);
	}

	public void OnRecievedScriptFrame(PbSCFrames frame)
	{
		this.battleController.OnRecievedScriptFrame(frame);
	}

	public void PlayerGiveUp()
	{
		this.battleController.PlayerGiveUp();
	}

	public void OnPlayerGiveUp(TEAM team)
	{
		this.battleController.OnPlayerGiveUp(team);
	}

	public void OnPlayerDirectQuit()
	{
		this.battleController.OnPlayerDirectQuit(this.battleData.currentTeam);
	}

	public int GetCurrentFrame()
	{
		return this.lockStep.GetCurrentFrame();
	}

	public void StartLockStep()
	{
		if (this.battleController != null)
		{
			this.battleController.Init();
		}
		Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = false;
		this.pause = false;
		this.bStartBattle = true;
		this.lockStep.StarLockStep();
		if (this.battleData.battleType == BattlePlayType.Normalize)
		{
			ReplayCollectManager.Get().Init();
		}
	}

	public void StopLockStep()
	{
		this.lockStep.playSpeed = 0f;
		this.lockStep.StopLockStep(false);
	}

	public void SetPause(bool status)
	{
		this.pause = status;
		if (this.onPauseDelegate != null)
		{
			this.onPauseDelegate(status);
		}
	}

	public bool IsPause()
	{
		return this.pause;
	}

	public LockStep lockStep;

	public SceneManager sceneManager;

	public BattleData battleData;

	public ReplayManager replayManager;

	private IBattleController battleController;

	public SingleBattleController singleBattleController;

	public PVPBattleController pvpBattleController;

	private bool pause;

	public bool bStartBattle;

	public bool canOperation = true;

	public BattleSystem.OnPauseDelegate onPauseDelegate;

	public delegate void OnPauseDelegate(bool pause);
}
