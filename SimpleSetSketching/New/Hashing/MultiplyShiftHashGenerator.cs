﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Hashing
{
	public static class MultiplyShiftHashGenerator
	{
		public static Expression<Func<ulong, ulong>> CreateHashFunction(ulong multiply, ulong size)
		{
			// (value * multiply) >> (64 - _KMerLength) % _KMerLength
			var multiplyConstant = Expression.Constant(multiply);
			var sizeConstant = Expression.Constant(size);
			var parameterValue = Expression.Parameter(typeof(ulong), "value");
			var multiplyExpression = Expression.Multiply(parameterValue, multiplyConstant);
			var shiftExpression = Expression.RightShift(multiplyExpression, Expression.Constant(BitOperations.TrailingZeroCount(size)));
			var moduloExpression = Expression.Modulo(shiftExpression, sizeConstant);
			var lambda = Expression.Lambda<Func<ulong, ulong>>(moduloExpression, parameterValue);
			return lambda;
		}

		public static ulong DoTrueOperation(ulong value, ulong multiply, ulong size)
		{
			return ((value * multiply) >> BitOperations.TrailingZeroCount(size)) % size;
		}
		public static Func<ulong, ulong> GetTrueOperation(ulong multiply, ulong size)
		{
			return (value) => ((value * multiply) >> BitOperations.TrailingZeroCount(size)) % size;
		}
	}
}