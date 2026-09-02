namespace MedicalStock.Exceptions
{
    public class NoExpiredStockException : DomainException
    {
        public NoExpiredStockException() : base("No expired stock available for disposal.")
        {
        }
    }
}
