using SimpleSetSketching;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public static class QuickSimpleSetSketcher
	{
		static QuickHashing quickHashing = new QuickHashing();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong FirstHash(ulong i, ulong size)
		{
			//return quickHashing.FirstHash(i) % size;
			return i & size;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static ulong SecondHash(ulong i, ulong size)
		{
			//return quickHashing.SecondHash(i) % size;
			return ((121312139322 * i + 2282313) % 100000004761) & size;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static ulong ThirdHash(ulong i, ulong size)
		{
			//return quickHashing.ThirdHash(i) % size;
			return ((927322731237 * i + 92342) % 100000004483) & size;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static void Toogle(ulong i, ulong[] data, ulong size)
		{
			unsafe
			{
				/*
				fixed (ulong* ptr = data)
				{
					*(ptr + FirstHash(i, size)) ^= i;
					*(ptr + SecondHash(i, size)) ^= i;
					*(ptr + ThirdHash(i, size)) ^= i;
				}
				*/
				ulong index = FirstHash(i, size);
				data[index] ^= i;
				index = SecondHash(i, size);
				data[index] ^= i;
				index = ThirdHash(i, size);
				data[index] ^= i;

				/*
				data[FirstHash(i, size)] ^= i;
				data[SecondHash(i, size)] ^= i;
				data[ThirdHash(i, size)] ^= i;
				*/

			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static bool LooksPure(ulong index, ulong[] data, ulong size)
		{
			var value = data[index];
			/*
			return ((data[index] != 0)) && (FirstHash(value, size) == index || SecondHash(value, size) == index || ThirdHash(value, size) == index);
			*/
			if (value == 0)
			{
				return false;
			}
			if (FirstHash(value, size) == index)
			{
				return true;
			}
			if (SecondHash(value, size) == index)
			{
				return true;
			}
			return ThirdHash(value, size) == index;
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


					AddIfPure(FirstHash(x, size), nextPure, data, size);
					AddIfPure(SecondHash(x, size), nextPure, data, size);
					AddIfPure(ThirdHash(x, size), nextPure, data, size);
				}
				pure = nextPure;
			}

			if (!data.All(x => x == 0))
			{
				return null;
			};
			return ansver;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static void Toogle(ulong[] toInsert, ulong[] simpleSetSketch)
		{
			for (int i = 0; i < toInsert.Length; i++)
			{
				Toogle(toInsert[i], simpleSetSketch, (ulong)simpleSetSketch.Length);
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

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

			_size = 1;
			while (_size < size)
			{
				_size <<= 1;
				_size += 1;
			}
			_data = new ulong[_size + 1];
		}
		public HashSet<ulong>? Decode()
		{
			return QuickSimpleSetSketcher.Decode(_data, _size, 1);
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

	/*
	public class ParallelSetSketcher : ISketcher
	{
		ulong[] _data;
		ulong _size;
		void Toogle(SketchStream insert)
		{
			while (true)
			{
				ulong next = insert.Next();
				if (next == 0) break;
				QuickSimpleSetSketcher.Toogle(next, _data, _size);
			}
		}

	}
	*/

}



