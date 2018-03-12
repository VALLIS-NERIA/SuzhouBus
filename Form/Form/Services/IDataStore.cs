using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibBusQuery;

namespace Form.Services
{
    public interface IDataStore<out T> where T:IInfoEntry {
        string FileName { get; }
        bool Bind(string fileName);
        bool Load();
        bool Save();
        // SaveAs should not change the bind
        bool SaveAs(string fileName);
        bool AddItem(IInfoEntry item);
        T GetItem(string id);
        bool UpdateItem(IInfoEntry item);
        bool DeleteItem(IInfoEntry item);
        IEnumerable<T> GetItems(bool forceRefresh = false);
    }

    public interface IStoredEntry : IInfoEntry {
        void FromString(string s);
        void FromEntry(IInfoEntry entry);
        new string ToString();
    }
}
