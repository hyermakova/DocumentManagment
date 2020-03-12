using DocumentManagment.DataAccess.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocumentManagment.Domain.Model;

namespace DocumentManagment.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetDocumentsAsync(OrderCriteria criteria);

        Task<Document> SaveDocumentAsync(Stream file, string fileName, int size);

        Task<Document> GetDocumentAsync(string id);

        Task<Stream> LoadDocumentStreamAsync(string location);

        Task<bool> DeleteDocumentAsync(string id);
    }
}
