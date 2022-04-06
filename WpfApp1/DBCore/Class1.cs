using Microsoft.Data.Sqlite;
using System;
using System.Security.Cryptography;
using System.Text;



namespace WpfApp1.DBcore
{
    public class DBreader : IDBDriver
    {
        private string db_name = "data.db";
        public bool IsCreate { 
            get 
            {
                return System.IO.File.Exists(db_name);
            }
        }
        string connectStr;

        public DBreader()
        {
            connectStr = "Data Source=" + db_name + ";";
        }

        public void Create()
        {
            string createString = connectStr + "Mode=ReadWriteCreate;";
            string createTableProfile = @"CREATE TABLE profile(
											name TEXT NOT NULL PRIMARY KEY NOT NULL UNIQUE,
											passw_hash TEXT NOT NULL);";
            string createTableGames = @"CREATE TABLE game(
											id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
											name TEXT NOT NULL,
											release_date TEXT);";
            string createTableGameIfo = @"CREATE TABLE gameinfo(
											game_id INTEGER,
											profile_id INTEGER,
											executable_file TEXT,
											save_file TEXT, 
											time_in_game INTEGER NOT NULL,
											FOREIGN KEY (game_id) REFERENCES profile(id)
											FOREIGN KEY (profile_id) REFERENCES games(id))";
            //PRIMARY KEY(game_id, profile_id),

            using (var connection = new SqliteConnection(createString))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = createTableProfile + createTableGames + createTableGameIfo
                };
                command.ExecuteNonQuery();
            }
        }


        public bool LogIn(string nick, string passw)
        {
            if (nick == "") 
                return false;

            string expression = $"SELECT name, passw_hash FROM profile WHERE name=@name";
            string? store_hash = "";

            using (SqliteConnection connection = new (connectStr))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                SqliteParameter paramNick = new("@name", nick);
                command.Parameters.Add(paramNick);

                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    store_hash = Convert.ToString(reader["passw_hash"]);
                }
            }

            byte[] utf8 = Encoding.UTF8.GetBytes(passw);
            string given_hash = Convert.ToBase64String(utf8);

            return given_hash == store_hash;
        }

        public bool SignIn(string nick, string passw)
        {
            if (Check_profile_by_name(nick) || nick == "")
                return false;

            string sign_in_mode = connectStr + "Mode=ReadWrite;";
            string sign_in_expression = $"INSERT INTO profile (name, passw_hash) VALUES (@name, @passw_hash)";

            var hash_finder = SHA1.Create();
            byte[] utf8 = Encoding.UTF8.GetBytes(passw);

            string hash_code = passw == "" ? "" : Convert.ToBase64String(utf8);


            using (var connection = new SqliteConnection(sign_in_mode))
            {
                connection.Open();

                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = sign_in_expression
                };

                SqliteParameter paramNick = new("@name", nick);
                SqliteParameter paramHash = new("@passw_hash", hash_code);
                command.Parameters.Add(paramNick);
                command.Parameters.Add(paramHash);

                command.ExecuteNonQuery();
            }
            return true;
        }
        private bool Check_profile_by_name(string nick)
        {
            string expression = $"SELECT COUNT(name) FROM profile WHERE name=@name";
            object? profile_exist;

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                SqliteParameter paramNick = new("@name", nick);
                command.Parameters.Add(paramNick);

                profile_exist = command.ExecuteScalar();
            }

            if (Convert.ToInt32(profile_exist) == 0)
                return false;
            return true;
        }
        public void SignOut() { }
    }
}