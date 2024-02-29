﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.StreamProviders
{
	public record struct TruncatedArray<TValue>(uint size, TValue[] array);
}
