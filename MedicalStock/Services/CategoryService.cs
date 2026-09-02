using MedicalStock.Data;
using MedicalStock.Models;
using MedicalStock.Exceptions;

namespace MedicalStock.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public void CreateCategory(string name)
        {
            var category = new Category(name);

            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void UpdateCategory(int id, string name)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                throw new CategoryNotFoundException(id);

            category.Name = name;

            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                throw new CategoryNotFoundException(id);

            if (_context.Products.Any(p => p.Id == id))
                throw new CategoryHasProductsException(id);

            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }
}
