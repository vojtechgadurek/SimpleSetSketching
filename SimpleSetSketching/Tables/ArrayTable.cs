using SimpleSetSketching.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public struct ArrayTable<TValue> : ITable<TValue> where TValue : struct, IValue<TValue>
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
			return _data.All(x => x.IsNull());
		}

		public uint Length()
		{
			return (uint)_data.Length;
		}

		public void Set(uint index, TValue value)
		{
			_data[index] = value;
		}

		public void Xor(uint index, TValue value)
		{
			throw new NotImplementedException();
		}
	}
}
