using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibBusQuery;
using Form.Models;
using Xamarin.Forms.Xaml;
using System.IO;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: Xamarin.Forms.Dependency(typeof(Form.Services.MockDataStore<IInfoEntry>))]

namespace Form.Services {
    public class MockDataStore <T> : IDataStore<T> where T : IInfoEntry {
        List<T> items;

        public MockDataStore() {
            items = new List<T>();
            var mockItems = new List<T>
                { };

            foreach (var item in mockItems) {
                items.Add(item);
            }
        }

        public Task<bool> LoadAsync(string fileName) { throw new NotImplementedException(); }

        public Task<bool> SaveAsync(string fileName) { throw new NotImplementedException(); }

        public Task<bool> SaveAsAsync(string fileName) { throw new NotImplementedException(); }

        public async Task<bool> AddItemAsync(T item) {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(T item) {
            var _item = items.FirstOrDefault(arg => arg.Id == item.Id);
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(T item) {
            var _item = items.FirstOrDefault(arg => arg.Id == item.Id);
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<T> GetItemAsync(string id) { return await Task.FromResult(items.FirstOrDefault(s => s.Id == id)); }

        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false) { return await Task.FromResult(items); }
    }
}