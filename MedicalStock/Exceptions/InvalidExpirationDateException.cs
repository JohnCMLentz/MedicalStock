namespace MedicalStock.Exceptions
{
    public class InvalidExpirationDateException : DomainException
    {
        public InvalidExpirationDateException(DateTime expirationDate)
            : base($"Invalid expiration date: {expirationDate}.")
        {
        }
    }
}
