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

		StreamReader reader;
		int bufferOffset = 0;
		char[] _buffer;
		int maxBufferOffest;
		K_Mer current;
		Circle<char> chars;
		ulong lenght;
		int lenghtOfKMer;
		

		public FastaFileReader(string path, int bufferSize)
		{
			reader = new StreamReader(path);
			(lenghtOfKMer, lenght) = ParseFirstLineData(reader.ReadLine());
			

			_buffer = new char[bufferSize];


			chars = new Circle<char>((int)lenghtOfKMer);
			current = K_Mer.Empty((int)lenghtOfKMer);



			for (int i = 0; i < (int) lenghtOfKMer - 1; i++)
			{
				char? nextBuffer = NextCharFromBuffer();
				if (nextBuffer == null) break;
				TryCreateNewK_MerFromChar((char)nextBuffer);
			}


		}

		public (int, ulong) ParseFirstLineData(string header) {
			var splited = header.Split(' ');
			if (splited is null ) throw new ArgumentException("Header is not in correct format");

			int k = int.Parse(splited[2].Split('=')[1]);
			ulong l = ulong.Parse(splited[1].Split('=')[1]);
			return(k, l);

		}

		public bool TryCreateNewK_MerFromChar(char c)
		{
			if (c == '\n' || c == '\r') { return false; };

			chars.Push(c);
			current = current.PushInNewSymbol(c);
			return char.IsUpper(chars.First());
		}

		void FillLocalBuffer()
		{
			maxBufferOffest = reader.Read(_buffer, 0, _buffer.Length);
			bufferOffset = 0;
		}

		public char? NextCharFromBuffer()
		{
			if (bufferOffset == maxBufferOffest)
			{
				FillLocalBuffer();
			}
			if (bufferOffset == maxBufferOffest)
			{
				return null;
			}
			else
			{
				return _buffer[bufferOffset++];
			}
		}


		public void FillBuffer(ulong[] buffer, out int maxOffset)
		{
			int count = 0;
			while (count < buffer.Length)
			{
				char? nextBuffer = NextCharFromBuffer();

				if (nextBuffer == null) break;

				if (TryCreateNewK_MerFromChar((char)nextBuffer)) buffer[count++] = current.data;
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
			reader.Dispose();
		}

		public ulong? Length()
		{
			return (ulong)reader.BaseStream.Length;
		}
	}
}
