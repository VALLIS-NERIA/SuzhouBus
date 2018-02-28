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
using HtmlAgilityPack;

namespace LibBusQuery {
    public class BusStation {
        public string StationCode;
        public string StationName;

        public BusStation(string href) {
            var match = Regex.Matches(href, @"(?<=.+?=).+?(?=&|$)");
            this.StationCode = match[1].Value;
            this.StationName = match[2].Value;
        }
    }

    public class BusLine {
        public string LineGuid;
        public string LineName;

        public BusLine(string href) {
            var match = Regex.Matches(href, @"(?<=.+?=).+?(?=&|$)");
            this.LineGuid = match[1].Value;
            this.LineName = match[2].Value;
        }
    }

    /// <summary>
    /// Entry when querying a bus line
    /// 站台 编号 车牌 进站时间
    /// </summary>
    [DebuggerDisplay("{Station.StationName},{StationCode},{BusNum},{EnterTime}")]
    public class LineInfoEntry {
        public BusStation Station;
        public string StationCode;
        public string BusNum;
        public DateTime? EnterTime;

        public LineInfoEntry(HtmlNode node) {
            var tds = node.SelectNodes("td");
            var a = tds[0].SelectSingleNode("a");
            if (a != null) {
                string href = a.GetAttributeValue("href", null);
                if (href != null) {
                    this.Station = new BusStation(href);
                }
                else {
                    throw new ArgumentException();
                }
                this.StationCode = tds[1].InnerHtml;
                this.BusNum = tds[2].InnerHtml;
                var time = tds[3].InnerHtml;
                if (time != "") {
                    this.EnterTime = DateTime.Parse(tds[3].InnerHtml);
                }
                else {
                    this.EnterTime = null;
                }
            }
            else {
                throw new ArgumentException();
            }
        }
    }

    /// <summary>
    /// Entry when querying a bus station, like the plate
    /// 线路 方向 车牌 更新时间 站距
    /// </summary>
    [DebuggerDisplay("{Line.LineName},{Direction},{BusNum},{EnterTime},{Distance}")]
    public class StationInfoEntry {
        public BusLine Line;
        public string Direction;
        public string BusNum;
        public DateTime? EnterTime;
        public int? Distance;

        public StationInfoEntry(HtmlNode node) {
            var tds = node.SelectNodes("td");
            var a = tds[0].SelectSingleNode("a");
            if (a != null) {
                string href = a.GetAttributeValue("href", null);
                if (href != null) {
                    this.Line = new BusLine(href);
                }
                else {
                    throw new ArgumentException();
                }
                this.Direction = tds[1].InnerHtml;
                this.BusNum = tds[2].InnerHtml;
                var time = tds[3].InnerHtml;
                if (time != "") {
                    this.EnterTime = DateTime.Parse(tds[3].InnerHtml);
                }
                else {
                    this.EnterTime = null;
                }
                var dist = tds[4].InnerHtml;
                if (int.TryParse(dist, out int d)) {
                    this.Distance = d;
                }
                else {
                    this.Distance = null;
                }
            }
            else {
                throw new ArgumentException();
            }
        }
    }

    /// <summary>
    /// Entry when searching a bus line
    /// </summary>
    [DebuggerDisplay("{Line.LineName},{Direction}")]
    public class LineCandidate {
        public BusLine Line;
        public string Direction;

        public LineCandidate(HtmlNode node) {
            var tds = node.SelectNodes("td");
            var a = tds[0].SelectSingleNode("a");
            if (a != null) {
                string href = a.GetAttributeValue("href", null);
                if (href != null) {
                    this.Line = new BusLine(href);
                }
                else {
                    throw new ArgumentException();
                }
                this.Direction = tds[1].InnerHtml;
            }
            else {
                throw new ArgumentException();
            }
        }
    }

    /// <summary>
    /// Entry when searching a station
    /// </summary>
    public class StationCandidate {
        public BusStation Station;
        public string StationCode;
        public string District, Road, Section, Compass;

        public StationCandidate(HtmlNode node) {
            var tds = node.SelectNodes("td");
            var a = tds[0].SelectSingleNode("a");
            if (a != null) {
                string href = a.GetAttributeValue("href", null);
                if (href != null) {
                    this.Station = new BusStation(href);
                }
                else {
                    throw new ArgumentException();
                }
                this.StationCode = tds[1].InnerHtml;
                this.District = tds[2].InnerHtml;
                this.Road = tds[3].InnerHtml;
                this.Section = tds[4].InnerHtml;
                this.Compass = tds[5].InnerHtml;
            }
            else {
                throw new ArgumentException();
            }
        }
    }


    public class BusQuery {
        public const string BaseUrl = "http://www.szjt.gov.cn/BusQuery/{0}?cid={1}";
        public const string LineAspx = "APTSLine.aspx";
        public const string StationAspx = "default.aspx";
        public const string Cid = "175ecd8d-c39d-4116-83ff-109b946d7cb4";

        public const string LineSearchPara =
            @"&__VIEWSTATE=%2FwEPDwUJNDk3MjU2MjgyD2QWAmYPZBYCAgMPZBYCAgEPZBYCAgYPDxYCHgdWaXNpYmxlaGRkZJoicdBkB9yhQuA%2FOdf79KTvSO4XgMh67pTvZ2uF3nG0" +
            @"&__VIEWSTATEGENERATOR=385F41AE&__EVENTVALIDATION=%2FwEWAwLmzYecBgL88Oh8AqX89aoKoyB8TWJGCcftPn6FMApjVOu8taYx65eqI%2BF5zhRrstE%3D&ctl00%24MainContent" +
            @"%24LineName={0}&ctl00%24MainContent%24SearchLine=%E6%90%9C%E7%B4%A2";


        public static string LineUrl = string.Format(BaseUrl, LineAspx, Cid);
        public static string StationUrl = string.Format(BaseUrl, StationAspx, Cid);

        public static IList<LineInfoEntry> QueryBusInfo(string BusGuid) {
            var s = DownloadString(LineUrl + "&LineGuid=" + BusGuid);
            HtmlNodeCollection tr = GetTable(s);
            var list = new List<LineInfoEntry>();
            foreach (HtmlNode node in tr.Skip(1)) {
                try {
                    var entry = new LineInfoEntry(node);
                    list.Add(entry);
                }
                catch (ArgumentException e) {
                    continue;
                }
            }
            return list;
        }


        public static IList<StationInfoEntry> QueryStationInfo(string StationCode) {
            var s = DownloadString(StationUrl + "&StandCode=" + StationCode);
            var tr = GetTable(s);
            var list = new List<StationInfoEntry>();
            foreach (HtmlNode node in tr.Skip(1)) {
                try {
                    var entry = new StationInfoEntry(node);
                    list.Add(entry);
                }
                catch (ArgumentException e) {
                    continue;
                }
            }
            return list;
        }

        public static IList<LineCandidate> SearchLine(string keyw) {
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

        public static IList<StationCandidate> SearchStation(string keyw) {
            var s = DownloadString(LineUrl + string.Format(LineSearchPara, keyw));
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