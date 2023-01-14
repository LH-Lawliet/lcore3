using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Npgsql;

namespace lcore3.Server
{
    public class Connection : BaseScript
    {
        public struct User
        {
            public string playerName;
            public int id;
            public string steam;
            public string license;
            public string xbl;
            public string live;
            public string discord;
            public string licence2;
            public string ip;
            public DateTime banned;
            public string ban_reason;
            public DateTime last_connection;
            public Int32 admin_level;

            public User(
                string cplayerName,
                int cid,
                string csteam,
                string clicense,
                string clicence2,
                string cxbl,
                string clive,
                string cdiscord,
                string cip,
                DateTime cbanned,
                string cban_reason,
                DateTime clast_connection,
                Int32 cadmin_level
            )
            {
                playerName = cplayerName;
                id = cid;
                steam = csteam;
                license = clicense;
                licence2 = clicence2;
                xbl = cxbl;   
                live = clive;     
                discord = cdiscord;  
                ip = cip;
                banned = cbanned;  
                ban_reason = cban_reason;   
                last_connection = clast_connection;
                admin_level = cadmin_level;
            }


            public override string ToString() => $"User <{playerName}> : (" +
                $"\n   id={id}," +
                $"\n   steam={steam}," +
                $"\n   license={license}," +
                $"\n   licence2={licence2}," +
                $"\n   xbl={xbl}," +
                $"\n   live={live}," +
                $"\n   discord={discord}," +
                $"\n   ip={ip}," +
                $"\n   banned={banned}," +
                $"\n   ban_reason={ban_reason}," +
                $"\n   last_connection={last_connection}," +
                $"\n   admin_level={admin_level}\n"
                +")";
        }



        public Connection()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
        }

        async private void OnPlayerConnecting([FromSource]Player player, string playerName, dynamic kickCallback, dynamic deferrals)
        {
            deferrals.defer();

            // mandatory wait!
            await Delay(0);

            deferrals.update("Vérifie les informations du joueur...");

            User? user = await GetUserInDB(player);

            if (user == null)
            {
                Debug.WriteLine("A new player is trying to connect to the server");
                user = await CreateUser(player);
                if (user == null)
                {
                    deferrals.update("Création d'un nouvelle utilisateur...");
                } else {
                    deferrals.kick("Erreur:Impossible de créer l'utilisateur");
                    return;
                }
                    
            } else {
                Debug.WriteLine($"Retour de : {user}");
                deferrals.update("Utilisateur trouvé, bienvenu");
            }
            await Delay(1000);
            deferrals.done();
        }

        private string StringOrBlank(object s)
        {
            if (s == null)
            {
                return "";
            }
            return s.ToString();
        }

        private DateTime DateOrZero(object d)
        {           
            if (d.GetType().ToString() == "System.DBNull")
            {
                return new DateTime(0);
            }
            return (DateTime) d;
        }

        private async Task<User?> GetUserInDB(Player player)
        {
            using (NpgsqlCommand command = ServerMain.db.CreateCommand())
            {
                command.CommandText = "SELECT * FROM users WHERE steam=@steam OR license=@license OR license2=@license2 OR xbl=@xbl OR live=@live OR discord=@discord";
                command.Parameters.AddWithValue("steam", player.Identifiers["steam"]);
                command.Parameters.AddWithValue("license", player.Identifiers["license"]);
                command.Parameters.AddWithValue("license2", player.Identifiers["license2"]);
                command.Parameters.AddWithValue("xbl", player.Identifiers["xbl"]);
                command.Parameters.AddWithValue("live", player.Identifiers["live"]);
                command.Parameters.AddWithValue("discord", player.Identifiers["discord"]);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return new User(
                            player.Name,
                            (int)reader["id"],
                            player.Identifiers["steam"] != null ? player.Identifiers["steam"] : StringOrBlank(reader["steam"]),
                            player.Identifiers["license"] != null ? player.Identifiers["license"] : StringOrBlank(reader["license"]),
                            player.Identifiers["license2"] != null ? player.Identifiers["license2"] : StringOrBlank(reader["license2"]),
                            player.Identifiers["xbl"] != null ? player.Identifiers["xbl"] : StringOrBlank(reader["xbl"]),
                            player.Identifiers["live"] != null ? player.Identifiers["live"] : StringOrBlank(reader["live"]),
                            player.Identifiers["discord"] != null ? player.Identifiers["discord"] : StringOrBlank(reader["discord"]),
                            player.Identifiers["ip"],
                            DateOrZero(reader["banned"]),
                            StringOrBlank(reader["ban_reason"]),
                            DateOrZero(reader["last_connection"]),
                            (Int32)reader["admin_level"]
                        );
                    }
                }
            }
            return null;
        }

        private async Task<User?> CreateUser(Player player)
        {
            using (NpgsqlCommand command = ServerMain.db.CreateCommand())
            {
                command.CommandText = "INSERT INTO users (steam, license, license2, xbl, live, discord, ip) VALUES (@steam,@license,@license2,@xbl,@live,@discord,@ip)";
                command.Parameters.AddWithValue("steam", player.Identifiers["steam"]);
                command.Parameters.AddWithValue("license", player.Identifiers["license"]);
                command.Parameters.AddWithValue("license2", player.Identifiers["license2"]);
                command.Parameters.AddWithValue("xbl", player.Identifiers["xbl"]);
                command.Parameters.AddWithValue("live", player.Identifiers["live"]);
                command.Parameters.AddWithValue("discord", player.Identifiers["discord"]);
                command.Parameters.AddWithValue("ip", player.Identifiers["ip"]);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    return await GetUserInDB(player);
                } 
                else
                {
                    Debug.WriteLine("^3Error:Impossible to create user");
                    return null;
                }
            }
        }
    }
}