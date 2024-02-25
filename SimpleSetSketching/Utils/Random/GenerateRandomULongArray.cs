using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public class GenerateRandomULongArray
	{
		public static ulong[] GenerateRandomULongArrayFunc(int size, ulong seed)
		{
			ulong[] result = new ulong[size];
			Random random = new Random((int)seed);
			for (int i = 0; i < size; i++)
			{
				result[i] = (ulong)random.Next();
			}
			return result;
		}
	}
}
