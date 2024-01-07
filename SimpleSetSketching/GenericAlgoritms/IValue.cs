using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	interface IValue
	{
		bool IsZero();
		bool IsEqual(IValue other);
		int BitLength();
	}
}
