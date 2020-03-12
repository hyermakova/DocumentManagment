using DocumentManagment.DataAccess.Models;
using DocumentManagment.Web.Models;

namespace DocumentManagment.Web.Mappers
{
    public class DocumentMapper
    {
        public DocumentResponseModel Map(Document document)
        {
            return new DocumentResponseModel(document.Name, document.Id, document.FileSize);
        }
    }
}
