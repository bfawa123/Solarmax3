using System;
using Solarmax;

public static class MathUtils
{
	public static float AngleProcess(float inputAngle)
	{
		LoggerSystem.CodeComments("角度处理方法：输入一个角度，将其处理到-180到180范围内");
		inputAngle %= 360f;
		inputAngle -= (float)Convert.ToInt32(Math.Truncate((double)(inputAngle / 180f))) * 360f;
		return inputAngle;
	}
}
