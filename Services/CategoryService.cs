using MedicalStock.Data;
using MedicalStock.Models;

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
            var category = new Category
            {
                Name = name
            };

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

        public bool UpdateCategory(int id, string name)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return false;

            category.Name = name;

            _context.SaveChanges();

            return true;
        }

        public bool DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return false;
            
            _context.Categories.Remove(category);
            _context.SaveChanges();

            return true;
        }
    }
}
