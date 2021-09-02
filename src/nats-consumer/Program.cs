using NATS.Client;
using System;
using System.Text;

namespace Nats.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine(" NATS Producer is up and running");
            Console.WriteLine("------------------------------------------------------");

            string queue = args.Length == 1
                ? args[0]
                : null;

            try
            {
                // Create Nats Connection
                ConnectionFactory factory = new();
                Options options = ConnectionFactory.GetDefaultOptions();
                options.Url = "nats://localhost:4222";
                IConnection connection = factory.CreateConnection(options);
                
                IAsyncSubscription subscription;

                // Subscribe
                if (queue == null)
                {
                    subscription = connection.SubscribeAsync(
                        "nats.demo.hello",      // <------ this is the subject to subscribe to
                        (sender, args) =>       // <------ Message Handler
                        {
                            string data = Encoding.UTF8.GetString(args.Message.Data);
                            Console.WriteLine($"DATA {data}");
                        });
                }
                if (queue == "reply")
                {
                     subscription = connection.SubscribeAsync(
                        "nats.demo.hello",      // <------ this is the subject to subscribe to
                        (sender, args) =>       // <------ Message Handler
                        {
                            string data = Encoding.UTF8.GetString(args.Message.Data);
                            Console.WriteLine($"DATA {data}");

                            if (args.Message.Reply != null)     // <------------- If reply is present ACK the message
                            {
                                connection.Publish(args.Message.Reply, Encoding.UTF8.GetBytes($"ACK: for {data}"));
                            }
                        });
                }
                else
                {
                    subscription = connection.SubscribeAsync(
                        "nats.demo.hello",      // <------ this is the subject to subscribe to
                        queue,                  // <------ This is the QueueGroup
                        (sender, args) =>       // <------ Message Handler
                        {
                            string data = Encoding.UTF8.GetString(args.Message.Data);
                            Console.WriteLine($"DATA {data}");
                        });
                }

                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();

                connection.Drain();
                connection.Close();
            }
            catch (NATSConnectionException nex)
            {
                Console.WriteLine("Unable to connect to NATS server. ERROR {0}", nex.Message);
                return;
            }
        }
    }
}
