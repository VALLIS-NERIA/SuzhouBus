using System;
using System.Collections.Generic;
using System.Text;
using LibBusQuery;
using Xamarin.Forms;

namespace Form {
    static class Utils {
        public static void SendFetchMessage(ContentPage page, Func<IEnumerable<IInfoEntry>> lambda) { MessagingCenter.Send(page, "FetchReplaceItems", lambda); }
    }
}