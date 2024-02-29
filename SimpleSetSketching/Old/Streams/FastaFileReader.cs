using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public class Circle<T>
	{
		T[] data;
		int offset;
		public readonly int _size;
		public Circle(int size)
		{
			data = new T[size * 2];
			offset = 0;
			_size = size;
		}

		public void Set(int index, T item)
		{
			data[index] = item;
			data[index + _size] = item;
		}
		public void Push(T item)
		{
			Set(offset, item);
			offset++;
			if (offset == _size) offset = 0;
		}

		public T First()
		{
			return data[offset + 1];
		}
		public T Get(int index)
		{
			return data[index + offset];
		}
	}
	public class FastaFileReader : ISketchStreamProvider<ulong>
	{
		//Tohle není správně to jde z druhé strany
		//Je potřeba číst přečíst první řádek
		TextReader _reader;
		int _bufferOffset = 0;
		char[] _buffer;
		int _maxBufferOffest;
		K_Mer _K_Mer;
		ulong _lenght;
		int _lenghtOfKMer;
		UnsafeFixedSizeQueue<char> _charBuffer;


		public FastaFileReader(TextReader reader, int bufferSize)
		{
			(_lenghtOfKMer, _lenght) = ParseFirstLineData(reader.ReadLine());
			_reader = reader;

			_buffer = new char[bufferSize];
			_charBuffer = new UnsafeFixedSizeQueue<char>(_lenghtOfKMer);
			_K_Mer = K_Mer.Empty((int)_lenghtOfKMer);

			for (int i = 0; i < _lenghtOfKMer - 1; i++) PushChar((char)NextCharFromBuffer());

		}

		private (int, ulong) ParseFirstLineData(string header)
		{
			var splited = header.Split(' ');
			if (splited is null) throw new ArgumentException("Header is not in correct format");

			int k = int.Parse(splited[2].Split('=')[1]);
			ulong l = ulong.Parse(splited[1].Split('=')[1]);
			return (k, l);

		}

		private void PushChar(char c)
		{
			_charBuffer.Enqueue(c);
			_K_Mer = _K_Mer.PushInNewSymbol(c);
		}
		private ulong? TryCreateNewK_MerFromChar(char c)
		{
			if (c == '\n' || c == '\r') { return null; };
			PushChar(c);
			if (char.IsUpper((char)_charBuffer.Dequeue()))
			{
				return _K_Mer.data;
			}
			return null;
		}

		void FillLocalBuffer()
		{
			_maxBufferOffest = _reader.Read(_buffer, 0, _buffer.Length);
			_bufferOffset = 0;
		}

		public char? NextCharFromBuffer()
		{
			if (_bufferOffset == _maxBufferOffest)
			{
				FillLocalBuffer();
			}
			if (_bufferOffset == _maxBufferOffest)
			{
				return null;
			}
			else
			{
				return _buffer[_bufferOffset++];
			}
		}


		public void FillBuffer(ulong[] buffer, out int maxOffset)
		{
			int count = 0;
			while (count < buffer.Length)
			{
				char? nextCharFromBufferToPush = NextCharFromBuffer();

				if (nextCharFromBufferToPush == null) break;

				if (TryCreateNewK_MerFromChar((char)nextCharFromBufferToPush) is not null) buffer[count++] = _K_Mer.data;
			}

			maxOffset = count;

			//Clear buffer
			if (count != buffer.Length)
			{
				while (count < buffer.Length)
				{
					buffer[count++] = 0;
				}
			}
		}

		public void Dispose()
		{
			_reader.Dispose();
		}

		public ulong? Length()
		{
			return _lenght;
		}
	}
}
