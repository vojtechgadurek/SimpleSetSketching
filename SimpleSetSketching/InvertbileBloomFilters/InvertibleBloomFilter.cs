using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSetSketching.SimpleSetSketchers
{
	class InvertibleBloomFilter : ISketcher
	{
		record struct RBFCell(int count, ulong keySum, ulong valueSum);
		RBFCell[] _data;
		const int _hashCount = 5;
		ulong[] _hashes = new ulong[_hashCount];
		ulong _size = 1;

		public InvertibleBloomFilter(ulong size)
		{
			_size = size;
			/*
			while (_size < size)
			{
				_size <<= 1;
			}
			*/
			_data = new RBFCell[_size];
		}
		public void Hashing(in ulong key, ulong[] hashes)
		{
			HashFunctions.HashToTable(key, hashes, _size);
		}
		public void Insert(ulong key, ulong value)
		{
			Hashing(key, _hashes);
			for (int i = 0; i < _hashCount; i++)
			{
				ulong hash = _hashes[i];
				RBFCell originalCell = _data[hash];
				_data[hash] = new RBFCell(
					originalCell.count + 1,
					originalCell.keySum + key,
					originalCell.valueSum + value
					);
			}
		}

		public void Delete(ulong key, ulong value)
		{
			Hashing(key, _hashes);
			for (int i = 0; i < _hashCount; i++)
			{
				ulong hash = _hashes[i];
				RBFCell originalCell = _data[hash];
				_data[hash] = new RBFCell(
					originalCell.count - 1,
					originalCell.keySum - key,
					originalCell.valueSum - value
					);
			}
		}

		public ulong? TryGet(ulong key)
		{
			Hashing(key, _hashes);
			for (int i = 0; i < _hashCount; i++)
			{
				ulong hash = _hashes[i];
				RBFCell cell = _data[hash];
				if (cell.count == 0)
				{
					return null;
				}
				else if (cell.count == 1)
				{
					if (cell.keySum == key)
					{
						return cell.valueSum;
					}
					else
					{
						return null;
					}
				}
			}
			throw new InvalidOperationException($"Key :{key} not found");
		}
		public (ulong, ulong)[] ListEntries()
		{
			List<(ulong, ulong)> ansver = new List<(ulong, ulong)>();
			List<ulong> ones = new List<ulong>();
			for (ulong i = 0; i < (ulong)_data.Length; i++)
				if (_data[i].count == 1)
				{
					ones.Add(i);
				}

			while (ones.Count > 0)
			{
				var cell = _data[ones.Last()];
				if (cell.count == 1)
				{
					ansver.Add(new(cell.keySum, cell.valueSum));
					ones.RemoveAt(ones.Count - 1);
					Delete(cell.keySum, cell.valueSum);
					//If any deletion lead to count 0, we add them to ones
					//This is more expensive than should be as hashing function is called twice
					Hashing(cell.keySum, _hashes);
					for (int i = 0; i < _hashCount; i++)
					{
						ulong hash = _hashes[i];
						if (_data[hash].count == 1)
						{
							ones.Add(hash);
						}
					}
				}
				else
				{
					ones.RemoveAt(ones.Count - 1);
				}

			}
			return ansver.ToArray();
		}

		public void Insert(SketchStream insert)
		{
			ulong toInsert;
			while (true)
			{
				toInsert = insert.Next();
				if (toInsert == 0)
				{
					break;
				}
				Insert(toInsert, toInsert);
			}
		}

		public void Remove(SketchStream remove)
		{
			ulong toRemove;
			while (true)
			{
				toRemove = remove.Next();
				if (toRemove == 0)
				{
					break;
				}
				Delete(toRemove, toRemove);
			}
		}

		public HashSet<ulong>? Decode()
		{
			return new HashSet<ulong>(ListEntries().Select(x => x.Item2));
		}
	}
}
