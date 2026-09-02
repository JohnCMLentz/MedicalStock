namespace MedicalStock.Exceptions
{
    public class ProductHasBatchesException : DomainException
    {
        public ProductHasBatchesException(int productId)
            : base($"The product with ID '{productId}' has existing batches and cannot be deleted.")
        {
        }
    }
}
