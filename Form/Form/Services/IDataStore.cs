using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibBusQuery;

namespace Form.Services
{
    public interface IDataStore<T> {
        Task<bool> LoadAsync(string fileName);

        Task<bool> SaveAsync(string fileName);
        Task<bool> SaveAsAsync(string fileName);
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(T item);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }

    public interface IStoredEntry : IInfoEntry {
        void FromString(string s);
        new string ToString();
    }
}
