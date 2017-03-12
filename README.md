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
wget --user=USER --password=PASSWORD --no-parent --recursive https://topo-data.caida.org/team-probing/list-7.allpref24/team-1/daily/2017/
wget --user=USER --password=PASSWORD --no-parent --recursive https://topo-data.caida.org/team-probing/list-7.allpref24/team-2/daily/2017/
wget --user=USER --password=PASSWORD --no-parent --recursive https://topo-data.caida.org/team-probing/list-7.allpref24/team-3/daily/2017/

gunzip -r ./
```
