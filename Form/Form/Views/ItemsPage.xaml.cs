using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using static Form.Utils;
using Form.Models;
using Form.Views;
using Form.ViewModels;
using LibBusQuery;

namespace Form.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage {
        public ItemsViewModel viewModel { get; set; }

        //public bool IsBusy { get; set; }
        public ItemsPage() {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
        }

        public ItemsPage(ItemsViewModel model) : this() { BindingContext = viewModel = model; }

        public ItemsPage(IInfoEntry head) : this(new ItemsViewModel(head)) { }

        public ItemsPage(Func<IEnumerable<IInfoEntry>> lambda, string title = "View") : this(new ItemsViewModel(lambda, title)) { }



        protected virtual async void Close_Clicked(object sender, EventArgs e) {
            var staack = Navigation.NavigationStack;
            var mainPage = this.Parent.Parent as TabbedPage;
            var navigationPage = this.Parent as NavigationPage;
            mainPage.Children.Remove(navigationPage);
            ;
            //await Navigation.PushModalAsync(new NavigationPage(new SearchPage()));
        }

        protected override void OnAppearing() { base.OnAppearing(); }

        private void Button_OnClicked(object sender, EventArgs e) {
            var button = sender as Button;
            var item = button.BindingContext as IInfoEntry;
            if (item == null) return;
            Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            //Navigation.PushAsync(new ItemsPage(item));
            //SendFetchMessage(this, () => item.LinkClick());
            //MessagingCenter.Send<ContentPage,Func<IEnumerable<IInfoEntry>>>(this, "FetchReplaceItems", ()=>item.LinkClick());

            //   this.IsBusy = true;
            //var list = item.LinkClick();
            //this.viewModel.ReplaceItems(this, list);
            //this.IsBusy = false;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args) {
            var s =System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var item = args.SelectedItem as IInfoEntry;
            if (item == null)
                return;
            await Navigation.PushAsync(new ItemsPage(item));

            // await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        private void ItemsListView_OnRefreshing(object sender, EventArgs e) {
            this.viewModel.Refresh();
            ((ListView) sender).IsRefreshing = false;
        }
    }
}