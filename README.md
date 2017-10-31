# Scamper Traceroute Parser
C# Library that can parse the text traceroute dumps extracted from binary warts files using sc_analysis_dump (part of the CAIDA Scamper project).

This library assumes you are parsing the text representation extracted from *warts* files. It also assumes the text representation contains **all** the columns (sc_analysis_dump allows you to use command line parameters to skip some columns).

## Collecting and unpacking traceroutes

### Prerequisites
- Install [scamper](https://www.caida.org/tools/measurement/scamper/), which contains the [sc_analysis_dump](https://www.caida.org/tools/measurement/scamper/man/sc_analysis_dump.1.pdf) tool
  - *apt-get install scamper*
- Collect traceroutes in the warts format using scamper, or get access to the [public](https://www.caida.org/data/active/ipv4_routed_24_topology_dataset.xml) or [restricted](https://www.caida.org/data/active/ipv4_routed_24_topology_dataset.xml) datasets from [CAIDA Ark](https://www.caida.org/projects/ark/topo_datasets.xml)

### Example of downloading and unpacking the restricted topology dataset
To access the restricted database you need to [request access](https://www.caida.org/data/active/topology_request.xml). CAIDA will give you the *USERNAME* and *PASSWORD* used below.

The example below downloads all daily data for 2017. If you need to download data for a different year, change the paths.

```Bash
# Download 2017 daily results for team-1, team-2, and team-3
wget --user=USER --password=PASSWORD --no-parent --recursive https://topo-data.caida.org/team-probing/list-7.allpref24/team-1/daily/2017/
wget --user=USER --password=PASSWORD --no-parent --recursive https://topo-data.caida.org/team-probing/list-7.allpref24/team-2/daily/2017/
wget --user=USER --password=PASSWORD --no-parent --recursive https://topo-data.caida.org/team-probing/list-7.allpref24/team-3/daily/2017/

# gunzip all warts files, one by one
gunzip -r ./

# Or gunzip multiple files in parallel (set to 16 at a time):
find . -name '*.gz' -print0 | xargs -0 -I {} -P 16 gunzip {}

# Run sc_analysis_dump on all warts files individually, and output the result as individual .txt files, one for each warts file
find . -name "*.warts" -exec sh -c 'sc_analysis_dump "{}" > "{}.txt"' \;

# Combine all txt files for January 2017 across all 3 teams into a single concatenated txt file (note you need to be in a parent folder!)
find team-*/daily/2017/cycle-201701*/*.txt -exec cat {} + >> 2017-01-traceroutes.txt

# Or run sc_analysis_dump on all warts files at the same time, and output a single .txt file
find . -name "*.warts" -exec sc_analysis_dump "{}" > "all_traceroutes.txt" +
```
## Parsing traceroutes

### Parsing a txt file extracted with sc_analysis_dump

```C#
var counter = 0;

foreach (var traceroute in TracerouteTextDumpParser.ParseFile(@"D:\Downloads\daily.txt"))
{
    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} - {1} - {2}", traceroute.Source, traceroute.Destination, traceroute.Hops.Count));

    counter++;

    if (counter % 10000 == 0)
    {
        Console.WriteLine(counter);
    }
}
```

The ScamperTraceroute class contains fields extracted from all columns of the txt file

```C#
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
        public double DestinationRTT { get; set; }

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
```

Each *ScamperHop* contains a list of *ScamperHopIPInfo*:

```C#
namespace ScamperTracerouteParser
{
    using System.Net;

    public class ScamperHopIPInfo
    {
        // IP -- IP address which sent a TTL expired packet
        public IPAddress IP { get; set; }

        // WARNING: Do not use float here or the parser will incorrectly store some values
        // RTT -- RTT of the TTL expired packet
        public double RTT { get; set; }

        // nTries -- number of tries before response received from hop
        public int Tries { get; set; }
    }
}
```
