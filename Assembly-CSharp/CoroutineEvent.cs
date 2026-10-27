using System;
using System.Collections.Generic;

public class CoroutineEvent
{
	public List<EventDelegate> handler;

	public float delay;

	public float currentTime;

	public bool end;
}
