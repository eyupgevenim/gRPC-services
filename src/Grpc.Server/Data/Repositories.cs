using System.Collections.Generic;
using System.Linq;

namespace Grpc.Server.Data
{
    public interface IRepository<T> where T : class
    {
        ICollection<T> Table { get; }
    }
    public class Repository<T> : IRepository<T> where T : class
    {
        public ICollection<T> Table { get; }
        public Repository(ICollection<T> table)
        {
            Table = table;
        }
    }
    public static class DbData
    { 
        public static ICollection<VendorProductEntity> VendorProductData
        {
            get
            {
                return new List<VendorProductEntity>
                {
                    new VendorProductEntity
                    {
                        Id = 1,
                        Price = 12.5,
                        AvailableStock = 5,
                        MaxStockThreshold = 1,
                        Vendor = VendorData.FirstOrDefault(x=>x.Id == 1),
                        Product = ProductData.FirstOrDefault(x=>x.Id == 1)
                    },
                    new VendorProductEntity
                    {
                        Id = 2,
                        Price = 9.80,
                        AvailableStock = 20,
                        MaxStockThreshold = 5,
                        Vendor = VendorData.FirstOrDefault(x=>x.Id == 2),
                        Product = ProductData.FirstOrDefault(x=>x.Id == 2)
                    },
                    new VendorProductEntity
                    {
                        Id = 3,
                        Price = 56.10,
                        AvailableStock = 14,
                        MaxStockThreshold = 1,
                        Vendor = VendorData.FirstOrDefault(x=>x.Id == 2),
                        Product = ProductData.FirstOrDefault(x=>x.Id == 3)
                    }
                };
            }
        }
        public static ICollection<VendorEntity> VendorData
        {
            get
            {
                return new List<VendorEntity>
                {
                    new VendorEntity
                    {
                        Id = 1,
                        Name = "vendor 1",
                        Description = "vendor 1 description"
                    },
                    new VendorEntity
                    {
                        Id = 2,
                        Name = "vendor 2",
                        Description = "vendor 2 description"
                    }
                };
            }
        }
        public static ICollection<ProductEntity> ProductData
        {
            get
            {
                return new List<ProductEntity>
                {
                    new ProductEntity
                    {
                        Id = 1,
                        Name = "product 1",
                        Description = "product 1 description",
                        PictureUri = "picture_uri_1",
                        ProductBrand = ProductBrandData.FirstOrDefault(x=>x.Id == 1)
                    },
                    new ProductEntity
                    {
                        Id = 2,
                        Name = "product 2",
                        Description = "product 2 description",
                        PictureUri = "picture_uri_2",
                        ProductBrand = ProductBrandData.FirstOrDefault(x=>x.Id == 1)
                    },
                    new ProductEntity
                    {
                        Id = 3,
                        Name = "product 3",
                        Description = "product 3 description",
                        PictureUri = "picture_uri_3",
                        ProductBrand = ProductBrandData.FirstOrDefault(x=>x.Id == 2)
                    }
                };
            }
        }
        public static ICollection<ProductBrandEntity> ProductBrandData
        {
            get
            {
                return new List<ProductBrandEntity>
                {
                    new ProductBrandEntity
                    {
                        Id = 1,
                        Name = "brand name 1"
                    },
                    new ProductBrandEntity
                    {
                        Id = 2,
                        Name = "brand name 2"
                    },
                    new ProductBrandEntity
                    {
                        Id = 3,
                        Name = "brand name 3"
                    }
                };
            }
        }
    }
}
