using System;

namespace TencentMobileGaming
{
	public abstract class ITMGAudioEffectCtrl
	{
		public abstract event QAVAccompanyFileCompleteHandler OnAccompanyFileCompleteHandler;

		public abstract int StartAccompany(string filePath, bool loopBack, int loopCount, int duckerTimeMs);

		public abstract int StartAccompanyDownloading(string filePath, bool loopBack, int loopCount, int duckerTimeMs, int fileSize);

		public abstract int StopAccompany(int duckerTimeMs);

		public abstract bool IsAccompanyPlayEnd();

		public abstract int PauseAccompany();

		public abstract int ResumeAccompany();

		public abstract int EnableAccompanyPlay(bool enable);

		public abstract int EnableAccompanyLoopBack(bool enable);

		public abstract int SetAccompanyVolume(int vol);

		public abstract int GetAccompanyVolume();

		public abstract uint GetAccompanyFileTotalTimeByMs();

		public abstract uint GetAccompanyFileCurrentPlayedTimeByMs();

		public abstract int SetAccompanyFileCurrentPlayedTimeByMs(uint time);

		public abstract int GetEffectsVolume();

		public abstract int SetEffectsVolume(int volume);

		public abstract int PlayEffect(int soundId, string filePath, bool loop = false, double pitch = 1.0, double pan = 0.0, double gain = 1.0);

		public abstract int PauseEffect(int soundId);

		public abstract int PauseAllEffects();

		public abstract int ResumeEffect(int soundId);

		public abstract int ResumeAllEffects();

		public abstract int StopEffect(int soundId);

		public abstract int StopAllEffects();

		public abstract int SetVoiceType(int voiceType);

		public abstract int SetKaraokeType(int type);

		public static int VOICE_TYPE_ORIGINAL_SOUND;

		public static int VOICE_TYPE_LOLITA = 1;

		public static int VOICE_TYPE_UNCLE = 2;

		public static int VOICE_TYPE_INTANGIBLE = 3;

		public static int VOICE_TYPE_DEAD_FATBOY = 4;

		public static int VOICE_TYPE_HEAVY_MENTAL = 5;

		public static int VOICE_TYPE_DIALECT = 6;

		public static int VOICE_TYPE_INFLUENZA = 7;

		public static int VOICE_TYPE_CAGED_ANIMAL = 8;

		public static int VOICE_TYPE_HEAVY_MACHINE = 9;

		public static int VOICE_TYPE_STRONG_CURRENT = 10;

		public static int VOICE_TYPE_KINDER_GARTEN = 11;

		public static int VOICE_TYPE_HUANG = 12;
	}
}
