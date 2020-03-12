using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentManagment.DataAccess.Storage
{
    public class BlobStorage : IBlobStorage
    {
        private readonly CloudStorageAccount storageAccount;

        public BlobStorage(string connectionString)
        {
            this.storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task<Uri> CreateContainerAsync(string containerName)
        {
            var cloudBlobContainer = this.GetContainerByName(containerName);
            await cloudBlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            return cloudBlobContainer?.Uri;
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            var cloudBlobContainer = this.GetContainerByName(containerName);
            await cloudBlobContainer.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string location)
        {
            var container = this.GetContainerByName(containerName);
            var blob = container.GetBlockBlobReference(location);
            return await blob.DeleteIfExistsAsync();
        }

        public async Task<Uri> CreateBlobFromStreamAsync(string containerName, Stream file, string location)
        {
            var container = this.GetContainerByName(containerName);
            var cloudBlockBlob = container.GetBlockBlobReference(location);
            await cloudBlockBlob.UploadFromStreamAsync(file).ConfigureAwait(false);
            return cloudBlockBlob.Uri;
        }

        public Task<Stream> GetContentOfBlobFromContainerAsync(string containerName, string blobName)
        {
            var container = this.GetContainerByName(containerName);
            var cloudBlockBlob = container.GetBlockBlobReference(blobName);
            return cloudBlockBlob.OpenReadAsync();
        }

        private CloudBlobClient GetBlobClient()
        {
            return this.storageAccount.CreateCloudBlobClient();
        }

        private CloudBlobContainer GetContainerByName(string name)
        {
            var cloudBlobClient = this.GetBlobClient();
            return cloudBlobClient.GetContainerReference(name);
        }
    }
}
