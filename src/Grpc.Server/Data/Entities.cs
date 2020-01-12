using System.Collections.Generic;

namespace Grpc.Server.Data
{
    public class VendorEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class VendorProductEntity
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int AvailableStock { get; set; }
        public int MaxStockThreshold { get; set; }
        public VendorEntity Vendor { get; set; }
        public ProductEntity Product { get; set; }
    }
    public class ProductBrandEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ProductEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUri { get; set; }
        public ProductBrandEntity ProductBrand { get; set; }
    }

    public class OrderEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double TotalPrice { get; set; }
        public ICollection<OrderItemEntity> OrderItems { get; set; }
    }
    public class OrderItemEntity
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public int VendorProductId { get; set; }
    }
}
