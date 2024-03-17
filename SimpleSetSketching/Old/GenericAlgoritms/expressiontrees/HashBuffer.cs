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
			// TO REMOVE
			throw new NotImplementedException();
		}
		/// <summary>
		/// Returns expression trees that toogles _value to to provided _table.
		/// </summary>
		/// <typeparam name="TTable"></typeparam> 
		/// <typeparam name="TValue"></typeparam> Type of _value to toogle, it is variable in the returned lambda expression tree.
		/// <param name="hashingFunction"></param> Expected to provide hash of the _value in the range of the _table.
		/// <param name="table"></param> ToTable to toogle to, it is fixed.
		/// <returns></returns>
		public static Expression<Action<TValue>> GetToogleToTable<TTable, TValue>(Expression<Func<TValue, ulong>> hashingFunction, TTable table)
		{
			var TableConst = Expression.Constant(table);
			var ValueParam = Expression.Parameter(typeof(TValue), "_value");
			var hash = Expression.Invoke(hashingFunction, ValueParam);
			var xorToTable = Expression.Call(TableConst, typeof(TTable).GetMethod("Xor"), hash, ValueParam);
			var ansver = Expression.Lambda<Action<TValue>>(xorToTable, ValueParam);
			return ansver;
		}
		/// <summary>
		/// Returns expression, that toogles a _value to provided _table n times, where n is number of hashing functions provided.
		/// </summary>
		/// <typeparam name="TTable"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="hashingFunctions"></param> All hashing functions are expected to provide hash of the _value in the range of the _table.
		/// <param name="table"></param> ToTable to toogle to, it is fixed.
		/// <returns></returns>

		public static Expression<Action<TValue>> GetMultiToogleToTable<TTable, TValue>(IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions, TTable table)
		{
			var ValueParam = Expression.Parameter(typeof(TValue), "_value");
			var hashes = hashingFunctions.Select(h => GetToogleToTable(h, table));

			var ansver = Expression.Lambda<Action<TValue>>(Expression.Block(hashes), ValueParam);
			return ansver;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TTable"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <typeparam name="TBuffer"></typeparam>
		/// <param name="hashingFunctions"></param>
		/// <param name="table"></param>
		/// <returns></returns>
		public static Expression<Action<TBuffer>> GetToogleAllElementsInBufferToTable<TValue, TTable, TBuffer>(
			IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions,
			TTable table
			)
			where TTable : ITable<TValue>
			where TValue : struct
		{
			var buffer = Expression.Parameter(typeof(TValue), "buffer");
			var valuesToAddParam = Expression.Parameter(typeof(TBuffer), "_value");

			var expression = ExpressionTreesIterators.ForEachInArray(buffer, GetMultiToogleToTable(hashingFunctions, table), valuesToAddParam);
			var ansver = Expression.Lambda<Action<TBuffer>>(expression, buffer);
			return ansver;
		}

		public static Expression<Func<ulong, TValue, bool>> GetLooksPureOneHashFunction<TValue>(Expression<Func<TValue, ulong>> hashingFunction)
		{
			var IndexParam = Expression.Parameter(typeof(ulong), "Index");
			var ValueParam = Expression.Parameter(typeof(TValue), "_value");
			var hash = Expression.Invoke(hashingFunction, ValueParam);
			var ansver = Expression.Lambda<Func<ulong, TValue, bool>>(Expression.Equal(hash, IndexParam), IndexParam);
			return ansver;
		}

		public static Expression<Func<ulong, bool>> GetLooksPureExpression<TValue, TTable>(IEnumerable<Expression<Func<TValue, ulong>>> hashingFunctions, TTable table)
		{
			var TableConst = Expression.Constant(table);
			var IndexParam = Expression.Parameter(typeof(ulong), "Index");
			var ValueParam = Expression.Call(Expression.Constant(table), typeof(TTable).GetMethod("GetExpression"), IndexParam);
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

	}
}
