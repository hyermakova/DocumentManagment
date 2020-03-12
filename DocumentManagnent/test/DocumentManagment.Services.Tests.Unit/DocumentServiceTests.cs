using AutoFixture;
using DocumentManagment.DataAccess.Models;
using DocumentManagment.DataAccess.Repository;
using DocumentManagment.DataAccess.Storage;
using DocumentManagment.Domain.Model;
using DocumentManagment.Services.Providers;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DocumentManagment.Services.Tests.Unit
{
    public class DocumentServiceTests
    {
        private readonly Mock<IBlobStorage> _blobStorageMock;
        private readonly Mock<IDocumentRepository> _documentRepositoryMock;
        private readonly Mock<IUserProvider> _userProviderMock;

        public DocumentServiceTests()
        {
            _blobStorageMock = new Mock<IBlobStorage>();
            _documentRepositoryMock = new Mock<IDocumentRepository>();
            _userProviderMock = new Mock<IUserProvider>();
        }

        [Fact]
        public async Task GetDocumentAsync_ReturnsCorrectResult()
        {
            //Assert
            var expectedDocument = CreateDocument();

            _userProviderMock.Setup(p => p.User).Returns(new User { Id = expectedDocument.UserId });

            _documentRepositoryMock.Setup(m => m.GetDocumentAsync(expectedDocument.UserId, expectedDocument.Id))
                .Returns(Task.FromResult(expectedDocument));
            var documentService = CreateService();

            //Act
            var actualDocument = await documentService.GetDocumentAsync(expectedDocument.Id);

            //Arrange
            Assert.Equal(expectedDocument.Id, actualDocument.Id);
        }

        [Fact]
        public async Task SaveDocumentAsync_ReturnsCorrectResult()
        {
            //Assert
            var expectedDocument = CreateDocument();

            _userProviderMock.Setup(p => p.User).Returns(new User { Id = expectedDocument.UserId });

            _blobStorageMock.Setup(m => m.CreateContainerAsync(expectedDocument.UserId));
            _blobStorageMock.Setup(m =>
                m.CreateBlobFromStreamAsync(It.IsAny<string>(), It.IsAny<Stream>(), expectedDocument.Id));

            _documentRepositoryMock.Setup(m => m.CreateItemAsync(It.IsAny<Document>(), expectedDocument.UserId))
                .Returns(Task.FromResult(expectedDocument));

            var documentService = CreateService();

            //Act
            var actualDocument =
                await documentService.SaveDocumentAsync(null, expectedDocument.Name, expectedDocument.FileSize);

            //Arrange
            Assert.Equal(expectedDocument.Id, actualDocument.Id);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsCorrectResult_WhenDocumentExists()
        {
            //Assert
            var expectedDocument = CreateDocument();

            _userProviderMock.Setup(p => p.User).Returns(new User { Id = expectedDocument.UserId });

            _blobStorageMock.Setup(m => m.DeleteBlobAsync(It.IsAny<string>(), expectedDocument.Id));

            _documentRepositoryMock.Setup(m => m.DeleteItemAsync(expectedDocument.Id, expectedDocument.UserId));
            _documentRepositoryMock.Setup(m => m.DocumentExistsAsync(expectedDocument.UserId, expectedDocument.Id))
                .Returns(Task.FromResult(true));
            var documentService = CreateService();

            //Act
            var actualResult =
                await documentService.DeleteDocumentAsync(expectedDocument.Id);

            //Arrange
            Assert.True(actualResult);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsCorrectResult_WhenDocumentDoseNotExists()
        {
            //Assert
            var expectedDocument = CreateDocument();

            _userProviderMock.Setup(p => p.User).Returns(new User { Id = expectedDocument.UserId });

            _blobStorageMock.Setup(m => m.DeleteBlobAsync(It.IsAny<string>(), expectedDocument.Id));

            _documentRepositoryMock.Setup(m => m.DeleteItemAsync(expectedDocument.Id, expectedDocument.UserId));
            _documentRepositoryMock.Setup(m => m.DocumentExistsAsync(expectedDocument.UserId, expectedDocument.Id))
                .Returns(Task.FromResult(true));
            var documentService = CreateService();

            //Act
            var actualResult =
                await documentService.DeleteDocumentAsync(Guid.NewGuid().ToString());

            //Arrange
            Assert.False(actualResult);
        }

        private static Document CreateDocument()
        {
            var fixture = new Fixture();
            var userId = fixture.Create<string>();
            var id = fixture.Create<string>();
            var size = fixture.Create<int>();

            return fixture.Build<Document>()
                .With(x => x.UserId, userId)
                .With(x => x.Id, id)
                .With(x => x.FileSize, size)
                .Create();
        }

        private IDocumentService CreateService()
        {
            return new DocumentService(_blobStorageMock.Object, _documentRepositoryMock.Object,
                _userProviderMock.Object);
        }
    }
}
