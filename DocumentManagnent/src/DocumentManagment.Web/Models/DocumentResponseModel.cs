namespace DocumentManagment.Web.Models
{
    public class DocumentResponseModel
    {
        public DocumentResponseModel(string name, string location, long fileSize)
        {
            this.Name = name;
            this.Location = location;
            this.FileSize = fileSize;
        }

        public string Name { get; set; }

        public string Location { get; set; }

        public long FileSize { get; set; }
    }
}
