namespace MedicalStock.Exceptions
{
    public class CategoryAlreadyExistsException : DomainException
    {
        public CategoryAlreadyExistsException(string name)
            : base($"Category with name '{name}' already exists.")
        {
        }
    }
}
