using System;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class FPS : MonoBehaviour
{
	private void Start()
	{
		this.m_FpsNextPeriod = Time.realtimeSinceStartup + 0.5f;
		this.m_Text = base.GetComponent<UILabel>();
	}

	private void Update()
	{
		this.m_FpsAccumulator++;
		if (Time.realtimeSinceStartup > this.m_FpsNextPeriod)
		{
			this.m_CurrentFps = (int)((float)this.m_FpsAccumulator / 0.5f);
			this.m_FpsAccumulator = 0;
			this.m_FpsNextPeriod += 0.5f;
			this.m_Text.text = string.Format("{0} FPS", this.m_CurrentFps);
		}
	}

	private const float fpsMeasurePeriod = 0.5f;

	private int m_FpsAccumulator;

	private float m_FpsNextPeriod;

	private int m_CurrentFps;

	private const string display = "{0} FPS";

	private UILabel m_Text;
}
