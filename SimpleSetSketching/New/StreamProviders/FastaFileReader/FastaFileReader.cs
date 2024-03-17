using SimpleSetSketching.New.StreamProviders.DNA.KMerCreators;
using SimpleSetSketching.New.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.New.StreamProviders.DNA
{
	public class FastaFileReader : ISketchStream<ulong>
	{
		//Tohle není správně to jde z druhé strany
		//Je potřeba číst přečíst první řádek
		TextReader _reader;

		Buffer<char> _buffer;
		KMerWithComplement _currentKMer;
		bool _lastCharReaded = false;


		readonly uint _lenght;
		readonly int _lenghtOfKMer;

		readonly UnsafeFixedSizeQueue<char> _charBuffer;
		readonly KMerCreator _KMerCreator;



		public FastaFileReader(TextReader reader, int bufferSize)
		{
			(_lenghtOfKMer, _lenght) = ParseFirstLineData(reader.ReadLine());
			_reader = reader;


			_buffer = new Buffer<char>(bufferSize, new char[bufferSize], 0);
			_charBuffer = new UnsafeFixedSizeQueue<char>(_lenghtOfKMer);

			_KMerCreator = new KMerCreator(_lenghtOfKMer);
			_currentKMer = _KMerCreator.AllDefaultKMer;
			for (int i = 0; i < _lenghtOfKMer - 1; i++) PushChar((char)TryGetNextCharFromBuffer());
		}

		private (int, uint) ParseFirstLineData(string header)
		{
			//ToDo: add better parameter parsing
			var splited = header.Split(' ');
			if (splited is null) throw new ArgumentException("Header is not in correct format");

			int kMerLength = int.Parse(splited[2].Split('=')[1]);
			uint length = uint.Parse(splited[1].Split('=')[1]);
			return (kMerLength, length);

		}

		private void PushChar(char c)
		{
			_charBuffer.Enqueue(c);
			_currentKMer = _KMerCreator.PushSymbolIn(_currentKMer, _KMerCreator.TranslateCharToSymbol(c));
		}
		private ulong? TryReplaceCurrentKMerByPushingNewChar(char c)
		{
			if (c == '\n' || c == '\r') { return null; };
			PushChar(c);
			if (char.IsUpper((char)_charBuffer.Dequeue()))
			{
				return _currentKMer.GetCanonical().GetBinaryRepresentation();
			}
			return null;
		}

		bool FillLocalBuffer()
		{
			_buffer.Size = _reader.Read(_buffer.Array, 0, _buffer.Array.Length);
			_buffer.CurrentOffset = 0;
			if (_buffer.Size == 0)
			{
				_lastCharReaded = true;
				return false;
			}
			return true;
		}

		public char? TryGetNextCharFromBuffer()
		{
			if (_lastCharReaded || !FillLocalBuffer()) return null;
			else
			{
				char ansver = _buffer.Array[_buffer.CurrentOffset];
				_buffer.CurrentOffset += 1;
				return ansver;
			}
		}


		public TruncatedArray<ulong> FillBuffer(ulong[] buffer)
		{
			int numberOfElementsInBuffer = 0;
			while (numberOfElementsInBuffer < buffer.Length)
			{
				char? nextCharFromBufferToPush = TryGetNextCharFromBuffer();

				if (nextCharFromBufferToPush == null) break;

				if (TryReplaceCurrentKMerByPushingNewChar((char)nextCharFromBufferToPush) is not null) buffer[numberOfElementsInBuffer++] = _currentKMer.GetCanonical().GetBinaryRepresentation();
			}


			//Clear buffer
			if (numberOfElementsInBuffer != buffer.Length)
			{
				while (numberOfElementsInBuffer < buffer.Length)
				{
					buffer[numberOfElementsInBuffer++] = 0;
				}
			}

			return new TruncatedArray<ulong>(numberOfElementsInBuffer, buffer);
		}

		public void Dispose()
		{
			_reader.Dispose();
		}

		public uint? Length()
		{
			return _lenght;
		}
	}
}
