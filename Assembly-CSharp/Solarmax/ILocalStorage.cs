using System;

namespace Solarmax
{
	public interface ILocalStorage
	{
		string Name();

		void Save(LocalStorageSystem manager);

		void Load(LocalStorageSystem manager);

		void Clear(LocalStorageSystem manager);
	}
}
