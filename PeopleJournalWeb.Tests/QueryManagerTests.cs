using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PeopleJournalWeb.Controllers;
using PeopleJournalWeb.Service;
using Microsoft.AspNetCore.Mvc;
using PeopleJournalWeb.Model;
using PeopleJournalWeb.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PeopleJournalWeb.Tests
{

    public class QueryManagerTests
    {
        [Fact]
        public void UserLoginTest()
        {
            // Arrange
            System.Diagnostics.Debug.WriteLine("Creating taskHandler");
            TaskHandler taskHandler = new TaskHandler();
            System.Diagnostics.Debug.WriteLine("taskHandler created");
            JObject userObj = new JObject(
                new JProperty("Login","Noka"),
                new JProperty("Password", "T2jsq2os4SEQ7xsA"),
                new JProperty("LocationData", "Here"));
            string userJson = userObj.ToString();
            System.Diagnostics.Debug.WriteLine($"{userJson} created");
            // Act
            bool result = taskHandler.UserLogin(userJson).Result;
            System.Diagnostics.Debug.WriteLine($"Got result {result}");
            // Assert
            Assert.True(result);

        }

        [Fact]
        public void TestTest()
        {
            System.Diagnostics.Debug.WriteLine("Just test");
            Assert.True(true);
        }

        public void DataEditingTest()
        {
            TaskHandler taskHandler = new TaskHandler();
        }

    }
}
