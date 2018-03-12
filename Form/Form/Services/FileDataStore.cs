using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Form.Services;
using HtmlAgilityPack;
using LibBusQuery;
using Xamarin.Forms.Xaml;


[assembly: Xamarin.Forms.Dependency(typeof(Form.Services.FileDataStore<IStoredEntry>))]

namespace Form.Services {
    public abstract class StoredBase : IStoredEntry {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
        public abstract IEnumerable<IInfoEntry> LinkClick();

        public void FromString(string s) {
            try {
                var sArray = s.Split(new[] {'$'}, 3);
                this.Id = sArray[0];
                this.Name = sArray[1];
                this.Description = sArray.Length == 3 ? sArray[2] : "";
            }
            catch (IndexOutOfRangeException ex) {
                throw new ArgumentException("Illegal string");
            }
        }

        protected StoredBase() { }

        protected StoredBase(IInfoEntry entry) {
            this.Id = entry.Id;
            this.Name = entry.Name;
            this.Description = entry.Description;
        }

        public new string ToString() { return $"{this.Id}${this.Name}${this.Description}"; }
    }

    public class StoredLine : StoredBase {
        public StoredLine() { }
        public StoredLine(LineBase runtimeLine) : base(runtimeLine) { }
        public override IEnumerable<IInfoEntry> LinkClick() { return BusQuery.QueryBusInfo(this.Id); }
    }

    public class StoredStation : StoredBase {
        public StoredStation() { }
        public StoredStation(StationBase runtimeStation) : base(runtimeStation) { }
        public override IEnumerable<IInfoEntry> LinkClick() { return BusQuery.QueryBusInfo(this.Id); }
    }

    public class FileDataStore <T> : IDataStore<T> where T : IStoredEntry {
        private static string GetDataPath() {
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return folderPath;
        }

        private static string fileName = Path.Combine(GetDataPath(), nameof(T));
        public Task<bool> LoadAsync(string fileName) { throw new NotImplementedException(); }
        public Task<bool> SaveAsync(string fileName) { throw new NotImplementedException(); }
        public Task<bool> SaveAsAsync(string fileName) { throw new NotImplementedException(); }
        public Task<bool> AddItemAsync(T item) { throw new NotImplementedException(); }
        public Task<bool> UpdateItemAsync(T item) { throw new NotImplementedException(); }
        public Task<bool> DeleteItemAsync(T item) { throw new NotImplementedException(); }
        public Task<T> GetItemAsync(string id) { throw new NotImplementedException(); }
        public Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false) { throw new NotImplementedException(); }
    }
}