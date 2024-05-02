using Iced.Intel;
using SimpleSetSketching.InvertiblBloomLookupTable;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.InvertibleBloomLookupTable.Tables
{
	public class BasicHypergraphTable : IHypergraphTable<ulong>
	{
		record struct Bucket(ulong sum, int count);

		private readonly Bucket[] buckets;
		public int Count => throw new NotImplementedException();

		public BasicHypergraphTable(int size)
		{
			buckets = new Bucket[size];
		}
		public ulong Get(int index)
		{
			return buckets[index].sum;
		}

		public void Add(int index, ulong value)
		{
			buckets[index].sum += value;
			buckets[index].count++;
		}

		public void Remove(int index, ulong value)
		{
			buckets[index].sum -= value;
			buckets[index].count--;
		}

		public int NumberOfHashedItems(int index)
		{
			return buckets[index].count;
		}

		public static Expression<Action<ulong, ulong, BasicHypergraphTable>> GetTogglingAction()
		{
			var a = CompiledActions.Create<ulong, ulong, BasicHypergraphTable>(out var hash, out var value, out var table);
			a.S.AddExpression(table.V.Call<NoneType>("Add", hash.V, value.V));
			return a.Construct();
		}
	}
}
