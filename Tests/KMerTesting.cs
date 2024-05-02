using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.StreamProviders.FastaFileReader;
using SimpleSetSketching.StreamProviders.FastaFileReader.KMerCreators;
using Xunit.Abstractions;

namespace Tests
{
	public class KMerTesting
	{
		ITestOutputHelper _output;
		public KMerTesting(ITestOutputHelper output)
		{
			_output = output;
		}
		public string GetKmerComplement(string KMer)
		{
			char[] complement = new char[KMer.Length];
			for (int i = 0; i < KMer.Length; i++)
			{
				switch (KMer[KMer.Length - i - 1])
				{
					case 'A':
						complement[i] = 'T';
						break;
					case 'T':
						complement[i] = 'A';
						break;
					case 'C':
						complement[i] = 'G';
						break;
					case 'G':
						complement[i] = 'C';
						break;
					default:
						throw new ArgumentException("Invalid character in KMer");
				}
			}
			return new string(complement);
		}
		public static IEnumerable<object[]> ComplementDataTest()
		{
			yield return new object[] { "A", "T" };
			yield return new object[] { "T", "A" };
			yield return new object[] { "G", "C" };
			yield return new object[] { "C", "G" };
			yield return new object[] { "AAAGGG", "CCCTTT" };
		}
		[Theory]
		[MemberData(nameof(ComplementDataTest))]
		public void TestComplement(string kMer, string complement)
		{
			Assert.Equal(complement, GetKmerComplement(kMer));
		}


		[Theory]
		[InlineData("CCTC", 1)]
		[InlineData("CCCCCCGA", 3)]
		[InlineData("CCCCCCCCAAAAG", 5)]
		public void TestKMerSimple(string dna, int kMerSize)
		{
			KMerCreator kMerCreator = new KMerCreator(kMerSize);
			KMerWithComplement kMer = InitiliazeKMerWithComplement(dna.Substring(0, kMerSize), kMerCreator);

			for (int i = 0; i < dna.Length - kMerSize; i++)
			{
				int index = kMerSize + i;
				string kmerExpected = dna.Substring(i, kMerSize);
				string answer = kMerCreator.TranslateKMerToString(kMer.KMer);
				_output.WriteLine($"{kmerExpected} {answer}");
				Assert.Equal(kmerExpected, answer);
				var symbolPushedIn = kMerCreator.TranslateCharToSymbol(dna[index]);
				kMer = kMerCreator.PushSymbolIn(kMer, symbolPushedIn);
			}
		}

		[Theory]
		[InlineData("CCTC", 1)]
		[InlineData("CCCCCCGA", 3)]
		[InlineData("CCCCCCCCAAAAG", 5)]
		public void TestKMerComplementSimple(string dna, int kMerSize)
		{
			KMerCreator kMerCreator = new KMerCreator(kMerSize);
			KMerWithComplement kMer = InitiliazeKMerWithComplement(dna.Substring(0, kMerSize), kMerCreator);

			for (int i = 0; i < dna.Length - kMerSize; i++)
			{
				int index = kMerSize + i;
				string kmerExpected = GetKmerComplement(dna.Substring(i, kMerSize));
				string answer = kMerCreator.TranslateKMerToString(kMer.Complement);
				_output.WriteLine($"{kmerExpected} {answer}");
				Assert.Equal(kmerExpected, answer);
				var symbolPushedIn = kMerCreator.TranslateCharToSymbol(dna[index]);
				kMer = kMerCreator.PushSymbolIn(kMer, symbolPushedIn);
			}
		}

		[Theory]
		[InlineData("A", "A")]
		[InlineData("C", "C")]
		[InlineData("T", "A")]
		[InlineData("G", "C")]
		[InlineData("AAAA", "AAAA")]
		[InlineData("CCCC", "CCCC")]
		[InlineData("TTTT", "AAAA")]
		[InlineData("GGGG", "CCCC")]
		[InlineData("TTTG", "CAAA")]

		public void TestKMerCanonicalOrder(string kMer, string testCanonical)
		{
			var kMerCreator = new KMerCreator(kMer.Length);
			var value = InitiliazeKMerWithComplement(kMer, kMerCreator);
			var canonical = value.GetCanonical();
			Assert.Equal(kMerCreator.TranslateKMerToString(canonical), testCanonical);
		}

		public KMer InitializeKmer(string kMer, KMerCreator kMerCreator)
		{
			var kMerCharArray = kMer.ToCharArray().Select(kMerCreator.TranslateCharToSymbol).ToArray();
			var value = kMerCreator.InitiliazeKMerFromSymbolArray(kMerCharArray);
			return value;
		}

		public KMerWithComplement InitiliazeKMerWithComplement(string kMer, KMerCreator kMerCreator)
		{
			var kmer = InitializeKmer(kMer, kMerCreator);
			var complement = InitializeKmer(GetKmerComplement(kMer), kMerCreator);
			return new KMerWithComplement(kmer, complement);
		}
		[Theory]
		[InlineData("C")]
		[InlineData("CC")]
		[InlineData("CCT")]
		[InlineData("CCTCAAAAACTG")]
		public void TestInitiliazeKMer(string kMer)
		{
			var kMerCreator = new KMerCreator(kMer.Length);
			var kMerCharArray = kMer.ToCharArray().Select(kMerCreator.TranslateCharToSymbol).ToArray();
			var value = kMerCreator.InitiliazeKMerFromSymbolArray(kMerCharArray);
			string ansver = kMerCreator.TranslateKMerToString(value);
			Assert.Equal(kMer, ansver);
		}

		[Theory]
		[InlineData("A", 'C')]
		[InlineData("C", 'C')]
		[InlineData("C", 'G')]
		[InlineData("CCCCG", 'T')]

		public void TestChangeLastSymbol(string kMerValue, char change)
		{
			string expected = kMerValue.Substring(0, kMerValue.Length - 1) + change;
			var kMerCreator = new KMerCreator(kMerValue.Length);
			var kMer = InitializeKmer(kMerValue, kMerCreator);
			kMer = kMerCreator.ChangeLastSymbol(kMer, kMerCreator.TranslateCharToSymbol(change));
			string ansver = kMerCreator.TranslateKMerToString(kMer);
			Assert.Equal(expected, ansver);
		}

		[Theory]
		[InlineData("A", 'C')]
		[InlineData("C", 'G')]
		[InlineData("C", 'C')]
		[InlineData("GCCCC", 'T')]

		public void TestChangeFirstSymbol(string kMerValue, char change)
		{
			string expected = change + kMerValue.Substring(1, kMerValue.Length - 1);
			var kMerCreator = new KMerCreator(kMerValue.Length);
			var kMer = InitializeKmer(kMerValue, kMerCreator);
			kMer = kMerCreator.ChangeFirstSymbol(kMer, kMerCreator.TranslateCharToSymbol(change));
			string ansver = kMerCreator.TranslateKMerToString(kMer);
			Assert.Equal(expected, ansver);
		}



		public string KMerRightShiftExpectedBehaviour(string kMer)
		{
			string kMerSuperSet = kMer + 'A';
			return kMerSuperSet.Substring(1, kMerSuperSet.Length - 1);
		}
		[Theory]
		[InlineData("A")]
		[InlineData("C")]
		[InlineData("ATAA")]
		[InlineData("GCCC")]
		public void TestRightShift(string kMer)
		{
			string expected = KMerRightShiftExpectedBehaviour(kMer);
			var kMerCreator = new KMerCreator(kMer.Length);
			var kMerValue = InitializeKmer(kMer, kMerCreator);
			kMerValue = kMerCreator.RightShift(kMerValue);
			string ansver = kMerCreator.TranslateKMerToString(kMerValue);
			Assert.Equal(expected, ansver);
		}
		public string KMerLeftShiftExpectedBehaviour(string kMer)
		{
			string kMerSuperSet = 'A' + kMer;
			return kMerSuperSet.Substring(0, kMerSuperSet.Length - 1);
		}

		[Theory]
		[InlineData("A")]
		[InlineData("C")]
		[InlineData("ATAA")]
		[InlineData("CCCG")]
		public void TestLeftShift(string kMer)
		{
			string expected = KMerLeftShiftExpectedBehaviour(kMer);
			var kMerCreator = new KMerCreator(kMer.Length);
			var kMerValue = InitializeKmer(kMer, kMerCreator);
			kMerValue = kMerCreator.LeftShift(kMerValue);
			string ansver = kMerCreator.TranslateKMerToString(kMerValue);
			Assert.Equal(expected, ansver);

		}

	}
}
