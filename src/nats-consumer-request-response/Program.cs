using NATS.Client;
using System;
using System.Text;

namespace Nats.Consumer.RequestResponse
{
    class Program
    {
        static void Main(string[] args)
        {
            string clientId = args.Length == 1 ? args[0] : Guid.NewGuid().ToString();

            Console.Clear();
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("NATS consumer request/response is up and running");
            Console.WriteLine($"CLIENT-ID: {clientId}");
            Console.WriteLine("------------------------------------------------------");
            
            try
            {
                // Create Nats Connection
                ConnectionFactory factory = new();
                Options options = ConnectionFactory.GetDefaultOptions();
                options.Url = "nats://localhost:4222";
                IConnection connection = factory.CreateConnection(options);

                IAsyncSubscription subscription = connection.SubscribeAsync(
                   "nats.demo.request.response",      // <------ this is the subject to subscribe to
                   (sender, args) =>       // <------ Message Handler
                   {
                       string data = Encoding.UTF8.GetString(args.Message.Data);
                       Console.WriteLine($"DATA {data}");

                       if (args.Message.Reply != null)     // <------------- If reply is present ACK the message
                       {
                           Console.WriteLine("Repying to message");
                           connection.Publish(args.Message.Reply, Encoding.UTF8.GetBytes($"CLIENT-ID: {clientId}\t ACK: 'for {data}'"));
                       }
                   });


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
