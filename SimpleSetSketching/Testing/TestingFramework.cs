using SimpleSetSketching.Streams;
using SimpleSetSketching.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	static class TestingFramework
	{

		public record struct TestResult
		{
			public bool Success;
			public TimeSpan Time;
			public TestResult(bool success, TimeSpan time)
			{
				Success = success;
				Time = time;
			}

		}

		public static TestResult TestOneDecoding(ISketcher sketcher, ITestDataProvider dataProvider)
		{
			var startTime = Stopwatch.GetTimestamp();
			var dataToInsert = dataProvider.GetDataToInsert();
			sketcher.Insert(dataToInsert);

			sketcher.Remove(dataProvider.GetDataToRemove());
			var result = sketcher.Decode();
			var endTime = Stopwatch.GetElapsedTime(startTime);

			var expected = dataProvider.GetExpectedResult();
			Console.WriteLine($"Different items: {expected.Length}");

			if (result is null)
			{
				return new TestResult(false, endTime);
			}

			dataProvider.Dispose();
			bool success = result.Count() == expected.Count() && expected.All((x) => result.Contains(x));
			result.SymmetricExceptWith(expected);
			Console.WriteLine($"Difference in result and expected {result.Count}");
			foreach (var item in result.Take(10))
			{
				//Console.WriteLine(new K_Mer(item));
			}
			if (result.Count > 0)
			{
				Console.WriteLine("...");
			}
			//Console.WriteLine($"Expected: {expected.Count()}");
			//Console.WriteLine($"Result: {result.Count()}");
			return new TestResult(success, endTime);
		}

		public record struct TestResultCollection(List<TestResult> Results);


		public static TestResultCollection TestMultipleDecodings(IEnumerator<ISketcher> sketcherProvider, IEnumerator<ITestDataProvider> dataProvider)
		{

			var results = new List<TestResult>();
			int count = 0;
			while (sketcherProvider.MoveNext() && dataProvider.MoveNext())
			{
				Console.WriteLine($"TestStarted{count++}");
				results.Add(TestOneDecoding(sketcherProvider.Current, dataProvider.Current));

			}

			return new TestResultCollection(results);
		}

		public static TestResultCollection TestWithRandomData(IEnumerator<ISketcher> sketcherProvider, int numberOfRounds, int numberOfSameItems, int NumberOfDifferentItems, Random random)
		{
			return TestMultipleDecodings(sketcherProvider, RandomTestingDataGenerator.GenerateNTestDataProviders(numberOfRounds, numberOfSameItems, NumberOfDifferentItems, random));
		}

		public static IEnumerator<ISketcher> GetSimpleSketcher_v02_Provider(ulong size, int number)
		{
			for (int i = 0; i < number; i++)
			{
				yield return new SimpleSetSketcher_v02(size);
			}
		}
		public static IEnumerator<ISketcher> GetSimpleSetSketcher_v01_Provider(ulong size, int number, ISketchHashFunction sketchHashFunction)
		{
			for (int i = 0; i < number; i++)
			{
				yield return new SimpleSetSketcher_v01((uint)size, sketchHashFunction);
			}
		}

		public static IEnumerator<ISketcher> GetSimpleParallerSketcherProvider(ulong size, int number, int maxThreads)
		{
			for (int i = 0; i < number; i++)
			{
				yield return new ParallelSetSketcher(size, maxThreads);
			}
		}

		public static IEnumerator<ISketcher> GetInvertibleBloomFilterProvider(ulong size, int number)
		{
			for (int i = 0; i < number; i++)
			{
				yield return new InvertibleBloomFilter(size);
			}
		}

		public static IEnumerator<ITestDataProvider> GetFastaFileDataProvider(string pathToFirstFile, string pathToSecondFile, int number)
		{
			for (int i = 0; i < number; i++)
			{
				yield return new FastaFileComparer(pathToFirstFile, pathToSecondFile);
			}
		}
	}
}
