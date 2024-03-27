using LittleSharp.Literals;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Testing
{
	public record class DecodingResult<TTable, TDecoder>(TestDecoderFactory<TTable, TDecoder> factory, TDecoder Decoder)
		where TDecoder : IDecoder<TTable> where TTable : IEnumerable<ulong>
		;

	public class TestDecoderFactory<TTable, TDecoder> where TTable : IEnumerable<ulong>
		where TDecoder : IDecoder<TTable>
	{
		readonly Func<int, TTable> _tableFactory;
		readonly Func<int, HashingFunctionExpression> _hashingFunctionFactory;
		readonly Func<TTable, int, HashingFunctions, TDecoder> _decoderFactory;
		readonly Func<int, ulong[]> _dataFactory;
		readonly Expression<Action<ulong, ulong, TTable>> _toggle;

		readonly public int Size;
		readonly int _numberOfHashingFunction;

		public TestDecoderFactory(
			Func<int, TTable> tableFactory,
			Func<int, HashingFunctionExpression> hashingFunctionFactory,
			Expression<Action<ulong, ulong, TTable>> toggle,
			Func<TTable, int, HashingFunctions, TDecoder> decoderFactory,
			Func<int, ulong[]> dataFactory
			)
		{
			_tableFactory = tableFactory;
			_hashingFunctionFactory = hashingFunctionFactory;
			_decoderFactory = decoderFactory;
			_dataFactory = dataFactory;
			_toggle = toggle;
		}


		public DecodingResult<TTable, TDecoder> TestOneDecoding(int numberOfItems)
		{
			TTable table = _tableFactory(Size);
			HashingFunctions hashingFunctions = Enumerable.Range(0, _numberOfHashingFunction).Select(_ => _hashingFunctionFactory(Size));
			TDecoder decoder = _decoderFactory(table, Size, hashingFunctions);
			ulong[] data = _dataFactory(numberOfItems);
			ISketchStream<ulong> sketchStream = new ArrayLongStream(data);

			Toggler<TTable> toggler = new Toggler<TTable>(Size, table, hashingFunctions, _toggle);
			toggler.ToggleStreamToTable(sketchStream);
			decoder.Decode();
			return new DecodingResult<TTable, TDecoder>(this, decoder);
		}

		public List<DecodingResult<TTable, TDecoder>> TestMultipleDecoding(int numberOfDecodings, int numberOfItemsInTable)
		{
			return Enumerable.Range(0, numberOfDecodings).Select(_ => TestOneDecoding(numberOfItemsInTable)).ToList();
		}
	}



	public class OptimalMultiplierToDataFinder<TTable, TDecoder>
		where TTable : IEnumerable<ulong>
		where TDecoder : IDecoder<TTable>
	{
		TestDecoderFactory<TTable, TDecoder> _testDecoderFactory;

		public OptimalMultiplierToDataFinder(TestDecoderFactory<TTable, TDecoder> testDecoderFactory)
		{
			_testDecoderFactory = testDecoderFactory;
		}

		public IDictionary<double, IEnumerable<DecodingResult<TTable, TDecoder>>> BatteryTest(double startingMultiplier, double endingMultiplier, double step, int numberOfTestsInBattery)
		{
			var results = new Dictionary<double, IEnumerable<DecodingResult<TTable, TDecoder>>>();
			for (double multiplier = startingMultiplier; multiplier < endingMultiplier; multiplier += step)
			{
				int numberOfItems = (int)(multiplier * _testDecoderFactory.Size);
				var decodingResults = _testDecoderFactory.TestMultipleDecoding(numberOfTestsInBattery, numberOfItems);
				results[multiplier] = decodingResults;
			}
			return results;
		}
	}

	struct DecodeData
	{
		ulong[] data;
	}


	interface IDecodeDataProvider
	{
	}
}
