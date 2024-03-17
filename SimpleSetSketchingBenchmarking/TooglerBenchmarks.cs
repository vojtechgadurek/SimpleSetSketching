using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Stacks;
using SimpleSetSketching;
using SimpleSetSketching.New.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.New.StreamProviders;
using SimpleSetSketching.New.Tooglers.TooglingFunctions;
using SimpleSetSketching.New.Utils;
using SimpleSetSketching.New.Hashing;

using HashingFunctionKind = SimpleSetSketching.New.Hashing.HashingFunctionProvider.HashingFunctionKind;
using System.Linq.Expressions;
using LittleSharp;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using LittleSharp.Callables;

namespace SimpleSetSketchingBenchmarking
{

	public class TestHashFunctions
	{
		public ulong[][]? data;
		public ulong[]? ansver;
		const ulong Size = 4096;
		const int Length = 1000;
		Func<ulong[], ulong[], ulong[]> hashFunction;
		[Params(42, 679, 809, 1238)]
		public ulong Seed;

		[Params(HashingFunctionKind.LinearCongrunce, HashingFunctionKind.MultiplyShift)]
		public HashingFunctionKind hashFunctionKind;

		[GlobalSetup]
		public void GlobalSetup()
		{
			data = new ulong[Length][];
			for (int i = 0; i < Length; i++)
			{
				data[i] = GenerateRandomULongArray.GenerateRandomULongArrayFunc((int)Size, Seed);
			}
			ansver = new ulong[Size];
		}

		[IterationSetup]
		public void IterationSetup()
		{
			data = new ulong[Length][];
			for (int i = 0; i < Length; i++)
			{
				data[i] = GenerateRandomULongArray.GenerateRandomULongArrayFunc((int)Size, Seed);
			}
			ansver = new ulong[Size];
			Random random = new Random((int)Seed);
		}

		public Func<ulong[], ulong[], int, ulong[]> GetHashFunction(HashingFunctionKind hashFunctionKind, Random random)
		{
			var hashingFunction = HashingFunctionProvider.GetHashingFunction(hashFunctionKind, Size, random);

			var f = CompiledFunctions.Create<ulong[], ulong[], int, ulong[]>(
				out var inputTable,
				out var outputTable,
				out var size
				);
			var x = outputTable.V.IsTable<ulong>();
			f.S.Assign(f.Output,
				inputTable.V.IsTable<ulong>().Select(
					hashingFunction,
					outputTable.V.IsTable<ulong>(),
					size.V
				).V
			);
			return f.Construct().Compile();
		}

		[Benchmark]
		public ulong TestHashing()
		{
			ulong sum = 0;
			for (int i = 0; i < Length; i++)
			{
				ansver = hashFunction(data[i], ansver);
				for (int j = 0; j < (int)Size; j++)
				{
					sum += ansver[j];
				}
			}
			return sum;
		}



	}

}
