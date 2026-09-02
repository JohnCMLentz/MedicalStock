namespace MedicalStock.Exceptions
{
    public class CategoryNotFoundException : DomainException
    {
        public CategoryNotFoundException(int categoryId)
            : base($"Category with ID {categoryId} not found.")
        {
        }
    }
}
