using System;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Dynamic;
using System.Formats.Asn1;
using System.Numerics;
using System.Security.AccessControl;
using System.Security.Cryptography;

public class Program {
    public static void Main(string[] args) 
    {
        var complexTest = new ComplexTests();
        int count = 0;
        for (int i = 0; i < 100; i++)
        {
            if (complexTest.TestRandomData(3000, 250 , 512, i))
            {
                count++;
            }
        }
        Console.WriteLine($"Success rate: {count}");

    }
}
public class BasicSimpleSetSketcher { 
    int[] data;
    IHashFunc hashFunc = new Md5Simple();
    public BasicSimpleSetSketcher(uint size)
    {
        data = new int[size];
    }
    public int[] GetData()
    {
        return data;
    }

    private (uint, uint, uint) GetTruncatedHash(int index)
    {
        var hash = hashFunc.GetHash(index);
        return ((uint) (hash.Item1 % data.Length), (uint) (hash.Item2 % data.Length), (uint) (hash.Item3 % data.Length));
    }
    public void Merge(BasicSimpleSetSketcher other)
    {
        for (int i = 0; i < data.Length; i++)
        {
            var otherData = other.GetData();
            data[i] ^= otherData[i];
        }
    }
    public bool LooksPure(uint index, int[] data)
    {
        var hash = GetTruncatedHash(data[index]);
        return ((data[index] != 0))&& (hash.Item1 == index || hash.Item2 == index || hash.Item3 == index);
    }
    public IList<int> Decode()
    {
        IList<int> ansver;
        if (TryDecode(out ansver))
        {
            return ansver;
        }
        else
        {
            throw new Exception($"It was not possible to decode {this}");
        }
    }
    public bool TryDecode(out IList<int> ansverOut)
    {
        HashSet<int>ansver = new HashSet<int>();
        HashSet<uint> pure = new HashSet<uint>();
        //Copydata allow Decode to be non destructive
        int[] copydata = (int[]) this.data.Clone();
        for (uint i = 0; i < copydata.Length; i++)
        {
            if (LooksPure(i, copydata))
            {
                pure.Add(i);
            }
        }
        int hardStop = copydata.Length * 100; //Tohle je nějaká random konstatnta, aby se to necyklilo
        int rounds = 0;
        while (pure.Count > 0)
        {
            rounds++;
            if (rounds > hardStop)
            {
                ansverOut = new List<int>();
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
        
        if(!copydata.All(x => x== 0)) { ansverOut = new List<int>(); return false; };
        ansverOut = ansver.ToList();
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
        Toogle(x, data);
    }
}

interface IHashFunc
{
    (uint, uint, uint) GetHash(int x);
}

class Md5Simple :IHashFunc
{
    MD5 md5;
    public Md5Simple()
    {
        md5 = MD5.Create();
    }
    public (uint, uint, uint) GetHash(int number)
    {

        var hash = md5.ComputeHash(BitConverter.GetBytes(number));
        return (BitConverter.ToUInt32(hash, 0), BitConverter.ToUInt32(hash, 4), BitConverter.ToUInt32(hash, 8));
    }
}

public class ComplexTests
{
    public bool TestRandomData(uint size, uint different, uint sizeofsketch, int seed)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        Random random = new Random(seed);

        (IList<int>, IList<int>, HashSet<int>) GenerateData(uint size, uint different)
        {
            HashSet<int> same = new HashSet<int>();
            while (same.Count < size)
            {
                same.Add(random.Next());
            }
            HashSet<int> other = new HashSet<int>();
            while (other.Count < different)
            {
                if (!same.Contains(random.Next()))
                {
                    other.Add(random.Next());
                }
            }
            HashSet<int> samecopy = new HashSet<int>(same);
            foreach (var i in other)
            {
                if (random.Next(2) == 0)
                { samecopy.Add(i); }
                else
                {
                    same.Add(i);
                }

            }
            return (same.OrderBy(x => random.Next()).ToList(), samecopy.OrderBy(x => random.Next()).ToList(), other);


        }
        var (same, samecopy, other) = GenerateData(size, different);

        Console.WriteLine($"Build data ended in {watch.Elapsed} ms");
        var ketcher = new BasicSimpleSetSketcher(sizeofsketch);
        foreach (var i in same)
        {
            ketcher.Toogle(i);
        }
        foreach (var i in samecopy)
        {
            ketcher.Toogle(i);
        }
        //If fails write a error message 
        IList<int> ansver;
        watch.Restart();
        Console.WriteLine("DecodingStarted");
        bool success = ketcher.TryDecode(out ansver);
        Console.WriteLine("DecodingTook" + watch.Elapsed + " ms");

        if (success)
        {
            other.SymmetricExceptWith(ansver);
            if (other.Count != 0)
            {
                success = false;
            }
        }
        Console.WriteLine($"Test {size}, {different}, {sizeofsketch}, {seed}, was succesful {success}");
        return success;
    }
}