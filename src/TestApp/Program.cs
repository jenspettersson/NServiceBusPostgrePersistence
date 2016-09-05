using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Persistence;
using NServiceBusPostgrePersistence;
using TestApp.Orders;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var endpointConfiguration = new EndpointConfiguration("PGPersistence.Dev");
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.UsePersistence<InMemoryPersistence, StorageType.Subscriptions>();
            endpointConfiguration.UsePersistence<InMemoryPersistence, StorageType.Timeouts>();

            endpointConfiguration.UsePersistence<PostgreSQLPersistence, StorageType.Sagas>();

            endpointConfiguration.SendFailedMessagesTo("error");

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Running...");

            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Press 1 to start new order and 2 to complete order:");
                var line = Console.ReadLine();

                switch (line)
                {
                    case "x":
                        break;
                    case "1":
                        {
                            Console.WriteLine("Write id to start order:");
                            var orderId = Console.ReadLine();
                            await endpointInstance.SendLocal(new StartOrder { OrderId = orderId });

                            Console.WriteLine($"Started order {orderId}");
                        }
                        continue;
                    case "2":
                        {
                            Console.WriteLine("Write id to complete order:");
                            var orderId = Console.ReadLine();
                            await endpointInstance.SendLocal(new CompleteOrder { OrderId = orderId });

                            Console.WriteLine($"Sent Complete Order message for: {orderId}");
                        }
                        continue;
                }
                if (line == "x")
                    break;
            }

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
