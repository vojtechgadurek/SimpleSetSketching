using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public class BitValueArray
	{
		/// <summary>
		/// Allows to store long value to any bit in concesutive block of memory 
		/// </summary>
		long[] _data;
		const int _numberOfBits = 6;
		const int _mask = 1 << (_numberOfBits + 1) + 1;


		public BitValueArray(int size)
		{
			int longarraylength = size / 64 + 1 + 1; // +1 for remainder +1 as we need to reed 2 long at a time
			_data = new long[size];
		}

		public long Get(int index)
		{
			int offset = index & _mask;
			int arrayIndex = index >> 6;
			long first = _data[arrayIndex];
			long second = _data[arrayIndex + 1];
			return (first << offset) | (second >> (64 - offset));
		}

		private void InnerSet(int index, long value)
		{
			_data[index] = value;
		}


		public (int, int) IndexToOffset(int index)
		{
			int offset = index & _mask;
			int arrayIndex = index >> _numberOfBits;
			return (arrayIndex, offset);
		}

		public void Set(int index, long value)
		{
			(int arrayIndex, int offset) = IndexToOffset(index);
			long first = value >> offset;
			long second = value << (64 - offset);
			_data[arrayIndex] = first;
			_data[arrayIndex + 1] = second;
		}

		public void Xor(int index, long value)
		{
			(int arrayIndex, int offset) = IndexToOffset(index);
			long first = value >> offset;
			long second = value << (64 - offset);
			_data[arrayIndex] ^= first;
			_data[arrayIndex + 1] ^= second;
		}

	}
}
