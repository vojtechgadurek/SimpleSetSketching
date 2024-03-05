using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
		[InlineData("CCTCGCCGA", 3)]
		[InlineData("CCTAGCCAAAAG", 5)]
		public void TestKMerSimple(string dna, int k_merSize)
		{
			KMerCreator kMerCreator = new KMerCreator(k_merSize);
			KMerWithComplement kMer = kMerCreator.EmptyKMer;
			for (int i = 0; i < k_merSize; i++)
			{
				kMer = kMerCreator.PushSymbolIn(kMer, kMerCreator.TranslateCharToSymbol(dna[i]));
			}
			for (int i = 0; i < dna.Length - k_merSize; i++)
			{
				string kmerExpected = dna.Substring(i, k_merSize);
				Assert.Equal(kmerExpected, kMerCreator.TranslateKMerToString(kMer.KMer));
				kMer = kMerCreator.PushSymbolIn(kMer, kMerCreator.TranslateCharToSymbol(dna[i]));
			}

		}

		public string KMerRightShiftExpectedBehaviour(string kMer)
		{
			string kMerSuperSet = kMer + 'A';
			return kMerSuperSet.Substring(1, kMerSuperSet.Length - 1);
		}
		[Theory]
		[InlineData("A", "A")]
		public void TestShiftRight(string kMer, string ansver)
		{
			KMerCreator kMerCreator = new KMerCreator(kMer.Length);
			KMerWithComplement kMerWithComplement = kMerCreator.EmptyKMer;
			foreach (char c in kMer)
			{
				kMerWithComplement = kMerCreator.PushSymbolIn(kMerWithComplement, kMerCreator.TranslateCharToSymbol(c));
			}
			//kMerWithComplement = kMerCreator.ShiftRight(kMerWithComplement.KMer.GetBinaryRepresentation());
		}

	}
}
