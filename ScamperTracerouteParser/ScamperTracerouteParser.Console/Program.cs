namespace ScamperTracerouteParser.Console
{
    using System;
    using System.Globalization;

    class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;

            foreach (var traceroute in TracerouteTextDumpParser.ParseFile(@"D:\Downloads\daily.txt"))
            {
                ////Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} - {1} - {2}", traceroute.Source, traceroute.Destination, traceroute.Hops.Count));

                counter++;

                if (counter % 10000 == 0)
                {
                    Console.WriteLine(counter);
                }
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
