using System.Drawing;
using Xunit.Sdk;
using Xunit.Abstractions;
using SimpleSetSketching;
using SimpleSetSketching.Data;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using SimpleSetSketching.Testing;
using Microsoft.FSharp.Core;
using System.Linq.Expressions;
using SimpleSetSketching.Values;
//fasta soubory .fa
//> TCC  



namespace Tests
{
	public class TestKMer
	{
		private readonly ITestOutputHelper output;

		public TestKMer(ITestOutputHelper output)
		{
			this.output = output;
		}
		[Theory]
		[InlineData("CCTC", 1)]
		[InlineData("CCTCGCCGA", 3)]
		[InlineData("CCTAGCCAAAAG", 5)]
		public void TestKMerSimple(string dna, int k_merSize)
		{
			K_Mer k_Mer = new K_Mer(1, k_merSize);
			for (int i = 0; i < k_merSize; i++)
			{
				k_Mer = k_Mer.PushInNewSymbol(dna[i]);

			}
			for (int i = 0; i < dna.Length - k_merSize; i++)
			{
				string kmerExpected = dna.Substring(i, k_merSize);
				Assert.Equal(kmerExpected, new string(k_Mer.ToString().Reverse().ToArray()));
				k_Mer = k_Mer.PushInNewSymbol(dna[i + k_merSize]);
			}

		}


		[Theory]
		[InlineData("CCTCAAAAACTG", 5)]
		[InlineData("CCTCAAAAAAAAAAAAAAACTG", 5)]
		[InlineData("CCTCAAAAAAAAAAAAAAACTG", 2)]
		[InlineData("CCTCAAAAAAAAAAAAAAACTG", 1)]
		[InlineData("AAAAA", 5)]
		[InlineData("aGaGaGaGaG", 5)]
		[InlineData("aGaGaGaGaGaGaGaGaGaG", 5)]
		[InlineData("GGGcccCCgccGGcccGcAaAAAccCgG", 5)]
		[InlineData("GGGcccCCgccGGcccGcAaAAAccCgG", 1)]
		[InlineData("GGGcccCCgccGGcccGcAaAAAccCgG", 2)]

		public void TestFastaReaderWithMockTextReader(string input, int k_mer)
		{
			string header = $">superstring l={input.Length} k={k_mer}";
			string file = header + "\n" + input;
			TextReader textReader = new StringReader(file);
			FastaFileReader fastaFileReader = new FastaFileReader(textReader, 1024);

			ulong[] buffer = new ulong[1];
			for (int i = 0; i < input.Length - k_mer + 1; i++)
			{
				if (char.IsUpper(input[i]))
				{
					string k_merExpected = input.Substring(i, k_mer).ToUpper();
					fastaFileReader.FillBuffer(buffer, out int _);
					string k_merActual = new string(new K_Mer(buffer[0], k_mer).ToString().Reverse().ToArray());
					Assert.Equal(k_merExpected, k_merActual);
				}
			}

		}

		[Fact]
		public void TestFastaReader()
		{
			SketchStream sketchStream = new SketchStream(new FastaFileReader(new StreamReader(NamesToFastaFiles.covid11), 1024), 1024);
			K_Mer next;
			StreamReader streamReader = new StreamReader(NamesToFastaFiles.covid11_copy);
			streamReader.ReadLine();
			string input = streamReader.ReadLine();
			ulong[] buffer = new ulong[1];
			int k_mer = 11;
			for (int i = 0; i < input.Length - k_mer + 1; i++)
			{
				if (char.IsUpper(input[i]))
				{
					string k_merExpected = input.Substring(i, k_mer).ToUpper();
					sketchStream.FillBuffer(buffer, out int _);
					string k_merActual = new string(new K_Mer(buffer[0], k_mer).ToString().Reverse().ToArray());
					Assert.Equal(k_merExpected, k_merActual);
				}
			}
		}
	}

	public class PerformanceTests
	{
	}

	public class TestExpressionTrees
	{
		[Fact]
		public void TestIterator()
		{
			int x = 0;
			void Add<UlongValue>(UlongValue _)
			{
				x++;
			}
			// var Iterator = HashBuffer.Iterator<UlongValue, ArrayTable<UlongValue>>(new ArrayTable<UlongValue>(), y => );
		}
		[Fact]
		public void TestWhile()
		{
			var parameterTestValue = Expression.Parameter(typeof(int));
			var block = Expression.Block(
				new ParameterExpression[] { parameterTestValue },
				new Expression[] {
					Expression.Assign(parameterTestValue, Expression.Constant(0)),
					HashBuffer.While(
						Expression.LessThan(parameterTestValue, Expression.Constant(10)),
						Expression.Assign(parameterTestValue, Expression.Add(parameterTestValue, Expression.Constant(1)))
					),
					parameterTestValue
				}
				);

			var lambda = Expression.Lambda<Func<int>>(block);
			Func<int> compiled = lambda.Compile();
			int result = compiled.Invoke();

		}
		[Fact]
		public void TestForEach()
		{
			var parameterTestValue = Expression.Parameter(typeof(ulong), "ansver");
			ArrayTable<UlongValue> arrayTable = new ArrayTable<UlongValue>(100);

			ulong expectedResult = 0;

			for (uint i = 0; i < 100; i++)
			{
				arrayTable.Set(i, new UlongValue(i));
				expectedResult += i;
			}
			ParameterExpression variable = Expression.Variable(typeof(UlongValue), "i");
			Expression block = Expression.Block(

				new ParameterExpression[] { parameterTestValue, variable },
				new Expression[]
				{
					Expression.Assign(parameterTestValue, Expression.Constant(0UL)),
					HashBuffer.ForEach<ArrayTable<UlongValue>, UlongValue>(
						arrayTable,
						Expression.AddAssign(parameterTestValue,Expression.Field(variable, "Value")),
						variable
						),
					parameterTestValue
				});
			var lambda = Expression.Lambda<Func<ulong>>(block);
			Func<ulong> compiled = lambda.Compile();
			ulong result = compiled.Invoke();
			Assert.Equal(expectedResult, result);

		}
	}

	public class TestDynamicConstantTypeCreator
	{
		[Fact]
		void SimpleCreationTest()
		{
			Type two = SimpleSetSketching.DynamicConstantTypeCreator<uint>.GetConstant(2);
			var value = two.GetFields();
			Assert.Equal(2u, two.GetField("value").GetValue(null));
		}

	}

	public class TestGenericSimpleSetSketcherCreator
	{
		[Fact]
		public void TestSimpleCreation()
		{
		}


		public class BitValueArrayTest
		{
			[Fact]
			public void TestSetGet()
			{
				BitValueArray bitValueArray = new BitValueArray(100);
				Random random = new Random(0);
				for (int i = 0; i < 100; i++)
				{
					long value = random.Next();
					bitValueArray.Set(i, value);
					Assert.Equal(value, bitValueArray.Get(i));
					bitValueArray.Set(i, 0);
				}
			}
			[Fact]
			public void TestXor()
			{
				BitValueArray bitValueArray = new BitValueArray(100);
				Random random = new Random(0);
				for (int i = 0; i < 100; i++)
				{
					long value = random.Next();
					bitValueArray.Set(i, value);
					Assert.Equal(value, bitValueArray.Get(i));
					bitValueArray.Set(i, value);
				}
			}
		}

		public class TestHashing
		{
			[Fact]
			public void SimpleTest()
			{
				Random random = new Random(42);
				for (int i = 0; i < 100; i++)
				{
					(ulong, uint) key = ((ulong)random.NextInt64(), (uint)random.Next());
					ulong valueToBeHashed = (ulong)random.NextInt64();
					var val = MultiplyShiftHashGenerator.CreateHashFunction(key.Item1, key.Item2).Compile()(valueToBeHashed);
					var expected = MultiplyShiftHashGenerator.GetTrueOperation(key.Item1, key.Item2).Invoke(valueToBeHashed);
					Assert.Equal(expected, val);

				}
			}

			[Fact]
			/// <summary>
			/// Tests if <code>GetHashingFunctionApplier</code> returns correct function
			public void TestHashBuffer()
			{

				ulong[] buffer = GenerateRandomULongArray.GenerateRandomULongArrayFunc(100, 42);
				ulong[] expectedResult = new ulong[100];
				ulong[] result = new ulong[100];

				//Some function ulong -> ulong is needed, so we will use simple multiplication by 2
				Expression<Func<ulong, ulong>> hash = (a) => 2 * a;

				var expr = HashBuffer.GetHashingFunctionApplier(hash);
				var expr_compiled = expr.Compile();
				expr_compiled.Invoke(buffer, 100, result);


				var hashCompiled = hash.Compile();
				for (int i = 0; i < 100; i++)
				{
					expectedResult[i] = hashCompiled(buffer[i]);
				}
				Assert.Equal(expectedResult, result);
			}
		}



	}
}
