using Microsoft.AspNetCore.Authentication.Cookies;
using PeopleJournalWeb.Controllers;
using PeopleJournalWeb.Model;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using PeopleJournalWeb.Controllers.Logging;
using Newtonsoft.Json.Linq;
using PeopleJournalWeb.Filters;

// List fills up from database by successful login requests.
List<User> users = new List<User>();

ListJsonSerializer listJson = new ListJsonSerializer();

TaskHandlerController taskHandler = new TaskHandlerController();

// Application builder and services.
#region AppBuilder
var builder = WebApplication.CreateBuilder((
    new WebApplicationOptions { 
        ApplicationName = typeof(Program).Assembly.FullName,
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = Environments.Production,
        WebRootPath = "Pages"
    }));

builder.WebHost.UseIISIntegration();

// Using RequestLoggingMiddleware to note each action of users.
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory()+"\\logs\\custom_logs", "logs.txt"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });


builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();
var app = builder.Build();
// Configure the HTTP request pipeline.

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseExtensionExceptionFilter();
app.UseHsts();



app.UseAuthentication();

#endregion

// Almost all map methods directs requests to TaskHandlerController
#region Maps

/// <summary>
/// Takes the user data from form. Sends user in JSON inside program
/// to compare with database.
/// If login and password are right, user will be added to claims and users.
/// </summary>
app.MapPost("/login", async (HttpContext context) =>
{
    
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
    users.Add(new User(login, password, locationData));

    var claims = new List<Claim> {  new Claim(ClaimTypes.Name, login)};
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    await context.SignInAsync(  CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity));
    
    return Results.Accepted();
});

app.MapGet("/logout", async (HttpContext context) =>
{
    if(context.User.FindFirst(ClaimTypes.Name)!=null)
    users.RemoveAll(user => user.Login==context.User.FindFirst(ClaimTypes.Name).ToString());
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});


// Person maps
app.MapGet("api/people", [Authorize] async () => taskHandler.GetData("api/people").Result);
app.MapDelete("api/people/{id}", [Authorize] async(int id) =>
{
    taskHandler.DeletePerson(id);
});
app.MapPost("api/people/createperson", [Authorize] async (HttpContext context) => ExecutePost(context));
app.MapPost("api/people/editperson", [Authorize] async (HttpContext context) => ExecutePost(context));


// Meet maps
app.MapGet("api/meets", [Authorize] async () => (taskHandler.GetData("api/meets").Result));
app.MapDelete("api/meets/{id}", [Authorize] async (int id) =>
{
    taskHandler.DeleteMeet(id);
});
app.MapPost("api/meets/createmeet", [Authorize] async (HttpContext context) => ExecutePost(context));
app.MapPost("api/meets/editmeet", [Authorize] async (HttpContext context) => ExecutePost(context));


// History maps
app.MapGet("api/histories", [Authorize] async () => (taskHandler.GetData("api/histories").Result));
app.MapDelete("api/histories/{id}", [Authorize] async (int id) =>
{
    taskHandler.DeleteHistory(id);
});
app.MapDelete("api/histories/clear", [Authorize] async () =>
{
    taskHandler.ClearHistory();
});

#endregion

/// <summary>
/// Directs POST method using JSON format to TaskHandlerController.
/// </summary>
async void ExecutePost(HttpContext context)
{
    var jsonString = new StreamReader(context.Request.Body).ReadToEndAsync().Result;
        switch (context.Request.Path)
        {
            case "/api/meets/createmeet":
                taskHandler.CreateMeet(jsonString);
                break;
            case "/api/people/createperson":
                taskHandler.CreatePerson(jsonString);
                break;
            case "/api/people/editperson":
                taskHandler.EditPerson(jsonString);
                break;
            case "/api/meets/editmeet":
                taskHandler.EditMeet(jsonString);
                break;
        }
    //}
}


#region AppBuilder
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHttpLogging();

app.UseRouting();
app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();

#endregion

app.Run();

