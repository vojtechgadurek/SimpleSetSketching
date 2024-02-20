using System.Drawing;
using Xunit.Sdk;
using Xunit.Abstractions;
using SimpleSetSketching;
using SimpleSetSketching.Data;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using SimpleSetSketching.Testing;
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
}
