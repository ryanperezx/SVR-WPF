using System;
using System.IO;
using System.Data.SqlServerCe;
namespace SVR_WPF
{
    class DBUtils
    {
        public static SqlCeConnection GetDBConnection()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string datasource = folder + "\\Student Violation Records\\StudentViolationRecords.sdf";
            return DBSQLServerUtils.GetDBConnection(datasource);
        }
    }
}
