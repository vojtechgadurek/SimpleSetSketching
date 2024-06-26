﻿using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Runtime;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Diagnostics;
using SimpleSetSketching;
using System.Numerics;
using System.Runtime.CompilerServices;
using SimpleSetSketchingBenchmarking;
using Gee.External.Capstone.X86;

namespace MyBenchmarks
{
	public class Program
	{
		public static void Main(string[] args)
		{
			HashSet<string> argsSet = new HashSet<string>(args);
			if (argsSet.Contains("toggler"))
			{
				var summary = BenchmarkRunner.Run<BenchmarkToogling>();
			}
		}
	}
}