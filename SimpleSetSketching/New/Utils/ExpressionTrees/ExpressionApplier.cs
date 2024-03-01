using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Utils.ExpressionTrees
{
	public static class ExpressionApplier
	{
		public record struct TableWithValue<TTable, TValue>(TTable Table, TValue Value);
		public static Expression<Action<TParameters>> ActionMerger<TParameters>(IEnumerable<Expression<Action<TParameters>>> actions)
		{
			var param = Expression.Parameter(typeof(TParameters), "param");
			var actionsList = actions.Select(a => Expression.Invoke(a, param));
			var block = Expression.Block(actions);
			var ansver = Expression.Lambda<Action<TParameters>>(block, param);
			return ansver;
		}
	}
}
