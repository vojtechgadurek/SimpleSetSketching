using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.New.Hashing;
using SimpleSetSketching.New.Utils;
using SimpleSetSketching.New.StreamProviders;
using System.Security.Cryptography.X509Certificates;
using BenchmarkDotNet.Reports;

namespace SimpleSetSketchingBenchmarking
{
	public class HashingBenchmark
	{

		[ParamsSource(nameof(HashingFunctionsToTest))]
		public HashingFunctionProvider.HashingFunctionKind hashingFunction;

		public static IEnumerable<HashingFunctionProvider.HashingFunctionKind> HashingFunctionsToTest() =>
			Enum.GetValues(typeof(HashingFunctionProvider.HashingFunctionKind)).Cast<HashingFunctionProvider.HashingFunctionKind>();

		public const int Length = 1024;
		public const int BufferLength = 4096;
		public const int DataLength = Length * BufferLength;
		public Random? random;
		public ArrayLongStream? Stream;


		[IterationSetup]
		public void IterationSetup()
		{
			random = new Random();
			Stream = RandomStreamData.Create((ulong)DataLength, random);
		}

		[Benchmark]
		public Delegate CompileHashFunction()
		{
			return HashingFunctionProvider.GetBufferedHashingFunction(hashingFunction, 4096).Compile();
		}

		[Benchmark]
		public ulong BenchmarkHashingFunction()
		{
			Func<ulong[], ulong[], int, ulong[]> f = HashingFunctionProvider.GetBufferedHashingFunction(hashingFunction, 4096).Compile();
			ulong sum = 0;
			ulong[] buffer = new ulong[BufferLength];
			for (int i = 0; i < Length; i++)
			{
				Stream!.FillBuffer(buffer);
				f(buffer, new ulong[BufferLength], BufferLength);
				for (int j = 0; j < BufferLength; j++)
				{
					sum += buffer[j];
				}
			}
			return sum;
		}

		[Benchmark]
		public ulong BenchmarkHashingFunctionRandomSize()
		{
			Func<ulong[], ulong[], int, ulong[]> f = HashingFunctionProvider.GetBufferedHashingFunction(hashingFunction, (uint)random.NextInt64()).Compile();
			ulong sum = 0;
			ulong[] buffer = new ulong[BufferLength];
			for (int i = 0; i < Length; i++)
			{
				Stream!.FillBuffer(buffer);
				f(buffer, new ulong[BufferLength], BufferLength);
				for (int j = 0; j < BufferLength; j++)
				{
					sum += buffer[j];
				}
			}
			return sum;
		}
	}

	public class TestHashingFunctionCompileTime
	{

	}
}
