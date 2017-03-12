namespace ScamperTracerouteParser.Console
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;

            foreach (var traceroute in TracerouteTextDumpParser.ParseFile(@"D:\Downloads\daily.txt"))
            {
                ////Console.WriteLine(traceroute.Destination);

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
