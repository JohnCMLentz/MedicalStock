namespace MedicalStock.Exceptions
{
    public class InvalidProductNameException :DomainException
    {
        public InvalidProductNameException(string productName)
            : base($"The product name '{productName}' is invalid. It must not be empty or whitespace.")
        {
        }
    }
}
