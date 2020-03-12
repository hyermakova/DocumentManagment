using DocumentManagment.DataAccess.Models;
using DocumentManagment.Services;
using DocumentManagment.Web.Filters;
using DocumentManagment.Web.Mappers;
using DocumentManagment.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagment.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService documentService;
        private readonly DocumentMapper mapper;
        private readonly ILogger<DocumentController> logger;

        public DocumentController(
            IDocumentService documentService,
            DocumentMapper mapper,
            ILogger<DocumentController> logger)
        {
            this.documentService = documentService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [Route("list/{order?}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentResponseModel>>> Get(Order criteria)
        {
            var documents = await this.documentService
                .GetDocumentsAsync(new OrderCriteria(criteria.Field, criteria.IsDesc));
            return this.Ok(documents.Select(this.mapper.Map));
        }

        [HttpGet("{location}")]
        public async Task<ActionResult> Download(string location)
        {
            var document = await this.documentService.GetDocumentAsync(location);
            if (document == null)
            {
                var message = $"Document info by location: {location} does not exist.";
                this.logger.LogTrace(message);

                return this.NotFound(message);
            }

            var stream = await this.documentService.LoadDocumentStreamAsync(location);
            if (stream == null)
            {
                var message = $"Document data by location: {location} does not exist.";
                this.logger.LogTrace(message);

                return this.NotFound();
            }

            return this.File(stream, "application/octet-stream", document.Name);
        }

        [HttpPost]
        [FileValidationFilter]
        public async Task<ActionResult<DocumentResponseModel>> Post(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var document = await this.documentService.SaveDocumentAsync(stream, file.FileName, (int)file.Length);
                var response = this.mapper.Map(document);
                return this.Ok(response);
            }
        }

        [HttpDelete("{location}")]
        public async Task<ActionResult<DocumentResponseModel>> Delete(string location)
        {
            var isDeleted = await this.documentService.DeleteDocumentAsync(location);
            if (!isDeleted)
            {
                var message = $"Document by for location: {location} does not exist.";
                this.logger.LogTrace(message);

                return this.NotFound(message);
            }

            return this.Ok($"Document by location: {location} was successfuly deleted.");
        }
    }
}
