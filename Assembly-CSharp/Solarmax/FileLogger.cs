using System;
using System.Collections.Generic;
using System.IO;

namespace Solarmax
{
	public class FileLogger : Logger
	{
		public FileLogger()
		{
			this.mSavePath = string.Empty;
			this.mSaveFrontName = "Log";
			this.mSaveExtName = "log";
			this.mFinalFilePath = string.Empty;
			this.mFileSaver = new FileSaver();
			this.mWaitMessages = new List<string>();
			this.mTempSeconds = 0f;
		}

		public override bool Init()
		{
			this.mFileSaver.Init(this.GetFinalFilePath());
			return true;
		}

		public override void Tick(float interval)
		{
			this.mTempSeconds += interval;
			if (this.mTempSeconds >= 10f)
			{
				this.mTempSeconds = 0f;
				this.DirectWriteAll();
			}
		}

		public override void Destroy()
		{
			this.DirectWriteAll();
			this.mFileSaver.Close();
			this.mFileSaver = null;
		}

		public override void Write(string message)
		{
			this.mWaitMessages.Add(message);
		}

		private void DirectWriteAll()
		{
			int i = 0;
			int count = this.mWaitMessages.Count;
			while (i < count)
			{
				string text = this.mWaitMessages[i];
				this.mFileSaver.WriteLine(text);
				this.mWaitMessages.Remove(text);
				i++;
			}
			this.mFileSaver.Flush();
		}

		public void SetSavePath(string path)
		{
			this.mSavePath = path;
			this.FormatFinalFileName();
		}

		public string GetFinalFilePath()
		{
			return this.mFinalFilePath;
		}

		public void SetFileLogFrontName(string name)
		{
			this.mSaveFrontName = name;
		}

		public void SetFileLogExtName(string name)
		{
			this.mSaveExtName = name;
		}

		private void FormatFinalFileName()
		{
			string text = string.Format("{0}/GameLog", this.mSavePath);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			this.mFinalFilePath = string.Format("{0}/{1}_{2}.{3}", new object[]
			{
				text,
				this.mSaveFrontName,
				DateTime.Now.ToString("yyyy-MM-dd"),
				this.mSaveExtName
			});
		}

		private const string _LogPath = "{0}/GameLog";

		private const string _LogFormat = "{0}/{1}_{2}.{3}";

		private const int _FlusInterval = 10;

		private string mSavePath;

		private string mSaveFrontName;

		private string mSaveExtName;

		private string mFinalFilePath;

		private FileSaver mFileSaver;

		private List<string> mWaitMessages;

		private float mTempSeconds;
	}
}
