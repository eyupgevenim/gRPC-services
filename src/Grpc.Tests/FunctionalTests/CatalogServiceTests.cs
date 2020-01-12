using Grpc.Core;
using Grpc.Server;
using Grpc.Server.GrpcServices;
using Grpc.Tests.FunctionalTests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Grpc.Tests.FunctionalTests
{
    public class CatalogServiceTests : FunctionalTestBase
    {
        public CatalogServiceTests(GrpcServerFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public void SearchProductByEmptyNameTest()
        {
            // Arrange
            var client = new Catalog.CatalogClient(_channel);

            // Assert
            Assert.ThrowsAsync<RpcException>(async () =>
            {
                try
                {
                    // Act
                    var response = await client.SearchProductByNameAsync(new SearchRequest());
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
                {
                    //test success
                    throw ex;
                }
                catch (Exception ex)
                {
                    //test fail
                    throw new Exception(ex.Message, ex);
                }
            });
        }

        [Fact]
        public async Task SearchProductByNameAsyncTest()
        {
            // Arrange
            var client = new Catalog.CatalogClient(_channel);
            var searchRequest = new SearchRequest
            {
                Name = "product",
                PageIndex = 0,
                PageSize = 10
            };

            // Act
            var response = await client.SearchProductByNameAsync(searchRequest);

            // Assert
            Assert.True(response.Count == 3);
        }

    }
}
