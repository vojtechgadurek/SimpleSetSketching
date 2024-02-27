using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public interface IValue<T> : IComparable<T> where T : struct
	{
		bool IsNull();
		int BitLength();
	}
}
