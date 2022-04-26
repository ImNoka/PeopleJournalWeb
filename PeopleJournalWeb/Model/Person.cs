using PeopleJournalWeb.Interface;

namespace PeopleJournalWeb.Model
{
    [Serializable]
    class Person : ObjModel
    {
        public int? Id { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public int? P_Year { get; set; }
        public string? P_Status { get; set; }
        public string? Phone_Number { get; set; }
        public string? VK { get; set; }
        public string? Instagram { get; set; }
        public string? City { get; set; }
        public int? Number { get; set; }

        public override string ToString()
        {
            return "Person: "+Id+" "+First_Name + " " + Last_Name;
        }
    }
}
