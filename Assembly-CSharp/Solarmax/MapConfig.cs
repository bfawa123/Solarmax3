﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using GameCore.Loader;
using UnityEngine;

namespace Solarmax
{
	public class MapConfig
	{
		public MapConfig(string p_name) : base()
		{
			this.mlcList = new List<MapLineConfig>();
			this.mbcList = new List<MapBuildingConfig>();
			this.mpcList = new List<MapPlayerConfig>();
			this.storySentences = new List<string>();
			this.defaultai = "-1";
			this.teamAiType = new List<int>();
			//base..ctor();
			this.name = p_name;
		}

		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/mapconfig/";
		}

		public void Load()
		{
			this.mlcList.Clear();
			this.mbcList.Clear();
			this.mpcList.Clear();
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path() + this.name + ".xml");
				if (!string.IsNullOrEmpty(text))
				{
					XElement xelement = XDocument.Parse(text).Element("map");
					if (xelement != null)
					{
						this.id = xelement.GetAttribute("id", string.Empty);
						this.player_count = Convert.ToInt32(xelement.GetAttribute("player_count", "0"));
						this.vertical = Convert.ToBoolean(xelement.GetAttribute("vertical", "False"));
						this.moveCamera = Convert.ToBoolean(xelement.GetAttribute("SetMoveCamera", "False"));
						this.pvp = Convert.ToBoolean(xelement.GetAttribute("pvp", "False"));
						this.coopTimeLimitMin = Convert.ToInt32(xelement.GetAttribute("CoopTimeLimitMin", "0"));
						this.coopAccelerateEnable = Convert.ToInt32(xelement.GetAttribute("CoopAccelerateEnable", "1"));
						this.coopAccelerateTimeMin = Convert.ToInt32(xelement.GetAttribute("CoopAccelerateTimeMin", "5"));
						this.allAlive = Convert.ToInt32(xelement.GetAttribute("Allalive", "0"));
						this.enableAddTowerChange = Convert.ToInt32(xelement.GetAttribute("EnableAddTowerChange", "0"));
						this.pvpTimeLimitMin = Convert.ToInt32(xelement.GetAttribute("PvpTimeLimitMin", "0"));
						this.pvpAccelerateEnable = Convert.ToInt32(xelement.GetAttribute("PvpAccelerateEnable", "-1"));
						this.pvpAccelerateTimeMin = Convert.ToInt32(xelement.GetAttribute("PvpAccelerateTimeMin", "-1"));
						this.survivalTimeSec = 0f;
						string text4 = xelement.GetAttribute("surival", string.Empty);
						if (string.IsNullOrEmpty(text4))
						{
							text4 = xelement.GetAttribute("survival", string.Empty);
						}
						if (!string.IsNullOrEmpty(text4))
						{
							int num13 = text4.IndexOf('(');
							if (num13 >= 0)
							{
								int num14 = text4.IndexOf(')', num13 + 1);
								if (num14 > num13)
								{
									text4 = text4.Substring(num13 + 1, num14 - num13 - 1);
								}
							}
							float num15;
							if (float.TryParse(text4.Trim(), out num15) && num15 > 0f)
							{
								this.survivalTimeSec = num15;
							}
						}
						this.mysteryEnabled = false;
						this.mysteryCamp = 0;
						this.mysteryDuration = 0f;
						this.mysteryStartTime = 0f;
						string text3 = xelement.GetAttribute("Mystery", string.Empty);
						if (string.IsNullOrEmpty(text3))
						{
							text3 = xelement.GetAttribute("invisibleArgs", string.Empty);
						}
						if (!string.IsNullOrEmpty(text3))
						{
							string[] array2 = text3.Split(new char[]
							{
								','
							});
							if (array2.Length >= 3)
							{
								int num10 = Convert.ToInt32(array2[0].Trim());
								float num11 = Convert.ToSingle(array2[1].Trim());
								float num12 = Convert.ToSingle(array2[2].Trim());
								if (num11 < 0f)
								{
									num11 = 0f;
								}
								if (num12 < 0f)
								{
									num12 = 0f;
								}
								if (num11 > 0f && num10 >= 0 && num10 < LocalPlayer.MaxTeamNum)
								{
									this.mysteryEnabled = true;
									this.mysteryCamp = num10;
									this.mysteryDuration = num11;
									this.mysteryStartTime = num12;
								}
							}
						}
						this.ifBlackStrike = Convert.ToInt32(xelement.GetAttribute("IfBlackStrike", "0"));
						this.blackStrikeThreshold = Convert.ToInt32(xelement.GetAttribute("IfBlackStrikeThreshold", "220"));
						this.assimilationEnabled = false;
						this.assimilationX = 0f;
						this.assimilationY = 0f;
						this.assimilationCamp = 0;
						this.assimilationRadius = 0f;
						this.assimilationTriggerTime = 0f;
						this.assimilationX2 = 0f;
						this.assimilationY2 = 0f;
						this.assimilationRepeatCount = 1;
						this.assimilationHasSecondPoint = false;
						string text2 = xelement.GetAttribute("Assimilation", string.Empty);
						if (!string.IsNullOrEmpty(text2))
						{
							string[] array = text2.Split(new char[]
							{
								','
							});
							if (array.Length >= 6)
							{
								int num = Convert.ToInt32(array[0].Trim());
								float num2 = Convert.ToSingle(array[1].Trim());
								float num3 = Convert.ToSingle(array[2].Trim());
								int num4 = Convert.ToInt32(array[3].Trim());
								float num5 = Convert.ToSingle(array[4].Trim());
								float num6 = Convert.ToSingle(array[5].Trim());
								float num7 = 0f;
								float num8 = 0f;
								int num9 = 1;
								if (array.Length >= 9)
								{
									num7 = Convert.ToSingle(array[6].Trim());
									num8 = Convert.ToSingle(array[7].Trim());
									num9 = Convert.ToInt32(array[8].Trim());
								}
								if (num6 < 0f)
								{
									num6 = 0f;
								}
								if (num9 < 1)
								{
									num9 = 1;
								}
								if (num != 0 && num5 > 0f && num4 >= 0 && num4 < LocalPlayer.MaxTeamNum)
								{
									this.assimilationEnabled = true;
									this.assimilationX = num2;
									this.assimilationY = num3;
									this.assimilationCamp = num4;
									this.assimilationRadius = num5;
									this.assimilationTriggerTime = num6;
									this.assimilationX2 = num7;
									this.assimilationY2 = num8;
									this.assimilationRepeatCount = num9;
									this.assimilationHasSecondPoint = (array.Length >= 9);
								}
							}
						}
						this.audio = xelement.GetAttribute("audio", string.Empty);
						this.lostAimPlanet = xelement.GetAttribute("lostaimplanet", string.Empty);
						this.the_end = xelement.GetAttribute("The_end", string.Empty);
							this.ParseStoryContent(xelement);
					this.ParseDestroyLine(xelement);
						this.defaultai = xelement.GetAttribute("defaultAIStrategy", "-1");
						this.teamAITypes = xelement.GetAttribute("teamAITypes", string.Empty);
						if (this.teamAITypes != string.Empty)
						{
							foreach (string value in this.teamAITypes.Split(new char[]
							{
								','
							}))
							{
								this.teamAiType.Add(Convert.ToInt32(value));
							}
						}
						for (int j = this.teamAiType.Count; j < LocalPlayer.MaxTeamNum; j++)
						{
							this.teamAiType.Add(-1);
						}
						this.teamFriend = xelement.GetAttribute("teamFriend", string.Empty);
						if (this.teamFriend != string.Empty)
						{
							foreach (string value2 in this.teamFriend.Split(new char[]
							{
								','
							}))
							{
								this.teamFriendList.Add(Convert.ToInt32(value2));
							}
						}
						IEnumerable<XElement> enumerable = xelement.Elements("mapbuildings");
						if (enumerable != null)
						{
							foreach (XElement xelement2 in enumerable)
							{
								foreach (XElement element in xelement2.Elements("mapbuilding"))
								{
									MapBuildingConfig mapBuildingConfig = new MapBuildingConfig();
									if (mapBuildingConfig.Load(element))
									{
										this.mbcList.Add(mapBuildingConfig);
									}
								}
							}
						}
						IEnumerable<XElement> enumerable2 = xelement.Elements("maplines");
						if (enumerable2 != null)
						{
							foreach (XElement xelement3 in enumerable2)
							{
								foreach (XElement element2 in xelement3.Elements("mapline"))
								{
									MapLineConfig mapLineConfig = new MapLineConfig();
									if (mapLineConfig.Load(element2))
									{
										this.mlcList.Add(mapLineConfig);
									}
								}
							}
						}
						IEnumerable<XElement> enumerable3 = xelement.Elements("mapplayers");
						if (enumerable3 != null)
						{
							foreach (XElement xelement4 in enumerable3)
							{
								foreach (XElement element3 in xelement4.Elements("mapplayer"))
								{
									MapPlayerConfig mapPlayerConfig = new MapPlayerConfig();
									if (mapPlayerConfig.Load(element3))
									{
										this.mpcList.Add(mapPlayerConfig);
									}
								}
							}
						}
						IEnumerable<XElement> enumerable4 = xelement.Elements("SetParty");
						if (enumerable4 != null)
						{
							foreach (XElement xelement5 in enumerable4)
							{
								MapSetPartyConfig mapSetPartyConfig = new MapSetPartyConfig();
								XElement xelement6 = xelement5.Element("CampNum");
								if (xelement6 != null)
								{
									string text5 = xelement6.GetAttribute("camp", string.Empty);
									if (!string.IsNullOrEmpty(text5))
									{
										string[] array3 = text5.Split(new char[] { ',' });
										foreach (string value3 in array3)
										{
											mapSetPartyConfig.camps.Add(Convert.ToInt32(value3.Trim()));
										}
									}
								}
								XElement xelement7 = xelement5.Element("SpeedUp");
								if (xelement7 != null)
								{
									mapSetPartyConfig.speedUp = Convert.ToSingle(xelement7.GetAttribute("speed", "1"));
								}
								XElement xelement8 = xelement5.Element("ProduceUp");
								if (xelement8 != null)
								{
									mapSetPartyConfig.produceUp = Convert.ToSingle(xelement8.GetAttribute("produce", "1"));
								}
								XElement xelement9 = xelement5.Element("CaptureUp");
								if (xelement9 != null)
								{
									mapSetPartyConfig.captureUp = Convert.ToSingle(xelement9.GetAttribute("capture", "1"));
								}
								XElement xelement10 = xelement5.Element("OccupyUp");
								if (xelement10 != null)
								{
									mapSetPartyConfig.occupyUp = Convert.ToSingle(xelement10.GetAttribute("occupy", "1"));
								}
								XElement xelement11 = xelement5.Element("TimeSet");
								if (xelement11 != null)
								{
									mapSetPartyConfig.interval = Convert.ToSingle(xelement11.GetAttribute("Interval", "0"));
									mapSetPartyConfig.continueTime = Convert.ToSingle(xelement11.GetAttribute("Continue", "0"));
								}
								XElement xelement12 = xelement5.Element("ShipAdd");
								if (xelement12 != null)
								{
									mapSetPartyConfig.shipAdd = Convert.ToInt32(xelement12.GetAttribute("ship", "0"));
								}
								if (mapSetPartyConfig.IsValid())
								{
									this.setPartyList.Add(mapSetPartyConfig);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/maplist/" + this.name + ".xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public void LoadExter(string filePath)
		{
			this.mlcList.Clear();
			this.mbcList.Clear();
			this.mpcList.Clear();
			try
			{
				WWW www = new WWW(filePath);
				while (!www.isDone)
				{
				}
				if (!string.IsNullOrEmpty(www.text))
				{
					XElement xelement = XDocument.Parse(www.text).Element("map");
					if (xelement != null)
					{
						this.id = xelement.GetAttribute("id", string.Empty);
						this.player_count = Convert.ToInt32(xelement.GetAttribute("player_count", "0"));
						this.vertical = Convert.ToBoolean(xelement.GetAttribute("vertical", "False"));
						this.moveCamera = Convert.ToBoolean(xelement.GetAttribute("SetMoveCamera", "False"));
						this.pvp = Convert.ToBoolean(xelement.GetAttribute("pvp", "False"));
						this.coopTimeLimitMin = Convert.ToInt32(xelement.GetAttribute("CoopTimeLimitMin", "0"));
						this.coopAccelerateEnable = Convert.ToInt32(xelement.GetAttribute("CoopAccelerateEnable", "1"));
						this.coopAccelerateTimeMin = Convert.ToInt32(xelement.GetAttribute("CoopAccelerateTimeMin", "5"));
						this.allAlive = Convert.ToInt32(xelement.GetAttribute("Allalive", "0"));
						this.enableAddTowerChange = Convert.ToInt32(xelement.GetAttribute("EnableAddTowerChange", "0"));
						this.pvpTimeLimitMin = Convert.ToInt32(xelement.GetAttribute("PvpTimeLimitMin", "0"));
						this.pvpAccelerateEnable = Convert.ToInt32(xelement.GetAttribute("PvpAccelerateEnable", "-1"));
						this.pvpAccelerateTimeMin = Convert.ToInt32(xelement.GetAttribute("PvpAccelerateTimeMin", "-1"));
						this.mysteryEnabled = false;
						this.mysteryCamp = 0;
						this.mysteryDuration = 0f;
						this.mysteryStartTime = 0f;
						string text3 = xelement.GetAttribute("Mystery", string.Empty);
						if (string.IsNullOrEmpty(text3))
						{
							text3 = xelement.GetAttribute("invisibleArgs", string.Empty);
						}
						if (!string.IsNullOrEmpty(text3))
						{
							string[] array2 = text3.Split(new char[]
							{
								','
							});
							if (array2.Length >= 3)
							{
								int num10 = Convert.ToInt32(array2[0].Trim());
								float num11 = Convert.ToSingle(array2[1].Trim());
								float num12 = Convert.ToSingle(array2[2].Trim());
								if (num11 < 0f)
								{
									num11 = 0f;
								}
								if (num12 < 0f)
								{
									num12 = 0f;
								}
								if (num11 > 0f && num10 >= 0 && num10 < LocalPlayer.MaxTeamNum)
								{
									this.mysteryEnabled = true;
									this.mysteryCamp = num10;
									this.mysteryDuration = num11;
									this.mysteryStartTime = num12;
								}
							}
						}
						this.ifBlackStrike = Convert.ToInt32(xelement.GetAttribute("IfBlackStrike", "0"));
						this.blackStrikeThreshold = Convert.ToInt32(xelement.GetAttribute("IfBlackStrikeThreshold", "220"));
						this.assimilationEnabled = false;
						this.assimilationX = 0f;
						this.assimilationY = 0f;
						this.assimilationCamp = 0;
						this.assimilationRadius = 0f;
						this.assimilationTriggerTime = 0f;
						this.assimilationX2 = 0f;
						this.assimilationY2 = 0f;
						this.assimilationRepeatCount = 1;
						this.assimilationHasSecondPoint = false;
						string text2 = xelement.GetAttribute("Assimilation", string.Empty);
						if (!string.IsNullOrEmpty(text2))
						{
							string[] array = text2.Split(new char[]
							{
								','
							});
							if (array.Length >= 6)
							{
								int num = Convert.ToInt32(array[0].Trim());
								float num2 = Convert.ToSingle(array[1].Trim());
								float num3 = Convert.ToSingle(array[2].Trim());
								int num4 = Convert.ToInt32(array[3].Trim());
								float num5 = Convert.ToSingle(array[4].Trim());
								float num6 = Convert.ToSingle(array[5].Trim());
								float num7 = 0f;
								float num8 = 0f;
								int num9 = 1;
								if (array.Length >= 9)
								{
									num7 = Convert.ToSingle(array[6].Trim());
									num8 = Convert.ToSingle(array[7].Trim());
									num9 = Convert.ToInt32(array[8].Trim());
								}
								if (num6 < 0f)
								{
									num6 = 0f;
								}
								if (num9 < 1)
								{
									num9 = 1;
								}
								if (num != 0 && num5 > 0f && num4 >= 0 && num4 < LocalPlayer.MaxTeamNum)
								{
									this.assimilationEnabled = true;
									this.assimilationX = num2;
									this.assimilationY = num3;
									this.assimilationCamp = num4;
									this.assimilationRadius = num5;
									this.assimilationTriggerTime = num6;
									this.assimilationX2 = num7;
									this.assimilationY2 = num8;
									this.assimilationRepeatCount = num9;
									this.assimilationHasSecondPoint = (array.Length >= 9);
								}
							}
						}
						this.audio = xelement.GetAttribute("audio", string.Empty);
						this.lostAimPlanet = xelement.GetAttribute("lostaimplanet", string.Empty);
						this.the_end = xelement.GetAttribute("The_end", string.Empty);
							this.ParseStoryContent(xelement);
					this.ParseDestroyLine(xelement);
						this.defaultai = xelement.GetAttribute("defaultAIStrategy", "-1");
						this.teamAITypes = xelement.GetAttribute("teamAITypes", string.Empty);
						if (this.teamAITypes != string.Empty)
						{
							foreach (string value in this.teamAITypes.Split(new char[]
							{
								','
							}))
							{
								this.teamAiType.Add(Convert.ToInt32(value));
							}
						}
						for (int j = this.teamAiType.Count; j < LocalPlayer.MaxTeamNum; j++)
						{
							this.teamAiType.Add(-1);
						}
						this.teamFriend = xelement.GetAttribute("teamFriend", string.Empty);
						if (this.teamFriend != string.Empty)
						{
							foreach (string value2 in this.teamFriend.Split(new char[]
							{
								','
							}))
							{
								this.teamFriendList.Add(Convert.ToInt32(value2));
							}
						}
						IEnumerable<XElement> enumerable = xelement.Elements("mapbuildings");
						if (enumerable != null)
						{
							foreach (XElement xelement2 in enumerable)
							{
								foreach (XElement element in xelement2.Elements("mapbuilding"))
								{
									MapBuildingConfig mapBuildingConfig = new MapBuildingConfig();
									if (mapBuildingConfig.Load(element))
									{
										this.mbcList.Add(mapBuildingConfig);
									}
								}
							}
						}
						IEnumerable<XElement> enumerable2 = xelement.Elements("maplines");
						if (enumerable2 != null)
						{
							foreach (XElement xelement3 in enumerable2)
							{
								foreach (XElement element2 in xelement3.Elements("mapline"))
								{
									MapLineConfig mapLineConfig = new MapLineConfig();
									if (mapLineConfig.Load(element2))
									{
										this.mlcList.Add(mapLineConfig);
									}
								}
							}
						}
						IEnumerable<XElement> enumerable3 = xelement.Elements("mapplayers");
						if (enumerable3 != null)
						{
							foreach (XElement xelement4 in enumerable3)
							{
								foreach (XElement element3 in xelement4.Elements("mapplayer"))
								{
									MapPlayerConfig mapPlayerConfig = new MapPlayerConfig();
									if (mapPlayerConfig.Load(element3))
									{
										this.mpcList.Add(mapPlayerConfig);
									}
								}
							}
						}
						IEnumerable<XElement> enumerable4 = xelement.Elements("SetParty");
						if (enumerable4 != null)
						{
							foreach (XElement xelement5 in enumerable4)
							{
								MapSetPartyConfig mapSetPartyConfig = new MapSetPartyConfig();
								XElement xelement6 = xelement5.Element("CampNum");
								if (xelement6 != null)
								{
									string text5 = xelement6.GetAttribute("camp", string.Empty);
									if (!string.IsNullOrEmpty(text5))
									{
										string[] array3 = text5.Split(new char[] { ',' });
										foreach (string value3 in array3)
										{
											mapSetPartyConfig.camps.Add(Convert.ToInt32(value3.Trim()));
										}
									}
								}
								XElement xelement7 = xelement5.Element("SpeedUp");
								if (xelement7 != null)
								{
									mapSetPartyConfig.speedUp = Convert.ToSingle(xelement7.GetAttribute("speed", "1"));
								}
								XElement xelement8 = xelement5.Element("ProduceUp");
								if (xelement8 != null)
								{
									mapSetPartyConfig.produceUp = Convert.ToSingle(xelement8.GetAttribute("produce", "1"));
								}
								XElement xelement9 = xelement5.Element("CaptureUp");
								if (xelement9 != null)
								{
									mapSetPartyConfig.captureUp = Convert.ToSingle(xelement9.GetAttribute("capture", "1"));
								}
								XElement xelement10 = xelement5.Element("OccupyUp");
								if (xelement10 != null)
								{
									mapSetPartyConfig.occupyUp = Convert.ToSingle(xelement10.GetAttribute("occupy", "1"));
								}
								XElement xelement11 = xelement5.Element("TimeSet");
								if (xelement11 != null)
								{
									mapSetPartyConfig.interval = Convert.ToSingle(xelement11.GetAttribute("Interval", "0"));
									mapSetPartyConfig.continueTime = Convert.ToSingle(xelement11.GetAttribute("Continue", "0"));
								}
								XElement xelement12 = xelement5.Element("ShipAdd");
								if (xelement12 != null)
								{
									mapSetPartyConfig.shipAdd = Convert.ToInt32(xelement12.GetAttribute("ship", "0"));
								}
								if (mapSetPartyConfig.IsValid())
								{
									this.setPartyList.Add(mapSetPartyConfig);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/maplist/" + this.name + ".xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Delete()
		{
			bool result = false;
			try
			{
				string path = string.Empty;
				path = Application.streamingAssetsPath + this.Path();
				if (File.Exists(path))
				{
					File.Delete(path);
					result = true;
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/maplist/" + this.name + ".xml delete failed " + ex.ToString(), new object[0]);
			}
			return result;
		}

		public bool Save()
		{
			bool result = false;
			try
			{
				string empty = string.Empty;
				XmlTextWriter xmlTextWriter = new XmlTextWriter(Application.dataPath + "/Res/" + this.Path(), new UTF8Encoding(false));
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartDocument();
				xmlTextWriter.WriteStartElement("map");
				xmlTextWriter.WriteAttributeString("id", this.name);
				xmlTextWriter.WriteAttributeString("player_count", this.player_count.ToString());
				xmlTextWriter.WriteAttributeString("vertical", this.vertical.ToString());
				xmlTextWriter.WriteAttributeString("pvp", this.pvp.ToString());
				if (this.coopTimeLimitMin > 0)
				{
					xmlTextWriter.WriteAttributeString("CoopTimeLimitMin", this.coopTimeLimitMin.ToString());
				}
				if (this.coopAccelerateEnable != 1)
				{
					xmlTextWriter.WriteAttributeString("CoopAccelerateEnable", this.coopAccelerateEnable.ToString());
				}
				if (this.coopAccelerateTimeMin != 5)
				{
					xmlTextWriter.WriteAttributeString("CoopAccelerateTimeMin", this.coopAccelerateTimeMin.ToString());
				}
				if (this.allAlive != 0)
				{
					xmlTextWriter.WriteAttributeString("Allalive", this.allAlive.ToString());
				}
				if (this.enableAddTowerChange != 0)
				{
					xmlTextWriter.WriteAttributeString("EnableAddTowerChange", this.enableAddTowerChange.ToString());
				}
				if (this.pvpTimeLimitMin > 0)
				{
					xmlTextWriter.WriteAttributeString("PvpTimeLimitMin", this.pvpTimeLimitMin.ToString());
				}
				if (this.pvpAccelerateEnable != -1)
				{
					xmlTextWriter.WriteAttributeString("PvpAccelerateEnable", this.pvpAccelerateEnable.ToString());
				}
				if (this.pvpAccelerateTimeMin != -1)
				{
					xmlTextWriter.WriteAttributeString("PvpAccelerateTimeMin", this.pvpAccelerateTimeMin.ToString());
				}
				if (this.survivalTimeSec > 0f)
				{
					xmlTextWriter.WriteAttributeString("survival", this.survivalTimeSec.ToString());
				}
				if (this.mysteryEnabled && this.mysteryDuration > 0f)
				{
					xmlTextWriter.WriteAttributeString("invisibleArgs", string.Format("{0},{1},{2}", new object[]
					{
						this.mysteryCamp,
						this.mysteryDuration,
						this.mysteryStartTime
					}));
				}
				if (this.ifBlackStrike > 0)
				{
					xmlTextWriter.WriteAttributeString("IfBlackStrike", this.ifBlackStrike.ToString());
				}
				if (this.blackStrikeThreshold > 0 && this.blackStrikeThreshold != 220)
				{
					xmlTextWriter.WriteAttributeString("IfBlackStrikeThreshold", this.blackStrikeThreshold.ToString());
				}
				if (this.assimilationEnabled && this.assimilationRadius > 0f)
				{
					if (this.assimilationHasSecondPoint || this.assimilationRepeatCount != 1)
					{
						xmlTextWriter.WriteAttributeString("Assimilation", string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", new object[]
						{
							1,
							this.assimilationX,
							this.assimilationY,
							this.assimilationCamp,
							this.assimilationRadius,
							this.assimilationTriggerTime,
							this.assimilationX2,
							this.assimilationY2,
							this.assimilationRepeatCount
						}));
					}
					else
					{
						xmlTextWriter.WriteAttributeString("Assimilation", string.Format("{0},{1},{2},{3},{4},{5}", new object[]
						{
							1,
							this.assimilationX,
							this.assimilationY,
							this.assimilationCamp,
							this.assimilationRadius,
							this.assimilationTriggerTime
						}));
					}
				}
				xmlTextWriter.WriteAttributeString("audio", this.audio);
				if (!string.IsNullOrEmpty(this.lostAimPlanet))
				{
					xmlTextWriter.WriteAttributeString("lostaimplanet", this.lostAimPlanet);
				}
				if (!string.IsNullOrEmpty(this.the_end))
				{
					xmlTextWriter.WriteAttributeString("The_end", this.the_end);
				}
				if (this.escapeEnabled)
				{
					xmlTextWriter.WriteAttributeString("Escape", "true");
					if (this.notMoved)
					{
						xmlTextWriter.WriteAttributeString("Notmoved", "true");
					}
					xmlTextWriter.WriteAttributeString("Destroy1", string.Format("{0},{1}", this.destroy1X, this.destroy1Y));
					xmlTextWriter.WriteAttributeString("Destroy2", string.Format("{0},{1}", this.destroy2X, this.destroy2Y));
					xmlTextWriter.WriteAttributeString("AttractiveForce", this.destroyLineSpeed.ToString());
					if (this.safeShipCamp >= 0)
					{
						xmlTextWriter.WriteAttributeString("safeship", string.Format("{0},{1}", this.safeShipCamp, this.safeShipTimes));
					}
				}
				xmlTextWriter.WriteAttributeString("defaultAIStrategy", this.defaultai);
				xmlTextWriter.WriteAttributeString("teamAITypes", this.teamAITypes);
				xmlTextWriter.WriteAttributeString("teamFriend", this.teamFriend);
				if (this.mlcList.Count > 0)
				{
					xmlTextWriter.WriteStartElement("maplines");
					foreach (MapLineConfig mapLineConfig in this.mlcList)
					{
						xmlTextWriter.WriteStartElement("mapline");
						xmlTextWriter.WriteAttributeString("point1", mapLineConfig.point1);
						xmlTextWriter.WriteAttributeString("point2", mapLineConfig.point2);
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.WriteEndElement();
				}
				if (this.mbcList.Count > 0)
				{
					xmlTextWriter.WriteStartElement("mapbuildings");
					foreach (MapBuildingConfig mapBuildingConfig in this.mbcList)
					{
						xmlTextWriter.WriteStartElement("mapbuilding");
						xmlTextWriter.WriteAttributeString("id", mapBuildingConfig.id);
						xmlTextWriter.WriteAttributeString("type", mapBuildingConfig.type);
						xmlTextWriter.WriteAttributeString("size", mapBuildingConfig.size.ToString());
						xmlTextWriter.WriteAttributeString("x", mapBuildingConfig.x.ToString());
						xmlTextWriter.WriteAttributeString("y", mapBuildingConfig.y.ToString());
						xmlTextWriter.WriteAttributeString("camption", mapBuildingConfig.camption.ToString());
						xmlTextWriter.WriteAttributeString("tag", mapBuildingConfig.tag);
						if (!string.IsNullOrEmpty(mapBuildingConfig.connected))
						{
							xmlTextWriter.WriteAttributeString("connected", mapBuildingConfig.connected);
						}
						string value = mapBuildingConfig.orbit.ToString();
						if (mapBuildingConfig.orbit == (int)RevolutionType.RT_Custom)
						{
							value = "custom";
						}
						xmlTextWriter.WriteAttributeString("orbit", value);
						xmlTextWriter.WriteAttributeString("orbitParam1", mapBuildingConfig.orbitParam1);
						xmlTextWriter.WriteAttributeString("orbitParam2", mapBuildingConfig.orbitParam2);
						if (mapBuildingConfig.orbit == (int)RevolutionType.RT_Custom)
						{
							string text = string.IsNullOrEmpty(mapBuildingConfig.equation) ? mapBuildingConfig.orbitParam1 : mapBuildingConfig.equation;
							string text2 = string.IsNullOrEmpty(mapBuildingConfig.orbitXChoose) ? mapBuildingConfig.orbitParam2 : mapBuildingConfig.orbitXChoose;
							if (!string.IsNullOrEmpty(text))
							{
								xmlTextWriter.WriteAttributeString("Equation", text);
							}
							if (!string.IsNullOrEmpty(text2))
							{
								xmlTextWriter.WriteAttributeString("orbit_x_choose", text2);
							}
							if (!string.IsNullOrEmpty(mapBuildingConfig.equationX))
							{
								xmlTextWriter.WriteAttributeString("EquationX", mapBuildingConfig.equationX);
							}
							if (!string.IsNullOrEmpty(mapBuildingConfig.equationY))
							{
								xmlTextWriter.WriteAttributeString("EquationY", mapBuildingConfig.equationY);
							}
							if (!string.IsNullOrEmpty(mapBuildingConfig.orbitTChoose))
							{
								xmlTextWriter.WriteAttributeString("orbit_t_choose", mapBuildingConfig.orbitTChoose);
							}
							if (!string.IsNullOrEmpty(mapBuildingConfig.orbitBox))
							{
								xmlTextWriter.WriteAttributeString("orbit_box", mapBuildingConfig.orbitBox);
							}
						}
						if (!string.IsNullOrEmpty(mapBuildingConfig.orbitParam3))
						{
							xmlTextWriter.WriteAttributeString("orbitParam3", mapBuildingConfig.orbitParam3);
						}
						if (!string.IsNullOrEmpty(mapBuildingConfig.orbitParam4))
						{
							xmlTextWriter.WriteAttributeString("orbitParam4", mapBuildingConfig.orbitParam4);
						}
						xmlTextWriter.WriteAttributeString("orbitClockWise", mapBuildingConfig.orbitClockWise.ToString());
						xmlTextWriter.WriteAttributeString("fAngle", mapBuildingConfig.fAngle.ToString());
						if (mapBuildingConfig.orbitRevoSpeed != 12)
						{
							xmlTextWriter.WriteAttributeString("orbitRevoSpeed", mapBuildingConfig.orbitRevoSpeed.ToString());
						}
						if (mapBuildingConfig.shipProduceOverride != -1)
						{
							xmlTextWriter.WriteAttributeString("shipProduceOverride", mapBuildingConfig.shipProduceOverride.ToString());
						}
						if (mapBuildingConfig.lasergunRange != 0f)
						{
							xmlTextWriter.WriteAttributeString("lasergunAngle", mapBuildingConfig.lasergunAngle.ToString());
							xmlTextWriter.WriteAttributeString("lasergunRange", mapBuildingConfig.lasergunRange.ToString());
							xmlTextWriter.WriteAttributeString("lasergunRotateSkip", mapBuildingConfig.lasergunRotateSkip.ToString());
						}
						xmlTextWriter.WriteAttributeString("transformBulidingID", mapBuildingConfig.transformBulidingID);
						xmlTextWriter.WriteAttributeString("curseDelay", mapBuildingConfig.curseDelay.ToString());
						if (!string.IsNullOrEmpty(mapBuildingConfig.aistrategy) && mapBuildingConfig.aistrategy != "-1")
						{
							xmlTextWriter.WriteAttributeString("aistrategy", mapBuildingConfig.aistrategy);
						}
						if (mapBuildingConfig.type == "BarrierPoint")
						{
							xmlTextWriter.WriteAttributeString("bpRange", mapBuildingConfig.fbpRange.ToString());
						}
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.WriteEndElement();
				}
				if (this.mpcList.Count > 0)
				{
					xmlTextWriter.WriteStartElement("mapplayers");
					foreach (MapPlayerConfig mapPlayerConfig in this.mpcList)
					{
						xmlTextWriter.WriteStartElement("mapplayer");
						xmlTextWriter.WriteAttributeString("id", mapPlayerConfig.id);
						xmlTextWriter.WriteAttributeString("tag", mapPlayerConfig.tag);
						xmlTextWriter.WriteAttributeString("ship", mapPlayerConfig.ship.ToString());
						xmlTextWriter.WriteAttributeString("camption", mapPlayerConfig.camption.ToString());
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/maplist/" + this.name + ".xml save failed " + ex.ToString(), new object[0]);
			}
			return result;
		}

		private void ParseStoryContent(XElement xelement)
		{
			this.storySentences.Clear();
			this.storyKeep = 4f;
			this.storyFade = 1.5f;
			XElement content = xelement.Element("content");
			if (content == null)
			{
				return;
			}
			this.storyKeep = content.GetFloatAttribute("keep", 4f);
			this.storyFade = content.GetFloatAttribute("fade", 1.5f);
			if (this.storyKeep <= 0f)
			{
				this.storyKeep = 4f;
			}
			if (this.storyFade <= 0f)
			{
				this.storyFade = 1.5f;
			}
			foreach (XElement sentence in content.Elements("sentence"))
			{
				if (sentence != null && !string.IsNullOrEmpty(sentence.Value))
				{
					this.storySentences.Add(sentence.Value);
				}
			}
		}

		private void ParseDestroyLine(XElement xelement)
		{
			bool escapeEnabled = false;
			bool.TryParse(xelement.GetAttribute("Escape", "False").Trim(), out escapeEnabled);
			this.escapeEnabled = escapeEnabled;
			bool notMoved = false;
			bool.TryParse(xelement.GetAttribute("Notmoved", "False").Trim(), out notMoved);
			this.notMoved = notMoved;
			this.destroy1X = 0f;
			this.destroy1Y = 0f;
			this.destroy2X = 0f;
			this.destroy2Y = 0f;
			this.destroyLineSpeed = 0f;
			this.safeShipCamp = -1;
			this.safeShipTimes = 0;
			string text = xelement.GetAttribute("Destroy1", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[] { ',' });
				if (array.Length >= 2)
				{
					float x;
					float y;
					if (float.TryParse(array[0].Trim(), out x) && float.TryParse(array[1].Trim(), out y))
					{
						this.destroy1X = x;
						this.destroy1Y = y;
					}
				}
			}
			string text2 = xelement.GetAttribute("Destroy2", string.Empty);
			if (!string.IsNullOrEmpty(text2))
			{
				string[] array2 = text2.Split(new char[] { ',' });
				if (array2.Length >= 2)
				{
					float x2;
					float y2;
					if (float.TryParse(array2[0].Trim(), out x2) && float.TryParse(array2[1].Trim(), out y2))
					{
						this.destroy2X = x2;
						this.destroy2Y = y2;
					}
				}
			}
			string text3 = xelement.GetAttribute("AttractiveForce", "0");
			float speed;
			if (float.TryParse(text3.Trim(), out speed))
			{
				this.destroyLineSpeed = speed;
			}
			string text4 = xelement.GetAttribute("safeship", string.Empty);
			if (!string.IsNullOrEmpty(text4))
			{
				string[] array3 = text4.Split(new char[] { ',' });
				if (array3.Length >= 2)
				{
					int camp;
					int times;
					if (int.TryParse(array3[0].Trim(), out camp) && int.TryParse(array3[1].Trim(), out times))
					{
						if (camp >= 0 && camp < LocalPlayer.MaxTeamNum)
						{
							this.safeShipCamp = camp;
							this.safeShipTimes = times;
						}
					}
				}
			}
		}

		public bool Verify()
		{
			return true;
		}

		public bool IfBlackStrike()
		{
			return this.ifBlackStrike > 0;
		}

		public int GetBlackStrikeThreshold()
		{
			return (this.blackStrikeThreshold > 0) ? this.blackStrikeThreshold : 220;
		}

		public bool IfAssimilation()
		{
			return this.assimilationEnabled && this.assimilationRadius > 0f;
		}

		public bool IfMystery()
		{
			return this.mysteryEnabled && this.mysteryDuration > 0f;
		}

		public void LoadExtraData()
		{
		}

		public List<MapLineConfig> mlcList;

		public List<MapBuildingConfig> mbcList;

		public List<MapPlayerConfig> mpcList;

		public string name;

		public const string root = "map";

		public int player_count;

		public bool vertical;

		public bool pvp;

		public int coopTimeLimitMin;

		public int coopAccelerateEnable = 1;

		public int coopAccelerateTimeMin = 5;

		public int allAlive;

		public int enableAddTowerChange;

		public int pvpTimeLimitMin;

		public int pvpAccelerateEnable = -1;

		public int pvpAccelerateTimeMin = -1;

		public float survivalTimeSec;

		public bool mysteryEnabled;

		public int mysteryCamp;

		public float mysteryDuration;

		public float mysteryStartTime;

		public int ifBlackStrike;

		public int blackStrikeThreshold = 220;

		public bool assimilationEnabled;

		public float assimilationX;

		public float assimilationY;

		public int assimilationCamp;

		public float assimilationRadius;

		public float assimilationTriggerTime;

		public float assimilationX2;

		public float assimilationY2;

		public int assimilationRepeatCount = 1;

		public bool assimilationHasSecondPoint;

		public string audio;
		public string lostAimPlanet;
		public string the_end;

		public List<string> storySentences = new List<string>();

		public float storyKeep = 4f;

		public float storyFade = 1.5f;

		public bool escapeEnabled;
		public bool notMoved;
		public float destroy1X;
		public float destroy1Y;
		public float destroy2X;
		public float destroy2Y;
		public float destroyLineSpeed;
		public int safeShipCamp = -1;
		public int safeShipTimes;

		public string defaultai;

		public string id;

		public string teamAITypes;

		public List<int> teamAiType;

		public string teamFriend;

		public List<int> teamFriendList = new List<int>();

		public bool moveCamera = false;

		public List<MapSetPartyConfig> setPartyList = new List<MapSetPartyConfig>();

		public bool IfSetParty()
		{
			return this.setPartyList != null && this.setPartyList.Count > 0;
		}

		public bool IfMoveCamera()
		{
			return this.moveCamera;
		}
	}

	public class MapSetPartyConfig
	{
		public float speedUp = 1f;

		public float produceUp = 1f;

		public float captureUp = 1f;

		public float occupyUp = 1f;

		public float interval;

		public float continueTime;

		public List<int> camps = new List<int>();

		public int shipAdd;

		public bool IsValid()
		{
			return this.camps != null && this.camps.Count > 0 && this.continueTime > 0f;
		}
	}
}
