using System;
using System.Collections.Generic;
using System.Text;

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
        public Category Category { get; set; }
        public ICollection<Batch> Batches { get; } = new List<Batch>();

    }
}
