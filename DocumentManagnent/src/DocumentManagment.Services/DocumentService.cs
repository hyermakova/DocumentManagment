using DocumentManagment.DataAccess.Models;
using DocumentManagment.DataAccess.Repository;
using DocumentManagment.DataAccess.Storage;
using DocumentManagment.Services.Providers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentManagment.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IBlobStorage blobStorage;
        private readonly IDocumentRepository documentRepository;
        private readonly IUserProvider userProvider;

        public DocumentService(
            IBlobStorage blobStorage,
            IDocumentRepository documentRepository,
            IUserProvider userProvider)
        {
            this.blobStorage = blobStorage;
            this.documentRepository = documentRepository;
            this.userProvider = userProvider;
        }

        public async Task<IEnumerable<Document>> GetDocumentsAsync(OrderCriteria criteria)
        {
            var userId = this.userProvider.User.Id;
            var documents = await this.documentRepository.GetDocumentsAsync(userId, criteria);
            
            return documents;
        }

        public async Task<Document> SaveDocumentAsync(Stream file, string fileName, int size)
        {
            var userId = this.userProvider.User.Id;
            var doc = new Document(userId, fileName, size);

            await this.blobStorage.CreateContainerAsync(userId);
            await this.blobStorage.CreateBlobFromStreamAsync(userId, file, doc.Id);

            var result = await this.documentRepository.CreateItemAsync(doc, userId);

            return result;
        }

        public async Task<Document> GetDocumentAsync(string id)
        {
            var userId = this.userProvider.User.Id;
            var document = await this.documentRepository.GetDocumentAsync(userId, id);
            return document;
        }

        public Task<Stream> LoadDocumentStreamAsync(string location)
        {
            var userId = this.userProvider.User.Id;
            return this.blobStorage.GetContentOfBlobFromContainerAsync(userId, location);
        }

        public async Task<bool> DeleteDocumentAsync(string id)
        {
            var userId = this.userProvider.User.Id;
            var isDocumentExist = await this.documentRepository.DocumentExistsAsync(userId, id);

            if (!isDocumentExist)
            {
                return false;
            }

            await this.documentRepository.DeleteItemAsync(id, userId);
            await this.blobStorage.DeleteBlobAsync(userId, id);

            return true;
        }
    }
}