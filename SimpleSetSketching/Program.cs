global using SimpleSetSketching.Hashing;
global using SimpleSetSketching.Data;
global using SimpleSetSketching.Testing;
global using SimpleSetSketching;
global using SimpleSetSketching.SimpleSetSketchers;
global using SimpleSetSketchingFSharp;

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
		int numberOfRounds = 10;

		/*
		var ansver = TestingFramework.TestWithRandomData(TestingFramework.GetSimpleSketcher_v02_Provider((ulong)(_lengthOfKMer * 1.4), numberOfRounds), numberOfRounds, _lengthOfKMer * 100, _lengthOfKMer, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/




		/*
		var ansver = TestingFramework.TestMultipleDecodings(
			TestingFramework.GetSimpleSketcher_v02_Provider((ulong)(_lengthOfKMer * 1.4), numberOfRounds),
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
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetSimpleSketcher_v02_Provider((ulong)(_lengthOfKMer * 1.4), numberOfRounds), numberOfRounds, _lengthOfKMer * 10, _lengthOfKMer, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");

		/*
		var ansver = TestingFramework.TestWithRandomData(TestingFramework.GetSimpleParrallerSketcherProvider((ulong)(_lengthOfKMer * 1.4), numberOfRounds, 4), numberOfRounds, _lengthOfKMer * 1000, _lengthOfKMer, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");


		/*
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetBasicSketcherProvider((ulong)(_lengthOfKMer * 1.4), numberOfRounds, new QuickHashing()), numberOfRounds, _lengthOfKMer * 10, _lengthOfKMer, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/
		/*
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetBasicSketcherProvider((ulong)(_lengthOfKMer * 1.4), numberOfRounds, new Md5Simple()), numberOfRounds, _lengthOfKMer * 10, _lengthOfKMer, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/

	}
}
