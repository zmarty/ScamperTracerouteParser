namespace ScamperTracerouteParser
{
    /*
    # 13. PathComplete -- Whether all hops to destination were found.
    #
    #        C - Complete, all hops found
    #        I - Incomplete, at least one hop is missing (i.e., did not
    #            respond)
    */
    public enum PathComplete
    {
        Incomplete = 0,
        Complete = 1
    }
}
