using System;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace SVR_WPF
{
    class DBSQLServerUtils
    {
        public static SqlCeConnection
         GetDBConnection(string datasource)
        {
            //Data Source=ADMINRG-S0R6T5U\SQLEXPRESS;Initial Catalog=studentDB;Integrated Security=True
            string connString = @"Data Source=" + datasource;
            SqlCeConnection conn = new SqlCeConnection(connString);

            return conn;
        }
    }
}
