using System.Drawing;
using Xunit.Sdk;
using SimpleSetSketching;
//fasta soubory .fa
//> TCC  



namespace Tests
{
	public class OneValueTest
	{
		/*
		[Fact]
		public void Test1()
		{
			var ketcher = new BasicSimpleSetSketcher(16, new Md5Simple());
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
			var sketcher = new BasicSimpleSetSketcher(range, new Md5Simple());
			for (int i = 1; i < 10; i++)
			{
				sketcher.Toogle(i);
			}
			for (int i = 2; i < 11; i++)
			{
				sketcher.Toogle(i);
			}
			var ansver = sketcher.Decode();
			Assert.Contains(1, ansver.Data);
			Assert.Contains(10, ansver.Data);
		}
	}
	public class LongTest
	{
		[Theory]
		[InlineData(100, 10, 20)]
		[InlineData(100, 10, 10)]
		[InlineData(100, 10, 8)]
		[InlineData(1000, 100, 180)]
		[InlineData(1000, 100, 200)]
		[InlineData(1000, 100, 300)]
		[InlineData(1000, 100, 400)]
		public void TestRandomData(uint size, uint different, uint sizeofsketch)
		{
			Random random = new Random(42);


			ComplexTests comtest = new ComplexTests();
			var ketcher = new BasicSimpleSetSketcher(sizeofsketch, new Md5Simple());



			Assert.True(comtest.TestSketcher(size, different, 42, (input) => ketcher.Toogle(input), (input) => ketcher.Toogle(input), () => ketcher.Decode().Data));

		}
	}
	*/
	}
}
