namespace PeopleJournalWeb.Service.Logging
{
    public class RequestLoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            finally
            {
                _logger.LogInformation( "Time {time} Client: {adress} Request {method} {url} => StatusCode {statusCode}",
                                        DateTime.Now.ToString(),
                                        httpContext.Connection?.RemoteIpAddress,
                                        httpContext.Request?.Method,
                                        httpContext.Request?.Path.Value,
                                        httpContext.Response?.StatusCode);
            }
        }
    }
}
