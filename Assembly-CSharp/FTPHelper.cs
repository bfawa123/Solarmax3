using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class FTPHelper
{
	public FTPHelper(string ftpServerIP, string ftpRemotePath, string ftpUserID, string ftpPassword)
	{
		this.ftpServerIP = ftpServerIP;
		this.ftpRemotePath = ftpRemotePath;
		this.ftpUserID = ftpUserID;
		this.ftpPassword = ftpPassword;
		this.ftpURI = string.Concat(new string[]
		{
			"ftp://",
			ftpServerIP,
			"/",
			ftpRemotePath,
			"/"
		});
	}

	public string ftpURI { get; private set; }

	public string ftpServerIP { get; private set; }

	public string ftpRemotePath { get; private set; }

	public string ftpUserID { get; private set; }

	public string ftpPassword { get; private set; }

	~FTPHelper()
	{
		if (this.response != null)
		{
			this.response.Close();
			this.response = null;
		}
		if (this.request != null)
		{
			this.request.Abort();
			this.request = null;
		}
	}

	private FtpWebResponse Open(Uri uri, string ftpMethod)
	{
		this.request = (FtpWebRequest)WebRequest.Create(uri);
		this.request.Method = ftpMethod;
		this.request.UseBinary = true;
		this.request.KeepAlive = false;
		this.request.UsePassive = true;
		if (!string.IsNullOrEmpty(this.ftpUserID))
		{
			this.request.Credentials = new NetworkCredential(this.ftpUserID, this.ftpPassword);
		}
		else
		{
			this.request.Credentials = new NetworkCredential("anonymous", string.Empty);
		}
		WebResponse webResponse = this.request.GetResponse();
		return (FtpWebResponse)webResponse;
	}

	private FtpWebRequest OpenRequest(Uri uri, string ftpMethod)
	{
		this.request = (FtpWebRequest)WebRequest.Create(uri);
		this.request.Method = ftpMethod;
		this.request.UseBinary = true;
		this.request.KeepAlive = false;
		this.request.UsePassive = true;
		if (!string.IsNullOrEmpty(this.ftpUserID))
		{
			this.request.Credentials = new NetworkCredential(this.ftpUserID, this.ftpPassword);
		}
		else
		{
			this.request.Credentials = new NetworkCredential("anonymous", "?");
		}
		return this.request;
	}

	public void CreateDirectory(string remoteDirectoryName)
	{
		this.response = this.Open(new Uri(this.ftpURI + remoteDirectoryName), "MKD");
	}

	public void ReName(string currentName, string newName)
	{
		this.request = this.OpenRequest(new Uri(this.ftpURI + currentName), "RENAME");
		this.request.RenameTo = newName;
		this.response = (FtpWebResponse)this.request.GetResponse();
	}

	public void GotoDirectory(string DirectoryName, bool IsRoot)
	{
		if (IsRoot)
		{
			this.ftpRemotePath = DirectoryName;
		}
		else
		{
			this.ftpRemotePath = this.ftpRemotePath + "/" + DirectoryName;
		}
		this.ftpURI = string.Concat(new string[]
		{
			"ftp://",
			this.ftpServerIP,
			"/",
			this.ftpRemotePath,
			"/"
		});
	}

	public void RemoveDirectory(string remoteDirectoryName)
	{
		this.GotoDirectory(remoteDirectoryName, true);
		List<FileStruct> list = this.ListFilesAndDirectories();
		foreach (FileStruct fileStruct in list)
		{
			if (fileStruct.IsDirectory)
			{
				this.RemoveDirectory(fileStruct.Path);
			}
			else
			{
				this.DeleteFile(fileStruct.Name);
			}
		}
		this.GotoDirectory(remoteDirectoryName, true);
		this.response = this.Open(new Uri(this.ftpURI), "RMD");
	}

	public void Upload(string localFilePath)
	{
		FileInfo fileInfo = new FileInfo(localFilePath);
		this.request = this.OpenRequest(new Uri(this.ftpURI + fileInfo.Name), "STOR");
		this.request.ContentLength = fileInfo.Length;
		int num = 2048;
		byte[] buffer = new byte[num];
		using (FileStream fileStream = fileInfo.OpenRead())
		{
			using (Stream requestStream = this.request.GetRequestStream())
			{
				for (int count = fileStream.Read(buffer, 0, num); count != 0; count = fileStream.Read(buffer, 0, num))
				{
					requestStream.Write(buffer, 0, count);
				}
			}
		}
	}

	public void DeleteFile(string remoteFileName)
	{
		this.response = this.Open(new Uri(this.ftpURI + remoteFileName), "DELE");
	}

	public List<FileStruct> ListFilesAndDirectories()
	{
		List<FileStruct> result = new List<FileStruct>();
		this.response = this.Open(new Uri(this.ftpURI), "LIST");
		using (Stream responseStream = this.response.GetResponseStream())
		{
			using (StreamReader streamReader = new StreamReader(responseStream))
			{
				string message;
				while ((message = streamReader.ReadLine()) != null)
				{
					Debug.Log(message);
				}
			}
		}
		return result;
	}

	public List<FileStruct> ListFiles()
	{
		List<FileStruct> source = this.ListFilesAndDirectories();
		return (from m in source
		where !m.IsDirectory
		select m).ToList<FileStruct>();
	}

	public List<FileStruct> ListDirectories()
	{
		List<FileStruct> source = this.ListFilesAndDirectories();
		return (from m in source
		where m.IsDirectory
		select m).ToList<FileStruct>();
	}

	public bool IsExist(string remoteName)
	{
		List<FileStruct> source = this.ListFilesAndDirectories();
		return source.Count((FileStruct m) => m.Name == remoteName) > 0;
	}

	public bool IsDirectoryExist(string remoteDirectoryName)
	{
		List<FileStruct> source = this.ListDirectories();
		return source.Count((FileStruct m) => m.Name == remoteDirectoryName) > 0;
	}

	public bool IsFileExist(string remoteFileName)
	{
		List<FileStruct> source = this.ListFiles();
		return source.Count((FileStruct m) => m.Name == remoteFileName) > 0;
	}

	public void Download(string saveFilePath, string downloadFileName)
	{
		if (!saveFilePath.EndsWith("/"))
		{
			saveFilePath += "/";
		}
		saveFilePath += downloadFileName;
		using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Create))
		{
			this.response = this.Open(new Uri(this.ftpURI + downloadFileName), "RETR");
			using (Stream responseStream = this.response.GetResponseStream())
			{
				long contentLength = this.response.ContentLength;
				int num = 2048;
				byte[] buffer = new byte[num];
				for (int i = responseStream.Read(buffer, 0, num); i > 0; i = responseStream.Read(buffer, 0, num))
				{
					fileStream.Write(buffer, 0, i);
				}
			}
		}
	}

	private FtpWebRequest request;

	private FtpWebResponse response;
}
