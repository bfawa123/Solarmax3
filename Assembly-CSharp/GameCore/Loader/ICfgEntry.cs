using System;
using System.Xml.Linq;

namespace GameCore.Loader
{
	public interface ICfgEntry
	{
		bool Load(XElement element);
	}
}
