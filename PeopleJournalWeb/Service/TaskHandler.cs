using PeopleJournalWeb.Controller;
using PeopleJournalWeb.Interface;
using PeopleJournalWeb.Model;
using PeopleJournalWeb.Service;

namespace PeopleJournalWeb.Service
{
    public class TaskHandler
    {
        /// <summary>
        /// SQL queries manager.
        /// </summary>
        QueryManager qManager;

        /// <summary>
        /// Query tasks list.
        /// </summary>
        private List<Task<List<ObjModel>>> tasks;
        private List<Task>? editTasks;
        private List<Task<bool>> loginTasks;
        private ListJsonSerializer listJson;

        /// <summary>
        /// Task handler, doing api query managmnet.
        /// </summary>
        public TaskHandler()
        {
            qManager = new QueryManager();
            tasks = new List<Task<List<ObjModel>>>();
            listJson = new ListJsonSerializer();
            editTasks = new List<Task>();
            loginTasks = new List<Task<bool>>();
            TickRun();
        }

        /// <summary>
        /// Async timer starter.
        /// </summary>
        /// <returns></returns>
        public async Task TickRun()
        {
            System.Diagnostics.Debug.WriteLine($"Running ticks.");
            await Task.Run(() => Tick());
            //Task tick = new Task(()=> Tick());
            //await tick.Start();
        }

        /// <summary>
        /// Tracking timer.
        /// </summary>
        /// <returns></returns>
        private Task Tick()
        {
            System.Diagnostics.Debug.WriteLine("Start ticks...");
            while (true)
            {
                //System.Diagnostics.Debug.WriteLine($"Tick...");
                Thread.Sleep(1000);
                if (tasks.Count != 0||editTasks.Count!=0||loginTasks.Count!=0)
                {
                    System.Diagnostics.Debug.WriteLine( $"Have {tasks.Count+editTasks.Count} " +
                                                        $"tasks, start these.");
                    DoTasks();
                }
            }
        }

        /// <summary>
        /// Task executor.
        /// </summary>
        private void DoTasks()
        {
            //List<Task<List<ObjModel>>> tasks_ = new List<Task<List<ObjModel>>>();
            int n = tasks.Count;
            int m = editTasks.Count;
            int l = loginTasks.Count; ;
            for (int i = 0; i < l; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Run getdata task.");
                loginTasks.First().Start();
                System.Diagnostics.Debug.WriteLine($"Wait getdata task.");
                loginTasks.First().Wait();
                System.Diagnostics.Debug.WriteLine($"Task getdata completed. Delete it.");
                loginTasks.RemoveAt(0);
            }
            for (int i = 0; i<n;i++)
            {
                System.Diagnostics.Debug.WriteLine($"Run getdata task.");
                tasks.First().Start();
                System.Diagnostics.Debug.WriteLine($"Wait getdata task.");
                tasks.First().Wait();
                System.Diagnostics.Debug.WriteLine($"Task getdata completed. Delete it.");
                tasks.RemoveAt(0);
            }

            for (int i = 0; i < m; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Run editdata task.");
                editTasks.First().Start();
                System.Diagnostics.Debug.WriteLine($"Wait editdata task.");
                editTasks.First().Wait();
                System.Diagnostics.Debug.WriteLine($"Task editdata completed. Delete it.");
                editTasks.RemoveAt(0);
            }
            /*
            foreach (Task<List<ObjModel>> t in tasks)
            {
                System.Diagnostics.Debug.WriteLine($"Run task {t}.");
                t.Start();
                System.Diagnostics.Debug.WriteLine($"Wait task {t}.");
                t.Wait();
            }
            foreach(Task t in editTasks)
            {
                System.Diagnostics.Debug.WriteLine($"Run task {t}.");
                t.Start();
                System.Diagnostics.Debug.WriteLine($"Wait task {t}.");
                t.Wait();
            }*/
            //System.Diagnostics.Debug.WriteLine($"Tasks started. Wait all.");
            //Task.WaitAll(tasks.ToArray());
            System.Diagnostics.Debug.WriteLine($"");
            //tasks.Clear();
            //editTasks.Clear();
            System.Diagnostics.Debug.WriteLine("Tasks cleared. Continue ticks...");

        }  

        public async Task<bool> UserLogin(string userJson)
        {
            System.Diagnostics.Debug.WriteLine($"Got create query for person {userJson}");
            Task<bool> t = new Task<bool>(() => qManager.UserLogin(userJson));
            loginTasks.Add(t);
            t.Wait();
            return t.Result;
        }

        public void clearHistory()
        {
            System.Diagnostics.Debug.WriteLine($"Got clear all history query");
            Task t = new Task(() => qManager.clearHistory());
            editTasks.Add(t);
        }

        public string GetData(string strApi)
        {
            System.Diagnostics.Debug.WriteLine($"Got api query {strApi}");
            List<ObjModel> res;
            Task<List<ObjModel>> t;
            switch (strApi)
            {
                case "api/people":
                    t = new Task<List<ObjModel>>(() => qManager.GetListPeople().Result);
                    tasks.Add(t);
                    break;
                case "api/meets":
                    t = new Task<List<ObjModel>>(() => qManager.GetListMeets().Result);
                    tasks.Add(t);
                    break;
                case "api/histories":
                    t = new Task<List<ObjModel>>(() => qManager.GetListHistories().Result);
                    tasks.Add(t);
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Wrong query");
                    return "Wrong query. No data";
                    //throw new Exception("Wrong api query.");
            }
            System.Diagnostics.Debug.WriteLine($"Task {strApi} created, lets wait.");
            t.Wait();
            System.Diagnostics.Debug.WriteLine($"{strApi} completed. Return.");
            res = t.Result;
            System.Diagnostics.Debug.WriteLine($"Returning data:\n{t.Result[0]}");
            return listJson.ListToJson(res);
            

        }
        
        // Single person tasks
        public void deletePerson(int id)
        {
            System.Diagnostics.Debug.WriteLine($"Got delete query for person {id}");
            Task t = new Task(() =>qManager.deletePerson(id));
            editTasks.Add(t);
        }
        public void editPerson(string json)
        {
            System.Diagnostics.Debug.WriteLine($"Got edit query for person {json}");
            Task t = new Task(() => qManager.editPerson(json));
            editTasks.Add(t);
        }
        public void createPerson(string json)
        {
            System.Diagnostics.Debug.WriteLine($"Got create query for person {json}");
            Task t = new Task(() => qManager.addPerson(json));
            editTasks.Add(t);
        }

        // Single meet tasks
        public void createMeet(string json)
        {
            System.Diagnostics.Debug.WriteLine($"Got create query for meet {json}");
            Task t = new Task(() => qManager.AddMeet(json));
            editTasks.Add(t);
        }
        public void EditMeet(string json)
        {
            System.Diagnostics.Debug.WriteLine($"Got edit query for meet {json}");
            Task t = new Task(() => qManager.EditMeet(json));
            editTasks.Add(t);
        }
        public void deleteMeet(int id)
        {
            System.Diagnostics.Debug.WriteLine($"Got delete query for meet {id}");
            Task t = new Task(() => qManager.deleteMeet(id));
            editTasks.Add(t);
        }

        // Single history tasks
        public void deleteHistory(int id)
        {
            System.Diagnostics.Debug.WriteLine($"Got delete query for history {id}");
            Task t = new Task(() => qManager.deleteHistory(id));
            editTasks.Add(t);
        }



        // Alter realization for single object tasks

        public enum ObjectAction
        {
            CreatePerson,
            DeletePerson,
            EditPerson,
            CreateMeet,
            DeleteMeet,
            EditMeet,
            DeleteHistory
        }

        public void SingleObjectAction(object data, ObjectAction action)
        {
            Task t;
            switch (action)
            {
                case ObjectAction.CreatePerson:
                    t = new Task(() => qManager.addPerson((string)data));
                    break;
                case ObjectAction.DeletePerson:
                    t = new Task(() => qManager.deletePerson((int)data));
                    break;
                case ObjectAction.EditPerson:
                    t = new Task(() => qManager.editPerson((string)data));
                    break;
                case ObjectAction.CreateMeet:
                    t = new Task(() => qManager.AddMeet((string)data));
                    break;
                case ObjectAction.DeleteMeet:
                    t = new Task(() => qManager.deleteMeet((int)data));
                    break;
                case ObjectAction.EditMeet:
                    t = new Task(() => qManager.EditMeet((string)data));
                    break;
                case ObjectAction.DeleteHistory:
                    t = new Task(() => qManager.deleteHistory((int)data));
                    break;
                default:
                    t = null;
                    break;
            }
                editTasks.Add(t??throw new ArgumentNullException("Task type not found."));

        }
    }
}
