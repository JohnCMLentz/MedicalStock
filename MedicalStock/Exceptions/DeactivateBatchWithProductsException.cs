
namespace MedicalStock.Exceptions
{
    public class DeactivateBatchWithProductsException : DomainException
    {
        public DeactivateBatchWithProductsException(int batchId)
            : base($"Cannot deactivate batch with ID {batchId} because it has products associated with it.")
        {
        }
    }
}
