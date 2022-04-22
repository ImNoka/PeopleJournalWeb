using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeopleJournalWeb.Model;
using System.Data;
using Microsoft.Data.SqlClient;
using PeopleJournalWeb.Interface;
using PeopleJournalWeb.Service;
using Newtonsoft.Json;

namespace PeopleJournalWeb.Controller
{
    internal class QueryManager
    {
        List<ObjModel> People;
        List<ObjModel> Histories;
        List<ObjModel> Meets;

        string connectionString = "Server=DESKTOP-GP81B11\\SQLNOKAINC;Database=FamilyAndFriends;User Id=NokaNew; Password=NDBpass1;" +
            "Encrypt=false";
        SqlConnection sqlConnection;

        
        public QueryManager()
        {
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }

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

        /// <summary>
        /// Get any table in Task<DataSet> format with using adapter.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private async Task<DataSet> GetData(string table)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(
                $"SELECT * FROM {table}", sqlConnection);
            DataSet ds = new DataSet();
            await Task.Run(() => { adapter.Fill(ds); });
            return ds;
        }

        /// <summary>
        /// Get people with GetData
        /// </summary>
        /// <returns></returns>
        public DataSet GetPeople()
        {
            //SqlCommand command = sqlConnection.CreateCommand();
            //command.CommandType = CommandType.StoredProcedure;
            //command.CommandText = "exec GetPersons";
            //using ()
            return GetData("Persons").Result;
        }


        public void deletePerson(int id)
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
        public void addPerson(string jsonPerson)
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
            sqlCommand.ExecuteNonQuery();
        }
        public void editPerson(string jsonPerson)
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
            sqlCommand.ExecuteNonQuery();
        }

        public void AddMeet(string jsonMeet)
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
            sqlCommand.ExecuteNonQuery();
        }
        public void EditMeet(string jsonMeet)
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
            sqlCommand.ExecuteNonQuery();
        }
        public void deleteMeet(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("DeleteMeetById", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = new SqlParameter
            {
                ParameterName = "@id",
                Value = id
            };
            sqlCommand.Parameters.Add(idParam);
            sqlCommand.ExecuteNonQuery();
        }

        public void deleteHistory(int id)
        {
            SqlCommand sqlCommand = new SqlCommand("DeleteHistoryId", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            SqlParameter idParam = new SqlParameter
            {
                ParameterName = "@id",
                Value = id
            };
            sqlCommand.Parameters.Add(idParam);
            sqlCommand.ExecuteNonQuery();
        }
        public void clearHistory()
        {
            SqlCommand sqlCommand = new SqlCommand("ClearHistory", sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.ExecuteNonQuery();
        }

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


        public async void ConnectDB()
        {
            await sqlConnection.OpenAsync();
        }

        public async void DisconnectDB()
        {
            await sqlConnection.CloseAsync();
        }
    }
}
