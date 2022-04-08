using Microsoft.Data.Sqlite;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;



namespace WpfApp1.DBcore
{
    public struct Profile
    {
        public const string _profiles = "profiles";
        public const string _id = "id";
        public const string _name = "name";
        public const string _passw_hash = "passw_hash";
    }
    public struct Game
    {
        public const string _games = "games";
        public const string _game_publishers = "game_publishers";
        public const string _title = "title";
        public const string _id = "id";
        public const string _relese_date = "release_date";
        public string? Name { get; set; }
        public string? Relese_date { get; set; }
        public int? ID { get; set; }
        public List<Publisher>? Publishers { get; set; }
    }
    public struct Publisher
    {
        public const string _publisher = "publishers";
        public const string _id = "id";
        public const string _name = "name";
        public int? ID { get; set; }
        public string? Name { get; set; }
    }
    public struct GamePublisher
    {
        public const string _game_publisher = "game_publisher";
        public const string _publisher_id = "publisher_id";
        public const string _game_id = "game_id";
        public const string _country = "country";

    }
    public struct GameInfo
    {
        public const string _game_infos = "game_infos";
        public static string _game_id { get => GamePublisher._game_id; }
        public const string _profile_id = "profile_id";
        public const string _executable_file = "executable_file";
        public const string _save_file = "save_file";
        public const string _time_in_game = "time_in_game";

        public List<Game>? MyGames { get; set; }
        public int? game_id { get; set; }
        public int? profile_id { get; set; }
        public string? executable_file { get; set; }
        public string? save_file { get; set; }
        public int? time_in_game { get; set; }
    }


    public static class DBreader
    {
        private const string _table_games = "games";
        private static readonly string db_name = "data.db";
        private static readonly string connectStr;
        private static string connectedNick = "";


        public static bool IsCreate
        {
            get
            {
                return System.IO.File.Exists(db_name);
            }
        }

        static DBreader()
        {
            connectStr = $"Data Source={db_name};";
        }

        static public void Create()
        {
            string createMode = connectStr + "Mode=ReadWriteCreate;Foreign Keys=true;";

            string createTableProfile = @$"CREATE TABLE {Profile._profiles}(
                                            {Profile._id} INTEGER PRIMARY KEY AUTOINCREMENT,
											{Profile._name} TEXT NOT NULL UNIQUE,
											{Profile._passw_hash} TEXT NOT NULL);";

            string createTableGames = @$"CREATE TABLE {Game._games}(
											{Game._id} INTEGER PRIMARY KEY AUTOINCREMENT, 
											{Game._title} TEXT NOT NULL,
											{Game._relese_date} TEXT);";

            string createTableGameIfo = @$"CREATE TABLE {GameInfo._game_infos}(
											{GameInfo._game_id} INTEGER,
											{GameInfo._profile_id} INTEGER,
											{GameInfo._executable_file} TEXT,
											{GameInfo._save_file} TEXT, 
											{GameInfo._time_in_game} INTEGER NOT NULL,
											
                                            FOREIGN KEY ({GameInfo._game_id}) 
                                            REFERENCES {Profile._profiles}({Profile._id})
											
                                            FOREIGN KEY ({GameInfo._profile_id}) 
                                            REFERENCES {Game._games}({Game._id}));";

            string createTablePublishers = @$"CREATE TABLE {Publisher._publisher}(
											{Publisher._id} INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            {Publisher._name} TEXT UNIQUE);";

            string createTableGamePublishers = @$"CREATE TABLE {Game._game_publishers}(
											{GamePublisher._game_id}    INTEGER NOT NULL , 
                                            {GamePublisher._publisher_id} INTEGER NOT NULL,
                                            {GamePublisher._country} TEXT,

                                            FOREIGN KEY {GamePublisher._game_id}    
                                            REFERENCES {Game._games}({Game._id})

                                            FOREIGN KEY {GamePublisher._publisher_id} 
                                            REFERENCES {Publisher._publisher}({Publisher._id}));";

            using (SqliteConnection connection = new(createMode))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = createTableProfile +
                        createTableGames +
                        createTableGameIfo +
                        createTablePublishers +
                        createTableGamePublishers
                };
                command.ExecuteNonQuery();
            }
        }

        #region Profile
        static public bool LogIn(string nick, string passw)
        {
            if (nick == "")
                return false;

            string expression = $"SELECT name, passw_hash FROM profiles WHERE name=@name";
            string? store_hash = "";

            using (SqliteConnection connection = new(connectStr))
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

            if (given_hash == store_hash)
                connectedNick = nick;

            return given_hash == store_hash;
        }
        static public bool SignIn(string nick, string passw)
        {
            if (Check_profile_by_name(nick) || nick == "")
                throw new Exception($"This nick [{nick}] is already used");

            string sign_in_mode = connectStr + "Mode=ReadWrite;";
            string sign_in_expression = $"INSERT INTO profiles (name, passw_hash) VALUES (@name, @passw_hash)";

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

            connectedNick = nick;
            return true;
        }
        static private bool Check_profile_by_name(string nick)
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
        static public void SignOut() { connectedNick = ""; }
        #endregion

        #region Set Data
        static private bool Check_publisher_by_name(string name)
        {
            string expression = $"SELECT COUNT(name) FROM publishers WHERE name=@name";
            object? profile_exist;

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                SqliteParameter paramNick = new("@name", name);
                command.Parameters.Add(paramNick);

                profile_exist = command.ExecuteScalar();
            }

            if (Convert.ToInt32(profile_exist) == 0)
                return false;
            return true;
        }
        static public bool Add_new_publisher(string name)
        {
            if (Check_publisher_by_name(name))
                throw new Exception($"This name [{name}] is already used");

            string new_publisher_expression = $"INSERT INTO publishers (name) VALUES (@name)";

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();

                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = new_publisher_expression
                };

                SqliteParameter paramNick = new("@name", name);
                command.Parameters.Add(paramNick);

                command.ExecuteNonQuery();
            }
            return true;
        }
        static public bool Add_new_game(string title, string date_MM_DD_GGGG)
        {
            string new_game_expression = $"INSERT INTO games (title, release_date) VALUES (@title, @release_date)";

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();

                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = new_game_expression
                };

                SqliteParameter paramNick = new("@title", title);
                SqliteParameter paramHash = new("@release_date", date_MM_DD_GGGG);
                command.Parameters.Add(paramNick);
                command.Parameters.Add(paramHash);

                command.ExecuteNonQuery();
            }
            return true;
        }
        static public bool Bound_game_publisher(int game_id, int publisher_id)
        {
            string new_game_expression = $"INSERT INTO gamePublishers (game_id, publisher_id) VALUES (@game_id, @publisher_id)";

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();

                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = new_game_expression
                };

                SqliteParameter paramNick = new("@game_id", game_id);
                SqliteParameter paramHash = new("@publisher_id", publisher_id);
                command.Parameters.Add(paramNick);
                command.Parameters.Add(paramHash);

                command.ExecuteNonQuery();
            }
            return true;
        }
        #endregion

        #region Get Data
        static public List<Game> Get_games()
        {
            string expression = $"SELECT id, title FROM games";
            SqliteDataReader gamesReader;
            List<Game> games = new();

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                gamesReader = command.ExecuteReader();

                while (gamesReader.Read())
                {
                    Game gm = new Game()
                    {
                        ID = Convert.ToInt32(gamesReader["id"]),
                        Name = gamesReader["title"].ToString(),
                    };

                    games.Add(gm);
                }
            }

            for (int i = 0; i < games.Count; i++)
            {
                Game gm = games[i];

                List<Publisher> publishers = Get_publishers_by_game_id(games[i].ID.Value);
                gm.Publishers = publishers;

                games[i] = gm;
            }


            return games;
        }
        static public Publisher Get_publisher_by_id(int id)
        {
            string expression = $"SELECT * " +
                $"FROM {Publisher._publisher} " +
                $"WHERE {Publisher._id} = @id";

            SqliteDataReader gamesReader;
            Publisher publisher = new() { ID = -1 };

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                SqliteParameter param = new("@id", id);
                command.Parameters.Add(param);

                gamesReader = command.ExecuteReader();

                if (gamesReader.Read())
                {
                    publisher = new Publisher()
                    {
                        ID = Convert.ToInt32(gamesReader[Publisher._id]),
                        Name = gamesReader[Publisher._name].ToString(),
                    };
                }
            }

            return publisher;
        }
        static public List<Publisher> Get_publishers_by_game_id(int game_id)
        {
            string expression = $"SELECT {GamePublisher._publisher_id} " +
                $"FROM {GamePublisher._game_publisher} " +
                $"WHERE {GamePublisher._game_id} = @game_id";

            SqliteDataReader gamesReader;
            List<int> publishers_id = new();
            List<Publisher> publishers = new();

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                SqliteParameter param = new("@game_id", game_id);
                command.Parameters.Add(param);

                gamesReader = command.ExecuteReader();

                while (gamesReader.Read())
                {
                    publishers_id.Add(
                        Convert.ToInt32(gamesReader[GamePublisher._publisher_id])
                    );
                }
            }
            foreach (var id in publishers_id)
            {
                var publ = Get_publisher_by_id(id);
                publishers.Add(publ);
            }
            return publishers;
        }
        //static public List<GameInfo> Get_my_games_info(string profile)
        //{
        //    string expression = $"SELECT * FROM games";
        //    SqliteDataReader gamesReader;
        //    List<Game> games = new();

        //    using (var connection = new SqliteConnection(connectStr + "Mode=ReadOnly;"))
        //    {
        //        connection.Open();
        //        SqliteCommand command = new()
        //        {
        //            Connection = connection,
        //            CommandText = expression
        //        };

        //        gamesReader = command.ExecuteReader();

        //        while (gamesReader.Read())
        //        {

        //            games.Add
        //            (
        //                new Game()
        //                {
        //                    ID = Convert.ToInt32(gamesReader["id"]),
        //                    Name = gamesReader["title"].ToString(),
        //                }
        //            );
        //        }
        //    }

        //    return games;
        //}

        #endregion
    }
}