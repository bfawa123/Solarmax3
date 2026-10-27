using System;
using System.Collections.Generic;

[Serializable]
public class MovePacket
{
	public TEAM team;

	public string from;

	public string to;

	public List<string> tags = new List<string>();

	public float rate;

	public int optype;
}
