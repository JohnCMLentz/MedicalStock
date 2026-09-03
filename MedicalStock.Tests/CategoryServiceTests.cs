using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MedicalStock.Data;
using MedicalStock.Services;
using MedicalStock.Exceptions;

namespace MedicalStock.Tests;
public class CategoryServiceTests
{
    private (SqliteConnection connection, AppDbContext context) CreateTestContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);

        context.Database.EnsureCreated();

        return (connection, context);
    }

    [Fact]
    public void GetCategories_WhenCategoriesExist_ReturnsAllCategories()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Test Category1");
            categoryService.CreateCategory("Test Category2");
            categoryService.CreateCategory("Test Category3");

            // Act + Assert
            Assert.Equal(3, categoryService.GetCategories().Count);
        }

    }

    [Fact]
    public void GetCategories_WhenNoCategoriesExist_ReturnsEmptyCollection()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            // Act + Assert
            Assert.Empty(categoryService.GetCategories());
        }
    }

    [Fact]
    public void CreateCategory_ValidName_CreatesCategory()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);

            // Act
            categoryService.CreateCategory("Test Category");

            // Assert
            Assert.Contains(
                categoryService.GetCategories(),
                c => c.Name == "Test Category"
                );
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateCategory_InvalidName_ThrowsInvalidCategoryNameException(string name)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);

            // Act + Assert
            Assert.Throws<InvalidCategoryNameException>(() => categoryService.CreateCategory(name));
        }
    }

    [Fact]
    public void CreateCategory_DuplicateName_ThrowsCategoryAlreadyExistsException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);

            categoryService.CreateCategory("Test Category");

            // Act + Assert
            Assert.Throws<CategoryAlreadyExistsException>(() => categoryService.CreateCategory("Test Category"));
        }
    }

    [Fact]
    public void GetCategoryById_ExistingId_ReturnsCategory()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);

            // Act
            var category = categoryService.CreateCategory("Test Category");

            // Assert
            var foundCategory = categoryService.GetCategoryById(category.Id);
            Assert.Equal(category.Id, foundCategory.Id);
            Assert.Equal("Test Category", foundCategory.Name);
        }
    }

    [Fact]
    public void GetCategoryById_NonExistingId_ThrowsCategoryNotFoundException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);

            // Act + Assert
            Assert.Throws<CategoryNotFoundException>(() => categoryService.GetCategoryById(999));
        }
    }

    [Fact]
    public void UpdateCategory_ValidData_UpdatesCategory()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            var category = categoryService.CreateCategory("Old Name");

            // Act
            categoryService.UpdateCategory(category.Id, "New Name");

            // Assert
            var updatedCategory = categoryService.GetCategoryById(category.Id);
            Assert.Equal("New Name", updatedCategory.Name);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateCategory_InvalidName_ThrowsInvalidCategoryNameException(string name)
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            var category = categoryService.CreateCategory("Old Name");

            // Act + Assert
            Assert.Throws<InvalidCategoryNameException>(() => categoryService.UpdateCategory(category.Id, name));
        }
    }

    [Fact]
    public void UpdateCategory_SameName_DoesNotThrowException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            var category = categoryService.CreateCategory("Same Name");
            // Act + Assert
            var exception = Record.Exception(() => categoryService.UpdateCategory(category.Id, "Same Name"));
            Assert.Null(exception);
        }
    }

    [Fact]
    public void UpdateCategory_DuplicateName_ThrowsCategoryAlreadyExistsException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            categoryService.CreateCategory("Category 1");
            var category2 = categoryService.CreateCategory("Category 2");

            // Act + Assert
            Assert.Throws<CategoryAlreadyExistsException>(() => categoryService.UpdateCategory(category2.Id, "Category 1"));
        }
    }

    [Fact]
    public void UpdateCategory_NonExistingId_ThrowsCategoryNotFoundException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);

            // Act + Assert
            Assert.Throws<CategoryNotFoundException>(() => categoryService.UpdateCategory(999, "New Name"));
        }
    }

    [Fact]
    public void DeleteCategory_NonExistingId_ThrowsCategoryNotFoundException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);

            // Act + Assert
            Assert.Throws<CategoryNotFoundException>(() => categoryService.DeleteCategory(999));
        }
    }

    [Fact]
    public void DeleteCategory_ExistingCategoryWithoutProducts_DeletesCategory()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            var category = categoryService.CreateCategory("Test Category");

            // Act
            categoryService.DeleteCategory(category.Id);

            // Assert
            Assert.Throws<CategoryNotFoundException>(() => categoryService.GetCategoryById(category.Id));
        }
    }

    [Fact]
    public void DeleteCategory_CategoryWithProducts_ThrowsCategoryHasProductsException()
    {
        // Arrange
        var (connection, context) = CreateTestContext();
        using (connection)
        using (context)
        {
            var categoryService = new CategoryService(context);
            var productService = new ProductService(context);
            var category = categoryService.CreateCategory("Test Category");
            productService.CreateProduct("Test Product", "1234567891", "Manufacturer", 5.0m, 10, category.Id);

            // Act + Assert
            Assert.Throws<CategoryHasProductsException>(() => categoryService.DeleteCategory(category.Id));
        }
    }
}