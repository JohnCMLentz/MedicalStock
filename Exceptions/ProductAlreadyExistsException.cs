namespace MedicalStock.Exceptions
{
    public class ProductAlreadyExistsException : DomainException
    {
        public ProductAlreadyExistsException(string productBarcode)
            : base($"The product with barcode '{productBarcode}' already exists.")
        {
        }
    }
}
