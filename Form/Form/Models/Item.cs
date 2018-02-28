using System;
using System.Collections.Generic;
using LibBusQuery;

namespace Form.Models {
    //public interface Item : IInfoEntry { }
    public class SimpleEntry : IInfoEntry {
        public string Id { get; set; } = "123";
        public string Name { get; set; } = "New simple entry";
        public string Description { get; set; } = "Just a simple entry";
        public Dictionary<string, string> Properties { get; }

        public IEnumerable<IInfoEntry> LinkClick() { throw new NotImplementedException($"{this.Name}"); }
    }
}