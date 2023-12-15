using ProgLeasing.System.Data.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Data.Configuration
{
    /// <summary>
    /// Factory to generate an <see cref="IDbConnection"/> based on the selected database.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly Dictionary<string, DatabaseInfo> _databaseInfos;

        public DbConnectionFactory(Dictionary<string, DatabaseInfo> databaseInfos)
        {
            _databaseInfos = databaseInfos;
        }

        /// <summary>
        /// Create a new <see cref="IDbConnection"/> for the specified databaseName that was already configured.
        /// </summary>
        /// <param name="databaseName">Key for the database to connect to</param>
        /// <returns>Instance of an <see cref="IDbConnection"/></returns>
        public IDbConnection CreateConnection(string databaseName)
        {
            var dbInfo = _databaseInfos[databaseName];
            if (dbInfo != null)
            {
                switch (dbInfo.DatabaseProvider)
                {
                    case DatabaseProvider.SqlServer:
                        return new SqlConnection(dbInfo.ConnectionString);

                }
            }

            throw new ArgumentException($"No connection string for {databaseName} is registered");
        }

    }
}
