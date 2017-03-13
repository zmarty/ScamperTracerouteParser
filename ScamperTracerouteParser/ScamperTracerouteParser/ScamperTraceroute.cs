namespace ScamperTracerouteParser
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class ScamperTraceroute
    {
        // 2. Source -- Source IP of skitter/scamper monitor performing the trace.
        public IPAddress Source { get; set; }

        // 3. Destination -- Destination IP being traced.
        public IPAddress Destination { get; set; }

        /*
        #  4. ListId -- ID of the list containing this destination address.
        #
        #        This value will be zero if no list ID was provided.  (uint32_t)
         */
        public int ListId { get; set; }

        /*
        #  5. CycleId -- ID of current probing cycle (a cycle is a single run
        #                through a given list).  For skitter traces, cycle IDs
        #                will be equal to or slightly earlier than the timestamp
        #                of the first trace in each cycle. There is no standard
        #                interpretation for scamper cycle IDs.
        #
        #        This value will be zero if no cycle ID was provided.  (uint32_t)
         */
        public int CycleId { get; set; }

        public int Timestamp { get; set; }

        public DateTime TraceStartDateTime { get; set; }

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
        public DestReplied DestinationReplied { get; set; }

        // WARNING: Do not use float here or the parser will incorrectly store some values
        // 8. DestRTT -- RTT (ms) of first response packet from destination. 0 if DestReplied is N.
        public decimal DestinationRTT { get; set; }

        // 9. RequestTTL -- TTL set in request packet which elicited a response (echo reply) from the destination. 0 if DestReplied is N.
        public int RequestTTL { get; set; }

        // 10. ReplyTTL -- TTL found in reply packet from destination; 0 if DestReplied is N.
        public int ReplyTTL { get; set; }

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
        public HaltReason HaltReason { get; set; }

        public string HaltReasonData { get; set; }

        /*
        # 13. PathComplete -- Whether all hops to destination were found.
        #
        #        C - Complete, all hops found
        #        I - Incomplete, at least one hop is missing (i.e., did not
        #            respond)
         */
        public PathComplete PathComplete { get; set; }

        public List<ScamperHop> Hops { get; set; }
    }
}
