namespace MedicalStock.Exceptions
{
    public class InvalidProductBarcodeException : DomainException
    {
        public InvalidProductBarcodeException(string productBarcode)
            : base($"The product barcode '{productBarcode}' is invalid. It must not be empty or whitespace.")
        {
        }
    }
}
