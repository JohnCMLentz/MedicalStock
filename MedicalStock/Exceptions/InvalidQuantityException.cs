namespace MedicalStock.Exceptions
{
    public class InvalidQuantityException : DomainException
    {
        public InvalidQuantityException()
            : base("Quantity must be greater than zero.")
        {
        }
    }
}
