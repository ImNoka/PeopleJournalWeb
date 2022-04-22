using PeopleJournalWeb.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleJournalWeb.Model
{
    [Serializable]
    class History : ObjModel
    {
        public int Id { get; set; }
        public int Person_Id { get; set; }
        public string Person_Full_Name { get; set; }
        public string Action { get; set; }
        public string CreateAt { get; set; }

        public override string ToString()
        {
            return "History: "+Person_Full_Name+" "+Action+" "+CreateAt;
        }
    }

    
}
