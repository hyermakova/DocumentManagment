using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentManagment.DataAccess.Storage
{
    public interface IBlobStorage
    {
        Task<Uri> CreateContainerAsync(string containerName);

        Task DeleteContainerAsync(string containerName);

        Task<bool> DeleteBlobAsync(string containerName, string location);

        Task<Uri> CreateBlobFromStreamAsync(string containerName, Stream file, string location);

        Task<Stream> GetContentOfBlobFromContainerAsync(string containerName, string blobName);
    }
}
