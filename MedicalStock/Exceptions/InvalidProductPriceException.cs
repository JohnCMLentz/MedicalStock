namespace MedicalStock.Exceptions
{
    public class InvalidProductPriceException : DomainException
    {
        public InvalidProductPriceException(decimal productPrice)
            : base($"The product price '{productPrice}' is invalid. It must be a positive value.")
        {
        }
    }
}
