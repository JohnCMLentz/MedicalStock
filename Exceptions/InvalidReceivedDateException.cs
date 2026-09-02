namespace MedicalStock.Exceptions
{
    public class InvalidReceivedDateException : DomainException
    {
        public InvalidReceivedDateException() : base("Received date cannot be in the future.")
        {
        }
    }
}
