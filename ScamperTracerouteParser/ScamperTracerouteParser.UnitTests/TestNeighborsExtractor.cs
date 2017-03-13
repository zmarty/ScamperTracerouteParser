namespace ScamperTracerouteParser.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Net;

    [TestClass]
    public class TestNeighborsExtractor
    {
        [DeploymentItem(@"App_Data\ScamperSample1.txt", @"App_Data")]
        [TestMethod]
        public void TestNeighbors()
        {
            ScamperTraceroute currentTraceroute = null;

            foreach (var traceroute in TracerouteTextDumpParser.ParseFile(@"App_Data\ScamperSample1.txt"))
            {
                currentTraceroute = traceroute;
            }

            var neighborPairs = currentTraceroute.FindLatencyNeighbors();

            Assert.AreEqual(10, neighborPairs.Count);
            var lastPair = neighborPairs[neighborPairs.Count - 1];

            Assert.AreEqual(IPAddress.Parse("195.229.4.97"), lastPair.Neighbor1);
            Assert.AreEqual(IPAddress.Parse("10.246.250.98"), lastPair.Neighbor2);

            Assert.AreEqual(12, lastPair.Neighbor1HopIndex);
            Assert.AreEqual(13, lastPair.Neighbor2HopIndex);

            Assert.AreEqual(1, lastPair.DiffDistanceInHops);

            Assert.AreEqual((decimal)0.1, lastPair.LatencyDiff);
        }
    }
}
