namespace ScamperTracerouteParser
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;

    public class TracerouteTextDumpParser
    {
        public static IEnumerable<ScamperTraceroute> ParseFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var traceroute = ParseLine(line);

                    if (traceroute != null)
                    {
                        yield return traceroute;
                    }
                }
            }
        }
        public static ScamperTraceroute ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }

            line = line.Trim();

            if (line.StartsWith("#"))
            {
                return null;
            }

            var parts = line.Split(new char[] { '\t' });

            // 1. Key -- Indicates the type of line and determines the meaning of the remaining fields.  This will always be 'T' for an IP trace.
            if (parts.Length == 0 || parts[0] != "T")
            {
                return null;
            }

            // We want to set each field separately to help in debugging
            // If we would set all fields at the same time the debugger would not show exactly which field is problematic
            var traceroute = new ScamperTraceroute();

            // 2. Source -- Source IP of skitter/scamper monitor performing the trace.
            traceroute.Source = IPAddress.Parse(parts[1]);

            // 3.Destination-- Destination IP being traced.
            traceroute.Destination = IPAddress.Parse(parts[2]);

            /*
            #  4. ListId -- ID of the list containing this destination address.
            #
            #        This value will be zero if no list ID was provided.  (uint32_t)
            */
            traceroute.ListId = int.Parse(parts[3]);

            /*
            #  5. CycleId -- ID of current probing cycle (a cycle is a single run
            #                through a given list).  For skitter traces, cycle IDs
            #                will be equal to or slightly earlier than the timestamp
            #                of the first trace in each cycle. There is no standard
            #                interpretation for scamper cycle IDs.
            #
            #        This value will be zero if no cycle ID was provided.  (uint32_t)
            */
            traceroute.CycleId = int.Parse(parts[4]);

            // 6. Timestamp -- Timestamp when trace began to this destination.
            traceroute.Timestamp = int.Parse(parts[5]);
            traceroute.TraceStartDateTime = UnixTimeStampToDateTime(int.Parse(parts[5]));

            /*
            #  7. DestReplied -- Whether a response from the destination was received.
            #
            #        R - Replied, reply was received
            #        N - Not-replied, no reply was received;
            #            Since skitter sends a packet with a TTL of 255 when it halts
            #            probing, it is still possible for the final destination to
            #            send a reply and for the HaltReasonData (see below) to not
            #            equal no_halt.  Note: scamper does not perform last-ditch
            #            probing at TTL 255 by default.
            */
            traceroute.DestinationReplied = ParseDestReplied(parts[6]);

            // 8. DestRTT -- RTT (ms) of first response packet from destination. 0 if DestReplied is N.
            traceroute.DestinationRTT = double.Parse(parts[7]);

            // 9. RequestTTL -- TTL set in request packet which elicited a response (echo reply) from the destination. 0 if DestReplied is N.
            traceroute.RequestTTL = int.Parse(parts[8]);

            // 10. ReplyTTL -- TTL found in reply packet from destination; 0 if DestReplied is N.
            traceroute.ReplyTTL = int.Parse(parts[9]);

            /*
            # 11. HaltReason -- The reason, if any, why incremental probing stopped.
            #
            # 12. HaltReasonData -- Extra data about why probing halted.
            #
            #        HaltReason            HaltReasonData
            #        ------------------------------------
            #        S (success/no_halt)    0
            #        U (icmp_unreachable)   icmp_code
            #        L (loop_detected)      loop_length
            #        G (gap_detected)       gap_limit
            */
            traceroute.HaltReason = ParseHaltReason(parts[10]);
            traceroute.HaltReasonData = parts[11];

            /*
            # 13. PathComplete -- Whether all hops to destination were found.
            #
            #        C - Complete, all hops found
            #        I - Incomplete, at least one hop is missing (i.e., did not
            #            respond)
            */
            traceroute.PathComplete = ParsePathComplete(parts[12]);

            var hops = new List<ScamperHop>();

            for (var i = 13; i < parts.Length; i++)
            {
                var hopData = parts[i];

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
                hops.Add(ParseHop(hopData));
            }

            traceroute.Hops = hops;

            return traceroute;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dt = dt.AddSeconds(unixTimeStamp);
            return dt;
        }

        /*
        #  7. DestReplied -- Whether a response from the destination was received.
        #
        #        R - Replied, reply was received
        #        N - Not-replied, no reply was received;
        #            Since skitter sends a packet with a TTL of 255 when it halts
        #            probing, it is still possible for the final destination to
        #            send a reply and for the HaltReasonData (see below) to not
        #            equal no_halt.  Note: scamper does not perform last-ditch
        #            probing at TTL 255 by default.
        */
        public static DestReplied ParseDestReplied(string value)
        {
            switch (value)
            {
                case "R":
                    return DestReplied.Replied;
                case "N":
                    return DestReplied.NotReplied;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Provided value is invalid in ParseDestReplied: {0}", value));
            }
        }

        /*
        # 11. HaltReason -- The reason, if any, why incremental probing stopped.
        #
        # 12. HaltReasonData -- Extra data about why probing halted.
        #
        #        HaltReason            HaltReasonData
        #        ------------------------------------
        #        S (success/no_halt)    0
        #        U (icmp_unreachable)   icmp_code
        #        L (loop_detected)      loop_length
        #        G (gap_detected)       gap_limit
        */
        public static HaltReason ParseHaltReason(string value)
        {
            switch (value)
            {
                case "S":
                    return HaltReason.Success;
                case "U":
                    return HaltReason.Unreachable;
                case "L":
                    return HaltReason.LoopDetected;
                case "G":
                    return HaltReason.GapDetected;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Provided value is invalid in ParseHaltReason: {0}", value));
            }
        }

        /*
        # 13. PathComplete -- Whether all hops to destination were found.
        #
        #        C - Complete, all hops found
        #        I - Incomplete, at least one hop is missing (i.e., did not
        #            respond)
        */
        public static PathComplete ParsePathComplete(string value)
        {
            switch (value)
            {
                case "C":
                    return PathComplete.Complete;
                case "I":
                    return PathComplete.Incomplete;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Provided value is invalid in ParsePathComplete: {0}", value));
            }
        }

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
        public static ScamperHop ParseHop(string value)
        {
            // This field will have the value 'q' if there was no response at this hop.
            if (value == "q")
            {
                return null;
            }

            var rawIPsInfo = value.Split(new char[] { ';' });

            var ipInfosInHop = new List<ScamperHopIPInfo>();

            foreach (var rawIPInfo in rawIPsInfo)
            {
                var infoParts = rawIPInfo.Split(new char[] { ',' });

                if (infoParts.Length != 3)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Number of infoParts in ParseHop was not 3 for rawIPInfo: {0}", rawIPInfo));
                }

                var hopIPInfo = new ScamperHopIPInfo()
                {
                    IP = IPAddress.Parse(infoParts[0]),
                    RTT = double.Parse(infoParts[1]),
                    Tries = int.Parse(infoParts[2])
                };

                ipInfosInHop.Add(hopIPInfo);
            }

            var hop = new ScamperHop()
            {
                IPs = ipInfosInHop
            };

            return hop;
        }
    }
}
