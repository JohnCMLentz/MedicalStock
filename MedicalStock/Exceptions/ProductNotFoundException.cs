namespace MedicalStock.Exceptions
{
    public  class ProductNotFoundException : DomainException
    {
        public ProductNotFoundException(int productId)
            : base($"Product with ID {productId} was not found.")
        {
        }
    }
}
