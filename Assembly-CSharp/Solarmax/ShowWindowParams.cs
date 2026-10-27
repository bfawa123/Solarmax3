using System;

namespace Solarmax
{
	public struct ShowWindowParams
	{
		public ShowWindowParams(string windowName, EventId initEventId = EventId.Undefine, params object[] initEventArgs)
		{
			this.WindowName = windowName;
			this.InitEventId = initEventId;
			this.InitEventArgs = initEventArgs;
		}

		public bool IsNone
		{
			get
			{
				return string.IsNullOrEmpty(this.WindowName);
			}
		}

		public static readonly ShowWindowParams None = new ShowWindowParams(null, EventId.Undefine, new object[0]);

		public string WindowName;

		public EventId InitEventId;

		public object[] InitEventArgs;
	}
}
