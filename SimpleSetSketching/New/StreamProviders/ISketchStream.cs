using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.New.Utils;

namespace SimpleSetSketching.New.StreamProviders
{
	public interface ISketchStream<TValue>
	{
		TruncatedArray<TValue> FillBuffer(TValue[] buffer);
		void Dispose();
		uint? Length();
	}
}
