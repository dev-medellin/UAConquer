using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace TheChosenProject
{
    public class DatabaseConfig
    {
        // Connection string property
        public static string ConnectionString { get; private set; }
        public static string ConnectionStringOpt { get; private set; }
        public static string DatabaseLoc { get; private set; }
        public static string db_name { get; private set; }
        public static string db_password { get; private set; }
        public static string db_username { get; private set; }

        public static bool discord_stat { get; private set; }

        // Environment type
        public static string EnvironmentType => "Development";

        // Initialize connection string
        static DatabaseConfig()
        {
            SetConnectionString();
        }

        // Method to switch based on environment
        private static void SetConnectionString()
        {
            if (EnvironmentType.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                // Local/Dev connection
                ConnectionString = "Server=localhost;username=root;password=P@ssw0rd;database=bd;";
                ConnectionStringOpt = "Server=localhost;Port=3306;Database=bd;Uid=root;Password=P@ssw0rd;Persist Security Info=True;Pooling=true; Min Pool Size = 32;  Max Pool Size = 300;";
                DatabaseLoc = "d:\\Users\\Toytoks\\Desktop\\UAConquer\\Database";
                db_name = "localhost";
                db_password = "P@ssw0rd";
                db_username = "root";
                discord_stat = false;
            } else if(EnvironmentType.Equals("Production_syntax", StringComparison.OrdinalIgnoreCase))
            {
                // Production connection
                ConnectionString = "Server=localhost;username=root;password=aa123456;database=u804293080_bdconquer;";
                ConnectionStringOpt = "Server=localhost;Port=3306;Database=u804293080_bdconquer;Uid=root;Password=aa123456;Persist Security Info=True;Pooling=true; Min Pool Size = 32;  Max Pool Size = 300;";
                DatabaseLoc = "C:\\Users\\Administrator\\Desktop\\Database";
                db_name = "u804293080_bdconquer";
                db_password = "aa123456";
                db_username = "root";
                discord_stat = false;
            }
            else
            {

                ConnectionString = "Server=194.59.164.68;username=u804293080_bdroot;password=Arthur@2025!;database=u804293080_bdconquer;";
                ConnectionStringOpt = "Server=194.59.164.68;Port=3306;Database=u804293080_bdconquer;Uid=u804293080_bdroot;Password=Arthur@2025!;Persist Security Info=True;Pooling=true; Min Pool Size = 32;  Max Pool Size = 300;";
                DatabaseLoc = "C:\\Users\\Administrator\\Desktop\\Database";
                db_name = "u804293080_bdconquer";
                db_password = "Arthur@2025!";
                db_username = "u804293080_bdroot";
                discord_stat = true;
            }
        }
    }
}
