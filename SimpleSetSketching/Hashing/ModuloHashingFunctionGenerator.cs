using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Hashing
{
	public class ModuloHashingFunctionGenerator : IHashingFunctionProvider
	{
		public HashingFunctionExpression GetHashingFunction(ulong size, Random random)
		{
			return Create(size, random);
		}

		public static HashingFunctionExpression Create(ulong size, Random random)
		{
			var f = CompiledFunctions.Create<ulong, ulong>(out var input);
			f.S.Assign(f.Output, input.V % size);
			return f.Construct();
		}
	}
}
