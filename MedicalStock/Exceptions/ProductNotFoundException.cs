namespace MedicalStock.Exceptions
{
    public  class ProductNotFoundException : DomainException
    {
        public ProductNotFoundException(int productId)
            : base($"Product with ID {productId} was not found.")
        {
        }
        public ProductNotFoundException(string productBarcode)
            : base($"Product with Barcode {productBarcode} was not found.")
        {
        }
    }
}
