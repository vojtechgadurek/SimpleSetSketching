using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.GenericAlgoritms
{
	static class GenericAlgorithmCreator<TValueType, TFirstHashingFunction, TSecondHashingFunction, TThirdHashingFunction, TTable>
	where TValueType : struct
	where TFirstHashingFunction : IHashingFunction<TValueType, ulong>
	where TSecondHashingFunction : IHashingFunction<TValueType, ulong>
	where TThirdHashingFunction : IHashingFunction<TValueType, ulong>
	{
		public static (Type, object) Create(
			uint size, TTable table,
			TFirstHashingFunction firstHashingFunction,
			TSecondHashingFunction secondHashingFunction,
			TThirdHashingFunction thirdHashingFunction
			)
		{
			Type constant = DynamicConstantTypeCreator<uint>.GetConstant(size);
			object? sketcher =
				Activator.CreateInstance(typeof(GenericSimpleSetSketcher<,,,,,>)
				.MakeGenericType(
					typeof(TValueType),
					typeof(TFirstHashingFunction),
					typeof(TSecondHashingFunction),
					typeof(TThirdHashingFunction),
					typeof(TTable),
					constant),
					table,
					firstHashingFunction,
					secondHashingFunction,
					thirdHashingFunction
				);

			if (sketcher == null)
			{
				throw new InvalidOperationException($"Failed to create a sketcher");
			}
			return (sketcher.GetType(), sketcher);
		}
	}
}
