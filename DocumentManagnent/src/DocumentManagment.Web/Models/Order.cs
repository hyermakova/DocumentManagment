using DocumentManagment.Web.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagment.Web.Models
{
    [ModelBinder(typeof(OrderCriteriaModelBinder))]
    public class Order
    {
        public string Field { get; set; } = "Name";

        public bool IsDesc { get; set; } = false;
    }
}
