using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Features;
using NServiceBus.Persistence;
using NServiceBus.Sagas;

namespace NServiceBusPostgrePersistence
{
    public class PostgreSQLPersistence : PersistenceDefinition
    {
        public PostgreSQLPersistence()
        {
            Supports<StorageType.Sagas>(s => s.EnableFeatureByDefault<PostgreSQLSagaStorage>());
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
            throw new NotImplementedException();
        }
    }

    public class SagaPersister : ISagaPersister
    {
        public Task Save(IContainSagaData sagaData, SagaCorrelationProperty correlationProperty, SynchronizedStorageSession session,
            ContextBag context)
        {
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
