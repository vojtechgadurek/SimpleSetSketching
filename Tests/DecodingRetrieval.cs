using SimpleSetSketching.Testing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests
{
	public class DecodingRetrieval
	{
		ITestOutputHelper _output;
		public DecodingRetrieval(ITestOutputHelper output)
		{
			_output = output;
		}

		public double RetrievalRate<TTable>(DecodingResult<TTable, SimpleSetDecoder<TTable>> decoder)
			where TTable : IEnumerable<ulong>
		{
			return (double)decoder.Decoder.GetCurrentDecodedValues().Count() / (double)decoder.NumberOfItemsInSymmetricDifference;
		}

		public TestDecoderFactory<ulong[], SimpleSetDecoder<ulong[]>> GetSimpleDecodingFactory(int tableSize)
		{
			return new TestDecoderFactoryConfiguration<ulong[], SimpleSetDecoder<ulong[]>>()
				.SetTableFactory(size => new ulong[size])
				.SetHashingFunctionFactory((size) => ModuloHashingFunctionGenerator.Create((ulong)size, new Random()))
				.SetDecoderFactory((table, size, hashingFunctions) => new OneHashIdentityXOR<ulong[]>(table, 4).Decoder)
				.SetDataFactory((size) => RandomDataGenerator.GenerateNotNullRandomUInt64(size, 0))
				.SetToggle(SimpleSetSketchFunc.GetXorToggle<ulong[]>())
				.SetNumberOfHashingFunction(1)
				.Build(tableSize);
		}

		public TestDecoderFactoryConfiguration<ulong[], SimpleSetDecoder<ulong[]>> GetTestingConfiguration()
		{
			return new TestDecoderFactoryConfiguration<ulong[], SimpleSetDecoder<ulong[]>>()
				.SetTableFactory(size => new ulong[size])
				.SetDecoderFactory((table, size, hashingFunctions) => new OneHashIdentityXOR<ulong[]>(table, 4).Decoder)
				.SetDataFactory((size) => RandomDataGenerator.GenerateNotNullRandomUInt64(size, 0));
		}

		[Fact]
		public void OneHashIdentity()
		{
			var factory = GetSimpleDecodingFactory(2000);
			var answer = factory.TestOneDecoding(1000);

			_output.WriteLine($"Retrieval rate is {RetrievalRate(answer)}");
		}


		[Theory]
		[InlineData(1000)]
		[InlineData(2000)]
		[InlineData(4000)]
		[InlineData(8000)]
		[InlineData(16000)]

		public void TestFindOptimalSize(int tableSize)
		{
			var factory = GetSimpleDecodingFactory(tableSize);
			var finder = new OptimalMultiplierToDataFinder<ulong[], SimpleSetDecoder<ulong[]>>(factory);
			var anwser = finder.BatteryTest(0.1, 3, 0.1, 10);
			var pipe = anwser.Select(x => (x.Key, x.Value.Select(RetrievalRate).Average())).OrderBy(x => x.Item1);


			double TrunctateToNDecimalPlaces(double x, int nDecimalPlaces)
			{
				return Math.Truncate(x * Math.Pow(10, nDecimalPlaces)) / Math.Pow(10, nDecimalPlaces);
			}

			_output.WriteLine("Table size, Retrieval rate");
			foreach (var item in pipe)
			{
				_output.WriteLine($"{TrunctateToNDecimalPlaces(item.Item1, 2)} " +
					$"{TrunctateToNDecimalPlaces(item.Item2, 2)}"
					);

			}

			_output.WriteLine("\nGraph\n");
			_output.WriteLine(new string('|', 20));
			_output.WriteLine("maximum");
			foreach (var item in pipe)
			{
				_output.WriteLine(new string('|', (int)(item.Item2 * 20)));
			}



		}


	}
}
