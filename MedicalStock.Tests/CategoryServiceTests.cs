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
}