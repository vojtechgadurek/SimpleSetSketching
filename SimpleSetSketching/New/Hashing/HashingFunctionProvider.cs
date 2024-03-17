using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Hashing
{
	public interface IHashingFunctionProvider
	{
		HashingFunctionExpression GetHashingFunction(ulong size, Random random);
	}

	public static class HashingFunctionProvider
	{
		static Random _random = new Random();
		static Dictionary<HashingFunctionKind, IHashingFunctionProvider> HashingFunctionProviders = new Dictionary<HashingFunctionKind, IHashingFunctionProvider>
		{
			{ HashingFunctionKind.LinearCongrunce, new LinearCongruenceHashingFunctionGenerator() },
			{ HashingFunctionKind.MultiplyShift, new MultiplyShiftHashingFunctionGenerator() }
		};
		public enum HashingFunctionKind
		{
			LinearCongrunce,
			MultiplyShift
		}
		public static HashingFunctionExpression GetHashingFunction(HashingFunctionKind hashingFunctionKind, ulong size, Random? random = null)
		{
			if (random == null)
			{
				random = HashingFunctionProvider._random;
			}
			return HashingFunctionProviders[hashingFunctionKind].GetHashingFunction(size, random);
		}
	}
}
