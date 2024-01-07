﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	interface IHashingFunction<TValue, THashedTo> where TValue : struct where THashedTo : struct
	{
		THashedTo Hash(TValue value);
	}
}
