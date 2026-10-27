using System;
using UnityEngine;

public class Timer
{
	public Timer(float max)
	{
		this.timer = Time.realtimeSinceStartup;
		this.timeMax = max;
	}

	public bool CheckTimer()
	{
		if (this.timer == 0f)
		{
			this.timer = Time.realtimeSinceStartup;
		}
		if (Time.realtimeSinceStartup - this.timer > this.timeMax)
		{
			this.timer = Time.realtimeSinceStartup;
			return true;
		}
		return false;
	}

	public float timer;

	public float timeMax;
}
