using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Togglers
{
	internal interface IToggler<TTable>
	{
		public TTable ToggleStreamToTable(ISketchStream<ValueType> stream);
	}
}
