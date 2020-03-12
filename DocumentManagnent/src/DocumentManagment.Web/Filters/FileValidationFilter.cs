using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace DocumentManagment.Web.Filters
{
    public class FileValidationFilterAttribute : Attribute, IActionFilter
    {
        public virtual void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var file = context.ActionArguments["file"] as IFormFile;

            const int sizeLimit = 5 * 1024 * 1024;
            if (!file.ContentType.Equals("application/pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Result = new BadRequestObjectResult("Only pdf files allowed.");
            }

            if (file.Length > sizeLimit)
            {
                context.Result = new BadRequestObjectResult("File size more then 5Mb.");
            }
        }
    }
}
