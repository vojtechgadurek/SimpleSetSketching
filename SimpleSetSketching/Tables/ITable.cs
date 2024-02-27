namespace SimpleSetSketching.Tables
{
	public interface ITable<TValue> where TValue : struct
	{
		TValue Get(uint index);
		bool IsEmpty();
		uint Length();
		void Set(uint index, TValue value);
	}
}