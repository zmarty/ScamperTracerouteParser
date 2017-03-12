namespace ScamperTracerouteParser
{
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
    public enum DestReplied
    {
        NotReplied = 0,
        Replied = 1
    }
}
