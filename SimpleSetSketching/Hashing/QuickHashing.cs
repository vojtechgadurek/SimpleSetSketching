using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public class QuickHashing : ISketchHashFunction
	{
		LinearCongruence First;
		LinearCongruence Second;
		LinearCongruence Third;
		public QuickHashing()
		{
			First = new LinearCongruence(30000, 10000, 100057);

			Second = new LinearCongruence(3038821, 1192348, 102199);

			Third = new LinearCongruence(10239, 1983283, 102233);
		}

		public (ulong, ulong, ulong) GetHash(ulong x)
		{
			return (First.Hash(x), Second.Hash(x), Third.Hash(x));
		}
	}

	public class LinearCongruence
	{
		ulong a;
		ulong b;
		ulong p;
		public LinearCongruence(ulong a, ulong b, ulong p)
		{
			this.a = a;
			this.b = b;
			this.p = p;
		}
		public ulong Hash(ulong x)
		{
			return (ulong)((a * x + b) % p);
		}
	}

	public static class LinearCongruenceUtils
	{
		public static ulong Hash(ulong x, ulong a, ulong b, ulong p)
		{
			return (ulong)((a * x + b) % p);
		}
	}
}
