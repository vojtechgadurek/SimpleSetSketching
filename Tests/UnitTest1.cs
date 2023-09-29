using System.Drawing;
using Xunit.Sdk;
using Xunit.Abstractions;
using SimpleSetSketching;
using SimpleSetSketching.Data;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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
				Assert.Equal(kmerExpected, new string (k_Mer.ToString().Reverse().ToArray()));
				k_Mer = k_Mer.PushInNewSymbol(dna[i + k_merSize]);
			}

		}
		[Fact]
		public void TestFastaReader()
		{
			void Push(char[] chars, char c)
			{
				for (int i = chars.Length - 1; i > 0; i--)
				{
					chars[i] = chars[i - 1];
				}
				chars[0] = c;
			}
			SketchStream sketchStream = new SketchStream(new FastaFileReader(NamesToFastaFiles.covid11, 1024), 1024);
			K_Mer next;
			StreamReader streamReader = new StreamReader(NamesToFastaFiles.covid11_copy);
			streamReader.ReadLine();
			char[] kmer = new char[31];
			for (int i = kmer.Length - 2; i >= 0; i--)
			{
				kmer[i] = char.ToUpper((char)streamReader.Read());
			}
			int count = 0;
			while (true)
			{
				count++;
				next = new K_Mer(sketchStream.Next(), 31);
				if (next.data == 0)
				{
					break;
				}
				while (true)
				{
					char c = (char)streamReader.Read();
					Push(kmer, char.ToUpper(c));
					if (char.IsUpper(c))
					{
						if (new string(kmer) != next.ToString())
						{
							output.WriteLine(count.ToString());
						}
						Assert.Equal(new String(kmer), next.ToString());
						break;
					}

				}
			}


		}
	}


	public class OneValueTest
	{
		/*
		[Fact]
		public void Test1()
		{
			var ketcher = new BasicSimpleSetSketcher(16, new Md5Simple());
			ketcher.Toogle(1);
			var ansver = ketcher.Decode();
			Assert.Equal(1, ansver.Data.Count);
			Assert.Contains(1, ansver.Data);
		}
		[Theory]
		[InlineData(4)]
		[InlineData(10)]
		[InlineData(100)]
		public void TestFailRange(uint range)
		{
			var sketcher = new BasicSimpleSetSketcher(range, new Md5Simple());
			for (int i = 1; i < 10; i++)
			{
				sketcher.Toogle(i);
			}
			for (int i = 2; i < 11; i++)
			{
				sketcher.Toogle(i);
			}
			var ansver = sketcher.Decode();
			Assert.Contains(1, ansver.Data);
			Assert.Contains(10, ansver.Data);
		}
	}
	public class LongTest
	{
		[Theory]
		[InlineData(100, 10, 20)]
		[InlineData(100, 10, 10)]
		[InlineData(100, 10, 8)]
		[InlineData(1000, 100, 180)]
		[InlineData(1000, 100, 200)]
		[InlineData(1000, 100, 300)]
		[InlineData(1000, 100, 400)]
		public void TestRandomData(uint size, uint different, uint sizeofsketch)
		{
			Random random = new Random(42);


			ComplexTests comtest = new ComplexTests();
			var ketcher = new BasicSimpleSetSketcher(sizeofsketch, new Md5Simple());



			Assert.True(comtest.TestSketcher(size, different, 42, (input) => ketcher.Toogle(input), (input) => ketcher.Toogle(input), () => ketcher.Decode().Data));

		}
	}
	*/
	}
}
