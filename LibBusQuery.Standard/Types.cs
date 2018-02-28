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
    public interface IInfoEntry {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        Dictionary<string, string> Properties { get; }

        IEnumerable<IInfoEntry> LinkClick();
    }

    public delegate IEnumerable<IInfoEntry> QueryMethod(string keyword);

    public class InfoList : List<IInfoEntry> {
        public IInfoEntry Head { get; set; }
    }

    public struct QueryRequest {
        public QueryMethod Method;
        public string keyword;
    }

    public class BusStation {
        public string StationCode;
        public string StationName;

        public BusStation(string href) {
            href = WebUtility.HtmlDecode(href);
            var match = Regex.Matches(href, @"(?<=.+?=).+?(?=&|$)");
            this.StationCode = match[1].Value;
            this.StationName = match[2].Value;
        }
    }

    public class BusLine {
        public string LineGuid;
        public string LineName;

        public BusLine(string href) {
            href = WebUtility.HtmlDecode(href);
            var match = Regex.Matches(href, @"(?<=.+?=).+?(?=&|$)");
            this.LineGuid = match[1].Value;
            this.LineName = match[2].Value;
        }
    }

    public abstract class StationBase : IInfoEntry {
        public BusStation Station;
        public string StationName;
        public string StationCode;

        /// <exception cref="ArgumentException"></exception>
        protected StationBase(HtmlNode node) {
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
                this.StationName = tds[0].InnerText;
                this.StationCode = tds[1].InnerText;
                this.Properties[nameof(this.StationName)] = this.StationName;
                this.Properties[nameof(this.StationCode)] = this.StationCode;
            }
            else {
                throw new ArgumentException();
            }
        }

        public string Id => this.StationCode;
        public virtual string Name => $"{this.StationName} ({this.StationCode})";
        public abstract string Description { get; }
        public Dictionary<string, string> Properties { get; protected set; } = new Dictionary<string, string>();

        public virtual IEnumerable<IInfoEntry> LinkClick() { return (IEnumerable<IInfoEntry>) BusQuery.QueryStationInfo(this.StationCode); }
    }

    public abstract class LineBase : IInfoEntry {
        public BusLine Line;
        public string LineName;
        public string Direction;


        /// <exception cref="ArgumentException"></exception>
        protected LineBase(HtmlNode node) {
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
                this.LineName = tds[0].InnerText;
                this.Direction = WebUtility.HtmlDecode(tds[1].InnerText);
                this.Properties[nameof(this.LineName)] = this.LineName;
                this.Properties[nameof(this.Direction)] = this.Direction;
                this.Properties[nameof(this.Line.LineGuid)] = this.Line.LineGuid;
            }
            else {
                throw new ArgumentException();
            }
        }

        public string Id => this.Line.LineGuid;
        public virtual string Name => $"{this.LineName} ({this.Direction})";
        public abstract string Description { get; }
        public Dictionary<string, string> Properties { get; protected set; } = new Dictionary<string, string>();

        public virtual IEnumerable<IInfoEntry> LinkClick() { return (IEnumerable<IInfoEntry>) BusQuery.QueryBusInfo(this.Line.LineGuid); }
    }


    // TODO:给以下类型加上Properties

    /// <summary>
    /// Entry when querying a bus line
    /// 站台 编号 车牌 进站时间
    /// </summary>
    [DebuggerDisplay("{Station.StationName},{StationCode},{BusNum},{EnterTime}")]
    public class StationInLine : StationBase {
        public string BusNum;
        public DateTime? EnterTime;

        public StationInLine(HtmlNode node) : base(node) {
            var tds = node.SelectNodes("td");
            this.BusNum = tds[2].InnerText;
            var time = tds[3].InnerText;
            if (time != "") {
                this.EnterTime = DateTime.Parse(tds[3].InnerText);
            }
            else {
                this.EnterTime = null;
            }
        }

        public override string ToString() =>
            $"{this.StationName} ({this.StationCode}): " +
            $"{(this.EnterTime == null ? "--- " : $"{this.BusNum} at {this.EnterTime.Value:T}")}";


        public override string Description => $"{(this.EnterTime == null ? "--- " : $"{this.BusNum} at {this.EnterTime.Value:T}")}";
    }

    /// <summary>
    /// Entry when querying a bus station, like the plate
    /// 线路 方向 车牌 更新时间 站距
    /// </summary>
    [DebuggerDisplay("{Line.LineName},{Direction},{BusNum},{UpdateTime},{Distance}")]
    public class LineOnStation : LineBase {
        public string DebugDisplay => $"{nameof(Line.LineName)},{nameof(Direction)},{nameof(BusNum)},{nameof(UpdateTime)},{nameof(Distance)}";
        public string BusNum;
        public DateTime? UpdateTime;
        public int? Distance;

        public LineOnStation(HtmlNode node) : base(node) {
            var tds = node.SelectNodes("td");
            this.BusNum = tds[2].InnerText;
            var time = tds[3].InnerText;
            if (time != "") {
                this.UpdateTime = DateTime.Parse(tds[3].InnerText);
            }
            else {
                this.UpdateTime = null;
            }
            var dist = tds[4].InnerText;
            if (int.TryParse(dist, out int d)) {
                this.Distance = d;
            }
            else if (dist == "进站") {
                this.Distance = 0;
            }
            else {
                this.Distance = null;
            }
        }

        public override string Description => this.UpdateTime != null ? $"in {this.Distance}@{this.UpdateTime.Value:T}" : "No Bus";
    }

    /// <summary>
    /// Entry when searching a bus line
    /// </summary>
    [DebuggerDisplay("{Line.LineName},{Direction}")]
    public class LineCandidate : LineBase {
        public LineCandidate(HtmlNode node) : base(node) { }

        public override string ToString() => $"{this.LineName}: {this.Direction}";

        public override string Description => "";
    }

    /// <summary>
    /// Entry when searching a station
    /// </summary>
    public class StationCandidate : StationBase {
        public string District, Road, Section, Compass;

        public StationCandidate(HtmlNode node) : base(node) {
            var tds = node.SelectNodes("td");
            this.District = tds[2].InnerText;
            this.Road = tds[3].InnerText;
            this.Section = tds[4].InnerText;
            this.Compass = tds[5].InnerText;
        }

        public override string Description => $"{this.Compass} side of {this.District}, {this.Road}, {this.Section}";
    }
}