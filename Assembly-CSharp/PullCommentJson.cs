using System;

[Serializable]
public class PullCommentJson
{
	public int id;

	public string openId;

	public string content;

	public string commitTime;

	public bool liked;

	public int totalLikes;

	public string[] tags;
}
