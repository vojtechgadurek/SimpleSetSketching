using LittleSharp.Literals;
using LittleSharp.Utils;
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
	public class Toggler<TTable> : IToggler<TTable>
	{
		int _bufferSize;
		public readonly IEnumerable<Action<ValueType[], HashType[], int, int>> _hashToBufferFunctions;
		public readonly Action<HashType[], ValueType[], int, TTable> _toggleToBufferAction;
		readonly TTable _table;
		public Toggler
		(
			int bufferSize,
			TTable table,
			HashingFunctions hashFunctions,
			Expression<Action<HashType, ValueType, TTable>> togglingAction
		)
		{
			_bufferSize = bufferSize;

			_hashToBufferFunctions = hashFunctions.
				Select(Buffers.BufferFunction)
				.Select(f => f.Compile())
				.ToList();

			_toggleToBufferAction =
				BufferToggleFunction(togglingAction).Compile();
			_table = table;
		}

		public Toggler
		(
			int bufferSize,
			TTable table,
			IEnumerable<Action<ValueType[], HashType[], int, int>> hashToBufferFunctions,
			Action<HashType[], ValueType[], int, TTable> toggleToBufferActions
		)
		{
			_bufferSize = bufferSize;
			_hashToBufferFunctions = hashToBufferFunctions;
			_toggleToBufferAction = toggleToBufferActions;
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
					func(truncatedArray.Array, hashesBuffer, 0, truncatedArray.Size);
					_toggleToBufferAction(hashesBuffer, truncatedArray.Array, truncatedArray.Size, _table);
				}
			}
			return _table;
		}

	}
}
