using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;



namespace SimpleSetSketching.SimpleSetSketchers
{
	public class SimpleSetSketcher_v01 : ISketcher
	{
		ulong[] _data;
		ulong _size;
		ISketchHashFunction _hashFunc;
		ulong _shotDownMultiplicator = 1; //Sets maximum number of decoding rounds to prevent infinite loop
		public SimpleSetSketcher_v01(ulong size, ISketchHashFunction hashFunction)
		{
			_size = size;
			_data = new ulong[size];
			_hashFunc = hashFunction;
		}
		public ulong[] GetData()
		{
			return _data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private (ulong, ulong, ulong) GetTruncatedHash(ulong index)
		{
			var hash = _hashFunc.GetHash(index);
			return ((ulong)(hash.Item1 % _size), (ulong)(hash.Item2 % _size), (ulong)(hash.Item3 % _size));
		}
		public void Merge(SimpleSetSketcher_v01 other)
		{
			for (int i = 0; i < _data.Length; i++)
			{
				var otherData = other.GetData();
				_data[i] ^= otherData[i];
			}
		}
		public bool LooksPure(ulong index, ulong[] data)
		{
			var hash = GetTruncatedHash(data[index]);
			return ((data[index] != 0)) && (hash.Item1 == index || hash.Item2 == index || hash.Item3 == index);
		}
		public HashSet<ulong>? TryDecode()
		{
			HashSet<ulong> ansver = new HashSet<ulong>();
			HashSet<ulong> pure = new HashSet<ulong>();
			//Copydata allow Decoder to be non destructive
			ulong[] copydata = (ulong[])this._data.Clone();
			for (ulong i = 0; i < _size; i++)
			{
				if (LooksPure(i, copydata))
				{
					pure.Add(i);
				}
			}
			ulong hardStop = _size * _shotDownMultiplicator; //Tohle je nějaká _random konstatnta, aby se to necyklilo
			ulong rounds = 0;
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
					if (!LooksPure(i, copydata))
					{
						continue;
					}
					var x = copydata[i];
					Toogle(x, copydata);
					if (ansver.Contains(x))
					{
						ansver.Remove(x);
					}
					else
					{
						ansver.Add(x);
					}
					(ulong, ulong, ulong) hash = GetTruncatedHash(x);
					void AddIfPure(ulong index)
					{
						if (LooksPure(index, copydata))
						{
							nextPure.Add(index);
						}
					}
					AddIfPure(hash.Item1);
					AddIfPure(hash.Item2);
					AddIfPure(hash.Item3);
				}
				pure = nextPure;
			}

			if (!copydata.All(x => x == 0))
			{
				return null;
			};
			return ansver;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		private void Toogle(ulong x, ulong[] data)
		{
			var hash = GetTruncatedHash(x);
			void SmallToogle(ulong index)
			{
				data[index] ^= x;
			}
			SmallToogle(hash.Item1);
			SmallToogle(hash.Item2);
			SmallToogle(hash.Item3);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public void Toogle(ulong x)
		{
			Toogle(x, _data);
		}

		public void Toogle(SketchStream insert)
		{
			while (true)
			{
				ulong next = insert.Next();
				if (next == 0) { break; };
				Toogle(next, _data);
			}
		}

		public void Insert(SketchStream insert)
		{
			Toogle(insert);

		}

		public void Remove(SketchStream insert)
		{
			Toogle(insert);
		}

		public HashSet<ulong>? Decode()
		{
			return TryDecode();
		}
	}
}
