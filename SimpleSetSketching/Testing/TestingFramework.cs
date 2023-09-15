using SimpleSetSketching.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			if (result is null)
			{
				return new TestResult(false, endTime);
			}
			var expected = dataProvider.GetExpectedResult();
			//Console.WriteLine($"Expected: {expected.Count()}");
			//Console.WriteLine($"Result: {result.Count()}");
			return new TestResult(result.Count() == expected.Count() && expected.All((x) => result.Contains(x)), endTime);
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

		public static IEnumerator<ISketcher> GetBasicSketcherProviderV02(ulong size, int number)
		{
			for (int i = 0; i < number; i++)
			{
				yield return new SimpleSetSketcher(size);
			}
		}
		public static IEnumerator<ISketcher> GetBasicSketcherProvider(ulong size, int number, ISketchHashFunction sketchHashFunction)
		{
			for (int i = 0; i < number; i++)
			{
				yield return new BasicSimpleSetSketcher((uint)size, sketchHashFunction);
			}
		}

	}
}
