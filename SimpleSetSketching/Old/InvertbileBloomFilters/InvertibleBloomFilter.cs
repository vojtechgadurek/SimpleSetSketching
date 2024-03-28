//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Metadata.Ecma335;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Linq.Expressions;
//using System.ComponentModel;

//namespace SimpleSetSketching.SimpleSetSketchers
//{
//	class InvertibleBloomFilter : ISketcher
//	{
//		public ulong G1HashFunction(ulong i)
//		{
//			i = (i ^ (i >> 33)) * 0x4A9D1C4D699E4B36UL >> 32;
//			i = (i ^ (i >> 29)) * 0x32BBD84192AABFEDUL >> 31;
//			return i & (SizeOfTable - 1);
//		}
//		record struct IBMFCell(int Count, ulong KeySum, ulong ValueSum, ulong HashKeySum);
//		IBMFCell[] _data;
//		const int _hashCount = 5;
//		ulong[] _hashes = new ulong[_hashCount];
//		ulong SizeOfTable = 1;

//		public InvertibleBloomFilter(ulong size)
//		{

//			while (SizeOfTable < size)
//			{
//				SizeOfTable <<= 1;
//			}

//			_data = new IBMFCell[SizeOfTable];
//			for (ulong i = 0; i < SizeOfTable; i++)
//			{
//				_data[i] = new IBMFCell(0, 0, 0, 0);
//			}
//		}
//		public void Hashing(in ulong key, ulong[] hashes)
//		{
//			HashFunctions.HashToTable(key, hashes, SizeOfTable);
//		}
//		public void Insert(ulong key, ulong value)
//		{
//			Hashing(key, _hashes);
//			for (int i = 0; i < _hashCount; i++)
//			{
//				ulong hashkey = _hashes[i];
//				IBMFCell originalCell = _data[hashkey];
//				_data[hashkey] = new IBMFCell(
//					originalCell.Count + 1,
//					originalCell.KeySum + key,
//					originalCell.ValueSum + value,
//					originalCell.HashKeySum + G1HashFunction(key)
//					);

//			}
//		}

//		public void Delete(ulong key, ulong value)
//		{
//			Hashing(key, _hashes);
//			for (int i = 0; i < _hashCount; i++)
//			{
//				ulong hashkey = _hashes[i];
//				IBMFCell originalCell = _data[hashkey];
//				_data[hashkey] = new IBMFCell(
//					originalCell.Count - 1,
//					originalCell.KeySum - key,
//					originalCell.ValueSum - value,
//					originalCell.HashKeySum - G1HashFunction(key)
//					);
//			}
//		}

//		public ulong? TryGet(ulong key)
//		{
//			Hashing(key, _hashes);
//			ulong G1Hash = G1HashFunction(key);
//			for (int i = 0; i < _hashCount; i++)
//			{
//				ulong hash = _hashes[i];
//				IBMFCell cell = _data[hash];
//				if (cell.Count == 0 && cell.KeySum == 0 && cell.HashKeySum == 0)
//				{
//					return null;
//				}
//				else if (cell.Count == 1 && cell.KeySum == key && cell.HashKeySum == G1Hash)
//				{
//					return cell.ValueSum;
//				}
//				else if (cell.Count == -1 && cell.KeySum == 0 - key && cell.HashKeySum == 0 - G1Hash)
//				{
//					return null;
//				}
//			}
//			return null;
//		}


//		public (ulong, ulong)[] ListEntries()
//		{
//			List<(ulong, ulong)> ansver = new List<(ulong, ulong)>();
//			List<ulong> ones = new List<ulong>();
//			for (ulong i = 0; i < (ulong)_data.Length; i++)
//			{
//				IBMFCell cell = _data[i];
//				if (cell.Count == 1)
//				{
//					ones.Add(i);
//				}
//				if (cell.Count == -1)
//				{
//					ones.Add(i);
//				}
//			}

//			while (ones.Count > 0)
//			{
//				var cell = _data[ones.Last()];
//				ones.RemoveAt(ones.Count - 1);

//				if (cell.Count == 1)
//				{
//					ulong G1Hash = G1HashFunction(cell.KeySum);
//					if (cell.HashKeySum != G1Hash) continue;
//					ansver.Add(new(cell.KeySum, cell.ValueSum));
//					Delete(cell.KeySum, cell.ValueSum);
//				}
//				else if (cell.Count == -1)
//				{
//					ulong G1Hash = G1HashFunction(0 - cell.KeySum);
//					if (cell.HashKeySum != 0 - G1Hash) continue;
//					Insert(0 - cell.KeySum, 0 - cell.ValueSum);
//					ansver.Add(new(0 - cell.KeySum, 0 - cell.ValueSum));
//				}
//				else
//				{
//					continue;
//				}
//				//If removed, check if any other cell should be added to queue
//				Hashing(cell.KeySum, _hashes);
//				for (int i = 0; i < _hashCount; i++)
//				{
//					ulong hash = _hashes[i];
//					var possibleCellWithOnes = _data[hash];
//					if (possibleCellWithOnes.Count == 1 | possibleCellWithOnes.Count == -1)
//					{
//						ones.Add(hash);
//					}
//				}

//			}
//			return ansver.ToArray();
//		}

//		public void Insert(SketchStream insert)
//		{
//			ulong toInsert;
//			while (true)
//			{
//				toInsert = insert.Next();
//				if (toInsert == 0)
//				{
//					break;
//				}
//				Insert(toInsert, toInsert);
//			}
//		}

//		public void Remove(SketchStream remove)
//		{
//			ulong toRemove;
//			while (true)
//			{
//				toRemove = remove.Next();
//				if (toRemove == 0)
//				{
//					break;
//				}
//				Delete(toRemove, toRemove);
//			}
//		}

//		public HashSet<ulong>? Decode()
//		{
//			return new HashSet<ulong>(ListEntries().Select(x => x.Item2));
//		}
//	}
//}
