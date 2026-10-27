using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ReplayCollectManager
{
	public ReplayCollectManager()
	{
		PbFrame pbFrame = new PbFrame();
		byte[] content = Json.EnCodeBytes(new FramePacket
		{
			type = 5
		});
		pbFrame.content = content;
	}

	public static ReplayCollectManager Get()
	{
		if (ReplayCollectManager.Instance == null)
		{
			ReplayCollectManager.Instance = new ReplayCollectManager();
		}
		return ReplayCollectManager.Instance;
	}

	public bool Init()
	{
		return true;
	}

	public void CreateReplayStruct(string LevelId, int seed)
	{
		this.pbRecord = new PbSCFrames();
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(LevelId);
		LevelConfig data2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(LevelId);
		int num = (data2.playerTeam >= 1) ? data2.playerTeam : 1;
		if (data == null)
		{
			return;
		}
		this.Ready = new SCReady();
		this.Ready.match_type = MatchType.MT_Sing;
		this.Ready.sub_type = CooperationType.CT_Null;
		this.Ready.match_id = LevelId;
		this.Ready.random_seed = seed;
		this.Ready.ai_type = string.Empty;
		this.Ready.ai_param = 0;
		this.LocalplayerTeam = num;
		List<MapPlayerConfig> list = new List<MapPlayerConfig>();
		for (int i = 0; i < data.mpcList.Count; i++)
		{
			if (data.mpcList[i].camption != 0)
			{
				list.Add(data.mpcList[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			UserData userData = new UserData();
			if (list[j].camption == num)
			{
				userData.userid = num;
				userData.name = Solarmax.Singleton<LocalPlayer>.Get().playerData.name;
				userData.icon = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
			}
			else
			{
				userData.userid = -list[j].camption;
				userData.name = "外星人";
				userData.icon = string.Empty;
			}
			userData.score = 0;
			userData.level = list[j].camption;
			this.Ready.data.Add(userData);
			if (string.IsNullOrEmpty(this.Ready.group))
			{
				SCReady ready = this.Ready;
				ready.group = ready.group + j.ToString() + "," + j.ToString();
			}
			else
			{
				SCReady ready2 = this.Ready;
				string group = ready2.group;
				ready2.group = string.Concat(new string[]
				{
					group,
					"|",
					j.ToString(),
					",",
					j.ToString()
				});
			}
		}
		SCStartBattle start = new SCStartBattle();
		SCFinishBattle scfinishBattle = new SCFinishBattle();
		scfinishBattle.users.Add(Solarmax.Singleton<LocalPlayer>.Get().playerData.userId);
		scfinishBattle.end_type.Add(EndType.ET_Timeout);
		this.pbRecord.finish = scfinishBattle;
		this.pbRecord.start = start;
		this.curPlayRecord = null;
	}

	public void Tick(int nFrame, float interval)
	{
	}

	public void EndReplayCollect(int nwin)
	{
		if (this.curPlayRecord == null)
		{
			this.AddEndFrame();
			string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
			this.SaveReplayCollect(matchId, nwin);
		}
	}

	public void Destroy()
	{
		this.pbRecord = null;
		this.curPlayRecord = null;
		this.curFrames.Clear();
		this.localRecordList.Clear();
	}

	public void AddReplayFrame(PbFrame frame, int FrameNum = -1)
	{
		PbFrames pbFrames = new PbFrames();
		pbFrames.frames.Add(frame);
		SCFrame scframe = new SCFrame();
		if (FrameNum >= 0)
		{
			scframe.frameNum = FrameNum;
		}
		else
		{
			scframe.frameNum = Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame();
		}
		scframe.frames.Add(pbFrames);
		scframe.users.Add(this.LocalplayerTeam);
		this.pbRecord.frames.Add(scframe);
	}

	private void AddEndFrame()
	{
		PbFrames pbFrames = new PbFrames();
		pbFrames.frames.Add(this.emptyFrame);
		SCFrame scframe = new SCFrame();
		scframe.frameNum = Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame();
		scframe.frames.Add(pbFrames);
		scframe.users.Add(0);
		this.pbRecord.frames.Add(scframe);
	}

	public void SaveReplayCollect(string LevelId, int nwin)
	{
		string saveVideo = MonoSingleton<UpdateSystem>.Instance.saveVideo;
		if (!Directory.Exists(saveVideo))
		{
			Directory.CreateDirectory(saveVideo);
		}
		string account = Solarmax.Singleton<LocalAccountStorage>.Get().account;
		DateTime d = new DateTime(1970, 1, 1);
		long timeStamp = (long)(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).TotalSeconds;
		string text = string.Format("{0}_{1}", account, LevelId);
		string path = saveVideo + text + ".video";
		File.Delete(path);
		this.Ready.misc_id = string.Format("{0}", timeStamp.ToString());
		this.pbRecord.ready = this.Ready;
		this.curPlayRecord = this.pbRecord;
		using (FileStream fileStream = new FileStream(path, FileMode.Create))
		{
			byte[] array = Json.EnCodeBytes(this.curPlayRecord);
			try
			{
				fileStream.Write(array, 0, array.Length);
			}
			catch (Exception ex)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("write video failed{0}", new object[]
				{
					ex.ToString()
				});
			}
			finally
			{
				fileStream.Flush();
				fileStream.Close();
			}
			ReplayFile replayFile = this.ReplayIsExists(text);
			if (replayFile == null)
			{
				replayFile = new ReplayFile();
				this.localRecordList.Add(replayFile);
			}
			replayFile.fileName = text;
			replayFile.levelId = LevelId;
			replayFile.timeStamp = timeStamp;
			replayFile.nWin = nwin;
			this.localRecordList.Sort(delegate(ReplayFile a, ReplayFile b)
			{
				if (a.timeStamp > b.timeStamp)
				{
					return -1;
				}
				if (a.timeStamp < b.timeStamp)
				{
					return 1;
				}
				return 0;
			});
		}
	}

	public void LoadReplayFromDisk()
	{
		if (this.bLoadLocalFiles)
		{
			return;
		}
		this.localRecordList.Clear();
		this.bLoadLocalFiles = true;
		List<ReplayFile> list = new List<ReplayFile>();
		string saveVideo = MonoSingleton<UpdateSystem>.Instance.saveVideo;
		if (Directory.Exists(saveVideo))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(saveVideo);
			FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Name.EndsWith(".video"))
				{
					string text = files[i].Name.Replace(".video", string.Empty);
					string[] array = text.Split(new char[]
					{
						'_'
					});
					if (array.Length <= 2)
					{
						ReplayFile replayFile = new ReplayFile();
						replayFile.fileName = text;
						replayFile.levelId = array[1];
						PbSCFrames pbSCFrames = this.LoadFileToReplayData(files[i].FullName, true);
						if (pbSCFrames != null)
						{
							string misc_id = pbSCFrames.ready.misc_id;
							replayFile.timeStamp = long.Parse(misc_id);
							replayFile.nWin = 1;
						}
						list.Add(replayFile);
					}
				}
			}
			list.Sort(delegate(ReplayFile a, ReplayFile b)
			{
				if (a.timeStamp > b.timeStamp)
				{
					return -1;
				}
				if (a.timeStamp < b.timeStamp)
				{
					return 1;
				}
				return 0;
			});
			int num = 50;
			if (list.Count < num)
			{
				num = list.Count;
			}
			for (int j = 0; j < num; j++)
			{
				this.localRecordList.Add(list[j]);
			}
			list.Clear();
			GC.Collect();
		}
	}

	public PbSCFrames LoadFileToReplayData(string file, bool isfull = false)
	{
		this.curPlayRecord = null;
		string text = MonoSingleton<UpdateSystem>.Instance.saveVideo + file + ".video";
		if (isfull)
		{
			text = file;
		}
		FileInfo fileInfo = new FileInfo(text);
		if (!fileInfo.Exists)
		{
			return null;
		}
		using (FileStream fileStream = new FileStream(text, FileMode.Open))
		{
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			fileStream.Close();
			try
			{
				this.curPlayRecord = Json.DeCode<PbSCFrames>(array);
			}
			catch (Exception ex)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("解析录像文件错误：{0}", new object[]
				{
					ex.ToString()
				});
			}
		}
		return this.curPlayRecord;
	}

	public PbSCFrames LoadScriptFileToReplayData(string mapId, bool isfull = false)
	{
		this.curPlayRecord = null;
		string path = Application.streamingAssetsPath + "/data/maplist/" + mapId + ".script";
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
			this.CreateReplayStruct(mapId, 0);
			List<ScriptFrame> list = new List<ScriptFrame>();
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (!string.IsNullOrEmpty(text) && !(text.Substring(0, 2) == "//"))
				{
					ScriptFrame scriptFrame = new ScriptFrame();
					string text2 = text.Split(new char[]
					{
						']'
					})[0];
					text2 = text2.Split(new char[]
					{
						'['
					})[1];
					scriptFrame.frame = (int.Parse(text2.Split(new char[]
					{
						':'
					})[0]) * 60 * 1000 + int.Parse(text2.Split(new char[]
					{
						':'
					})[1]) * 1000 + int.Parse(text2.Split(new char[]
					{
						':'
					})[2])) / 20;
					text2 = text.Split(new char[]
					{
						']'
					})[1];
					scriptFrame.tag = text2.Split(new char[]
					{
						'['
					})[1];
					scriptFrame.type = 0;
					if (scriptFrame.tag.IndexOf('1') >= 0)
					{
						scriptFrame.type = 1;
					}
					if (scriptFrame.tag.IndexOf('(') >= 0)
					{
						scriptFrame.tag = scriptFrame.tag.Split(new char[]
						{
							'('
						})[0];
					}
					text2 = text.Split(new char[]
					{
						']'
					})[2];
					scriptFrame.act = text2.Split(new char[]
					{
						'['
					})[1];
					scriptFrame.pars = string.Empty;
					if (scriptFrame.act.IndexOf('(') >= 0)
					{
						scriptFrame.pars = scriptFrame.act.Split(new char[]
						{
							'('
						})[1];
						scriptFrame.pars = scriptFrame.pars.Split(new char[]
						{
							')'
						})[0];
						scriptFrame.act = scriptFrame.act.Split(new char[]
						{
							'('
						})[0];
					}
					list.Add(scriptFrame);
					text2 = text.Split(new char[]
					{
						']'
					})[3];
					text2 = text2.Split(new char[]
					{
						'['
					})[1];
					int num = int.Parse(text2.Split(new char[]
					{
						','
					})[0]);
					int num2 = 0;
					if (num > 0)
					{
						num2 = int.Parse(text2.Split(new char[]
						{
							','
						})[1]) / num;
					}
					for (int i = 0; i < num2; i++)
					{
						list.Add(new ScriptFrame
						{
							frame = scriptFrame.frame + num * (i + 1),
							tag = scriptFrame.tag,
							type = scriptFrame.type,
							act = scriptFrame.act,
							pars = scriptFrame.pars
						});
					}
				}
			}
			list.Sort(delegate(ScriptFrame a, ScriptFrame b)
			{
				if (a.frame > b.frame)
				{
					return 1;
				}
				if (a.frame < b.frame)
				{
					return -1;
				}
				return 0;
			});
			for (int j = 0; j < list.Count; j++)
			{
				ScriptFrame scriptFrame2 = list[j];
				byte[] array = null;
				PbFrame pbFrame = new PbFrame();
				if (scriptFrame2.act == "Move")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 0,
						move = new MovePacket
						{
							team = (TEAM)int.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[0]),
							from = scriptFrame2.pars.Split(new char[]
							{
								','
							})[1],
							to = scriptFrame2.pars.Split(new char[]
							{
								','
							})[2],
							rate = float.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[3])
						}
					});
				}
				else if (scriptFrame2.act == "Attack")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 7,
						attack = new PlanetAttack
						{
							tag = scriptFrame2.tag
						}
					});
				}
				else if (scriptFrame2.act == "AddAttack")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 8,
						addattack = new AddAttack
						{
							tag = scriptFrame2.tag
						}
					});
				}
				else if (scriptFrame2.act == "GunturretAttack")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 9,
						gunattack = new GunturretAttack
						{
							tag = scriptFrame2.tag
						}
					});
				}
				else if (scriptFrame2.act == "LasergunAttack")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 10,
						laserattack = new LasergunAttack
						{
							tag = scriptFrame2.tag
						}
					});
				}
				else if (scriptFrame2.act == "TwistShip")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 11,
						twistattack = new TwistAttack
						{
							tag = scriptFrame2.tag
						}
					});
				}
				else if (scriptFrame2.act == "Skill")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 14,
						skill = new SkillPacket
						{
							tag = scriptFrame2.tag,
							from = (TEAM)int.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[0]),
							to = (TEAM)int.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[1]),
							skillID = int.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[2])
						}
					});
				}
				else if (scriptFrame2.act == "UnknownSkill")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 15,
						unknown = new UnknownSkillPacket
						{
							tag = scriptFrame2.tag,
							from = (TEAM)int.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[0]),
							to = (TEAM)int.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[1]),
							skillID = int.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[2]),
							transformId = scriptFrame2.pars.Split(new char[]
							{
								','
							})[3]
						}
					});
				}
				else if (scriptFrame2.act == "Bomb")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 4,
						bomb = new PlanetBomb
						{
							tag = scriptFrame2.tag
						}
					});
				}
				else if (scriptFrame2.act == "Effect")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 6,
						effect = new DriftEffect
						{
							tag = scriptFrame2.tag,
							effect = scriptFrame2.pars.Split(new char[]
							{
								','
							})[0],
							time = float.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[1]),
							scale = float.Parse(scriptFrame2.pars.Split(new char[]
							{
								','
							})[2])
						}
					});
				}
				else if (scriptFrame2.act == "Team")
				{
					array = Json.EnCodeBytes(new FramePacket
					{
						type = 12,
						team = new PlanetTeam
						{
							tag = scriptFrame2.tag,
							t = (TEAM)int.Parse(scriptFrame2.pars)
						}
					});
				}
				if (array != null)
				{
					pbFrame.content = array;
					this.AddReplayFrame(pbFrame, scriptFrame2.frame);
				}
			}
			this.AddEndFrame();
			this.Ready.misc_id = "0";
			this.pbRecord.ready = this.Ready;
			this.curPlayRecord = this.pbRecord;
			fileStream.Close();
			streamReader.Close();
		}
		return this.curPlayRecord;
	}

	private ReplayFile ReplayIsExists(string fileName)
	{
		for (int i = 0; i < this.localRecordList.Count; i++)
		{
			if (this.localRecordList[i].fileName.Equals(fileName))
			{
				return this.localRecordList[i];
			}
		}
		return null;
	}

	public static ReplayCollectManager Instance;

	private PbSCFrames pbRecord;

	private SCReady Ready;

	public PbSCFrames curPlayRecord;

	private PbFrame emptyFrame;

	private List<PbFrame> curFrames = new List<PbFrame>();

	private int LocalplayerTeam = 1;

	public List<ReplayFile> localRecordList = new List<ReplayFile>();

	private bool bLoadLocalFiles;
}
