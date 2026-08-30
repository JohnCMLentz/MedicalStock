using System.Globalization;

namespace MedicalStock.Models
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Batch>? Batches { get; } = new List<Batch>();

        public Product(string name, string barcode, string manufacturer, decimal price, int categoryId)
        {
            Name = name;
            Barcode = barcode;
            Manufacturer = manufacturer;
            Price = price;
            CategoryId = categoryId;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Barcode: {Barcode}, Manufacturer: {Manufacturer}, " +
                $"Price: {Price.ToString("F2",CultureInfo.InvariantCulture)}, CategoryId: {CategoryId};";
        }

    }
}
