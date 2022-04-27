using Microsoft.AspNetCore.Mvc;
using PeopleJournalWeb.Controllers;
using PeopleJournalWeb.Filters;
using PeopleJournalWeb.Interface;

namespace PeopleJournalWeb.Controllers
{
    public class TaskHandlerController : Controller
    {

        /// SQL queries manager. Controllers.
        private QueryManager qManager;

        private ListJsonSerializer listJson;


        // Query tasks list. Separation by returning values.
        private List<Task<List<ObjModel>>> getDataTasks;
        private List<Task>? simpleTasks;
        private List<Task<bool>> loginTasks;
        
        private readonly object _getDataTasksLock = new object();
        private readonly object _simpleTasksLock = new object();
        private readonly object _loginTasksLock = new object();

        /// <summary>
        /// Forms a queue of requests and direct them to main controller.
        /// Public methods adds new tasks to queue.
        /// </summary>
        public TaskHandlerController()
        {
            qManager = new QueryManager("","","","","");
            getDataTasks = new List<Task<List<ObjModel>>>();
            listJson = new ListJsonSerializer();
            simpleTasks = new List<Task>();
            loginTasks = new List<Task<bool>>();
            tickRun();
        }

        /// <summary>
        /// Async timer starter.
        /// </summary>
        /// <returns></returns>
        private async Task tickRun()
        {
            await Task.Run(() => tick());
        }

        /// <summary>
        /// Tracking timer. Checks task storage for unacted
        /// and initiate run of them if it's exists.
        /// </summary>
        /// <returns></returns>
        private Task tick()
        {
            while (true)
            {
                //System.Diagnostics.Debug.WriteLine("Tick...");
                int n = getDataTasks.Count;
                int m = simpleTasks.Count;
                int l = loginTasks.Count;     
                if (n != 0|| m != 0|| l != 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Got {getDataTasks.Count+simpleTasks.Count+loginTasks.Count} tasks. Run.");
                    doTasks(n,m,l);
                }
                //Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Task executor. Run tasks in accumulated queue.
        /// </summary>
        /// <param name="n">GetDataTasks count</param>
        /// <param name="m">SimpleTasks count</param>
        /// <param name="l">LoginTasks count</param>
        private void doTasks(int n, int m, int l)
        {
            // To continue putting new tasks to lists without exceptions
            // cycles realized with simple for instead of foreach.
            for (int i = 0; i < l; i++)
            {
                loginTasks.First().Start();
                loginTasks.First().Wait();
                loginTasks.RemoveAt(0);
            }
            for (int i = 0; i<n;i++)
            {
                getDataTasks.First().Start();
                getDataTasks.First().Wait();
                getDataTasks.RemoveAt(0);
            }

            for (int i = 0; i < m; i++)
            {
                simpleTasks.First().Start();
                simpleTasks.First().Wait();
                simpleTasks.RemoveAt(0);
            }
        }  

        /// <summary>
        /// Sends the login, password, location data
        /// to check user in database.
        /// If login and password are right, return True.
        /// </summary>
        /// <param name="userJson"></param>
        /// <returns></returns>
        public async Task<bool> UserLogin(string userJson)
        {
            Task<bool> t = new Task<bool>(() => qManager.UserLogin(userJson));
            lock (_loginTasksLock)
            {
                loginTasks.Add(t);
            }
            t.Wait();
            return t.Result;
        }

        public void ClearHistory()
        {
            Task t = new Task(() => qManager.ClearHistory());
            AddSimpleTask(t);
        }

        /// <summary>
        /// Get data table depending on the API string in JSON format.
        /// </summary>
        /// <param name="strApi"></param>
        /// <returns>JSON string</returns>
        public async Task<string> GetData(string strApi)
        {
                List<ObjModel> res;
                Task<List<ObjModel>> t;
                switch (strApi)
                {
                    case "api/people":
                        t = new Task<List<ObjModel>>(() => qManager.GetListPeople().Result);
                    lock (_getDataTasksLock)
                    {
                        getDataTasks.Add(t);
                    }
                        break;
                    case "api/meets":
                        t = new Task<List<ObjModel>>(() => qManager.GetListMeets().Result);
                    lock (_getDataTasksLock)
                    {
                        getDataTasks.Add(t);
                    }
                    break;
                    case "api/histories":
                        t = new Task<List<ObjModel>>(() => qManager.GetListHistories().Result);
                    lock (_getDataTasksLock)
                    {
                        getDataTasks.Add(t);
                    }
                    break;
                    default:
                        return "Wrong query. No data";
                }
                System.Diagnostics.Debug.WriteLine($"Task {strApi} created, lets wait.");
                t.Wait();
                System.Diagnostics.Debug.WriteLine($"{strApi} completed. Return.");
                res = t.Result;
                System.Diagnostics.Debug.WriteLine($"Returning data:\n{t.Result[0]}");
                return listJson.ListToJson(res);
        }



        // Simple tasks using ID or data in JSON format.
        #region Simple tasks region
        // Person tasks.
        public async void DeletePerson(int id)
        {

            Task t = new Task(() => qManager.DeletePerson(id));
            AddSimpleTask(t);
        }
        public async void EditPerson(string json)
        {
            Task t = new Task(() => qManager.EditPerson(json));
            AddSimpleTask(t);
        }
        public async void CreatePerson(string json)
        {
            Task t = new Task(() => qManager.AddPerson(json));
            AddSimpleTask(t);
        }

        // Meet tasks.
        public async void CreateMeet(string json)
        {
            Task t = new Task(() => qManager.AddMeet(json));
            AddSimpleTask(t);
        }
        public async void EditMeet(string json)
        {
            Task t = new Task(() => qManager.EditMeet(json));
            AddSimpleTask(t);
        }
        public async void DeleteMeet(int id)
        {
            Task t = new Task(() => qManager.DeleteMeet(id));
            AddSimpleTask(t);
        }

        // History tasks.
        public async void DeleteHistory(int id)
        {
            Task t = new Task(() => qManager.DeleteHistory(id));
            AddSimpleTask(t);
        }



        private async void AddSimpleTask(Task t)
        {
            lock(_simpleTasksLock)
            {
                simpleTasks.Add(t);
            }
        }

        #endregion


        
        // Alter realization for simple tasks adding

        public enum TaskAction
        {
            CreatePerson,
            DeletePerson,
            EditPerson,
            CreateMeet,
            DeleteMeet,
            EditMeet,
            DeleteHistory
        }

        public async void SingleObjectAction(object data, TaskAction action)
        {
            Task t;
            switch (action)
            {
                case TaskAction.CreatePerson:
                    t = new Task(() => qManager.AddPerson((string)data));
                    break;
                case TaskAction.DeletePerson:
                    t = new Task(() => qManager.DeletePerson((int)data));
                    break;
                case TaskAction.EditPerson:
                    t = new Task(() => qManager.EditPerson((string)data));
                    break;
                case TaskAction.CreateMeet:
                    t = new Task(() => qManager.AddMeet((string)data));
                    break;
                case TaskAction.DeleteMeet:
                    t = new Task(() => qManager.DeleteMeet((int)data));
                    break;
                case TaskAction.EditMeet:
                    t = new Task(() => qManager.EditMeet((string)data));
                    break;
                case TaskAction.DeleteHistory:
                    t = new Task(() => qManager.DeleteHistory((int)data));
                    break;
                default:
                    t = null;
                    break;
            }
            AddSimpleTask(t??throw new ArgumentNullException("Task type not found."));

        }
    }
}
