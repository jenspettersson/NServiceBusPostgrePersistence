using System;
using System.Data.SqlClient;
using NServiceBus.Persistence;

namespace NServiceBusPostgrePersistence
{
    public static class SynchronizedStorageSessionExtensions
    {
        public static IPostgreSQLSession PostgreSqlSession(this SynchronizedStorageSession session)
        {
            var storageSession = session as StorageSession;
            if (storageSession == null)
            {
                throw new InvalidOperationException("Session not configured!");
            }

            return storageSession;
        }
    }
}