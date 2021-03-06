﻿namespace ScamperTracerouteParser
{
    using System.Globalization;
    using System.Net;

    public class ScamperHopIPInfo
    {
        // IP -- IP address which sent a TTL expired packet
        public IPAddress IP { get; set; }

        // WARNING: Do not use float here or the parser will incorrectly store some values
        // RTT -- RTT of the TTL expired packet
        public decimal RTT { get; set; }

        // nTries -- number of tries before response received from hop
        public int Tries { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1}, {2})", this.IP, this.RTT, this.Tries);
        }
    }
}
