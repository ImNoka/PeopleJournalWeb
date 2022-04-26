namespace PeopleJournalWeb.Filters
{
    public static class ControllerExceptionFilterExtensions
    {
        public static void UseExtensionExceptionFilter(this IApplicationBuilder app)
        {
            app.UseMiddleware<ControllerExceptionFilter>();
        }
    }
}
