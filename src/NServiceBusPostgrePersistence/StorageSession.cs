using System.Threading.Tasks;
using Npgsql;
using NServiceBus.Persistence;

namespace NServiceBusPostgrePersistence
{
    public class StorageSession : CompletableSynchronizedStorageSession, IPostgreSQLSession
    {
        public NpgsqlConnection Connection { get; }
        public NpgsqlTransaction Transaction { get; }

        public StorageSession(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection.Dispose();
        }

        public Task CompleteAsync()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
            }

            Connection.Dispose();

            return Task.FromResult(0);
        }
    }

    public interface IPostgreSQLSession
    {
        NpgsqlConnection Connection { get; }
        NpgsqlTransaction Transaction { get; }
    }
}