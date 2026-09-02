namespace MedicalStock.Exceptions
{
    public class InvalidProductManufacturerException : DomainException
    {
        public InvalidProductManufacturerException(string productManufacturer)
            : base($"The product manufacturer '{productManufacturer}' is invalid. It must not be empty or whitespace.")
        {
        }
    }
}
