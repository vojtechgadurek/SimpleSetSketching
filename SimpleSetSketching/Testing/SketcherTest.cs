using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SimpleSetSketching.TestingFramework;

namespace SimpleSetSketching.Testing
{
	public class SketcherTest
	{
		ISketcher sketcher;
		ITestDataProvider dataProvider;
		ILogger<string> logger;
		public TestResult Result { get; private set; }

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

		public SketcherTest SetSketcher(ISketcher sketcher)
		{
			this.sketcher = sketcher;
			return this;
		}
		public SketcherTest SetDataProvider(ITestDataProvider dataProvider)
		{
			this.dataProvider = dataProvider;
			return this;
		}
		public SketcherTest SetLoger(ILogger<string> logger)
		{
			this.logger = logger;
			return this;
		}
		public void Run()
		{
			var startTime = Stopwatch.GetTimestamp();
			var dataToInsert = dataProvider.GetDataToInsert();
			sketcher.Insert(dataToInsert);

			sketcher.Remove(dataProvider.GetDataToRemove());
			var result = sketcher.Decode();
			var endTime = Stopwatch.GetElapsedTime(startTime);

			var expected = dataProvider.GetExpectedResult();
			logger.Log($"Different items: {expected.Length}");

			if (result is null)
			{
				Result = new TestResult(false, endTime);
				return;
			}

			dataProvider.Dispose();
			bool success = result.Count() == expected.Count() && expected.All((x) => result.Contains(x));
			result.SymmetricExceptWith(expected);
			logger.Log($"Difference in result and expected {result.Count}");
			foreach (var item in result.Take(10))
			{
				Console.WriteLine(new K_Mer(item));
			}
			if (result.Count > 10)
			{
				logger.Log("...");
			}

			if (result is null)
			{
				Result = new TestResult(false, endTime);
				return;
			}
		}
	}
}
