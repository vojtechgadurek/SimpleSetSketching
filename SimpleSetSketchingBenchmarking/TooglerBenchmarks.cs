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
using SimpleSetSketching.New.Tooglers;
using SimpleSetSketching.New.Utils;
using SimpleSetSketching.New.Hashing;
using SimpleSetSketching.New.Sketchers;
using HashingFunctionKind = SimpleSetSketching.New.Hashing.HashingFunctionProvider.HashingFunctionKind;
using System.Linq.Expressions;
using LittleSharp;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using LittleSharp.Callables;

namespace SimpleSetSketchingBenchmarking
{

	public class BenchmarkToogling
	{

		[ParamsSource(nameof(HashingFunctionsToTest))]
		public HashingFunctionProvider.HashingFunctionKind hashingFunction;

		public static IEnumerable<HashingFunctionProvider.HashingFunctionKind> HashingFunctionsToTest() =>
			Enum.GetValues(typeof(HashingFunctionProvider.HashingFunctionKind)).Cast<HashingFunctionProvider.HashingFunctionKind>();

		public const int Length = 1024;
		public const int BufferLength = 4096;
		public const int DataLength = Length * BufferLength;

		[ParamsSource(nameof(TableLengths))]
		public ulong TableLength;

		public static IEnumerable<ulong> TableLengths() => new[] { 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576 };

		public Random? random;
		public ArrayLongStream? Stream;

		[IterationSetup]
		public void IterationSetup()
		{
			random = new Random();
			Stream = RandomStreamData.Create((ulong)DataLength, random);
		}

		[Benchmark]
		public ulong[] BenchmarkToogler()
		{
			var table = new ulong[TableLength];

			var hashFunction = HashingFunctionProvider.GetHashingFunction(hashingFunction, TableLength);

			var toogler = new Toogler<ulong[]>(BufferLength, table, new[] { hashFunction }, SimpleSetSketchFunc.GetXorToogle<ulong[]>());

			toogler.ToogleStreamToTable(Stream!);
			return table;

		}
	}
}
