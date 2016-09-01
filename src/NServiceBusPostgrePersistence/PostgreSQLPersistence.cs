using System;
using System.Threading.Tasks;
using Npgsql;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Features;
using NServiceBus.Persistence;
using NServiceBus.Sagas;

namespace NServiceBusPostgrePersistence
{
    public class StorageSession : CompletableSynchronizedStorageSession
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction _transaction;

        public StorageSession(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }

        public Task CompleteAsync()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
            }

            _connection.Dispose();

            return Task.FromResult(0);
        }
    }

    public class SynchronizedStorage : ISynchronizedStorage
    {
        private readonly string _connectionString;

        public SynchronizedStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CompletableSynchronizedStorageSession> OpenSession(ContextBag contextBag)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var transaction = connection.BeginTransaction();
            return new StorageSession(connection, transaction);
        }
    }

    public class PostgreSQLPersistence : PersistenceDefinition
    {
        public PostgreSQLPersistence()
        {
            Defaults(s =>
            {
                s.EnableFeatureByDefault<StorageSessionFeature>();
            });

            Supports<StorageType.Sagas>(s => s.EnableFeatureByDefault<PostgreSQLSagaStorage>());
        }
    }

    public class StorageSessionFeature : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            var connectionString = "todo";
            context.Container.ConfigureComponent(b => new SynchronizedStorage(connectionString), DependencyLifecycle.SingleInstance);
        }
    }

    public class PostgreSQLSagaStorage : Feature
    {
        public PostgreSQLSagaStorage()
        {
            DependsOn<Sagas>();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var sagaPersister = new SagaPersister();
            context.Container.ConfigureComponent<ISagaPersister>(() => sagaPersister, DependencyLifecycle.SingleInstance);
        }
    }

    public class SagaPersister : ISagaPersister
    {
        public Task Save(IContainSagaData sagaData, SagaCorrelationProperty correlationProperty, SynchronizedStorageSession session,
            ContextBag context)
        {
            var test = session as StorageSession;

            throw new NotImplementedException();
        }

        public Task Update(IContainSagaData sagaData, SynchronizedStorageSession session, ContextBag context)
        {
            throw new NotImplementedException();
        }

        public Task<TSagaData> Get<TSagaData>(Guid sagaId, SynchronizedStorageSession session, ContextBag context) where TSagaData : IContainSagaData
        {
            throw new NotImplementedException();
        }

        public Task<TSagaData> Get<TSagaData>(string propertyName, object propertyValue, SynchronizedStorageSession session, ContextBag context) where TSagaData : IContainSagaData
        {
            throw new NotImplementedException();
        }

        public Task Complete(IContainSagaData sagaData, SynchronizedStorageSession session, ContextBag context)
        {
            throw new NotImplementedException();
        }
    }
}
