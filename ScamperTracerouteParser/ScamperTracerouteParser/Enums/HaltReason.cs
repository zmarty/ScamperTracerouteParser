namespace ScamperTracerouteParser
{
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
    public enum HaltReason
    {
        Unknown = -1,
        Unreachable = 0,
        Success = 1,
        LoopDetected = 2,
        GapDetected = 3
    }
}
