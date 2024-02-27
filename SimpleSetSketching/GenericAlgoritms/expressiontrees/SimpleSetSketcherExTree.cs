using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.Tables;

namespace SimpleSetSketching
{
	internal class SimpleSetSketcherExTree<TTable, TValue> where TTable : struct, ITable<ulong> where TValue : struct, IValue<TValue>
	{
		/// <summary>
		/// Generic implementation of SimpleSetSketch algorithm
		/// </summary>
		TTable _table;
		int _shotdownMultiplicator = 10;
		public Action<TValue[], int> Toogle;

		public SimpleSetSketcherExTree(TTable table, IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions)
		{
			_table = table;
			Toogle = HashBuffer.GetToogleBufferToTableExpressionTree(hashingFunctions, table).Compile();
		}

	}
}
