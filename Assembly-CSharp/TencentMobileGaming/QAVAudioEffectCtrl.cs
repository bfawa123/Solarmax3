using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AOT;

namespace TencentMobileGaming
{
	public class QAVAudioEffectCtrl : ITMGAudioEffectCtrl
	{
		[MonoPInvokeCallback(typeof(QAVAccompanyFileCompleteHandler))]
		private static void s_OnAccompanyComplete(int code, bool isfinished, string filepath)
		{
			if (QAVContext.GetInstance().GetAudioEffectCtrlInner().OnAccompanyFileCompleteHandler != null)
			{
				QAVContext.GetInstance().GetAudioEffectCtrlInner().OnAccompanyFileCompleteHandler(code, isfinished, filepath);
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		// 修复
		public override event QAVAccompanyFileCompleteHandler OnAccompanyFileCompleteHandler;

		public override int StartAccompany(string filePath, bool loopBack, int loopCount, int duckerTimeMs)
		{
			int fileSize = 0;
			if (QAVAudioEffectCtrl.cache0 == null)
			{
				QAVAudioEffectCtrl.cache0 = new QAVAccompanyFileCompleteHandler(QAVAudioEffectCtrl.s_OnAccompanyComplete);
			}
			return QAVNative.QAVSDK_AVAudioCtrl_StartAccompany(filePath, loopBack, loopCount, duckerTimeMs, fileSize, QAVAudioEffectCtrl.cache0);
		}

		public override int StartAccompanyDownloading(string filePath, bool loopBack, int loopCount, int duckerTimeMs, int fileSize)
		{
			if (QAVAudioEffectCtrl.cache1 == null)
			{
				QAVAudioEffectCtrl.cache1 = new QAVAccompanyFileCompleteHandler(QAVAudioEffectCtrl.s_OnAccompanyComplete);
			}
			return QAVNative.QAVSDK_AVAudioCtrl_StartAccompany(filePath, loopBack, loopCount, duckerTimeMs, fileSize, QAVAudioEffectCtrl.cache1);
		}

		public override int StopAccompany(int duckerTimeMs)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_StopAccompany(duckerTimeMs);
		}

		public override bool IsAccompanyPlayEnd()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAccompanyPlayEnd();
		}

		public override int PauseAccompany()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_PauseAccompany();
		}

		public override int ResumeAccompany()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_ResumeAccompany();
		}

		public override int EnableAccompanyPlay(bool enable)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAccompanyPlay(enable);
		}

		public override int EnableAccompanyLoopBack(bool enable)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAccompanyLoopBack(enable);
		}

		public override int SetAccompanyVolume(int vol)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetAccompanyVolume(vol);
		}

		public override int GetAccompanyVolume()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetAccompanyVolume();
		}

		public override uint GetAccompanyFileTotalTimeByMs()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetAccompanyFileTotalTimeByMs();
		}

		public override uint GetAccompanyFileCurrentPlayedTimeByMs()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetAccompanyFileCurrentPlayedTimeByMs();
		}

		public override int SetAccompanyFileCurrentPlayedTimeByMs(uint timeMs)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetAccompanyFileCurrentPlayedTimeByMs(timeMs);
		}

		public override int GetEffectsVolume()
		{
			return 0;
		}

		public override int SetEffectsVolume(int volume)
		{
			return 0;
		}

		public override int PlayEffect(int soundId, string filePath, bool loop = false, double pitch = 1.0, double pan = 0.0, double gain = 1.0)
		{
			return 0;
		}

		public override int PauseEffect(int soundId)
		{
			return 0;
		}

		public override int PauseAllEffects()
		{
			return 0;
		}

		public override int ResumeEffect(int soundId)
		{
			return 0;
		}

		public override int ResumeAllEffects()
		{
			return 0;
		}

		public override int StopEffect(int soundId)
		{
			return 0;
		}

		public override int StopAllEffects()
		{
			return 0;
		}

		public override int SetVoiceType(int voiceType)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetVoiceType(voiceType);
		}

		public override int SetKaraokeType(int type)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetKaraokeType(type);
		}

		[CompilerGenerated]
		private static QAVAccompanyFileCompleteHandler cache0;

		[CompilerGenerated]
		private static QAVAccompanyFileCompleteHandler cache1;
	}
}
