using System;

namespace DocumentManagment.DataAccess.Models
{
    public class Document
    {
        public Document()
        {
        }

        public Document(string userId, string name, int fileSize)
        {
            this.Id = Guid.NewGuid().ToString();
            this.UserId = userId;
            this.Name = name;
            this.FileSize = fileSize;
        }

        public string UserId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public int FileSize { get; set; }
    }
}
