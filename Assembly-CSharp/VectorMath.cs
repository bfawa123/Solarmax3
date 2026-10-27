using System;
using UnityEngine;

public static class VectorMath
{
	public static Quaternion AngleAxis(float angle, Vector3 axis)
	{
		float magnitude = axis.magnitude;
		if (magnitude > 1E-06f)
		{
			float f = angle * 0.5f;
			Quaternion result = default(Quaternion);
			result.w = Mathf.Cos(f);
			float num = Mathf.Sin(f) / magnitude;
			result.x = num * axis.x;
			result.y = num * axis.y;
			result.z = num * axis.z;
			return result;
		}
		return Quaternion.identity;
	}

	public static Vector3 RotateAround(this Vector3 position, Vector3 point, Vector3 axis, float angle)
	{
		Vector3 vector = position;
		Quaternion rotation = VectorMath.AngleAxis(angle, axis);
		Vector3 vector2 = vector - point;
		vector2 = rotation * vector2;
		vector = point + vector2;
		position = vector;
		return position;
	}

	public static Quaternion Rotate(this Quaternion quaternion, Vector3 eulerAngles)
	{
		Quaternion rhs = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
		return quaternion *= rhs;
	}
}
