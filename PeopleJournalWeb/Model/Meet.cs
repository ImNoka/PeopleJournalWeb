using PeopleJournalWeb.Interface;

namespace PeopleJournalWeb.Model
{
    [Serializable]
    class Meet : ObjModel
    {
        public int? Id { get; set; }
        public int? Person_Id { get; set; }
        public string? Meet_Date { get; set; }
        public string? Place { get; set; }
        public string? City { get; set; }
        public string? Full_Name { get; set; }

        public override string ToString()
        {
            return "Meet: "+Person_Id + " " + Full_Name + " " + Meet_Date;
        }

    }
}
