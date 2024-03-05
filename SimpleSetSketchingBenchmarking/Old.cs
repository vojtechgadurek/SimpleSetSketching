using BenchmarkDotNet.Attributes;
using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketchingBenchmarking
{
	public class TestLambdaExpresionTrees
	{

		public readonly Func<int, int> DivisionLambdaExpression;
		public readonly Func<int, int> DivisionLambdaBuiledExpression;

		private const int N = 1000;
		private const int divisor = 2;

		private readonly Random _random = new Random();

		int[] data = new int[N];

		public TestLambdaExpresionTrees()
		{
			DivisionLambdaBuiledExpression = GetDivisionByConstant(divisor);
			DivisionLambdaExpression = GetDivisionByConstantLambda(divisor);
			for (int i = 0; i < N; i++)
			{
				data[i] = _random.Next();
			}
		}

		public Func<int, int> GetDivisionByConstantLambda(int divisor)
		{
			Expression<Func<int, int>> lambda = a => a / divisor;
			return lambda.Compile();


		}

		public Func<int, int> GetDivisionByConstant(int divisor)
		{
			ParameterExpression a = Expression.Parameter(typeof(int), "toBeDivided");
			Expression body = Expression.Divide(a, Expression.Constant(divisor));
			return Expression.Lambda<Func<int, int>>(body, a).Compile();
		}

		[Benchmark]
		public int PureDivision()
		{
			int ans = 0;
			for (int i = 0; i < N; i++)
			{
				ans += data[i] / divisor;
			}
			return ans;
		}

		[Benchmark]
		public void DivisionPureLambda()
		{
			int ans = 0;
			for (int i = 0; i < N; i++)
			{
				ans += DivisionLambdaExpression(data[i]);
			}
		}

		[Benchmark]
		public void DivisionBuiledLambda()
		{
			int ans = 0;
			for (int i = 0; i < N; i++)
			{
				ans += DivisionLambdaBuiledExpression(data[i]);
			}
		}

		public class TestCyclingInHashing
		{
			private const int N = 1000;
			const ulong size = 132647;
			readonly int size_power = BitOperations.TrailingZeroCount(size);
			const ulong multiply = 348421;
			public ulong sizeUnoptimized = size;
			public ulong multplyUnoptimized = multiply;

			readonly Action<ulong[], int, ulong[]> hashFunction = HashBuffer.GetHashingFunctionApplier(MultiplyShiftHashGenerator.CreateHashFunction(multiply, size)).Compile();
			public Func<ulong, ulong> hashFunctionPure = MultiplyShiftHashGenerator.GetTrueOperation(multiply, size);
			public Func<ulong, ulong> hashFunctionPureOptimized;

			ulong[] data = GenerateRandomULongArray.GenerateRandomULongArrayFunc(N, 42);

			public TestCyclingInHashing()
			{
				hashFunctionPureOptimized = (value) => ((value * multiply) >> size_power) % size;
			}

			[Benchmark]
			public ulong[] TestCyclingInHashingFunction()
			{
				ulong[] result = new ulong[N];
				hashFunction(data, N, result);
				return result;
			}

			[Benchmark]
			public ulong[] TestCyclingInHashingFunctionPure()
			{
				ulong[] result = new ulong[N];
				for (int i = 0; i < N; i++)
				{
					result[i] = MultiplyShiftHashGenerator.DoTrueOperation(data[i], multiply, size);
				}
				return result;
			}

			[Benchmark]
			public ulong[] TestCyclingInHashingFunctionPure2()
			{
				ulong[] result = new ulong[N];
				int push = BitOperations.TrailingZeroCount(size);
				for (int i = 0; i < N; i++)
				{
					result[i] = ((data[i] * multiply) >> push) % size;
				}
				return result;

			}
			[Benchmark]

			public ulong[] TestCyclingInHashingFunctionPureUnoptimized()
			{
				ulong[] result = new ulong[N];
				for (int i = 0; i < N; i++)
				{
					result[i] = MultiplyShiftHashGenerator.DoTrueOperation(data[i], multplyUnoptimized, sizeUnoptimized);
				}
				return result;
			}

			[Benchmark]
			[MethodImpl(MethodImplOptions.NoInlining)]
			public ulong[] TestCyclingInHashingFunctionPureUnoptimizedNotInlined()
			{
				ulong[] result = new ulong[N];
				for (int i = 0; i < N; i++)
				{
					result[i] = hashFunctionPure.Invoke(result[i]);
				}
				return result;
			}

			[Benchmark]
			[MethodImpl(MethodImplOptions.NoInlining)]
			public ulong[] TestCyclingInHashingFunctionPureNotInlined()
			{
				ulong[] result = new ulong[N];

				for (int i = 0; i < N; i++)
				{
					result[i] = hashFunctionPureOptimized.Invoke(result[i]);
				}
				return result;
			}







		}
	}
}
