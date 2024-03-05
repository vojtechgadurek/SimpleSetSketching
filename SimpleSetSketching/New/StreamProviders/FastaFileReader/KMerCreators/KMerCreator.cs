using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.StreamProviders.DNA.KMerCreators
{
	public class KMerCreator
	{
		readonly public int KMerLength;
		readonly ulong _lengthMask;
		const int _numberOfBitsInUlong = 64;
		const int _maxLength = _numberOfBitsInUlong / 2 - 1;
		const ulong _headerValue = 0b11;
		const ulong _removeHeaderMask = ~0b11UL;
		const ulong _lastSymbolMask = ~0b1100UL;
		readonly ulong _firstSymbolMask;
		//Default symbol is represented as 00 and currently is mapped to A
		public KMerCreator(int lengthOfKMer)
		{
			KMerLength = lengthOfKMer;
			int numberOfBitsUsed = lengthOfKMer * 2 + 2;
			if (lengthOfKMer > _maxLength) throw new ArgumentException($"{lengthOfKMer} is over limit 31");
			_lengthMask = (1UL << (numberOfBitsUsed + 1)) - 1 & _removeHeaderMask;
			_firstSymbolMask = (11UL << numberOfBitsUsed);
		}

		public KMerWithComplement EmptyKMer => new KMerWithComplement(new KMer(_headerValue), new KMer(AddHeader(RemoveHeader(~0ul))));

		public KMerWithComplement PushSymbolIn(KMerWithComplement kMerWithComplement, Symbol symbol)
		{
			ulong value = kMerWithComplement.KMer.GetBinaryRepresentation();
			ulong complement = kMerWithComplement.Complement.GetBinaryRepresentation();

			value = PushSymbolFromStart(value, symbol);
			complement = PushSymbolFromEnd(complement, GetSymbolComplement(symbol));

			return new KMerWithComplement(new KMer(value), new KMer(complement));
		}

		public Symbol GetSymbolComplement(Symbol symbol)
		{
			ulong value = (ulong)symbol;
			value = value ^ 0b11;
			return (Symbol)value;
		}
		public Symbol TranslateCharToSymbol(char c)
		{
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
		public char TranslateSymbolToChar(Symbol symbol)
		{
			switch (symbol)
			{
				case Symbol.A:
					return 'A';
				case Symbol.C:
					return 'C';
				case Symbol.G:
					return 'G';
				case Symbol.T:
					return 'T';
				default:
					throw new InvalidEnumArgumentException();
			}
		}

		public KMer InitiliazeKMerFromSymbolArray(Symbol[] symbols)
		{
			if (symbols.Length != KMerLength)
			{
				throw new ArgumentException("Length of symbols array is not equal to KMerLength");
			}
			ulong value = 0;
			for (int i = 0; i < symbols.Length; i++)
			{
				value |= GetDefaultValuesWithSymbolInPlace(symbols[i], i);
			}
			return new KMer(value);

		}
		public ulong RemoveHeader(ulong value)
		{
			return value & _removeHeaderMask;
		}
		public ulong AddHeader(ulong value)
		{
			return value | _headerValue;
		}

		public ulong GetMask(int index)
		{
			return 0b11UL << ((index + 1) * 2);
		}

		public ulong GetDefaultValuesWithSymbolInPlace(Symbol symbol, int index)
		{
			ulong value = (ulong)symbol << ((index + 1) * 2);
			return value;
		}

		public ulong GetInverseMask(ulong mask)
		{
			return ~mask;
		}

		public KMer SetNthSymbolToDefault(KMer kMer, int index)
		{
			ulong value = kMer.GetBinaryRepresentation();
			ulong mask = GetInverseMask(GetMask(index));
			return new KMer(value & mask);
		}
		public KMer SetNthSymbol(KMer kMer, int index, Symbol symbol)
		{

			ulong value = (ulong)symbol << ((index + 1) * 2);
			return new KMer(SetNthSymbolToDefault(kMer, index).GetBinaryRepresentation() | value);
		}
		public ulong ChangeFirstSymbol(ulong value, Symbol symbol)
		{
			value = value & _firstSymbolMask;
			value = value | (ulong)symbol;
			return value;
		}

		public ulong ChangeLastSymbol(ulong value, Symbol symbol)
		{
			value = value & _lastSymbolMask;
			value = value | (ulong)symbol;
			return value;
		}
		private ulong ShiftLeft(ulong value)
		{
			value = RemoveHeader(value);
			value = value << 2;
			value = AddHeader(value);
			value &= _lengthMask;
			return value;
		}

		public ulong ShiftRight(ulong value)
		{
			value = RemoveHeader(value);
			value = value >> 2;
			value = AddHeader(value);
			value &= _lengthMask;
			return value;
		}

		public ulong PushSymbolFromStart(ulong value, Symbol symbol)
		{
			value = ShiftRight(value);
			value = ChangeFirstSymbol(value, symbol);
			return value;
		}
		public ulong PushSymbolFromEnd(ulong value, Symbol symbol)
		{
			value = ShiftLeft(value);
			value = ChangeLastSymbol(value, symbol);
			return value;
		}

		public string TranslateKMerToString(KMer kMer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ulong mask = 0b11;
			ulong value = kMer.GetBinaryRepresentation();
			for (int i = 0; i < KMerLength; i++)
			{
				value = value >> 2;
				Symbol symbol = (Symbol)(value & mask);
				stringBuilder.Append(TranslateSymbolToChar(symbol));
			}
			return new string(stringBuilder.ToString());

		}
	}
}
