using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solarmax
{
	public class ConfigSystem : Solarmax.Singleton<ConfigSystem>, Lifecycle
	{
		public ConfigSystem()
		{
			this.mConfigFilePath = "config/config.txt";
		}

		public bool Init()
		{
			this.mConfigs.Clear();
			FileReader.LoadPath(this.mConfigFilePath);
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				FileReader.ReadInt();
				string text = FileReader.ReadString();
				string value = FileReader.ReadString();
				this.mConfigs.Add(text.ToLowerInvariant(), value);
			}
			FileReader.UnLoad();
			return true;
		}

		public void Tick(float interval)
		{
		}

		public void Destroy()
		{
		}

		public bool TryGetConfig(string key, out string v)
		{
			if (!this.mConfigs.TryGetValue(key.ToLowerInvariant(), out v))
			{
				Debug.LogErrorFormat("Get config {0} failed!", new object[]
				{
					key
				});
				return false;
			}
			return true;
		}

		public string GetKeyByValue(string v)
		{
			foreach (KeyValuePair<string, string> keyValuePair in this.mConfigs)
			{
				if (keyValuePair.Value == v)
				{
					return keyValuePair.Key;
				}
			}
			return null;
		}

		private string mConfigFilePath = string.Empty;

		private Dictionary<string, string> mConfigs = new Dictionary<string, string>();
	}
}
