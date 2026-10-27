using System;
using UnityEngine;

namespace Solarmax
{
	public class UnityTools
	{
		public static GameObject AddChild(GameObject parent, GameObject prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
			if (null != gameObject && null != parent)
			{
				Transform transform = gameObject.transform;
				transform.SetParent(parent.transform);
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				gameObject.layer = parent.layer;
			}
			return gameObject;
		}
	}
}
