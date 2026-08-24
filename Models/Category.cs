using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalStock.Models
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; } = new List<Product>();

    }
}
