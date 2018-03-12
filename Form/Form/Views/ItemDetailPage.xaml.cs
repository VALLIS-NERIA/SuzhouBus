using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using static Form.Utils;
using Form.Models;
using Form.ViewModels;
using LibBusQuery;

namespace Form.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemDetailPage : ContentPage
	{
	    private ItemDetailViewModel viewModel { get; set; }

	    public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new SimpleEntry()
            {
                Name = "Item 1",
                Description = "This is an item description."
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }

	    private async void SeeDetail(object sender, EventArgs e) {
            //var list = this.viewModel.Click();
	        await Navigation.PushAsync(new ItemsPage(viewModel.Item));
        }
    }
}