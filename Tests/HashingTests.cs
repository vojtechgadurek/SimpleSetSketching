using LittleSharp.Callables;
using SimpleSetSketching.New.Hashing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	public class HashingTests
	{
		public static bool CompareHashingFunctions(Func<ulong, ulong> f1, Func<ulong, ulong> f2, int numberOfCasesToTest)
		{
			Random random = new Random();
			for (int i = 0; i < numberOfCasesToTest; i++)
			{
				ulong a = (ulong)random.Next();
				if (f1(a) != f2(a))
				{
					return false;
				}
			}
			return true;
		}

		[Theory]
		[InlineData(1000)]
		public void TestMultiplyShiftHashingFunction(ulong numberOfCasesToTest)
		{
			Random random = new Random();
			int a;
			for (int i = 0; i < 100; i++)
			{
				ulong size = (ulong)random.Next();
				int kMerLength = BitOperations.TrailingZeroCount(size);
				ulong multiply = (ulong)random.NextInt64();
				Func<ulong, ulong> f1 = (x) => (x * multiply >> (64 - kMerLength)) % size;
				Func<ulong, ulong> f2 = MultiplyShiftHashingFunctionGenerator.CreateHashFunction(multiply, size).Compile();
				Assert.True(CompareHashingFunctions(f1, f2, (int)numberOfCasesToTest));
			}
		}

		[Theory]
		[InlineData(1000)]
		public void TestLinearCongruenceHashingFunction(ulong numberOfCasesToTest)
		{
			Random random = new Random();
			for (int i = 0; i < 100; i++)
			{
				ulong size = (ulong)random.Next();
				ulong a = (ulong)random.Next();
				ulong b = (ulong)random.Next();
				Func<ulong, ulong> f1 = LinearCongruenceHashingFunctionGenerator.GetUnderlyingHashFunction(a, b, size);
				Func<ulong, ulong> f2 = LinearCongruenceHashingFunctionGenerator.CreateHashFunction(a, b, size).Compile();
				Assert.True(CompareHashingFunctions(f1, f2, (int)numberOfCasesToTest));
			}
		}
	}
}
