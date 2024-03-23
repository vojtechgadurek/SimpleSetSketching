using LittleSharp.Literals;
using SimpleSetSketching.Hashing;
using SimpleSetSketching.StreamProviders;
using SimpleSetSketching.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Togglers
{
	public class Toggler<TTable> : IToggler<ValueType, HashType, TTable>
	{
		int _bufferSize;
		public readonly IEnumerable<Func<ValueType[], HashType[], int, HashType[]>> _hashToBufferFunctions;
		public readonly Action<HashType[], ValueType[], int, TTable> _toggleToBufferAction;
		readonly TTable _table;
		public Toggler(int bufferSize, TTable table, HashingFunctions hashFunctions, Expression<Action<HashType, ValueType, TTable>> tooglingAction)
		{
			_bufferSize = bufferSize;
			_hashToBufferFunctions = hashFunctions.Select(HashingFunctionProvider.BufferHashingFunction).Select(f => f.Compile()).ToList();
			_toggleToBufferAction = BufferToggleFunction(tooglingAction).Compile();
			_table = table;
		}

		public static Expression<Action<HashType[], ValueType[], int, TTable>> BufferToggleFunction(
			Expression<Action<HashType, ValueType, TTable>> _toggleAction)
		{
			var f = CompiledActions.Create<HashType[], ValueType[], int, TTable>(
					out var hashes_,
					out var corespondingValues_,
					out var numberOfItems_,
					out var table_
			);
			f.S.DeclareVariable<int>(out var i_, 0)
				.Macro(out var hashes_T, hashes_.V.ToTable<ulong>())
				.Macro(out var values_T, corespondingValues_.V.ToTable<ulong>())
				.While(
					i_.V < numberOfItems_.V,
					new Scope().Action(_toggleAction, hashes_T[i_.V].V, values_T[i_.V].V, table_.V)
					.Assign(i_, i_.V + 1)
				);
			return f.Construct();
		}


		public TTable ToggleStreamToTable(
			ISketchStream<ValueType> stream
			)
		{
			ulong[] buffer = new ulong[_bufferSize];
			ulong[] hashesBuffer = new ulong[_bufferSize];
			while (true)
			{
				var truncatedArray = stream.FillBuffer(buffer);
				if (truncatedArray.Size <= 0) break;
				foreach (var func in _hashToBufferFunctions)
				{
					func(truncatedArray.Array, hashesBuffer, truncatedArray.Size);
					_toggleToBufferAction(hashesBuffer, truncatedArray.Array, truncatedArray.Size, _table);
				}
			}
			return _table;
		}

	}
}
