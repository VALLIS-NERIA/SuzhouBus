using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Form.Services;
using Xamarin.Forms;

//using Form.Models;
using LibBusQuery;
using Form.Views;

namespace Form.ViewModels {
    public class SavedItemsViewModel : ItemsViewModel {
        public Command LoadItemsCommand { get; set; }
        public FileDataStore<StoredLine> FavLines { get; set; }
        public FileDataStore<StoredStation> FavStations { get; set; }

        public SavedItemsViewModel() {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            this.FavLines = new FileDataStore<StoredLine>("favlines.txt");
            this.FavStations = new FileDataStore<StoredStation>("favstations.txt");
        }

        async Task ExecuteLoadItemsCommand() {
            if (IsBusy)
                return;

            IsBusy = true;

            try {
                Items.Clear();
                var lines = (IEnumerable<IInfoEntry>)this.FavLines.GetItems(true);
                var stations = this.FavStations.GetItems(true);
                var items = lines.Concat(stations);
                foreach (var item in items) {
                    Items.Add(item);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            finally {
                IsBusy = false;
            }
        }
    }

    public class ItemsViewModel : BaseViewModel {
        public ObservableCollection<IInfoEntry> Items { get; set; }
        public IInfoEntry Head { get; set; }

        private Func<IEnumerable<IInfoEntry>> refreshLambda;

        public ItemsViewModel() {
            Title = "Browse";
            Items = new ObservableCollection<IInfoEntry>();

            //MessagingCenter.Subscribe<SearchPage, IInfoEntry>(this, "AddItem", Add);
            //MessagingCenter.Subscribe<ContentPage, IEnumerable<IInfoEntry>>(this, "ReplaceItems", ReplaceItems);
            //MessagingCenter.Subscribe<ContentPage, Func<IEnumerable<IInfoEntry>>>(this, "FetchReplaceItems", FetchItemsAndReplace);
        }

        public ItemsViewModel(IInfoEntry head) : this() {
            Title = head.Name;
            this.Head = head;
            this.refreshLambda = head.LinkClick;
            this.FetchItemsAndReplace(null, head.LinkClick);
        }

        public ItemsViewModel(Func<IEnumerable<IInfoEntry>> lambda, string title = "View") : this() {
            Title = title;
            refreshLambda = lambda;
            FetchItemsAndReplace(null, lambda);
        }

        //~ItemsViewModel() {
        //    MessagingCenter.Unsubscribe<SearchPage, IInfoEntry>(this, "AddItem");
        //    MessagingCenter.Unsubscribe<ContentPage, IEnumerable<IInfoEntry>>(this, "ReplaceItems");
        //    MessagingCenter.Unsubscribe<ContentPage, Func<IEnumerable<IInfoEntry>>>(this, "FetchReplaceItems");

        //}


        public void Refresh() {
            FetchItemsAndReplace(null, this.refreshLambda);
        }

        void Add(SearchPage sender, IInfoEntry item) {
            var _item = item as IInfoEntry;
            Items.Add(_item);
        }

        public void ReplaceItems(ContentPage sender, IEnumerable<IInfoEntry> items) {
            this.Items.Clear();
            foreach (IInfoEntry item in items) {
                this.Items.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="lambda">A delegate to fetch items. Will be ran async</param>
        public async void FetchItemsAndReplace(ContentPage sender, Func<IEnumerable<IInfoEntry>> lambda) {
            this.IsBusy = true;
            var task = Task.Run(
                () => {
                    var list = lambda();
                    ReplaceItems(sender, list);
                });
            await task;
            this.IsBusy = false;
            //ReplaceItems(sender, req.Method(req.keyword));
        }
    }
}