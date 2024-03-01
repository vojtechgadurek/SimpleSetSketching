using SimpleSetSketching.New.StreamProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.New.Utils;

namespace SimpleSetSketching.New.Tooglers.TooglingFunctions
{
	public interface ITooglingFunction
	{
		public ITable<ulong> ToogleStreamToTableTruncatedArray(TruncatedArray<ulong> buffer, ITable<ulong> toogleTo);
	}
}
