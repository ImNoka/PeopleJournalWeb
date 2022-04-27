using System.Data;
using Newtonsoft.Json;

namespace PeopleJournalWeb.Controllers
{
    public class DataTableJSONSerializer
    {
        public DataTableJSONSerializer()
        {

        }

        public string DTtoJSON(DataTable dt)
        {
            return JsonConvert.SerializeObject(dt);
        }
        public DataTable JSONtoDT(string json)
        {
            return JsonConvert.DeserializeObject<DataTable>(json);
        }
    }
}
