using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	internal class SimpleSetSketcherExTree<TTable, TValue, TBuffer>
		where TTable : struct, ITable<TValue>
		where TValue : struct, IValue<TValue>
	{
		/// <summary>
		/// Generic implementation of SimpleSetSketchFunc algorithm
		/// </summary>
		TTable _table;
		int _shotdownMultiplicator = 10;
		Action<TBuffer> toogle;
		public SimpleSetSketcherExTree(TTable table, IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions)
		{
			_table = table;
			toogle = HashBuffer.GetToogleAllElementsInBufferToTable<TValue, TTable, TBuffer>(hashingFunctions, table).Compile();
		}
		public void Toogle(TBuffer value)
		{
			toogle(value);
		}

		public void Toogle(SketchStream stream)
		{
		}

	}
}
