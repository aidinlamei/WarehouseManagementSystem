using AutoMapper;
using WarehouseManagement.Core.DTOs;
using WarehouseManagement.Core.DTOs.Categories;
using WarehouseManagement.Core.DTOs.Inventory;
using WarehouseManagement.Core.DTOs.Products;
using WarehouseManagement.Core.DTOs.PurchaseOrders;
using WarehouseManagement.Core.DTOs.Suppliers;
using WarehouseManagement.Core.DTOs.Transactions;
using WarehouseManagement.Core.DTOs.Warehouses;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product Mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.TotalQuantity, opt => opt.Ignore());

            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Category Mappings
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Category, CategoryWithProductsDto>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();

            // Supplier Mappings
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>();
            CreateMap<UpdateSupplierDto, Supplier>();
            CreateMap<Supplier, SupplierWithProductsDto>();

            // Warehouse Mappings
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<CreateWarehouseDto, Warehouse>();
            CreateMap<UpdateWarehouseDto, Warehouse>();
            CreateMap<Warehouse, WarehouseWithInventoryDto>();

            // Inventory Mappings
            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price));

            // Transaction Mappings
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name));

            // Purchase Order Mappings
            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count));

            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.SKU));
        }
    }
}