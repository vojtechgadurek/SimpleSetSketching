using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	public class DecoderTesting
	{
		public static SimpleSetDecoder<ulong[]> GetDecoder()
		{

		}

		[Fact]
		public void TestEmptyDecoding()
		{
			// Test decoding over one 
			const int size = 1000;
			var hashingFunction = LinearCongruenceHashingFunctionGenerator.CreateHashFunction(1, 0, size);
			ulong[] table = new ulong[size];

			var decoder = new SimpleSetDecoder<ulong[]>(
				new[] { hashingFunction },
				table,
				size,
				x => x.All(x => x == 0),
				SimpleSetSketchFunc.GetXorToggle<ulong[]>()
				);

			for (ulong i = 0; i < size; i++)
			{
				table[i] = i;
			}
			decoder.Init().Decode();
			Assert.True(table.All(x => x == 0));
			for (ulong i = 0; i < size; i++)
			{
				Assert.Contains(i, decoder.GetCurrentDecodedValues());
			}
		}

		[Fact]
		public void TestInitialize()
		{

		}
	}
}
