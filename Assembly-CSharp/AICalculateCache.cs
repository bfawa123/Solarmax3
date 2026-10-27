using System;

public class AICalculateCache
{
	public AICalculateCache()
	{
		for (int i = 0; i < this.resetData.Length; i++)
		{
			this.resetData[i] = -1;
		}
	}

	public void Reset()
	{
		Array.Copy(this.resetData, 0, this.teamStrength, 0, LocalPlayer.MaxTeamNum);
		Array.Copy(this.resetData, 0, this.teamStrengthMine, 0, LocalPlayer.MaxTeamNum);
		Array.Copy(this.resetData, 0, this.predictedTeamStrength, 0, LocalPlayer.MaxTeamNum);
		Array.Copy(this.resetData, 0, this.oppStrength, 0, LocalPlayer.MaxTeamNum);
		Array.Copy(this.resetData, 0, this.predictedOppStrength, 0, LocalPlayer.MaxTeamNum);
	}

	public int[] resetData = new int[LocalPlayer.MaxTeamNum];

	public int[] teamStrength = new int[LocalPlayer.MaxTeamNum];

	public int[] teamStrengthMine = new int[LocalPlayer.MaxTeamNum];

	public int[] predictedTeamStrength = new int[LocalPlayer.MaxTeamNum];

	public int[] oppStrength = new int[LocalPlayer.MaxTeamNum];

	public int[] predictedOppStrength = new int[LocalPlayer.MaxTeamNum];
}
