using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Testing
{
	public static class RandomDataGenerator
	{
		public record class SymmetricDifferenceData(
			ulong[] FirstSet,
			ulong[] SecondSet,
			Set SymmetricDifference,
			int Seed,
			int NumberOfItems,
			int NumberOfItemInSymmetricDifference,
			int NumberOfItemsInFirstSet,
			int NumberOfItemsInSecondSet
			)
		{
			public IEnumerable<ulong> GetFirstSetInRandomOrder()
			{
				return Shuffle(FirstSet, new Random(Seed));
			}
			public IEnumerable<ulong> GetSecondSetInRandomOrder()
			{
				return Shuffle(SecondSet, new Random(Seed + Seed));
			}
		}
		;
		public static void CreateNewTest(SymmetricDifferenceData testingData, string path, string testName)
		{
			void SafeSetToFile(IEnumerable<ulong> set, string path, string name)
			{
				using (StreamWriter file = new(path + name + ".txt"))
				{
					foreach (var i in set)
					{

						file.WriteLine(i);
					}
					file.Close();
				}
			}

			SafeSetToFile(testingData.GetFirstSetInRandomOrder(), path, testingData.Seed + "-firstSet");
			SafeSetToFile(testingData.GetSecondSetInRandomOrder(), path, testingData.Seed + "-secondSet");
			SafeSetToFile(testingData.SymmetricDifference, path, testingData.Seed + "-symmetricDifference");

			void CreateDescriptionFile(SymmetricDifferenceData symmetricDifference)
			{
				using (StreamWriter file = new(path + symmetricDifference.Seed + "-Description" + ".txt"))
				{
					file.WriteLine("Seed\tNumberOfItems\tNumberOfItemsInSymmetricDifference\tNumberOfItemsInFirstSet\tNumberOfItemsInSecondSet");
					file.WriteLine(
						symmetricDifference.Seed
						+ "\t" + symmetricDifference.NumberOfItems
						+ "\t" + symmetricDifference.NumberOfItemInSymmetricDifference
						+ "\t" + symmetricDifference.NumberOfItemsInFirstSet
						+ "\t" + symmetricDifference.NumberOfItemsInSecondSet);
				};

			}
			CreateDescriptionFile(testingData);
		}
		public static ulong GenerateNotNullRandomUInt64(ulong nullValue, Random random)
		{
			ulong randomLong = nullValue;
			while (randomLong == nullValue)
			{
				randomLong = (ulong)random.NextInt64();
			}
			return randomLong;
		}

		public static SymmetricDifferenceData GenerateRandomSymmetricDifferenceData(int numberOfSameElements, int numberOfDifferentElements, ulong nullValue, Random random)
		{
			//Create two sets one contain elements to be in both sets, one containing elements to be in only one set

			//Create same elements set
			Set bothSetsElements = new();
			while (bothSetsElements.Count < numberOfSameElements)
			{
				bothSetsElements.Add(GenerateNotNullRandomUInt64(nullValue, random));
			}

			//Create other elements set
			Set onlyOneSetElements = new();
			while (onlyOneSetElements.Count < numberOfDifferentElements)
			{
				ulong randomUlong = GenerateNotNullRandomUInt64(nullValue, random);
				if (!bothSetsElements.Contains(randomUlong))
				{
					onlyOneSetElements.Add(randomUlong);
				}
			}

			//Create sets for symmetrical difference
			Set firstSet = new(bothSetsElements);
			Set secondSet = new(bothSetsElements);

			//Splits elements to be in only one set between the two sets
			foreach (var i in onlyOneSetElements)
			{
				firstSet.Add(i);

			}
			return new SymmetricDifferenceData(
				firstSet.ToArray(),
				secondSet.ToArray(),
				onlyOneSetElements,
				random.Next(),
				numberOfSameElements + numberOfDifferentElements,
				numberOfDifferentElements,
				firstSet.Count,
				secondSet.Count);
		}

		public static IEnumerable<ulong> Shuffle(IEnumerable<ulong> list, Random random)
		{
			return list.OrderBy(x => random.Next()).ToList();
		}
	}
}
