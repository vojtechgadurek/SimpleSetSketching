using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public static class HashBuffer
	{
		public static Expression<Action<ulong[], int, ulong[]>> GetHashingFunctionApplier(Expression<Func<ulong, ulong>> hashingFunction)
		{

			ParameterExpression paramToBeHashedBuffer = Expression.Parameter(typeof(ulong[]), "toBeHashedBuffer");
			ParameterExpression paramResult = Expression.Parameter(typeof(ulong[]), "result");
			ParameterExpression paramItemsToHash = Expression.Parameter(typeof(int), "itemsToHash");

			var breakLabel = Expression.Label("LoopBreak");

			//Build cycle
			var expression = Expression.Block(
				new Expression[]
				{
					Expression.Loop(
						Expression.Block(
							Expression.Assign(
										paramItemsToHash,
										Expression.Subtract(paramItemsToHash, Expression.Constant(1))
									),
							Expression.IfThenElse(
								Expression.LessThan(paramItemsToHash, Expression.Constant(0)),
								Expression.Break(breakLabel),
								Expression.Block(
									Expression.Assign(
										Expression.ArrayAccess(
											paramResult,
											paramItemsToHash
										),
										Expression.Invoke(
											hashingFunction,
											Expression.ArrayAccess(
												paramToBeHashedBuffer,
												paramItemsToHash
											)
										)
									)
								)
							)
						),
						breakLabel
					)
				}

				);

			var ansver = Expression.Lambda<Action<ulong[], int, ulong[]>>(expression, paramToBeHashedBuffer, paramItemsToHash, paramResult);
			return ansver;

		}
		public static Expression<Action<TValue>> GetToogleToTable<TTable, TValue>(Expression<Func<TValue, ulong>> hashingFunction, TTable table)
		{
			var TableConst = Expression.Constant(table);
			var ValueParam = Expression.Parameter(typeof(TValue), "value");
			var hash = Expression.Invoke(hashingFunction, ValueParam);
			var xorToTable = Expression.Call(TableConst, typeof(TTable).GetMethod("Xor"), hash, ValueParam);
			var ansver = Expression.Lambda<Action<TValue>>(xorToTable, ValueParam);
			return ansver;
		}

		public static Expression<Action<TValue>> GetMultiToogleToTable<TTable, TValue>(IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions, TTable table)
		{
			var ValueParam = Expression.Parameter(typeof(TValue), "value");
			var hashes = hashingFunctions.Select(h => GetToogleToTable(h, table));

			var ansver = Expression.Lambda<Action<TValue>>(Expression.Block(hashes), ValueParam);
			return ansver;
		}


		public static Expression<Action<TValue[], int>> GetToogleBufferToTableExpressionTree<TTable, TValue>(
			IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions,
			TTable table
			)
		{
			var TableParam = Expression.Parameter(typeof(TTable), "table");
			var valuesToAddParam = Expression.Parameter(typeof(TValue[]), "value");
			var sizeParam = Expression.Parameter(typeof(int), "size");

			var breakLabel = Expression.Label("LoopBreak");


			var expression = Expression.Block(
				new Expression[]
				{
					Expression.Loop(
						Expression.Block(
							Expression.Assign(
										sizeParam,
										Expression.Subtract(sizeParam, Expression.Constant(1))
									),
							Expression.IfThenElse(
								Expression.LessThan(sizeParam, Expression.Constant(0)),
								Expression.Break(breakLabel),
								Expression.Invoke(GetMultiToogleToTable(hashingFunctions, table), Expression.ArrayAccess(valuesToAddParam, sizeParam))

								)
							),
						breakLabel
						)
				}
			);

			var ansver = Expression.Lambda<Action<TValue[], int>>(expression, valuesToAddParam, sizeParam);
			return ansver;
		}

		public static Expression<Func<ulong, TValue, bool>> GetLooksPureOneHashFunction<TValue>(Expression<Func<TValue, ulong>> hashingFunction)
		{
			var IndexParam = Expression.Parameter(typeof(ulong), "Index");
			var ValueParam = Expression.Parameter(typeof(TValue), "value");
			var hash = Expression.Invoke(hashingFunction, ValueParam);
			var ansver = Expression.Lambda<Func<ulong, TValue, bool>>(Expression.Equal(hash, IndexParam), IndexParam);
			return ansver;
		}

		public static Expression<Func<ulong, bool>> GetLooksPureExpression<TValue, TTable>(IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions, TTable table)
		{
			var TableConst = Expression.Constant(table);
			var IndexParam = Expression.Parameter(typeof(ulong), "Index");
			var ValueParam = Expression.Call(Expression.Constant(table), typeof(TTable).GetMethod("Get"), IndexParam);
			var purelooking =
				hashingFunctions
				.Select(h => GetLooksPureOneHashFunction(h))
				.Select(h => Expression.Invoke(h, IndexParam, ValueParam))
				.Cast<Expression>()
				.Aggregate(Expression.OrElse);
			var ansver = Expression.Lambda<Func<ulong, bool>>(purelooking, IndexParam);
			return ansver;

		}

		public static Expression<Action<ulong, HashSet<ulong>>> GetAddIfPureExpression<TTable, TValue>(Expression<Func<ulong, bool>> looksPureExpression)
		{
			var IndexParam = Expression.Parameter(typeof(ulong), "Index");
			var HashSetParam = Expression.Parameter(typeof(HashSet<ulong>), "HashSet");
			var addIfPure = Expression.IfThen(
				Expression.Invoke(looksPureExpression, IndexParam),
				Expression.Call(HashSetParam, typeof(HashSet<ulong>).GetMethod("Add"), IndexParam)
			);
			var ansver = Expression.Lambda<Action<ulong, HashSet<ulong>>>(addIfPure, IndexParam, HashSetParam);
			return ansver;
		}

		public static LoopExpression RangeFromTo(int from, int to, Expression<Action<int>> action)
		{
			var fromParam = Expression.Constant(from);
			var toParam = Expression.Constant(to);
			var i = Expression.Parameter(typeof(int), "i");
			var iValue = Expression.Assign(i, fromParam);
			var breakLabel = Expression.Label("LoopBreak");
			var expression =
				Expression.Loop(
					Expression.Block(
						Expression.IfThen(
							Expression.GreaterThan(iValue, toParam),
							Expression.Break(breakLabel)
							),
						Expression.Invoke(action, iValue),
						Expression.Assign(iValue, Expression.Add(iValue, Expression.Constant(1)))
						),
					breakLabel
					);
			return expression;
		}

		public static Expression<Func<HashSet<ulong>>> GetInitializer<TTable, TValue>
			(TTable table, Expression<Action<ulong, HashSet<ulong>>> AddIfPureExpression)
			where TTable : ITable<TValue>
			where TValue : struct
		{
			var TableConst = Expression.Constant(table);
			var HashSetVar = Expression.Variable(typeof(HashSet<ulong>), "HashSet");
			var HashSetNew = Expression.New(typeof(HashSet<ulong>));
			var HashSetAssign = Expression.Assign(HashSetVar, HashSetNew);
			var action = Expression.Lambda<Action<int>>();

			var Range = RangeFromTo(0, (int)table.Length(), AddIfPureExpression);
			var ansver = Expression.Lambda<Func<HashSet<ulong>>>(expression);
			return ansver;


		}
	}
}
