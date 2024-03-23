using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Testing
{
	public static class RandomTestingDataGenerator
	{
		public class RandomSketchStreamProvider : ISketchStream<ulong>
		{
			int offset;
			ulong[] data;

			public RandomSketchStreamProvider(ulong[] data)
			{
				this.data = data;
				offset = 0;
			}

			public void Dispose()
			{
				//There is nothing to dispose
			}

			public void FillBuffer(ulong[] buffer, out int maxOffest)
			{
				if (offset + buffer.Length < data.Length)
				{
					Array.Copy(data, offset, buffer, 0, buffer.Length);
					offset += buffer.Length;
					maxOffest = buffer.Length;
				}
				else
				{
					int count = 0;
					while (offset + count < data.Length)
					{
						buffer[count] = data[count + offset];
						count++;
					}
					maxOffest = count;
					while (count < buffer.Length)
					{
						buffer[count] = 0;
						count++;
					}
					offset += count;
				}
			}

			public TruncatedArray<ulong> FillBuffer(ulong[] buffer)
			{
				throw new NotImplementedException();
			}

			public ulong? Length()
			{
				return (ulong)data.Length;
			}

			uint? ISketchStream<ulong>.Length()
			{
				throw new NotImplementedException();
			}
		}
	}
}
