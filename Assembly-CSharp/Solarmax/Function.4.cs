using System;

namespace Solarmax
{
	public delegate T Function<out T, T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);
}
