using LittleSharp.Literals;
using SimpleSetSketching.Sketchers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleSetSketching.Decoders
{
	public class SimpleSetDecoder<TTable> : IDecoder<ulong>
	where TTable : IEnumerable<ulong>
	{

		readonly Action<ulong[], int, HashSet<ulong>, HashSet<ulong>, TTable> OneDecodeStep;
		readonly Action<TTable, int, HashSet<ulong>> InitDecoding;

		public uint CurrentIteration = 0;
		public uint BatchSize = 1;

		readonly TTable _table;
		int _size;
		readonly HashSet<ulong> _answer;
		HashSet<ulong> _pure;
		HashSet<ulong> _pureNext;
		readonly bool _isEven;
		public bool Initialized { get; private set; }
		public DecodingState State { get; private set; }


		readonly ulong[] _pureBuffer;

		public SimpleSetDecoder(
			HashingFunctions hashingFunctions,
			TTable table, int size,
			Expression<Action<ulong, ulong, TTable>> toggle
			)
		{
			_table = table;
			_size = size;
			_pureBuffer = new ulong[size];
			_pure = new HashSet<ulong>();
			_answer = new HashSet<ulong>(_size);
			Initialized = false;

			var looksPure = SimpleSetSketchFunc.LooksPure<TTable>(hashingFunctions);
			var addIfLooksPure = SimpleSetSketchFunc.AddIfLooksPure<HashSet<ulong>, TTable>(looksPure);
			var oneDecodeStep = SimpleSetSketchFunc.OneDecodingStep(hashingFunctions, toggle, looksPure, addIfLooksPure);
			OneDecodeStep = oneDecodeStep.Compile();
			InitDecoding = SimpleSetSketchFunc.Initialize(addIfLooksPure).Compile();
		}

		internal virtual bool ContinueCondition()
		{
			const uint maxIterations = 10000;
			if (CurrentIteration < maxIterations)
			{
				return true;
			}
			return false;
		}

		public SimpleSetDecoder<TTable> Init()
		{
			_answer.Clear();
			_pure = new HashSet<ulong>(_size);
			_pureNext = new HashSet<ulong>(_size);
			CurrentIteration = 0;
			InitDecoding(_table, _size, _pure);
			Initialized = true;
			return this;
		}

		public HashSet<ulong> GetCurrentDecodedValues()
		{
			return _answer;
		}
		public void Decode()
		{
			if (!Initialized)
			{
				Init();
			}

			while (ContinueCondition())
			{
				for (int i = 0; i < BatchSize; i++)
				{
					int numberOfItems = 0;
					foreach (var item in _pure)
					{
						_pureBuffer[numberOfItems] = item;
						numberOfItems++;
					}
					OneDecodeStep(_pureBuffer, numberOfItems, _pureNext, _answer, _table);

					_pure = _pureNext;
					_pureNext = new HashSet<ulong>(_pure.Count * 2);
					if (_pure.Count == 0)
					{
						if (_table.All(x => x == 0))
						{
							State = DecodingState.Success;
							return;
						}
						else
						{
							State = DecodingState.Failed;
							return;
						}
					}
					CurrentIteration++;
				}
			}
			State = DecodingState.Shotdown;
		}
	}
}
