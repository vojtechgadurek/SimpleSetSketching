using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleSetSketching
{
	public class SketchStream : ISketchStreamProvider<ulong>
	{
		int offest;
		int maxOffest;
		ulong[] buffer;
		public int BufferSize { get { return buffer.Length; } }
		ISketchStreamProvider<ulong> provider;
		public SketchStream(ISketchStreamProvider<ulong> provider, int bufferSize)
		{
			this.provider = provider;
			buffer = new ulong[bufferSize];
		}
		/// <summary>
		/// If the stream is empty, returns default(TValue)
		/// </summary>
		/// <returns></returns>
		public ulong Next()
		{

			if (offest == maxOffest)
			{
				//Load new 
				offest = 0;
				provider.FillBuffer(buffer, out maxOffest);
			}
			if (maxOffest == 0) return 0;
			return buffer[offest++];
		}
		/// <summary>
		/// Provide buffer to data provider to fill it with data as such it does not use its own buffer in any way, data may not thus be in same order.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="maxOffset"></param>
		public void FillBuffer(ulong[] buffer, out int maxOffset)
		{
			lock (this)
			{
				provider.FillBuffer(buffer, out maxOffset);
			}
		}

		public void Dispose()
		{
			provider.Dispose();
		}

		public ulong? Length()
		{
			return provider.Length();
		}
	}
}
