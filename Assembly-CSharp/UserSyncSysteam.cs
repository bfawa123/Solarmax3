using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using NetMessage;
using Solarmax;
using UnityEngine;

public class UserSyncSysteam : Solarmax.Singleton<UserSyncSysteam>, IDataHandler, Lifecycle
{
	private string FileName
	{
		get
		{
			return Solarmax.Singleton<LocalAccountStorage>.Get().account;
		}
	}

	public void StartThread()
	{
		if (this.workThread != null)
		{
			this.workThread.Stop();
		}
		this.workThread = new AsyncThread(new Callback<AsyncThread>(this.AsyncLogic));
		this.workThread.Start();
	}

	public bool Init()
	{
		this.workThread = new AsyncThread(new Callback<AsyncThread>(this.AsyncLogic));
		this.workThread.Start();
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnGenPresignedUrlRep, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnSyncUserAndChapters, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnGenPresignedUrlMaplist, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnGenVideoUrl, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		return true;
	}

	public void Tick(float interval)
	{
		if (this.bStarDownLoad)
		{
			this.ProcessDownLoadOne();
			if (UserSyncSysteam.needLoadMap.Count <= 0)
			{
				this.bStarDownLoad = false;
			}
		}
	}

	public void Destroy()
	{
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnGenPresignedUrlRep, this);
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnSyncUserAndChapters, this);
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnGenPresignedUrlMaplist, this);
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnGenVideoUrl, this);
	}

	public void DestroyThread()
	{
		if (this.workThread != null)
		{
			this.workThread.Stop();
		}
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
		if (eventId == 113)
		{
			SCGenerateUrl scgenerateUrl = args[0] as SCGenerateUrl;
			if (scgenerateUrl.method == "PUT")
			{
				UserSyncSysteam.UploadAvatar(scgenerateUrl.url, scgenerateUrl.file);
			}
			if (scgenerateUrl.method == "GET")
			{
				UserSyncSysteam.DownloadAvatar(scgenerateUrl.url, scgenerateUrl.file);
			}
		}
		else if (eventId == 114)
		{
			SCGenerateUrl scgenerateUrl2 = args[0] as SCGenerateUrl;
			this.DownloadMapList(scgenerateUrl2.url, scgenerateUrl2.file);
		}
		else if (eventId == 117)
		{
			SCGenerateUrl scgenerateUrl3 = args[0] as SCGenerateUrl;
			this.AddOperatorEvent(0, scgenerateUrl3.method, scgenerateUrl3.url, scgenerateUrl3.file);
		}
		else if (eventId == 131)
		{
			SCGenerateUrl scgenerateUrl4 = args[0] as SCGenerateUrl;
			this.AddOperatorEvent(1, scgenerateUrl4.method, scgenerateUrl4.url, scgenerateUrl4.file);
		}
	}

	public static void DownloadAvatar(string url, string filePath)
	{
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentType = "text/plain";
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			byte[] array = new byte[4096];
			int i = responseStream.Read(array, 0, array.Length);
			FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			while (i > 0)
			{
				fileStream.Write(array, 0, i);
				i = responseStream.Read(array, 0, array.Length);
			}
			fileStream.Close();
			responseStream.Close();
			Solarmax.Singleton<MapConfigProvider>.Get().DownLoadMap(filePath, Solarmax.Singleton<UserSyncSysteam>.Get().curDownLoadMap);
			if (UserSyncSysteam.needLoadMap.Count == 0 && Solarmax.Singleton<UserSyncSysteam>.Get().bDownLoadWorking)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomTestLevelWindow");
			}
			Solarmax.Singleton<UserSyncSysteam>.Get().bDownLoadWorking = false;
		}
		catch (Exception ex)
		{
			Debug.Log("DownloadAvatar exception:  " + ex.ToString());
			if (UserSyncSysteam.needLoadMap.Count == 0 && Solarmax.Singleton<UserSyncSysteam>.Get().bDownLoadWorking)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomTestLevelWindow");
			}
			Solarmax.Singleton<UserSyncSysteam>.Get().bDownLoadWorking = false;
		}
	}

	public static void UploadAvatar(string url, string filePath)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
		httpWebRequest.Method = "PUT";
		httpWebRequest.ContentType = "text/plain";
		FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		long length = fileStream.Length;
		httpWebRequest.ContentLength = length;
		try
		{
			int num = 4096;
			byte[] buffer = new byte[num];
			long num2 = 0L;
			int i = binaryReader.Read(buffer, 0, num);
			Stream requestStream = httpWebRequest.GetRequestStream();
			while (i > 0)
			{
				requestStream.Write(buffer, 0, i);
				num2 += (long)i;
				i = binaryReader.Read(buffer, 0, num);
			}
			requestStream.Close();
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				Stream responseStream = httpWebResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
				string text = streamReader.ReadToEnd();
				httpWebResponse.Close();
				streamReader.Close();
				if (MapEdit.Instance != null)
				{
					MapEdit.Instance.OnCompleteUpLoad();
				}
			}
		}
		catch (Exception arg)
		{
			Debug.Log("UploadAvatar : " + arg);
		}
		finally
		{
			fileStream.Close();
			binaryReader.Close();
		}
	}

	public static void UploadLevelRankVideo(string url, string filePath)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("上传录像url：{0}", url), new object[0]);
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("上传录像文件路径：{0}", filePath), new object[0]);
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
		httpWebRequest.Method = "PUT";
		httpWebRequest.ContentType = "text/plain";
		FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		long length = fileStream.Length;
		httpWebRequest.ContentLength = length;
		try
		{
			int num = 4096;
			byte[] buffer = new byte[num];
			long num2 = 0L;
			int i = binaryReader.Read(buffer, 0, num);
			Stream requestStream = httpWebRequest.GetRequestStream();
			while (i > 0)
			{
				requestStream.Write(buffer, 0, i);
				num2 += (long)i;
				i = binaryReader.Read(buffer, 0, num);
			}
			requestStream.Close();
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				Stream responseStream = httpWebResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
				string text = streamReader.ReadToEnd();
				httpWebResponse.Close();
				streamReader.Close();
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					Solarmax.Singleton<LoggerSystem>.Get().Info("---upload pve record success----", new object[0]);
				}
				else
				{
					Solarmax.Singleton<LoggerSystem>.Get().Info("---upload pve record failed----", new object[0]);
				}
			}
		}
		catch (Exception arg)
		{
			Debug.Log("UploadAvatar : " + arg);
		}
		finally
		{
			fileStream.Close();
			binaryReader.Close();
		}
	}

	public static void DownloadLevelRankVideo(string url, string filePath)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("下载录像url：{0}", url), new object[0]);
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("下载录像文件路径：{0}", filePath), new object[0]);
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentType = "text/plain";
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				Solarmax.Singleton<LoggerSystem>.Get().Info("---download pve record success----", new object[0]);
				byte[] array = new byte[4096];
				int i = responseStream.Read(array, 0, array.Length);
				FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				while (i > 0)
				{
					fileStream.Write(array, 0, i);
					i = responseStream.Read(array, 0, array.Length);
				}
				fileStream.Close();
				responseStream.Close();
				Solarmax.Singleton<UserSyncSysteam>.Get().bDownLoadWorking = false;
				Solarmax.Singleton<EventSystem>.Instance.FireEventCache(132, new object[]
				{
					filePath
				});
			}
			else
			{
				Solarmax.Singleton<LoggerSystem>.Get().Info("---download pve record failed----", new object[0]);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("DownloadAvatar exception:  " + ex.ToString());
			if (UserSyncSysteam.needLoadMap.Count == 0 && Solarmax.Singleton<UserSyncSysteam>.Get().bDownLoadWorking)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomTestLevelWindow");
			}
			Solarmax.Singleton<UserSyncSysteam>.Get().bDownLoadWorking = false;
		}
	}

	private void DownLoadUserAndChapters(string url, string filePath)
	{
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentType = "text/plain";
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			byte[] array = new byte[4096];
			int i = responseStream.Read(array, 0, array.Length);
			FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			while (i > 0)
			{
				fileStream.Write(array, 0, i);
				i = responseStream.Read(array, 0, array.Length);
			}
			fileStream.Close();
			responseStream.Close();
			this.SaveFileToUserChapter(filePath);
			Solarmax.Singleton<EventSystem>.Instance.FireEventCache(118, new object[]
			{
				0,
				1
			});
		}
		catch (Exception ex)
		{
			Debug.Log("DownloadAvatar exception:  " + ex.ToString());
			Solarmax.Singleton<EventSystem>.Instance.FireEventCache(118, new object[]
			{
				0,
				0
			});
		}
	}

	private void UpLoadUserAndChapters(string url, string filePath)
	{
		this.UserChapterSaveFile(filePath);
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
		httpWebRequest.Method = "PUT";
		httpWebRequest.ContentType = "text/plain";
		FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		long length = fileStream.Length;
		httpWebRequest.ContentLength = length;
		try
		{
			int num = 4096;
			byte[] buffer = new byte[num];
			long num2 = 0L;
			int i = binaryReader.Read(buffer, 0, num);
			Stream requestStream = httpWebRequest.GetRequestStream();
			while (i > 0)
			{
				requestStream.Write(buffer, 0, i);
				num2 += (long)i;
				i = binaryReader.Read(buffer, 0, num);
			}
			requestStream.Close();
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				Stream responseStream = httpWebResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
				string text = streamReader.ReadToEnd();
				httpWebResponse.Close();
				streamReader.Close();
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEventCache(118, new object[]
			{
				1,
				1
			});
		}
		catch (Exception ex)
		{
			Debug.Log("DownloadAvatar exception:  " + ex.ToString());
			Solarmax.Singleton<EventSystem>.Instance.FireEventCache(118, new object[]
			{
				1,
				0
			});
		}
		finally
		{
			fileStream.Close();
			binaryReader.Close();
		}
	}

	private void UserChapterSaveFile(string filePath)
	{
	}

	private void SaveFileToUserChapter(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return;
		}
		using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
		{
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			this.curLoadStruct = Json.DeCode<SyncMessageStruct>(array);
		}
	}

	private void SaveFile(string path, byte[] bytes)
	{
		int length = path.LastIndexOf('/');
		string path2 = path.Substring(0, length);
		if (!Directory.Exists(path2))
		{
			Directory.CreateDirectory(path2);
		}
		FileStream fileStream = new FileStream(path, FileMode.Create);
		fileStream.Write(bytes, 0, bytes.Length);
		fileStream.Flush();
		fileStream.Close();
		fileStream.Dispose();
	}

	public void DownloadMapList(string url, string filePath)
	{
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentType = "text/plain";
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			byte[] array = new byte[4096];
			int i = responseStream.Read(array, 0, array.Length);
			FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			while (i > 0)
			{
				fileStream.Write(array, 0, i);
				i = responseStream.Read(array, 0, array.Length);
			}
			fileStream.Close();
			responseStream.Close();
			UserSyncSysteam.needLoadMap.Clear();
			UserSyncSysteam.needLoadMap = Solarmax.Singleton<MapConfigProvider>.Get().DownLoadMapList(filePath);
			if (UserSyncSysteam.needLoadMap.Count > 0)
			{
				this.bStarDownLoad = true;
			}
			if (UserSyncSysteam.needLoadMap.Count == 0)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomTestLevelWindow");
			}
		}
		catch (Exception arg)
		{
			Debug.Log("DownloadAvatar exception:  " + arg);
		}
	}

	private void ProcessDownLoadOne()
	{
		if (this.bDownLoadWorking)
		{
			return;
		}
		if (UserSyncSysteam.needLoadMap.Count <= 0)
		{
			return;
		}
		string text = MonoSingleton<UpdateSystem>.Instance.saveRoot + "EditMap/data/";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = UserSyncSysteam.needLoadMap.Pop();
		if (!string.IsNullOrEmpty(text2))
		{
			string file = string.Format("{0}{1}.xml", text, text2);
			Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl(text2 + ".xml", "GET", "text/plain", file, 113);
			this.curDownLoadMap = text2;
			this.bDownLoadWorking = true;
		}
	}

	public void GenPveUploadUrl(string userId, string levelId, string groupID, string score)
	{
		string str = string.Format("{0}_{1}.video", userId, levelId);
		string text = string.Format("{0}_{1}_{2}.video", userId, groupID, score);
		string text2 = Application.dataPath + "/video2/" + str;
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			text2 = Application.persistentDataPath + "/video2/" + str;
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			text2 = Application.persistentDataPath + "/video2/" + str;
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			text2 = Application.persistentDataPath + "/video2/" + str;
		}
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("录像文件路径：{0}", text2), new object[0]);
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("上传录像文件名称：{0}", text), new object[0]);
		Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl(text, "PUT", "text/plain", text2, 131);
	}

	public void GenPveDownloadUrl(string userId, string levelId, string score)
	{
		string text = string.Format("{0}_{1}_{2}.video", userId, levelId, score);
		string text2 = Application.dataPath + "/video2/" + text;
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			text2 = Application.persistentDataPath + "/video2/" + text;
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			text2 = Application.persistentDataPath + "/video2/" + text;
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			text2 = Application.persistentDataPath + "/video2/" + text;
		}
		if (File.Exists(text2))
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEventCache(132, new object[]
			{
				text2
			});
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl(text, "GET", "text/plain", text2, 131);
	}

	private void AsyncLogic(AsyncThread thread)
	{
		while (thread.IsWorking())
		{
			if (this.eventPool.Count == 0)
			{
				UserSyncSysteam.threadMutex.WaitOne();
			}
			OperatorEvent operatorEvent = null;
			object obj = this.eventPool;
			lock (obj)
			{
				if (this.eventPool.Count <= 0)
				{
					continue;
				}
				operatorEvent = this.eventPool.Pop();
			}
			if (operatorEvent != null && operatorEvent.type == 0)
			{
				if (operatorEvent.Method == "PUT")
				{
					this.UpLoadUserAndChapters(operatorEvent.Url, operatorEvent.fullPath);
				}
				if (operatorEvent.Method == "GET")
				{
					this.DownLoadUserAndChapters(operatorEvent.Url, operatorEvent.fullPath);
				}
			}
			if (operatorEvent != null && operatorEvent.type == 1)
			{
				if (operatorEvent.Method == "PUT")
				{
					UserSyncSysteam.UploadLevelRankVideo(operatorEvent.Url, operatorEvent.fullPath);
				}
				if (operatorEvent.Method == "GET")
				{
					UserSyncSysteam.DownloadLevelRankVideo(operatorEvent.Url, operatorEvent.fullPath);
				}
			}
		}
	}

	public void AddOperatorEvent(int type, string method, string url, string fullpath)
	{
		object obj = this.eventPool;
		lock (obj)
		{
			foreach (OperatorEvent operatorEvent in this.eventPool)
			{
				if (operatorEvent.type == type && type == 0)
				{
					return;
				}
			}
			OperatorEvent operatorEvent2 = new OperatorEvent();
			operatorEvent2.type = type;
			operatorEvent2.Method = method;
			operatorEvent2.Url = url;
			operatorEvent2.fullPath = fullpath;
			this.eventPool.Push(operatorEvent2);
		}
		UserSyncSysteam.threadMutex.Set();
	}

	public void SaveAccout2Cloud()
	{
		string text = Solarmax.Singleton<LocalAccountStorage>.Get().account + ".txt";
		string file = MonoSingleton<UpdateSystem>.Instance.saveRoot + text;
		Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl(text, "PUT", "text/plain", file, 117);
	}

	public const string UPLOAD_AVATAR_HTTP_CONTENT_TYPE = "text/plain";

	public const string DOWNLOAD_AVATAR_PATH = "/mnt/sdcard/";

	private string appVersion = "0.0.0.0";

	private bool bStarDownLoad;

	public bool bDownLoadWorking;

	public string curDownLoadMap = string.Empty;

	private Stack<OperatorEvent> eventPool = new Stack<OperatorEvent>();

	private AsyncThread workThread;

	public SyncMessageStruct curLoadStruct;

	private static AutoResetEvent threadMutex = new AutoResetEvent(false);

	private static Stack<string> needLoadMap = new Stack<string>();
}
