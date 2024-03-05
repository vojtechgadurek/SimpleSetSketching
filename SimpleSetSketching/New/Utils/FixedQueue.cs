using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Utils
{
	public class UnsafeFixedSizeQueue<T> where T : struct
	{/// <summary>
	 /// This class should be as quick as possible as such it is not safe.
	 /// It gurantees that up to KMerLength given it will work as normal queue, otherwise behavior is UNDEFINED and you SHOULD NOT depend on it.
	 /// </summary>
		readonly T?[] _queue;
		int _head = 0;
		int _tail = 0;
		int _mask;
		public UnsafeFixedSizeQueue(int size)
		{
			int actual_size = 1;
			while (actual_size < size)
			{
				actual_size <<= 1;
			}
			_mask = actual_size - 1;
			_queue = new T?[actual_size];
		}

		public void Enqueue(T? item)
		{
			_queue[_tail] = item;
			_tail = (_tail + 1) & _mask;
		}
		public T? Dequeue()
		{
			T? ansver = _queue[_head];
			_head = (_head + 1) & _mask;
			return ansver;
		}
	}
}
