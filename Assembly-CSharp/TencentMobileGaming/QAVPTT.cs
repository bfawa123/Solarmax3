using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AOT;

namespace TencentMobileGaming
{
	public class QAVPTT : ITMGPTT
	{
		public new static QAVPTT GetInstance()
		{
			if (QAVPTT.sInstance == null)
			{
				QAVPTT.sInstance = new QAVPTT();
			}
			return QAVPTT.sInstance;
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVRecordFileCompleteCallback OnRecordFileComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVUploadFileCompleteCallback OnUploadFileComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVDownloadFileCompleteCallback OnDownloadFileComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVPlayFileCompleteCallback OnPlayFileComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVSpeechToTextCallback OnSpeechToTextComplete;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVStreamingRecognitionCallback OnStreamingSpeechComplete;

		public override int ApplyPTTAuthbuffer(byte[] authBuffer)
		{
			return QAVNative.QAVSDK_PTT_ApplyPTTAuthbuffer(authBuffer, authBuffer.Length);
		}

		public override int SetMaxMessageLength(int msTime)
		{
			return QAVNative.QAVSDK_PTT_SetMaxMessageLength(msTime);
		}

		public override int StartRecording(string filePath)
		{
			if (QAVPTT.cache0 == null)
			{
				QAVPTT.cache0 = new QAVRecordFileCompleteCallback(QAVPTT.s_OnRecordFileComplete);
			}
			return QAVNative.QAVSDK_PTT_StartRecording(filePath, QAVPTT.cache0);
		}

		public override int StopRecording()
		{
			return QAVNative.QAVSDK_PTT_StopRecording();
		}

		public override int CancelRecording()
		{
			return QAVNative.QAVSDK_PTT_CancelRecording();
		}

		public override int UploadRecordedFile(string filePath)
		{
			if (QAVPTT.cache1 == null)
			{
				QAVPTT.cache1 = new QAVUploadFileCompleteCallback(QAVPTT.s_OnUploadFileComplete);
			}
			return QAVNative.QAVSDK_PTT_UploadRecordedFile(filePath, QAVPTT.cache1);
		}

		public override int DownloadRecordedFile(string fileID, string downloadFilePath)
		{
			if (QAVPTT.cache2 == null)
			{
				QAVPTT.cache2 = new QAVDownloadFileCompleteCallback(QAVPTT.s_OnDownloadFileComplete);
			}
			return QAVNative.QAVSDK_PTT_DownloadRecordedFile(fileID, downloadFilePath, QAVPTT.cache2);
		}

		public override int PlayRecordedFile(string filePath)
		{
			if (QAVPTT.cache3 == null)
			{
				QAVPTT.cache3 = new QAVPlayFileCompleteCallback(QAVPTT.s_OnPlayFileComplete);
			}
			return QAVNative.QAVSDK_PTT_StartPlayFile(filePath, QAVPTT.cache3);
		}

		public override int StopPlayFile()
		{
			return QAVNative.QAVSDK_PTT_StopPlayFile();
		}

		public override int GetMicLevel()
		{
			return QAVNative.QAVSDK_PTT_GetMicLevel();
		}

		public override int SetMicVolume(int volume)
		{
			return QAVNative.QAVSDK_PTT_SetMicVolume(volume);
		}

		public override int GetMicVolume()
		{
			return QAVNative.QAVSDK_PTT_GetMicVolume();
		}

		public override int GetSpeakerLevel()
		{
			return QAVNative.QAVSDK_PTT_GetSpeakerLevel();
		}

		public override int SetSpeakerVolume(int volume)
		{
			return QAVNative.QAVSDK_PTT_SetSpeakerVolume(volume);
		}

		public override int GetSpeakerVolume()
		{
			return QAVNative.QAVSDK_PTT_GetSpeakerVolume();
		}

		public override int GetFileSize(string filePath)
		{
			return QAVNative.QAVSDK_PTT_GetFileSize(filePath);
		}

		public override int GetVoiceFileDuration(string filePath)
		{
			return QAVNative.QAVSDK_PTT_GetVoiceFileDuration(filePath);
		}

		public override int SpeechToText(string fileID)
		{
			string speechLanguage = "cmn-Hans-CN";
			string translateLanguage = "cmn-Hans-CN";
			if (QAVPTT.cache4 == null)
			{
				QAVPTT.cache4 = new QAVSpeechToTextCallback(QAVPTT.s_OnSpeechToTextComplete);
			}
			return QAVNative.QAVSDK_PTT_SpeechToText(fileID, speechLanguage, translateLanguage, QAVPTT.cache4);
		}

		public override int SpeechToText(string fileID, string speechLanguage)
		{
			if (QAVPTT.cache5 == null)
			{
				QAVPTT.cache5 = new QAVSpeechToTextCallback(QAVPTT.s_OnSpeechToTextComplete);
			}
			return QAVNative.QAVSDK_PTT_SpeechToText(fileID, speechLanguage, speechLanguage, QAVPTT.cache5);
		}

		public override int SpeechToText(string fileID, string speechLanguage, string translatelanguage)
		{
			if (QAVPTT.cache6 == null)
			{
				QAVPTT.cache6 = new QAVSpeechToTextCallback(QAVPTT.s_OnSpeechToTextComplete);
			}
			return QAVNative.QAVSDK_PTT_SpeechToText(fileID, speechLanguage, translatelanguage, QAVPTT.cache6);
		}

		public override int StartRecordingWithStreamingRecognition(string filePath)
		{
			string speechLanguage = "cmn-Hans-CN";
			string translatelanguage = "cmn-Hans-CN";
			if (QAVPTT.cache7 == null)
			{
				QAVPTT.cache7 = new QAVStreamingRecognitionCallback(QAVPTT.s_OnStreamingRecognitionComplete);
			}
			return QAVNative.QAVSDK_PTT_StartRecordingWithStreamingRecognition(filePath, speechLanguage, translatelanguage, QAVPTT.cache7);
		}

		public override int StartRecordingWithStreamingRecognition(string filePath, string speechLanguage)
		{
			if (QAVPTT.cache8 == null)
			{
				QAVPTT.cache8 = new QAVStreamingRecognitionCallback(QAVPTT.s_OnStreamingRecognitionComplete);
			}
			return QAVNative.QAVSDK_PTT_StartRecordingWithStreamingRecognition(filePath, speechLanguage, speechLanguage, QAVPTT.cache8);
		}

		public override int StartRecordingWithStreamingRecognition(string filePath, string speechLanguage, string translatelanguage)
		{
			if (QAVPTT.cache9 == null)
			{
				QAVPTT.cache9 = new QAVStreamingRecognitionCallback(QAVPTT.s_OnStreamingRecognitionComplete);
			}
			return QAVNative.QAVSDK_PTT_StartRecordingWithStreamingRecognition(filePath, speechLanguage, translatelanguage, QAVPTT.cache9);
		}

		public override int PauseRecording()
		{
			return QAVNative.QAVSDK_PTT_PauseRecording();
		}

		public override int ResumeRecording()
		{
			return QAVNative.QAVSDK_PTT_ResumeRecording();
		}

		public int SetAppInfo(string appID, string openID)
		{
			return QAVNative.QAVSDK_PTT_SetAppInfo(appID, openID);
		}

		[MonoPInvokeCallback(typeof(QAVRecordFileCompleteCallback))]
		private static void s_OnRecordFileComplete(int code, string filepath)
		{
			if (QAVPTT.GetInstance().OnRecordFileComplete != null)
			{
				QAVPTT.GetInstance().OnRecordFileComplete(code, filepath);
			}
		}

		[MonoPInvokeCallback(typeof(QAVUploadFileCompleteCallback))]
		private static void s_OnUploadFileComplete(int code, string filepath, string fileid)
		{
			if (QAVPTT.GetInstance().OnUploadFileComplete != null)
			{
				QAVPTT.GetInstance().OnUploadFileComplete(code, filepath, fileid);
			}
		}

		[MonoPInvokeCallback(typeof(QAVDownloadFileCompleteCallback))]
		private static void s_OnDownloadFileComplete(int code, string filepath, string fileid)
		{
			if (QAVPTT.GetInstance().OnDownloadFileComplete != null)
			{
				QAVPTT.GetInstance().OnDownloadFileComplete(code, filepath, fileid);
			}
		}

		[MonoPInvokeCallback(typeof(QAVPlayFileCompleteCallback))]
		private static void s_OnPlayFileComplete(int code, string filepath)
		{
			if (QAVPTT.GetInstance().OnPlayFileComplete != null)
			{
				QAVPTT.GetInstance().OnPlayFileComplete(code, filepath);
			}
		}

		[MonoPInvokeCallback(typeof(QAVSpeechToTextCallback))]
		private static void s_OnSpeechToTextComplete(int code, string fileid, string result)
		{
			if (QAVPTT.GetInstance().OnSpeechToTextComplete != null)
			{
				QAVPTT.GetInstance().OnSpeechToTextComplete(code, fileid, result);
			}
		}

		[MonoPInvokeCallback(typeof(QAVStreamingRecognitionCallback))]
		private static void s_OnStreamingRecognitionComplete(int code, string filepath, string fileid, string result)
		{
			if (QAVPTT.GetInstance().OnStreamingSpeechComplete != null)
			{
				QAVPTT.GetInstance().OnStreamingSpeechComplete(code, fileid, filepath, result);
			}
		}

		private static QAVPTT sInstance;

		private static readonly object sLock = new object();

		[CompilerGenerated]
		private static QAVRecordFileCompleteCallback cache0;

		[CompilerGenerated]
		private static QAVUploadFileCompleteCallback cache1;

		[CompilerGenerated]
		private static QAVDownloadFileCompleteCallback cache2;

		[CompilerGenerated]
		private static QAVPlayFileCompleteCallback cache3;

		[CompilerGenerated]
		private static QAVSpeechToTextCallback cache4;

		[CompilerGenerated]
		private static QAVSpeechToTextCallback cache5;

		[CompilerGenerated]
		private static QAVSpeechToTextCallback cache6;

		[CompilerGenerated]
		private static QAVStreamingRecognitionCallback cache7;

		[CompilerGenerated]
		private static QAVStreamingRecognitionCallback cache8;

		[CompilerGenerated]
		private static QAVStreamingRecognitionCallback cache9;
	}
}
