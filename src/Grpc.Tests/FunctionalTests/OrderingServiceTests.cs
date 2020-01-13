using Grpc.Core;
using Grpc.Server;
using Grpc.Server.GrpcServices;
using Grpc.Tests.FunctionalTests.Helpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static Grpc.Server.GrpcServices.Ordering;

namespace Grpc.Tests.FunctionalTests
{
    public class OrderingServiceTests : FunctionalTestBase
    {
        public OrderingServiceTests(GrpcServerFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public void UnauthorizedCreateOrderTest()
        {
            // Arrange
            var client = new OrderingClient(_channel);

            // Assert
            Assert.Throws<RpcException>(() =>
            {
                try
                {
                    // Act
                    var response = client.CreateOrder(new Order());
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
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
        public async Task AuthorizedCreateOrderForEmptyOrderTest()
        {
            // Arrange
            var client = new OrderingClient(_channel);

            var token = await GetToken();
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");

            // Assert
            Assert.Throws<RpcException>(() =>
            {
                try
                {
                    // Act
                    var response = client.CreateOrder(new Order(), headers: headers);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
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
        public async Task AuthorizedCreateOrderTest()
        {
            // Arrange
            var client = new OrderingClient(_channel);

            var token = await GetToken();
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = client.CreateOrder(GetOrder, headers: headers);

            // Assert
            Assert.True(response.Success);
            Assert.Contains("Added order as successfull", response.Message);
        }

        [Fact]
        public async Task GetTokenTest()
        {
            // Arrange
            var token = await GetToken();

            // Assert
            Assert.NotNull(token);
        }

        #region Helpers

        private async Task<string> GetToken()
        {
            var userName = "abc";
            var password = "123";

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Address}/auth/token?username={userName}&password={password}"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0)
            };
            var tokenResponse = await _client.SendAsync(request);
            tokenResponse.EnsureSuccessStatusCode();

            var token = await tokenResponse.Content.ReadAsStringAsync();

            return token;
        }

        private Order GetOrder
        {
            get
            {
                var order = new Order
                {
                    CustomerId = 1,
                    TotalPrice = 45.00
                };
                order.OrderItems.Add(new OrderItem
                {
                    Price = 7.95,
                    Stock = 3,
                    VendorProductId = 4
                });
                order.OrderItems.Add(new OrderItem
                {
                    Price = 12.00,
                    Stock = 5,
                    VendorProductId = 1
                });
                order.OrderItems.Add(new OrderItem
                {
                    Price = 10.05,
                    Stock = 2,
                    VendorProductId = 3
                });
                order.OrderItems.Add(new OrderItem
                {
                    Price = 15.00,
                    Stock = 1,
                    VendorProductId = 2
                });

                return order;
            }
        }

        #endregion
    }
}
