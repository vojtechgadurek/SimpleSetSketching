using SimpleSetSketching.Testing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using static SimpleSetSketching.Hashing.HashingFunctionProvider;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
			return GetBaseTestingConfiguration()
				.SetHashingFunctionFactory((size) => ModuloHashingFunctionGenerator.Create((ulong)size, new Random()))
				.SetDecoderFactory((table, size, hashingFunctions, toggle) => new SimpleSetDecoder<ulong[]>(hashingFunctions, table, size, toggle))
				.SetNumberOfHashingFunctions(1)
				.Build(tableSize);
		}

		public TestDecoderFactoryConfiguration<ulong[], SimpleSetDecoder<ulong[]>> GetBaseTestingConfiguration()
		{
			return new TestDecoderFactoryConfiguration<ulong[], SimpleSetDecoder<ulong[]>>()
				.SetTableFactory(size => new ulong[size])
				.SetDataFactory((size) => RandomDataGenerator.GenerateNotNullRandomUInt64(size, 0))
				.SetToggle(SimpleSetSketchFunc.GetXorToggle<ulong[]>())
;
		}
		private double Truncate(double x, int nDecimalPlaces)
		{
			return Math.Truncate(x * Math.Pow(10, nDecimalPlaces)) / Math.Pow(10, nDecimalPlaces);
		}

		private void PrintData(IEnumerable<(double, double)> data)
		{
			_output.WriteLine("Ratio table size and number of items, Retrieval rate");
			foreach (var item in data)
			{
				_output.WriteLine($"{Truncate(item.Item1, 3)} {Truncate(item.Item2, 3)}");
			}
		}
		private void PrintGraph(IEnumerable<(double, double)> data)
		{
			_output.WriteLine("\nGraph\n");
			_output.WriteLine(new string('|', 20));
			_output.WriteLine("maximum");
			foreach (var item in data)
			{
				_output.WriteLine(new string('|', (int)(item.Item2 * 20)));
			}
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
			var answer = finder.BatteryTest(0.1, 3, 0.1, 10);
			var pipe = answer.Select(x => (x.Key, x.Value.Select(RetrievalRate).Average())).OrderBy(x => x.Item1);
			PrintData(pipe);
			PrintGraph(pipe);
		}

		[Theory]
		[InlineData(HashingFunctionProvider.HashingFunctionKind.MultiplyShift)]
		[InlineData(HashingFunctionProvider.HashingFunctionKind.LinearCongruence)]
		public void TestDifferentHashingFunction(HashingFunctionProvider.HashingFunctionKind hashingFunctionKind)
		{

			var factory = GetBaseTestingConfiguration()
				.SetHashingFunctionFactory((size) =>
				HashingFunctionProvider.GetHashingFunction(hashingFunctionKind, (ulong)size))
				.SetDecoderFactory((table, size, hashingFunctions, toggle) => new SimpleSetDecoder<ulong[]>(hashingFunctions, table, size, toggle))
				.SetNumberOfHashingFunctions(1)
				.SetSizeOfBuffer(1024)
			.Build(2000);

			var finder = new OptimalMultiplierToDataFinder<ulong[], SimpleSetDecoder<ulong[]>>(factory);
			var anwser = finder.BatteryTest(0.1, 3, 0.1, 10);
			var pipe = anwser.Select(x => (x.Key, x.Value.Select(RetrievalRate).Average())).OrderBy(x => x.Item1);
			PrintData(pipe);
			PrintGraph(pipe);
		}

		[Theory]
		[InlineData(HashingFunctionProvider.HashingFunctionKind.MultiplyShift)]
		[InlineData(HashingFunctionProvider.HashingFunctionKind.LinearCongruence)]
		public void TestSSSAlgorithm(HashingFunctionProvider.HashingFunctionKind hashingFunctionKind)
		{
			var factory = GetBaseTestingConfiguration()
				.SetHashingFunctionFactory((size) =>
				HashingFunctionProvider.GetHashingFunction(hashingFunctionKind, (ulong)size))
				.SetDecoderFactory((table, size, hashingFunctions, toggle) => new SimpleSetDecoder<ulong[]>(hashingFunctions, table, size, toggle))
				.SetNumberOfHashingFunctions(3)
				.SetSizeOfBuffer(1024)
			.Build(2000);

			var finder = new OptimalMultiplierToDataFinder<ulong[], SimpleSetDecoder<ulong[]>>(factory);
			var anwser = finder.BatteryTest(0.75, 1, 0.01, 10);
			var pipe = anwser.Select(x => (x.Key, x.Value.Select(RetrievalRate).Average())).OrderBy(x => x.Item1);
			PrintData(pipe);
			PrintGraph(pipe);
		}

		[Theory]
		[InlineData(HashingFunctionProvider.HashingFunctionKind.MultiplyShift)]
		[InlineData(HashingFunctionProvider.HashingFunctionKind.LinearCongruence)]
		public void TestSSSAlgorithm5(HashingFunctionProvider.HashingFunctionKind hashingFunctionKind)
		{
			var factory = GetBaseTestingConfiguration()
				.SetHashingFunctionFactory((size) =>
				HashingFunctionProvider.GetHashingFunction(hashingFunctionKind, (ulong)size))
				.SetDecoderFactory((table, size, hashingFunctions, toggle) => new SimpleSetDecoder<ulong[]>(hashingFunctions, table, size, toggle))
				.SetNumberOfHashingFunctions(2)
				.SetSizeOfBuffer(1024)
			.Build(2000);

			var finder = new OptimalMultiplierToDataFinder<ulong[], SimpleSetDecoder<ulong[]>>(factory);
			var anwser = finder.BatteryTest(0.20, 1, 0.005, 10);
			var pipe = anwser.Select(x => (x.Key, x.Value.Select(RetrievalRate).Average())).OrderBy(x => x.Item1);
			PrintData(pipe);
			PrintGraph(pipe);
		}


		public ulong[] GetTableWithRandomMess(int length, double fullness)
		{
			var table = new ulong[length];
			var random = new Random();
			for (int i = 0; i < length * fullness; i++)
			{
				table[i] = (ulong)random.NextInt64();
			}

			random.Shuffle(table);
			return table;
		}

		public static IEnumerable<object[]> GetFullness()
		{
			for (double i = 0; i < 0.5; i += 0.05)
			{
				yield return new object[] { i };
			}

		}


		public void TestSSSAlgorithmMessStability()
		{
			var hashingFunctionKind = HashingFunctionProvider.HashingFunctionKind.MultiplyShift;

			var pipe = GetFullness().Select(
				fullness => ((double)fullness[0],
				new OptimalMultiplierToDataFinder<ulong[], SimpleSetDecoder<ulong[]>>(
					GetBaseTestingConfiguration()
					.SetHashingFunctionFactory((size) =>
					HashingFunctionProvider.GetHashingFunction(hashingFunctionKind, (ulong)size))
					.SetDecoderFactory((table, size, hashingFunctions, toggle) => new SimpleSetDecoder<ulong[]>(hashingFunctions, table, size, toggle))
					.SetNumberOfHashingFunctions(3)
					.SetTableFactory((size) => GetTableWithRandomMess(size, ((double)fullness[0])))
					.SetSizeOfBuffer(1024)
					.Build(2000))
					.BatteryTest(0.20, 1, 0.005, 10)
						.Select(x => (x.Key, x.Value.Average(RetrievalRate)))
						.Where(x => x.Item2 >= 0.9999)
						.Max(x => (x.Key)))
				)
				;

			PrintData(pipe);
			PrintGraph(pipe);
		}

	}
}
