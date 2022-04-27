using PeopleJournalWeb.Interface;
using Newtonsoft.Json;

namespace PeopleJournalWeb.Controllers
{
    public class ListJsonSerializer
    {
        public ListJsonSerializer()
        {

        }

        public string ListToJson(List<ObjModel> list)
        {
            return JsonConvert.SerializeObject(list);
        }

        internal List<T> JsonToList<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }

        internal T JsonToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }
    }
}
