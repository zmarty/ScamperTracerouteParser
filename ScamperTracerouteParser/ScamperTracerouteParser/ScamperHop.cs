namespace ScamperTracerouteParser
{
    using System.Collections.Generic;

    /*
    # 14. PerHopData -- Response data for the first hop.
    #
    #       If multiple IP addresses respond at the same hop, response data
    #       for each IP address are separated by semicolons:
    #
    #       IP,RTT,nTries                   (for only one responding IP)
    #       IP,RTT,nTries;IP,RTT,nTries;... (for multiple responding IPs)
    #
    #         where
    #
    #       IP -- IP address which sent a TTL expired packet
    #       RTT -- RTT of the TTL expired packet
    #       nTries -- number of tries before response received from hop
    #
    #       This field will have the value 'q' if there was no response at
    #       this hop.
    */
    public class ScamperHop
    {
        // If multiple IP addresses respond at the same hop, response data for each IP address are separated by semicolons:
        public List<ScamperHopIPInfo> IPs { get; set; }

        public override string ToString()
        {
            if (this.IPs == null || this.IPs.Count == 0)
            {
                return "(No IPs)";
            }

            return string.Join(", ", this.IPs);
        }
    }
}
