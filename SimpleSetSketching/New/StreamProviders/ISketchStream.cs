﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.StreamProviders
{
	public interface ISketchStreamProvider<TValue>
	{
		TruncatedArray<TValue> FillBuffer(TValue[] buffer);
		void Dispose();
		uint? Length();
	}
}
