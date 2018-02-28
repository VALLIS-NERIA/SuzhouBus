using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LibBusQuery;
using Form.Models;
using static Form.Utils;

namespace Form.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage {
        public string Keyword { get; set; }
        //public IInfoEntry Item { get; set; }

        public SearchPage() {
            InitializeComponent();

            //Item = new SimpleEntry()
            //{
            //    Name = "Item name",
            //    Description = "This is an item description."
            //};
            this.EntryKeyword.Focus();
            BindingContext = this;
        }


        async void Save_Clicked(object sender, EventArgs e) {
            //MessagingCenter.Send(this, "AddItem", Item);

            await Navigation.PopModalAsync();
        }

        private async void SearchLine(object sender, EventArgs e) {
            var itemPage = new ItemsPage(() => BusQuery.SearchLine(this.Keyword), "Search Result: " + this.Keyword);
            PushToMainPage(itemPage);
            //await Navigation.PushAsync(new ItemsPage(() => BusQuery.SearchLine(this.Keyword), this.Keyword));
            //MessagingCenter.Send<ContentPage,IEnumerable<IInfoEntry>>(this,"ReplaceItems", BusQuery.SearchLine(this.KeyWord));
            //MessagingCenter.Send<ContentPage, QueryRequest>(this, "FetchReplaceItems", new QueryRequest {keyword = this.Keyword, Method = BusQuery.SearchLine});
            //SendFetchMessage(this, () => BusQuery.SearchLine(this.Keyword));
            //await Navigation.PopModalAsync();
        }

        private async void SearchStation(object sender, EventArgs e) {
            var itemPage = new ItemsPage(() => BusQuery.SearchStation(this.Keyword), "Search Result: " + this.Keyword);
            PushToMainPage(itemPage);
            //await Navigation.PushAsync(new ItemsPage(() => BusQuery.SearchStation(this.Keyword), this.Keyword));
            //MessagingCenter.Send<ContentPage, IEnumerable<IInfoEntry>>(this, "ReplaceItems", BusQuery.SearchStation(this.Keyword));
            //MessagingCenter.Send<ContentPage, QueryRequest>(this, "FetchReplaceItems", new QueryRequest {keyword = this.Keyword, Method = BusQuery.SearchStation});
            //SendFetchMessage(this, () => BusQuery.SearchStation(this.Keyword));
            //await Navigation.PopModalAsync();
        }

        private async void PushToMainPage(ItemsPage itemPage) {
            var mainPage = this.Parent.Parent as TabbedPage;
            var naviPage = new NavigationPage(new Page()) {Title = "Result"};
            mainPage.Children.Add(naviPage);
            mainPage.CurrentPage = naviPage;
            naviPage.Navigation.InsertPageBefore(itemPage, naviPage.RootPage);
            naviPage.PopToRootAsync();
        }

        private void OnFocused(object sender, FocusEventArgs e) {
            Device.BeginInvokeOnMainThread(
                async () => {
                    await Task.Delay(200);
                    this.EntryKeyword.Focus();
                });
        }
    }
}