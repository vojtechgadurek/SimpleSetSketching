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
			First = new LinearCongruence(100000000063, 100000000223, 100000004987);

			Second = new LinearCongruence(100000003921, 203921, 100000002943);

			Third = new LinearCongruence(100000004837, 1983283, 100000003277);
		}

		public ulong FirstHash(ulong x)
		{
			return First.Hash(x);
		}
		public ulong SecondHash(ulong x)
		{
			return Second.Hash(x);
		}
		public ulong ThirdHash(ulong x)
		{
			return Third.Hash(x);
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
