using DocumentManagment.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagment.Web.ModelBinders
{
    public class OrderCriteriaModelBinder : IModelBinder
    {
        private const string Separator = ".";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            const string key = "order";
            var value = bindingContext.ValueProvider.GetValue(key).ToString();

            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(new Order());
            }
            else if (value.Contains(Separator))
            {
                var values = value.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                bindingContext.Result = ModelBindingResult.Success(new Order
                {
                    Field = values[0],
                    IsDesc = values[1].Equals("desc", StringComparison.InvariantCultureIgnoreCase)
                });
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
