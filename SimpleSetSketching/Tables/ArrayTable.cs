using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Tables
{
	struct ArrayTable<TValue> : ITable<TValue> where TValue : struct, IValue
	{
		TValue[] _data;
		public ArrayTable(uint size)
		{
			_data = new TValue[size];
		}

		public TValue Get(uint index)
		{
			return _data[index];
		}

		public bool IsEmpty()
		{
			return _data.All(x => x.IsZero());
		}

		public uint Length()
		{
			return (uint)_data.Length;
		}

		public void Set(uint index, TValue value)
		{
			_data[index] = value;
		}
	}
}
