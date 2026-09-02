namespace MedicalStock.Exceptions
{
    public class BatchNotFoundException : DomainException
    {
        public BatchNotFoundException(int batchId)
            : base($"Batch with ID {batchId} not found.")
        {
        }
    }
}
