using NATS.Client;
using System;
using System.Text;
using System.Threading;

namespace Nats.Publisher
{
    class Program
    {
        public static void Main(string[] args)
        {
            int messageCount = 0;
            bool isRunning;

            try
            {
                // Create Nats Connection
                ConnectionFactory factory = new();
                Options options = ConnectionFactory.GetDefaultOptions();
                options.Url = "nats://localhost:4222";
                IConnection connection = factory.CreateConnection(options);

                do
                {
                    Console.Clear();
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("NATS Publisher is listening for events from NATS Server");
                    Console.WriteLine("------------------------------------------------------");

                    // Publish messages to NATS
                    for (int i = 0; i < messageCount; i++)
                    {
                        string subject = "nats.demo.hello";
                        string message = $"Message {i} to SUBJECT: {subject}";
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        connection.Publish(subject, data);

                        Console.WriteLine($"Sending: {message}");

                        Thread.Sleep(10);
                    }

                    Console.WriteLine("Enter the number of messages to PUBLISH to NATS or 0 to exit");
                    bool isValid = int.TryParse(Console.ReadLine(), out messageCount);
                    isRunning = isValid && messageCount > 0;
                } while (isRunning);

                connection.Drain();
                connection.Close();

            }
            catch (NATSConnectionException ex)
            {
                Console.WriteLine("Unable to connect to NATS server. ERROR {0}", ex.Message);
                return;
            }
        }
    }
}
