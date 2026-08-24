using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalStock.Models
{
    public class Batch
    {
        public int Id { get; private set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
