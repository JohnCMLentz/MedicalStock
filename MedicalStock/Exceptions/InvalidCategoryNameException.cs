namespace MedicalStock.Exceptions
{
    public class InvalidCategoryNameException : DomainException
    {
        public InvalidCategoryNameException(string name)
            : base($"The category name: '{name}' is invalid. It must not be empty or whitespace.")
        {
        }
    }
}
