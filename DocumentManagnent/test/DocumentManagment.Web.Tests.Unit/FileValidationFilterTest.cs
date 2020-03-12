using DocumentManagment.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DocumentManagment.Web.Tests.Unit
{
    public class FileValidationFilterTest
    {
        private readonly Mock<IFormFile> _mockFormFile = new Mock<IFormFile>();

        [Fact]
        public void OnActionExecuting_FileDataIsValid_ContextResultIsNull()
        {
            // Arrange
            _mockFormFile.Setup(p => p.ContentType).Returns("application/pdf");
            _mockFormFile.Setup(p => p.Length).Returns(1000);

            var context = CreateActionExecutingContext(_mockFormFile.Object);
            var fileValidationFilter = new FileValidationFilterAttribute();

            // Act
            fileValidationFilter.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }

        [Fact]
        public void OnActionExecuting_FileDataToLarge_ContextResultHasBadRequestObjectResult()
        {
            // Arrange
            _mockFormFile.Setup(p => p.ContentType).Returns("application/pdf");
            _mockFormFile.Setup(p => p.Length).Returns(6 * 1024 * 1024);

            var context = CreateActionExecutingContext(_mockFormFile.Object);
            var fileValidationFilter = new FileValidationFilterAttribute();

            // Act
            fileValidationFilter.OnActionExecuting(context);

            // Assert
            var actual = context.Result as BadRequestObjectResult;
            Assert.NotNull(actual);
        }

        [Fact]
        public void OnActionExecuting_FileContentTypeIsIncorrect_ContextResultHasBadRequestObjectResult()
        {
            // Arrange
            _mockFormFile.Setup(p => p.ContentType).Returns("application/jpeg");
            _mockFormFile.Setup(p => p.Length).Returns(1000);

            var context = CreateActionExecutingContext(_mockFormFile.Object);
            var fileValidationFilter = new FileValidationFilterAttribute();

            // Act
            fileValidationFilter.OnActionExecuting(context);

            // Assert
            var actual = context.Result as BadRequestObjectResult;
            Assert.NotNull(actual);
        }

        private static ActionExecutingContext CreateActionExecutingContext(IFormFile file)
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            };

            var context = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(),
                new Dictionary<string, object>{{"file", file} }, new object());
            return context;
        }
    }
}
