using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	internal struct MultiplyShiftHash<TMultiplyConstant, TSize> : IHashingFunction<uint, uint> where TMultiplyConstant : struct where TSize : struct
	{
		private static readonly uint multiplyConstant = DynamicConstantTypeCreator<uint>.GetValue(typeof(TMultiplyConstant));
		private static readonly uint size = DynamicConstantTypeCreator<uint>.GetValue(typeof(TSize));
		uint IHashingFunction<uint, uint>.Hash(uint value)
		{
			return ((value * multiplyConstant) >> (int)(32 - size)) % size;
		}

	}
}
