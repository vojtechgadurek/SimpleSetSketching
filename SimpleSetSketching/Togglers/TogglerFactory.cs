using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Togglers
{
	public class TogglerFactory<TTable>
	{

		public readonly IEnumerable<Func<ValueType[], HashType[], int, HashType[]>> _hashToBufferFunctions;
		public readonly Action<HashType[], ValueType[], int, TTable> _toggleToBufferAction;
		public TogglerFactory
		(
			TTable table,
			HashingFunctions hashFunctions,
			Expression<Action<HashType, ValueType, TTable>> togglingAction
		)
		{
			_hashToBufferFunctions = hashFunctions.
				Select(HashingFunctionProvider.BufferHashingFunction)
				.Select(f => f.Compile())
				.ToList();
			_toggleToBufferAction =
				Toggler<TTable>.BufferToggleFunction(togglingAction).Compile();
		}
		IToggler<TTable> GetToggler(TTable table, int bufferSize)
		{
			return new Toggler<TTable>(bufferSize, table, _hashToBufferFunctions, _toggleToBufferAction);
		}
	}
}
