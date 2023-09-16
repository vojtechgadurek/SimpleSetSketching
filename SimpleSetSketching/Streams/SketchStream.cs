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
		ISketchStreamProvider<ulong> provider;
		public SketchStream(ISketchStreamProvider<ulong> provider, int bufferSize)
		{
			this.provider = provider;
			buffer = new ulong[bufferSize];
		}
		/// <summary>
		/// If the stream is empty, returns default(T)
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
	}
}
