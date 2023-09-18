global using SimpleSetSketching.Hashing;
global using SimpleSetSketching.Data;
global using SimpleSetSketching.Testing;
global using SimpleSetSketching;
global using SimpleSetSketching.SimpleSetSketchers;

using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Dynamic;
using System.Formats.Asn1;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.AccessControl;
using System.Security.Cryptography;

#nullable enable
public class Program
{
	public static void Main(string[] args)
	{
		int size = 4_000_000; //83380;
		int numberOfRounds = 1;

		/*
		var ansver = TestingFramework.TestWithRandomData(TestingFramework.GetInvertibleBloomFilterProvider((ulong)(size * 1.4), numberOfRounds), numberOfRounds, size * 10, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/

		/*
		var ansver = TestingFramework.TestMultipleDecodings(
			TestingFramework.GetSimpleSketcher_v02_Provider((ulong)(size * 1.4), numberOfRounds),
			TestingFramework.GetFastaFileDataProvider(NamesToFastaFiles.covid11, NamesToFastaFiles.covid12,
			numberOfRounds)
			);
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/

		var ansver = TestingFramework.TestMultipleDecodings(
			TestingFramework.GetInvertibleBloomFilterProvider((ulong)(size * 1.4), numberOfRounds),
			TestingFramework.GetFastaFileDataProvider(NamesToFastaFiles.covid11, NamesToFastaFiles.covid12,
			numberOfRounds)
			);

		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");


		/*
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetSimpleSketcher_v02_Provider((ulong)(size * 1.4), numberOfRounds), numberOfRounds, size * 10, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");

		/*
		var ansver = TestingFramework.TestWithRandomData(TestingFramework.GetSimpleParrallerSketcherProvider((ulong)(size * 1.4), numberOfRounds, 4), numberOfRounds, size * 1000, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");


		/*
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetBasicSketcherProvider((ulong)(size * 1.4), numberOfRounds, new QuickHashing()), numberOfRounds, size * 10, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/
		/*
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetBasicSketcherProvider((ulong)(size * 1.4), numberOfRounds, new Md5Simple()), numberOfRounds, size * 10, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/

	}
}
