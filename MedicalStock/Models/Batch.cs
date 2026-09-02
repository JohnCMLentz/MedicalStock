namespace MedicalStock.Models
{
    public class Batch
    {
        public int Id { get; private set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ReceivedAt { get; set; }
        public ICollection<StockMovement>? StockMovements { get; } = new List<StockMovement>();

        public Batch() { }

        public Batch(int productId, int quantity, DateTime expirationDate, DateTime? receivedAt)
        {
            ProductId = productId;
            Quantity = quantity;
            ExpirationDate = expirationDate;
            if (receivedAt.HasValue)
                ReceivedAt = receivedAt.Value;
            else
                ReceivedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Batch ID: {Id}, Product ID: {ProductId}, Quantity: {Quantity}, " +
                $"Expiration Date: {ExpirationDate.ToString("yyyy/MM/dd")}, Received At: {ReceivedAt.ToString("yyyy/MM/dd")}";
        }
    }
}
