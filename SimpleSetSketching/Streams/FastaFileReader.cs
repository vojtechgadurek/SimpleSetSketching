using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SimpleSetSketching
{
	public class FastaFileReader : ISketchStreamProvider<ulong>
	{
		StreamReader reader;
		int bufferOffset = 0;
		char[] _buffer;
		int maxBufferOffest;
		K_Mer current = K_Mer.Empty;

		public FastaFileReader(string path, int bufferSize)
		{
			reader = new StreamReader(path);
			reader.ReadLine();
			_buffer = new char[bufferSize];
			for (int i = 0; i < K_Mer.K - 1; i++)
			{
				char? nextBuffer = NextCharFromBuffer();
				if (nextBuffer == null) break;
				TryCreateNewK_MerFromChar((char)nextBuffer);
			}
		}

		public bool TryCreateNewK_MerFromChar(char c)
		{
			if (c == '\n' || c == '\r') { return false; };

			current = current.PushInNewSymbol(c);
			return Char.IsUpper(c);
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
	}
}
