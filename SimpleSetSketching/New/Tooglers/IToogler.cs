using SimpleSetSketching.New.StreamProviders;
using SimpleSetSketching.New.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Hashers
{
	public interface IToogler
	{
		ITable<ulong> ToogleStreamToTable(Func<TruncatedArray<ulong>, ITable<ulong>, ITable<ulong>> tooglingFunction, ISketchStream<ulong> stream, ITable<ulong> table);
	}
}
