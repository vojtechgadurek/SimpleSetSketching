using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.Testing
{
	public interface ILogger<T>
	{
		void Log(T message);
		IList<T> GetLog();
	}
}
