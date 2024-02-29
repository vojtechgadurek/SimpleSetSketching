using SimpleSetSketching.New.Hashers;
using SimpleSetSketching.New.StreamProviders;
using SimpleSetSketching.New.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Tooglers
{
	public class Toogler : IToogler
	{
		int _bufferSize;
		public Toogler(int bufferSize)
		{
			_bufferSize = bufferSize;
		}
		public ITable<ulong> ToogleStreamToTable(Func<TruncatedArray<ulong>, ITable<ulong>, ITable<ulong>> tooglingFunction, ISketchStream<ulong> stream, ITable<ulong> table)
		{
			ulong[] buffer = new ulong[_bufferSize];
			while (true)
			{
				var truncatedArray = stream.FillBuffer(buffer);
				if (truncatedArray.Size <= 0) break;
				table = tooglingFunction.Invoke(truncatedArray, table);
			}
			return table;
		}
	}
}
