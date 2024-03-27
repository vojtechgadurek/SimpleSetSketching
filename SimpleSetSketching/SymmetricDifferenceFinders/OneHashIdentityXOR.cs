using SimpleSetSketching.Decoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.SymmetricDifferenceFinders
{
	public class OneHashIdentityXOR<TTable> where TTable : IEnumerable<ulong>
	{
		public Toggler<TTable> Toggler;
		public SimpleSetDecoder<TTable> Decoder;

		public OneHashIdentityXOR(TTable table, int bufferSize)
		{
			int size = table.Count();
			var identityHashFunc = ModuloHashingFunctionProvider.Create((ulong)size, new Random());
			var togglingAction = SimpleSetSketchFunc.GetXorToggle<TTable>();

			Toggler = new Toggler<TTable>(bufferSize, table, new HashingFunctionExpression[] { identityHashFunc }, togglingAction);
			Decoder = new SimpleSetDecoder<TTable>(new HashingFunctionExpression[] { identityHashFunc }, table, size, togglingAction);
		}
	}
}
