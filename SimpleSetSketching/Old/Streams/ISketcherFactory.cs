using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Streams
{
	public interface ISketcherFactory
	{
		ISketcher CreateSketcher(ulong size);
	}
}
