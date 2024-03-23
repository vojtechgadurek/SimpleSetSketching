using Microsoft.FSharp.Core;
using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.StreamProviders.FastaFileReader;
using SimpleSetSketching.StreamProviders.FastaFileReader.KMerCreators;

namespace Tests
{
	public class KMerTesting
	{
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
			yield return new object[] { "A", "ToTable" };
			yield return new object[] { "ToTable", "A" };
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
		[InlineData("CCTCGCCGA", 3)]
		[InlineData("CCTAGCCAAAAG", 5)]
		public void TestKMerSimple(string dna, int k_merSize)
		{
			KMerCreator kMerCreator = new KMerCreator(k_merSize);
			KMerWithComplement kMer = InitiliazeKMerWithComplement(dna.Substring(0, k_merSize), kMerCreator);

			for (int i = 0; i < dna.Length - k_merSize; i++)
			{
				string kmerExpected = dna.Substring(i, k_merSize);
				Assert.Equal(kmerExpected, kMerCreator.TranslateKMerToString(kMer.KMer));
				kMer = kMerCreator.PushSymbolIn(kMer, kMerCreator.TranslateCharToSymbol(dna[i]));
			}
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
