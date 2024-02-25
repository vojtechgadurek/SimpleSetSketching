using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	internal class SimpleSetSketcherExTree<TTable, TValue> where TTable : struct, ITable<ulong> where TValue : struct, IValue
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

		private HashSet<ulong> InitiliazeDecoding()
		public HashSet<TValue>? TryDecode()
		{
			HashSet<TValue> ansver = new HashSet<TValue>();
			HashSet<uint> pure = new HashSet<uint>();
			for (uint i = 0; i < _table.Length(); i++)
			{
				AddIfPure(i, pure);
			}
			int hardStop = (int)_table.Length() * _shotdownMultiplicator; //Tohle je nějaká random konstatnta, aby se to necyklilo
			int rounds = 0;
			while (pure.Count > 0)
			{
				rounds++;
				if (rounds > hardStop)
				{
					return null;
				}
				HashSet<uint> nextPure = new HashSet<uint>();
				foreach (var i in pure)
				{
					if (!LooksPure(i))
					{
						continue;
					}
					TValue x = _table.Get(i);
					Toogle(x);
					if (ansver.Contains(x))
					{
						ansver.Remove(x);
					}
					else
					{
						ansver.Add(x);
					}

					AddIfPure(_firstHashingFunction.Hash(x), nextPure);
					AddIfPure(_secondHashingFunction.Hash(x), nextPure);
					AddIfPure(_thirdHashingFunction.Hash(x), nextPure);
				}
				pure = nextPure;
			}

			if (!_table.IsEmpty())
			{
				return null;
			};
			return ansver;
		}

	}
}
