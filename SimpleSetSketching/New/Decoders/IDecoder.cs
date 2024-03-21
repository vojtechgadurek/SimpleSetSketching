using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.Decoders
{
	internal interface IDecoder<T>
	{
		void Decode();


		HashSet<T> GetCurrentDecodedValues();

		DecodingState State { get; }

	}
}
