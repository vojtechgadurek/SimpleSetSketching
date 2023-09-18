using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public class HashFunctions
	{
		public static void HashToTable(ulong key, ulong[] hashes, ulong size)
		{
			if (hashes.Length >= 1)
			{
				hashes[0] = FirstHash(key, size);
			}
			if (hashes.Length >= 2)
			{
				hashes[1] = SecondHash(key, size);
			}
			if (hashes.Length >= 3)
			{
				hashes[2] = ThirdHash(key, size);
			}
			if (hashes.Length >= 4)
			{
				hashes[3] = FourthHash(key, size);
			}
			if (hashes.Length >= 5)
			{
				hashes[4] = FifthHash(key, size);
			}
		}
		public static ulong FirstHash(ulong i, ulong size)
		{
			return i & (size - 1);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

		public static ulong SecondHash(ulong i, ulong size)
		{
			ulong a = (i ^ (i >> 35)) * 0xbf58476d1de4e5b9UL >> 33;
			ulong b = (i ^ (i >> 29)) * 0x94d049bb133111ebUL;
			return (a ^ b ^ i) & (size - 1);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

		public static ulong ThirdHash(ulong i, ulong size)
		{
			i = (i ^ (i >> 28)) * 0x3C79AC482BA7B653UL >> 33;
			i = i ^ ((i * 0x1C69B3F74AC4AE35UL) >> 32);
			return i & (size - 1);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public static ulong FourthHash(ulong i, ulong size)
		{
			i = (i ^ (i >> 30)) * 0xBF58476D1CE4E5B9UL >> 31;
			i = (i ^ (i >> 27)) * 0x94D049BB133111EBUL >> 33;
			return i & (size - 1);
		}

		public static ulong FifthHash(ulong i, ulong size)
		{
			i = (i ^ (i >> 31)) * 0x7FB5D329728EA185UL >> 27;
			i = (i ^ (i >> 33)) * 0x81A01B605B5E0B6FUL >> 31;
			return i & (size - 1);
		}

	}
}
