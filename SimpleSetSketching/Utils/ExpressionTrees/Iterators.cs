﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Utils.ExpressionTrees
{
	public static class Iterators
	{
		public static Expression For(Expression condition, Expression action, Expression increment)
		{
			Expression innerAction = Expression.Block(action, increment);
			return While(condition, innerAction);
		}

		public static Expression ForEach<TTable, TValue>(Expression array, Expression action, Expression item)
		{
			var i = Expression.Parameter(typeof(uint), "i");
			var length = Expression.Parameter(typeof(uint), "length");
			var getLenght = Expression.Assign(length, Expression.Call(array, typeof(TTable).GetMethod("Length")));

			var forExpression = For(
				Expression.LessThan(i, length),
				Expression.Block(
					Expression.Assign(item, Expression.ArrayAccess(array, i)),
					action
					),
				Expression.AddAssign(i, Expression.Constant(1U))
				);
			var ansver = Expression.Block(new ParameterExpression[] { i, length }, getLenght, forExpression);
			return ansver;
		}

		public static Expression While(Expression condition, Expression action)
		{
			var breakLabel = Expression.Label("LoopBreak");
			Expression expression = Expression.Loop(
				Expression.IfThenElse(
					condition,
					action,
					Expression.Break(breakLabel)
					),
				breakLabel
				);
			return expression;
		}
	}
}
