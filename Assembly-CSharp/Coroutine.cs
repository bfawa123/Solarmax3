using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine : MonoBehaviour
{
	private void Awake()
	{
		global::Coroutine.coroutine = this;
		this.enumeratorQueue = new Queue<IEnumerator>();
	}

	private void Start()
	{
	}

	public static void Start(IEnumerator ie)
	{
		global::Coroutine.coroutine.enumeratorQueue.Enqueue(ie);
		global::Coroutine.coroutine.Check();
	}

	public static int DelayDo(float delay, EventDelegate handler)
	{
		object obj = global::Coroutine.coroutine.delayEvents;
		int result;
		lock (obj)
		{
			CoroutineEvent coroutineEvent = new CoroutineEvent();
			coroutineEvent.handler = new List<EventDelegate>();
			coroutineEvent.handler.Add(handler);
			coroutineEvent.currentTime = 0f;
			coroutineEvent.delay = delay;
			coroutineEvent.end = false;
			global::Coroutine.coroutine.delayEvents.Add(++global::Coroutine.delayKey, coroutineEvent);
			result = global::Coroutine.delayKey;
		}
		return result;
	}

	public static void CancelDelayDo(int key)
	{
		object obj = global::Coroutine.coroutine.delayEvents;
		lock (obj)
		{
			global::Coroutine.coroutine.delayEvents.Remove(key);
		}
	}

	private void Do()
	{
		object obj = global::Coroutine.coroutine.events;
		lock (obj)
		{
			EventDelegate.Execute(this.events);
		}
	}

	private IEnumerator Runtor(IEnumerator ie)
	{
		yield return ie;
		this.nowRunningCount--;
		this.Check();
		yield break;
	}

	private void Check()
	{
		for (int i = this.nowRunningCount; i < this.maxRunningCoroutine; i++)
		{
			if (this.enumeratorQueue.Count <= 0)
			{
				break;
			}
			base.StartCoroutine(this.Runtor(this.enumeratorQueue.Dequeue()));
			this.nowRunningCount++;
		}
	}

	private void Update()
	{
		if (global::Coroutine.coroutine.delayEvents.Count > 0)
		{
			object obj = global::Coroutine.coroutine.delayEvents;
			lock (obj)
			{
				foreach (CoroutineEvent coroutineEvent in global::Coroutine.coroutine.delayEvents.Values)
				{
					if (!coroutineEvent.end)
					{
						coroutineEvent.currentTime += Time.deltaTime;
						if (coroutineEvent.currentTime > coroutineEvent.delay)
						{
							coroutineEvent.end = true;
							EventDelegate.Execute(coroutineEvent.handler);
						}
					}
				}
				foreach (KeyValuePair<int, CoroutineEvent> keyValuePair in global::Coroutine.coroutine.delayEvents)
				{
					if (keyValuePair.Value.end)
					{
						global::Coroutine.coroutine.delayEvents.Remove(keyValuePair.Key);
						break;
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		base.StopAllCoroutines();
	}

	public static void StopCoroutines()
	{
		global::Coroutine.coroutine.StopAllCoroutines();
	}

	public int maxRunningCoroutine = 5;

	private static global::Coroutine coroutine;

	private int nowRunningCount;

	private Queue<IEnumerator> enumeratorQueue;

	private float delta;

	private List<EventDelegate> events = new List<EventDelegate>();

	private Dictionary<int, CoroutineEvent> delayEvents = new Dictionary<int, CoroutineEvent>();

	private static int delayKey;
}
