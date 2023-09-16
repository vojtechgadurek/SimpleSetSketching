using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	internal interface ISketcher
	{
		void Insert(SketchStream insert);
		void Remove(SketchStream insert);
		HashSet<ulong>? Decode();
	}

}
