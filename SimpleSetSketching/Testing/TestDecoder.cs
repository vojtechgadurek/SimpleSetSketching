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

	public class TestDecoderFactoryConfiguration<TTable, TDecoder>
		where TTable : IEnumerable<ulong>
		where TDecoder : IDecoder<ulong>
	{
		Func<int, TTable>? _tableFactory;
		Func<int, HashingFunctionExpression>? _hashingFunctionFactory;
		Func<TTable, int, HashingFunctions, Expression<Action<ulong, ulong, TTable>>, TDecoder>? _decoderFactory;
		Func<int, ulong[]>? _dataFactory;
		Expression<Action<ulong, ulong, TTable>>? _toggle;
		int _sizeOfBuffer = 1024;
		int? _numberOfHashingFunction = null;

		public TestDecoderFactoryConfiguration<TTable, TDecoder> SetTableFactory(Func<int, TTable> tableFactory)
		{
			_tableFactory = tableFactory;
			return this;
		}

		public TestDecoderFactoryConfiguration<TTable, TDecoder> SetHashingFunctionFactory(Func<int, HashingFunctionExpression> hashingFunctionFactory)
		{
			_hashingFunctionFactory = hashingFunctionFactory;
			return this;
		}

		public TestDecoderFactoryConfiguration<TTable, TDecoder> SetDecoderFactory(Func<TTable, int, HashingFunctions, Expression<Action<ulong, ulong, TTable>>, TDecoder> decoderFactory)
		{
			_decoderFactory = decoderFactory;
			return this;
		}

		public TestDecoderFactoryConfiguration<TTable, TDecoder> SetDataFactory(Func<int, ulong[]> dataFactory)
		{
			_dataFactory = dataFactory;
			return this;
		}

		public TestDecoderFactoryConfiguration<TTable, TDecoder> SetToggle(Expression<Action<ulong, ulong, TTable>> toggle)
		{
			_toggle = toggle;
			return this;
		}

		public TestDecoderFactoryConfiguration<TTable, TDecoder> SetNumberOfHashingFunctions(int numberOfHashingFunction)
		{
			_numberOfHashingFunction = numberOfHashingFunction;
			return this;
		}

		public TestDecoderFactoryConfiguration<TTable, TDecoder> SetSizeOfBuffer(int sizeOfBuffer)
		{
			_sizeOfBuffer = sizeOfBuffer;
			return this;
		}

		public TestDecoderFactory<TTable, TDecoder> Build(int sizeOfTable)
		{
			if (_tableFactory == null || _hashingFunctionFactory == null || _decoderFactory == null || _dataFactory == null || _toggle == null || _numberOfHashingFunction == null)
			{
				throw new InvalidOperationException("All the required fields must be set");
			}
			return new TestDecoderFactory<TTable, TDecoder>(
				_tableFactory,
				_hashingFunctionFactory,
				_toggle,
				_decoderFactory,
				_dataFactory,
				sizeOfTable,
				_numberOfHashingFunction.Value,
				_sizeOfBuffer
				);
		}


	}
	public record class DecodingResult<TTable, TDecoder>(TestDecoderFactory<TTable, TDecoder> Factory, TDecoder Decoder, int NumberOfItemsInSymmetricDifference)
		where TDecoder : IDecoder<ulong> where TTable : IEnumerable<ulong>
		;

	public class TestDecoderFactory<TTable, TDecoder> where TTable : IEnumerable<ulong>
		where TDecoder : IDecoder<ulong>
	{
		readonly Func<int, TTable> _tableFactory;
		readonly Func<int, HashingFunctionExpression> _hashingFunctionFactory;
		readonly Func<TTable, int, HashingFunctions, Expression<Action<ulong, ulong, TTable>>, TDecoder> _decoderFactory;
		readonly Func<int, ulong[]> _dataFactory;
		readonly Expression<Action<ulong, ulong, TTable>> _toggle;

		readonly public int SizeOfTable;
		readonly int? _numberOfHashingFunction = null;
		readonly int? _sizeOfBuffer;

		public TestDecoderFactory(
			Func<int, TTable> tableFactory,
			Func<int, HashingFunctionExpression> hashingFunctionFactory,
			Expression<Action<ulong, ulong, TTable>> toggle,
			Func<TTable, int, HashingFunctions, Expression<Action<ulong, ulong, TTable>>, TDecoder> decoderFactory,
			Func<int, ulong[]> dataFactory,
			int size,
			int numberOfHashingFunction,
			int sizeOfBuffer = 1024
			)
		{
			_tableFactory = tableFactory;
			_hashingFunctionFactory = hashingFunctionFactory;
			_decoderFactory = decoderFactory;
			_dataFactory = dataFactory;
			_toggle = toggle;
			SizeOfTable = size;
			_numberOfHashingFunction = numberOfHashingFunction;
			_sizeOfBuffer = sizeOfBuffer;

		}


		public DecodingResult<TTable, TDecoder> TestOneDecoding(int nOfItemsInSymmetricDifference)
		{
			TTable table = _tableFactory(SizeOfTable);
			HashingFunctions hashingFunctions =
				Enumerable
					.Range(0, (int)_numberOfHashingFunction!)
					.Select(_ => _hashingFunctionFactory(SizeOfTable)).ToList();

			TDecoder decoder = _decoderFactory(table, SizeOfTable, hashingFunctions, _toggle);
			ulong[] data = _dataFactory(nOfItemsInSymmetricDifference);
			ISketchStream<ulong> sketchStream = new ArrayLongStream(data);

			Toggler<TTable> toggler = new Toggler<TTable>((int)_sizeOfBuffer!, table, hashingFunctions, _toggle);

			toggler.ToggleStreamToTable(sketchStream);

			decoder.Decode();
			return new DecodingResult<TTable, TDecoder>(this, decoder, nOfItemsInSymmetricDifference);
		}

		public List<DecodingResult<TTable, TDecoder>> TestMultipleDecoding(int numberOfDecodings, int numberOfItemsInTable)
		{
			return Enumerable.Range(0, numberOfDecodings).Select(_ => TestOneDecoding(numberOfItemsInTable)).ToList();
		}
	}



	public class OptimalMultiplierToDataFinder<TTable, TDecoder>
		where TTable : IEnumerable<ulong>
		where TDecoder : IDecoder<ulong>
	{
		TestDecoderFactory<TTable, TDecoder> _testDecoderFactory;

		public OptimalMultiplierToDataFinder(TestDecoderFactory<TTable, TDecoder> testDecoderFactory)
		{
			_testDecoderFactory = testDecoderFactory;
		}

		public IDictionary<double, IEnumerable<DecodingResult<TTable, TDecoder>>> BatteryTest(
			double startingMultiplier, double endingMultiplier, double step, int numberOfTestsInBattery)
		{
			var results = new Dictionary<double, IEnumerable<DecodingResult<TTable, TDecoder>>>();
			for (double multiplier = startingMultiplier; multiplier < endingMultiplier; multiplier += step)
			{
				int numberOfItems = (int)(multiplier * _testDecoderFactory.SizeOfTable);
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
