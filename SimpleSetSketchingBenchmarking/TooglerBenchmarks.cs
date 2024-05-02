using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Stacks;
using SimpleSetSketching;
using SimpleSetSketching.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.StreamProviders;
using SimpleSetSketching.Togglers;
using SimpleSetSketching.Utils;
using SimpleSetSketching.Sketchers;
using SimpleSetSketching.InvertibleBloomLookupTable.Tables;

using System.Linq.Expressions;
using LittleSharp;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using LittleSharp.Callables;

namespace SimpleSetSketchingBenchmarking
{

	public class BenchmarkToogling
	{

		[ParamsSource(nameof(HashingFunctionsToTest))]
		public Type hashingFunctionFamily;

		public static IEnumerable<Type> HashingFunctionsToTest() => HashingFunctionProvider.GetAllHashingFunctionFamilies();

		public const int Length = 1024;
		public const int BufferLength = 4096;
		public const int DataLength = Length * BufferLength;

		[ParamsSource(nameof(TableLengths))]
		public ulong TableLength;


		public static IEnumerable<ulong> TableLengths()
			=> new ulong[] { 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576 };

		public Random? random;
		public ArrayLongStream? Stream;

		[IterationSetup]
		public void IterationSetup()
		{
			random = new Random();
			Stream = RandomStreamData.Create((ulong)DataLength, random);
		}



		[Benchmark]
		public ulong[] BenchmarkToggler()
		{
			var table = new ulong[TableLength];

			var hashFunction = HashingFunctionProvider.Get(hashingFunctionFamily, TableLength).Create();

			var toggler = new Toggler<ulong[]>(BufferLength, table, new[] { hashFunction }, SimpleSetSketchFunc.GetXorToggle<ulong[]>());

			toggler.ToggleStreamToTable(Stream!);
			return table;
		}

		[Benchmark]
		public BasicHypergraphTable BenchmarkIBLTBenchmark()
		{
			var table = new BasicHypergraphTable((int)TableLength);

			var hashFunction = HashingFunctionProvider.Get(hashingFunctionFamily, TableLength).Create();

			var toggler = new Toggler<BasicHypergraphTable>(BufferLength, table, new[] { hashFunction }, BasicHypergraphTable.GetTogglingAction());

			toggler.ToggleStreamToTable(Stream!);
			return table;
		}
	}

	public class BenchmarkHashSetToggling
	{

		public const int Length = 1024;
		public const int BufferLength = 4096;
		public const int DataLength = Length * BufferLength;

		[ParamsSource(nameof(TableLengths))]
		public ulong TableLength;

		public static IEnumerable<ulong> TableLengths() => new ulong[] { 1024, 65536, 262144, 524288 };

		public Random? random;
		public ArrayLongStream? Stream;

		[Params(true, false)]
		public bool FixedSize;

		[IterationSetup]
		public void IterationSetup()
		{
			random = new Random();
			Stream = RandomStreamData.Create((ulong)DataLength, random);
		}

		void ToggleStreamToTable(ISketchStream<ulong> stream, HashSet<ulong> set)
		{
			ulong[] buffer = new ulong[BufferLength];
			for (int i = 0; i < Length; i++)
			{
				stream.FillBuffer(buffer);
				for (int j = 0; j < BufferLength; j++)
				{
					if (set.Contains(buffer[j]))
					{
						set.Remove(buffer[j]);
					}
					else
					{
						set.Add(buffer[j]);
					}
				}
			}
		}


		[Benchmark]
		public HashSet<ulong> BenchmarkToggler()
		{
			HashSet<ulong> set;
			if (FixedSize)
			{
				set = new HashSet<ulong>((int)TableLength);
			}
			else
			{
				set = new HashSet<ulong>();
			}

			ToggleStreamToTable(Stream, set);
			return set;

		}
	}

}
