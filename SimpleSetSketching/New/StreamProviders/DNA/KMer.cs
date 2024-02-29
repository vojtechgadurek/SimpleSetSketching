using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.StreamProviders.DNA
{
	/// <summary>
	/// Every Mer take 2 bits, last 2 bits are reserved for header.
	/// 
	/// </summary>
	/// 
	public enum Symbol : ulong
	{
		A = 0,
		C = 1,
		G = 2,
		T = 3
	}
	public struct KMer
	{
		ulong _value;
		public KMer(ulong value)
		{
			_value = value;
		}
		public ulong GetBinaryRepresentation()
		{
			return _value;
		}
	}
	public record struct KMerWithComplement(KMer KMer, KMer Complement);
}




