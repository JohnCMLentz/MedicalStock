namespace MedicalStock.Exceptions
{
    public class DuplicateBarcodeException : DomainException
    {
        public DuplicateBarcodeException(string barcode)
            : base($"Product with barcode {barcode} already exists.")
        {
        }
    }
}
