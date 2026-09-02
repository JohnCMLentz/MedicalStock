namespace MedicalStock.Exceptions
{
    public class BatchHasMovementsException : DomainException
    {
        public BatchHasMovementsException(int batchId)
            : base($"Batch with ID {batchId} has associated movements and cannot be deleted.")
        {
        }
    }
}
