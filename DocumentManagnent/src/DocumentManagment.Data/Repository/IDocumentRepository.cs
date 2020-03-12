using DocumentManagment.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentManagment.DataAccess.Repository
{
    public interface IDocumentRepository
    {
        Task<bool> DocumentExistsAsync(string userId, string id);

        Task<List<Document>> GetDocumentsAsync(string userId, OrderCriteria criteria);

        Task<Document> GetDocumentAsync(string partitionKey, string id);

        Task<Document> CreateItemAsync(Document document, string partitionKey);

        Task<Document> DeleteItemAsync(string id, string partitionKey);
    }
}
