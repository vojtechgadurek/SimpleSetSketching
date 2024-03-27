using BenchmarkDotNet.Reports;
using LittleSharp;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests
{
	public class DecoderTesting
	{
		public static SimpleSetDecoder<ulong[]> GetDecoder()
		{
			throw new NotImplementedException();

		}


		public DecoderTesting(ITestOutputHelper output)
		{
			Scope._debugOutput = output;
		}
		[Fact]
		public void TestSimpleDecoding()
		{
			// Test decoding over one 
			const int size = 10;
			ulong[] table = new ulong[size];

			var decoder = new OneHashIdentityXOR<ulong[]>(table, 4).Decoder;

			for (ulong i = 0; i < size; i++)
			{
				table[i] = i;
			}

			decoder.Init().Decode();
			Assert.True(table.All(x => x == 0));
			for (ulong i = 1; i < size; i++)
			{
				Assert.Contains(i, decoder.GetCurrentDecodedValues());
			}
		}

		[Fact]
		public void TestSimplePlusSizeDecoding()
		{
			// Test decoding over one 
			const int size = 10;
			ulong[] table = new ulong[size];

			var decoder = new OneHashIdentityXOR<ulong[]>(table, 4).Decoder;

			for (ulong i = 0; i < size; i++)
			{
				table[i] = i + size;
			}

			decoder.Init().Decode();
			Assert.True(table.All(x => x == 0));
			for (ulong i = 0; i < size; i++)
			{
				Assert.Contains(i + size, decoder.GetCurrentDecodedValues());
			}
		}


		[Fact]
		public void TestInitialize()
		{
			Summary summary;

		}
	}
}
