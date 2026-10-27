using System;

namespace Solarmax
{
	[Serializable]
	public class NoticeResponse : IWebResponse
	{
		public bool HasError
		{
			get
			{
				return this.ErrorCode != 200;
			}
		}

		public static NoticeResponse TestData
		{
			get
			{
				return new NoticeResponse
				{
					ErrorCode = 200,
					Notice = "{\"Type\":0, \"Content\":\"Hello world!\"}"
				};
			}
		}

		public int ErrorCode;

		public string Notice;
	}
}
