using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using PeopleJournalWeb.Pages;
using PeopleJournalWeb.Service;
using PeopleJournalWeb.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore;
using PeopleJournalWeb.Service.Logging;
using System.Web;
using Newtonsoft.Json.Linq;

List<User> users = new List<User>();


TaskHandler taskHandler = new TaskHandler();
System.Diagnostics.Debug.WriteLine($"Main run ticks.");
System.Diagnostics.Debug.WriteLine($"Ticks started.");
var builder = WebApplication.CreateBuilder((
    new WebApplicationOptions { 
        ApplicationName = typeof(Program).Assembly.FullName,
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = Environments.Production,
        WebRootPath = "Pages"
    }));

builder.WebHost.UseIISIntegration();
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory()+"\\logs\\custom_logs", "logs.txt"));
System.Diagnostics.Debug.WriteLine(Path.Combine(Directory.GetCurrentDirectory() + "\\logs\\custom_logs", "logs.txt"));



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();

ListJsonSerializer listJson = new ListJsonSerializer();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();

app.MapGet("/",(HttpContext context)=> 
{
    app.Logger.LogInformation($"Time: {DateTime.Now.ToLongDateString()} Path: {context.Request.Path} ");
    return Results.Redirect("index.html");
    //Results.Redirect("index.html"); 
});

// Users
app.MapPost("/login", async (HttpContext context) =>
{
    System.Diagnostics.Debug.WriteLine($"Got /login query {context}");
    app.Logger.LogInformation($"Time: {DateTime.Now.ToLongDateString()} Path: {context.Request.Path} ");
    //System.Diagnostics.Debug.WriteLine(context.Request.Cookies["value"]);
    var form = context.Request.Form;
    if (!form.ContainsKey("login") || !form.ContainsKey("password"))
        return Results.BadRequest("Login and password wasn't set.");

    string login = form["login"];
    string password = form["password"];
    string locationData = context.Connection.RemoteIpAddress.ToString();
    JObject userObj = new JObject(
                new JProperty("Login", login),
                new JProperty("Password", password),
                new JProperty("LocationData", locationData));
    if (!taskHandler.UserLogin(userObj.ToString()).Result)
        return Results.Unauthorized();
    //User? user = users.FirstOrDefault(u => u.Login==login && u.Password == password);
    users.Add(new User(login, password, locationData));
    //if (user is null)
    //    return Results.Unauthorized();
    var claims = new List<Claim> {  new Claim(ClaimTypes.Name, login)};
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    await context.SignInAsync(  CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity));
    
    return Results.Accepted();
});
/*
app.MapPost("/login", async (HttpContext context) =>
{
    System.Diagnostics.Debug.WriteLine($"Got /login query {context}");
    app.Logger.LogInformation($"Time: {DateTime.Now.ToLongDateString()} Path: {context.Request.Path} ");
    var form = context.Request.Form;

});*/

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

// Person maps
app.MapGet("api/people", [Authorize](HttpContext context) => 
{
    //app.Logger.LogInformation($"Time: {DateTime.Now.ToLongDateString()} Path: {context.Request.Path} StatusCode: {context.Response.StatusCode}");
    
    return taskHandler.GetData("api/people");
    });
app.MapDelete("api/people/{id}", [Authorize](int id) =>
{
    taskHandler.deletePerson(id);
});
app.MapPost("api/people/createperson", [Authorize](HttpContext context) => ExecutePost(context));
app.MapPost("api/people/editperson", [Authorize](HttpContext context) => ExecutePost(context));


// Meet maps
app.MapGet("api/meets", [Authorize]() => (taskHandler.GetData("api/meets")));
app.MapDelete("api/meets/{id}", [Authorize](int id) =>
{
    taskHandler.deleteMeet(id);
});
app.MapPost("api/meets/createmeet", [Authorize](HttpContext context) => ExecutePost(context));
app.MapPost("api/meets/editmeet", [Authorize](HttpContext context) => ExecutePost(context));


// History maps
app.MapGet("api/histories", [Authorize]() => (taskHandler.GetData("api/histories")));
app.MapDelete("api/histories/{id}", [Authorize](int id) =>
{
    taskHandler.deleteHistory(id);
});
app.MapDelete("api/histories/clear", [Authorize]() =>
{
    taskHandler.clearHistory();
});


async void ExecutePost(HttpContext context)
{
    using (StreamReader reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8))
    {
        string jsonString = await reader.ReadToEndAsync();
        System.Diagnostics.Debug.WriteLine("Got: \n" + jsonString);
        System.Diagnostics.Debug.WriteLine("POST: "+context.Request.Path);
        switch (context.Request.Path)
        {
            case "/api/meets/createmeet":
                taskHandler.createMeet(jsonString);
                break;
            case "/api/people/createperson":
                taskHandler.createPerson(jsonString);
                break;
            case "/api/people/editperson":
                taskHandler.editPerson(jsonString);
                break;
            case "/api/meets/editmeet":
                taskHandler.EditMeet(jsonString);
                break;
        }
    }
}



app.UseHttpsRedirection();
//app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpLogging();

app.UseRouting();
app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();
app.MapRazorPages();

System.Diagnostics.Debug.WriteLine("DIRECTORY: "+Directory.GetCurrentDirectory());

/*app.Run( async (HttpContext context) =>
{
    app.Logger.LogInformation($"Time: {DateTime.Now.ToLongDateString} Path: {context.Request.Path} ");
    context.Response.Redirect("index.html");
});*/

app.Run();

System.Diagnostics.Debug.WriteLine("APP RAN");
