using System;

namespace ProtoBuf
{
	public struct SubItemToken
	{
		internal SubItemToken(int value)
		{
			this.value = value;
		}

		internal readonly int value;
	}
}
