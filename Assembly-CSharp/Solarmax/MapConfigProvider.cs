using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Solarmax
{
	public class MapConfigProvider : Solarmax.Singleton<MapConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/maplist.xml";
		}

		public bool Delete(string name)
		{
			bool result = false;
			if (this.mapDictionary.ContainsKey(name))
			{
				this.GetData(name).Delete();
				this.mapDictionary.Remove(name);
				result = true;
				if (this.mapVersion.ContainsKey(name))
				{
					this.mapVersion.Remove(name);
				}
				this.WriteMapList(false);
			}
			return result;
		}

		public bool WriteMapList(bool bSync = false)
		{
			bool result = false;
			try
			{
				string filename = string.Empty;
				filename = Application.dataPath + "/Res/" + this.Path();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(filename, new UTF8Encoding(false));
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartDocument();
				xmlTextWriter.WriteStartElement("maps");
				foreach (MapListConfig mapListConfig in this.mapVersion.Values)
				{
					xmlTextWriter.WriteStartElement("map");
					xmlTextWriter.WriteAttributeString("name", mapListConfig.mapID);
					if (bSync)
					{
						mapListConfig.nAdd = 0;
						xmlTextWriter.WriteAttributeString("version", (mapListConfig.version + mapListConfig.nAdd).ToString());
					}
					else
					{
						xmlTextWriter.WriteAttributeString("version", mapListConfig.version.ToString());
					}
					xmlTextWriter.WriteAttributeString("add", mapListConfig.nAdd.ToString());
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
				result = true;
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/MapList.xml save failed " + ex.ToString(), new object[0]);
			}
			return result;
		}

		public bool Save(MapConfig mc)
		{
			bool result;
			if (this.mapDictionary.ContainsKey(mc.name))
			{
				mc.Save();
				result = true;
			}
			else
			{
				this.mapDictionary.Add(mc.name, mc);
				MapListConfig mapListConfig = new MapListConfig();
				mapListConfig.mapID = mc.name;
				mapListConfig.version = 0;
				mapListConfig.nAdd = 0;
				this.mapVersion.Add(mapListConfig.mapID, mapListConfig);
				mc.Save();
				result = true;
			}
			this.WriteMapList(false);
			return result;
		}

		public void SavaAll()
		{
			foreach (KeyValuePair<string, MapConfig> keyValuePair in this.mapDictionary)
			{
				this.Save(keyValuePair.Value);
			}
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			this.mapVersion.Clear();
			this.mapDictionary.Clear();
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XElement xelement = XDocument.Parse(text).Element("maps");
					if (xelement != null)
					{
						foreach (XElement element in xelement.Elements("map"))
						{
							MapListConfig mapListConfig = new MapListConfig();
							if (mapListConfig.Load(element))
							{
								MapConfig mapConfig = new MapConfig(mapListConfig.mapID);
								mapConfig.Load();
								this.mapDictionary.Add(mapListConfig.mapID, mapConfig);
								this.mapVersion.Add(mapListConfig.mapID, mapListConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("maplist resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public Dictionary<string, MapConfig> GetAllData()
		{
			return this.mapDictionary;
		}

		public MapConfig GetData(string id)
		{
			MapConfig result = null;
			if (this.mapDictionary.ContainsKey(id))
			{
				result = this.mapDictionary[id];
			}
			return result;
		}

		public MapListConfig GetMapVersion(string Id)
		{
			MapListConfig result = null;
			if (this.mapVersion.ContainsKey(Id))
			{
				return this.mapVersion[Id];
			}
			return result;
		}

		public bool ModifymapVersion(string mapId)
		{
			MapListConfig mapListConfig = this.GetMapVersion(mapId);
			if (mapListConfig != null)
			{
				mapListConfig.nAdd++;
				return true;
			}
			return true;
		}

		public bool SyncmapVersion(string mapId)
		{
			MapListConfig mapListConfig = this.GetMapVersion(mapId);
			if (mapListConfig != null)
			{
				mapListConfig.version += mapListConfig.nAdd;
				mapListConfig.nAdd = 0;
				return true;
			}
			return false;
		}

		public void LoadExtraData()
		{
			Dictionary<string, MapListConfig> dictionary = this.LoadExtraMapList();
			if (dictionary != null && dictionary.Count > 0)
			{
				this.LoadExtraMap(dictionary);
			}
			dictionary.Clear();
		}

		public Dictionary<string, MapListConfig> LoadExtraMapList()
		{
			Dictionary<string, MapListConfig> dictionary = new Dictionary<string, MapListConfig>();
			try
			{
				string path = string.Empty;
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					path = Application.dataPath + "/cache/EditMap/MapList.xml";
				}
				else if (Application.platform == RuntimePlatform.Android)
				{
					path = Application.persistentDataPath + "/EditMap/MapList.xml";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					path = "file://" + Application.persistentDataPath + "/EditMap/MapList.xml";
				}
				else if (Application.platform == RuntimePlatform.WindowsPlayer)
				{
					path = Application.dataPath + "/cache/EditMap/MapList.xml";
				}
				StreamReader streamReader = File.OpenText(path);
				string text = streamReader.ReadToEnd();
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					XElement xelement = xdocument.Element("maps");
					if (xelement == null)
					{
						return null;
					}
					IEnumerable<XElement> enumerable = xelement.Elements("map");
					foreach (XElement element in enumerable)
					{
						MapListConfig mapListConfig = new MapListConfig();
						if (mapListConfig.Load(element))
						{
							MapListConfig mapListConfig2 = null;
							this.mapVersion.TryGetValue(mapListConfig.mapID, out mapListConfig2);
							if (mapListConfig2 != null)
							{
								if (mapListConfig.version > mapListConfig2.version)
								{
									mapListConfig2.version = mapListConfig.version;
									dictionary.Add(mapListConfig.mapID, mapListConfig);
								}
							}
							else
							{
								this.mapVersion.Add(mapListConfig.mapID, mapListConfig);
								dictionary.Add(mapListConfig.mapID, mapListConfig);
							}
						}
					}
				}
				streamReader.Close();
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("maplist resource failed " + ex.ToString(), new object[0]);
			}
			return dictionary;
		}

		public void LoadExtraMap(Dictionary<string, MapListConfig> maps)
		{
			string str = string.Empty;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				str = Application.dataPath + "/cache/EditMap/data/";
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				str = Application.persistentDataPath + "/EditMap/data/";
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				str = "file://" + Application.persistentDataPath + "/EditMap/data/";
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				str = Application.dataPath + "/cache/EditMap/data/";
			}
			foreach (KeyValuePair<string, MapListConfig> keyValuePair in maps)
			{
				try
				{
					string filePath = str + keyValuePair.Value.mapID + ".xml";
					MapConfig mapConfig = new MapConfig(keyValuePair.Value.mapID);
					mapConfig.LoadExter(filePath);
					this.mapDictionary.Add(mapConfig.id, mapConfig);
				}
				catch (Exception ex)
				{
                    Solarmax.Singleton<LoggerSystem>.Instance.Error("data/MapList.xml save failed " + ex.ToString(), new object[0]);
				}
			}
		}

		public void DownLoadMap(string filePath, string mapId)
		{
			bool flag = false;
			MapConfig mapConfig = this.GetData(mapId);
			if (mapConfig == null)
			{
				mapConfig = new MapConfig(mapId);
				mapConfig.LoadExter(filePath);
				mapConfig.id = mapId;
				flag = true;
			}
			else
			{
				mapConfig.LoadExter(filePath);
			}
			if (flag)
			{
				this.mapDictionary.Add(mapConfig.id, mapConfig);
			}
		}

		public Stack<string> DownLoadMapList(string filePath)
		{
			try
			{
				Stack<string> stack = new Stack<string>();
				StreamReader streamReader = File.OpenText(filePath);
				string text = streamReader.ReadToEnd();
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					XElement xelement = xdocument.Element("maps");
					if (xelement == null)
					{
						return null;
					}
					IEnumerable<XElement> enumerable = xelement.Elements("map");
					foreach (XElement element in enumerable)
					{
						MapListConfig mapListConfig = new MapListConfig();
						if (mapListConfig.Load(element))
						{
							MapListConfig mapListConfig2 = null;
							this.mapVersion.TryGetValue(mapListConfig.mapID, out mapListConfig2);
							if (mapListConfig2 == null)
							{
								stack.Push(mapListConfig.mapID);
							}
							else if (mapListConfig.version > mapListConfig2.version)
							{
								stack.Push(mapListConfig.mapID);
							}
						}
					}
				}
				streamReader.Close();
				return stack;
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("maplist resource failed " + ex.ToString(), new object[0]);
			}
			return null;
		}

		public Dictionary<string, MapConfig> GetAllDataExtra()
		{
			return this.mapDictionary;
		}

		public Dictionary<string, MapListConfig> mapVersion = new Dictionary<string, MapListConfig>();

		public Dictionary<string, MapConfig> mapDictionary = new Dictionary<string, MapConfig>();
	}
}
