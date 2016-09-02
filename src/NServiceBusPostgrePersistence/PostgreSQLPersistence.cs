using NServiceBus.Features;
using NServiceBus.Persistence;

namespace NServiceBusPostgrePersistence
{
    public class PostgreSQLPersistence : PersistenceDefinition
    {
        public PostgreSQLPersistence()
        {
            Defaults(s =>
            {
                //s.EnableFeatureByDefault<StorageSessionFeature>();
            });

            Supports<StorageType.Sagas>(s => s.EnableFeatureByDefault<PostgreSQLSagaStorage>());
        }
    }
}
