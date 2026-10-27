using System;

[Serializable]
public class UnknownSkillPacket
{
	public int skillID;

	public TEAM from;

	public TEAM to;

	public string tag;

	public string transformId;
}
