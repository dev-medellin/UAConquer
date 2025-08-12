using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using MYSQLCONNECTION = MySql.Data.MySqlClient.MySqlConnection;

namespace TheChosenProject.Database
{
    public class BackupService
    {

    }
    public unsafe static class DataHolder
    {
        public static string ConnectionString;
        private static string MySqlUsername, MySqlPassword, MySqlDatabase, MySqlHost;
        public static void CreateConnection()
        {
            ConnectionString = DatabaseConfig.ConnectionStringOpt;
        }

        public static MYSQLCONNECTION MySqlConnection
        {
            get
            {
                MYSQLCONNECTION conn = new MYSQLCONNECTION();
                conn.ConnectionString = ConnectionString;
                return conn;
            }
        }
    }
}