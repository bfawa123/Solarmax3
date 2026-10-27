using System;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleSystemReset : MonoBehaviour
{
	private void Start()
	{
		this.effect = base.transform.GetComponent<ParticleSystem>();
		if (this.effect != null)
		{
			this.orgDelay = this.effect.main.startDelay.constant;
		}
	}

	private void OnEnable()
	{
		if (this.effect != null)
		{
			MainModule mm = this.effect.main;
			mm.startDelay = this.orgDelay;
		}
	}

	private ParticleSystem effect;

	private float orgDelay;
}
