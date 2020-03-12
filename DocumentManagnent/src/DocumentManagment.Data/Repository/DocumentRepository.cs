using DocumentManagment.DataAccess.Extensions;
using DocumentManagment.DataAccess.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Linq;

namespace DocumentManagment.DataAccess.Repository
{
    public class DocumentRepository : BaseRepository, IDocumentRepository
    {
        public DocumentRepository(CosmosClient cosmosClient)
            : base(cosmosClient)
        {
        }

        protected override string CollectionId => "documents";

        public async Task<bool> DocumentExistsAsync(string userId, string id)
        {
            var container = this.Client.GetContainer(DatabaseId, this.CollectionId);

            var query = container.GetItemLinqQueryable<Document>()
                .Where(c => c.UserId == userId &&
                            c.Id == id);

            var results = await this.ExecuteQueryAsync(query);

            return results.Any();
        }

        public async Task<List<Document>> GetDocumentsAsync(string userId, OrderCriteria criteria)
        {
            var container = this.Client.GetContainer(DatabaseId, this.CollectionId);

            var query = container.GetItemLinqQueryable<Document>(true)
                .Where(c => c.UserId == userId)
                .SortBy(criteria.Field, criteria.IsDesc);

            var results = await this.ExecuteQueryAsync(query);

            return results;
        }

        public async Task<Document> GetDocumentAsync(string partitionKey, string id)
        {
            var container = this.Client.GetContainer(DatabaseId, this.CollectionId);

            var result = await container.ReadItemAsync<Document>(id, new PartitionKey(partitionKey));

            return result.Resource;
        }

        public async Task<Document> CreateItemAsync(Document document, string partitionKey)
        {
            var container = this.Client.GetContainer(DatabaseId, this.CollectionId);

            var result = await container.CreateItemAsync(document, new PartitionKey(partitionKey));

            return result.Resource;
        }

        public async Task<Document> DeleteItemAsync(string id, string partitionKey)
        {
            var container = this.Client.GetContainer(DatabaseId, this.CollectionId);

            var result = await container.DeleteItemAsync<Document>(id, new PartitionKey(partitionKey));

            return result.Resource;
        }

        protected async Task<List<TDocument>> ExecuteQueryAsync<TDocument>(IQueryable<TDocument> query)
        {
            var documents = new List<TDocument>();

            var feedIterator = query.ToFeedIterator();

            while (feedIterator.HasMoreResults)
            {
                var nextResults = await feedIterator.ReadNextAsync();
                documents.AddRange(nextResults);
            }

            return documents;
        }
    }
}