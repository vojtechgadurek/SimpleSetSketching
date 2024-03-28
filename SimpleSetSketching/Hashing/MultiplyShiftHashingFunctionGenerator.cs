using LittleSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Hashing
{
	public class MultiplyShiftHashingFunctionGenerator : IHashingFunctionProvider
	{
		static Random _random = new Random();
		public static HashingFunctionExpression CreateHashFunction(ulong multiply, ulong size)
		{
			var kMerLength = BitOperations.LeadingZeroCount(size);
			var f = new CompiledFunction<ulong, ulong>(out var value_);

			f.S.Assign(f.Output, (value_.V * multiply >> (64 - kMerLength)) % size);
			return f.Construct();
		}

		public HashingFunctionExpression GetHashingFunction(ulong size, Random random)
		{
			return CreateHashFunction((ulong)random.NextInt64(), size);
		}
	}
}
