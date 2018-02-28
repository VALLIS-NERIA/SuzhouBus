using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

//using PCLWebUtility;
namespace LibBusQuery {
    public class BusQuery {
        private const string BaseUrl = "http://www.szjt.gov.cn/BusQuery/{0}?cid={1}";
        private const string LineAspx = "APTSLine.aspx";
        private const string StationAspx = "default.aspx";
        private const string Cid = "175ecd8d-c39d-4116-83ff-109b946d7cb4";

        private const string LineSearchPara =
            @"&__VIEWSTATE=%2FwEPDwUJNDk3MjU2MjgyD2QWAmYPZBYCAgMPZBYCAgEPZBYCAgYPDxYCHgdWaXNpYmxlaGRkZJoicdBkB9yhQuA%2FOdf79KTvSO4XgMh67pTvZ2uF3nG0" +
            @"&__VIEWSTATEGENERATOR=385F41AE&__EVENTVALIDATION=%2FwEWAwLmzYecBgL88Oh8AqX89aoKoyB8TWJGCcftPn6FMApjVOu8taYx65eqI%2BF5zhRrstE%3D&ctl00%24MainContent" +
            @"%24LineName={0}&ctl00%24MainContent%24SearchLine=%E6%90%9C%E7%B4%A2";

        private const string StationSearchPara =
            @"&__VIEWSTATE=%2FwEPDwULLTE5ODM5MjcxNzlkZAmiJJ2tnI%2FuORAHha02uiTP20zkcIPFI%2F03%2BQRVt2jm&__VIEWSTATEGENERATOR=1DDAEB65" +
            @"&__EVENTVALIDATION=%2FwEWBQLCvY65BALq%2BuyKCAKkmJj%2FDwL0%2BsTIDgLl5vKEDsL%2FhpJTaG5nWiRLbcu6Hd%2BJXovreFUihHtwVJ7zNsPc&ctl00%24MainContent%24" +
            @"StandName={0}&ctl00%24MainContent%24SearchCode=%E6%90%9C%E7%B4%A2&ctl00%24MainContent%24StandCode=";


        private static string LineUrl = string.Format(BaseUrl, LineAspx, Cid);
        private static string StationUrl = string.Format(BaseUrl, StationAspx, Cid);

        public static IEnumerable<StationInLine> QueryBusInfo(string BusGuid) {
            try {
                var s = DownloadString(LineUrl + "&LineGuid=" + BusGuid);
                HtmlNodeCollection tr = GetTable(s);
                var list = new List<StationInLine>();
                foreach (HtmlNode node in tr.Skip(1)) {
                    try {
                        var entry = new StationInLine(node);
                        list.Add(entry);
                    }
                    catch (ArgumentException e) {
                        continue;
                    }
                }
                return list;
            }
            catch (WebException e) {
                return new List<StationInLine>();
            }
        }


        public static IEnumerable<LineOnStation> QueryStationInfo(string StationCode) {
            try {
                var s = DownloadString(StationUrl + "&StandCode=" + StationCode);
                var tr = GetTable(s);
                var list = new List<LineOnStation>();
                foreach (HtmlNode node in tr.Skip(1)) {
                    try {
                        var entry = new LineOnStation(node);
                        list.Add(entry);
                    }
                    catch (ArgumentException e) {
                        continue;
                    }
                }
                return list;
            }
            catch (WebException e) {
                return new List<LineOnStation>();
            }
        }

        public static IEnumerable<LineCandidate> SearchLine(string keyw) {
            try { 
            var s = DownloadString(LineUrl + string.Format(LineSearchPara, keyw));
            var tr = GetTable(s);
            var list = new List<LineCandidate>();
            foreach (HtmlNode node in tr.Skip(1)) {
                try {
                    var entry = new LineCandidate(node);
                    list.Add(entry);
                }
                catch (ArgumentException e) {
                    continue;
                }
            }
            return list;
            }
            catch (WebException e) {
                return new List<LineCandidate>();
            }
        }

        public static IEnumerable<StationCandidate> SearchStation(string keyw) {
            try {
                var s = DownloadString(StationUrl + string.Format(StationSearchPara, keyw));
                var tr = GetTable(s);
                var list = new List<StationCandidate>();
                foreach (HtmlNode node in tr.Skip(1)) {
                    try {
                        var entry = new StationCandidate(node);
                        list.Add(entry);
                    }
                    catch (ArgumentException e) {
                        continue;
                    }
                }
                return list;
            }
            catch (WebException e) {
                return new List<StationCandidate>();
            }
        }

        private static string DownloadString(string url) {
            var webC = new WebClient();
            var data = webC.DownloadData(url);

            // convert coding
            var b1 = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, data);
            var s = Encoding.Unicode.GetString(b1);
            return s;
        }

        private static HtmlNodeCollection GetTable(string s) {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);

            // locate to the table
            var root = doc.DocumentNode;
            var table = root.SelectSingleNode(@"//span[@id='MainContent_DATA']/table");
            var tr = table.SelectNodes(@"tr");
            return tr;
        }
    }
}