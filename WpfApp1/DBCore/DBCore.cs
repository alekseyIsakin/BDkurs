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

        public int ID { get; set; }
        public string? Nick { get; set; }
    }
    public struct Game
    {
        public const string _games = "games";
        public const string _game_publishers = "game_publishers";
        public const string _title = "title";
        public const string _id = "id";
        public const string _relese_date = "release_date";
        public const string _descr = "description";
        public static Game EMPTY => new() { ID = -1, Descriptions = "", Publishers = new(), Relese_date = DateTime.Parse("1970/1/1"), Title = "new game" };
        public string Title { get; set; }
        public DateTime Relese_date { get; set; }
        public int ID { get; set; }
        public List<Publisher> Publishers { get; set; }
        public string? Descriptions { get; set; }
        public static bool operator ==(Game p1, Game p2)
        {
            return p1.ID == p2.ID;
        }
        public static bool operator !=(Game p1, Game p2)
        {
            return p1.ID != p2.ID;
        }
    }
    public struct Publisher
    {
        public const string _publisher = "publishers";
        public const string _id = "id";
        public const string _name = "name";
        public int ID { get; set; }
        public string Name { get; set; }
        public static bool operator ==(Publisher p1, Publisher p2) 
        {
            return p1.ID == p2.ID;
        }
        public static bool operator !=(Publisher p1, Publisher p2)
        {
            return p1.ID != p2.ID;
        }
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
        public const string _status = "status";
        public const string _executable_file = "executable_file";
        public const string _save_file = "save_file";
        public const string _minuts_in_game = "minuts_in_game";
        public static GameInfo EMPTY => new() { game_id = -1, profile_id = -1, save_file = "", executable_file = "", game_title = "", time_in_game = 0 };

        public int game_id { get; set; }
        public string game_title { get; set; }
        public int profile_id { get; set; }
        public string? executable_file { get; set; }
        public string? save_file { get; set; }
        public ulong time_in_game { get; set; }
        public static bool operator ==(GameInfo p1, GameInfo p2)
        {
            return p1.game_id == p2.game_id && p1.profile_id == p2.profile_id;
        }
        public static bool operator !=(GameInfo p1, GameInfo p2)
        {
            return p1.game_id != p2.game_id || p1.profile_id != p2.profile_id;
        }
    }


    public static class DBreader
    {
        public static readonly string db_name = "data.db";
        private static readonly string connectStr;
        private static string connectedNick = "";


        public static bool IsCreate => System.IO.File.Exists(db_name);

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
											{Game._title} TEXT NOT NULL UNIQUE,
											{Game._descr} TEXT,
											{Game._relese_date} TEXT);";

            string createTableGameIfo = @$"CREATE TABLE {GameInfo._game_infos}(
											{GameInfo._game_id} INTEGER,
											{GameInfo._profile_id} INTEGER,
											{GameInfo._executable_file} TEXT,
											{GameInfo._save_file} TEXT, 
											{GameInfo._minuts_in_game} INTEGER NOT NULL,
											
                                            FOREIGN KEY ({GameInfo._profile_id})
                                            REFERENCES {Profile._profiles}({Profile._id}),
											
                                            FOREIGN KEY ({GameInfo._game_id})
                                            REFERENCES {Game._games} ({Game._id}),
                                            PRIMARY KEY ({GameInfo._profile_id}, {GameInfo._game_id}));";

            string createTablePublishers = @$"CREATE TABLE {Publisher._publisher}(
											{Publisher._id} INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            {Publisher._name} TEXT UNIQUE);";

            string createTableGamePublishers = @$"CREATE TABLE {GamePublisher._game_publisher}(
											{GamePublisher._game_id}      INTEGER NOT NULL, 
                                            {GamePublisher._publisher_id} INTEGER NOT NULL,
                                            {GamePublisher._country} TEXT,

                                            FOREIGN KEY ({GamePublisher._game_id})
                                            REFERENCES {Game._games}({Game._id}),

                                            FOREIGN KEY ({GamePublisher._publisher_id})
                                            REFERENCES {Publisher._publisher}({Publisher._id}),
                                            PRIMARY KEY ({GamePublisher._game_id}, {GamePublisher._publisher_id}));";

            using (SqliteConnection connection = new(createMode))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = createTableProfile +
                        createTableGames
                        + createTableGameIfo
                        + createTablePublishers
                        + createTableGamePublishers
                };
                command.ExecuteNonQuery();
            }
        }

        #region Profile
        static public bool LogIn(string nick, string passw)
        {
            if (nick == "")
                return false;

            string expression = $"SELECT {Profile._name}, {Profile._passw_hash} FROM {Profile._profiles} " +
                $"WHERE {Profile._name}=@name";
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
                return false;
                //throw new Exception($"This nick [{nick}] is already used");

            string sign_in_mode = connectStr + "Mode=ReadWrite;";
            string sign_in_expression = $"INSERT INTO {Profile._profiles} ({Profile._name}, {Profile._passw_hash}) " +
                $"VALUES (@name, @passw_hash)";

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
            string expression = $"SELECT COUNT(*) FROM {Profile._profiles} WHERE {Profile._name}=@name";
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
        static public Profile Get_profile(string nick) 
        {
            string expression = $"SELECT {Profile._id} FROM {Profile._profiles} " +
                $"WHERE {Profile._name}=@name";
            SqliteDataReader profileReader;
            Profile profile = new ();

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                SqliteParameter param = new("@name", nick);
                command.Parameters.Add(param);

                profileReader = command.ExecuteReader();

                while (profileReader.Read()) 
                {
                    profile = new()
                    {
                        ID = Convert.ToInt32(profileReader[Profile._id]),
                        Nick = nick
                    };
                }
            }

            return profile;
        }
#endregion

#region Set Data
        static public bool AddNewPublisher(string name)
        {
            if (Check_publisher_by_name(name))
                throw new Exception($"This name [{name}] is already used");

            string new_publisher_expression = $"INSERT INTO {Publisher._publisher} ({Publisher._name}) VALUES (@name)";

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
        static public int AddNewGame(Game gm) 
        {
            int id = AddNewGame(gm.Title, gm.Relese_date, gm.Descriptions);

            foreach (var publ in gm.Publishers)
                BindGamePublisher(id, publ.ID);

            return id;
        }
        static public int AddNewGame(string title, DateTime date_MM_DD_GGGG, string descr="")
        {
            int last_id = -1;
            string new_game_expression = $"INSERT INTO {Game._games} " +
                $"({Game._title}, {Game._relese_date}, {Game._descr}) " +
                $"VALUES (@title, @release_date, @descr);";

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();

                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = new_game_expression
                };

                SqliteParameter paramNick = new("@title", title);
                SqliteParameter paramHash = new("@release_date", date_MM_DD_GGGG.ToString(@"yyyy/MM/dd"));
                SqliteParameter paramDescr = new("@descr", descr);
                command.Parameters.Add(paramNick);
                command.Parameters.Add(paramHash);
                command.Parameters.Add(paramDescr);

                command.ExecuteNonQuery();
            }

            string last_id_expression = $"SELECT seq FROM sqlite_sequence " +
                $"WHERE name='{Game._games}'";

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();

                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = last_id_expression
                };

                var reader = command.ExecuteReader();

                if (reader.Read()) 
                {
                    last_id = Convert.ToInt32(reader["seq"]);
                }
            }

            return last_id;
        }
        static public bool BindGamePublisher(int game_id, int publisher_id)
        {
            string new_game_expression = $"INSERT INTO {GamePublisher._game_publisher} " +
                $"({GamePublisher._game_id}, {GamePublisher._publisher_id}) " +
                $"VALUES (@game_id, @publisher_id)";

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
        static public void SetupNewGame(GameInfo gi) 
        {
            MyNewGame(gi.game_id, gi.profile_id, gi.time_in_game, gi.executable_file, gi.save_file);
        }
        static public void MyNewGame(int game_id, int profile_id, ulong minuts_in_game = 0, string path_to_exe = "", string save_file = "")
        {

            string new_game_expression = $"INSERT INTO {GameInfo._game_infos} " +
                $"({GameInfo._game_id}, {GameInfo._profile_id}, {GameInfo._minuts_in_game}, {GameInfo._executable_file}, {GameInfo._save_file}) " +
                $"VALUES (@game_id, @profile_id, @minuts_in_game, @executable_file, @save_file)";

            using (var connection = new SqliteConnection(connectStr))
            {
                connection.Open();

                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = new_game_expression
                };

                Stack<SqliteParameter> parameters = new();
                parameters.Push(new("@game_id", game_id));
                parameters.Push(new("@profile_id", profile_id));
                parameters.Push(new("@minuts_in_game", minuts_in_game));
                parameters.Push(new("@executable_file", path_to_exe));
                parameters.Push(new("@save_file", save_file));

                while (parameters.Count > 0)
                    command.Parameters.Add(parameters.Pop());
                command.ExecuteNonQuery();
            } 
        }
        #endregion


        #region Get Data

        #region Game
        static public List<Game> Get_games()
        {
            string expression = $"SELECT * FROM {Game._games}";
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
                    Game gm = new ()
                    {
                        ID = Convert.ToInt32(gamesReader[Game._id]),
                        Title = gamesReader[Game._title].ToString(),
                        Descriptions = gamesReader[Game._descr].ToString(),
                        Relese_date = DateTime.Parse(gamesReader[Game._relese_date].ToString()),
                    };

                    games.Add(gm);
                }
            }

            for (int i = 0; i < games.Count; i++)
            {
                Game gm = games[i];

                List<Publisher> publishers = Get_publishers_by_game_id(games[i].ID);
                gm.Publishers = publishers;

                games[i] = gm;
            }


            return games;
        }
        static public Game Get_Game(int game_id) 
        {
            if (game_id == -1) return Game.EMPTY;

            string expression = $"SELECT * FROM {Game._games} WHERE {Game._id}=@game_id";
            SqliteDataReader gamesReader;
            Game game = new();

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

                if (gamesReader.Read())
                {
                    game = new()
                    {
                        ID = Convert.ToInt32(gamesReader[Game._id]),
                        Title = gamesReader[Game._title].ToString(),
                        Descriptions = gamesReader[Game._descr].ToString(),
                        Relese_date = DateTime.Parse(gamesReader[Game._relese_date].ToString()),
                    };
                }
            }

            List<Publisher> publishers = Get_publishers_by_game_id(game.ID);
            game.Publishers = publishers;

            return game;
        }
        #endregion
        
        #region Publishers
        static private bool Check_publisher_by_name(string name)
        {
            string expression = $"SELECT COUNT({Publisher._name}) FROM {Publisher._publisher} WHERE {Publisher._name}=@name";
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
        static public List<Publisher> Get_publishers()
        {
            string expression = $"SELECT * " +
                $"FROM {Publisher._publisher} ";

            SqliteDataReader gamesReader;
            List<Publisher> publishers = new();

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
                    Publisher publ = new ()
                    {
                        ID = Convert.ToInt32(gamesReader[Publisher._id]),
                        Name = gamesReader[Publisher._name].ToString(),
                    };
                    publishers.Add(publ);
                }
            }

            return publishers;
        }
        static public List<Publisher> Get_publishers_by_game_id(int game_id)
        {
            string expression = $"SELECT {GamePublisher._publisher_id} " +
                $"FROM {GamePublisher._game_publisher} " +
                $"WHERE {GamePublisher._game_id}=@game_id";

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

                SqliteDataReader gamesReader = command.ExecuteReader();

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
        #endregion

        static public List<GameInfo> Get_my_game_infos(int profile_id, int game_id=-1)
        {
            string expression = $"SELECT * FROM {GameInfo._game_infos} " +
                $"WHERE {GameInfo._profile_id}=@profile_id";

            expression += game_id == -1 ? 
                ";" : $" AND {GameInfo._game_id}=@game_id";

            SqliteDataReader gamesReader;
            List<GameInfo> games = new();

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadOnly;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                SqliteParameter param = new("@profile_id", profile_id);
                if (game_id != -1) 
                {
                    SqliteParameter param_gmid = new("@game_id", game_id); ;
                    command.Parameters.Add(param_gmid);
                }

                command.Parameters.Add(param);

                gamesReader = command.ExecuteReader();

                while (gamesReader.Read())
                {
                    GameInfo gi = new()
                    {
                        game_id = Convert.ToInt32( gamesReader[GameInfo._game_id]),
                        profile_id = Convert.ToInt32(gamesReader[GameInfo._profile_id]),
                        time_in_game = Convert.ToUInt64(gamesReader[GameInfo._minuts_in_game]),
                        executable_file = gamesReader[GameInfo._executable_file].ToString(),
                        save_file = gamesReader[GameInfo._save_file].ToString(),
                    };
                    gi.game_title = Get_Game(gi.game_id).Title ?? "-";
                    games.Add(gi);
                }
            }

            return games;
        }
        #endregion

        #region UpdateData

        internal static void UpdateGame(Game gm)
        {
            UpdateGame(gm.ID, gm.Title, gm.Descriptions, gm.Relese_date);
            UnbindPublisherFromGame(gm.ID);
            foreach (Publisher publ in gm.Publishers) 
            {
                BindGamePublisher(gm.ID, publ.ID);
            }
        }
        static public void UpdateGame(int game_id, string title="", string descr="", DateTime? release_date=null) 
        {
            string expression = $"UPDATE {Game._games} ";

            Stack<string> exprParams = new();
            Stack<SqliteParameter> cmdParams = new();

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadWrite;"))
            {
                if (title != "")
                {
                    cmdParams.Push(new SqliteParameter("@title", title));
                    exprParams.Push($"{Game._title}=@title");
                }
                if (descr != "")
                {
                    cmdParams.Push(new SqliteParameter("@descr", descr));
                    exprParams.Push($"{Game._descr}=@descr");
                }
                if (release_date != null)
                {
                    cmdParams.Push(new SqliteParameter("@release_date", release_date?.ToString(@"yyyy/MM/dd")));
                    exprParams.Push($"{Game._relese_date}=@release_date");
                }

                if (exprParams.Count == 0) return;
                
                expression += $"SET {exprParams.Pop()}";
                while(exprParams.Count > 0)
                    expression += $", {exprParams.Pop()}";
                expression += $" WHERE {Game._id}=@id";

                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                while (cmdParams.Count > 0)
                    command.Parameters.Add(cmdParams.Pop());
                command.Parameters.Add(new SqliteParameter("@id", game_id));

                command.ExecuteNonQuery();

            }
        }

        internal static void UpdateMyGame(GameInfo gi)
        {
            UpdateMyGame(gi.game_id, gi.profile_id, gi.executable_file, gi.save_file, gi.time_in_game);
        }
        static public void UpdateMyGame(int game_id, int profile_id, string? path_to_exe=null, string? path_to_save=null, ulong? minuts_in_game=null) 
        {
            string expression = $"UPDATE {GameInfo._game_infos} ";

            Stack<string> exprParams = new();
            Stack<SqliteParameter> cmdParams = new();

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadWrite;"))
            {
                if (path_to_exe != null)
                {
                    cmdParams.Push(new SqliteParameter("@path_to_exe", path_to_exe));
                    exprParams.Push($"{GameInfo._executable_file}=@path_to_exe");
                }
                if (path_to_save != null)
                {
                    cmdParams.Push(new SqliteParameter("@path_to_save", path_to_save));
                    exprParams.Push($"{GameInfo._save_file}=@path_to_save");
                }
                if (minuts_in_game != null)
                {
                    cmdParams.Push(new SqliteParameter("@minuts_in_game", minuts_in_game));
                    exprParams.Push($"{GameInfo._minuts_in_game}=@minuts_in_game");
                }

                if (exprParams.Count == 0) return;
                
                expression += $"SET {exprParams.Pop()}";
                while(exprParams.Count > 0)
                    expression += $", {exprParams.Pop()}";
                expression += $" WHERE {GameInfo._profile_id}=@profile_id AND {GameInfo._game_id}=@game_id";

                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                while (cmdParams.Count > 0)
                    command.Parameters.Add(cmdParams.Pop());
                command.Parameters.Add(new SqliteParameter("@game_id", game_id));
                command.Parameters.Add(new SqliteParameter("@profile_id", profile_id));

                command.ExecuteNonQuery();

            }
        }
        #endregion

        #region Remove data
        public static void UnbindPublisherFromGame(int game_id)
        {
            string expression = $"DELETE FROM {GamePublisher._game_publisher} " +
                $"WHERE {GamePublisher._game_id}=@game_id";

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                command.Parameters.Add(new SqliteParameter("@game_id", game_id));
                command.ExecuteNonQuery();
            }
        }
        static public void DeleteMyGame(int profile_id, int game_id) 
        {
            string expression = $"DELETE FROM {GameInfo._game_infos} " +
                    $"WHERE {GameInfo._game_id}=@game_id AND {GameInfo._profile_id}=@profile_id";

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                command.Parameters.Add(new SqliteParameter("@game_id", game_id));
                command.Parameters.Add(new SqliteParameter("@profile_id", profile_id));
                command.ExecuteNonQuery();
            }
        }
        static public bool DeleteGame(Game gm)
        {
            return DeleteGame(gm.ID);
        }
        static public bool DeleteGame(int id)
        {
            string expression = $"DELETE FROM {GameInfo._game_infos} " +
                $"WHERE {GameInfo._game_id}=@game_id";

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                command.Parameters.Add(new SqliteParameter("@game_id", id));
                command.ExecuteNonQuery();
            }

            expression = $"DELETE FROM {GamePublisher._game_publisher} " +
                $"WHERE {GamePublisher._game_id}=@game_id";

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {   
                    Connection = connection,
                    CommandText = expression
                };

                command.Parameters.Add(new SqliteParameter("@game_id", id));
                command.ExecuteNonQuery();
            }

            expression = $"DELETE FROM {Game._games} " +
               $"WHERE {Game._id}=@id";

            using (var connection = new SqliteConnection(connectStr + "Mode=ReadWrite;"))
            {
                connection.Open();
                SqliteCommand command = new()
                {
                    Connection = connection,
                    CommandText = expression
                };

                command.Parameters.Add(new SqliteParameter("@id", id));
                command.ExecuteNonQuery();
            }
            return true;
        }
        #endregion
    }
}