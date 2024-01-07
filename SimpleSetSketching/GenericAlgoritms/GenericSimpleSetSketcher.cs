namespace SimpleSetSketching
{
	internal class GenericSimpleSetSketcher<TValue, TFirstHashingFunction, TSecondHashingFunction, TThirdHashingFunction, TTable, TSize>
		where TValue : struct, IValue
		where TFirstHashingFunction : struct, IHashingFunction<TValue, int>
		where TSecondHashingFunction : struct, IHashingFunction<TValue, int>
		where TThirdHashingFunction : struct, IHashingFunction<TValue, int>
		where TTable : struct, ITable<TValue>
		where TSize : struct, IConstant<int>
	{
		/// <summary>
		/// Generic implementation of SimpleSetSketch algorithm
		/// </summary>
		TTable _table;
		TFirstHashingFunction _firstHashingFunction;
		TSecondHashingFunction _secondHashingFunction;
		TThirdHashingFunction _thirdHashingFunction;
		int _shotdownMultiplicator = 10;
		int _size;

		public GenericSimpleSetSketcher(TTable table, TFirstHashingFunction firstHashingFunction, TSecondHashingFunction secondHashingFunction, TThirdHashingFunction thirdHashingFunction, TSize size)
		{
			_table = table;
			_firstHashingFunction = firstHashingFunction;
			_secondHashingFunction = secondHashingFunction;
			_thirdHashingFunction = thirdHashingFunction;
			_size = size.Get();
		}

		public void Toogle(TValue value)
		{
			_table.Set(_firstHashingFunction.Hash(value), value);
			_table.Set(_secondHashingFunction.Hash(value), value);
			_table.Set(_thirdHashingFunction.Hash(value), value);
		}
		public bool LooksPure(int index)
		{
			TValue number = _table.Get(index);
			if (number.IsZero())
			{
				return false;
			}
			var firstHash = _firstHashingFunction.Hash(number);
			if (firstHash == index)
			{
				return true;
			}
			var secondHash = _secondHashingFunction.Hash(number);
			if (secondHash == index)
			{
				return true;
			}
			var thirdHash = _thirdHashingFunction.Hash(number);
			if (thirdHash == index)
			{
				return true;
			}
			return false;
		}

		void AddIfPure(int index, HashSet<int> pure)
		{

			if (LooksPure(index))
			{
				pure.Add(index);
			}
		}

		public HashSet<TValue>? TryDecode()
		{
			HashSet<TValue> ansver = new HashSet<TValue>();
			HashSet<int> pure = new HashSet<int>();
			for (int i = 0; i < _table.Length(); i++)
			{
				AddIfPure(i, pure);
			}
			int hardStop = _table.Length() * _shotdownMultiplicator; //Tohle je nějaká random konstatnta, aby se to necyklilo
			int rounds = 0;
			while (pure.Count > 0)
			{
				rounds++;
				if (rounds > hardStop)
				{
					return null;
				}
				HashSet<int> nextPure = new HashSet<int>();
				foreach (var i in pure)
				{
					if (!LooksPure(i))
					{
						continue;
					}
					TValue x = _table.Get(i);
					Toogle(x);
					if (ansver.Contains(x))
					{
						ansver.Remove(x);
					}
					else
					{
						ansver.Add(x);
					}

					AddIfPure(_firstHashingFunction.Hash(x), nextPure);
					AddIfPure(_secondHashingFunction.Hash(x), nextPure);
					AddIfPure(_thirdHashingFunction.Hash(x), nextPure);
				}
				pure = nextPure;
			}

			if (!_table.IsEmpty())
			{
				return null;
			};
			return ansver;
		}

	}
}
