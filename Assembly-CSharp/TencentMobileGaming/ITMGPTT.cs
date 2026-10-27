using System;

namespace TencentMobileGaming
{
	public abstract class ITMGPTT
	{
		static ITMGPTT()
		{
			ITMGContext.GetInstance().GetPttCtrl();
		}

		public static ITMGPTT GetInstance()
		{
			return ITMGContext.GetInstance().GetPttCtrl();
		}

		public abstract event QAVRecordFileCompleteCallback OnRecordFileComplete;

		public abstract event QAVUploadFileCompleteCallback OnUploadFileComplete;

		public abstract event QAVDownloadFileCompleteCallback OnDownloadFileComplete;

		public abstract event QAVPlayFileCompleteCallback OnPlayFileComplete;

		public abstract event QAVSpeechToTextCallback OnSpeechToTextComplete;

		public abstract event QAVStreamingRecognitionCallback OnStreamingSpeechComplete;

		public abstract int ApplyPTTAuthbuffer(byte[] authBuffer);

		public abstract int SetMaxMessageLength(int msTime);

		public abstract int StartRecording(string filePath);

		public abstract int StopRecording();

		public abstract int CancelRecording();

		public abstract int UploadRecordedFile(string filePath);

		public abstract int DownloadRecordedFile(string fileID, string downloadFilePath);

		public abstract int PlayRecordedFile(string filePath);

		public abstract int StopPlayFile();

		public abstract int SpeechToText(string fileID);

		public abstract int SpeechToText(string fileID, string speechLanguage);

		public abstract int SpeechToText(string fileID, string speechLanguage, string translatelanguage);

		public abstract int GetFileSize(string filePath);

		public abstract int GetVoiceFileDuration(string filePath);

		public abstract int StartRecordingWithStreamingRecognition(string filePath);

		public abstract int StartRecordingWithStreamingRecognition(string filePath, string speechLanguage);

		public abstract int StartRecordingWithStreamingRecognition(string filePath, string speechLanguage, string translatelanguage);

		public abstract int GetMicLevel();

		public abstract int SetMicVolume(int volume);

		public abstract int GetMicVolume();

		public abstract int GetSpeakerLevel();

		public abstract int SetSpeakerVolume(int volume);

		public abstract int GetSpeakerVolume();

		public abstract int PauseRecording();

		public abstract int ResumeRecording();
	}
}
