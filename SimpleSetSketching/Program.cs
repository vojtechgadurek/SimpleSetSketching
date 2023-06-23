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
using System.Security.AccessControl;
using System.Security.Cryptography;

#nullable enable
public class Program {
    public static void Main(string[] args) 
    {
        ComplexTests complexTests = new ComplexTests();
        var Clock = System.Diagnostics.Stopwatch.StartNew();
        var ansver = complexTests.TestMultipleMultiplicator(1000, 1, 0, 1.3, 10000, 1000, 42, true);
        foreach (var item in ansver)
        {
            var statesGrouped = item.data.GroupBy(x => x.state);
            Console.Write($"{item.sizeOfSketchMultiplicator} ");
            foreach(var state in statesGrouped)
            {
                Console.Write($"{state.Key} {state.Count()} ");
            }
            Console.Write($"time {item.data.Sum(x => x.timeTaken)/ item.data.Count}");
            Console.WriteLine();
        }
        Console.WriteLine($"Total time spent computinng {ansver.Sum(x => x.data.Sum(y => y.timeTaken))}");
        Console.WriteLine($"Total time spent shotdown {ansver.Sum(x => x.data.Where(z => z.state == ComplexTests.DecodingState.shotDown).Sum(y => y.timeTaken))}");
        Console.WriteLine($"Total time {Clock.ElapsedMilliseconds}");
        
    }
}

public enum DecodeState
{
    ok,
    notAbleToDecode,
    shotDown,
}
public record DecodedBasicSimpleSetSketcherToken ( DecodeState state, IList<int> Data);
public class BasicSimpleSetSketcher { 
    int[] _data;
    ISketchHashFunction _hashFunc = new Md5Simple();
    int _shotDownMultiplicator = 10; //Sets maximum number of decoding rounds to prevent infinite loop
    public BasicSimpleSetSketcher(uint size)
    {
        _data = new int[size];
    }
    public int[] GetData()
    {
        return _data;
    }

    private (uint, uint, uint) GetTruncatedHash(int index)
    {
        var hash = _hashFunc.GetHash(index);
        return ((uint) (hash.Item1 % _data.Length), (uint) (hash.Item2 % _data.Length), (uint) (hash.Item3 % _data.Length));
    }
    public void Merge(BasicSimpleSetSketcher other)
    {
        for (int i = 0; i < _data.Length; i++)
        {
            var otherData = other.GetData();
            _data[i] ^= otherData[i];
        }
    }
    public bool LooksPure(uint index, int[] data)
    {
        var hash = GetTruncatedHash(data[index]);
        return ((data[index] != 0))&& (hash.Item1 == index || hash.Item2 == index || hash.Item3 == index);
    }
    public DecodedBasicSimpleSetSketcherToken Decode()
    {
        DecodedBasicSimpleSetSketcherToken ansver;
        if (TryDecode(out ansver))
        {
            return ansver;
        }
        else
        {
            //There should be some better exception
            throw new Exception($"It was not possible to decode {this}");
        }
    }
    public bool TryDecode(out DecodedBasicSimpleSetSketcherToken decodedAnsver)
    {
        HashSet<int>ansver = new HashSet<int>();
        HashSet<uint> pure = new HashSet<uint>();
        //Copydata allow Decode to be non destructive
        int[] copydata = (int[]) this._data.Clone();
        for (uint i = 0; i < copydata.Length; i++)
        {
            if (LooksPure(i, copydata))
            {
                pure.Add(i);
            }
        }
        int hardStop = copydata.Length * _shotDownMultiplicator; //Tohle je nějaká random konstatnta, aby se to necyklilo
        int rounds = 0;
        while (pure.Count > 0)
        {
            rounds++;
            if (rounds > hardStop)
            {
                decodedAnsver = new DecodedBasicSimpleSetSketcherToken(DecodeState.shotDown, new List<int>());
                return false;
            }
            HashSet<uint> nextPure = new HashSet<uint>();
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
                (uint, uint, uint) hash = GetTruncatedHash(x);
                void AddIfPure(uint index)
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
        
        if(!copydata.All(x => x== 0))
        { 
            decodedAnsver = new DecodedBasicSimpleSetSketcherToken(DecodeState.notAbleToDecode, new List<int>());
            return false;
        };
        decodedAnsver = new DecodedBasicSimpleSetSketcherToken(DecodeState.ok, ansver.ToList());
        return true;
    }
    private void Toogle(int x, int[] data)
    {
        var hash = GetTruncatedHash(x);
        void SmallToogle(uint index)
        {
            data[index] ^= x;
        }
        SmallToogle(hash.Item1);
        SmallToogle(hash.Item2);
        SmallToogle(hash.Item3);
    }
    public void Toogle(int x)
    {
        Toogle(x, _data);
    }
}


class ReverseBloomFilter
{
    record struct RBFCell(int count, uint keySum, int valueSum);
    RBFCell[] data;
    List<Func<uint, uint>> hashfuncs;

    public uint GetHashIndex(Func<uint, uint> hashFunc, uint index)
    {
        return (uint) (hashFunc(index) % data.Length);
    }
    public void InsertByOneHashFunc(Func<uint, uint> hashFunc, uint index, int value)
    {

        uint tableIndex = GetHashIndex(hashFunc, index);
        RBFCell originalCell = data[tableIndex];
        data[tableIndex] = new RBFCell(originalCell.count + 1, 
                originalCell.keySum + index, 
                originalCell.valueSum + value);
    }
    public void Insert(uint index, int value)
    {
        hashfuncs.ForEach(hashFunc => InsertByOneHashFunc(hashFunc, index, value));
    }

    public void DeleteByOneHashFunc(Func<uint, uint> hashFunc, uint index, int value)
    {
        uint tableIndex = GetHashIndex(hashFunc, index);
        RBFCell originalCell = data[tableIndex];
        data[tableIndex] = new RBFCell(originalCell.count - 1,
                           originalCell.keySum - index,
                                          originalCell.valueSum - value);
    }
    public void Delete(uint index, int value)
    {
        hashfuncs.ForEach(hashFunc => DeleteByOneHashFunc(hashFunc, index, value));
    }

    public int? Get(uint index)
    {
        foreach(var func in hashfuncs)
        {
            uint tableIndex = GetHashIndex(func, index);
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
    public IList<(uint, int)> ListEntries()
    {
        List<(uint, int)> ansver = new List<(uint, int)>();
        List<uint> ones = new List<uint>();
        for (uint i = 0; i < data.Length; i++)
            if (data[i].count == 1)
            {
                ones.Add(i);
            }

        while (ones.Count > 0) {
            var cell = data[ones[ones.Count - 1]];
            if (cell.count == 1)
            {
                ansver.Add(new (cell.keySum, cell.valueSum));
                ones.RemoveAt(ones.Count - 1);
                Delete(cell.keySum, cell.valueSum);
                //If any deletion lead to count 0, we add them to ones
                //This is more expensive than should be as hashing function is called twice
                hashfuncs.ForEach(hashFunc => {
                    if (hashFunc(cell.keySum) == 1)  { ones.Add(GetHashIndex(hashFunc, cell.keySum)); } }
                );
            }

        }
        return ansver;
    }

}
interface ISketchHashFunction
{
    /// <summary>
    /// Expect a int to be hashed, returns 3 independant hashes
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    (uint, uint, uint) GetHash(int x);
}

class Md5Simple :ISketchHashFunction
{
    
    MD5 md5;
    public Md5Simple()
    {
        md5 = MD5.Create();
    }

    public (uint, uint, uint) GetHash(int number)
    {
        //I do there hashing for the price of one
        var hash = md5.ComputeHash(BitConverter.GetBytes(number));
        return (BitConverter.ToUInt32(hash, 0), BitConverter.ToUInt32(hash, 4), BitConverter.ToUInt32(hash, 8));
    }
}

public interface HashFunction
{
    int GetHash(int x);
}

public class Md5Hash
{
    MD5 md5;
    uint seed;
    public Md5Hash(Random random)
    {
        seed = (uint)(random.Next(1 << 30)) << 2 | (uint)(random.Next(1 << 2));
        md5 = MD5.Create();
    }

    public uint GetHash(int number)
    {
        //I do there hashing for the price of one
        var hash = md5.ComputeHash(BitConverter.GetBytes(number + seed));
        return BitConverter.ToUInt32(hash, 0);
    }
}

public class ComplexTests
{
    /// <summary>
    ///  Generate triplet, first two being two list containg numbers, last being their symetric difference
    /// </summary>
    /// <param name="numberOfSameElements"></param>
    /// How much elements should be in both sets
    /// <param name="numberOfDifferentElements"></param>
    /// Size of symetric difference of the two sets
    /// <param name="seed"></param>
    /// <returns></returns>
    (IList<int>, IList<int>, HashSet<int>) GenerateRandomSymetricDifferenceData(uint numberOfSameElements, uint numberOfDifferentElements, int seed )
    {
        Random random = new Random(seed);
        //Create two sets one containg elements to be in both sets, one containing elements to be in only one set

        //Create same elements set
        HashSet<int> bothSetsElements = new HashSet<int>();
        while (bothSetsElements.Count < numberOfSameElements)
        {
            bothSetsElements.Add(random.Next());
        }

        //Create other elements set
        HashSet<int> onlyOneSetElements = new HashSet<int>();
        while (onlyOneSetElements.Count < numberOfDifferentElements)
        {
            if (!bothSetsElements.Contains(random.Next()))
            {
                onlyOneSetElements.Add(random.Next());
            }
        }

        //Create sets for symetrical difference
        HashSet<int> firstSet = new HashSet<int>(bothSetsElements);
        HashSet<int> secondSet = new HashSet<int>(bothSetsElements);

        //Splits elements to be in only one set between the two sets
        foreach (var i in onlyOneSetElements)
        {
            if (random.Next(2) == 0)
            { secondSet.Add(i); }
            else
            {
                firstSet.Add(i);
            }

        }

        var firstShuffled = firstSet.OrderBy(x => random.Next()).ToList();
        var secondShuffled = secondSet.OrderBy(x => random.Next()).ToList();
        return (firstShuffled, secondShuffled, onlyOneSetElements);
    }
    public void RunTests(uint size)
    {

    }

    public enum DecodingState
    {
        ok,
        shotDown,
        notAbleToDecode,
        badDecoding
    }
    public record struct DecondingToken (long timeTaken, DecodingState state);


    DecondingToken RunOneTest(uint numberOfSameElements, uint numberOfDifferentElements, int seed, uint sizeofsketch)
    {
        var (firstSet, secondSet, symetricDifference) = GenerateRandomSymetricDifferenceData(numberOfSameElements, numberOfDifferentElements, seed);
        var sketcher = new BasicSimpleSetSketcher(sizeofsketch);
        foreach (var i in firstSet)
        {
            sketcher.Toogle(i);
        }
        foreach (var i in secondSet)
        {
            sketcher.Toogle(i);
        }
        
        DecodedBasicSimpleSetSketcherToken decodedDataToken;

        //Test decoding

        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        sketcher.TryDecode(out decodedDataToken);
        stopWatch.Stop();
        var elapsedMs = stopWatch.ElapsedMilliseconds;

        //Test if decoding was correct, state is ok and all elements are in symetric difference
        if (decodedDataToken.state == DecodeState.ok &&
            (!decodedDataToken.Data.All(x => symetricDifference.Contains(x) 
            || !(decodedDataToken.Data.Count == symetricDifference.Count)))
            )
        {
            return new DecondingToken(elapsedMs, DecodingState.badDecoding);
        }
        
        switch (decodedDataToken.state)
        {
            case DecodeState.ok:
                return new DecondingToken(elapsedMs, DecodingState.ok);
            case DecodeState.shotDown:
                return new DecondingToken(elapsedMs, DecodingState.shotDown);
            case DecodeState.notAbleToDecode:
                return new DecondingToken(elapsedMs, DecodingState.notAbleToDecode);
            default:
                throw new Exception("Unknown decoding state");
        }
    }

    public record class TestResult(double sizeOfSketchMultiplicator, List<DecondingToken> data);
    public TestResult RunTests(int numberOfRounds, uint numberOfSameElements, uint numberOfDifferentElements, int seed, double sizeOfSketchMultiplicator)
    {
        uint sizofsketch = (uint)(sizeOfSketchMultiplicator * (numberOfDifferentElements));
        List<DecondingToken> ansver = new List<DecondingToken>();
        for (int i = 0; i < numberOfRounds; i++)
        {
            ansver.Add(RunOneTest(numberOfSameElements, numberOfDifferentElements, seed + i, sizofsketch));
        }
        return new TestResult(sizeOfSketchMultiplicator, ansver);
    }

    public IList<TestResult> TestMultipleMultiplicator(int numberOfRounds,int numberOfTests, double  stepSize, double StartMultiplicator, uint numberOfSameElements, uint numberOfDifferentElements, int seed, bool vocal)
    {
        IList<TestResult> ansver = new List<TestResult>();
        for (int i = 0; i < numberOfTests; i++)
        {
            if (vocal)
            {
                Console.WriteLine("Test number " + i + " out of " + numberOfTests);
            }
            ansver.Add(RunTests(numberOfRounds, numberOfSameElements, numberOfDifferentElements, seed + i*numberOfTests, StartMultiplicator + i * stepSize));
        }
        return ansver;

    }
    
}