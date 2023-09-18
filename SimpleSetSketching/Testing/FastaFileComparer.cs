using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Testing
{
	internal class FastaFileComparer : ITestDataProvider
	{
		string _firstFile;
		string _secondFile;
		FastaFileReader _firstReader;
		FastaFileReader _secondReader;

		public FastaFileComparer(string firstFile, string secondFile)
		{
			_firstFile = firstFile;
			_secondFile = secondFile;

		}
		public SketchStream GetDataToInsert()
		{
			_firstReader = new FastaFileReader(_firstFile, 1024 * 4);
			return new SketchStream(_firstReader, 1024);
		}

		public SketchStream GetDataToRemove()
		{
			_secondReader = new FastaFileReader(_secondFile, 1024 * 4);
			return new SketchStream(_secondReader, 1024);
		}

		public void PrintSomeInterestingInfo(HashSet<ulong> fastafile)
		{
			Console.WriteLine($"First has {fastafile.Count} elements");
		}
		public ulong[] GetExpectedResult()
		{
			_firstReader = new FastaFileReader(_firstFile, 1024 * 4);
			_secondReader = new FastaFileReader(_secondFile, 1024 * 4);

			SketchStream sketchStream = new SketchStream(_firstReader, 1024);
			SketchStream sketchStream2 = new SketchStream(_secondReader, 1024);

			HashSet<ulong> firstSet = new HashSet<ulong>();

			ulong first = sketchStream.Next();
			while (first != 0)
			{
				first = sketchStream.Next();
				firstSet.Add(first);
			}

			HashSet<ulong> secondSet = new HashSet<ulong>();

			ulong second = sketchStream2.Next();
			while (second != 0)
			{
				second = sketchStream2.Next();
				secondSet.Add(second);
			}

			firstSet.SymmetricExceptWith(secondSet);
			Dispose();
			return firstSet.ToArray();
		}

		public void Dispose()
		{
			_firstReader.Dispose();
			_secondReader.Dispose();
		}
	}
}
