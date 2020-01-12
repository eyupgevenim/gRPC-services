using Grpc.Server.GrpcServices;
using System.Threading.Tasks;
using Xunit;
using Grpc.Tests.UnitTests.Helpers;
using Grpc.Server.Data;
using Moq;
using System.Linq;

namespace Grpc.Tests.UnitTests
{
    public class CatalogServiceTests
    {
        private readonly CatalogService catalogService;
        public CatalogServiceTests()
        {
            // Arrange
            var repository = new Mock<IRepository<VendorProductEntity>>();
            repository.Setup(x => x.Table).Returns(DbData.VendorProductData);
            catalogService = new CatalogService(repository.Object);
        }

        [Fact]
        public async Task SearchProductByEmptyNameTest()
        {
            // Act
            var response = await catalogService.SearchProductByName(new SearchRequest(), TestServerCallContext.Create());

            // Assert
            Assert.True(response.Count == 0);
            Assert.False(response.Data.Any());
        }

        [Fact]
        public async Task SearchProductByNameTest()
        {
            // Arrange
            var searchRequest = new SearchRequest
            {
                Name = "product",
                PageIndex = 0,
                PageSize = 10
            };

            // Act
            var response = await catalogService.SearchProductByName(searchRequest, TestServerCallContext.Create());

            // Assert
            Assert.True(response.Count == 3);
        }
    }
}
