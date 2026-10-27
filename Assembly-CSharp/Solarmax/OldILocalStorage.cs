using System;

namespace Solarmax
{
	public interface OldILocalStorage
	{
		string Name();

		void Save(OldLocalStorageSystem manager);

		void Load(OldLocalStorageSystem manager);

		void Clear(OldLocalStorageSystem manager);
	}
}
