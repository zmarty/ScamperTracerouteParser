namespace ScamperTracerouteParser.Console
{
    using System;
    using System.Globalization;

    class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;
            ScamperTraceroute currentTraceroute = null;

            foreach (var traceroute in TracerouteTextDumpParser.ParseFile(@"D:\Downloads\daily.txt"))
            {
                ////Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} - {1} - {2}", traceroute.Source, traceroute.Destination, traceroute.Hops.Count));

                counter++;
                currentTraceroute = traceroute;

                if (counter % 10000 == 0)
                {
                    Console.WriteLine(counter);
                }
            }

            foreach (var neighborPair in currentTraceroute.FindLatencyNeighbors())
            {
                Console.WriteLine(neighborPair);
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
