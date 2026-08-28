using MedicalStock.Data;
using MedicalStock.Models;

namespace MedicalStock.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;


        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateProduct(string name, string barcode, string manufacturer, decimal price, int categoryId)
        {
            if (ProductByBarcode(barcode) != null)
                return false;
            if (!_context.Categories.Any(c => c.Id == categoryId))
                return false;

            var product = new Product
                (
                name,
                barcode,
                manufacturer,
                price,
                categoryId
                );

            _context.Products.Add(product);
            _context.SaveChanges();

            return true;
        }

        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public Product? ProductByBarcode(string barcode)
        {
            return _context.Products.FirstOrDefault(p => p.Barcode == barcode);
        }

        public Product? ProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public bool UpdateProduct(int id, string? name, string? barcode, string? manufacturer, decimal? price, int? categoryId)
        {
            var product = ProductById(id);

            if (product == null) return false;

            if (categoryId.HasValue)
            {
                if (!_context.Categories.Any(c => c.Id == categoryId))
                    return false;

                product.CategoryId = categoryId.Value;
            }

            var existingProduct = ProductByBarcode(barcode);

            if (existingProduct != null && existingProduct.Id != id)
                return false;

            if (name != null) product.Name = name;
            if (barcode  != null) product.Barcode = barcode;
            if (manufacturer != null) product.Manufacturer = manufacturer;
            if (price.HasValue) product.Price = price.Value;

            _context.SaveChanges();

            return true;
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null) return false;

            _context.Products.Remove(product);
            _context.SaveChanges();

            return true;
        }

    }
}
