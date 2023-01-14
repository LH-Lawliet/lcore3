using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Npgsql;
using static CitizenFX.Core.Native.API;

namespace lcore3.Server
{
    public class DatabaseHandler : BaseScript
    {
        private NpgsqlConnection _connection;
        string user;
        string host;
        string database;
        string password;
        string port;
        string connectionString;

        public DatabaseHandler()
        {
            user = GetConvar("dbUSER", "defaultUser");
            host = GetConvar("dbIP", "defaultIP");
            database = GetConvar("dbDB", "defaultDB");
            password = GetConvar("dbPASSWORD", "defaultPASSWORD");
            port = GetConvar("dbPORT", "defaultPORT");
            connectionString = $"Server={host};Port={port};Database={database};User Id={user};Password={password};";
            initDatabase();

            EventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
        }

        private void initDatabase()
        {
            _connection = new NpgsqlConnection(connectionString);
            Console.WriteLine("Database connection openning ...");
            _connection.Open();
        }

        private void OnResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() == resourceName)
            {
                // Appeler la méthode ici
                Console.WriteLine("Database connection closing !!!");
                _connection.Close();
            }
        }


        public NpgsqlCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }
    }
}