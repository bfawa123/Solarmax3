using System;

namespace Solarmax
{
	public interface IBuffer<T>
	{
		void Push(T[] data);

		void Push(T[] data, int length);

		T[] Pop();

		T[] Pop(int length);

		T Get(int index);

		void Clear();

		T[] Buffer();

		int DataSize();

		int Size();
	}
}
