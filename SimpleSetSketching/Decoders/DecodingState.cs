﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Decoders
{
	public enum DecodingState
	{
		Decoding,
		Success,
		Failed,
		Shotdown,
		NotStarted
	}
}
