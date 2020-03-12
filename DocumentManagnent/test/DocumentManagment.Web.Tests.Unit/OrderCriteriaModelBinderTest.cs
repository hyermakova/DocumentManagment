using DocumentManagment.Web.ModelBinders;
using DocumentManagment.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DocumentManagment.Web.Tests.Unit
{
    public class OrderCriteriaModelBinderTest
    {
        private readonly Mock<IValueProvider> _mockValueProvider = new Mock<IValueProvider>();

        [Fact]
        public async Task BindModelAsync_DataFormatIsValid_ContextResultModelIsOrder()
        {
            // Arrange
            _mockValueProvider.Setup(m => m.GetValue("order")).Returns(new ValueProviderResult("Location.desc"));

            var context = CreateModelBindingContext(_mockValueProvider.Object);
            var binder = new OrderCriteriaModelBinder();

            // Act
            await binder.BindModelAsync(context);

            // Assert
            var resultModel = context.Result.Model as Order;

            Assert.Equal("Location", resultModel?.Field);
            Assert.Equal(true, resultModel?.IsDesc);
        }

        [Fact]
        public async Task BindModelAsync_DataFormatIsEmpty_ContextResultModelHasDefaulOrder()
        {
            // Arrange
            _mockValueProvider.Setup(m => m.GetValue("order")).Returns(new ValueProviderResult(""));

            var context = CreateModelBindingContext(_mockValueProvider.Object);
            var binder = new OrderCriteriaModelBinder();

            // Act
            await binder.BindModelAsync(context);

            // Assert
            var resultModel = context.Result.Model as Order;

            Assert.Equal("Name", resultModel?.Field);
            Assert.Equal(false, resultModel?.IsDesc);
        }

        [Fact]
        public async Task BindModelAsync_DataFormatIsInvalid_ContextResultModelIsNull()
        {
            // Arrange
            _mockValueProvider.Setup(m => m.GetValue("order")).Returns(new ValueProviderResult("size"));

            var context = CreateModelBindingContext(_mockValueProvider.Object);
            var binder = new OrderCriteriaModelBinder();

            // Act
            await binder.BindModelAsync(context);

            // Assert
            var resultModel = context.Result.Model as Order;
            Assert.Null(resultModel);
        }

        private static ModelBindingContext CreateModelBindingContext(IValueProvider valueProvider)
        {
            return new DefaultModelBindingContext {ValueProvider = valueProvider};
        }
    }
}
