using System.Threading.Tasks;
using System.Transactions;
using Npgsql;
using NServiceBus.Extensibility;
using NServiceBus.Outbox;
using NServiceBus.Persistence;
using NServiceBus.Transport;

namespace NServiceBusPostgrePersistence
{
    public class StorageAdapter : ISynchronizedStorageAdapter
    {
        readonly string _connectionString;

        public StorageAdapter(string connectionString)
        {
            _connectionString = connectionString;
        }

        static readonly Task<CompletableSynchronizedStorageSession> EmptyResult = Task.FromResult((CompletableSynchronizedStorageSession)null);

        public Task<CompletableSynchronizedStorageSession> TryAdapt(OutboxTransaction transaction, ContextBag context)
        {
            //var outboxTransaction = transaction as SqlOutboxTransaction;
            //if (outboxTransaction != null)
            //{
            //    CompletableSynchronizedStorageSession session = new StorageSession(outboxTransaction.SqlConnection, outboxTransaction.SqlTransaction, false);
            //    return Task.FromResult(session);
            //}
            return EmptyResult;
        }

        public async Task<CompletableSynchronizedStorageSession> TryAdapt(TransportTransaction transportTransaction, ContextBag context)
        {
            Transaction ambientTransaction;
            if (transportTransaction.TryGet(out ambientTransaction))
            {
                NpgsqlConnection existingSqlConnection;
                //SQL server transport in ambient TX mode
                if (transportTransaction.TryGet(out existingSqlConnection))
                {
                    return new StorageSession(existingSqlConnection, null);
                }
                //Other transport in ambient TX mode
                var connection = await NpgSqlHelpers.NewConnection(_connectionString);
                return new StorageSession(connection, connection.BeginTransaction());
            }
            return null;
        }
    }
}