using LittleSharp.Callables;
using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tests
{
	internal class TooglerTest
	{
		[Fact]
		public void TestToogling()
		{
			ulong size = 1024;
			int bufferSize = 4;
			var identityHashFunction = LinearCongruenceHashingFunctionGenerator.CreateHashFunction(1, 0, size);
			var hashFunctionList = new List<Expression<Func<ulong, ulong>>> { identityHashFunction };
			ulong[] table = new ulong[size];

			var assignToArray = CompiledActions.Create<ulong, ulong, ulong[]>(out var hash_, out var value_, out var table_);
			assignToArray.S.Macro(out var table_T, table_.V.IsTable<ulong>())
				.Assign(table_T[hash_.V], value_.V);

			var tooglingAction = assignToArray.Construct();


			Toogler<ulong[]> toogler = new Toogler<ulong[]>(bufferSize, table, hashFunctionList, tooglingAction);

			ISketchStream<ulong> stream = new
			toogler.
		}
	}
}
