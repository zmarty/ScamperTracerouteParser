namespace ScamperTracerouteParser
{
    using System.Globalization;
    using System.Net;

    public class LatencyNeighborPair
    {
        public IPAddress Neighbor1 { get; set; }

        public IPAddress Neighbor2 { get; set; }

        public int Neighbor1HopIndex { get; set; }

        public int Neighbor2HopIndex { get; set; }

        public int DiffDistanceInHops { get; set; }

        public decimal LatencyDiff { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} - {1} - {2}ms", this.Neighbor1, this.Neighbor2, this.LatencyDiff);
        }
    }
}
