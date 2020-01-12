using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Server.Data;
using Microsoft.AspNetCore.Authorization;
using static Grpc.Server.GrpcServices.Ordering;

namespace Grpc.Server.GrpcServices
{
    public class OrderingService : OrderingBase
    {
        private readonly IRepository<OrderEntity> repository;
        public OrderingService(IRepository<OrderEntity> repository)
        {
            this.repository = repository;
        }

        [Authorize()]
        public override Task<CreateOrderResponse> CreateOrder(Order order, ServerCallContext context)
        {
            if(order == null)
            {
                context.Status = new Status(StatusCode.InvalidArgument, "order not be null");
                return Task.FromResult(new CreateOrderResponse { Success = false, Message = "order not be null" });
            }

            if (!order.OrderItems.Any())
            {
                context.Status = new Status(StatusCode.InvalidArgument, "OrderItems not exists");
                return Task.FromResult(new CreateOrderResponse { Success = false, Message = "OrderItems not exists" });
            }

            var orederEntity = new OrderEntity
            {
                CustomerId = order.CustomerId,
                TotalPrice = order.TotalPrice,
                OrderItems = order.OrderItems.Select(x => new OrderItemEntity
                {
                    Price = x.Price,
                    Stock = x.Stock,
                    VendorProductId = x.VendorProductId
                }).ToList()
            };
            repository.Table.Add(orederEntity);

            context.Status = new Status(StatusCode.OK, string.Empty);
            return Task.FromResult(new CreateOrderResponse { Success = true, Message="Added order as successfully" });
        }
    }
}
