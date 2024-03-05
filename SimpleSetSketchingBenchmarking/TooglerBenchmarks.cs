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

namespace SimpleSetSketchingBenchmarking
{

	public static class StreamProvider
	{
		public class ArrayLongStream : ISketchStream<ulong>
		{
			int count = 0;
			ulong[][] _data;
			public ArrayLongStream(ulong[][] data)
			{
				_data = data;
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public TruncatedArray<ulong> FillBuffer(ulong[] buffer)
			{
				if (count >= _data.Length)
				{
					return new TruncatedArray<ulong>(0, new ulong[0]);
				}
				var data = _data[count++];
				var length = data.Length;
				return new TruncatedArray<ulong>(length, data);
			}

			public uint? Length()
			{
				throw new NotImplementedException();
			}
		}
		static ulong[][] _data;
		static StreamProvider()
		{
			int n = 1000;
			int size = 4096;
			var data = new ulong[n][];

			for (int i = 0; i < n; i++)
			{
				data[i] = GenerateRandomULongArray.GenerateRandomULongArrayFunc(size, 42);
			}
			_data = data;
		}
		public static ISketchStream<ulong> Get()
		{
			return new ArrayLongStream(_data);
		}

	}
	internal class TooglerBenchmarks
	{
		[Benchmark]
		public void TestToogleStreamToTable()
		{
		}
	}
}
