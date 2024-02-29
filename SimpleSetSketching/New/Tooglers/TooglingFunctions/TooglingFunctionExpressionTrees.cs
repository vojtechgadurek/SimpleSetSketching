using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.New.StreamProviders;
using SimpleSetSketching.New.Utils;
using static SimpleSetSketching.New.Utils.ExpressionTrees.ExpressionApplier;
using Table = ExpressionApplier;
TableWithValue<ulong, ulong>;


namespace SimpleSetSketching.New.Tooglers.TooglingFunctions
{
	public class TooglingFunctionExpressionTrees
	{
		public static Expression<Action<TableWithValue<TTable, TValue>>> GetToogleToTable<TTable, TValue>(Expression<Func<TValue, ulong>> hashingFunction)
		{
			var TableConst = Expression.Parameter(typeof(TTable), "table");
			var ValueParam = Expression.Parameter(typeof(TValue), "value");
			var hash = Expression.Invoke(hashingFunction, ValueParam);
			var xorToTable = Expression.Call(TableConst, typeof(TTable).GetMethod("Xor"), hash, ValueParam);
			var ansver = Expression.Lambda<Action<TableWithValue<TTable, TValue>>>(xorToTable, ValueParam);
			return ansver;
		}
	}
}
