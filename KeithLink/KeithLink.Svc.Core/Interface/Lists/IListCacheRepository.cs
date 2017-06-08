namespace KeithLink.Svc.Core.Interface.Lists
{
	public interface IListCacheRepository
	{
		void AddItem<T>(string key, T item);
		void ResetAllItems();
		void RemoveItem(string key);
		T GetItem<T>(string key);
	}
}
