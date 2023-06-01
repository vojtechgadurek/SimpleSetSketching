using System.Drawing;
using Xunit.Sdk;

namespace Tests
{
    public class OneValueTest
    {
        [Fact]
        public void Test1()
        {
            var ketcher = new BasicSimpleSetSketcher(16);
            ketcher.Toogle(1);
            var ansver = ketcher.Decode();
            Assert.Equal(1, ansver.Data.Count);
            Assert.Contains(1, ansver.Data);
        }
        [Theory]
        [InlineData(4)]
        [InlineData(10)]
        [InlineData(100)]
        public void TestFailRange(uint range)
        {
            var ketcher = new BasicSimpleSetSketcher(range);
            for (int i = 1; i < 10; i++)
            {
                ketcher.Toogle(i);
            }
            for (int i = 2; i < 11; i++)
            {
                ketcher.Toogle(i);
            }
            var ansver = ketcher.Decode();
            Assert.Contains(1, ansver.Data);
            Assert.Contains(10, ansver.Data);
        }
    }
    public class ComplexTests
    {
        [Theory]
        [InlineData(100, 10, 20)]
        [InlineData(100, 10, 10)]
        [InlineData(100, 10, 8)]
        [InlineData(1000, 100, 180)]
        [InlineData(1000, 100, 120)]
        [InlineData(1000, 100, 100)]
        [InlineData(1000, 100, 80)]
        public void TestRandomData(uint size, uint different, uint sizeofsketch)
        {
            Random random = new Random(42);

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
           
            var ketcher = new BasicSimpleSetSketcher(sizeofsketch);
            foreach(var i in same)
            {
                ketcher.Toogle(i);
            }
            foreach (var i in samecopy)
            {
                ketcher.Toogle(i);
            }
            var ansver = ketcher.Decode();
            foreach (var i in other)
            {
                Assert.Contains(i, ansver.Data);
            } 
        }
    }
}