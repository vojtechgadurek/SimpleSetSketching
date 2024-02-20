using SimpleSetSketching;
using System.Data.HashFunction.MurmurHash;
using System.Runtime.CompilerServices;


namespace SimpleSetSketching
{
	public static class StaticSimpleSetSketcher_v02
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

		public static void Toogle(ulong i, ulong[] data, ulong size)
		{
			unsafe
			{
				ulong index = HashFunctions.FirstHash(i, size);
				data[index] ^= i;
				index = HashFunctions.SecondHash(i, size);
				data[index] ^= i;
				index = HashFunctions.ThirdHash(i, size);
				data[index] ^= i;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

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
			if (HashFunctions.FirstHash(value, size) == index)
			{
				return true;
			}
			if (HashFunctions.SecondHash(value, size) == index)
			{
				return true;
			}
			return HashFunctions.ThirdHash(value, size) == index;
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
					AddIfPure(HashFunctions.FirstHash(x, size), nextPure, data, size);
					AddIfPure(HashFunctions.SecondHash(x, size), nextPure, data, size);
					AddIfPure(HashFunctions.ThirdHash(x, size), nextPure, data, size);
				}
				pure = nextPure;
			}

			if (!data.All(x => x == 0))
			{
				return null;
			};
			return ansver;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

		public static HashSet<ulong>? Decode(ulong[] simpleSetSketch, ulong size, int shotdownMultiplicator)
		{
			return TryDecode(simpleSetSketch, size, shotdownMultiplicator);
		}
	}
	public class SimpleSetSketcher_v02 : ISketcher
	{
		readonly ulong[] _data;
		ulong _size;
		public SimpleSetSketcher_v02(ulong size)
		{

			int power = 0;
			while (((ulong)1 << power) < size)
			{
				power += 1;
			}
			_size = (ulong)1 << power;
			_data = new ulong[_size];
		}
		public HashSet<ulong>? Decode()
		{
			return StaticSimpleSetSketcher_v02.Decode(_data, _size, 1); //1 - constant
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Toogle(SketchStream insert)
		{
			while (true)
			{
				ulong next = insert.Next();
				if (next == 0) break;
				//SimpleSetSketchingFSharp.SimpleSetSketcher.testToogle(_data, next, _size - 1);
				StaticSimpleSetSketcher_v02.Toogle(next, _data, _size);
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

		public SimpleSetSketcher_v02 Merge(SimpleSetSketcher_v02 other)
		{
			if (other._size != _size)
			{
				throw new Exception("Cannot merge SimpleSetSketchers of different sizes");
			}
			ulong[] otherdata = other._data;
			for (ulong i = 0; i < (ulong)otherdata.Length; i++)
			{
				_data[i] ^= otherdata[i];
			}
			return this;
		}
	}





}
public class ParallelSetSketcher : ISketcher
{
	SimpleSetSketcher_v02[] _simpleSetSketchers;
	SketchStream[] _streams;
	ulong _size;
	public ParallelSetSketcher(ulong size, int maxThreads)
	{
		_size = size;
		_simpleSetSketchers = new SimpleSetSketcher_v02[maxThreads];
		_streams = new SketchStream[maxThreads];
		for (int i = 0; i < maxThreads; i++)
		{
			var simpleSetSketcher = new SimpleSetSketcher_v02(size);
			_simpleSetSketchers[i] = simpleSetSketcher;
		}

	}

	void Toogle(SketchStream insert)
	{
		lock (this)
		{
			for (int i = 0; i < _streams.Length; i++)
			{
				_streams[i] = new SketchStream(insert, 1024);
				//Old ->
				//THIS IS PROBLEMATIC - we expect buffer size to allow sizes up to 1024, IT NEEDS TO BE REDONE
				//<-End 
				//This should work correctly 10.2.2023
			}
			Parallel.For(0, _simpleSetSketchers.Length, (i) =>
			{
				_simpleSetSketchers[i].Toogle(_streams[i]);
			});
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

	public HashSet<ulong>? Decode()
	{
		SimpleSetSketcher_v02 ansver = new SimpleSetSketcher_v02(_size);
		foreach (var simpleSetSketcher in _simpleSetSketchers)
		{
			ansver.Merge(simpleSetSketcher);
		}
		return ansver.Decode();
	}
}




