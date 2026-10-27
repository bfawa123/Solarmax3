using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class InnateSkillConfigProvider : Solarmax.Singleton<InnateSkillConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/innateskill.txt";
		}

		public bool IsXML()
		{
			return false;
		}

		public void Load()
		{
			this.dataList.Clear();
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				InnateSkillConfig innateSkillConfig = new InnateSkillConfig();
				innateSkillConfig.innateskillid = FileReader.ReadInt();
				innateSkillConfig.name = FileReader.ReadString();
				innateSkillConfig.desc = FileReader.ReadString();
				innateSkillConfig.defaultmaxfix = FileReader.ReadFloat();
				innateSkillConfig.movespeedfix = FileReader.ReadFloat();
				innateSkillConfig.occupiedspeedfix = FileReader.ReadFloat();
				innateSkillConfig.createspeedfix = FileReader.ReadFloat();
				innateSkillConfig.totalmaxfix = FileReader.ReadFloat();
				innateSkillConfig.occupiedcreate = FileReader.ReadFloat();
				innateSkillConfig.occupieddestroy = FileReader.ReadFloat();
				innateSkillConfig.flyhide = FileReader.ReadFloat();
				innateSkillConfig.populationhide = FileReader.ReadFloat();
				innateSkillConfig.towertrueview = FileReader.ReadFloat();
				innateSkillConfig.towerrangefix = FileReader.ReadFloat();
				innateSkillConfig.occupiedtripspeedfix = FileReader.ReadFloat();
				innateSkillConfig.occupiedfakepop = FileReader.ReadFloat();
				innateSkillConfig.occupiedcreatespeedarg1 = FileReader.ReadFloat();
				innateSkillConfig.occupiedcreatespeedarg2 = FileReader.ReadFloat();
				innateSkillConfig.incapturespeedfix = FileReader.ReadFloat();
				innateSkillConfig.pozhuArg1 = FileReader.ReadFloat();
				innateSkillConfig.pozhuArg2 = FileReader.ReadFloat();
				innateSkillConfig.chizhiArg1 = FileReader.ReadFloat();
				innateSkillConfig.chizhiArg2 = FileReader.ReadFloat();
				innateSkillConfig.pidiArg1 = FileReader.ReadFloat();
				innateSkillConfig.pidiArg2 = FileReader.ReadFloat();
				innateSkillConfig.diguArg1 = FileReader.ReadFloat();
				innateSkillConfig.diguArg2 = FileReader.ReadFloat();
				innateSkillConfig.tips = FileReader.ReadString();
				innateSkillConfig.tipsdir = FileReader.ReadInt();
				innateSkillConfig.tipsallshow = FileReader.ReadInt();
				this.dataList.Add(innateSkillConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<InnateSkillConfig> GetAllData()
		{
			return this.dataList;
		}

		public InnateSkillConfig GetData(int innateskillid)
		{
			InnateSkillConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].innateskillid.Equals(innateskillid))
				{
					result = this.dataList[i];
					break;
				}
			}
			return result;
		}

		public void LoadExtraData()
		{
		}

		private List<InnateSkillConfig> dataList = new List<InnateSkillConfig>();
	}
}
