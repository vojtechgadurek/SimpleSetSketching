using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Dynamic;
using System.Formats.Asn1;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.AccessControl;
using System.Security.Cryptography;
using SimpleSetSketching;
using SimpleSetSketching.Data;
using SimpleSetSketching.Testing;

#nullable enable
public class Program
{
	public static void Main(string[] args)
	{
		/*
		K_Mer k_Mer = new K_Mer(1);
		k_Mer = k_Mer.PushInNewSymbol('A');
		k_Mer = k_Mer.PushInNewSymbol('A');
		k_Mer = k_Mer.PushInNewSymbol('C');
		k_Mer = k_Mer.PushInNewSymbol('C');
		k_Mer = k_Mer.PushInNewSymbol('T');
		k_Mer = k_Mer.PushInNewSymbol('C');
		k_Mer = k_Mer.PushInNewSymbol('C');
		k_Mer = k_Mer.PushInNewSymbol('T');
		k_Mer = k_Mer.PushInNewSymbol('A');
		k_Mer = k_Mer.PushInNewSymbol('G');
		k_Mer = k_Mer.PushInNewSymbol('G');


		*/
		int size = 2_000_000; //83380;
		int numberOfRounds = 10;

		var ansver = TestingFramework.TestMultipleDecodings(
			TestingFramework.GetBasicSketcherProviderV02((ulong)(size * 1.3), numberOfRounds),
			TestingFramework.GetFastaFileDataProvider(NamesToFastaFiles.covid11_copy, NamesToFastaFiles.covid11,
			numberOfRounds)
			);
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");


		/*
		var ansver = TestingFramework.TestWithRandomData(TestingFramework.GetBasicSketcherProviderV02((ulong)(size * 1.4), numberOfRounds), numberOfRounds, size * 10, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/
		/*
		var ansver = TestingFramework.TestWithRandomData(TestingFramework.GetSimpleParrallerSketcherProvider((ulong)(size * 1.4), numberOfRounds, 4), numberOfRounds, size * 1000, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");


		/*
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetBasicSketcherProvider((ulong)(size * 1.4), numberOfRounds, new QuickHashing()), numberOfRounds, size * 10, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/
		/*
		ansver = TestingFramework.TestWithRandomData(TestingFramework.GetBasicSketcherProvider((ulong)(size * 1.4), numberOfRounds, new Md5Simple()), numberOfRounds, size * 10, size, new Random(42));
		Console.WriteLine($"Time used: {ansver.Results.Sum(x => x.Time.TotalMilliseconds)} ms");
		Console.WriteLine($"Ok: {ansver.Results.Count(x => x.Success)}, all {ansver.Results.Count()}");
		*/

	}
	/*
	static public void Test(ISketchHashFunction hashFunction)
	{
		ComplexTests complexTests = new ComplexTests();
		var Clock = System.Diagnostics.Stopwatch.StartNew();
		var ansver = complexTests.TestMultipleMultiplicator(1, 10, 0, 1.3, 1000000, 100000, 42, true, hashFunction);
		foreach (var item in ansver)
		{
			var statesGrouped = item.data.GroupBy(x => x.state);
			Console.Write($"{item.sizeOfSketchMultiplicator} ");
			foreach (var state in statesGrouped)
			{
				Console.Write($"{state.Key} {state.Count()} ");
			}
			Console.Write($"time {item.data.Sum(x => x.timeTaken) / item.data.Count}");
			Console.WriteLine();
		}
		Console.WriteLine($"Total time spent computinng {ansver.Sum(x => x.data.Sum(y => y.timeTaken))}");
		Console.WriteLine($"Total time spent shotdown {ansver.Sum(x => x.data.Where(z => z.state == ComplexTests.DecodingState.shotDown).Sum(y => y.timeTaken))}");
		Console.WriteLine($"Total time {Clock.ElapsedMilliseconds}");

	}
	*/

}

public enum DecodeState
{
	ok,
	notAbleToDecode,
	shotDown,
}
public record DecodedBasicSimpleSetSketcherToken(DecodeState state, IList<ulong> Data);


public class BasicSimpleSetSketcher : ISketcher
{
	ulong[] _data;
	ulong _size;
	ISketchHashFunction _hashFunc;
	ulong _shotDownMultiplicator = 1; //Sets maximum number of decoding rounds to prevent infinite loop
	public BasicSimpleSetSketcher(ulong size, ISketchHashFunction hashFunction)
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
	public void Merge(BasicSimpleSetSketcher other)
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
		//Copydata allow Decode to be non destructive
		ulong[] copydata = (ulong[])this._data.Clone();
		for (ulong i = 0; i < _size; i++)
		{
			if (LooksPure(i, copydata))
			{
				pure.Add(i);
			}
		}
		ulong hardStop = _size * _shotDownMultiplicator; //Tohle je nějaká random konstatnta, aby se to necyklilo
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

/*
class ReverseBloomFilter
{
	record struct RBFCell(int count, ulong keySum, int valueSum);
	RBFCell[] data;
	List<Func<ulong, ulong>> hashfuncs;

	public ulong GetHashIndex(Func<ulong, ulong> hashFunc, ulong index)
	{
		return (ulong)(hashFunc(index) % data.Length);
	}
	public void InsertByOneHashFunc(Func<ulong, ulong> hashFunc, ulong index, int value)
	{

		ulong tableIndex = GetHashIndex(hashFunc, index);
		RBFCell originalCell = data[tableIndex];
		data[tableIndex] = new RBFCell(originalCell.count + 1,
				originalCell.keySum + index,
				originalCell.valueSum + value);
	}
	public void Insert(ulong index, int value)
	{
		hashfuncs.ForEach(hashFunc => InsertByOneHashFunc(hashFunc, index, value));
	}

	public void DeleteByOneHashFunc(Func<ulong, ulong> hashFunc, ulong index, int value)
	{
		ulong tableIndex = GetHashIndex(hashFunc, index);
		RBFCell originalCell = data[tableIndex];
		data[tableIndex] = new RBFCell(originalCell.count - 1,
						   originalCell.keySum - index,
										  originalCell.valueSum - value);
	}
	public void Delete(ulong index, int value)
	{
		hashfuncs.ForEach(hashFunc => DeleteByOneHashFunc(hashFunc, index, value));
	}

	public int? Get(ulong index)
	{
		foreach (var func in hashfuncs)
		{
			ulong tableIndex = GetHashIndex(func, index);
			RBFCell cell = data[tableIndex];
			if (cell.count == 0)
			{
				return null;
			}
			else if (cell.count == 1)
			{
				if (cell.keySum == index)
				{
					return cell.valueSum;
				}
				else
				{
					return null;
				}
			}
		}
		throw new NotImplementedException();
	}
	public IList<(ulong, int)> ListEntries()
	{
		List<(ulong, int)> ansver = new List<(ulong, int)>();
		List<ulong> ones = new List<ulong>();
		for (ulong i = 0; i < data.Length; i++)
			if (data[i].count == 1)
			{
				ones.Add(i);
			}

		while (ones.Count > 0)
		{
			var cell = data[ones[ones.Count - 1]];
			if (cell.count == 1)
			{
				ansver.Add(new(cell.keySum, cell.valueSum));
				ones.RemoveAt(ones.Count - 1);
				Delete(cell.keySum, cell.valueSum);
				//If any deletion lead to count 0, we add them to ones
				//This is more expensive than should be as hashing function is called twice
				hashfuncs.ForEach(hashFunc =>
				{
					if (hashFunc(cell.keySum) == 1) { ones.Add(GetHashIndex(hashFunc, cell.keySum)); }
				}
				);
			}

		}
		return ansver;
	}

}

public interface ISketch
{
	void Add(ulong index, int value);
	void Delete(ulong index, int value);
	IList<(ulong, int)> ListEntries();
}

public class BasicSimpleSetSketcherToISketch : ISketch
{
	BasicSimpleSetSketcher sketcher;
	public BasicSimpleSetSketcherToISketch(BasicSimpleSetSketcher sketcher)
	{
		this.sketcher = sketcher;
	}

	public IList<(ulong, int)> ListEntries()
	{
		throw new NotImplementedException();
	}

	void Add(ulong index, int value)
	{
		sketcher.Toogle((int)index);
	}

	void ISketch.Add(ulong index, int value)
	{
		throw new NotImplementedException();
	}

	void Delete(ulong index, int value)
	{
		sketcher.Toogle((int)index);
	}

	void ISketch.Delete(ulong index, int value)
	{
		throw new NotImplementedException();
	}
}
*/
public interface ISketchHashFunction
{
	/// <summary>
	/// Expect a int to be hashed, returns 3 independant hashes
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	(ulong, ulong, ulong) GetHash(ulong x);
}

public class Md5Simple : ISketchHashFunction
{

	MD5 md5;
	public Md5Simple()
	{
		md5 = MD5.Create();
	}

	public (ulong, ulong, ulong) GetHash(ulong number)
	{
		//I do there hashing for the price of one
		var hash = md5.ComputeHash(BitConverter.GetBytes(number));
		return (BitConverter.ToUInt64(hash, 0), BitConverter.ToUInt64(hash, 4), BitConverter.ToUInt64(hash, 8));
	}
}


public interface HashFunction
{
	int GetHash(int x);
}

/*public class SHA1Hash : ISketchHashFunction
{
	SHA1 sha1;
	uint seed;
	Random random = new Random();
	public SHA1Hash()
	{
		seed = (uint)(random.Next(1 << 30)) << 2 | (uint)(random.Next(1 << 2));
		sha1 = SHA1.Create();
	}

	public (ulong, ulong, ulong) GetHash(int number)
	{
		//I do there hashing for the price of one
		var hash = sha1.ComputeHash(BitConverter.GetBytes(number));
		return (BitConverter.Toulong32(hash, 0), BitConverter.Toulong32(hash, 4), BitConverter.Toulong32(hash, 8));
	}
}
*/
/*
public class Md5Hash
{
	MD5 md5;
	ulong seed;
	static Random random = new Random();
	public Md5Hash()
	{
		seed = (ulong)(random.Next(1 << 30)) << 2 | (ulong)(random.Next(1 << 2));
		md5 = MD5.Create();
	}

	public ulong GetHash(int number)
	{
		//I do there hashing for the price of one
		var hash = md5.ComputeHash(BitConverter.GetBytes(number + seed));
		return BitConverter.Toulong32(hash, 0);
	}
}
*/
