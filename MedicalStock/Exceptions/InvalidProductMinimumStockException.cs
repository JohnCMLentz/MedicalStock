namespace MedicalStock.Exceptions
{
    public class InvalidProductMinimumStockException : DomainException
    {
        public InvalidProductMinimumStockException(int productMinimumStock)
            : base($"The product minimum stock '{productMinimumStock}' is invalid. It must be zero or a positive value.")
        {
        }
    }
}
