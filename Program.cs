using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;

namespace SimplePortScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PortScanner";
            Console.WriteLine("Portscanner by syst3m1z3r");
            Console.WriteLine();
            Console.WriteLine("Targetaddress:");

            string address = Console.ReadLine();
            Console.WriteLine("Range (s-e):");
            string[] r_Range = Console.ReadLine().Split('-');
            Range range = new Range(int.Parse(r_Range[0]), int.Parse(r_Range[1]));

            Console.WriteLine("Timeout:");
            int timeout = int.Parse(Console.ReadLine());

            Console.WriteLine("Scanner starting...");
            var scanner = new Scanner(IPAddress.Parse(address), range, timeout);
            scanner.Start();
        }
    }

    class Scanner
    {
        private IPAddress Address { get; set; }
        private Range Range { get; set; }
        private int Timeout { get; set; }

        public Scanner(IPAddress address, Range range, int timeout)
        {
            Address = address;
            Range = range;
            Timeout = timeout;
        }

        public void Start()
        {
            List<int> openPorts = new List<int>();

            int portRangeStart = Range.Start.Value;
            int portRangeEnd = Range.End.Value;

            while (true)
            {
                if (portRangeStart > portRangeEnd)
                    break;

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint remoteEndPoint = new IPEndPoint(Address, portRangeStart);

                var result = socket.BeginConnect(remoteEndPoint, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(Timeout, true);

                if (success)
                    openPorts.Add(portRangeStart);

                Console.WriteLine($"Port is {(success ? "open" : "closed")}!\t{Address}\tPort:{portRangeStart++}");
            }

            Console.WriteLine($"Port scanning closed.");
            openPorts.ForEach(x => Console.Write(x));
        }
    }
}
