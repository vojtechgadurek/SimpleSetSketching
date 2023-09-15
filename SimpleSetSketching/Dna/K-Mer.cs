using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public struct K_Mer
	{
		public readonly ulong data;
		public K_Mer(ulong data)
		{
			this.data = data;
		}
		public static K_Mer Empty { get { return new K_Mer(1); } }

		ulong AddHeader(ulong data)
		{
			return data + 1;
		}

		static ulong PushOneStep(ulong data)
		{
			return (data << 2);
		}
		static ulong RemoveHeader(ulong data)
		{
			return data - 1;
		}
		K_Mer PushInNewSymbol(char symbol)
		{
			ulong newData = PushOneStep(RemoveHeader(data));
			switch (symbol)
			{
				case 'A':
					newData |= (0 << 2);
					break;
				case 'C':
					newData |= (1 << 2);
					break;
				case 'G':
					newData |= (2 << 2);
					break;
				case 'T':
					newData |= (3 << 2);
					break;
				default:
					throw new InvalidEnumArgumentException();
			}
			return new K_Mer(AddHeader(newData));
		}
	}
}
