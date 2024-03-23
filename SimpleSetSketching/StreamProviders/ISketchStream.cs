using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.Utils;

namespace SimpleSetSketching.StreamProviders
{
	public interface ISketchStream<TValue>
	{
		TruncatedArray<TValue> FillBuffer(TValue[] buffer);
		void Dispose();
		uint? Length();
	}
}
