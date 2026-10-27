using System;

[Serializable]
public class AverageScoreJson
{
	public string appId;

	public string scoreId;

	public float average;

	public int total;

	public ScoreDetailJson[] detail;
}
