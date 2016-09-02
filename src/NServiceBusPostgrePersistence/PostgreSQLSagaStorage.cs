using Npgsql.Logging;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Sagas;

namespace NServiceBusPostgrePersistence
{
    public class PostgreSQLSagaStorage : Feature
    {
        public PostgreSQLSagaStorage()
        {
            DependsOn<Sagas>();
        }

        protected override async void Setup(FeatureConfigurationContext context)
        {
            NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug);

            var connectionString = "host=localhost;database=nsb.pgpersister;username=postgres;password=admin;";

            var synchronizedStorage = new SynchronizedStorage(connectionString);
            context.Container.ConfigureComponent(b => synchronizedStorage, DependencyLifecycle.SingleInstance);
            context.Container.ConfigureComponent(b => new StorageAdapter(connectionString), DependencyLifecycle.SingleInstance);

            var sagaPersister = new SagaPersister();
            context.Container.ConfigureComponent<ISagaPersister>(() => sagaPersister, DependencyLifecycle.SingleInstance);

            var storage = await synchronizedStorage.OpenSession(null);
            SagaPersister.Initialize(storage);
            await storage.CompleteAsync();
        }
    }
}