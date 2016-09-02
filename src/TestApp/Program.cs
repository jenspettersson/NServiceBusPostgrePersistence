using System;
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
                Console.WriteLine("Write id to start order");
                var line = Console.ReadLine();

                if (line == "x")
                    break;

                await endpointInstance.SendLocal(new StartOrder {OrderId = line});

                Console.WriteLine($"Started order {line}");
            }
            

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
