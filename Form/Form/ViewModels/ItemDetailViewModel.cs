using System;
using System.Collections.Generic;
using LibBusQuery;
using Xamarin.Forms;

//using Form.Models;

namespace Form.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public IInfoEntry Item { get; set; }
        public ItemDetailViewModel(IInfoEntry item = null)
        {
            Title = item?.Name;
            Item = item;
        }

        public IEnumerable<IInfoEntry> Click() {
            var list = this.Item.LinkClick();
            return list;
        }
    }
}
