using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SimpleSetSketching.New.Utils
{
	public record struct TruncatedArray<TValue>(int Size, TValue[] Array);
	public record struct Buffer<TValue>(int Size, TValue[] Array, int CurrentOffset);
}
