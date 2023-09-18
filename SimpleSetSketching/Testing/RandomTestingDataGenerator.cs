using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Testing
{
	public static class RandomTestingDataGenerator
	{
		public class RandomSketchStreamProvider : ISketchStreamProvider<ulong>
		{
			int offset;
			ulong[] data;

			public RandomSketchStreamProvider(ulong[] data)
			{
				this.data = data;
				offset = 0;
			}

			public void Dispose()
			{
				//There is nothing to dispose
			}

			public void FillBuffer(ulong[] buffer, out int maxOffest)
			{
				if (offset + buffer.Length < data.Length)
				{
					Array.Copy(data, offset, buffer, 0, buffer.Length);
					offset += buffer.Length;
					maxOffest = buffer.Length;
				}
				else
				{
					int count = 0;
					while (offset + count < data.Length)
					{
						buffer[count] = data[count + offset];
						count++;
					}
					maxOffest = count;
					while (count < buffer.Length)
					{
						buffer[count] = 0;
						count++;
					}
					offset += count;
				}
			}

			public ulong? Length()
			{
				return (ulong)data.Length;
			}
		}
		public class RandomTestDataProvider : ITestDataProvider
		{
			ulong[] insert;
			ulong[] remove;
			HashSet<ulong> ansvers;
			public RandomTestDataProvider(ulong[] insert, ulong[] remove, HashSet<ulong> ansvers)
			{
				this.insert = insert;
				this.remove = remove;
				this.ansvers = ansvers;
			}

			public void Dispose()
			{
				//Nothing to dispose
			}

			public SketchStream GetDataToInsert()
			{
				return new SketchStream(new RandomSketchStreamProvider(insert), 1024);
			}

			public SketchStream GetDataToRemove()
			{
				return new SketchStream(new RandomSketchStreamProvider(remove), 1024);
			}

			public ulong[] GetExpectedResult()
			{
				return ansvers.ToArray();
			}
		}

		public static IEnumerator<ITestDataProvider> GenerateNTestDataProviders(int number, int numberOfSameElements, int numberOfDifferentElements, Random random)
		{
			for (int i = 0; i < number; i++)
			{
				yield return GenerarateRandomTestDataProvider(numberOfSameElements, numberOfDifferentElements, random);
			}
		}
		public static ulong GenerateRandomUlong(Random random)
		{
			ulong randomLong = 0;
			//Generate some random ulong - mostly like 111..1 not possible
			//But thats not issue
			while (randomLong == 0)
			{

				randomLong = (ulong)random.NextInt64();
			}
			return randomLong;
		}



		public static ITestDataProvider GenerarateRandomTestDataProvider(int numberOfSameElements, int numberOfDifferentElements, Random random)
		{
			var data = GenerateRandomSymetricDifferenceData(numberOfSameElements, numberOfDifferentElements, random);
			return new RandomTestDataProvider(data.Item1, data.Item2, data.Item3);
		}
		public static (ulong[], ulong[], HashSet<ulong>) GenerateRandomSymetricDifferenceData(int numberOfSameElements, int numberOfDifferentElements, Random random)
		{
			//Create two sets one containg elements to be in both sets, one containing elements to be in only one set

			//Create same elements set
			HashSet<ulong> bothSetsElements = new HashSet<ulong>();
			while (bothSetsElements.Count < numberOfSameElements)
			{
				bothSetsElements.Add(GenerateRandomUlong(random));
			}

			//Create other elements set
			HashSet<ulong> onlyOneSetElements = new HashSet<ulong>();
			while (onlyOneSetElements.Count < numberOfDifferentElements)
			{
				ulong randomUlong = GenerateRandomUlong(random);
				if (!bothSetsElements.Contains(randomUlong))
				{
					onlyOneSetElements.Add(randomUlong);
				}
			}

			//Create sets for symetrical difference
			HashSet<ulong> firstSet = new HashSet<ulong>(bothSetsElements);
			HashSet<ulong> secondSet = new HashSet<ulong>(bothSetsElements);

			//Splits elements to be in only one set between the two sets
			foreach (var i in onlyOneSetElements)
			{
				firstSet.Add(i);

			}
			return (firstSet.ToArray(), secondSet.ToArray(), onlyOneSetElements);
		}
	}
}
