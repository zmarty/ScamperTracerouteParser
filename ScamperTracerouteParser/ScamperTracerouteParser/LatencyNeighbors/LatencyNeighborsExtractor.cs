namespace ScamperTracerouteParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public static class LatencyNeighborsExtractor
    {
        public static List<LatencyNeighborPair> FindLatencyNeighbors(this ScamperTraceroute traceroute, decimal maximumLatencyDiff = 1, byte maxHopDifference = 255, decimal maxRTT = 262144)
        {
            return FindNeighbors(traceroute.Hops, maximumLatencyDiff, maxHopDifference, maxRTT);
        }

        public static List<LatencyNeighborPair> FindNeighbors(List<ScamperHop> hops, decimal maximumLatencyDiff = 1, byte maxHopDifference = 255, decimal maxRTT = 262144)
        {
            var neighbors = new List<LatencyNeighborPair>();

            if (hops == null || hops.Count <= 1)
            {
                return neighbors;
            }

            for (var i = 0; i < hops.Count - 1; i++)
            {
                var currentHop = hops[i];

                for (var j = i + 1; j < hops.Count; j++)
                {
                    var neighborHop = hops[j];

                    if (j-i <= maxHopDifference)
                    {
                        neighbors.AddRange(FindNeighborsInHopPair(currentHop, i, neighborHop, j, maximumLatencyDiff, maxRTT));
                    }
                }
            }

            return neighbors;
        }

        private static IEnumerable<LatencyNeighborPair> FindNeighborsInHopPair(ScamperHop currentHop, int currentHopIndex, ScamperHop neighborHop, int neighborHopIndex, decimal maximumLatencyDiff, decimal maxRTT)
        {
            if (currentHop != null && neighborHop != null && currentHop.IPs != null && neighborHop.IPs != null)
            {
                for (var i = 0; i < currentHop.IPs.Count; i++)
                {
                    var currentIPInfo = currentHop.IPs[i];

                    if (currentIPInfo != null && currentIPInfo.IP != null)
                    {
                        for (var j = 0; j < neighborHop.IPs.Count; j++)
                        {
                            var neighborIPInfo = neighborHop.IPs[j];

                            if (neighborIPInfo != null && neighborIPInfo.IP != null && currentIPInfo.IP != neighborIPInfo.IP && currentIPInfo.RTT <= maxRTT && neighborIPInfo.RTT <= maxRTT)
                            {
                                var latencyDiff = Math.Abs(currentIPInfo.RTT - neighborIPInfo.RTT);

                                if (latencyDiff <= maximumLatencyDiff)
                                {
                                    yield return new LatencyNeighborPair()
                                    {
                                        Neighbor1 = currentIPInfo.IP,
                                        Neighbor2 = neighborIPInfo.IP,
                                        Neighbor1HopIndex = currentHopIndex,
                                        Neighbor2HopIndex = neighborHopIndex,
                                        Neighbor1RTT = currentIPInfo.RTT,
                                        Neighbor2RTT = neighborIPInfo.RTT,
                                        DiffDistanceInHops = neighborHopIndex - currentHopIndex,
                                        LatencyDiff = latencyDiff
                                    };
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
