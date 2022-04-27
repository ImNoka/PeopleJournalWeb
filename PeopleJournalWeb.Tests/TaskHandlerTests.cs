using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PeopleJournalWeb.Controllers;
using PeopleJournalWeb.Interface;
namespace PeopleJournalWeb.Tests
{
    public class TaskHandlerTests
    {
        [Fact]
        public async void HighLoadTest()
        {
            TaskHandlerController taskHandler = new TaskHandlerController();
            List<string> lists = new List<string>();
            int count = 0;
            //List<Task> tasks = new List<Task>();
            Task[] tasks = new Task[10];
            var k = Task.Run(async () =>
            {
                for (int i = 0; i < tasks.Length; i++)
                    tasks[i] = (Task.Run(async () =>
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var s = Task.Run(() => lists.Add(taskHandler.GetData("api/people")));
                            var a = Task.Run(() => lists.Add(taskHandler.GetData("api/histories")));
                            var v = Task.Run(() => lists.Add(taskHandler.GetData("api/VSYADATA")));
                            var w = Task.Run(() => lists.Add(taskHandler.GetData("api/meets")));
                            count += 4;
                        }
                    }));
            });
            
            for (int i = 0; i < 10; i++)
            {
                
                lists.Add(taskHandler.GetData("api/people"));
                lists.Add(taskHandler.GetData("api/histories"));
                lists.Add(taskHandler.GetData("api/VSYADATA"));
                lists.Add(taskHandler.GetData("api/meets"));
                count += 4;
            }
            
            Task.WaitAll(tasks);
            System.Diagnostics.Debug.WriteLine(count);
            Assert.Equal(count, lists.Count);
        }
    }
}
