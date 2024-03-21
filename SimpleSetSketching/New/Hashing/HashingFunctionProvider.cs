using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Hashing
{
	public interface IHashingFunctionProvider
	{
		HashingFunctionExpression GetHashingFunction(ulong size, Random random);
	}

	public static class HashingFunctionProvider
	{
		static Random _random = new Random();
		static Dictionary<HashingFunctionKind, IHashingFunctionProvider> HashingFunctionProviders = new Dictionary<HashingFunctionKind, IHashingFunctionProvider>
		{
			{ HashingFunctionKind.LinearCongruence, new LinearCongruenceHashingFunctionGenerator() },
			{ HashingFunctionKind.MultiplyShift, new MultiplyShiftHashingFunctionGenerator() }
		};
		public enum HashingFunctionKind
		{
			LinearCongruence,
			MultiplyShift
		}
		public static HashingFunctionExpression GetHashingFunction(HashingFunctionKind hashingFunctionKind, ulong size, Random? random = null)
		{
			if (random == null)
			{
				random = HashingFunctionProvider._random;
			}
			return HashingFunctionProviders[hashingFunctionKind].GetHashingFunction(size, random);
		}

		public static Expression<Func<ValueType[], HashType[], int, HashType[]>> GetBufferedHashingFunction(
			HashingFunctionKind hashingFunctionKind, ulong size, Random? random = null)
		{
			return BufferHashingFunction(GetHashingFunction(hashingFunctionKind, size, random));
		}

		public static Expression<Func<ValueType[], HashType[], int, HashType[]>> BufferHashingFunction(HashingFunctionExpression hashingFunction)
		{
			var f = CompiledFunctions.Create<ValueType[], HashType[], int, HashType[]>(
				out var input_,
				out var output_,
				out var size_
				);
			f.S.DeclareVariable<int>(out var i_)
				.While(
					i_.V < size_.V,
					new Scope()
						.Macro(out var input_T, input_.V.ToTable<ulong>())
						.Macro(out var output_T, output_.V.ToTable<ulong>())
						.Function(hashingFunction, input_T[i_.V].V, out var hash_)
						.Assign(output_T[i_.V], hash_)
						.Assign(i_, i_.V + 1)
						)
				.Assign(f.Output, output_.V);
			return f.Construct();
		}
	}
}
