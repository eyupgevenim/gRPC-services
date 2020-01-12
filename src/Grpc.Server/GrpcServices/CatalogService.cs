using Grpc.Core;
using Grpc.Server.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Grpc.Server.GrpcServices.Catalog;

namespace Grpc.Server.GrpcServices
{
    public class CatalogService : CatalogBase
    {
        private readonly IRepository<VendorProductEntity> repository;
        public CatalogService(IRepository<VendorProductEntity> repository)
        {
            this.repository = repository;
        }

        public override Task<PagingResponse> SearchProductByName(SearchRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                context.Status = new Status(StatusCode.NotFound, "Name not be empty");
                return Task.FromResult(new PagingResponse());
            }

            var items = repository.Table.Where(x => x.Product.Name.Contains(request.Name));
            context.Status = new Status(StatusCode.OK, string.Empty);

            return PagingToResponse(items, pageIndex: request.PageIndex, pageSize: request.PageSize);
        }

        private Task<PagingResponse> PagingToResponse(IEnumerable<VendorProductEntity> items, int pageIndex = 0, int pageSize = 20)
        {
            var result = new PagingResponse()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var vendorProducts = items.Skip(pageIndex * pageSize).Take(pageSize).Select(x => new VendorProduct
            {
                Id = x.Id,
                AvailableStock = x.AvailableStock,
                MaxStockThreshold = x.MaxStockThreshold,
                Price = x.Price,
                Vendor = new Vendor
                {
                    Id = x.Vendor.Id,
                    Name = x.Vendor.Name,
                    Description = x.Vendor.Description
                },
                Product = new Product
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    Description = x.Product.Description,
                    PictureUri = x.Product.PictureUri,
                    ProductBrand = new ProductBrand
                    {
                        Id = x.Product.ProductBrand.Id,
                        Name = x.Product.ProductBrand.Name
                    }
                }
            });
            result.Data.AddRange(vendorProducts);
            result.Count = result.Data.Count;

            return Task.FromResult(result);
        }
    }
}
