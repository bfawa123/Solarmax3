using System;
using UnityEngine;

public class ParticleLife : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (!base.GetComponent<ParticleSystem>().IsAlive())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
