using System;
using UnityEngine;

public class Active : MonoBehaviour
{
	public void SetActive(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		base.gameObject.SetActive(!go.activeSelf);
	}
}
