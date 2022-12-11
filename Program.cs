using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

class InternetUsage
{
    private PerformanceCounterCategory category;
    private string networkCard;

    public InternetUsage(string networkCard)
    {
        this.networkCard = networkCard;
        category = new PerformanceCounterCategory("Network Interface");
    }

    public float GetCurrentUsage()
    {
        var instances = category.GetInstanceNames();

        foreach (var instance in instances)
        {
            if (instance == networkCard)
            {
                using (var downloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance))
                using (var uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance))
                {
                    return downloadCounter.NextValue() + uploadCounter.NextValue();
                }
            }
        }

        throw new Exception("Network card not found");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var networkCards = NetworkInterface.GetAllNetworkInterfaces();
        for (int i = 0; i < networkCards.Length; i++)
        {
            Console.WriteLine($"{i + 1}. Name: {networkCards[i].Name} - Description: {networkCards[i].Description}");
        }

        Console.Write("Enter # of the interface: ");
        var input = Console.ReadLine();
        int index;
        if (!int.TryParse(input, out index) || index < 1 || index > networkCards.Length)
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        try
        {
            var usage = new InternetUsage(networkCards[index - 1].Name);

            while (true)
            {
                var currentUsage = usage.GetCurrentUsage();
                Console.WriteLine($"Current internet usage: {currentUsage} bytes/sec");
                System.Threading.Thread.Sleep(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.ReadKey();
    }
}
