using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
namespace SimpleSetSketching
{
	
	public class ComplexTests
	{
		/// <summary>
		///  Generate triplet, first two being two list containg numbers, last being their symetric difference
		/// </summary>
		/// <param name="numberOfSameElements"></param>
		/// How much elements should be in both sets
		/// <param name="numberOfDifferentElements"></param>
		/// Size of symetric difference of the two sets
		/// <param name="seed"></param>
		/// <returns></returns>
		(IList<int>, IList<int>, HashSet<int>) GenerateRandomSymetricDifferenceData(uint numberOfSameElements, uint numberOfDifferentElements, int seed)
		{
			Random random = new Random(seed);
			//Create two sets one containg elements to be in both sets, one containing elements to be in only one set

			//Create same elements set
			HashSet<int> bothSetsElements = new HashSet<int>();
			while (bothSetsElements.Count < numberOfSameElements)
			{
				bothSetsElements.Add(random.Next());
			}

			//Create other elements set
			HashSet<int> onlyOneSetElements = new HashSet<int>();
			while (onlyOneSetElements.Count < numberOfDifferentElements)
			{
				if (!bothSetsElements.Contains(random.Next()))
				{
					onlyOneSetElements.Add(random.Next());
				}
			}

			//Create sets for symetrical difference
			HashSet<int> firstSet = new HashSet<int>(bothSetsElements);
			HashSet<int> secondSet = new HashSet<int>(bothSetsElements);

			//Splits elements to be in only one set between the two sets
			foreach (var i in onlyOneSetElements)
			{
				if (random.Next(2) == 0)
				{ secondSet.Add(i); }
				else
				{
					firstSet.Add(i);
				}

			}

			var firstShuffled = firstSet.OrderBy(x => random.Next()).ToArray();
			var secondShuffled = secondSet.OrderBy(x => random.Next()).ToArray();
			return (firstShuffled, secondShuffled, onlyOneSetElements);
		}

		public bool TestSketcher(uint numberOfSameElements, uint numberOfDifferentElements, int seed, Action<int> Add, Action<int> Remove, Func<IList<int>> Decode)
		{
			var (firstSet, secondSet, difference) = GenerateRandomSymetricDifferenceData(numberOfSameElements, numberOfDifferentElements, seed);

			foreach (var i in firstSet)
			{
				Add(i);
			}
			foreach (var i in secondSet)
			{
				Remove(i);
			}
			IList<int> decoded = Decode();
			HashSet<int> decodedSet = new HashSet<int>(decoded);
			if (decodedSet.SetEquals(difference))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public void RunTests(uint size)
		{

		}

		public enum DecodingState
		{
			ok,
			shotDown,
			notAbleToDecode,
			badDecoding
		}
		public record struct DecondingToken(long timeTaken, DecodingState state);


		DecondingToken RunOneTest(uint numberOfSameElements, uint numberOfDifferentElements, int seed, uint sizeofsketch, ISketchHashFunction hashFunction)
		{
			var (firstSet, secondSet, symetricDifference) = GenerateRandomSymetricDifferenceData(numberOfSameElements, numberOfDifferentElements, seed);
			var sketcher = new BasicSimpleSetSketcher(sizeofsketch, hashFunction);
			foreach (var i in firstSet)
			{
				sketcher.Toogle(i);
			}
			foreach (var i in secondSet)
			{
				sketcher.Toogle(i);
			}

			DecodedBasicSimpleSetSketcherToken decodedDataToken;

			//Test decoding

			var stopWatch = System.Diagnostics.Stopwatch.StartNew();
			sketcher.TryDecode(out decodedDataToken);
			stopWatch.Stop();
			var elapsedMs = stopWatch.ElapsedMilliseconds;

			//Test if decoding was correct, state is ok and all elements are in symetric difference
			if (decodedDataToken.state == DecodeState.ok &&
				(!decodedDataToken.Data.All(x => symetricDifference.Contains(x)
				|| !(decodedDataToken.Data.Count == symetricDifference.Count)))
				)
			{
				return new DecondingToken(elapsedMs, DecodingState.badDecoding);
			}

			switch (decodedDataToken.state)
			{
				case DecodeState.ok:
					return new DecondingToken(elapsedMs, DecodingState.ok);
				case DecodeState.shotDown:
					return new DecondingToken(elapsedMs, DecodingState.shotDown);
				case DecodeState.notAbleToDecode:
					return new DecondingToken(elapsedMs, DecodingState.notAbleToDecode);
				default:
					throw new Exception("Unknown decoding state");
			}
		}

		public record class TestResult(double sizeOfSketchMultiplicator, List<DecondingToken> data);
		public TestResult RunTests(int numberOfRounds, uint numberOfSameElements, uint numberOfDifferentElements, int seed, double sizeOfSketchMultiplicator, ISketchHashFunction hashFunction)
		{
			uint sizofsketch = (uint)(sizeOfSketchMultiplicator * (numberOfDifferentElements));
			List<DecondingToken> ansver = new List<DecondingToken>();
			for (int i = 0; i < numberOfRounds; i++)
			{
				ansver.Add(RunOneTest(numberOfSameElements, numberOfDifferentElements, seed + i, sizofsketch, hashFunction));
			}
			return new TestResult(sizeOfSketchMultiplicator, ansver);
		}

		public IList<TestResult> TestMultipleMultiplicator(int numberOfRounds, int numberOfTests, double stepSize, double StartMultiplicator, uint numberOfSameElements, uint numberOfDifferentElements, int seed, bool vocal, ISketchHashFunction hashFunction)
		{
			IList<TestResult> ansver = new List<TestResult>();
			for (int i = 0; i < numberOfTests; i++)
			{
				if (vocal)
				{
					Console.WriteLine("Test number " + i + " out of " + numberOfTests);
				}
				ansver.Add(RunTests(numberOfRounds, numberOfSameElements, numberOfDifferentElements, seed + i * numberOfTests, StartMultiplicator + i * stepSize, hashFunction));
			}
			return ansver;

		}

	}
}
*/