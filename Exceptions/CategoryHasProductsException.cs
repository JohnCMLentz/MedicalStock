namespace MedicalStock.Exceptions
{
    public class CategoryHasProductsException : DomainException
    {
        public CategoryHasProductsException(int categoryId)
            : base($"Cannot delete category with ID {categoryId} because it has associated products.")
        {
        }
    }
}
