using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ReplayManager : Lifecycle2
{
	public ReplayManager(BattleData bd)
	{
		this.battleData = bd;
	}

	public bool Init()
	{
		this.curPlayRecord = null;
		this.reportData = null;
		return true;
	}

	public void Tick(int frame, float interval)
	{
		this.UpdateRecord(frame, interval);
	}

	public void Destroy()
	{
		this.battleData.isReplay = false;
	}

	public void TryPlayRecord(PbSCFrames msg, bool levelRecord = false)
	{
		this.battleData.isReplay = true;
		this.battleData.isLevelReplay = levelRecord;
		this.curPlayRecord = msg;
		this.battleData.matchId = this.curPlayRecord.ready.match_id;
		Solarmax.Singleton<NetSystem>.Instance.helper.OnReady(this.curPlayRecord.ready);
		this.battleData.isReplay = true;
		Solarmax.Singleton<NetSystem>.Instance.helper.OnNetStartBattle(this.curPlayRecord.start);
		this.AddReplayFrame();
	}

	public void TryPlayScript(PbSCFrames msg)
	{
		this.battleData.isReplay = true;
		this.curPlayRecord = msg;
		this.battleData.matchId = this.curPlayRecord.ready.match_id;
		Solarmax.Singleton<NetSystem>.Instance.helper.OnReady(this.curPlayRecord.ready);
		Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript = true;
		this.battleData.isReplay = true;
		Solarmax.Singleton<NetSystem>.Instance.helper.OnNetStartBattle(this.curPlayRecord.start);
		this.FramesCount = 0;
		for (int i = 0; i < this.curPlayRecord.frames.Count; i++)
		{
			SCFrame scframe = this.curPlayRecord.frames[i];
			this.FramesCount = scframe.frameNum;
		}
		Solarmax.Singleton<BattleSystem>.Instance.OnRecievedScriptFrame(this.curPlayRecord);
	}

	public void AddReplayFrame()
	{
		if (this.curPlayRecord == null)
		{
			return;
		}
		for (int i = 0; i < this.curPlayRecord.frames.Count; i++)
		{
			SCFrame scframe = this.curPlayRecord.frames[i];
			Solarmax.Singleton<NetSystem>.Instance.helper.OnNetFrame(scframe);
			this.FramesCount = scframe.frameNum;
		}
	}

	public bool PlayRecordOver()
	{
		if (this.curPlayRecord == null)
		{
			return false;
		}
		this.curPlayRecord = null;
		Time.timeScale = 1f;
		return true;
	}

	private void UpdateRecord(int frame, float dt)
	{
		if (this.curPlayRecord != null)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBattleReplayFrame, new object[]
			{
				frame,
				this.FramesCount
			});
		}
	}

	public void NotifyPlayRecordEnd()
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType == BattlePlayType.Replay)
		{
			return;
		}
		SCFinishBattle finish = this.curPlayRecord.finish;
		for (int i = 0; i < finish.users.Count; i++)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(finish.users[i]);
			if (team != null)
			{
				team.scoreMod = finish.score_mods[i];
				team.resultOrder = -3 + i;
				team.resultRank = finish.rank[i];
				team.resultEndtype = finish.end_type[i];
				if (finish.mvp_num.Count > 0)
				{
					team.leagueMvp = finish.mvp_num[i];
				}
				for (int j = 0; j < this.reportData.playerList.Count; j++)
				{
					if (team.playerData.userId == this.reportData.playerList[j].userId && team.destory != this.reportData.playerList[j].destroyNum)
					{
						Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Format("数据数据不一致!队伍: {0}, 摧毁数据，服务器: {1}, 本地: {2}", team.team, this.reportData.playerList[i].destroyNum, team.destory), new object[0]);
					}
				}
			}
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFinished, new object[0]);
		Solarmax.Singleton<UISystem>.Get().HideWindow("UnTouchWindow");
	}

	public void SetPlaySpeed(int value)
	{
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = (float)value;
	}

	public void SetPlayToPercent(float percent)
	{
		if (this.curPlayRecord == null)
		{
			return;
		}
		int num = (int)((double)(percent * (float)this.curPlayRecord.frames.Count) + 0.5);
		if (num >= this.curPlayRecord.frames.Count)
		{
			num = this.curPlayRecord.frames.Count - 1;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.runFrameCount = 20;
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.RunToFrame(num);
	}

	public BattleData battleData;

	public BattleReportData reportData;

	public PbSCFrames curPlayRecord;

	private int FramesCount;
}
