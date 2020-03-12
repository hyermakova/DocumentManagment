using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DocumentManagment.Web.Handlers
{
    public interface IExceptionHandler
    {
        Task Handle(HttpContext context, Exception exception);
    }
}
