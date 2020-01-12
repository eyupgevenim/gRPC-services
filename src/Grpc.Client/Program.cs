using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Server.GrpcServices;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Grpc.Server.GrpcServices.Catalog;
using static Grpc.Server.GrpcServices.Ordering;

namespace Grpc.Client
{
    class Program
    {
        private const string Address = "https://localhost:5001";

        static void Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress(Address);

            SearchProduct(channel);

            Console.WriteLine($"===================================================================");
            UnauthorizedCreateOrder(channel);

            Console.WriteLine($"====================================================================");
            AuthorizatedCreateOrderWithToken(channel, GetToken().Result);

            Console.ReadKey();
        }

        private static void SearchProduct(GrpcChannel channel)
        {
            Console.WriteLine($"*** SearchProduct() ***");
            var client = new CatalogClient(channel);

            SearchRequest searchRequest;
            PagingResponse response;

            Console.WriteLine($"*** 1 - Search : empty name");
            try
            {
                searchRequest = new SearchRequest();
                response = client.SearchProductByName(searchRequest);
            }
            catch(RpcException ex)
            {
                Console.WriteLine($"StatusCode:{ex.StatusCode}|Status.StatusCode:{ex.Status.StatusCode}|Status.Detail:{ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }

            System.Threading.Thread.Sleep(2000);

            Console.WriteLine($"*** 2 - Search : by name as 'product'");
            searchRequest = new SearchRequest
            {
                Name = "product",
                PageIndex = 0,
                PageSize = 10
            };
            response = client.SearchProductByName(searchRequest);

            foreach (var item in response.Data)
            {
                var vp = item;
                Console.WriteLine($" vp => Id={vp.Id}|Price:{vp.Price}|AvailableStock:{vp.AvailableStock}|MaxStockThreshold:{vp.MaxStockThreshold}");

                var p = item.Product;
                Console.WriteLine($"\t p => Id={p.Id}|Name:{p.Name}|Description:{p.Description}|PictureUri:{p.PictureUri}");

                var pb = item.Product.ProductBrand;
                Console.WriteLine($"\t\t pb => Id={pb.Id}|Name:{pb.Name}");

                var v = item.Vendor;
                Console.WriteLine($"\t v => Id={v.Id}|Name:{v.Name}|Description:{v.Description}");

                Console.WriteLine($"======================================================================================================================");
            }

            System.Threading.Thread.Sleep(2000);
        }

        private static void UnauthorizedCreateOrder(GrpcChannel channel)
        {
            Console.WriteLine($"*** UnauthorizedCreateOrder() ***");
            var client = new OrderingClient(channel);

            try
            {
                Console.WriteLine($"*** 1 - CreateOrder(new Order())");
                client.CreateOrder(new Order());
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"ex|Status.StatusCode:{ex.Status.StatusCode}|Status.Detail:{ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }

            System.Threading.Thread.Sleep(2000);
        }

        private static void AuthorizatedCreateOrderWithToken(GrpcChannel channel, string token)
        {
            Console.WriteLine($"*** AuthorizatedCreateOrderWithToken() ***");
            var client = new OrderingClient(channel);
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");

            try
            {
                Console.WriteLine($"*** 1 - CreateOrder(new Order())");
                client.CreateOrder(new Order(), headers: headers);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"StatusCode:{ex.StatusCode}|Status.StatusCode:{ex.Status.StatusCode}|Status.Detail:{ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message:{ex.Message}");
            }

            System.Threading.Thread.Sleep(2000);
            Console.WriteLine($"*** 2 - CreateOrder(order)");

            var response = client.CreateOrder(GetOrder, headers: headers);
            Console.WriteLine($"Success:{response.Success} | Message:{response.Message}");

            System.Threading.Thread.Sleep(2000);
        }

        private static async Task<string> GetToken()
        {
            var userName = "abc";
            var password = "123";
            Console.WriteLine($"Authenticating as userName:{userName} | password:{password} |...");

            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Address}/auth/token?username={userName}&password={password}"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0)
            };
            var tokenResponse = await httpClient.SendAsync(request);
            tokenResponse.EnsureSuccessStatusCode();

            var token = await tokenResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Successfully authenticated.");

            return token;
        }

        private static Order GetOrder
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
    }
}
