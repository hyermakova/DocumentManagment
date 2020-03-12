using DocumentManagment.Web.Handlers;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DocumentManagment.Web.Infrastructure
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IExceptionHandler exceptionRequestHandler;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            IExceptionHandler exceptionRequestHandler)
        {
            this.exceptionRequestHandler = exceptionRequestHandler;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this.next(httpContext);
            }
            catch (Exception ex)
            {
                var trackingId = Guid.NewGuid();
                await this.exceptionRequestHandler.Handle(httpContext, ex);
            }
        }
    }
}
