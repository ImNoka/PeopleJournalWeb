using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Diagnostics;

namespace PeopleJournalWeb.Filters
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ControllerExceptionFilter : ExceptionFilterAttribute
    {
        //private readonly RequestDelegate next;
        private readonly string path = Directory.GetCurrentDirectory()+"\\logs\\exception_logs.txt";
        private readonly RequestDelegate _next;

        public ControllerExceptionFilter(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionLoggingAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionLoggingAsync(HttpContext httpContext, Exception ex)
        {
            string? actionName = ex.Source;
            string? stack = ex.StackTrace;
            string? message = ex.Message;

            using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\logs\\exception_logs.txt",true))
            {
                string exceptionData = $"------------{DateTime.Now.ToString()}--------------\n{actionName}\n" +
                    $"{message}\n {stack}";
                writer.WriteLineAsync(exceptionData);
            }
            return httpContext.Response.WriteAsync($"Handle Works.\n Source: {actionName} \n Exception: {message} \n Stack: {stack}");
        }


        public override void OnException(ExceptionContext exceptionContext)
        {
            string? actionName = exceptionContext.ActionDescriptor.DisplayName;
            string? exceptionStack = exceptionContext.Exception.StackTrace;
            string? message = exceptionContext.Exception.Message;
            


            exceptionContext.Result = new ContentResult
            {
                Content = $"{DateTime.Now.ToString()}:{actionName}\n" +
                    $"{message}"
            };

            using (StreamWriter writer = new StreamWriter(path))
            {
                string exceptionData = $"{DateTime.Now.ToString()}:{actionName}\n" +
                    $"{message}";
                writer.WriteLineAsync(exceptionData);
            }
            exceptionContext.ExceptionHandled = true;
            base.OnException(exceptionContext);
        }
    }
}
