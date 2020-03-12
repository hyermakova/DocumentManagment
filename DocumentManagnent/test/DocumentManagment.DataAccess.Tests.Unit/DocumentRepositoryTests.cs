using AutoFixture;
using DocumentManagment.DataAccess.Models;
using DocumentManagment.DataAccess.Repository;
using Microsoft.Azure.Cosmos;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DocumentManagment.DataAccess.Tests.Unit
{
    public class DocumentRepositoryTests
    {
        private readonly DocumentRepository _documentRepository;
        private readonly Mock<CosmosClient> _cosmosClientMock;
        private readonly Mock<Container> _containerMock;
        private readonly Mock<ItemResponse<Document>> _responseMock;

        public DocumentRepositoryTests()
        {
            _cosmosClientMock = new Mock<CosmosClient>(MockBehavior.Strict);
            _containerMock = new Mock<Container>();
            _documentRepository = new DocumentRepository(_cosmosClientMock.Object);
            _responseMock = new Mock<ItemResponse<Document>>();
        }

        [Fact]
        public async Task GetDocumentTask_ReturnsCorrectResult()
        {
            // Arrange
            var fixture = new Fixture();
            var userId = fixture.Create<string>();
            var id = fixture.Create<string>();
            var size = fixture.Create<int>();

            var expectedDocument = fixture.Build<Document>()
                .With(x => x.UserId, userId)
                .With(x => x.Id, id)
                .With(x => x.FileSize, size)
                .Create();

            _responseMock.Setup(p => p.Resource).Returns(expectedDocument);

            _containerMock.Setup(m =>
                    m.ReadItemAsync<Document>(id, new PartitionKey(userId), null, default(CancellationToken)))
                .Returns(Task.FromResult(_responseMock.Object));

            _cosmosClientMock.Setup(
                    x => x.GetContainer("document-managment", "documents"))
                .Returns(_containerMock.Object);

            //Act
            var actualResult = await _documentRepository.GetDocumentAsync(userId, id);

            //Assert
            Assert.Equal(userId, actualResult.UserId);
        }
    }
}
