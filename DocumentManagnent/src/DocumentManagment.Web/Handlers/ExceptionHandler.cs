using DocumentManagment.Web.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DocumentManagment.Web.Filters
{
    public class ExceptionHandler : IExceptionHandler
    {
        private const string DefaultMessage = "Unexpected error occurred.";

        private readonly ILogger<IExceptionHandler> logger;

        public ExceptionHandler(ILogger<IExceptionHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(HttpContext context, Exception exception)
        {
            this.logger.LogError(exception, "Internal Server Error");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return context.Response.WriteAsync(DefaultMessage);
        }
    }
}
