using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public interface IDataProvider
	{
		SketchStream GetDataToInsert();
		SketchStream GetDataToRemove();
		void Dispose();
	}
	public interface ITestDataProvider : IDataProvider
	{
		ulong[] GetExpectedResult();
	}


}
