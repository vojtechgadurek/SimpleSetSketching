namespace SimpleSetSketching
{
	internal interface ITable<TValue> where TValue : struct
	{
		TValue Get(int index);
		bool IsEmpty();
		int Length();
		void Set(int index, TValue value);
	}
}