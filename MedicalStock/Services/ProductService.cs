using MedicalStock.Data;
using MedicalStock.Models;
using MedicalStock.Exceptions;

namespace MedicalStock.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;


        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public Product CreateProduct(string name, string barcode, string manufacturer, decimal price, int minimumStock, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new InvalidProductNameException(name);
            if (string.IsNullOrWhiteSpace(barcode))
                throw new InvalidProductBarcodeException(barcode);
            if (string.IsNullOrWhiteSpace(manufacturer))
                throw new InvalidProductManufacturerException(manufacturer);
            if (price <= 0)
                throw new InvalidProductPriceException(price);
            if (minimumStock < 0)
                throw new InvalidProductMinimumStockException(minimumStock);
            if (!_context.Categories.Any(c => c.Id == categoryId))
                throw new CategoryNotFoundException(categoryId);
            if (GetProductByBarcode(barcode) != null)
                throw new ProductAlreadyExistsException(barcode);

            var product = new Product
                (
                name,
                barcode,
                manufacturer,
                price,
                minimumStock,
                categoryId
                );

            _context.Products.Add(product);
            _context.SaveChanges();

            return product;
        }

        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public Product? GetProductByBarcode(string barcode)
        {
            return _context.Products.FirstOrDefault(p => p.Barcode == barcode);
        }

        public Product? GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(p => p.Id == productId);
        }

        public void UpdateProduct(int id, string? name, string? barcode, string? manufacturer, decimal? price, int? minimumStock, int? categoryId)
        {
            var product = GetProductById(id);

            if (product == null)
                throw new ProductNotFoundException(id);

            if (categoryId.HasValue)
            {
                if (!_context.Categories.Any(c => c.Id == categoryId))
                    throw new CategoryNotFoundException(categoryId.Value);

                product.CategoryId = categoryId.Value;
            }

            if(barcode != null)
            {
                if (string.IsNullOrWhiteSpace(barcode))
                    throw new InvalidProductBarcodeException(barcode);

                var existingProduct = GetProductByBarcode(barcode);

                if (existingProduct != null && existingProduct.Id != id)
                    throw new ProductAlreadyExistsException(barcode);

                product.Barcode = barcode;
            }

            if (name != null && !string.IsNullOrWhiteSpace(name))
                product.Name = name;
            if (manufacturer != null && !string.IsNullOrWhiteSpace(manufacturer))
                product.Manufacturer = manufacturer;
            if (price.HasValue && price.Value > 0)
                product.Price = price.Value;
            if (minimumStock.HasValue && minimumStock.Value >= 0)
                product.MinimumStock = minimumStock.Value;

            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                throw new ProductNotFoundException(id);

            if (_context.Batches.Any(b => b.ProductId == id))
                throw new ProductHasBatchesException(id);

            _context.Products.Remove(product);
            _context.SaveChanges();
        }

    }
}
