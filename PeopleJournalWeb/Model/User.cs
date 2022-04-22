using PeopleJournalWeb.Interface;

namespace PeopleJournalWeb.Model
{
    [Serializable]
    class User : ObjModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? LocationData { get; set; }

        public User(string name, string password, string locationData)
        {
            Login=name;
            Password = password;
            LocationData = locationData;
        }

        public override string ToString()
        {
            return Login;
        }
    }
}
