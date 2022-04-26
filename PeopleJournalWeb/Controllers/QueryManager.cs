using PeopleJournalWeb.Model;
using System.Data;
using Microsoft.Data.SqlClient;
using PeopleJournalWeb.Interface;
using Newtonsoft.Json;
using PeopleJournalWeb.Filters;



namespace PeopleJournalWeb.Controllers
{
    internal class QueryManager
    {
        // Lists, loading from database on request.
        List<ObjModel> People;
        List<ObjModel> Histories;
        List<ObjModel> Meets;

        private readonly object _executeLock = new object();

        // Connection string parts.
        private string _server;
        private string _database;
        private string _dbuser;
        private string _password;
        private string _encrypt;

        string connectionString;
        SqlConnection sqlConnection;

        /// <summary>
        /// Represents a stack of methods SQL queries. Use connection string parts to create object.
        /// </summary>
        /// <param name="server">Server address</param>
        /// <param name="database">Database name</param>
        /// <param name="dbuser">Database user</param>
        /// <param name="password">Database user password</param>
        /// <param name="encrypt">Enable encrypt. True or false.</param>
        public QueryManager(string server, string database, string dbuser, string password, string encrypt = "false")
        {
            _server = server;
            _database = database;
            _dbuser = dbuser;
            _password = password;
            _encrypt = encrypt;
            connectionString = $"{_server};{_database};{_dbuser};{_password};{_encrypt}";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }


        #region Get lists methods

        /// <summary>
        /// Get Meets from DB in Task<List<Meet>> format.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ObjModel>> GetListMeets()
        {
            Meets = new List<ObjModel>();
            //if (sqlConnection.State != ConnectionState.Open)
                //await sqlConnection.OpenAsync();
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Meets", sqlConnection);
            sqlCommand.CommandType = CommandType.Text;
            using (SqlDataReader r = await sqlCommand.ExecuteReaderAsync())
            {
                if (r.HasRows)
                {
                    while (await r.ReadAsync())
                    {
                        Meets.Add(new Meet()
                        {
                            Id = r.GetInt32(0),
                            Person_Id = r.GetInt32(1),
                            Meet_Date = r.GetDateTime(2).ToString("yyyy-MM-dd"),
                            Place = (string)r.GetValue(3) ?? "Some",
                            City = (string)r.GetValue(4) ?? "Some",
                            Full_Name = (string)r.GetValue(5)
                        });
                        //System.Diagnostics.Debug.WriteLine(r.GetString(5)+" "+r.GetDateTime(2).ToString("yyyy-MM-dd")+ " loaded");
                    }
                    if (r != null)
                        await r.CloseAsync();
                    //await sqlConnection.CloseAsync();
                }
            }
            return Meets;
        }

        /// <summary>
        /// Get Histories list from DB in Task<List<History>> format.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ObjModel>> GetListHistories() 
        {
            Histories = new List<ObjModel>();
            //if (sqlConnection.State != ConnectionState.Open)
                //await sqlConnection.OpenAsync();
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM History", sqlConnection);
            sqlCommand.CommandType = CommandType.Text;
            using (SqlDataReader r = await sqlCommand.ExecuteReaderAsync())
            {
                if (r.HasRows)
                {
                    while (await r.ReadAsync())
                    {
                        Histories.Add(new History()
                        {
                            Id = r.GetInt32(0),
                            Person_Id = r.GetInt32(1),
                            Person_Full_Name = (string)r.GetValue(2),
                            Action = (string)r.GetValue(3) ?? "Did something.",
                            CreateAt = r.GetDateTime(4).ToString()
                        });
                        //System.Diagnostics.Debug.WriteLine(r.GetString(2) + " " + r.GetDateTime(4).ToString() + " loaded");
                    }
                    if (r != null)
                        await r.CloseAsync();
                    //await sqlConnection.CloseAsync();
                }
            }
            return Histories;
        }

        /// <summary>
        /// Get People from DB in Task<List<Person>> format.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ObjModel>> GetListPeople()
        {
            People = new List<ObjModel>();
            //if(sqlConnection.State!=ConnectionState.Open)
                //await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Persons",sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                using(SqlDataReader r = await sqlCommand.ExecuteReaderAsync())
                {
                    if(r.HasRows)
                    {
                        while (await r.ReadAsync())
                        {
                            People.Add(new Person()
                            {
                                Id = r.GetInt32(0),
                                First_Name = (string)r.GetValue(1),
                                Last_Name = (string)r.GetValue(2) ?? "Some",
                                P_Year = (short)r.GetValue(3),
                                P_Status = (string)r.GetValue(4),
                                Phone_Number = (string)r.GetValue(5),
                                VK = r.GetValue(6).ToString() ?? "Some",
                                Instagram = r.GetValue(7).ToString() ?? "Some",
                                City = r.GetValue(8).ToString() ?? "Some",
                                Number = (short)r.GetValue(9)
                            });
                        //System.Diagnostics.Debug.WriteLine(r.GetString(1)+" loaded");
                        }
                    if (r != null)
                        r.Close();
                    //await sqlConnection.CloseAsync();
                }
                }
            return People;
        }

        #endregion

        #region Simple methods withoud returning data
        public async void DeletePerson(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("DeletePersonById", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = new SqlParameter
            {
                ParameterName = "@id",
                Value = id
            };
            sqlCommand.Parameters.Add(idParam);
            sqlCommand.ExecuteNonQuery();
        }
        public async void AddPerson(string jsonPerson)
        {
            if (jsonPerson == null)
                throw new ArgumentNullException("Person is empty");
            Person person = JsonConvert.DeserializeObject<Person>(jsonPerson);
            SqlCommand sqlCommand = new SqlCommand("AddPerson", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            foreach (var p in person.GetType().GetProperties())
            {
                if (p.Name.ToLower() == "id")
                    continue;
                System.Diagnostics.Debug.WriteLine($"Put {p.Name.ToLower()} to parametrs.");
                sqlCommand.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@" + p.Name.ToLower(),
                    Value = p.GetValue(person)
                });
                System.Diagnostics.Debug.WriteLine($"Put completed.");
            }
            ExecuteNonQueryWithLock(sqlCommand);
        }
        public async void EditPerson(string jsonPerson)
        {
            if (jsonPerson == null)
                throw new ArgumentNullException("Person is empty");
            Person person = JsonConvert.DeserializeObject<Person>(jsonPerson);
            SqlCommand sqlCommand = new SqlCommand("EditPerson", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            foreach(var p in person.GetType().GetProperties())
            {
                sqlCommand.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@"+p.Name.ToLower(),
                    Value = p.GetValue(person)
                });
            }
            ExecuteNonQueryWithLock(sqlCommand);
        }
        public async void AddMeet(string jsonMeet)
        {
            if (jsonMeet == null)
                throw new ArgumentNullException("Person is empty");
            Meet meet = JsonConvert.DeserializeObject<Meet>(jsonMeet);
            SqlCommand sqlCommand = new SqlCommand("AddMeetById", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            foreach (var p in meet.GetType().GetProperties())
            {
                if (p.Name.ToLower() == "id"||p.Name.ToLower()=="full_name")
                    continue;
                sqlCommand.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@" + p.Name.ToLower(),
                    Value = p.GetValue(meet)
                });
            }
            ExecuteNonQueryWithLock(sqlCommand);
        }
        public async void EditMeet(string jsonMeet)
        {
            if (jsonMeet == null)
                throw new ArgumentNullException("Meet is empty");
            Meet meet = JsonConvert.DeserializeObject<Meet>(jsonMeet);
            SqlCommand sqlCommand = new SqlCommand("EditMeet", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            foreach (var p in meet.GetType().GetProperties())
            {
                if (p.Name.ToLower() == "person_id"||p.Name.ToLower()=="full_name")
                    continue;
                sqlCommand.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@" + p.Name.ToLower(),
                    Value = p.GetValue(meet)
                });
            }
            ExecuteNonQueryWithLock(sqlCommand);
        }
        public async void DeleteMeet(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("DeleteMeetById", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = new SqlParameter
            {
                ParameterName = "@id",
                Value = id
            };
            sqlCommand.Parameters.Add(idParam);
            ExecuteNonQueryWithLock(sqlCommand);
        }
        public async void DeleteHistory(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("DeleteHistoryId", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = new SqlParameter
            {
                ParameterName = "@id",
                Value = id
            };
            sqlCommand.Parameters.Add(idParam);
            ExecuteNonQueryWithLock(sqlCommand);
        }
        public async void ClearHistory()
        {
            SqlCommand sqlCommand = new SqlCommand("ClearHistory", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            ExecuteNonQueryWithLock(sqlCommand);
        }

        #endregion

        /// <summary>
        /// Accesses the database to verify the login and password. 
        /// If user exists, returns True.
        /// </summary>
        /// <param name="jsonUser"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool UserLogin(string? jsonUser)
        {
            if (jsonUser == null)
                throw new ArgumentNullException("User is empty");
            User user = JsonConvert.DeserializeObject<User>(jsonUser);
            SqlCommand sqlCommand = new SqlCommand("GetUserData", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@userName",
                Value= user.Login,
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input
            });
            sqlCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@userPassword",
                Value = user.Password,
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input
            });
            sqlCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@locationData",
                Value = user.LocationData,
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input
            });
            sqlCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@status",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            });
            sqlCommand.ExecuteNonQuery();
            bool status = (bool)sqlCommand.Parameters["@status"].Value;
            return status;
        }

        private void ExecuteNonQueryWithLock(SqlCommand sqlCommand)
        {
            lock (_executeLock)
            {
                sqlCommand.ExecuteNonQuery();
            }
        }

    }
}
