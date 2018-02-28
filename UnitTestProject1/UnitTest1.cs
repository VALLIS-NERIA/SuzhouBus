using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibBusQuery;
namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            //BusQuery.QueryBusInfo("d18f2ad5-b746-4a5f-a783-823f963e67fa");
            BusQuery.QueryStationInfo("MNR");
        }
    }
}
