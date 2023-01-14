using System;
using CitizenFX.Core;
using Npgsql;
using static CitizenFX.Core.Native.API;

namespace lcore3.Server
{
    public class ServerMain : BaseScript
    {
        static public DatabaseHandler db;
        public ServerMain()
        {
            Debug.WriteLine("Hi from lcore3.Server!");
            initDatabase();
        }

        [Command("hello_server")]
        public void HelloServer()
        {
            Debug.WriteLine("Sure, hello.");
        }

        private void initDatabase()
        {
            db = new DatabaseHandler();

            using (NpgsqlCommand command = db.CreateCommand())
            {
                command.CommandText = "SELECT version FROM ServerData";
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"lcore3 version is {0} while database version is : {reader["version"]}");
                    }
                }
            }
        }
    }
}