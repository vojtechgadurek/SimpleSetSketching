using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Runtime;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Diagnostics;
using SimpleSetSketching;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.FSharp.Core;
using SimpleSetSketchingBenchmarking;

namespace MyBenchmarks
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run<TestHashFunctions>();

		}
	}
}