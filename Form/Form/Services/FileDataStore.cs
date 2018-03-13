using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Form.Services;
using HtmlAgilityPack;
using LibBusQuery;
using Xamarin.Forms.Xaml;


//[assembly: Xamarin.Forms.Dependency(typeof(Form.Services.FileDataStore<StoredBase>))]

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

        public void FromEntry(IInfoEntry entry) {
            this.Id = entry.Id;
            this.Name = entry.Name;
            this.Description = entry.Description;
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

    public class FileDataStore <T> : IDataStore<T> where T : IStoredEntry, new() {
        private static string GetDataPath() {
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return folderPath;
        }

        private string fileName = Path.Combine(GetDataPath(), nameof(T));

        public string FileName => fileName;

        private Dictionary<string, T> dict = new Dictionary<string, T>();

        public FileDataStore() { }

        public FileDataStore(string fileName) {
            this.Bind(fileName);
            Load();
        }


        public bool Bind(string fileName) {
            this.fileName = Path.Combine(GetDataPath(), fileName);
            if (File.Exists(this.fileName)) return true;
            else {
                File.Create(this.fileName);
                return false;
            }
        }

        public bool Load() {
            try {
                using (var sr = new StreamReader(this.fileName)) {
                    while (!sr.EndOfStream) {
                        var line = sr.ReadLine();
                        var entry = new T();
                        entry.FromString(line);
                        this.dict.Add(entry.Id, entry);
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public bool Save() {
            try {
                using (var sw = new StreamWriter(this.fileName)) {
                    foreach (var entry in this.dict.Values) {
                        sw.WriteLine(entry.ToString());
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public bool SaveAs(string fileName) {
            try {
                using (var sw = new StreamWriter(fileName)) {
                    foreach (var entry in this.dict.Values) {
                        sw.WriteLine(entry.ToString());
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }

        public bool AddItem(IInfoEntry item) {
            try {
                var entry = new T();
                entry.FromEntry(item);
                this.dict.Add(entry.Id, entry);
                return true;
            }
            catch {
                return false;
            }
        }

        // throws ArgumentExpection
        public T GetItem(string id) { return this.dict[id]; }

        public bool UpdateItem(IInfoEntry item) {
            try {
                var entry = new T();
                entry.FromEntry(item);
                this.dict[entry.Id] = entry;
                return true;
            }
            catch {
                return false;
            }
        }

        public bool DeleteItem(IInfoEntry item) {
            try {
                this.dict.Remove(item.Id);
                return true;
            }
            catch {
                return false;
            }
        }

        public IEnumerable<T> GetItems(bool forceRefresh = false) {
            if (forceRefresh) {
                Load();
            }
            return this.dict.Values;
        }
    }

    public static class Fav {
        public static FileDataStore<StoredLine> FavLines { get; }
        public static FileDataStore<StoredStation> FavStations { get;}

        static Fav() {
            FavLines = new FileDataStore<StoredLine>("favlines.txt");
            FavStations = new FileDataStore<StoredStation>("favstations.txt");
        }
    }
}