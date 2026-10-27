using System;

namespace Solarmax
{
	public class TBuffer<T> : IBuffer<T>
	{
		public TBuffer(int length)
		{
			this._offset = 0;
			this._data = new T[length];
			this._lock = new object();
		}

		private void Resize(int times)
		{
			T[] data = this._data;
			this._data = new T[data.Length * times];
			Array.Copy(data, this._data, data.Length);
		}

		public void Push(T[] data)
		{
			this.Push(data, data.Length);
		}

		public void Push(T[] data, int length)
		{
			if (length <= 0)
			{
				return;
			}
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._offset + length > this._data.Length)
				{
					int times = (this._offset + length) / this._data.Length + 1;
					this.Resize(times);
				}
				Array.Copy(data, 0, this._data, this._offset, length);
				this._offset += length;
			}
		}

		public T[] Pop()
		{
			return this.Pop(this._offset);
		}

		public T[] Pop(int length)
		{
			if (length > this._offset || length <= 0)
			{
				return null;
			}
			T[] array = new T[length];
			object @lock = this._lock;
			lock (@lock)
			{
				Array.Copy(this._data, array, length);
				Array.Copy(this._data, length, this._data, 0, this._offset - length);
				this._offset -= length;
			}
			return array;
		}

		public T Get(int index)
		{
			return (index >= this._offset) ? default(T) : this._data[index];
		}

		public void Clear()
		{
			this._offset = 0;
		}

		public T[] Buffer()
		{
			return this._data;
		}

		public int DataSize()
		{
			return this._offset;
		}

		public int Size()
		{
			return this._data.Length;
		}

		private int _offset;

		private T[] _data;

		private object _lock;
	}
}
