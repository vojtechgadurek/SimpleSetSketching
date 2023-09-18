using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public interface ISketchStreamProvider<T>
	{
		void FillBuffer(T[] buffer, out int maxOffest);
		void Dispose();
		ulong? Length();
	}
}
