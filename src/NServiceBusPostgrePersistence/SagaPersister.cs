using System;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Persistence;
using NServiceBus.Sagas;

namespace NServiceBusPostgrePersistence
{
    public class SagaPersister : ISagaPersister
    {
        public async Task Save(IContainSagaData sagaData, SagaCorrelationProperty correlationProperty, SynchronizedStorageSession session,
            ContextBag context)
        {
            string sql = @"
INSERT INTO sagas (
    id, 
    originator, 
    originalmessageid, 
    data
) 
VALUES (
    @Id, 
    @Originator, 
    @OriginalMessageId, 
    @Data
)
";
            var storageSession = session.PostgreSqlSession();
            
            using (var command = new NpgsqlCommand(sql, storageSession.Connection, storageSession.Transaction))
            {
                command.Parameters.AddWithValue("Id", sagaData.Id);
                command.Parameters.AddWithValue("Originator", sagaData.Originator);
                command.Parameters.AddWithValue("OriginalMessageId", sagaData.OriginalMessageId);

                var json = JsonConvert.SerializeObject(sagaData);
                command.Parameters.AddWithValue("Data", NpgsqlDbType.Jsonb, json);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(IContainSagaData sagaData, SynchronizedStorageSession session, ContextBag context)
        {
            string sql = @"
UPDATE sagas SET
    originator = @Originator, 
    originalmessageid = @OriginalMessageId, 
    data = @Data
WHERE
    id = @Id
";
            var storageSession = session.PostgreSqlSession();

            using (var command = new NpgsqlCommand(sql, storageSession.Connection, storageSession.Transaction))
            {
                command.Parameters.AddWithValue("Id", sagaData.Id);
                command.Parameters.AddWithValue("Originator", sagaData.Originator);
                command.Parameters.AddWithValue("OriginalMessageId", sagaData.OriginalMessageId);

                var json = JsonConvert.SerializeObject(sagaData);
                command.Parameters.AddWithValue("Data", NpgsqlDbType.Jsonb, json);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<TSagaData> Get<TSagaData>(Guid sagaId, SynchronizedStorageSession session, ContextBag context) where TSagaData : IContainSagaData
        {
            var sql = @"
SELECT 
    data
FROM 
    sagas
WHERE 
    id = @Id
            ";

            var storageSession = session.PostgreSqlSession();

            using (var command = new NpgsqlCommand(sql, storageSession.Connection, storageSession.Transaction))
            {
                command.Parameters.AddWithValue("Id", sagaId);

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                {
                    if (!await reader.ReadAsync())
                    {
                        return default(TSagaData);
                    }

                    var json = reader.GetString(0);

                    TSagaData sagaData = JsonConvert.DeserializeObject<TSagaData>(json);
                    return sagaData;
                }
            }
        }

        public async Task<TSagaData> Get<TSagaData>(string propertyName, object propertyValue, SynchronizedStorageSession session, ContextBag context) where TSagaData : IContainSagaData
        {
            //TODO: INDEX
            string select = "SELECT id, originator, originalmessageid, data FROM sagas WHERE data ->> @propertyName = @propertyValue";

            var storageSession = session.PostgreSqlSession();
            using (var command = new NpgsqlCommand(select, storageSession.Connection, storageSession.Transaction))
            {
                command.Parameters.AddWithValue("propertyName", propertyName);
                command.Parameters.AddWithValue("propertyValue", propertyValue);

                using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
                {
                    if (!await reader.ReadAsync())
                    {
                        return default(TSagaData);
                    }

                    var json = reader.GetString(3);

                    TSagaData sagaData = JsonConvert.DeserializeObject<TSagaData>(json);
                    return sagaData;
                }
            }
        }

        public async Task Complete(IContainSagaData sagaData, SynchronizedStorageSession session, ContextBag context)
        {
            string sql = @"
DELETE FROM 
    sagas
WHERE
    id = @Id
";
            var storageSession = session.PostgreSqlSession();

            using (var command = new NpgsqlCommand(sql, storageSession.Connection, storageSession.Transaction))
            {
                command.Parameters.AddWithValue("Id", sagaData.Id);
                
                await command.ExecuteNonQueryAsync();
            }
        }

        //Todo: Move to another feature. Will keep it here for now
        public static void Initialize(SynchronizedStorageSession session)
        {
            var storageSession = session.PostgreSqlSession();

            //Todo: Extend the SQL
            string createTableSql = @"CREATE TABLE  IF NOT EXISTS sagas (
	id UUID,
    originator TEXT,
	originalmessageid TEXT,
	data JSONB,
	PRIMARY KEY(id)
)";
            using (var command = new NpgsqlCommand(createTableSql, storageSession.Connection, storageSession.Transaction))
            {
                command.ExecuteNonQuery();
            }         
        }
    }
}