using System;

namespace Solarmax
{
	public interface IDataProvider
	{
		bool IsXML();

		string Path();

		void Load();

		bool Verify();

		void LoadExtraData();
	}
}
