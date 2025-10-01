using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Core.Exceptions
{
    public static class ErrorCodes
    {
        // General
        public const string UNEXPECTED_ERROR = "UNEXPECTED_ERROR";
        public const string VALIDATION_ERROR = "VALIDATION_ERROR";
        public const string NOT_FOUND = "NOT_FOUND";
        public const string ACCESS_DENIED = "ACCESS_DENIED";

        // Business
        public const string BUSINESS_RULE_VIOLATION = "BUSINESS_RULE_VIOLATION";
        public const string DATA_INTEGRITY_VIOLATION = "DATA_INTEGRITY_VIOLATION";
        public const string INVENTORY_ERROR = "INVENTORY_ERROR";

        // Products
        public const string PRODUCT_SKU_DUPLICATE = "PRODUCT_SKU_DUPLICATE";
        public const string PRODUCT_HAS_INVENTORY = "PRODUCT_HAS_INVENTORY";
        public const string PRODUCT_LOW_STOCK = "PRODUCT_LOW_STOCK";
        public const string PRODUCT_STOCK_LEVEL_INVALID = "PRODUCT_STOCK_LEVEL_INVALID";

        // Categories
        public const string CATEGORY_HAS_PRODUCTS = "CATEGORY_HAS_PRODUCTS";

        // Suppliers
        public const string SUPPLIER_HAS_PRODUCTS = "SUPPLIER_HAS_PRODUCTS";

        // Inventory
        public const string INSUFFICIENT_STOCK = "INSUFFICIENT_STOCK";
        public const string INVENTORY_NOT_FOUND = "INVENTORY_NOT_FOUND";
    }
}
