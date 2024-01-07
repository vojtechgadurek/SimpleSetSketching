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
		ISketcher? sketcher;
		ITestDataProvider? dataProvider;
		ILogger<string>? logger;
		public TestResult Result { get; private set; }

		public record struct TestResult
		{
			public int? Diferences;
			public bool Decoded;
			public TimeSpan Time;
			public TestResult(int? diferences, bool decoded, TimeSpan time)
			{
				Diferences = diferences;
				Time = time;
				Decoded = decoded;
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
			if (sketcher is null)
			{
				throw new InvalidOperationException("Sketcher is not set");
			}
			if (dataProvider is null)
			{
				throw new InvalidOperationException("DataProvider is not set");
			}
			if (logger is null)
			{
				throw new InvalidOperationException("Logger is not set");
			}

			//Start testing
			var startTime = Stopwatch.GetTimestamp();
			var dataToInsert = dataProvider.GetDataToInsert();

			//Insert data
			sketcher.Insert(dataToInsert);

			//Remove data
			sketcher.Remove(dataProvider.GetDataToRemove());

			//Try decode
			var result = sketcher.Decode();

			//Stop testing
			var endTime = Stopwatch.GetElapsedTime(startTime);

			//Chceck result and expected data
			var expected = dataProvider.GetExpectedResult();
			logger.Log($"Different items: {expected.Length}");

			if (result is null)
			{
				Result = new TestResult(null, false, endTime);
				return;
			}

			dataProvider.Dispose();
			bool success = result.Count() == expected.Count() && expected.All((x) => result.Contains(x));
			result.SymmetricExceptWith(expected);
			logger.Log($"Difference in result and expected {result.Count}");

			Result = new TestResult(result.Count, true, endTime);
		}
	}
}
