using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Data;

namespace SimpleSetSketching.New.Sketchers
{
	static public class SimpleSetSketching
	{
		static Expression<Action<ulong, ulong, TTable>> GetXorToogle<TTable>()
		{
			var a = CompiledActions.Create<ulong, ulong, TTable>(out var hash_, out var value_, out var table_);
			a.S.Macro(out var table_T, table_.V.ToTable<ulong>())
				.Assign(table_T[hash_.V], table_T[hash_.V].V ^ value_.V);
			return a.Construct();
		}

		static Expression<Func<ulong, TTable, bool>> LooksPure<TTable>(HashingFunctions hashingFunctions)
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
		static Expression<Action<double, TTable>> Toogle<TTable>()
		{

		}
		static Expression<Action<ulong, TSet, TTable>> AddIfLooksPure<TSet, TTable>(Expression<Func<ulong, TTable, bool>> looksPure)
		{
			var l = new Lambda();




			var a = CompiledActions.Create<ulong, TSet, TTable>(out var hash_, out var set_, out var table_);
			a.S.Macro(out var set_SET, set_.V.ToSet<ulong>())
				.Function(looksPure, hash_.V, table_.V, out var check)
				.IfThen(check, new Scope().Add(set_SET, hash_.V))
				.DeclareVariable<int>(out var i_, 0)
				.While(i_.V < 10, new Scope().;
			return a.Construct();
		}

		public static (HashSet<ulong>, HashSet<ulong>) Initialize<TTable>(Action<ulong, HashSet<ulong>, TTable> AddIfLooksPure, TTable table, int size)
		{
			HashSet<ulong> ansver = new HashSet<ulong>();
			HashSet<ulong> pure = new HashSet<ulong>();
			for (ulong i = 0; i < (ulong)size; i++)
			{

			}

		}

		static void OneAction<TSet, TTable>(Expression<Func<ulong, TTable, bool>> looksPure, Expression<Action<ulong, TTable>> Toogle)
		{
			var f = CompiledActions.Create<ulong, TSet, TSet, TSet, TTable>(out var testedHash_, out var pure_, out var nextPure_, out var ansver_, out var table_t);

			f.S.Function(looksPure, testedHash_.V, table_t.V, out var check)
				.IfThen(!check, new Scope().GoToEnd(f.S))
				.Macro(out var x_, table_t.V.ToTable<ulong>()[testedHash_.V])
				.Action(Toogle, x_.V, table_t.V)
				.Macro(out var ansver_SET, ansver_.V.ToSet<ulong>())
				.IfThenElse(ansver_SET.Contains(x_.V), new Scope());



			HashSet<ulong> nextPure = new HashSet<ulong>();
			foreach (var i in pure)
			{
				if (!LooksPure(i, copydata))
				{
					continue;
				}
				var x = copydata[i];
				Toogle(x, copydata);
				if (ansver.Contains(x))
				{
					ansver.Remove(x);
				}
				else
				{
					ansver.Add(x);
				}
				(ulong, ulong, ulong) hash = GetTruncatedHash(x);
				void AddIfPure(ulong index)
				{
					if (LooksPure(index, copydata))
					{
						nextPure.Add(index);
					}
				}
				AddIfPure(hash.Item1);
				AddIfPure(hash.Item2);
				AddIfPure(hash.Item3);
			}
			pure = nextPure;
		}

		static void Decode()
		{

		}
	}
}
