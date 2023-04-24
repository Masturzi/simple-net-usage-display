using System;
using System.Net.NetworkInformation;
using System.Threading;

namespace NetworkUsageMonitor
{
    class Program
    {
        public static (long, long) GetNetworkUsage()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            long totalBytesSent = 0;
            long totalBytesReceived = 0;

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPv4InterfaceStatistics statistics = networkInterface.GetIPv4Statistics();
                    totalBytesSent += statistics.BytesSent;
                    totalBytesReceived += statistics.BytesReceived;
                }
            }

            return (totalBytesSent, totalBytesReceived);
        }

        public static void DisplayNetworkUsage(long oldBytesSent, long oldBytesReceived)
        {
            (long newBytesSent, long newBytesReceived) = GetNetworkUsage();
            long sentDifference = newBytesSent - oldBytesSent;
            long receivedDifference = newBytesReceived - oldBytesReceived;

            Console.WriteLine($"Bytes sent: {sentDifference}");
            Console.WriteLine($"Bytes received: {receivedDifference}");
            Console.WriteLine($"Total data: {sentDifference + receivedDifference}");
        }

        public static void Main(string[] args)
        {
            while (true)
            {
                (long oldBytesSent, long oldBytesReceived) = GetNetworkUsage();
                Thread.Sleep(1000);
                DisplayNetworkUsage(oldBytesSent, oldBytesReceived);
            }
        }
    }
}
