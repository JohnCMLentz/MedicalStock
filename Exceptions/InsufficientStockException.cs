namespace MedicalStock.Exceptions
{
    public class InsufficientStockException : DomainException
    {
        public InsufficientStockException(int productId, int requestedQuantity, int availableQuantity)
            : base($"Insufficient stock for product with ID {productId}. Requested: {requestedQuantity}, Available: {availableQuantity}")
        {
        }
    }
}
