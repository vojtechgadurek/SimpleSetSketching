using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.StreamProviders.FastaFileReader;

namespace SimpleSetSketching.StreamProviders.FastaFileReader.KMerCreators
{
	public class OtherRepresentationKMerCreator
	{
		//last 2 bits are reserved for header
		//every mer takes 2 bits
		//thus there are 31 spaces for mers
		//We spilt kmer to halfs, first half is in little endian, second half is in big endian, they are intertwinned
		//word ACGT is thus represented as TCGA, A_T_  _G_C
		readonly int _lengthOfKMer;
		readonly ulong _lengthMask;
		const int _numberOfBitsInUlong = 64;
		const int _maxLength = _numberOfBitsInUlong / 2 - 1;
		const ulong _everySecondMerMask = 0b0011_0011_0011_0011__0011_0011_0011_0011__11_0011_0011_0011__0011_0011_0011_0000;
		const ulong _everySecondMerComplementMask = _everySecondMerMask << 2 + 0b1100;
		const ulong _headerMask = 0b11;
		const ulong _headerValue = 0b11;
		const ulong _removeHeaderMask = ulong.MaxValue - 0b11;
		readonly ulong _oddOverflowShiftMask;
		readonly ulong _everyShiftOverflowMask;

		public OtherRepresentationKMerCreator(int lengthOfKMer)
		{
			_lengthOfKMer = lengthOfKMer;
			int numberOfBitsUsed = lengthOfKMer * 2 + 2;
			if (lengthOfKMer > _maxLength) throw new ArgumentException($"{lengthOfKMer} is over limit 31");
			_lengthMask = (1UL << numberOfBitsUsed + 1) - 1 & _removeHeaderMask;

			_everyShiftOverflowMask = 0b11UL << numberOfBitsUsed - 2;
			if (numberOfBitsUsed % 2 == 1)
			{
				_oddOverflowShiftMask = 0b11UL << numberOfBitsUsed - 2;
			}
			else
			{
				_oddOverflowShiftMask = 0;
			}

		}

		ulong GetEverySecondMer(ulong value)
		{
			return value & _everySecondMerMask;
		}
		ulong GetEverySecondMerStartingWithFirst(ulong value)
		{
			return value & _everySecondMerComplementMask;
		}
		ulong RotateMer(ulong value)
		{
			ulong everySecondMer = GetEverySecondMer(value);
			ulong everySecondMerComplement = GetEverySecondMerStartingWithFirst(value);
			ulong rotatedMer = (everySecondMer >> 2) + (everySecondMerComplement << 2);
			ulong overflow = rotatedMer & _headerMask;
			return rotatedMer;
		}
		ulong GetComplement(ulong value)
		{
			ulong overflowValue = value & _oddOverflowShiftMask;

			value = value & _removeHeaderMask;
			ulong rotation = RotateMer(value);
			ulong complement = ~rotation;
			complement = complement & _lengthMask;
			complement = complement | overflowValue;
			complement = complement | _headerValue;
			return complement;
		}

		public KMer GetCanonicalValue(KMer kMer)
		{
			ulong value = kMer.GetBinaryRepresentation();
			ulong complement = GetComplement(value);
			if (value < complement) return new KMer(value);
			return new KMer(complement);
		}

		public KMer GetComplement(KMer kMer)
		{
			return new KMer(GetComplement(kMer.GetBinaryRepresentation()));
		}

		//One does not need to forget that A equals 00, thus this shift is equivalent to shift by 1 and adding A
		//But one should not rely on this
		private KMer ShiftByOne(KMer kMer)
		{
			ulong value = kMer.GetBinaryRepresentation();
			value = _removeHeaderMask & value;
			ulong getEverySecondMer = GetEverySecondMer(value);
			ulong getEverySecondMerStartingWithFirst = GetEverySecondMerStartingWithFirst(value);
			ulong overflow = value & _oddOverflowShiftMask >> 2;
			ulong shifted = getEverySecondMer >> 4 | getEverySecondMerStartingWithFirst << 4;
			return new KMer(shifted | overflow);
		}
		public KMer PushCharacterIn(KMer kMer, Symbol symbol)
		{
			ulong value = kMer.GetBinaryRepresentation();
			ulong overflowValue = value & _oddOverflowShiftMask;
			value = value & _removeHeaderMask;
			ulong rotation = RotateMer(value);
			ulong complement = ~rotation;
			complement = complement & _lengthMask;
			complement = complement | overflowValue;
			complement = complement | _headerValue;
			return new KMer(complement);
		}



		public Symbol TransalteCharToSymbol(char c)
		{
			c = char.ToUpper(c);
			switch (c)
			{
				case 'A':
					return Symbol.A;
				case 'C':
					return Symbol.C;
				case 'G':
					return Symbol.G;
				case 'T':
					return Symbol.T;
				default:
					throw new InvalidEnumArgumentException();
			}
		}
	}
}
