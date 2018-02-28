using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibBusQuery;
using Form.Models;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: Xamarin.Forms.Dependency(typeof(Form.Services.MockDataStore))]
namespace Form.Services
{
    public class MockDataStore : IDataStore<IInfoEntry>
    {
        List<IInfoEntry> items;

        public MockDataStore()
        {
            items = new List<IInfoEntry>();
            var mockItems = new List<IInfoEntry>
            {

            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(IInfoEntry item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(IInfoEntry item)
        {
            var _item = items.FirstOrDefault(arg => arg.Id == item.Id);
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(IInfoEntry item)
        {
            var _item = items.FirstOrDefault(arg => arg.Id == item.Id);
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<IInfoEntry> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<IInfoEntry>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}