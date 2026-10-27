using System;

namespace Solarmax
{
	public interface Lifecycle2
	{
		bool Init();

		void Tick(int frame, float interval);

		void Destroy();
	}
}
