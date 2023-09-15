using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public static class QuickSimpleSetSketcher
	{


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong FirstHash(ulong i, ulong size)
		{
			return ((30000 * i + 10000) % 100057) % size;
		}

		public static ulong SecondHash(ulong i, ulong size)
		{
			return ((121312139322 * i + 2282313) % 102199) % size;
		}

		public static ulong ThirdHash(ulong i, ulong size)
		{
			return ((927322731237 * i + 92342) % 102233) % size;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static void Toogle(ulong i, ulong[] data, ulong size)
		{
			data[FirstHash(i, size)] ^= i;
			data[SecondHash(i, size)] ^= i;
			data[ThirdHash(i, size)] ^= i;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static bool LooksPure(ulong i, ulong[] data, ulong size)
		{
			ulong value = data[i];
			if (value == 0) return false;
			if (FirstHash(value, size) == i) return true;
			if (SecondHash(value, size) == i) return true;
			return (ThirdHash(value, size) == i);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		static void AddIfPure(ulong index, HashSet<ulong> pure, ulong[] data, ulong size)
		{

			if (LooksPure(index, data, size))
			{
				pure.Add(index);
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		static public HashSet<ulong>? TryDecode(ulong[] data, ulong size, int shotDownMultiplicator)
		{
			HashSet<ulong> ansver = new HashSet<ulong>();
			HashSet<ulong> pure = new HashSet<ulong>();
			for (uint i = 0; i < data.Length; i++)
			{
				AddIfPure(i, pure, data, size);
			}
			int hardStop = data.Length * shotDownMultiplicator; //Tohle je nějaká random konstatnta, aby se to necyklilo
			int rounds = 0;
			while (pure.Count > 0)
			{
				rounds++;
				if (rounds > hardStop)
				{
					return null;
				}
				HashSet<ulong> nextPure = new HashSet<ulong>();
				foreach (var i in pure)
				{
					if (!LooksPure(i, data, size))
					{
						continue;
					}
					var x = data[i];
					Toogle(x, data, size);
					if (ansver.Contains(x))
					{
						ansver.Remove(x);
					}
					else
					{
						ansver.Add(x);
					}


					AddIfPure(FirstHash(i, size), nextPure, data, size);
					AddIfPure(SecondHash(i, size), nextPure, data, size);
					AddIfPure(ThirdHash(i, size), nextPure, data, size);
				}
				pure = nextPure;
			}

			if (!data.All(x => x == 0))
			{
				return null;
			};
			return ansver;
		}

		public static void Toogle(ulong[] toInsert, ulong[] simpleSetSketch)
		{
			for (int i = 0; i < toInsert.Length; i++)
			{
				Toogle(toInsert[i], simpleSetSketch, (ulong)simpleSetSketch.Length);
			}
		}

		public static HashSet<ulong>? Decode(ulong[] simpleSetSketch, ulong size, int shotdownMultiplicator)
		{
			return TryDecode(simpleSetSketch, size, shotdownMultiplicator);
		}
	}
	public class SimpleSetSketcher : ISketcher
	{
		ulong[] _data;
		ulong _size;
		public SimpleSetSketcher(ulong size)
		{
			_data = new ulong[size];
			this._size = size;
		}
		public HashSet<ulong>? Decode()
		{
			return QuickSimpleSetSketcher.Decode(_data, _size, 100);
		}

		void Toogle(SketchStream insert)
		{
			while (true)
			{
				ulong next = insert.Next();
				if (next == 0) break;
				QuickSimpleSetSketcher.Toogle(next, _data, _size);
			}
		}
		public void Insert(SketchStream insert)
		{
			Toogle(insert);
		}

		public void Remove(SketchStream remove)
		{
			Toogle(remove);
		}
	}


}



