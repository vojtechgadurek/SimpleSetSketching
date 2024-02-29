using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Testing
{
	public class BasicLogger<T> : ILogger<T>
	{
		IList<T> list;
		bool vocal;
		public BasicLogger(bool vocal = false)
		{
			this.list = new List<T>();
			this.vocal = vocal;
		}
		public void Log(T item)
		{
			if (vocal)
				Console.WriteLine(item);
			list.Add(item);
		}

		public IList<T> GetLog()
		{
			{ return list; }
		}
	}
}
