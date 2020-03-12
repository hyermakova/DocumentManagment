using Microsoft.Azure.Cosmos;

namespace DocumentManagment.DataAccess.Repository
{
    public abstract class BaseRepository
    {
        protected const string DatabaseId = "document-managment";

        protected BaseRepository(CosmosClient client)
        {
            this.Client = client;
        }

        protected abstract string CollectionId { get; }

        protected CosmosClient Client { get; }
    }
}