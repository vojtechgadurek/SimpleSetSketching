using Microsoft.Diagnostics.Tracing.Stacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Gee.External.Capstone.X86;
using BenchmarkDotNet.Validators;

namespace SimpleSetSketching.InvertiblBloomLookupTable
{
	public interface IHypergraphTable<TValue>
	{
		TValue Get(int index);
		int Count { get; }
		int NumberOfHashedItems(int index);
	}
	public class HyperGraphDecoder<TTable> : IDecoder<TTable> where TTable : IHypergraphTable<ulong>
	{
		TTable table;
		readonly public Expression<Func<int, TTable, bool>> IsPure;
		readonly public Expression<Action<ulong, TTable>> Remove;
		readonly public Expression<Action<ulong, TTable>> Add;

		readonly public Expression<Action<int, TTable, List<int>>> RemoveAndAddIfPure;

		public DecodingState State => DecodingState.NotStarted;

		public HyperGraphDecoder()
		{


		}

		void BuildDecode()
		{
			var f = CompiledActions.Create<TTable, List<int>>(out var table_, out var pure_);
			f.S.While(
			pure_.V.Call<int>("Count") > 0,

				new Scope()
					.This(out var S)
					//Get the last element of pure list
					.DeclareVariable(
						out var pureBucketIndex,
						pure_.V.Call<int>("Get", pure_.V.Call<int>("Count") - 1)
					)
					//Remove last element of pure list
					.AddExpression(pure_.V.Call<int>("RemoveAt", pure_.V.Call<int>("Count") - 1))
					.IfThen(
						S.Function(IsPure, pureBucketIndex.V, table_.V),
						new Scope().GoToEnd(S))
					.Action(RemoveAndAddIfPure, pureBucketIndex.V, table_.V, pure_.V)
			);
		}

		(List<ulong>, bool) GetValues()
		{
			List<ulong> values = new List<ulong>();
			bool decodingFailed = false;
			for (int i = 0; i < table.Count; i++)
			{
				int numberOfItems = table.NumberOfHashedItems(i);
				ulong value = table.Get(i);
				switch (numberOfItems)
				{
					case 1:
						values.Add(value);
						break;
					case 0:
						if (value != 0)
						{
							decodingFailed = true;
						}
						break;
					default:
						decodingFailed = true;
						break;
				}
			}
			return (values, decodingFailed);
		}

		public void Decode()
		{


		}

		public HashSet<TTable> GetCurrentDecodedValues()
		{
			throw new NotImplementedException();
		}
	}

}
