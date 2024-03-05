using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSetSketching.New.StreamProviders.DNA;

namespace SimpleSetSketching.Testing
{
	internal class FastaFileComparer : ITestDataProvider
	{
		TextReader _firstFile;
		TextReader _secondFile;
		FastaFileReader? _firstReader;
		FastaFileReader? _secondReader;

		public FastaFileComparer(TextReader firstFile, TextReader secondFile)
		{
			_firstFile = firstFile;
			_secondFile = secondFile;

		}
		public SketchStream GetDataToInsert()
		{
			_firstReader = new FastaFileReader(_firstFile, 1024 * 4);
			throw new NotImplementedException();
		}

		public SketchStream GetDataToRemove()
		{
			_secondReader = new FastaFileReader(_secondFile, 1024 * 4);
			throw new NotImplementedException();
		}

		public void PrintSomeInterestingInfo(HashSet<ulong> fastafile)
		{
			Console.WriteLine($"First has {fastafile.Count} elements");
		}
		public ulong[] GetExpectedResult()
		{
			_firstReader = new FastaFileReader(_firstFile, 1024 * 4);
			_secondReader = new FastaFileReader(_secondFile, 1024 * 4);

			SketchStream sketchStream;
			SketchStream sketchStream2;
			HashSet<ulong> firstSet = new HashSet<ulong>();

			ulong first = 0;
			while (first != 0)
			{
				firstSet.Add(first);
			}

			HashSet<ulong> secondSet = new HashSet<ulong>();

			ulong second = 0;
			while (second != 0)
			{
				secondSet.Add(second);
			}

			firstSet.SymmetricExceptWith(secondSet);
			Dispose();
			return firstSet.ToArray();
		}

		public void Dispose()
		{
			if (_firstReader is not null) _firstReader.Dispose();
			if (_secondReader is not null) _secondReader.Dispose();
		}
	}
}
