using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Values
{
	public struct UlongValue : IValue<UlongValue>
	{
		public ulong Value;
		public UlongValue(ulong value)
		{
			Value = value;
		}
		public bool IsNull()
		{
			return Value == 0;
		}
		public int BitLength()
		{
			throw new NotImplementedException();
		}

		public int CompareTo(UlongValue other)
		{
			if (Value == other.Value)
				return 0;
			else if (Value > other.Value)
				return 1;
			else
				return -1;
		}
	}
}
