using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using LittleSharp;

namespace SimpleSetSketching.New.Hashing
{
	public class LinearCongruenceHashingFunctionGenerator : IHashingFunctionProvider
	{

		static List<int> MersenePrimesExponents = new List<int>
		{
			2,
			3,
			5,
			7,
			13,
			17,
			19,
			31,
			61,
		};
		static List<ulong> MersenePrimes = new List<ulong> { };
		static LinearCongruenceHashingFunctionGenerator()
		{
			foreach (var exponent in MersenePrimesExponents)
			{
				MersenePrimes.Add((2UL << exponent) - 1UL);
			}

		}
		public static ulong GetGoodPrime(ulong size)
		{
			var index = MersenePrimes.FindIndex(x => x > size);
			if (index == -1)
			{
				throw new Exception("Size is too big");
			}
			return MersenePrimes[index];
		}
		public static HashingFunctionExpression CreateHashFunction(ulong multiply, ulong add, ulong size)
		{
			var f = new CompiledFunction<ulong, ulong>(out var value);
			f.S.Assign(f.Output, ((value.V * multiply) % GetGoodPrime(size) + add) % size);
			return f.Construct();
		}

		public static Func<ulong, ulong> GetUnderlyingHashFunction(ulong multiply, ulong add, ulong size)
		{
			return (x) => ((x * multiply) % GetGoodPrime(size) + add) % size;
		}

		public HashingFunctionExpression GetHashingFunction(ulong size, Random random)
		{
			return CreateHashFunction((ulong)random.NextInt64(), (ulong)random.NextInt64(), size);
		}
	}
}
