using SimpleSetSketching.New.Tooglers.TooglingFunctions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.New.Utils;
using SimpleSetSketching.New.Utils.ExpressionTrees;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Net.WebSockets;


namespace SimpleSetSketching.New.Decoders
{
	/*
	class Decoder
	{
		public static Expression<Func<TTable, (ulong[], bool)>> TryDecode<TTable, TSet>(HashingFunctions hashingFunctions)
			where TTable : IEnumerable<ValueType>
		{

			List<Expression> block = new List<Expression>();
			List<ParameterExpression> parameters = new List<ParameterExpression>();

			var tableParam = Expression.Parameter(typeof(TTable), "table");
			parameters.Add(tableParam);

			var pureIndexesSet = Expression.Parameter(typeof(TSet), "pureIndexesSet");
			parameters.Add(pureIndexesSet);
			block.Add(
				Expression.Assign(pureIndexesSet, Expression.New(typeof(TSet).GetConstructor(new Type[] { }))
				)
			);

			var tableLenghtParam = Expression.Parameter(typeof(uint));
			parameters.Add(tableLenghtParam);
			block.Add(
				Expression.Assign(tableLenghtParam, Expression.Convert(
					Expression.Call(tableParam, typeof(TTable).GetMethod("Length")), typeof(uint)
					)
				)
			);

			var looksPure = LooksPure<TTable>(hashingFunctions);
			var addIfPure = AddIfPure<TTable, TSet>(looksPure);

			{
				var i = Expression.Parameter(typeof(Va), "");

				block.Add(Iterators.For())
			}
			// Initialize pureIndexesSet

			//Test Sketch is not incorrectly decoded

			var wasSketchDecodedIncorectly = Expression.Invoke(typeof(TTable).GetMethod("All"), ;





		}
		public static Expression DecodeMainLoopAction<TTable, TSet>(
			Expression table,
			Expression pureIndexesSet,
			Expression roundNumber,
			Expression setOfValuesInAnsver,
			Expression<Action<TTable, TSet, HashType>> AddIfPure,
			Expression<Func<Func<TTable, HashType, bool>>> LooksPure,
			Expression<Action<TTable, ValueType>> Toogle,
			HashingFunctions hashingFunctions
		)

		{

			Expression<Action<TTable, TSet, HashType>> TooglePureAnsver(
				Expression<Action<TTable, TSet, HashType>> AddIfPure,
				Expression<Func<Func<TTable, HashType, bool>>> LooksPure,
				Expression<Action<TTable, ValueType>> Toogle,
				HashingFunctions hashingFunctions
				)
			{
				List<ParameterExpression> parameters = new List<ParameterExpression>();

				var tableParam = Expression.Parameter(typeof(TTable), "table");
				parameters.Add(tableParam);

				var setParam = Expression.Parameter(typeof(TSet), "set");
				parameters.Add(setParam);

				var indexParam = Expression.Parameter(typeof(HashType), "index");
				parameters.Add(indexParam);


				List<Expression> block = new List<Expression>();
				var condition = Expression.IfThen(
					Expression.Invoke(LooksPure, tableParam, setParam, indexParam),
					Expression.Block(block)
					);

				var valueCorrespondingToPureIndex = Expression.Parameter(typeof(ValueType));
				parameters.Add(valueCorrespondingToPureIndex);

				block.Add(Expression.Assign(valueCorrespondingToPureIndex, Expression.ArrayAccess(table, indexParam)));
				block.Add(Expression.Invoke(Toogle, tableParam, valueCorrespondingToPureIndex));

				block.Add(Expression.IfThenElse(
					Expression.Call(setParam, typeof(TSet).GetMethod("Contains"), indexParam),
					Expression.Call(setOfValuesInAnsver, typeof(TSet).GetMethod("Add"), valueCorrespondingToPureIndex),
					Expression.Call(setOfValuesInAnsver, typeof(TSet).GetMethod("Remove"), valueCorrespondingToPureIndex)
					)
				);

				var AddIfPureForAllHashingFunctions = hashingFunctions
					.Select(h => Expression.Invoke(h, valueCorrespondingToPureIndex))
					.Select(e => Expression.Invoke(AddIfPure, tableParam, setParam, e))
					;

				block.Add(Expression.Block(AddIfPureForAllHashingFunctions));

				var ansver = Expression.Lambda<Action<TTable, TSet, HashType>>(condition, parameters);
				return ansver;
			}

			//WHILE LOOP
			List<ParameterExpression> parameters = new List<ParameterExpression>();
			List<Expression> block = new List<Expression>();



			block.Add(Expression.AddAssign(roundNumber, Expression.Constant(1)));

			var nextPureIndexesSet = Expression.Parameter(typeof(TSet));
			parameters.Add(nextPureIndexesSet);

			var array = Expression.Call(pureIndexesSet, pureIndexesSet.GetType().GetMethod("ToArray"));

			var possiblePureValueParam = Expression.Parameter(typeof(HashType), "possiblePureValue");
			parameters.Add(possiblePureValueParam);

			var toogleIndexToAnsverIfPure = Expression.Invoke(
				TooglePureAnsver(AddIfPure, LooksPure, Toogle, hashingFunctions), table, nextPureIndexesSet, possiblePureValueParam
				);


			block.Add(Iterators.ForEach<HashType[], HashType>(
				array, toogleIndexToAnsverIfPure, possiblePureValueParam
				)
			);

			block.Add(Expression.Assign(pureIndexesSet, nextPureIndexesSet));

			return







		}
		public static Expression<Func<TTable, HashType, bool>> LooksPure<TTable>(HashingFunctions hashingFunctions)
		{
			var tableParam = Expression.Parameter(typeof(TTable), "table");
			var indexParam = Expression.Parameter(typeof(HashType), "index");
			var value = Expression.Call(tableParam, typeof(TTable).GetMethod("Get"), indexParam);


			var indexes = hashingFunctions
				.Select(h => Expression.Invoke(h, value))
				.Select(i => Expression.Equal(indexParam, i))
				.Aggregate(Expression.OrElse);

			var ansver = Expression.Lambda<Func<TTable, HashType, bool>>(indexes, tableParam, indexParam);
			return ansver;
		}

		public static Expression<Action<TTable, TSet, HashType>> AddIfPure<TTable, TSet>(Expression<Func<TTable, HashType, bool>> looksPure)
		{
			var indexParam = Expression.Parameter(typeof(HashType), "index");
			var setParam = Expression.Parameter(typeof(TSet), "set");
			var tableParam = Expression.Parameter(typeof(TTable), "table");

			var addIfPure = Expression.IfThen(
				Expression.Invoke(looksPure, tableParam, indexParam),
				Expression.Call(setParam, typeof(TSet).GetMethod("Add"), indexParam)
			);

			var ansver = Expression.Lambda<Action<TTable, TSet, HashType>>(addIfPure, tableParam, setParam, indexParam);
			return ansver;
		}
	}	
	*/
}


