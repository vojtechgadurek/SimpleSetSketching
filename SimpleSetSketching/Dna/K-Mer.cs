using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public struct K_Mer
	{
		public readonly ulong data;
		public const int _MaxK = 31;
		public readonly int K;
		private ulong _sizemask;
		public K_Mer(ulong data, int size)
		{
			this.data = data;
			this.K = size;
			if (size > _MaxK) throw new ArgumentException($"{size} is over limit {_MaxK}");
			//Last two bits are left to header
			_sizemask =  ((ulong) 1l << ((size * 2) + 2 + 1)) - 1;
		}
		public static K_Mer Empty(int K) => new K_Mer(1, K);

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			ulong mask = 0b11;
			ulong data = this.data;
			for (int i = 0; i < K; i++)
			{
				data = data >> 2;
				ulong symbol = data & mask;
				switch (symbol)
				{
					case 0:
						stringBuilder.Append('A');
						break;
					case 1:
						stringBuilder.Append('C');
						break;
					case 2:
						stringBuilder.Append('G');
						break;
					case 3:
						stringBuilder.Append('T');
						break;
					default:
						throw new InvalidEnumArgumentException();
				}
			}
			return new string(stringBuilder.ToString());
		}
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
		public K_Mer PushInNewSymbol(char symbol)
		{
			ulong newData = PushOneStep(RemoveHeader(data));
			symbol = Char.ToUpper(symbol);
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
			//throw out ones that are over the limit
			newData = newData & _sizemask;
			return new K_Mer(AddHeader(newData), K);
		}
	}
}
