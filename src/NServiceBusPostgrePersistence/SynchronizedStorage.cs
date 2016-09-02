using System.Threading.Tasks;
using Npgsql;
using NServiceBus.Extensibility;
using NServiceBus.Persistence;

namespace NServiceBusPostgrePersistence
{
    public class SynchronizedStorage : ISynchronizedStorage
    {
        private readonly string _connectionString;

        public SynchronizedStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CompletableSynchronizedStorageSession> OpenSession(ContextBag contextBag)
        {
            var connection = await NpgSqlHelpers.NewConnection(_connectionString);

            var transaction = connection.BeginTransaction();
            return new StorageSession(connection, transaction);
        }
    }

    public class NpgSqlHelpers
    {
        public static async Task<NpgsqlConnection> NewConnection(string connectionString)
        {
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}