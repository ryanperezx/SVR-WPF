using System;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
namespace SVR_WPF
{
    class DBUtils
    {
        public static SqlCeConnection GetDBConnection()
        {
            string datasource = "StudentViolationRecords.sdf";
            return DBSQLServerUtils.GetDBConnection(datasource);
        }
    }
}
