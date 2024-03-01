using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimpleSetSketching.New.StreamProviders;
using SimpleSetSketching.New.Utils;
using SimpleSetSketching.New.Utils.ExpressionTrees;
using static SimpleSetSketching.New.Utils.ExpressionTrees.ExpressionApplier;



namespace SimpleSetSketching.New.Tooglers.TooglingFunctions
{
	public static class TooglingFunctionExpressionTrees
	{
		public static Expression Toogling(Expression index, Expression table, Expression value)
		{
			var DoXor = Expression.ExclusiveOrAssign(
				Expression.ArrayAccess(table, index),
				value);
			return DoXor;
		}
		public static Expression<Action<TruncatedArray<ValueType>, TTable>> GetToogleTruncatedArrayToTable<TTable>(
			IEnumerable<Expression<Func<ValueType, HashType>>> hashingFunctions)
		{

			Expression<Action<TableWithValue<TTable, ValueType>>> CreateToogleAction(Expression<Func<ValueType, HashType>> hashingFunction)
			{
				var tableWithValue = Expression.Parameter(typeof(TableWithValue<TTable, ValueType>));
				var table = Expression.Property(tableWithValue, nameof(TableWithValue<TTable, ValueType>.Table));
				var value = Expression.Property(tableWithValue, nameof(TableWithValue<TTable, ValueType>.Value));
				var index = Expression.Invoke(hashingFunction, value);


				var toogle = Toogling(index, table, value);
				var toogleAction = Expression.Lambda<Action<TableWithValue<TTable, ValueType>>>(toogle, tableWithValue);
				return toogleAction;
			}


			//Merge functions to one
			var toogleExpresions = hashingFunctions
				.Select(h => CreateToogleAction(h));
			var merged = ActionMerger(toogleExpresions);

			//Iterate over truncated array

			var truncatedArray = Expression.Parameter(typeof(TruncatedArray<ValueType>));
			var buffer = Expression.Property(truncatedArray, nameof(TruncatedArray<ValueType>.Array));
			var size = Expression.Property(truncatedArray, nameof(TruncatedArray<ValueType>.Size));
			var table = Expression.Parameter(typeof(TTable), "table");

			var var = Expression.Parameter(typeof(uint), "i");
			var condition = Expression.LessThan(var, size);



			var iterateOverTruncatedArray = Iterators.For(
				condition,
				Expression.Block(
					Expression.Invoke(
						merged,
						Expression.New(
							typeof(TableWithValue<TTable, ValueType>).GetConstructors()[0],
							table,
							Expression.ArrayAccess(buffer, var)
							)
						)
					),
				Expression.AddAssign(var, Expression.Constant(1U))
				);

			var ansver = Expression.Lambda<Action<TruncatedArray<ValueType>, TTable>>(iterateOverTruncatedArray, truncatedArray, table);
			return ansver;
		}
	}
}
