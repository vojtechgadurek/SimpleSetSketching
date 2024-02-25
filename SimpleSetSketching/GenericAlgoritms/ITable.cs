namespace SimpleSetSketching
{

	public interface ITable<TValue> where TValue : struct
	{
		TValue Get(uint index);
		bool IsEmpty();
		uint Length();

		void Xor(uint index, TValue value);
		void Set(uint index, TValue value);
	}
}