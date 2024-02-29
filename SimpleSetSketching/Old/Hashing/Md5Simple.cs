using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Hashing
{
	public class Md5Simple : ISketchHashFunction
	{

		MD5 md5;
		public Md5Simple()
		{
			md5 = MD5.Create();
		}

		public (ulong, ulong, ulong) GetHash(ulong number)
		{
			//I do there hashing for the price of one
			var hash = md5.ComputeHash(BitConverter.GetBytes(number));
			return (BitConverter.ToUInt64(hash, 0), BitConverter.ToUInt64(hash, 4), BitConverter.ToUInt64(hash, 8));
		}
	}
}
