using LittleSharp.Callables;
using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tests
{
	public class TogglerTest
	{
		[Fact]
		public void TestToogling()
		{
			// Test toogling by iserting identity hash function and assign toogling function
			// Thus [0, 1, 2, 3, 4] should be [0, 1, 2, 3, 4]
			ulong size = 1025;
			int bufferSize = 4;

			var identityHashFunction = LinearCongruenceHashingFunctionGenerator.CreateHashFunction(1, 0, size);
			var hashFunctionList = new List<Expression<Func<ulong, ulong>>> { identityHashFunction };
			ulong[] table = new ulong[size];

			var assignToArray = CompiledActions.Create<ulong, ulong, ulong[]>(out var hash_, out var value_, out var table_);
			assignToArray.S.Macro(out var table_T, table_.V.ToTable<ulong>())
				.Assign(table_T[hash_.V.Convert<int>()], value_.V);

			var tooglingAction = assignToArray.Construct();

			Toggler<ulong[]> toggler = new Toggler<ulong[]>(bufferSize, table, hashFunctionList, tooglingAction);

			ulong[] input = new ulong[size];
			for (ulong i = 0; i < size; i++)
			{
				input[i] = i;
			}

			ISketchStream<ulong> stream = new ArrayLongStream(input);

			var sketch = toggler.ToggleStreamToTable(stream);
			Assert.Equal(table, sketch);
		}
	}
}
