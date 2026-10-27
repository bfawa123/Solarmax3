using System;
using Newtonsoft.Json;

public class JsonHelper
{
	public static string SerializeObject(object o)
	{
		return JsonConvert.SerializeObject(o);
	}
}
