namespace ScamperTracerouteParser.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Net;

    [TestClass]
    public class TestParser
    {
        [DeploymentItem(@"App_Data\ScamperSample1.txt", @"App_Data")]
        [TestMethod]
        public void TestFieldExtraction()
        {
            ScamperTraceroute currentTraceroute = null;

            foreach (var traceroute in TracerouteTextDumpParser.ParseFile(@"App_Data\ScamperSample1.txt"))
            {
                currentTraceroute = traceroute;
            }

            Assert.AreEqual(IPAddress.Parse("129.186.1.240"), currentTraceroute.Source);
            Assert.AreEqual(IPAddress.Parse("2.50.184.243"), currentTraceroute.Destination);
            Assert.AreEqual(7, currentTraceroute.ListId);
            Assert.AreEqual(5520, currentTraceroute.CycleId);
            Assert.AreEqual(1489105699, currentTraceroute.Timestamp);
            Assert.AreEqual(new DateTime(2017, 3, 10, 0, 28, 19), currentTraceroute.TraceStartDateTime);
            Assert.AreEqual(DestReplied.NotReplied, currentTraceroute.DestinationReplied);
            Assert.AreEqual(0, currentTraceroute.DestinationRTT);
            Assert.AreEqual(0, currentTraceroute.RequestTTL);
            Assert.AreEqual(0, currentTraceroute.ReplyTTL);
            Assert.AreEqual(HaltReason.GapDetected, currentTraceroute.HaltReason);
            Assert.AreEqual("0", currentTraceroute.HaltReasonData);
            Assert.AreEqual(PathComplete.Incomplete, currentTraceroute.PathComplete);

            Assert.AreEqual(14, currentTraceroute.Hops.Count);

            var lastHop = currentTraceroute.Hops[13];
            Assert.AreEqual(2, lastHop.IPs.Count);

            var lastIPInfoInHop = lastHop.IPs[1];
            Assert.AreEqual(IPAddress.Parse("10.246.250.98"), lastIPInfoInHop.IP);
            Assert.AreEqual(26434.998, lastIPInfoInHop.RTT);
            Assert.AreEqual(3, lastIPInfoInHop.Tries);
        }
    }
}
