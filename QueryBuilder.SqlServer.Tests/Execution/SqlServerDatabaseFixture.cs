using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SqlKata.Tests;
using SqlKata.Tests.Execution;

namespace QueryBuilder.SqlServer.Tests.Execution
{
    public class SqlServerDatabaseFixture : IExecutionDatabaseFixture
    {
        private readonly HashSet<WeakReference<SqlConnection>> _tracked = new HashSet<WeakReference<SqlConnection>>();

        private static SqlConnection GetConnection()
        {
            var connectionString =
                QueryBuilderTestSettings.ConnectionStrings.Single(s => s.Name == "sqlserver").ConnectionString;
            return new SqlConnection(connectionString);
        }

        public SqlServerDatabaseFixture()
        {
            Connection = GetConnection();
            TestConnection();
        }

        private void TestConnection()
        {
            Connection.ExecuteScalar("SELECT 1");
        }

        public IDbConnection Connection { get; private set; }

        public IDbConnection IsolatedConnection()
        {
            var connection = GetConnection();
            _tracked.Add(new WeakReference<SqlConnection>(connection));
            return connection;
        }

        public void Dispose()
        {
            Connection.Dispose();
            foreach (var weakReference in _tracked)
            {
                try
                {
                    if (weakReference.TryGetTarget(out SqlConnection cnn))
                    {
                        cnn.Dispose();
                    }
                }
                catch
                {

                }
            }
        }
    }
}