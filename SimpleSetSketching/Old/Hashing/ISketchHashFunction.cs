using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Hashing
{
	public interface ISketchHashFunction
	{
		/// <summary>
		/// Expect a int to be hashed, returns 3 independant hashes
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		(ulong, ulong, ulong) GetHash(ulong x);
	}
}
