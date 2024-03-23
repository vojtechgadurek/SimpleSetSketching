using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Data;
using LittleSharp.Literals;
using BenchmarkDotNet.Attributes;
using Gee.External.Capstone.X86;

namespace SimpleSetSketching.Sketchers
{
	static public class SimpleSetSketchFunc
	{
		public static Expression<Action<ulong, ulong, TTable>> GetXorToggle<TTable>()
		{
			var a = CompiledActions.Create<ulong, ulong, TTable>(out var hash_, out var value_, out var table_);
			a.S.Macro(out var table_T, table_.V.ToTable<ulong>())
				.Assign(table_T[hash_.V], table_T[hash_.V].V ^ value_.V);
			return a.Construct();
		}

		public static Expression<Func<ulong, TTable, bool>> LooksPure<TTable>(HashingFunctions hashingFunctions)
		{
			var f = CompiledFunctions.Create<ulong, TTable, bool>(out var hash_, out var table_);

			f.S.Macro(out var table_T, table_.V.ToTable<ulong>())
				.DeclareVariable(out var value_, table_T[hash_.V].V);
			foreach (var hashFunc in hashingFunctions)
			{
				f.S.Function(hashFunc, value_.V, out var testedHash_)
					.IfThen(testedHash_ == hash_.V,
						new Scope()
						.Assign(f.Output, true)
						.GoToEnd(f.S));
			}
			f.S.Assign(f.Output, false);
			return f.Construct();
		}
		public static Expression<Action<ulong, TSet, TTable>> AddIfLooksPure<TSet, TTable>(Expression<Func<ulong, TTable, bool>> looksPure)
		{
			var a = CompiledActions.Create<ulong, TSet, TTable>(out var hash_, out var set_, out var table_);

			a.S.Macro(out var set_SET, set_.V.ToSet<ulong>())
				.Function(looksPure, hash_.V, table_.V, out var check)
				.IfThen(check, new Scope().Add(set_SET, hash_.V));
			return a.Construct();
		}

		public static Expression<Action<TTable, int, TSet>> Initialize<TTable, TSet>(
			Expression<Action<ulong, TSet, TTable>> AddIfLooksPure
		)
		{
			var a = CompiledActions.Create<TTable, int, TSet>(out var table_, out var size_, out var set_);
			a.S.DeclareVariable<int>(out var i_, 0)
				.Macro(out var table_T, table_.V.ToTable<ulong>())
				.While(
					i_.V < size_.V,
					new Scope()
						.Action(AddIfLooksPure, table_T[i_.V].V, set_.V, table_.V)
						.Assign(i_, i_.V + 1));
			return a.Construct();
		}
		public static Expression<Action<ulong[], int, TSet, TSet, TTable>> OneDecodingStep<TSet, TTable>(
			HashingFunctions hashingFunctions,
			Expression<Action<ulong, ulong, TTable>> Toggle,
			Expression<Func<ulong, TTable, bool>> LooksPure,
			Expression<Action<ulong, TSet, TTable>> AddIfLooksPure
			)
		{
			var f = CompiledActions.Create<ulong[], int, TSet, TSet, TTable>(
				out var pures_, out var numberOfItems_, out var nextStepPures_, out var answerKeys_, out var table_
				);

			f.S.DeclareVariable<int>(out var i_, 0)
				.Macro(out var pures_T, pures_.V.ToTable<ulong>())
				.Macro(out var table_T, table_.V.ToTable<ulong>())
				.Macro(out var answerKeys_SET, answerKeys_.V.ToSet<ulong>())
				.While(i_.V < numberOfItems_.V,
					new Scope()
						.This(out var S)
						.AddFinalizer(new Scope().Assign(i_, i_.V + 1)
						.Macro(out var pures_i, pures_T[i_.V])
						.IfThen(S.Function(LooksPure, pures_i.V, table_.V),
							new Scope().GoToEnd(S)
						)
						.Macro(out var x_, table_T[pures_i.V])
						.Macro(out var _,
							hashingFunctions
							.Select(h => S.Function(h, x_.V))
							.Select(v => S.Action(Toggle, v, x_.V, table_.V)).ToList()
							)
						.IfThenElse(
							answerKeys_SET.Contains(x_.V),
							new Scope().AddExpression(answerKeys_SET.Remove(x_.V)),
							new Scope().AddExpression(answerKeys_SET.Add(x_.V))
							)
						.Macro(out var _,
								hashingFunctions
									.Select(h => S.Function(h, x_.V))
									.Select(v => S.Action(AddIfLooksPure, v, nextStepPures_.V, table_.V)).ToList()
									//This is extremely cursed
									//Actions are added to expression list, thus by explicitly calling .ToList()
									//We are adding these expression into S scope
									)
						)
					);
			return f.Construct();
		}
	}
}
