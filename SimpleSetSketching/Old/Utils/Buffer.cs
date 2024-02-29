using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{

	/*
	internal class Buffer<T> where T : struct
	{
		private readonly T[] _buffer;
		private int _index = 0;
		private int _nItemsRecieved = 0;
		public delegate int FillBuffer(T[] buffer, int index, int length);
		private FillBuffer _fillBufferDelegate;
		public Buffer(int size, FillBuffer fillBuffer)
		{
			_buffer = new T[size];
			_fillBufferDelegate = fillBuffer;
		}
		public T? Get()
		{
			if (_index < _nItemsRecieved) return _buffer[_index++];
			ReFill();
			if (_nItemsRecieved == 0) return null;
			return _buffer[_index++];
		}

		private void ReFill()
		{
			_index = 0;
			_nItemsRecieved = _fillBufferDelegate(_buffer, 0, _buffer.Length);
		}
	}
	*/
	public class UnsafeFixedSizeQueue<T> where T : struct
	{/// <summary>
	 /// This class should be as quick as possible as such it is not safe.
	 /// It gurantees that up to _KMerLength given it will work as normal queue, otherwise behavior is UNDEFINED and you SHOULD NOT depend on it.
	 /// </summary>
		T?[]? _queue;
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

	internal class FixedHistoryBuffer<T> where T : struct
	{
		private readonly T[] _buffer;
		private int _index = 0;
		private int _nItemsRecieved = 0;
		private int _historySize;
		public delegate int FillBuffer(T[] buffer, int index, int length);
		private FillBuffer _fillBufferDelegate;
		public FixedHistoryBuffer(int size, int history_size, FillBuffer fillBuffer)
		{
			_buffer = new T[size + history_size];
			_fillBufferDelegate = fillBuffer;
		}
		public T? Get()
		{
			if (_index < _nItemsRecieved) return _buffer[_index++];
			ReFill();
			if (_nItemsRecieved == 0) return null;
			return _buffer[_index++];
		}

		private void ReFill()
		{
			_index = _historySize;
			Array.Copy(_buffer, _historySize, _buffer, 0, _buffer.Length - _historySize);
			_nItemsRecieved = _fillBufferDelegate(_buffer, _index, _buffer.Length - _index);
		}
	}
}
