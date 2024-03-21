using BenchmarkDotNet.Attributes;
using SimpleSetSketching.New.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.StreamProviders
{
	public static class RandomStreamData
	{
		public static ulong[] GenerateRandomArray(ulong length, Random random)
		{
			ulong[] data = new ulong[length];
			for (ulong i = 0; i < length; i++)
			{
				data[i] = (ulong)random.NextInt64();
			}
			return data;
		}
		public static ArrayLongStream Create(ulong length, Random random)
		{
			return new ArrayLongStream(GenerateRandomArray(length, random));
		}
	}
	public class ArrayLongStream : ISketchStream<ulong>
	{
		int count = 0;
		ulong[] _data;
		public ArrayLongStream(ulong[] data)
		{
			_data = data;
		}

		public void Dispose()
		{
		}

		public TruncatedArray<ulong> FillBuffer(ulong[] buffer)
		{
			int unfiled = buffer.Length - (_data.Length - count);
			if (unfiled < 0)
			{
				unfiled = 0;
			}

			int returnedItems = buffer.Length - unfiled;
			Array.Copy(_data, count, buffer, 0, returnedItems);
			count += returnedItems;

			return new TruncatedArray<ulong>(returnedItems, buffer);
		}

		public uint? Length()
		{
			return (uint)_data.Length;
		}
	}
}
