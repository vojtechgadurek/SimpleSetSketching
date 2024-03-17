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

		static Expression<Action<ulong, TSet>> AddIfLooksPure<TSet>()
		{

		}
		static void Decode()
	}
}
