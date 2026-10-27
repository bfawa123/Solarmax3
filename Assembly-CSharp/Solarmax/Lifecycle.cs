using System;

namespace Solarmax
{
	public interface Lifecycle
	{
		bool Init();

		void Tick(float interval);

		void Destroy();
	}
}
