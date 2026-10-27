using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class EventSystem : Solarmax.Singleton<EventSystem>, Lifecycle
	{
		public EventSystem()
		{
			this.mEventHandlerMap = new Dictionary<int, List<EventHandler2>>();
			this.mEventPool = new SimplePool<Event2>();
			this.mFiredEventList = new List<Event2>();
		}

		public bool Init()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("EventSystem    init  begin", new object[0]);
			this.mEventHandlerMap.Clear();
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("EventSystem    init  end", new object[0]);
			return true;
		}

		public void Tick(float interval)
		{
			for (int i = 0; i < 10; i++)
			{
				if (i >= this.mFiredEventList.Count)
				{
					break;
				}
				this.TrigEvent(this.mFiredEventList[i]);
				this.mFiredEventList.RemoveAt(i);
			}
		}

		public void Destroy()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("EventSystem    destroy  begin", new object[0]);
			this.mEventHandlerMap.Clear();
			for (int i = 0; i < this.mFiredEventList.Count; i++)
			{
				this.mEventPool.Recycle(this.mFiredEventList[i]);
			}
			this.mFiredEventList.Clear();
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("EventSystem    destroy  end", new object[0]);
		}

		public void RegisterEvent(EventId id, object hoster, object data, Callback<int, object, object[]> handler)
		{
			this.RegisterEvent((int)id, hoster, data, handler);
		}

		public void RegisterEvent(int key, object hoster, object data, Callback<int, object, object[]> handler)
		{
			EventHandler2 eventHandler = new EventHandler2();
			eventHandler.Init(key, hoster, data, handler);
			List<EventHandler2> list;
			if (this.mEventHandlerMap.ContainsKey(key))
			{
				list = this.mEventHandlerMap[key];
			}
			else
			{
				list = new List<EventHandler2>();
				this.mEventHandlerMap.Add(key, list);
			}
			list.Add(eventHandler);
		}

		public void UnRegisterEvent(EventId id, object hoster)
		{
			this.UnRegisterEvent((int)id, hoster);
		}

		public void UnRegisterEvent(int key, object hoster)
		{
			List<EventHandler2> list = null;
			if (this.mEventHandlerMap.TryGetValue(key, out list))
			{
				List<EventHandler2> list2 = new List<EventHandler2>();
				list2.AddRange(list);
				for (int i = 0; i < list2.Count; i++)
				{
					EventHandler2 eventHandler = list2[i];
					if (eventHandler != null && eventHandler.IsEvent(key, hoster))
					{
						list.Remove(eventHandler);
					}
				}
			}
		}

		public void FireEvent(EventId id, params object[] args)
		{
			this.FireEvent((int)id, args);
		}

		public void FireEvent(int key, params object[] args)
		{
			Event2 @event = this.mEventPool.Alloc();
			@event.Set(key, args);
			this.TrigEvent(@event);
		}

		public void FireEventCache(int key, params object[] args)
		{
			Event2 @event = null;
			for (int i = 0; i < this.mFiredEventList.Count; i++)
			{
				if (this.mFiredEventList[i].GetKey() == key)
				{
					@event = this.mFiredEventList[i];
					break;
				}
			}
			if (@event == null)
			{
				@event = this.mEventPool.Alloc();
				@event.Set(key, args);
				this.mFiredEventList.Add(@event);
			}
			else
			{
				@event.Set(key, args);
			}
		}

		private void TrigEvent(Event2 e)
		{
			List<EventHandler2> list = null;
			if (this.mEventHandlerMap.TryGetValue(e.GetKey(), out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					EventHandler2 eventHandler = list[i];
					if (eventHandler != null)
					{
						eventHandler.Fire(e);
					}
				}
			}
			this.mEventPool.Recycle(e);
		}

		private const int MAX_PROCESS_PER_TICK = 10;

		private Dictionary<int, List<EventHandler2>> mEventHandlerMap;

		private SimplePool<Event2> mEventPool;

		private List<Event2> mFiredEventList;
	}
}
