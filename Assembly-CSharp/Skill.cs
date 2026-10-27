using System;
using System.Collections.Generic;
using Solarmax;

public class Skill
{
	public SkillConfig tab { get; set; }

	public float lifetime { get; set; }

	public float actiontime { get; set; }

	public TEAM actTEAM;

	public TEAM srcTEAM;

	public List<string> tags = new List<string>();
}
